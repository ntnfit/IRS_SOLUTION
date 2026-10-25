-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:PT:PROCESS_END

--alter session set current_schema=SBODEMOUS;
--drop procedure TmSp_IVIInventoryValueDiffsFIFOItems;
Create procedure TmSp_IVIInventoryValueDiffsFIFOItems(out retCode INT)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as
	Perpetual_Inventory nvarchar(1);
	isPricePerWH nvarchar(1);
	PriceDec TinyInt;
	roundmthd nvarchar(1);
	maincurrency char(3);
	sumdec int;
	deciamls int;
	rowcount int;
begin
	select "ContInvnt" into Perpetual_Inventory from "OADM";
	select "RoundMthd" into roundmthd from "OADM";
	select "MainCurncy" into maincurrency from "OADM";
	select "SumDec" into sumdec from "OADM";
	select "Decimals" into deciamls from "OCRN" where "CurrCode" = :maincurrency;
	Select T1."PriceSys" INTO isPricePerWH From OADM T1;

if :roundmthd = 'Y' and :deciamls <> -1
then      
	sumdec := :deciamls;
end if;

	delete from "TmSp_IVIInventoryValueDiffsFIFOItems_TMP";
 
if :Perpetual_Inventory = 'Y' and :isPricePerWH = 'Y' then


INSERT INTO "TmSp_IVIInventoryValueDiffsFIFOItems_TMP"
Select W."ItemCode" as "ItemCode", M."ItemName" as "ItemName", M."EvalSystem" as "InvnetoryMethod", W."WhsCode" as "WarehouseCode",
      W."OnHand" as "QtyInWH", W."AvgPrice", SUM (IFNULL(V."InQty",0) - IFNULL(V."OutQty",0)) AS "QtyInUIVL", W."StockValue",
	case when W."OnHand" < 0 then round(W."OnHand"*W."AvgPrice",:sumdec)  else '0' end as "Qty*StaticPrice",
	SUM(V1."TransValue")as "Sum_TransValue", Q."OpenValue" as "Sum_OpenValue"
From "UITW" W INNER join "UITM" M on W."ItemCode" = M."ItemCode" INNER join "UIVL" V on W."ItemCode" = V."ItemCode" and W."WhsCode" = V."LocCode"
INNER join "UIVL1" V1 on V."TransSeq" = V1."TransSeq" INNER join

 (select "ItemCode","LocCode",sum("OpenValue") "OpenValue" from "UIVQ" group by "ItemCode","LocCode" )Q on Q."ItemCode" = V."ItemCode" and Q."LocCode" = V."LocCode"

Group by 

W."ItemCode",M."ItemCode", V."ItemCode", Q."ItemCode", M."ItemName", M."EvalSystem", W."WhsCode", V."LocCode",Q."LocCode", W."OnHand", W."AvgPrice",W."StockValue",Q."OpenValue"
having M."EvalSystem" = 'F' and 

W."StockValue" <> sum(V1."TransValue") or  
M."EvalSystem" <> 'B' AND abs((Case when W."OnHand"  = 0 
Then '0'
else
case when W."OnHand"  < 0 
Then W."StockValue"
Else
Q."OpenValue" end end) - W."StockValue")  > 1 or 
 M."EvalSystem" = 'B' AND Q."OpenValue" <> 0;
end if;



IF :Perpetual_Inventory = 'Y' and :isPricePerWH = 'N' THEN

insert into "TmSp_IVIInventoryValueDiffsFIFOItems_TMP"
Select M."ItemCode" as "ItemCode", M."ItemName" as "ItemName", M."EvalSystem" as "InvnetoryMethod",

		'',

		M."OnHand" as "QtyInWH",

		M."AvgPrice",
				
		SUM (IFNULL(V."InQty",0) - IFNULL(V."OutQty",0)) AS "QtyInUIVL",

		 

		M."StockValue",

		case when M."OnHand" < 0

		then round(M."OnHand"*M."AvgPrice",:sumdec) 

		else

		'0'

		end as "Qty*StaticPrice",

		SUM(V1."TransValue")as "Sum_TransValue",

		Q."OpenValue" as "Sum_OpenValue"

From 

      "UITM" M INNER join "UIVL" V on M."ItemCode" = V."ItemCode" INNER join "UIVL1" V1 on V."TransSeq" = V1."TransSeq" 

      INNER join (select "ItemCode",sum("OpenValue") "OpenValue" from UIVQ group by "ItemCode" )Q on Q."ItemCode" = V."ItemCode" 

Group by 

      M."ItemCode",V."ItemCode",Q."ItemCode",M."ItemName",M."EvalSystem",M."OnHand",M."AvgPrice",M."StockValue",Q."OpenValue"

having 

      M."EvalSystem" = 'F' and 

      M."StockValue" <> sum(V1."TransValue") or 
 
      M."EvalSystem" <> 'B' and abs((Case when M."OnHand"  = 0 
      Then '0'
	  else
	  case when M."OnHand"  < 0 
      Then M."StockValue"
	  Else
	  Q."OpenValue" end end) - M."StockValue") > 1 or
     M."EvalSystem" = 'B' AND Q."OpenValue" <> 0 ;

end IF;
select count(1) into rowcount from "TmSp_IVIInventoryValueDiffsFIFOItems_TMP";
delete from "TmSp_IVIInventoryValueDiffsFIFOItems_TMP";
if(:rowcount > 0) 
then
retCode := -1;
return;

--Return -1

else
--/*Inventory Value recalculation for FIFO items is succesfull*/;
retCode := 0;
return;
--Return 0
end if;
end;


