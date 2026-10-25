CREATE procedure ILD_F_PhieuInventoryTransfer
(
	DocEntry int
)
as 
Begin
Create local temporary table #tbl
(
	DocNum nvarchar(50),
	DocDate date,
	DocTime nvarchar(20),
	
	ItemCode nvarchar(50),
	ItemName nvarchar(150),
	ItemGroup nvarchar(10),
	FromWhs  nvarchar(50),
	ToWhs	 nvarchar(50),
	BinLocation nvarchar(50),
	Unit_Storage nvarchar(15),
	UOM nvarchar(15),
	Qty decimal(19,6),
	Batch nvarchar(50),
	SSCC nvarchar(50),
	QtyInSSCC decimal(19,6),
	ComplianceFEFO nvarchar(20),
	Appearance  nvarchar(20),
	CreateBy nvarchar(50),
	RptName nvarchar(50),
	
		--format number
	AmountDec		smallint,
	QtyDec			smallint,
	PriceDec		smallint,
	RateDec			smallint,
	PercentDec		smallint,
	DecSep			nvarchar(1),
	ThousSep		nvarchar(1)
	
);
insert into #tbl (DocNum,DocDate, DocTime,
					ItemCode,
					ItemName,
					ItemGroup,
					FromWhs,ToWhs,
					BinLocation,Unit_Storage,UOM,
					Qty,
					Batch,SSCC,
					ComplianceFEFO,
					Appearance				
					)

	select distinct a."AppDocNum" as "DocNum", a."CreateDate",
	
			        case when LENGTH(a."CreateTime")  = 3 then  '0'||left(a."CreateTime",1) || ':' || right(a."CreateTime",2) 
						 when LENGTH(a."CreateTime")  = 4 then   left(a."CreateTime",2) || ':' || right(a."CreateTime",2)  else  '' end
						 as "DocTime" ,
		
			d."ItemCode",
			a."ItemName" as "ItemName",
			f."ItmsGrpCod" as "ItemGroup",
			--d."FromWhsCod" as "FromWhs", d."WhsCode" as "ToWhs",
			fromwhs."WhsName" as "FromWhs",
			towhs."WhsName" as "ToWhs",
			
			e."BinCode",
			f."InvntryUom" as "StorageUnit",
			'' as "UoM",
			
			 b."Quantity",
			 c."MnfSerial" as "Batch",
			 c."DistNumber" as "SSCC",

			'' as "ComplianceFEFO",
			'' as "Appearance"
			 
			
			
	from OITL a -- Inventory Transactions Log
	inner join  OBTL b on  a."LogEntry" = b."ITLEntry" -- Bin Transaction Log
	inner join  OBTN c on b."SnBMDAbs"  = c."AbsEntry" -- Batch Numbers Master Data
	inner join  WTR1 d on d."ItemCode"  = a."ItemCode" and d."DocEntry" = :DocEntry
	inner join  OBIN e on e."AbsEntry" = b."BinAbs" -- Bin
	inner join 	OITM f on f."ItemCode"  = d."ItemCode"
	inner join 	OWHS fromwhs on fromwhs."WhsCode" =  d."FromWhsCod"
	inner join 	OWHS towhs on towhs."WhsCode" =  d."WhsCode"

	where a."DocType" =  67 
		and a."DocEntry" = :DocEntry and a."BaseType"  =  -1
		and a."DocQty">0 ;
----
Update #tbl
set  CreateBy  = 
				(select ifnull("lastName",'') || ' ' ||
				
				case when  ifnull("middleName",'')  <> '' then "middleName" || ' ' || "firstName" else ' ' ||"firstName" end 
				from OHEM 
				where "userId"  = (select "UserSign" from OWTR where "DocEntry"  = :DocEntry ));
---
	update x set 
			AmountDec	= y."SumDec",
			QtyDec		= 0,
			PriceDec	= y."PriceDec",
			RateDec		= 0,
			PercentDec	= y."PercentDec",
			DecSep		= y."DecSep",
			ThousSep	= y."ThousSep",
			RptName = N'INVENTORY TRANSFER'
	from #tbl x inner join OADM y on 1=1;
---
update a
set QtyInSSCC  =  (select sum("Quantity") from IBT1 
					where "BaseType" = (case when a.ItemGroup in (113,114) then 20 
											when a.ItemGroup in (120,121) then 59 end) and "BatchNum" = a.SSCC  )
from #tbl a;

 
--
select distinct * from #tbl;

drop table #tbl;
End;

