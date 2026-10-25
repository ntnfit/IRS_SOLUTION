-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_ICRA_GetWarehouseValue ( 
IN CountDate NVARCHAR(20),
IN PriceListName char(32),
IN NoRate nvarchar(1)) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS 
LocalCurrency nvarChar(3);
IsPricePerWH nvarchar(1);
ConvertedCountDate DATE;
BEGIN
SELECT "MainCurncy" INTO LocalCurrency FROM OADM;
SELECT "PriceSys" INTO IsPricePerWH FROM OADM;
ConvertedCountDate := TO_DATE(:CountDate,'MM/DD/YYYY');
If :PriceListName='Last Evaluated Price' then
SELECT * FROM (
SELECT TOP 10 '#' || T0."WhsCode" AS "code", "BinActivat", IFNULL ("valuecounted", 0) AS "valuecounted", (IFNULL("valuetotal", 0) - IFNULL("valuecounted", 0)) AS "valueuncounted", IFNULL("valuecountedbin", 0) AS "valuecountedbin", CASE WHEN "BinActivat" = 'Y' THEN (IFNULL("valuetotalbin", 0) - IFNULL("valuecountedbin", 0) - IFNULL("valuecounted", 0)) ELSE (IFNULL("valuetotalbin", 0) - IFNULL("valuecountedbin", 0)) END AS "valueuncountedbin", CASE WHEN "BinActivat" = 'Y' THEN IFNULL("valuetotalbin", 0) ELSE IFNULL("valuetotal", 0) END AS "value" FROM (
SELECT "WhsCode","BinActivat" FROM OWHS TOWHS INNER JOIN (
SELECT DISTINCT "LocCode" FROM OIVL) AS TOIVL ON TOWHS."WhsCode" = TOIVL."LocCode") AS T0 LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."LstEvlPric") AS "valuecounted" FROM OITW T0 LEFT OUTER JOIN OITM T3 ON T3."ItemCode" = T0."ItemCode" WHERE T0."OnHand" > 0 AND T0."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T1 WHERE T0."WhsCode" = T1."WhsCode" AND (T1."BinEntry" IS NULL) AND T1."CountDate" >= :ConvertedCountDate) GROUP BY "WhsCode") AS TCounted ON TCounted."WhsCode" = T0."WhsCode" LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."LstEvlPric") AS "valuetotal" FROM OITW T0 LEFT OUTER JOIN OITM T3 ON T3."ItemCode" = T0."ItemCode" GROUP BY "WhsCode") AS TTotal ON T0."WhsCode" = TTotal."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty" * T3."LstEvlPric") AS "valuecountedbin" FROM OIBQ T1 INNER JOIN OITM T3 ON T3."ItemCode" = T1."ItemCode" WHERE t1."OnHandQty" > 0 AND T1."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T2 WHERE T1."WhsCode" = T2."WhsCode" AND T2."BinEntry" = T1."BinAbs" AND T2."CountDate" >= :ConvertedCountDate) AND T1."ItemCode" NOT IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T3 WHERE T3."WhsCode" = T1."WhsCode" AND (T3."BinEntry" IS NULL) AND T3."CountDate" >= :ConvertedCountDate) GROUP BY T1."WhsCode") AS T4 ON T0."WhsCode" = T4."WhsCode" LEFT OUTER JOIN (
SELECT T5."WhsCode",sum(T5."OnHandQty" * T3."LstEvlPric") AS "valuetotalbin" FROM OIBQ T5 INNER JOIN OITM T3 ON T3."ItemCode" = T5."ItemCode" GROUP BY T5."WhsCode") AS T7 ON T0."WhsCode" = T7."WhsCode" ORDER BY "value" DESC) T ORDER BY T."code";
elseif :PriceListName='Last Purchase Price' then 
if :NoRate='0' then 
SELECT * FROM (
SELECT TOP 10 '#' || TLOCAL."WhsCode" AS "code", TLOCAL."BinActivat", ("valueCountedLocal" + "valueCountedForeign") AS "valuecounted", ("valueTotalLocal" + "valueTotalForeign" - "valueCountedLocal" - "valueCountedForeign") AS "valueUncounted", ("valueCountedLocalbin" + "valueCountedForeignbin") AS "valuecountedbin", CASE WHEN "BinActivat" = 'Y' THEN ("valueTotalLocalbin" + "valueTotalForeignbin" - "valueCountedLocalbin" - "valueCountedForeignbin" - "valueCountedLocal" - "valueCountedForeign") ELSE ("valueTotalLocalbin" + "valueTotalForeignbin" - "valueCountedLocalbin" - "valueCountedForeignbin") END AS "valueuncountedbin", CASE WHEN "BinActivat" = 'Y' THEN ("valueTotalLocalbin" + "valueTotalForeignbin") ELSE ("valueTotalLocal" + "valueTotalForeign") END AS "value" FROM (
SELECT T0."WhsCode","BinActivat",IFNULL("valueCountedLocal",0) AS "valueCountedLocal",IFNULL("valueTotalLocal",0) AS "valueTotalLocal",IFNULL("valueCountedLocalbin",0) AS "valueCountedLocalbin",IFNULL("valueTotalLocalbin",0) AS "valueTotalLocalbin" FROM (
SELECT "WhsCode","BinActivat" FROM OWHS TOWHS INNER JOIN (
SELECT DISTINCT "LocCode" FROM OIVL) AS TOIVL ON TOWHS."WhsCode" = TOIVL."LocCode") AS T0 LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."LastPurPrc") AS "valueCountedLocal" FROM OITW T0 LEFT OUTER JOIN OITM T3 ON T3."ItemCode" = T0."ItemCode" AND T3."LastPurCur" = :LocalCurrency WHERE T0."OnHand" > 0 AND T0."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T1 WHERE T0."WhsCode" = T1."WhsCode" AND (T1."BinEntry" IS NULL) AND T1."CountDate" >= :ConvertedCountDate) GROUP BY "WhsCode") AS T2 ON T0."WhsCode" = T2."WhsCode" LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."LastPurPrc") AS "valueTotalLocal" FROM OITW T0 LEFT OUTER JOIN OITM T3 ON T3."ItemCode" = T0."ItemCode" AND T3."LastPurCur" = :LocalCurrency GROUP BY "WhsCode") AS T1 ON T0."WhsCode" = T1."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty" * T3."LastPurPrc") AS "valueCountedLocalbin" FROM OIBQ T1 LEFT OUTER JOIN OITM T3 ON T3."ItemCode" = T1."ItemCode" AND T3."LastPurCur" = :LocalCurrency WHERE t1."OnHandQty" > 0 AND T1."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T2 WHERE T1."WhsCode" = T2."WhsCode" AND T2."BinEntry" = T1."BinAbs" AND T2."CountDate" >= :ConvertedCountDate) AND T1."ItemCode" NOT IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T3 WHERE T3."WhsCode" = T1."WhsCode" AND (T3."BinEntry" IS NULL) AND T3."CountDate" >= :ConvertedCountDate) GROUP BY T1."WhsCode") AS T4 ON T0."WhsCode" = T4."WhsCode" LEFT OUTER JOIN (
SELECT T5."WhsCode",sum(T5."OnHandQty" * T3."LastPurPrc") AS "valueTotalLocalbin" FROM OIBQ T5 LEFT OUTER JOIN OITM T3 ON T3."ItemCode" = T5."ItemCode" AND T3."LastPurCur" = :LocalCurrency GROUP BY T5."WhsCode") AS T7 ON T0."WhsCode" = T7."WhsCode") AS TLOCAL INNER JOIN (
SELECT T0."WhsCode",IFNULL("valueCountedForeign",0) AS "valueCountedForeign",IFNULL("valueTotalForeign",0) AS "valueTotalForeign",IFNULL("valueCountedForeignbin",0) AS "valueCountedForeignbin",IFNULL("valueTotalForeignbin",0) AS "valueTotalForeignbin" FROM (
SELECT "WhsCode","BinActivat" FROM OWHS TOWHS INNER JOIN (
SELECT DISTINCT "LocCode" FROM OIVL) AS TOIVL ON TOWHS."WhsCode" = TOIVL."LocCode") AS T0 LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."LastPurPrc" * T5."Rate") AS "valueCountedForeign" FROM OITW T0 LEFT OUTER JOIN OITM T3 ON T3."ItemCode" = T0."ItemCode" AND T3."LastPurCur" <> :LocalCurrency INNER JOIN (
SELECT "Currency","Rate" FROM ORTT WHERE "RateDate" = cast(cast(NOW() AS VARCHAR) AS TIMESTAMP)) AS T5 ON T5."Currency" = T3."LastPurCur" WHERE T0."OnHand" > 0 AND T0."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T1 WHERE T0."WhsCode" = T1."WhsCode" AND (T1."BinEntry" IS NULL) AND T1."CountDate" >= :ConvertedCountDate) GROUP BY "WhsCode") AS T2 ON T0."WhsCode" = T2."WhsCode" LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."LastPurPrc" * T5."Rate") AS "valueTotalForeign" FROM OITW T0 LEFT OUTER JOIN OITM T3 ON T3."ItemCode" = T0."ItemCode" AND T3."LastPurCur" <> :LocalCurrency INNER JOIN (
SELECT "Currency","Rate" FROM ORTT WHERE "RateDate" = cast(cast(NOW() AS VARCHAR) AS TIMESTAMP)) AS T5 ON T5."Currency" = T3."LastPurCur" GROUP BY "WhsCode") AS T1 ON T0."WhsCode" = T1."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty" * T3."LastPurPrc" * T5."Rate") AS "valueCountedForeignbin" FROM OIBQ T1 LEFT OUTER JOIN OITM T3 ON T3."ItemCode" = T1."ItemCode" AND T3."LastPurCur" <> :LocalCurrency INNER JOIN (
SELECT "Currency","Rate" FROM ORTT WHERE "RateDate" = cast(cast(NOW() AS VARCHAR) AS TIMESTAMP)) AS T5 ON T5."Currency" = T3."LastPurCur" WHERE t1."OnHandQty" > 0 AND T1."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T2 WHERE T1."WhsCode" = T2."WhsCode" AND T2."BinEntry" = T1."BinAbs" AND T2."CountDate" >= :ConvertedCountDate) AND T1."ItemCode" NOT IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T3 WHERE T3."WhsCode" = T1."WhsCode" AND (T3."BinEntry" IS NULL) AND T3."CountDate" >= :ConvertedCountDate) GROUP BY T1."WhsCode") AS T4 ON T0."WhsCode" = T4."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty" * T3."LastPurPrc" * T5."Rate") AS "valueTotalForeignbin" FROM OIBQ T1 INNER JOIN OITM T3 ON T3."ItemCode" = T1."ItemCode" AND T3."LastPurCur" <> :LocalCurrency INNER JOIN (
SELECT "Currency","Rate" FROM ORTT WHERE "RateDate" = cast(cast(NOW() AS VARCHAR) AS TIMESTAMP)) AS T5 ON T5."Currency" = T3."LastPurCur" GROUP BY T1."WhsCode") AS T7 ON T0."WhsCode" = T7."WhsCode") AS TFOREIGN ON TLOCAL."WhsCode" = TFOREIGN."WhsCode" ORDER BY "value" DESC) T ORDER BY T."code";
end if;
elseif :PriceListName='Item Cost' then 
if :IsPricePerWH = 'Y' then 
SELECT * FROM (
SELECT TOP 10 '#' || TNOTFIFO."WhsCode" AS "code", TNOTFIFO."BinActivat", ("valueCountedNOTFIFO" + "valueCountedFIFO") AS "valuecounted", ("valueTotalNOTFIFO" + "valueTotalFIFO" - "valueCountedNOTFIFO" - "valueCountedFIFO") AS "valueUncounted", ("valueCountedNOTFIFObin" + "valueCountedFIFObin") AS "valuecountedbin", CASE WHEN "BinActivat" = 'Y' THEN ("valueTotalNotFIFObin" + "valueTotalFIFObin" - "valueCountedNOTFIFObin" - "valueCountedFIFObin" - "valueCountedNOTFIFO" - "valueCountedFIFO") ELSE ("valueTotalNotFIFObin" + "valueTotalFIFObin" - "valueCountedNOTFIFObin" - "valueCountedFIFObin") END AS "valueuncountedbin", CASE WHEN "BinActivat" = 'Y' THEN ("valueTotalFIFObin" + "valueTotalNotFIFObin") ELSE ("valueTotalFIFO" + "valueTotalNOTFIFO") END AS "value" FROM (
SELECT T0."WhsCode",T0."BinActivat",IFNULL("valueCountedNOTFIFO",0) AS "valueCountedNOTFIFO",IFNULL("valueTotalNOTFIFO",0) AS "valueTotalNOTFIFO",IFNULL("valueCountedNOTFIFObin",0) AS "valueCountedNOTFIFObin",IFNULL("valueTotalNotFIFObin",0) AS "valueTotalNotFIFObin" FROM (
SELECT "WhsCode","BinActivat" FROM OWHS TOWHS INNER JOIN (
SELECT DISTINCT "LocCode" FROM OIVL) AS TOIVL ON TOWHS."WhsCode" = TOIVL."LocCode") AS T0 LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T0."AvgPrice") AS "valueCountedNOTFIFO" FROM OITW T0 INNER JOIN OITM T2 ON T2."ItemCode" = T0."ItemCode" AND T2."EvalSystem" <> 'F' WHERE T0."OnHand" > 0 AND T0."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T1 WHERE T0."WhsCode" = T1."WhsCode" AND (T1."BinEntry" IS NULL) AND T1."CountDate" >= :ConvertedCountDate) GROUP BY "WhsCode") AS T2 ON T0."WhsCode" = T2."WhsCode" LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T0."AvgPrice") AS "valueTotalNOTFIFO" FROM OITW T0 INNER JOIN OITM T2 ON T2."ItemCode" = T0."ItemCode" AND T2."EvalSystem" <> 'F' GROUP BY "WhsCode") AS T1 ON T0."WhsCode" = T1."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty" * T3."AvgPrice") AS "valueCountedNOTFIFObin" FROM OIBQ T1 INNER JOIN OITM T2 ON T2."ItemCode" = T1."ItemCode" AND T2."EvalSystem" <> 'F' INNER JOIN OITW T3 ON T3."ItemCode" = T1."ItemCode" AND T3."WhsCode" = T1."WhsCode" WHERE t1."OnHandQty" > 0 AND T1."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T2 WHERE T1."WhsCode" = T2."WhsCode" AND T2."BinEntry" = T1."BinAbs" AND T2."CountDate" >= :ConvertedCountDate) AND T1."ItemCode" NOT IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T3 WHERE T3."WhsCode" = T1."WhsCode" AND (T3."BinEntry" IS NULL) AND T3."CountDate" >= :ConvertedCountDate) GROUP BY T1."WhsCode") AS T4 ON T0."WhsCode" = T4."WhsCode" LEFT OUTER JOIN (
SELECT T5."WhsCode",sum(T5."OnHandQty" * T3."AvgPrice") AS "valueTotalNotFIFObin" FROM OIBQ T5 INNER JOIN OITM T2 ON T2."ItemCode" = T5."ItemCode" AND T2."EvalSystem" <> 'F' INNER JOIN OITW T3 ON T3."ItemCode" = T5."ItemCode" AND T3."WhsCode" = T5."WhsCode" GROUP BY T5."WhsCode") AS T7 ON T0."WhsCode" = T7."WhsCode") AS TNOTFIFO INNER JOIN (
SELECT T0."WhsCode",IFNULL("valueCountedFIFO",0) AS "valueCountedFIFO",IFNULL("valueTotalFIFO",0) AS "valueTotalFIFO",IFNULL("valueCountedFIFObin",0) AS "valueCountedFIFObin",IFNULL("valueTotalFIFObin",0) AS "valueTotalFIFObin" FROM (
SELECT "WhsCode","BinActivat" FROM OWHS TOWHS INNER JOIN (
SELECT DISTINCT "LocCode" FROM OIVL) AS TOIVL ON TOWHS."WhsCode" = TOIVL."LocCode") AS T0 LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."Price") AS "valueCountedFIFO" FROM OITW T0 INNER JOIN OITM T2 ON T2."ItemCode" = T0."ItemCode" AND T2."EvalSystem" = 'F' INNER JOIN (
SELECT "ItemCode","LocCode",sum("OpenValue") / sum("OpenQty") AS "Price" FROM OIVQ GROUP BY "ItemCode","LocCode") AS T3 ON T3."ItemCode" = T2."ItemCode" AND T3."LocCode" = T0."WhsCode" WHERE T0."OnHand" > 0 AND T0."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T1 WHERE T0."WhsCode" = T1."WhsCode" AND (T1."BinEntry" IS NULL) AND T1."CountDate" >= :ConvertedCountDate) GROUP BY "WhsCode") AS T2 ON T0."WhsCode" = T2."WhsCode" LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."Price") AS "valueTotalFIFO" FROM OITW T0 INNER JOIN OITM T2 ON T2."ItemCode" = T0."ItemCode" AND T2."EvalSystem" = 'F' INNER JOIN (
SELECT "ItemCode","LocCode",sum("OpenValue") / sum("OpenQty") AS "Price" FROM OIVQ GROUP BY "ItemCode","LocCode") AS T3 ON T3."ItemCode" = T2."ItemCode" AND T3."LocCode" = T0."WhsCode" GROUP BY "WhsCode") AS T1 ON T0."WhsCode" = T1."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty" * T3."Price") AS "valueCountedFIFObin" FROM OIBQ T1 INNER JOIN OITM T2 ON T2."ItemCode" = T1."ItemCode" AND T2."EvalSystem" = 'F' INNER JOIN (
SELECT "ItemCode","LocCode",sum("OpenValue") / sum("OpenQty") AS "Price" FROM OIVQ GROUP BY "ItemCode","LocCode") AS T3 ON T3."ItemCode" = T2."ItemCode" AND T3."LocCode" = T1."WhsCode" WHERE t1."OnHandQty" > 0 AND T1."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T2 WHERE T1."WhsCode" = T2."WhsCode" AND T2."BinEntry" = T1."BinAbs" AND T2."CountDate" >= :ConvertedCountDate) AND T1."ItemCode" NOT IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T3 WHERE T3."WhsCode" = T1."WhsCode" AND (T3."BinEntry" IS NULL) AND T3."CountDate" >= :ConvertedCountDate) GROUP BY T1."WhsCode") AS T4 ON T0."WhsCode" = T4."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty" * T3."Price") AS "valueTotalFIFObin" FROM OIBQ T1 INNER JOIN OITM T2 ON T2."ItemCode" = T1."ItemCode" AND T2."EvalSystem" = 'F' INNER JOIN (
SELECT "ItemCode","LocCode",sum("OpenValue") / sum("OpenQty") AS "Price" FROM OIVQ GROUP BY "ItemCode","LocCode") AS T3 ON T3."ItemCode" = T2."ItemCode" AND T3."LocCode" = T1."WhsCode" GROUP BY T1."WhsCode") AS T7 ON T0."WhsCode" = T7."WhsCode") AS TFIFO ON TNOTFIFO."WhsCode" = TFIFO."WhsCode" ORDER BY "value" DESC) T ORDER BY T."code";

ELSE 
SELECT * FROM (
SELECT TOP 10 '#' || TNOTFIFO."WhsCode" AS "code", TNOTFIFO."BinActivat", ("valueCountedNOTFIFO" + "valueCountedFIFO") AS "valuecounted", ("valueTotalNOTFIFO" + "valueTotalFIFO" - "valueCountedNOTFIFO" - "valueCountedFIFO") AS "valueUncounted", ("valueCountedNOTFIFObin" + "valueCountedFIFObin") AS "valuecountedbin", CASE WHEN "BinActivat" = 'Y' THEN ("valueTotalNOTFIFObin" + "valueTotalFIFObin" - "valueCountedNOTFIFObin" - "valueCountedFIFObin" - "valueCountedNOTFIFO" - "valueCountedFIFO") ELSE ("valueTotalNOTFIFObin" + "valueTotalFIFObin" - "valueCountedNOTFIFObin" - "valueCountedFIFObin") END AS "valueuncountedbin", CASE WHEN "BinActivat" = 'Y' THEN ("valueTotalNOTFIFObin" + "valueTotalFIFObin") ELSE ("valueTotalNOTFIFO" + "valueTotalFIFO") END AS "value" FROM (
SELECT T0."WhsCode",T0."BinActivat",IFNULL("valueCountedNOTFIFO",0) AS "valueCountedNOTFIFO",IFNULL("valueTotalNOTFIFO",0) AS "valueTotalNOTFIFO",IFNULL("valueCountedNOTFIFObin",0) AS "valueCountedNOTFIFObin",IFNULL("valueTotalNOTFIFObin",0) AS "valueTotalNOTFIFObin" FROM (
SELECT "WhsCode","BinActivat" FROM OWHS TOWHS INNER JOIN (
SELECT DISTINCT "LocCode" FROM OIVL) AS TOIVL ON TOWHS."WhsCode" = TOIVL."LocCode") AS T0 LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T2."AvgPrice") AS "valueCountedNOTFIFO" FROM OITW T0 INNER JOIN OITM T2 ON T2."ItemCode" = T0."ItemCode" AND T2."EvalSystem" <> 'F' WHERE T0."OnHand" > 0 AND T0."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T1 WHERE T0."WhsCode" = T1."WhsCode" AND (T1."BinEntry" IS NULL) AND T1."CountDate" >= :ConvertedCountDate) GROUP BY "WhsCode") AS T2 ON T0."WhsCode" = T2."WhsCode" LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T2."AvgPrice") AS "valueTotalNOTFIFO" FROM OITW T0 INNER JOIN OITM T2 ON T2."ItemCode" = T0."ItemCode" AND T2."EvalSystem" <> 'F' GROUP BY "WhsCode") AS T1 ON T0."WhsCode" = T1."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty" * T2."AvgPrice") AS "valueCountedNOTFIFObin" FROM OIBQ T1 INNER JOIN OITM T2 ON T2."ItemCode" = T1."ItemCode" AND T2."EvalSystem" <> 'F' INNER JOIN OITW T3 ON T3."ItemCode" = T1."ItemCode" AND T3."WhsCode" = T1."WhsCode" WHERE t1."OnHandQty" > 0 AND T1."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T2 WHERE T1."WhsCode" = T2."WhsCode" AND T2."BinEntry" = T1."BinAbs" AND T2."CountDate" >= :ConvertedCountDate) AND T1."ItemCode" NOT IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T3 WHERE T3."WhsCode" = T1."WhsCode" AND (T3."BinEntry" IS NULL) AND T3."CountDate" >= :ConvertedCountDate) GROUP BY T1."WhsCode") AS T4 ON T0."WhsCode" = T4."WhsCode" LEFT OUTER JOIN (
SELECT T5."WhsCode",sum(T5."OnHandQty" * T2."AvgPrice") AS "valueTotalNOTFIFObin" FROM OIBQ T5 INNER JOIN OITM T2 ON T2."ItemCode" = T5."ItemCode" AND T2."EvalSystem" <> 'F' INNER JOIN OITW T3 ON T3."ItemCode" = T5."ItemCode" AND T3."WhsCode" = T5."WhsCode" GROUP BY T5."WhsCode") AS T7 ON T0."WhsCode" = T7."WhsCode") AS TNOTFIFO INNER JOIN (
SELECT T0."WhsCode",IFNULL("qtyCountedFIFO",0) AS "qtyCountedFIFO",IFNULL("qtyTotalFIFO",0) AS "qtyTotalFIFO",IFNULL("valueCountedFIFO",0) AS "valueCountedFIFO",IFNULL("valueTotalFIFO",0) AS "valueTotalFIFO",IFNULL("qtyCountedFIFObin",0) AS "qtyCountedFIFObin",IFNULL("qtyTotalFIFObin",0) AS "qtyTotalFIFObin",IFNULL("valueCountedFIFObin",0) AS "valueCountedFIFObin",IFNULL("valueTotalFIFObin",0) AS "valueTotalFIFObin" FROM (
SELECT "WhsCode","BinActivat" FROM OWHS TOWHS INNER JOIN (
SELECT DISTINCT "LocCode" FROM OIVL) AS TOIVL ON TOWHS."WhsCode" = TOIVL."LocCode") AS T0 LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand") AS "qtyCountedFIFO",sum(T0."OnHand" * T3."Price") AS "valueCountedFIFO" FROM OITW T0 INNER JOIN OITM T2 ON T2."ItemCode" = T0."ItemCode" AND T2."EvalSystem" = 'F' INNER JOIN (
SELECT "ItemCode","LocCode",sum("OpenValue") / sum("OpenQty") AS "Price" FROM OIVQ GROUP BY "ItemCode","LocCode") AS T3 ON T3."ItemCode" = T2."ItemCode" AND T3."LocCode" = T0."WhsCode" WHERE T0."OnHand" > 0 AND T0."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T1 WHERE T0."WhsCode" = T1."WhsCode" AND (T1."BinEntry" IS NULL) AND T1."CountDate" >= :ConvertedCountDate) GROUP BY "WhsCode") AS T2 ON T0."WhsCode" = T2."WhsCode" LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand") AS "qtyTotalFIFO",sum(T0."OnHand" * T3."Price") AS "valueTotalFIFO" FROM OITW T0 INNER JOIN OITM T2 ON T2."ItemCode" = T0."ItemCode" AND T2."EvalSystem" = 'F' INNER JOIN (
SELECT "ItemCode","LocCode",sum("OpenValue") / sum("OpenQty") AS "Price" FROM OIVQ GROUP BY "ItemCode","LocCode") AS T3 ON T3."ItemCode" = T2."ItemCode" AND T3."LocCode" = T0."WhsCode" GROUP BY "WhsCode") AS T1 ON T0."WhsCode" = T1."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty") AS "qtyCountedFIFObin",sum(T1."OnHandQty" * T3."Price") AS "valueCountedFIFObin" FROM OIBQ T1 INNER JOIN OITM T2 ON T2."ItemCode" = T1."ItemCode" AND T2."EvalSystem" = 'F' INNER JOIN (
SELECT "ItemCode","LocCode",sum("OpenValue") / sum("OpenQty") AS "Price" FROM OIVQ GROUP BY "ItemCode","LocCode") AS T3 ON T3."ItemCode" = T2."ItemCode" AND T3."LocCode" = T1."WhsCode" WHERE t1."OnHandQty" > 0 AND T1."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T2 WHERE T1."WhsCode" = T2."WhsCode" AND T2."BinEntry" = T1."BinAbs" AND T2."CountDate" >= :ConvertedCountDate) AND T1."ItemCode" NOT IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T3 WHERE T3."WhsCode" = T1."WhsCode" AND (T3."BinEntry" IS NULL) AND T3."CountDate" >= :ConvertedCountDate) GROUP BY T1."WhsCode") AS T4 ON T0."WhsCode" = T4."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty") AS "qtyTotalFIFObin",sum(T1."OnHandQty" * T3."Price") AS "valueTotalFIFObin" FROM OIBQ T1 INNER JOIN OITM T2 ON T2."ItemCode" = T1."ItemCode" AND T2."EvalSystem" = 'F' INNER JOIN (
SELECT "ItemCode","LocCode",sum("OpenValue") / sum("OpenQty") AS "Price" FROM OIVQ GROUP BY "ItemCode","LocCode") AS T3 ON T3."ItemCode" = T2."ItemCode" AND T3."LocCode" = T1."WhsCode" GROUP BY T1."WhsCode") AS T7 ON T0."WhsCode" = T7."WhsCode") AS TFIFO ON TNOTFIFO."WhsCode" = TFIFO."WhsCode" ORDER BY "value" DESC) T ORDER BY T."code";
END if;
elseif :NoRate='0' then 
SELECT * FROM (
SELECT TOP 10 '#' || TLOCAL."WhsCode" AS "code", TLOCAL."BinActivat", ("valueCountedLocal" + "valueCountedForeign") AS "valuecounted", ("valueTotalLocal" + "valueTotalForeign" - "valueCountedLocal" - "valueCountedForeign") AS "valueuncounted", ("valueCountedLocalbin" + "valueCountedForeignbin") AS "valuecountedbin", CASE WHEN "BinActivat" = 'Y' THEN ("valueTotalLocalbin" + "valueTotalForeignbin" - "valueCountedLocalbin" - "valueCountedForeignbin" - "valueCountedLocal" - "valueCountedForeign") ELSE ("valueTotalLocalbin" + "valueTotalForeignbin" - "valueCountedLocalbin" - "valueCountedForeignbin") END AS "valueuncountedbin", CASE WHEN "BinActivat" = 'Y' THEN ("valueTotalLocalbin" + "valueTotalForeignbin") ELSE ("valueTotalLocal" + "valueTotalForeign") END AS "value" FROM (
SELECT T0."WhsCode","BinActivat",IFNULL("valueCountedLocal",0) AS "valueCountedLocal",IFNULL("valueTotalLocal",0) AS "valueTotalLocal",IFNULL("valueCountedLocalbin",0) AS "valueCountedLocalbin",IFNULL("valueTotalLocalbin",0) AS "valueTotalLocalbin" FROM (
SELECT "WhsCode","BinActivat" FROM OWHS TOWHS INNER JOIN (
SELECT DISTINCT "LocCode" FROM OIVL) AS TOIVL ON TOWHS."WhsCode" = TOIVL."LocCode") AS T0 LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."Price") AS "valueCountedLocal" FROM OITW T0 INNER JOIN (
SELECT TITM1."ItemCode",TITM1."Price" FROM ITM1 TITM1 INNER JOIN OPLN TOPLN ON TITM1."PriceList" = TOPLN."ListNum" AND TITM1."Currency" = (:LocalCurrency) AND TOPLN."ListName" = :priceListName) AS T3 ON T0."ItemCode" = T3."ItemCode" WHERE T0."OnHand" > 0 AND T0."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T1 WHERE T0."WhsCode" = T1."WhsCode" AND (T1."BinEntry" IS NULL) AND T1."CountDate" >= :ConvertedCountDate) GROUP BY "WhsCode") AS T2 ON T0."WhsCode" = T2."WhsCode" LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."Price") AS "valueTotalLocal" FROM OITW T0 INNER JOIN (
SELECT TITM1."ItemCode",TITM1."Price" FROM ITM1 TITM1 INNER JOIN OPLN TOPLN ON TITM1."PriceList" = TOPLN."ListNum" AND TITM1."Currency" = (:LocalCurrency) AND TOPLN."ListName" = :priceListName) AS T3 ON T0."ItemCode" = T3."ItemCode" GROUP BY "WhsCode") AS T1 ON T0."WhsCode" = T1."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty" * T3."Price") AS "valueCountedLocalbin" FROM OIBQ T1 INNER JOIN (
SELECT TITM1."ItemCode",TITM1."Price" FROM ITM1 TITM1 INNER JOIN OPLN TOPLN ON TITM1."PriceList" = TOPLN."ListNum" AND TITM1."Currency" = (:LocalCurrency) AND TOPLN."ListName" = :priceListName) AS T3 ON T1."ItemCode" = T3."ItemCode" WHERE t1."OnHandQty" > 0 AND T1."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T2 WHERE T1."WhsCode" = T2."WhsCode" AND T2."BinEntry" = T1."BinAbs" AND T2."CountDate" >= :ConvertedCountDate) AND T1."ItemCode" NOT IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T3 WHERE T3."WhsCode" = T1."WhsCode" AND (T3."BinEntry" IS NULL) AND T3."CountDate" >= :ConvertedCountDate) GROUP BY T1."WhsCode") AS T4 ON T0."WhsCode" = T4."WhsCode" LEFT OUTER JOIN (
SELECT T5."WhsCode",sum(T5."OnHandQty" * T3."Price") AS "valueTotalLocalbin" FROM OIBQ T5 INNER JOIN (
SELECT TITM1."ItemCode",TITM1."Price" FROM ITM1 TITM1 INNER JOIN OPLN TOPLN ON TITM1."PriceList" = TOPLN."ListNum" AND TITM1."Currency" = (:LocalCurrency) AND TOPLN."ListName" = :priceListName) AS T3 ON T5."ItemCode" = T3."ItemCode" GROUP BY T5."WhsCode") AS T7 ON T0."WhsCode" = T7."WhsCode") AS TLOCAL INNER JOIN (
SELECT T0."WhsCode",IFNULL("valueCountedForeign",0) AS "valueCountedForeign",IFNULL("valueTotalForeign",0) AS "valueTotalForeign",IFNULL("valueCountedForeignbin",0) AS "valueCountedForeignbin",IFNULL("valueTotalForeignbin",0) AS "valueTotalForeignbin" FROM (
SELECT "WhsCode","BinActivat" FROM OWHS TOWHS INNER JOIN (
SELECT DISTINCT "LocCode" FROM OIVL) AS TOIVL ON TOWHS."WhsCode" = TOIVL."LocCode") AS T0 LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."Price" * T5."Rate") AS "valueCountedForeign" FROM OITW T0 INNER JOIN (
SELECT TITM1."ItemCode",TITM1."Price",TITM1."Currency" FROM ITM1 TITM1 INNER JOIN OPLN TOPLN ON TITM1."PriceList" = TOPLN."ListNum" AND TITM1."Currency" <> (:LocalCurrency) AND TOPLN."ListName" = :priceListName) AS T3 ON T0."ItemCode" = T3."ItemCode" INNER JOIN (
SELECT "Currency","Rate" FROM ORTT WHERE "RateDate" = cast(cast(NOW() AS VARCHAR) AS TIMESTAMP)) AS T5 ON T5."Currency" = T3."Currency" WHERE T0."OnHand" > 0 AND T0."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T1 WHERE T0."WhsCode" = T1."WhsCode" AND (T1."BinEntry" IS NULL) AND T1."CountDate" >= :ConvertedCountDate) GROUP BY "WhsCode") AS T2 ON T0."WhsCode" = T2."WhsCode" LEFT OUTER JOIN (
SELECT "WhsCode",sum(T0."OnHand" * T3."Price" * T5."Rate") AS "valueTotalForeign" FROM OITW T0 INNER JOIN (
SELECT TITM1."ItemCode",TITM1."Price",TITM1."Currency" FROM ITM1 TITM1 INNER JOIN OPLN TOPLN ON TITM1."PriceList" = TOPLN."ListNum" AND TITM1."Currency" <> (:LocalCurrency) AND TOPLN."ListName" = :priceListName) AS T3 ON T0."ItemCode" = T3."ItemCode" INNER JOIN (
SELECT "Currency","Rate" FROM ORTT WHERE "RateDate" = cast(cast(NOW() AS VARCHAR) AS TIMESTAMP)) AS T5 ON T5."Currency" = T3."Currency" GROUP BY "WhsCode") AS T1 ON T0."WhsCode" = T1."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty" * T3."Price" * T5."Rate") AS "valueCountedForeignbin" FROM OIBQ T1 INNER JOIN (
SELECT TITM1."ItemCode",TITM1."Price",TITM1."Currency" FROM ITM1 TITM1 INNER JOIN OPLN TOPLN ON TITM1."PriceList" = TOPLN."ListNum" AND TITM1."Currency" <> (:LocalCurrency) AND TOPLN."ListName" = :priceListName) AS T3 ON T1."ItemCode" = T3."ItemCode" INNER JOIN (
SELECT "Currency","Rate" FROM ORTT WHERE "RateDate" = cast(cast(NOW() AS VARCHAR) AS TIMESTAMP)) AS T5 ON T5."Currency" = T3."Currency" WHERE t1."OnHandQty" > 0 AND T1."ItemCode" IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T2 WHERE T1."WhsCode" = T2."WhsCode" AND T2."BinEntry" = T1."BinAbs" AND T2."CountDate" >= :ConvertedCountDate) AND T1."ItemCode" NOT IN (
SELECT "ItemCode" FROM (
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM INC1 UNION
SELECT "ItemCode","WhsCode","BinEntry","CountDate" FROM IQR1 WHERE "BaseEntry" IS NULL) AS T3 WHERE T3."WhsCode" = T1."WhsCode" AND (T3."BinEntry" IS NULL) AND T3."CountDate" >= :ConvertedCountDate) GROUP BY T1."WhsCode") AS T4 ON T0."WhsCode" = T4."WhsCode" LEFT OUTER JOIN (
SELECT T1."WhsCode",sum(T1."OnHandQty" * T3."Price" * T5."Rate") AS "valueTotalForeignbin" FROM OIBQ T1 INNER JOIN (
SELECT TITM1."ItemCode",TITM1."Price",TITM1."Currency" FROM ITM1 TITM1 INNER JOIN OPLN TOPLN ON TITM1."PriceList" = TOPLN."ListNum" AND TITM1."Currency" <> (:LocalCurrency) AND TOPLN."ListName" = :priceListName) AS T3 ON T1."ItemCode" = T3."ItemCode" INNER JOIN (
SELECT "Currency","Rate" FROM ORTT WHERE "RateDate" = cast(cast(NOW() AS VARCHAR) AS TIMESTAMP)) AS T5 ON T5."Currency" = T3."Currency" GROUP BY T1."WhsCode") AS T7 ON T0."WhsCode" = T7."WhsCode") AS TFOREIGN ON TLOCAL."WhsCode" = TFOREIGN."WhsCode" ORDER BY "value" DESC ) T ORDER BY "code";
end if;
end;


