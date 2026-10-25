CREATE procedure ILD_R_IRS_BaoTriBaoDuong_KiemDinhChiTiet
(
	in FromDate date,
	in ToDate date,
	in PlanNo nvarchar(1000),-- store ILD_LayPlanNo
	in TasklistNo nvarchar(1000),-- store ILD_LayTaskList
	in TaiSanCode nvarchar(1000),-- store ILD_LayItemTaiSan,
	in Floc nvarchar(1000),-- store ILD_LayFlocCumChucNang
	in ActivityType nvarchar(1000), -- store ILD_LayActivityType
	in DocType nvarchar(50),
	in Completed	char(1) -- N: Chưa hoàn thành --> Chưa có ngày CompletionDate
							-- Y: Đã hoàn thành   --> Có ngày CompletionDate
)
as
begin
create local temporary table #tbl
(
	PlanNo nvarchar(50),
	TasklistNo nvarchar(50),
	FixAssetCode nvarchar(50),
	FixAssetName nvarchar(100),
	Floc nvarchar(50),
	ActivityType nvarchar(50),
	TasklistName nvarchar(50),
	--Detail
	ItemCode nvarchar(50),
	ItemName nvarchar(100),
	Quantity decimal(21,6),
	UOM nvarchar(20),
	Price decimal(21,6),
	Amount decimal(21,6),		 
		 --
	CreateDate nvarchar(50),
	CreatedByName nvarchar(50),
	UpdateDate nvarchar(50),
	ChangedByName nvarchar(50),
	PWO nvarchar(50),-- Previous Work Order
	Notes nvarchar(50),
	Remark nvarchar(50),
	CompletedByName nvarchar(50),
	CompletionDate nvarchar(50),
	Creator nvarchar(50),
	Lvl1 nvarchar(50),
	Lvl2 nvarchar(250),
	Lvl3 nvarchar(250),
	StartDate date,
	Interval smallint,
	EndDate date,
	NextDate date

);
	insert into #tbl (	 PlanNo,
					     TasklistNo,
					     FixAssetCode,
					     FixAssetName,
					     Floc,
					     ActivityType,
					     TasklistName,
					     --Detail
					     ItemCode,
						 ItemName,
						 Quantity,
						 UOM,
						 Price,
						 Amount,
						 --
					     CreateDate,
					     CreatedByName,
					     UpdateDate,
					     ChangedByName,
					     PWO,-- Previous Work Order
					     Notes,
					     Remark,
					     CompletedByName,
					     CompletionDate,
					     Creator,
					     StartDate,
					     Interval,
					     EndDate,
					     NextDate)

 
	select
	     "U_PlanNo",
	     "U_TasklistNo",
	     "U_ItemCodeF",
	     "U_ItemNameF",
	     a."U_Floc",
	     "U_ActivityType",
	     c."Name" as "U_TasklistName",
	     --Detail
	     b."U_ItemCode",
		 b."U_ItemName",
		 b."U_Quantity",
		 b."U_UOM",
		 b."U_Price",
		 b."U_Amount",
		--
	     "CreateDate",
	     "U_CreatedByName",
	     "UpdateDate",
	     "U_ChangedByName",
	     "U_PWO",-- Previous Work Order
	     a."U_Notes",
	     "Remark",
	     "U_CompletedByName",
	     "U_CompletionDate",
	     "Creator",
	     a."U_StartDate",
		 a."U_Interval",
		 a."U_EndDate",
		 a."U_NextDate"
from "@MAINTENANCE_H" a
inner join "@MAINTENANCE_L" b on a."DocEntry" = b."DocEntry"
left join "@TASKLISTTEMPLATE" c on a."U_TasklistNo"  = c."Code"
where a."U_StartDate" between :FromDate and :ToDate
	  and
		(
				 ifnull(:PlanNo, '') = ''  or :DocType  = 'ALL' 
				 or cast (a."U_PlanNo" as nvarchar) in (select "DATA" from split(:PlanNo,','))
		)
		and
		(
				 ifnull(:TasklistNo, '') = '' 
				 or cast (a."U_TasklistNo" as nvarchar) in (select "DATA" from split(:TasklistNo,','))
		)
		and
		(
				 ifnull(:TaiSanCode, '') = '' 
				 or cast (a."U_ItemCodeF" as nvarchar) in (select "DATA" from split(:TaiSanCode,','))
		)
		and
		(
				 ifnull(:Floc, '') = '' 
				 or cast (a."U_Floc" as nvarchar) in (select "DATA" from split(:Floc,','))
		)
		and
		(
				 ifnull(:ActivityType, '') = '' 
				 or cast (a."U_ActivityType" as nvarchar) in (select "DATA" from split(:ActivityType,','))
		);

--- update 3 lvel: --> LV1: Nhà máy; LV2: Hệ thống; LV3: Quy trình
update a
set Lvl1   = Asset_Lvel."Lvl1",
	Lvl2   = Asset_Lvel."Lvl2",
	Lvl3   = Asset_Lvel."Lvl3"

from #tbl a 
inner join 	( select a."ItemCode" as "FixAssetCode",
					 a."U_Level1" as "Lvl1",
					 a."U_Level2" as "Lvl2",
					 a."U_Level3" as "Lvl3",
					 
					c."U_ItemCode" as "ItemCode"
			from OITM a 
			inner join "@BOMAINTENANCE_H" b on a."ItemCode"  =  b."U_ItemCodeF"
			inner join  "@BOMAINTENANCE_L" c on b."DocEntry" = c."DocEntry"
			where   a."ItemType"  =  'F'
			and (a."U_IsNeedCheckM" = 'Y' or  a."U_IsNeedCheckHRE" = 'Y' )) as Asset_Lvel
on a.FIXASSETCODE = Asset_Lvel."FixAssetCode" and a.ITEMCODE =  Asset_Lvel."ItemCode";
--
select * 
from #tbl 
where  ifnull(CompletionDate,'N') =  (case  when :Completed = 'N' then 'N' else CompletionDate end )
order by  CreateDate , FixAssetCode ;
drop table #tbl;
end;

