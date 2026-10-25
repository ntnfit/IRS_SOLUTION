CREATE procedure USP_PROD_COSTING_STEP1(
	in period 	nvarchar(20)
	
)
as
Begin
declare fdate,tdate date;
declare ProcessingFee_Actual_JE, Total_CmpltQty decimal (28,10);
declare pro09 table
(
	"CmpltQty"  decimal (28,10),
	"AmountFG"  decimal (28,10),
	"Amount" decimal (28,10),
	"SdtAmount" decimal (28,10),
	"IssuedQty" decimal (28,10),
	"FGCode" nvarchar(50),
	"ItmsGrpCod_FGCode" int,
	
	"ItmsGrpCod" int,
	"ItemCode" nvarchar(50)
	
);

declare ItemGiaCong table
(	
	"CmpltQty"   decimal (28,10),
	"AmountFG"  decimal (28,10),
	"WIP_GB_Code"  nvarchar(50),
	"Amount" decimal (28,10),
	"SdtAmount" decimal (28,10),
	"IssuedQty" decimal (28,10),
	"FGCode" nvarchar(50),
	"ItmsGrpCod" int
);
declare ItemFG table 
(
	"FGCode" nvarchar(50),
	"ItemCode" nvarchar(50)
	
);
declare tbl table
(
	"STT" int,
	"FGCode" nvarchar(50),
	"FGName" nvarchar(150),
	"WIP_GB_Code"  nvarchar(50),
	"CmpltQty"  decimal (28,10),
	"AmountFG"  decimal (28,10),
	"RawMaterials_Pr09" decimal (28,10),
	"ProcessingFee_Pr09" decimal (28,10),
	"StdUnitCost" decimal (28,10),
	"RawMaterials_Actual"  decimal (28,10),
	"ProcessingFee_Actual"  decimal (28,10),
	
	"Total_Mnf_Cost" decimal (28,10),
	"UnitCost" decimal (28,10), 
	
	"Variance" decimal (28,10),
	"EBQty" decimal (28,10),
	"CONSUMED_QTY" decimal (28,10),
	"ALLOCTOSTOCK" decimal (28,10),
	"Alloc_Consumption" decimal (28,10)
);
select "F_RefDate", "T_RefDate" into fdate,tdate from "OFPR" where "Code"=period;
 		CALL ILD_R_IRS_LenhSanXuatDaNhapThanhPham(fdate,tdate,'Y','N','','', datapro09);
		--
		insert into :pro09 ("CmpltQty","AmountFG","Amount", "SdtAmount", "IssuedQty", "FGCode", "ItmsGrpCod", "ItemCode")
		select sum("CmpltQty"),  sum("AmountFG"),sum("Amount") as "Amount",sum("PlanCompltAmount") as "SdtAmount",
				 sum("IssuedQty") as "IssuedQty", "FGCode", "ItmsGrpCod", "ItemCode"
		from :datapro09 
		where "ItmsGrpCod"   IN (113,114,121,127,233,253,254,126,120)
		group by "FGCode","ItemCode", "ItmsGrpCod" ;
		--
		update a
		set  a."ItmsGrpCod_FGCode"  = b."ItmsGrpCod"
		from :pro09 a
		inner join OITM b on a."FGCode"  = b."ItemCode";
		--
Insert into :ItemGiaCong  ("WIP_GB_Code", "CmpltQty" ,"AmountFG","Amount","SdtAmount", "IssuedQty", "FGCode", 	"ItmsGrpCod")
select  "FGCode", "CmpltQty" ,"AmountFG","Amount","SdtAmount", "IssuedQty", "ItemCode", 	"ItmsGrpCod" 
from :pro09
where  "ItmsGrpCod_FGCode" =  253 ;	
--

--
insert into :ItemFG ( "FGCode",  "ItemCode")
select  b."FGCode" , a."WIP_GB_Code"
from :ItemGiaCong a
left join (select  distinct "FGCode" , "ItemCode"  from	:pro09 ) b  on b."ItemCode"  = a."WIP_GB_Code"
where   ifnull(a."AmountFG",0) >0;
 
--
insert into  :tbl ("STT","FGCode", "WIP_GB_Code","FGName")
select  ROW_NUMBER() OVER (PARTITION BY a."ItemCode"  ORDER BY a."FGCode") , a."FGCode",  a."ItemCode", b."ItemName"
from :ItemFG a
left join OITM b on a."ItemCode"  = b."ItemCode";
--
--
update a
set a."CmpltQty" =  (select sum (b."CmpltQty") from  :ItemGiaCong b where a."WIP_GB_Code" = b."WIP_GB_Code")
from  :tbl a;
--
update a
set a."AmountFG" =  (select sum (b."AmountFG") from  :ItemGiaCong b where a."WIP_GB_Code" = b."WIP_GB_Code")
from  :tbl a;
--
update a
set a."RawMaterials_Pr09" =  (select sum (b."Amount") from  :ItemGiaCong b 
									where b."ItmsGrpCod"  = 113 and a."WIP_GB_Code" = b."WIP_GB_Code")
from  :tbl a;
--
update a
set a."ProcessingFee_Pr09" =  (select sum (b."Amount") from  :ItemGiaCong b 
									where b."ItmsGrpCod"  = 254 and a."WIP_GB_Code" = b."WIP_GB_Code")
from  :tbl a;
--
update a
set "StdUnitCost" = (ifnull("RawMaterials_Pr09" ,0) +  ifnull( "ProcessingFee_Pr09",0)) / NULLIF ("CmpltQty", null)
from  :tbl a; 
-- 
update :tbl set  "RawMaterials_Actual"  =  ifnull("RawMaterials_Pr09",0);
--======================= Actual Processing fee
select  sum("Debit"-"Credit")  into ProcessingFee_Actual_JE default NULL
from "JDT1"
where ("Account" like '154502' or "Account" like '154503' ) and "RefDate" between fdate and tdate;
--
update a
set  "CmpltQty" = NULL, "AmountFG"  =  NULL, "RawMaterials_Pr09" = null,
		"ProcessingFee_Pr09" = null,"StdUnitCost" = null,"RawMaterials_Actual" = null

from :tbl  a
where "STT" >1;
--
select sum("CmpltQty") into Total_CmpltQty default  NULL from :tbl;
--	
IF (IFNULL(:ProcessingFee_Actual_JE,0)  <> 0 AND IFNULL(:Total_CmpltQty,0)  <> 0) THEN
	update :tbl set  "ProcessingFee_Actual"  = case when "STT" = 1 then 
													round(ifnull("CmpltQty",0)/ NULLIF(:Total_CmpltQty,0) * NULLIF(ProcessingFee_Actual_JE,NULL),2)
													else Null end;
													
													
	-- Xu ly chenh lech
	update a
	set "ProcessingFee_Actual" = a."ProcessingFee_Actual" +  (:ProcessingFee_Actual_JE  - (select sum(b."ProcessingFee_Actual") from :tbl b))
	from :tbl a
	where a."CmpltQty"  = (select max("CmpltQty") from   :tbl c);
END IF;
--
update :tbl set "Total_Mnf_Cost"  =  ifnull("ProcessingFee_Actual",0)  + ifnull( "RawMaterials_Actual",0);
--
update :tbl set "UnitCost"  =  ifnull("Total_Mnf_Cost",0)/ NULLIF( "CmpltQty",NULL);
--
update :tbl set "Variance"  =  ifnull("Total_Mnf_Cost",0) - ifnull("AmountFG",0);
-- Consumed Qty_Processing
 
update a
set a."CONSUMED_QTY" =  (select sum (b."IssuedQty") from  :pro09 b 
									where  b."ItemCode" = a."WIP_GB_Code" and b."FGCode" = a."FGCode" )
from  :tbl a;
--
/* OLD
update :tbl set "EBQty"  =  ifnull("CmpltQty",0) - ifnull("CONSUMED_QTY",0) where  ifnull("CmpltQty",0) >0;


*/
update a 
set a."EBQty"  = (select  sum("OnHandQty")     from OBBQ c
					where   c."OnHandQty"  > 0 and a."WIP_GB_Code" = c."ItemCode"
					
					group by  c."ItemCode"
 )
from  :tbl a
where "STT"  = 1;
--
update :tbl set "ALLOCTOSTOCK"  = ifnull("EBQty",0) /NULLIF("CmpltQty",NULL)  *  ifnull("Variance",0);
--
update a set "Alloc_Consumption"  = ifnull(a."CONSUMED_QTY",0) / 
					NULLIF((select sum(b."CmpltQty") from :tbl b where  b."WIP_GB_Code"  = a."WIP_GB_Code" ),NULL) 
					 * IFNULL((select sum(c."Variance") from :tbl c where  c."WIP_GB_Code"  = a."WIP_GB_Code" ),0) 
from :tbl a;
 
--  Làm tròn
UPDATE :tbl
SET 
    "CmpltQty"          = ROUND("CmpltQty", 2),
    "AmountFG"          = ROUND("AmountFG", 2),
    "RawMaterials_Pr09" = ROUND("RawMaterials_Pr09", 2),
    "ProcessingFee_Pr09"= ROUND("ProcessingFee_Pr09", 2),
    "StdUnitCost"       = ROUND("StdUnitCost", 2),
    "ProcessingFee_Actual"  = cast ("ProcessingFee_Actual" as decimal(28,2)),
    "RawMaterials_Actual"   = ROUND("RawMaterials_Actual", 2),
    "Total_Mnf_Cost"    = ROUND("Total_Mnf_Cost", 2),
    "UnitCost"          = ROUND("UnitCost", 2),
    "Variance"          = ROUND("Variance", 2),
    "EBQty"             = ROUND("EBQty", 2),
    "CONSUMED_QTY"      = ROUND("CONSUMED_QTY", 2),
    "ALLOCTOSTOCK"      = ROUND("ALLOCTOSTOCK", 2),
    "Alloc_Consumption" = ROUND("Alloc_Consumption", 2);
--
delete from "TBL_COSTING_TOTAL_STEP1" where "PERCODE" =   :period;
--
insert into  "TBL_COSTING_TOTAL_STEP1"  (Type, PERCODE, GRPID, GRPNAME, FGCode, FGName,
											WIP_GB_CODE, FGCmpltQty, FGCmpltAmount, 
											
											RawMaterials_Pr09, ProcessingFee_Pr09, StdUnitCost, 
											RawMaterials_Actual, ProcessingFee_Actual, 
											
											Total_Mnf_Cost, UnitCost, Variance, 
											EBQty, CONSUMED_QTY, ALLOCTOSTOCK, Alloc_Consumption)

select 'Semi Goods',:period,253,'Nguyên liệu, vật liệu chính đã qua gia công',"FGCode","FGName",
	   "WIP_GB_Code","CmpltQty","AmountFG",
	   
	   "RawMaterials_Pr09" ,"ProcessingFee_Pr09","StdUnitCost",
	   "RawMaterials_Actual","ProcessingFee_Actual",
	   
	   "Total_Mnf_Cost","UnitCost","Variance",
	   "EBQty","CONSUMED_QTY","ALLOCTOSTOCK","Alloc_Consumption" 
from :tbl;

													
													
End
