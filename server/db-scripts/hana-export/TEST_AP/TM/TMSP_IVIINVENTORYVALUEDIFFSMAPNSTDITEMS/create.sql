-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:PT:PROCESS_END

--alter session set current_schema=SBODEMOUS;
--drop procedure TmSp_IVIInventoryValueDiffsMAPnSTDItems;
Create procedure TmSp_IVIInventoryValueDiffsMAPnSTDItems(OUT retCode INT)
-- Return Codes
--(-1) = Empty Parameter Was Entered
--(-2)  --Enpty or invalid Line set
--(-3) -- Invalid Currency or Bad Account code
--(-4) -- Invalid Card Currency or Bad Account code

LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as


isPricePerWH nvarchar(1);
PriceDec TinyInt;

roundmthd nvarchar(1);

maincurrency char(3);

sumdec int;

deciamls int;
rowcount int;

begin


select "RoundMthd" into roundmthd from "OADM";

select "MainCurncy" into maincurrency from "OADM";

select "SumDec" into sumdec from "OADM";

select "Decimals" into deciamls from "OCRN" where "CurrCode" = :maincurrency;

Select T1."PriceSys" into isPricePerWH From "OADM" T1; 

delete from "TmSp_IVIInventoryValueDiffsMAPnSTDItems_tmp";


if :roundmthd = 'Y' and :deciamls <> -1

then

      

     sumdec := :deciamls;

end if;

 if :isPricePerWH = 'Y'

 

then


insert into "TmSp_IVIInventoryValueDiffsMAPnSTDItems_tmp"
select Top 1
      W."ItemCode" as "ItemCode", 

      M."ItemName" as "ItemName",

      M."EvalSystem" as "InvnetoryMethod", 

      W."WhsCode" as "WarehouseCode",

      W."OnHand" as "QtyInWH",
	  W."AvgPrice",
	  SUM (IFNULL(V."InQty", 0) - IFNULL(V."OutQty", 0)) AS "QtyInUIVL",



W."StockValue",

case when W."OnHand" < 0

then round(W."OnHand"*W."AvgPrice",:sumdec) 
else

'0'

end as "Qty*StaticPrice",

SUM(V1."TransValue")as "Sum_TransValue"



From "UITW" W INNER join "UITM" M on W."ItemCode" = M."ItemCode" INNER join "UIVL" V on W."ItemCode" = V."ItemCode" and W."WhsCode" = V."LocCode"

 INNER join "UIVL1" V1 on V."TransSeq" = V1."TransSeq" 

Group by 

W."ItemCode",M."ItemCode", V."ItemCode",  M."ItemName", M."EvalSystem", W."WhsCode", V."LocCode", W."OnHand",

W."AvgPrice",W."StockValue"

having 

M."EvalSystem" in ('A','S') and 

(W."StockValue" <> sum(V1."TransValue") or 
	   W."OnHand"  = 0 and (W."StockValue" > 1 or W."StockValue" < -1));
end if;




if :isPricePerWH = 'N'


then



insert into "TmSp_IVIInventoryValueDiffsMAPnSTDItems_tmp"

Select  top 1

		M."ItemCode" as "ItemCode", 

		M."ItemName" as "ItemName",

		M."EvalSystem" as "InvnetoryMethod",

		'',

		M."OnHand" as "QtyInWH",

		M."AvgPrice",
				
		SUM (IFNULL(V."InQty",0) - IFNULL(V."OutQty",0)) AS "QtyInUIVLM",

		

		M."StockValue",

		case when M."OnHand" < 0

		then round(M."OnHand"*M."AvgPrice",:sumdec) 

		else

		'0'
         end as "Qty*StaticPrice",

		SUM(V1."TransValue")as "Sum_TransValue"

	

From 

      "UITM" M INNER join "UIVL" V on M."ItemCode" = V."ItemCode" INNER join "UIVL1" V1 on V."TransSeq" = V1."TransSeq" 

     

Group by 

      M."ItemCode",V."ItemCode",M."ItemName",M."EvalSystem",M."OnHand",M."AvgPrice",M."StockValue"

having 

M."EvalSystem" in ('A','S') and 

(M."StockValue" <> sum(V1."TransValue") or 
	   M."OnHand"  = 0 and (M."StockValue" > 1 or M."StockValue" < -1));

end if;
select count(1) into rowcount from "TmSp_IVIInventoryValueDiffsMAPnSTDItems_tmp";
delete from "TmSp_IVIInventoryValueDiffsMAPnSTDItems_tmp";
if(:rowcount > 0) 
then
--return -1;
retCode := -1;
return;
else
--/*Inventory Value recalculation for FIFO items is succesfull*/

--return 0;
retCode := 0;
return;
end if;
End;


