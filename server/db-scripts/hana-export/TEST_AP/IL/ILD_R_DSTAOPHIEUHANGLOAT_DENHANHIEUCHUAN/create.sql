CREATE procedure ILD_R_DSTaoPhieuHangLoat_DenHanHieuChuan
(
	in FromDate date,
	in ToDate date,
   	in CreatedByCode nvarchar(10), -- store ILD_LayDanhSachNhanVien_HieuChuan
	in ItemCode nvarchar(100)-- store ILD_LayItemHieuChuan
)
as
begin
-- Lay nhung item co danh dau la Hieuchuan va active  = Y
	create local temporary table #tbl1 -- Calibration: C
	(
		ItemCode nvarchar(50),
		ItemName nvarchar(150),
		StartDate date,
		Interval smallint,
		EndDate date,
		NextDate date,
		CreatedByCode nvarchar(10),
		NominalInput nvarchar(50)
		
	);
insert into  #tbl1(  
					ItemCode,
					ItemName,
					StartDate,
					Interval,--Sẽ lấy IntervalC (days):  số ngày tần suất hiệu chuẩn ở trong item master data
					EndDate,
					NextDate,
					CreatedByCode,
					NominalInput
					)
								
select "ItemCode",
		"ItemName",
		"U_StartDateC",
		"U_IntervalC",
		ADD_DAYS("U_StartDateC",ifnull("U_IntervalC",0)) as  "EndDate",
		ADD_DAYS(ADD_DAYS("U_StartDateC",ifnull("U_IntervalC",0)),ifnull("U_IntervalC",0))as  "NextDate",
		:CreatedByCode,
		a."U_NominalInput"
from OITM  a
--where   a."ItemCode" in ('2110100001','1528100001');
where ifnull("U_IsNeedCheckC",'') = 'Y';
---- 
create local temporary table #item_dataophieu
(
	ItemCode nvarchar(50),
	StartDate date, -- chinh la StartDate trong item master date
	IntervalC	smallint,
	EndDate	date,
	NextDate date
		
);
insert into #item_dataophieu (ItemCode, StartDate,IntervalC,EndDate,NextDate)
select "U_ItemCode","U_StartDate", "U_IntervalC" ,"U_EndDate","U_NextDate"
from "@CALIBRATIONS_H" a
where "U_StartDate" = (select max("U_StartDate") from "@CALIBRATIONS_H" b where a."U_ItemCode" = b."U_ItemCode");
---
update a
set a.Interval = b.IntervalC
from  #tbl1 a
inner join  #item_dataophieu b on a.ItemCode = b.ItemCode;
---
Update a
set a.StartDate  = ifnull(b.NextDate,ADD_DAYS (b.EndDate,ifnull(a.Interval,0))),
	a.EndDate  =   ADD_DAYS( ifnull(b.NextDate,ADD_DAYS (b.EndDate,ifnull(a.Interval,0))),ifnull(a.Interval,0)),  -- StartDate+ Interval
	a.NextDate  =  ADD_DAYS (ADD_DAYS( ifnull(b.NextDate,ADD_DAYS (b.EndDate,ifnull(a.Interval,0))),ifnull(a.Interval,0))
							,ifnull(a.Interval,0))
from #tbl1 a
inner join  #item_dataophieu b on a.ItemCode = b.ItemCode;

select  ItemCode,
	    ItemName,
		StartDate,
		Interval,
		EndDate,
		NextDate,
		CreatedByCode,
		NominalInput

from #tbl1 a
--where a.ItemCode in ('2110100001','1528100001')

where   cast(a.StartDate as date) between  Fromdate and  ToDate
		and
		(
				 ifnull(:ItemCode, '') = '' 
				 or cast (a.ItemCode as nvarchar) in (select "DATA" from split(:ItemCode,','))
		)

order by NextDate;

drop table #tbl1;
drop table #item_dataophieu;

end;

