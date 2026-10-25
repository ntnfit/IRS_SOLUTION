CREATE procedure ILD_R_InventoryCounting_Old (
	
	 in list_Period	table (Code Nvarchar(20)),
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
		CountedTime			varchar(50)
	)
)
as
begin

	declare Todate date;
	select "T_RefDate" into Todate from  OFPR where "Code" = (select cast(Code as nvarchar(20)) from :list_Period); 

	create local temporary table #tbl (
		
		RptName				nvarchar(100) default 'INVENTORY COUNTING',
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
		
		Dec_CountedQty		int default 2,
		Dec_SystemQty		int default 2,
		Dec_Deviation		int default 2,
		
		Round_CountedQty	int default 2,
		Round_SystemQty		int default 2,
		Round_Deviation		int default 2,
		CountedDate			date,
		CountedTime			varchar(50)		
	
	) ;
	
insert into #tbl (DocNum,DocEntry, SSCC, Batch,LineID)
select a."DocNum",a."DocEntry", b."U_Batch" as "SSCC", b."U_MnfSerial", b."LineId"
from "@INVENTORYCOUNTING_H" a
inner join "@INVENTORYCOUNTING_L" b on a."DocEntry"  = b."DocEntry"
where  cast(a."U_PeriodCode" as nvarchar) =   (select cast(Code as nvarchar(20)) from :list_Period); 
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
	Batch	nvarchar(50)
);

insert into #DocEntry_Trung(DocNum,DocEntry,SSCC, LineID, Batch)
Select a."DocNum",a."DocEntry", b."U_Batch" as "SSCC",  b."LineId",  b."U_MnfSerial"
				 from "@INVENTORYCOUNTING_H" a
				 inner join "@INVENTORYCOUNTING_L" b on a."DocEntry"  = b."DocEntry"
				 where b."U_Batch" in (select distinct "SSCC" from :SSCC_Trung);


SSCC_Trung_CanGiuLai = select * from  #DocEntry_Trung a where DocEntry  in (select max (DocEntry) from  #DocEntry_Trung b where a.SSCC = b.SSCC );
--

delete 
from #tbl a where a.SSCC in (select "SSCC" from :SSCC_Trung_CanGiuLai);
--- 
insert into #tbl (DocNum,DocEntry, SSCC, LineID,Batch)
select DocNum,DocEntry,SSCC,LineID,Batch
from :SSCC_Trung_CanGiuLai;
----
update a
set Period = c."U_PeriodCode",
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
inner join OITM d on b."U_ItemCode" = d."ItemCode";
---  Qty in system sẽ lấy và fix giá trị tại thời điểm scan

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

update  a
set a.SystemQty =  b."Qty"
from  #tbl  a
inner join  :Qty_System b on a.SSCC = b."SSCC";

--
update #tbl
set Deviation = ifnull(SystemQty,0)  - ifnull(CountedQty,0);
--


tab_result = select *
			from  #tbl  order by SSCC;
	
drop table #tbl ;
drop table #DocEntry_Trung;
end

