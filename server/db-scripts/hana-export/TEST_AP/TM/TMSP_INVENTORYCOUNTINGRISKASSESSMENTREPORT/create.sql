-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_InventoryCountingRiskAssessmentReport ( 
IN priceListName char(32), 
IN riskcode nvarchar(1),
IN ItemPara1Min nvarChar(32),
IN ItemPara1Max nvarChar(32),
IN ItemPara2Min nvarChar(32),
IN ItemPara2Max nvarChar(32),
IN ItemPara3Min nvarChar(32),
IN ItemPara3Max nvarChar(32),
IN WhsParaMin nvarChar(32),
IN WhsParaMax nvarChar(32))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	LocalCurrency nvarChar(3);
	IsPricePerWH nvarchar(1);
	High nvarchar(1);
	Medium nvarchar(1);
	Low nvarchar(1);
	DaysMin DECIMAL(21,6);
	DaysMax DECIMAL(21,6);
	VolumeMin DECIMAL(21,6);
	VolumeMax DECIMAL(21,6);
	ValueMin DECIMAL(21,6);
	ValueMax DECIMAL(21,6);
	HRiskPercentMin DECIMAL(21,6);
	HRiskPercentMax DECIMAL(21,6);
BEGIN
	SELECT "MainCurncy" into LocalCurrency FROM OADM;
	SELECT "PriceSys" into IsPricePerWH FROM OADM;

	-- Create temporary table to store data to
	CREATE LOCAL TEMPORARY 
	TABLE "#tblRiskTempData" ("ItemCode" nvarChar(50), 
		"ItemName" nvarChar(200),
		"WhsCode"	nvarChar(8),
		"DaysSinceLastCount" INT,
		"Risk1" nvarChar(10),
		"TransVolume" DECIMAL(21,6),
		"Risk2" nvarChar(10),
		"ValueOfItem" DECIMAL(21,6),
		"Risk3" nvarChar(10),
		"CalcRisk" nvarChar(10));
	
	delete from "#tblRiskTempData" where 1 = 1;
		
	High := 'H';
	Medium := 'M';
	Low := 'L';
	DaysMin := CAST(:ItemPara1Min AS DECIMAL(21,6));
	DaysMax := CAST(:ItemPara1Max AS DECIMAL(21,6));
	VolumeMin := CAST(:ItemPara2Min AS DECIMAL(21,6));
	VolumeMax := CAST(:ItemPara2Max AS DECIMAL(21,6));
	ValueMin := CAST(:ItemPara3Min AS DECIMAL(21,6));
	ValueMax := CAST(:ItemPara3Max AS DECIMAL(21,6));
	HRiskPercentMin := CAST(:WhsParaMin AS DECIMAL(21,6));
	HRiskPercentMax := CAST(:WhsParaMax AS DECIMAL(21,6));
	
	IF :priceListName = 'Last Evaluated Price' OR :priceListName = 'Last Purchase Price' OR :priceListName = 'Item Cost' THEN
	insert into "#tblRiskTempData"--("ItemCode", "ItemName", "WhsCode", "DaysSinceLastCount", "TransVolume", "ValueOfItem")
		SELECT TMIDDLE."ItemCode", TMIDDLE."ItemName", TMIDDLE."WhsCode", TMIDDLE."DaysSinceLastCount",
			(CASE WHEN TMIDDLE."DaysSinceLastCount" > :DaysMax  THEN  :High 
							WHEN TMIDDLE."DaysSinceLastCount" < :DaysMin THEN :Low 
							ELSE :Medium END) AS "Risk1",
			TMIDDLE."TransVolume",
			(CASE WHEN TMIDDLE."TransVolume" >  :VolumeMax  THEN :High 
							 WHEN TMIDDLE."TransVolume" < :VolumeMin  THEN :Low 
							 ELSE :Medium  END) AS "Risk2",
			TMIDDLE."ValueOfItem",
			(CASE WHEN TMIDDLE."ValueOfItem" > :ValueMax  THEN :High 
							WHEN TMIDDLE."ValueOfItem" < :ValueMin THEN :Low 
							ELSE :Medium END) AS "Risk3",
			(CASE  WHEN (TMIDDLE."DaysSinceLastCount" > :DaysMax ) AND ((TMIDDLE."TransVolume" >  :VolumeMax ) OR (TMIDDLE."ValueOfItem" > :ValueMax)) THEN :High 
				 WHEN TMIDDLE."DaysSinceLastCount" < :DaysMin  AND TMIDDLE."TransVolume" < :VolumeMin  AND TMIDDLE."ValueOfItem" < :ValueMin  THEN :Low 
				 ELSE :Medium END) As "CalcRisk" 
		FROM		
		(SELECT T2."ItemCode", T3."ItemName", T2."WhsCode", 
			IFNULL(DAYS_BETWEEN("LastDate", Now()),0) AS "DaysSinceLastCount",
			IFNULL((select sum("InQty" + "OutQty") from OIVL T4 WHERE T4."LocCode" = T2."WhsCode" AND T4."ItemCode" = T2."ItemCode" 
				AND (T4."CreateDate" > T2."LastDate" OR (T4."CreateDate" = T2."LastDate" AND T4."CreateTime" > T2."LastTime"))), 0) AS "TransVolume",
			(CASE WHEN :priceListName = 'Last Evaluated Price' THEN T3."LstEvlPric"
				WHEN :priceListName = 'Last Purchase Price' THEN
					(CASE WHEN "LastPurCur" = :LocalCurrency THEN "LastPurPrc" 
						ELSE "LastPurPrc" * (
							IFNULL(
								CASE WHEN DAYS_BETWEEN(ORTT."RateDate",Now()) = 0 THEN ORTT."Rate"
									ELSE NULL END, 0)) 
					END)
				WHEN :priceListName = 'Item Cost' THEN
					(CASE WHEN "EvalSystem" = 'F' AND 
							( CASE WHEN (SELECT sum(T5."OpenQty") FROM OIVQ T5 WHERE T2."WhsCode" = T5."LocCode" AND T2."ItemCode" = T5."ItemCode") = 0 THEN 0
									ELSE (SELECT sum(T6."OpenValue")/sum(T6."OpenQty") FROM OIVQ T6 WHERE T2."WhsCode" = T6."LocCode" AND T2."ItemCode" = T6."ItemCode")	END) > 0
								THEN (SELECT sum(T7."OpenValue")/sum(T7."OpenQty") FROM OIVQ T7 WHERE T2."WhsCode" = T7."LocCode" AND T2."ItemCode" = T7."ItemCode")											
						ELSE (CASE WHEN :IsPricePerWH ='Y' THEN T2."AvgPrice"
								ELSE T3."AvgPrice" END) END)
				END) AS "ValueOfItem" 						
		FROM
			(SELECT T1."ItemCode", T1."WhsCode", T1."AvgPrice",
			 CASE WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") < 0 THEN T1."LastCountDate"
				WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") > 0  THEN T1."LastPostDate"
				WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") IS NULL THEN 
					IFNULL(IFNULL("LastCountDate", "LastPostDate"), T1."CreateDate") END AS "LastDate",
			CASE WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") < 0 THEN T1."LastCountTime"
				WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") > 0  THEN T1."LastPostTime"
				WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") IS NULL THEN 
					IFNULL(IFNULL("LastCountTime", "LastPostTime"), 0) END AS "LastTime"
			FROM (SELECT T0."ItemCode" AS "ItemCode", T0."WhsCode" AS "WhsCode", T0."createDate" AS "CreateDate", T0."AvgPrice" AS "AvgPrice",
					(SELECT MAX("CountDate") FROM INC1 WHERE INC1."WhsCode" = T0."WhsCode" AND INC1."ItemCode" = T0."ItemCode") AS "LastCountDate",
					(SELECT MAX("CountTime") FROM INC1 WHERE INC1."WhsCode" = T0."WhsCode" AND INC1."ItemCode" = T0."ItemCode" 
						AND INC1."CountDate" = (SELECT MAX("CountDate") FROM INC1 WHERE INC1."WhsCode" = T0."WhsCode" AND INC1."ItemCode" = T0."ItemCode")) AS "LastCountTime",
					(SELECT MAX("CountDate") FROM IQR1 WHERE IQR1."WhsCode" = T0."WhsCode" AND IQR1."ItemCode" = T0."ItemCode") AS "LastPostDate",
					(SELECT MAX("CountTime") FROM IQR1 WHERE IQR1."WhsCode" = T0."WhsCode" AND IQR1."ItemCode" = T0."ItemCode" 
						AND IQR1."CountDate" = (SELECT MAX("CountDate") FROM IQR1 WHERE IQR1."WhsCode" = T0."WhsCode" AND IQR1."ItemCode" = T0."ItemCode")) AS "LastPostTime"
				FROM OITW T0) T1) AS T2 
			INNER JOIN OITM T3 ON T2."ItemCode" = T3."ItemCode"
			LEFT JOIN ORTT ON T3."LastPurCur" = ORTT."Currency"
			WHERE EXISTS(SELECT "ItemCode" FROM OIVL WHERE OIVL."ItemCode"=T2."ItemCode" AND OIVL."LocCode"=T2."WhsCode")) AS TMIDDLE;
	ELSE
		insert into "#tblRiskTempData"--("ItemCode", "ItemName", "WhsCode", "DaysSinceLastCount", "TransVolume", "ValueOfItem")
		SELECT TMIDDLE."ItemCode", TMIDDLE."ItemName", TMIDDLE."WhsCode", TMIDDLE."DaysSinceLastCount",
			(CASE WHEN TMIDDLE."DaysSinceLastCount" > :DaysMax  THEN  :High 
							WHEN TMIDDLE."DaysSinceLastCount" < :DaysMin THEN :Low 
							ELSE :Medium END) AS "Risk1",
			TMIDDLE."TransVolume",
			(CASE WHEN TMIDDLE."TransVolume" >  :VolumeMax  THEN :High 
							 WHEN TMIDDLE."TransVolume" < :VolumeMin  THEN :Low 
							 ELSE :Medium  END) AS "Risk2",
			TMIDDLE."ValueOfItem",
			(CASE WHEN TMIDDLE."ValueOfItem" > :ValueMax  THEN :High 
							WHEN TMIDDLE."ValueOfItem" < :ValueMin THEN :Low 
							ELSE :Medium END) AS "Risk3",
			(CASE  WHEN (TMIDDLE."DaysSinceLastCount" > :DaysMax ) AND ((TMIDDLE."TransVolume" > :VolumeMax ) OR (TMIDDLE."ValueOfItem" > :ValueMax)) THEN :High 
				 WHEN TMIDDLE."DaysSinceLastCount" < :DaysMin  AND TMIDDLE."TransVolume" < :VolumeMin  AND TMIDDLE."ValueOfItem" < :ValueMin  THEN :Low 
				 ELSE :Medium END) As "CalcRisk" 
		FROM		
		(SELECT T2."ItemCode", T3."ItemName", T2."WhsCode", 
			IFNULL(DAYS_BETWEEN("LastDate", Now()),0) AS "DaysSinceLastCount",
			IFNULL((select sum("InQty" + "OutQty") from OIVL T4 WHERE T4."LocCode" = T2."WhsCode" AND T4."ItemCode" = T2."ItemCode" 
				AND (T4."CreateDate" > T2."LastDate" OR (T4."CreateDate" = T2."LastDate" AND T4."CreateTime" > T2."LastTime"))), 0) AS "TransVolume",
			(CASE WHEN ITM1."Currency" = :LocalCurrency THEN IFNULL(ITM1."Price", 0) 
				ELSE ITM1."Price" * (
						IFNULL(
							CASE WHEN DAYS_BETWEEN(T4."RateDate", Now()) = 0 THEN T4."Rate"
						ELSE NULL END, 0)) END) AS "ValueOfItem" 						
		FROM
			(SELECT T1."ItemCode", T1."WhsCode", T1."AvgPrice",
			 CASE WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") < 0 THEN T1."LastCountDate"
				WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") > 0  THEN T1."LastPostDate"
				WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") IS NULL THEN 
					IFNULL(IFNULL("LastCountDate", "LastPostDate"), T1."CreateDate") END AS "LastDate",
			CASE WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") < 0 THEN T1."LastCountTime"
				WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") > 0  THEN T1."LastPostTime"
				WHEN DAYS_BETWEEN(T1."LastCountDate", T1."LastPostDate") IS NULL THEN 
					IFNULL(IFNULL("LastCountTime", "LastPostTime"), 0) END AS "LastTime"
			FROM (SELECT T0."ItemCode" AS "ItemCode", T0."WhsCode" AS "WhsCode", T0."createDate" AS "CreateDate", T0."AvgPrice" AS "AvgPrice",
					(SELECT MAX("CountDate") FROM INC1 WHERE INC1."WhsCode" = T0."WhsCode" AND INC1."ItemCode" = T0."ItemCode") AS "LastCountDate",
					(SELECT MAX("CountTime") FROM INC1 WHERE INC1."WhsCode" = T0."WhsCode" AND INC1."ItemCode" = T0."ItemCode" 
						AND INC1."CountDate" = (SELECT MAX("CountDate") FROM INC1 WHERE INC1."WhsCode" = T0."WhsCode" AND INC1."ItemCode" = T0."ItemCode")) AS "LastCountTime",
					(SELECT MAX("CountDate") FROM IQR1 WHERE IQR1."WhsCode" = T0."WhsCode" AND IQR1."ItemCode" = T0."ItemCode") AS "LastPostDate",
					(SELECT MAX("CountTime") FROM IQR1 WHERE IQR1."WhsCode" = T0."WhsCode" AND IQR1."ItemCode" = T0."ItemCode" 
						AND IQR1."CountDate" = (SELECT MAX("CountDate") FROM IQR1 WHERE IQR1."WhsCode" = T0."WhsCode" AND IQR1."ItemCode" = T0."ItemCode")) AS "LastPostTime"
				FROM OITW T0) T1) AS T2 
			INNER JOIN OITM T3 ON T2."ItemCode" = T3."ItemCode"
			INNER JOIN ITM1 ON T3."ItemCode" = ITM1."ItemCode" 
			INNER JOIN OPLN ON ITM1."PriceList" = OPLN."ListNum" AND OPLN."ListName" = :priceListName 
			LEFT JOIN ORTT T4 ON T4."Currency" = ITM1."Currency"
			WHERE EXISTS(SELECT "ItemCode" FROM OIVL WHERE OIVL."ItemCode"=T2."ItemCode" AND OIVL."LocCode"=T2."WhsCode")) AS TMIDDLE;
	END IF;
	
					
	IF :riskcode = :High THEN
		SELECT "#tblRiskTempData"."ItemCode", "#tblRiskTempData"."ItemName",
		 	"#tblRiskTempData"."WhsCode", "#tblRiskTempData"."DaysSinceLastCount","#tblRiskTempData"."Risk1",
		 	"#tblRiskTempData"."TransVolume", "#tblRiskTempData"."Risk2", "#tblRiskTempData"."ValueOfItem",
		 	"#tblRiskTempData"."Risk3", "#tblRiskTempData"."CalcRisk", "OBPL"."BPLName", "OBPL"."BPLId" FROM "#tblRiskTempData"
		 	INNER JOIN "OWHS" ON "#tblRiskTempData"."WhsCode" = "OWHS"."WhsCode"
			LEFT JOIN "OBPL" ON "OWHS"."BPLid" = "OBPL"."BPLId"
			WHERE "#tblRiskTempData"."WhsCode" IN(
				SELECT T0."WhsCode"  AS "WhsCode" FROM (
					(SELECT "WhsCode", count("ItemCode") AS TOTAL FROM "#tblRiskTempData" GROUP BY "WhsCode") T0
					LEFT JOIN			
					(SELECT "WhsCode", "CalcRisk", IFNULL(count("ItemCode"), 0)AS TOTALBYRISK FROM "#tblRiskTempData" 
						WHERE "CalcRisk" = :High 
						GROUP BY "WhsCode", "CalcRisk") T1
						ON T0."WhsCode" = T1."WhsCode" )
				WHERE (T0.TOTAL <> 0 AND TO_DOUBLE(IFNULL(T1.TOTALBYRISK, 0))/ T0.TOTAL > :HRiskPercentMax)
			)
			ORDER BY "#tblRiskTempData"."WhsCode", "#tblRiskTempData"."ItemCode";
	ELSEIF :riskcode = :Low THEN
		SELECT "#tblRiskTempData"."ItemCode", "#tblRiskTempData"."ItemName",
		 	"#tblRiskTempData"."WhsCode", "#tblRiskTempData"."DaysSinceLastCount","#tblRiskTempData"."Risk1",
		 	"#tblRiskTempData"."TransVolume", "#tblRiskTempData"."Risk2", "#tblRiskTempData"."ValueOfItem",
		 	"#tblRiskTempData"."Risk3", "#tblRiskTempData"."CalcRisk", "OBPL"."BPLName", "OBPL"."BPLId" FROM "#tblRiskTempData"
		 	INNER JOIN "OWHS" ON "#tblRiskTempData"."WhsCode" = "OWHS"."WhsCode"
			LEFT JOIN "OBPL" ON "OWHS"."BPLid" = "OBPL"."BPLId"
			WHERE "#tblRiskTempData"."WhsCode" IN(
				SELECT T0."WhsCode"  AS "WhsCode" FROM (
						(SELECT "WhsCode", COUNT("ItemCode") AS TOTAL FROM "#tblRiskTempData" GROUP BY "WhsCode") T0
						LEFT JOIN 
						(SELECT "WhsCode", "CalcRisk", IFNULL(COUNT("ItemCode"), 0) AS TOTALBYRISK FROM "#tblRiskTempData" 
							WHERE "CalcRisk" = :High 
							GROUP BY "WhsCode", "CalcRisk") T1
						ON T0."WhsCode" = T1."WhsCode")
					WHERE (TOTAL <> 0 AND TO_DOUBLE(IFNULL(T1.TOTALBYRISK, 0))/ TOTAL < :HRiskPercentMin) OR TOTAL = 0
			)
			ORDER BY "#tblRiskTempData"."WhsCode", "#tblRiskTempData"."ItemCode";
	
	ELSEIF :riskcode = :Medium THEN
		SELECT "#tblRiskTempData"."ItemCode", "#tblRiskTempData"."ItemName",
		 	"#tblRiskTempData"."WhsCode", "#tblRiskTempData"."DaysSinceLastCount","#tblRiskTempData"."Risk1",
		 	"#tblRiskTempData"."TransVolume", "#tblRiskTempData"."Risk2", "#tblRiskTempData"."ValueOfItem",
		 	"#tblRiskTempData"."Risk3", "#tblRiskTempData"."CalcRisk", "OBPL"."BPLName", "OBPL"."BPLId" FROM "#tblRiskTempData"
		 	INNER JOIN "OWHS" ON "#tblRiskTempData"."WhsCode" = "OWHS"."WhsCode"
			LEFT JOIN "OBPL" ON "OWHS"."BPLid" = "OBPL"."BPLId"
			WHERE "#tblRiskTempData"."WhsCode" IN(
				SELECT T0."WhsCode"  AS "WhsCode" FROM (
					(SELECT "WhsCode", count("ItemCode") AS TOTAL FROM "#tblRiskTempData" GROUP BY "WhsCode") T0
					LEFT JOIN			
					(SELECT "WhsCode", "CalcRisk", IFNULL(count("ItemCode"), 0)AS TOTALBYRISK FROM "#tblRiskTempData" 
						WHERE "CalcRisk" = :High 
						GROUP BY "WhsCode", "CalcRisk") T1
						ON T0."WhsCode" = T1."WhsCode" )
				WHERE (T0.TOTAL <> 0 AND TO_DOUBLE(IFNULL(T1.TOTALBYRISK, 0))/ T0.TOTAL >= :HRiskPercentMin AND TO_DOUBLE(IFNULL(T1.TOTALBYRISK, 0))/ T0.TOTAL <= :HRiskPercentMax)
			)
			ORDER BY "#tblRiskTempData"."WhsCode", "#tblRiskTempData"."ItemCode";
	END IF;
		
	DROP TABLE "#tblRiskTempData";
END;


