Create procedure ILD_R_IRS_InventoryCounting (
	
	 in Period	varchar(100), -- ILD_LayPeriod_InventoryCounting
	 in SoLan int
	 /*
	 out tab_result table 
	(	
		RptName				nvarchar(100),
		DocNum				int,
		DocEntry			int,
		LineID				int,
		
		ItemCode			nvarchar(50),
		ItemName			nvarchar(100),
		UoM					nvarchar(100),
		CountedQty			numeric(19,6),
		Warehouse			nvarchar(8),
		Bin					nvarchar(50),
		SSCC				nvarchar(50),
		Batch				nvarchar(50),
		SystemQty			numeric(19,6),
		Deviation			numeric(19,6),
		Period				nvarchar(50),
		SolanDem			int,	
		
		Dec_CountedQty		int,
		Dec_SystemQty		int,
		Dec_Deviation		int,
		
		Round_CountedQty	int,
		Round_SystemQty		int,
		Round_Deviation		int,
		--
		CountedDate			date,
		CountedTime			varchar(50),
		Counted_Date_Time	varchar(50),
		CountingLocked_Date_Time varchar(50)--Lấy ngày giờ mở kỳ counting	
	)
*/
)
as
begin

	declare Todate date;
	select "T_RefDate" into Todate from  OFPR where "Code" = :Period; 

	create local temporary table #tbl (
		
		RptName				nvarchar(100),
		DocNum				int,
		DocEntry			int,
		LineID				int,
		
		ItemCode			nvarchar(50),
		ItemName			nvarchar(100),
		UoM					nvarchar(100),
		CountedQty			numeric(19,6),
		Warehouse			nvarchar(8),
		Bin					nvarchar(50),
		SSCC				nvarchar(50),
		Batch				nvarchar(50),
		SystemQty			numeric(19,6),
		Deviation			numeric(19,6),
		Period				nvarchar(20),
		N_of_INV_Counting	int,	
		
		Dec_CountedQty		int default 2,
		Dec_SystemQty		int default 2,
		Dec_Deviation		int default 2,
		
		Round_CountedQty	int default 2,
		Round_SystemQty		int default 2,
		Round_Deviation		int default 2,
		CountedDate			date,
		CountedTime			varchar(50),
		Counted_Date_Time	varchar(50),
		CountingLocked_Date_Time varchar(50)--Lấy ngày giờ mở kỳ counting		
	
	) ;
	
insert into #tbl (DocNum,DocEntry, SSCC, Batch,LineID, Counted_Date_Time, Period)
select a."DocNum",a."DocEntry", b."U_Batch" as "SSCC", b."U_MnfSerial", b."LineId",  
			TO_VARCHAR(a."CreateDate",'DDMMYYYY') || a."CreateTime", :Period
from "@INVENTORYCOUNTING_H" a
inner join "@INVENTORYCOUNTING_L" b on a."DocEntry"  = b."DocEntry"
where  cast(a."U_PeriodCode" as nvarchar) =  :Period
		and a."U_Slan" = :SoLan; 
		
		 
		
		
------------------------
SSCC_Trung= select SSCC, count (*)
from #tbl
group by  SSCC 
having count (SSCC)  >1;
--
create local temporary table #DocEntry_Trung
(
	DocNum	int,
	DocEntry int,
	SSCC varchar(50),
	LineID	int,
	Batch	nvarchar(50),
	Counted_Date_Time	varchar(50)
);

insert into #DocEntry_Trung(DocNum,DocEntry,SSCC, LineID, Batch, Counted_Date_Time)
Select a."DocNum",a."DocEntry", b."U_Batch" as "SSCC",  b."LineId",  b."U_MnfSerial", 
		TO_VARCHAR(a."CreateDate",'DDMMYYYY') || a."CreateTime"
				 from "@INVENTORYCOUNTING_H" a
				 inner join "@INVENTORYCOUNTING_L" b on a."DocEntry"  = b."DocEntry"
				 where b."U_Batch" in (select distinct "SSCC" from :SSCC_Trung)
				 		and a."U_Slan"  = :SoLan
				 		and  cast(a."U_PeriodCode" as nvarchar) =  :Period;


SSCC_Trung_CanGiuLai = select * from  #DocEntry_Trung a where DocEntry  in (select max (DocEntry) from  #DocEntry_Trung b where a.SSCC = b.SSCC );
--

delete 
from #tbl a where a.SSCC in (select "SSCC" from :SSCC_Trung_CanGiuLai);
--- 
insert into #tbl (DocNum,DocEntry, SSCC, LineID,Batch, Period)
select DocNum,DocEntry,SSCC,LineID,Batch, :Period
from :SSCC_Trung_CanGiuLai;
----
update a
set 
	--Period = :Period,
	ItemCode = b."U_ItemCode",
	ItemName = b."U_ItemName",
	UoM = d."InvntryUom",
	CountedQty = b."U_CQty",
	Warehouse = b."U_Whs",
	Bin  = b."U_Bin",
	CountedDate = c."CreateDate",
	CountedTime = LEFT(CASE 
				WHEN LENGTH(c."CreateTime") = 3 THEN CAST('0' || c."CreateTime" AS TIME) 
				WHEN LENGTH(c."CreateTime") = 2 THEN CAST('00' || c."CreateTime" AS TIME) 
				WHEN LENGTH(c."CreateTime") = 1 THEN CAST('000' || c."CreateTime" AS TIME) 
				ELSE CAST(c."CreateTime" AS TIME) END,5)
	
	 
from #tbl a
inner join "@INVENTORYCOUNTING_L" b on a.DocEntry||LineID||SSCC  = b."DocEntry"||b."LineId"||b."U_Batch"
inner join "@INVENTORYCOUNTING_H" c  on b."DocEntry"  = c."DocEntry"
inner join OITM d on b."U_ItemCode" = d."ItemCode"
where  c."U_Slan" = :SoLan and c."U_PeriodCode"  = :Period;
---  Qty in system sẽ lấy và fix giá trị tại thời điểm scan
Qty_System = select * from TBL_Counting_SysQty 
				where Period =  :Period
					  and SoLan  = :SoLan
					  and ifnull(Canceled,'')  <> 'Y';
--Thêm SSCC và Bin vào #tbl (hiện tại quét thiếu: Có thể tồn tại sscc A  với Bin A #tbl nhưng chưa có quét bin B_ SSCC A
insert into #tbl (ItemCode, ItemName, UoM, Bin, Warehouse, SSCC,Period )
select a.ITEMCODE, b."ItemName", b."InvntryUom", a.BIN, a.WHSCODE,a.SSCC, :Period
from :Qty_System a
inner join OITM b on a.ITEMCODE  = b."ItemCode"
where a.SSCC||a.BIN not in (select distinct SSCC||Bin from #tbl); 
--Update Số lượng Qty System cho các SSCC tồn tại ở UDO Counting
update a
set  a.SystemQty= b.QTY
from  #tbl a 
inner join :Qty_System b on a.SSCC  = b.SSCC;

---- Bổ sung thêm SSCC mà chưa được counting
insert into #tbl (ItemCode, ItemName, UoM, Bin, Warehouse, SSCC, SystemQty)
select  "ITEMCODE", "ITEMNAME","UOM","BIN", "WHSCODE","SSCC","QTY"
from :Qty_System  
 
where  "SSCC" not in (select distinct SSCC from #tbl);
---
	update a
	set Batch  =  b."MnfSerial"
	from #tbl a
	inner join OBTN b  on a.SSCC  = b."DistNumber"
	where ifnull(a.Batch,'') = '';
--


update a
set CountingLocked_Date_Time = (select TO_VARCHAR("CreateDate",'DD/MM/YYYY') || ' ' || 
										LEFT(CASE 
											WHEN LENGTH("CreateTime") = 3 THEN CAST('0' || "CreateTime" AS TIME) 
											WHEN LENGTH("CreateTime") = 2 THEN CAST('00' || "CreateTime" AS TIME) 
											WHEN LENGTH("CreateTime") = 1 THEN CAST('000' || "CreateTime" AS TIME) 
											ELSE CAST("CreateTime" AS TIME) END,5)   
							from "@COUNTINGLOCKED"
							where "U_Period"  =:Period
								   and "U_SoLan"  =:SoLan),
								   
	N_of_INV_Counting  = :SoLan,
	RptName = 'INVENTORY COUNTING'
	
from #tbl a;
 
/*
Qty_System =  select "ItemCode","BatchNum" as "SSCC", sum("InQty") - sum("OutQty") as "Qty"
				from 
				(
						select "ItemCode","BatchNum", 
								case when "BaseType" =  59 then "Quantity" else 0 end as  "InQty",
								case when "BaseType" =  60 then "Quantity" else 0 end as  "OutQty"
						from (
								select "ItemCode","BatchNum", "BaseType",sum("Quantity") as "Quantity"    from IBT1
								where "BaseType"  in (59, 60) and "DocDate" <= :Todate 
								group by "BaseType","ItemCode","BatchNum" ) as Nguon1 ) as Nguon2
						group by  "ItemCode","BatchNum";
*/
--
update #tbl
set Deviation = ifnull(SystemQty,0)  - ifnull(CountedQty,0);

--


 select RptName,
		
		ItemCode,
		ItemName,
		UoM,
		CountedQty,
		Warehouse,
		Bin,
		SSCC,
		Batch,
		SystemQty,
		Deviation,
		Period,
		N_of_INV_Counting,
		
		Dec_CountedQty,
		Dec_SystemQty,
		Dec_Deviation,
		
		Round_CountedQty,
		Round_SystemQty,
		Round_Deviation,
		--
		CountedDate,
		CountedTime,
		Counted_Date_Time,
		CountingLocked_Date_Time
		from  #tbl  order by SSCC;
	
drop table #tbl ;
drop table #DocEntry_Trung;
end

