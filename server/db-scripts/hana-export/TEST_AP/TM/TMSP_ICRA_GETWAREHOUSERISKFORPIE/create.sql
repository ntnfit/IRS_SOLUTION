-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_ICRA_GetWarehouseRiskForPie ( IN priceListName char(32),
	 IN norate nvarchar(1))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS 
localCurrency nvarChar(3);
IsPricePerWH nvarchar(1);
ItemPara1Min DECIMAL(21,6);
ItemPara1Max DECIMAL(21,6);
ItemPara2Min DECIMAL(21,6);
ItemPara2Max DECIMAL(21,6);
ItemPara3Min DECIMAL(21,6);
ItemPara3Max DECIMAL(21,6);
WhseParaMin DECIMAL(21,6);
WhseParaMax DECIMAL(21,6);
BEGIN
select "KpiValue1" into ItemPara1Min from KPS1 where "KpsCode"='ICR' and "LineNum" = 1;
select "KpiValue2" into ItemPara1Max from KPS1 where "KpsCode"='ICR' and "LineNum" = 1;
select "KpiValue1" into ItemPara2Min from KPS1 where "KpsCode"='ICR' and "LineNum" = 2;
select "KpiValue2" into ItemPara2Max from KPS1 where "KpsCode"='ICR' and "LineNum" = 2;
select "KpiValue1" into ItemPara3Min from KPS1 where "KpsCode"='ICR' and "LineNum" = 3;
select "KpiValue2" into ItemPara3Max from KPS1 where "KpsCode"='ICR' and "LineNum" = 3;
select "KpiValue1"/100 into WhseParaMin from KPS1 where "KpsCode"='ICR2' and "LineNum" = 1;
select "KpiValue2"/100 into WhseParaMax from KPS1 where "KpsCode"='ICR2' and "LineNum" = 1;
IF :ItemPara1Min IS NULL OR :ItemPara1Max IS NULL OR :ItemPara1Min IS NULL OR :ItemPara2Min IS NULL OR :ItemPara2Max IS NULL OR :ItemPara3Min IS NULL OR :ItemPara3Max IS NULL OR :WhseParaMin IS NULL OR :WhseParaMax IS NULL OR :norate <> '0' THEN
	Select 0, 0, 0 from dummy;
ELSEIf :norate = '0' then
SELECT sum(CASE WHEN "percentage" > :WhseParaMax THEN 1 ELSE 0 END) AS "high",sum(CASE WHEN ("percentage" <= :WhseParaMax AND "percentage" >= :WhseParaMin) THEN 1 ELSE 0 END) AS "medium",sum(CASE WHEN "percentage" < :WhseParaMin THEN 1 ELSE 0 END) AS "low" FROM (
SELECT CASE WHEN "total" = 0 THEN 0 ELSE IFNULL(round(TO_DECIMAL("highitemvolume",19,6) / "total",2),0) END AS "percentage" FROM (
SELECT TLASTDATE."WhsCode" AS "WhsCode1",count(TLASTDATE."ItemCode") AS "highitemvolume" FROM (
SELECT T0."ItemCode",T0."WhsCode",T0."StockValue","AvgPrice" AS "AvgPriceWH",IFNULL(max(T1."CountDate"),T0."createDate") AS "lastCountDate" FROM OITW T0 LEFT OUTER JOIN (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T1 ON T0."WhsCode" = T1."WhsCode" AND T0."ItemCode" = T1."ItemCode" GROUP BY T0."WhsCode",T0."ItemCode",T0."AvgPrice",T0."createDate",T0."OnHand",T0."StockValue") AS TLASTDATE INNER JOIN (
SELECT "ItemCode","LstEvlPric","LastPurPrc","LastPurCur","EvalSystem","AvgPrice" FROM OITM) AS TVALUE ON TVALUE."ItemCode" = TLASTDATE."ItemCode" LEFT OUTER JOIN (
SELECT "Currency","Rate" AS "lastpurrat" FROM "ORTT" WHERE "RateDate" = cast(cast(NOW() AS VARCHAR) AS TIMESTAMP)) AS TRate ON TRate."Currency" = TVALUE."LastPurCur" LEFT OUTER JOIN (
SELECT "ItemCode","Price" AS "PLPrice","Currency" AS "PLCurrency",TRate1."plrat" FROM ITM1 T3 INNER JOIN OPLN T4 ON T4."ListName" = :priceListName AND T3."PriceList" = T4."ListNum" LEFT OUTER JOIN (
SELECT "Currency" AS "plCur","Rate" AS "plrat" FROM "ORTT" WHERE "RateDate" = cast(cast(NOW() AS VARCHAR) AS TIMESTAMP)) AS TRate1 ON TRate1."plCur" = T3."Currency") AS TPL ON TPL."ItemCode" = TLASTDATE."ItemCode" LEFT OUTER JOIN (
SELECT TLAST."ItemCode",TLAST."WhsCode",IFNULL(sum("InQty" + "OutQty"),0) AS "volume" FROM OIVL TIVL RIGHT OUTER JOIN (
SELECT T0."ItemCode",T0."WhsCode",IFNULL(T1."CountDate",T0."createDate") AS "lastCountDate",IFNULL(T1."CountTime",0) AS "lastCountTime" FROM OITW T0 LEFT OUTER JOIN (
SELECT TMAXDATE."ItemCode",TMAXDATE."WhsCode",TMAXDATE."CountDate",TMAXTIME."CountTime" FROM ((
SELECT "ItemCode","WhsCode",max("CountDate") AS "CountDate" FROM (
SELECT "ItemCode","WhsCode","CountDate","CountTime" FROM INC1 UNION
SELECT "ItemCode","WhsCode","CountDate","CountTime" FROM IQR1 WHERE "BaseEntry" IS NULL) AS TBASE GROUP BY "ItemCode","WhsCode") AS TMAXDATE LEFT OUTER JOIN (
SELECT "ItemCode","WhsCode","CountDate",max("CountTime") AS "CountTime" FROM (
SELECT "ItemCode","WhsCode","CountDate","CountTime" FROM INC1 UNION
SELECT "ItemCode","WhsCode","CountDate","CountTime" FROM IQR1 WHERE "BaseEntry" IS NULL) AS TBASE1 GROUP BY "ItemCode","WhsCode","CountDate") AS TMAXTIME ON TMAXDATE."ItemCode" = TMAXTIME."ItemCode" AND TMAXDATE."WhsCode" = TMAXTIME."WhsCode" AND TMAXDATE."CountDate" = TMAXTIME."CountDate")) AS T1 ON T0."WhsCode" = T1."WhsCode" AND T0."ItemCode" = T1."ItemCode" GROUP BY T0."WhsCode",T0."ItemCode",T0."createDate",T1."CountDate",T1."CountTime") AS TLAST ON TLAST."ItemCode" = TIVL."ItemCode" AND TLAST."WhsCode" = TIVL."LocCode" AND (TIVL."CreateDate" > TLAST."lastCountDate" OR (TIVL."CreateDate" = TLAST."lastCountDate" AND TIVL."CreateTime" > TLAST."lastCountTime")) GROUP BY TLAST."ItemCode",TLAST."WhsCode") AS TVOLUME ON TVOLUME."ItemCode" = TLASTDATE."ItemCode" AND TVOLUME."WhsCode" = TLASTDATE."WhsCode" LEFT OUTER JOIN (
SELECT "ItemCode","LocCode",CASE WHEN sum("OpenQty") = 0 THEN 0 ELSE sum("OpenValue") / sum("OpenQty") END AS "itemcost" FROM OIVQ GROUP BY "ItemCode","LocCode") AS TFIFOCOST ON TLASTDATE."ItemCode" = TFIFOCOST."ItemCode" AND TLASTDATE."WhsCode" = TFIFOCOST."LocCode" WHERE EXISTS (
SELECT * FROM OIVL WHERE "ItemCode" = TLASTDATE."ItemCode" AND "LocCode" = TLASTDATE."WhsCode") AND DAYS_BETWEEN("lastCountDate",NOW()) > :ItemPara1Max AND ("volume" > :ItemPara2Max OR (
SELECT CASE WHEN :priceListName = 'Last Evaluated Price' THEN "LstEvlPric" WHEN :priceListName = 'Last Purchase Price' THEN (CASE WHEN "LastPurCur" = (
SELECT "MainCurncy" FROM OADM) THEN "LastPurPrc" ELSE "lastpurrat" * "LastPurPrc" END) WHEN :priceListName = 'Item Cost' THEN (CASE WHEN ("EvalSystem" = 'F' AND "itemcost" > 0) THEN "itemcost" ELSE (CASE WHEN 'Y' = 'Y' THEN "AvgPriceWH" ELSE "AvgPrice" END) END) ELSE (CASE WHEN "PLCurrency" = (
SELECT "MainCurncy" FROM OADM) THEN "PLPrice" ELSE "plrat" * "PLPrice" END) END AS "lastpurlocal" FROM DUMMY) > :ItemPara3Max) GROUP BY TLASTDATE."WhsCode") AS THIGH RIGHT OUTER JOIN (
SELECT "LocCode",count(DISTINCT "ItemCode") AS "total" FROM OIVL GROUP BY "LocCode") AS TTotalItemVolume ON THIGH."WhsCode1" = TTotalItemVolume."LocCode") AS TPERCENT;
END IF;
END;


