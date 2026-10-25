CREATE PROCEDURE "_TmSp_BinLocationListCRPrint"
(
	IN type integer, 
	IN binSublevels nvarchar(500), 
	IN binAttributes nvarchar(500), 
	IN binCodeFromTo nvarchar(500), 
	IN itemCodeFromTo nvarchar(110), 
	IN itemGroups nvarchar(5000), 
	IN batchFromTo nvarchar(300), 
	IN serialFromTo nvarchar(300), 
	IN binAbsSet nvarchar(5000), 
	IN batchAbs nvarchar(100), 
	IN serialAbs nvarchar(100), 
	IN whsCodeFromTo nvarchar(100), 
	IN flag integer
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	BIN_SUBLEVEL_LENGTH integer := 50;
	BIN_ATTR_LENGTH integer := 20;
	BIN_LOC_CODE_LENGTH integer := 228;
	ITEM_CODE_LENGTH integer := 50;
	SNB_NUMBER_LENGTH integer := 36;
	SNB_ATTR_LENGTH integer := 32;
	WHS_CODE_LENGTH integer := 8;
	BIN_SUBLEVEL_COUNT integer := 4;
	BIN_ATTR_COUNT integer := 10;
	
	ITM_RTRICT_SPC_ITM nvarchar(1) := '1';
	ITM_RTRICT_SNG_ITM nvarchar(1) := '2';
	ITM_RTRICT_SPC_ITM_GROUP nvarchar(1) := '3';
	ITM_RTRICT_SNG_ITM_GROUP nvarchar(1) := '4';
	
	COMMA nvarchar(1) := ',';
	V_INT integer;
	V_START integer;
	V_LENGTH integer;
		
	T_SQL nclob;
	BinAbs nvarchar(10);
	displayInactiveBin nvarchar(1);
	
	binSbl1From nvarchar(50);
	binSbl1To nvarchar(50);
	binSbl2From nvarchar(50);
	binSbl2To nvarchar(50);
	binSbl3From nvarchar(50);
	binSbl3To nvarchar(50);		
	binSbl4From nvarchar(50);
	binSbl4To nvarchar(50);
		
	binAttr1From nvarchar(20);
	binAttr1To nvarchar(20);
	binAttr2From nvarchar(20);
	binAttr2To nvarchar(20);
	binAttr3From nvarchar(20);
	binAttr3To nvarchar(20);
	binAttr4From nvarchar(20);
	binAttr4To nvarchar(20);
	binAttr5From nvarchar(20);
	binAttr5To nvarchar(20);
	binAttr6From nvarchar(20);
	binAttr6To nvarchar(20);
	binAttr7From nvarchar(20);
	binAttr7To nvarchar(20);
	binAttr8From nvarchar(20);
	binAttr8To nvarchar(20);
	binAttr9From nvarchar(20);
	binAttr9To nvarchar(20);
	binAttr10From nvarchar(20);
	binAttr10To nvarchar(20);
		
	binLocCodeFrom nvarchar(228);
	binLocCodeTo nvarchar(228);
		
	itemCodeFrom nvarchar(50);
	itemCodeTo nvarchar(50);
		
	batchNumberFrom nvarchar(36);
	batchNumberTo nvarchar(36);
	batchAttr1From nvarchar(32);
	batchAttr1To nvarchar(32);
	batchAttr2From nvarchar(32);
	batchAttr2To nvarchar(32);
		
	serialNumberFrom nvarchar(36);
	serialNumberTo nvarchar(36);
	mfrSerialNumberFrom nvarchar(32);
	mfrSerialNumberTo nvarchar(32);
	lotNumberFrom nvarchar(32);
	lotNumberTo nvarchar(32);
		
	whsIncludingFrom nvarchar(8);
	whsIncludingTo nvarchar(8);
		
	whsExcludingFrom nvarchar(8);
	whsExcludingTo nvarchar(8);
	
	BIN_LOC_TABLE_BASE_FIELD nvarchar(5000);
	T_SQL_ITEM_RTRICT nvarchar(5000);
	T_SQL_BATCH_RTRICT nvarchar(5000);
	T_SQL_REPLENISH_QTY nvarchar(5000);
	
BEGIN
	IF :binSublevels <> '' THEN
		V_INT := 1;
		binSbl1From := RTRIM(SUBSTRING(:binSublevels, :V_INT, :BIN_SUBLEVEL_LENGTH));
		V_INT := :V_INT + :BIN_SUBLEVEL_LENGTH;
		binSbl1To := RTRIM(SUBSTRING(:binSublevels, :V_INT, :BIN_SUBLEVEL_LENGTH));
		V_INT := :V_INT + :BIN_SUBLEVEL_LENGTH;
		binSbl2From := RTRIM(SUBSTRING(:binSublevels, :V_INT, :BIN_SUBLEVEL_LENGTH));
		V_INT := :V_INT + :BIN_SUBLEVEL_LENGTH;
		binSbl2To := RTRIM(SUBSTRING(:binSublevels, :V_INT, :BIN_SUBLEVEL_LENGTH));
		V_INT := :V_INT + :BIN_SUBLEVEL_LENGTH;
		binSbl3From := RTRIM(SUBSTRING(:binSublevels, :V_INT, :BIN_SUBLEVEL_LENGTH));
		V_INT := :V_INT + :BIN_SUBLEVEL_LENGTH;
		binSbl3To := RTRIM(SUBSTRING(:binSublevels, :V_INT, :BIN_SUBLEVEL_LENGTH));
		V_INT := :V_INT + :BIN_SUBLEVEL_LENGTH;
		binSbl4From := RTRIM(SUBSTRING(:binSublevels, :V_INT, :BIN_SUBLEVEL_LENGTH));
		V_INT := :V_INT + :BIN_SUBLEVEL_LENGTH;
		binSbl4To := RTRIM(SUBSTRING(:binSublevels, :V_INT, :BIN_SUBLEVEL_LENGTH));
	END IF;
			
	IF :binAttributes <> '' THEN
		V_INT := 1;
		binAttr1From := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr1To := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr2From := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr2To := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr3From := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr3To := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr4From := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr4To := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr5From := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr5To := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr6From := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr6To := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr7From := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr7To := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr8From := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr8To := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr9From := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr9To := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr10From := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
		V_INT := :V_INT + :BIN_ATTR_LENGTH;
		binAttr10To := RTRIM(SUBSTRING(:binAttributes, :V_INT, :BIN_ATTR_LENGTH));
	END IF;
			
	IF :binCodeFromTo <> '' THEN
		V_INT := 1;
		binLocCodeFrom := RTRIM(SUBSTRING(:binCodeFromTo, :V_INT, :BIN_LOC_CODE_LENGTH));
		V_INT := :V_INT + :BIN_LOC_CODE_LENGTH;
		binLocCodeTo := RTRIM(SUBSTRING(:binCodeFromTo, :V_INT, :BIN_LOC_CODE_LENGTH));
	END IF;
			
	IF :itemCodeFromTo <> '' THEN
		V_INT := 1;
		itemCodeFrom := RTRIM(SUBSTRING(:itemCodeFromTo, :V_INT, :ITEM_CODE_LENGTH));
		V_INT := :V_INT + :ITEM_CODE_LENGTH;
		itemCodeTo := RTRIM(SUBSTRING(:itemCodeFromTo, :V_INT, :ITEM_CODE_LENGTH));
	END IF;
			
	IF batchFromTo <> '' THEN
		V_INT := 1;
		batchNumberFrom := RTRIM(SUBSTRING(:batchFromTo, :V_INT, :SNB_NUMBER_LENGTH));
		V_INT := :V_INT + :SNB_NUMBER_LENGTH;
		batchNumberTo := RTRIM(SUBSTRING(:batchFromTo, :V_INT, :SNB_NUMBER_LENGTH));
		V_INT := :V_INT + :SNB_NUMBER_LENGTH;
				
		batchAttr1From := RTRIM(SUBSTRING(:batchFromTo, :V_INT, :SNB_ATTR_LENGTH));
		V_INT := :V_INT + :SNB_ATTR_LENGTH;
		batchAttr1To := RTRIM(SUBSTRING(:batchFromTo, :V_INT, :SNB_ATTR_LENGTH));
		V_INT := :V_INT + :SNB_ATTR_LENGTH;
		batchAttr2From := RTRIM(SUBSTRING(:batchFromTo, :V_INT, :SNB_ATTR_LENGTH));
		V_INT := :V_INT + :SNB_ATTR_LENGTH;
		batchAttr2To := RTRIM(SUBSTRING(:batchFromTo, :V_INT, :SNB_ATTR_LENGTH));
	END IF;
			
	IF :serialFromTo <> '' THEN
		V_INT := 1;
		serialNumberFrom := RTRIM(SUBSTRING(:serialFromTo, :V_INT, :SNB_NUMBER_LENGTH));
		V_INT := :V_INT + :SNB_NUMBER_LENGTH;
		serialNumberTo := RTRIM(SUBSTRING(:serialFromTo, :V_INT, :SNB_NUMBER_LENGTH));
		V_INT := :V_INT + :SNB_NUMBER_LENGTH;
				
		mfrSerialNumberFrom := RTRIM(SUBSTRING(:serialFromTo, :V_INT, :SNB_ATTR_LENGTH));
		V_INT := :V_INT + :SNB_ATTR_LENGTH;
		mfrSerialNumberTo := RTRIM(SUBSTRING(:serialFromTo, :V_INT, :SNB_ATTR_LENGTH));
		V_INT := :V_INT + :SNB_ATTR_LENGTH;
		lotNumberFrom := RTRIM(SUBSTRING(:serialFromTo, :V_INT, :SNB_ATTR_LENGTH));
		V_INT := :V_INT + :SNB_ATTR_LENGTH;
		lotNumberTo := RTRIM(SUBSTRING(:serialFromTo, :V_INT, :SNB_ATTR_LENGTH));
	END IF;
			
	IF :whsCodeFromTo <> '' THEN
		V_INT := 1;
		whsIncludingFrom := RTRIM(SUBSTRING(:whsCodeFromTo, :V_INT, :WHS_CODE_LENGTH));
		V_INT := :V_INT + :WHS_CODE_LENGTH;
		whsIncludingTo := RTRIM(SUBSTRING(:whsCodeFromTo, :V_INT, :WHS_CODE_LENGTH));
		V_INT := :V_INT + :WHS_CODE_LENGTH;
		whsExcludingFrom := RTRIM(SUBSTRING(:whsCodeFromTo, :V_INT, :WHS_CODE_LENGTH));
		V_INT := :V_INT + :WHS_CODE_LENGTH;
		whsExcludingTo := RTRIM(SUBSTRING(:whsCodeFromTo, :V_INT, :WHS_CODE_LENGTH));
	END IF;
	
	--binAbsSet := LTRIM(RTRIM(:binAbsSet));
	IF :binAbsSet <> '' THEN
		V_START := 1;
		V_LENGTH := 1;
		
		select count (*) into V_INT from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#TMP_TABLE_BIN_ABS3' AND SCHEMA_NAME = CURRENT_SCHEMA AND CONNECTION_ID = CURRENT_CONNECTION;
		IF :V_INT > 0 THEN
			DROP TABLE #TMP_TABLE_BIN_ABS3;
		END IF;
		
		V_INT := 0;
		CREATE LOCAL TEMPORARY TABLE #TMP_TABLE_BIN_ABS3 ("BinAbs" INTEGER);
		DELETE FROM #TMP_TABLE_BIN_ABS3 WHERE 1=1;
		V_INT := LOCATE(:binAbsSet, :COMMA);
					
		WHILE :V_INT <> 0 DO
			V_LENGTH := :V_INT - :V_START;
			BinAbs := SUBSTRING(:binAbsSet, :V_START, :V_LENGTH);
						
			INSERT INTO #TMP_TABLE_BIN_ABS3 VALUES(:BinAbs);
						
			V_START := :V_INT + 1;
			V_INT := LOCATE(:binAbsSet, :COMMA, :V_START);
		END WHILE;

		IF :V_INT = 0 THEN
			V_LENGTH := LENGTH(:binAbsSet) + 1 - :V_START;
			BinAbs := SUBSTRING(:binAbsSet, :V_START, :V_LENGTH);
			INSERT INTO #TMP_TABLE_BIN_ABS3 VALUES(:BinAbs);
		END IF;		

	END IF;
		
	BIN_LOC_TABLE_BASE_FIELD := 
	'"OBIN"."BinCode", "OBIN"."WhsCode", "OBIN"."SL1Code", "OBIN"."SL2Code", "OBIN"."SL3Code",
	"OBIN"."SL4Code", "OBIN"."Disabled", "OBIN"."ReceiveBin", "OBIN"."Descr", "OBIN"."AltSortCod", "OBIN"."BarCode",
	CASE "OBIN"."MinLevel" WHEN 0 THEN NULL ELSE "OBIN"."MinLevel" END AS "MinLevel",
	CASE "OBIN"."MaxLevel" WHEN 0 THEN NULL ELSE "OBIN"."MaxLevel" END AS "MaxLevel",
	"OBIN"."Attr1Val", "OBIN"."Attr2Val", "OBIN"."Attr3Val", "OBIN"."Attr4Val", "OBIN"."Attr5Val",
	"OBIN"."Attr6Val", "OBIN"."Attr7Val", "OBIN"."Attr8Val", "OBIN"."Attr9Val", "OBIN"."Attr10Val", 
	"OBIN"."RtrictType", "OBIN"."RtrictResn", "OBIN"."RtrictDate", ';

	T_SQL_ITEM_RTRICT :=
	' (SELECT "OBIN"."AbsEntry" AS "BinAbs", "OBIN"."SpcItmCode" AS "RestrictedTo" FROM 
	"OBIN" WHERE "OBIN"."ItmRtrictT" = ' || :ITM_RTRICT_SPC_ITM 
	|| ' UNION ALL ' || 
	' SELECT "OIBQ"."BinAbs" AS "BinAbs", "OIBQ"."ItemCode" AS "RestrictedTo" FROM 
	"OIBQ" WHERE "OIBQ"."BinAbs" IN (SELECT "AbsEntry" FROM "OBIN" WHERE
	"OBIN"."ItmRtrictT" = ' || :ITM_RTRICT_SNG_ITM || ') AND "OIBQ"."OnHandQty" > 0 GROUP BY
	"OIBQ"."BinAbs", "OIBQ"."ItemCode" HAVING COUNT("OIBQ"."BinAbs") < 2 '
	|| ' UNION ALL ' || 
	' SELECT "OBIN"."AbsEntry" AS "BinAbs", "OITB"."ItmsGrpNam" AS "RestrictedTo" FROM
	"OBIN" INNER JOIN "OITB" ON "OBIN"."SpcItmGrpC" = "OITB"."ItmsGrpCod"
	WHERE "OBIN"."ItmRtrictT" = ' || :ITM_RTRICT_SPC_ITM_GROUP
	|| ' UNION ALL ' ||
	' SELECT "OIBQ"."BinAbs" AS "BinAbs", MAX("OITB"."ItmsGrpNam")
	AS "RestrictedTo" FROM "OIBQ" INNER JOIN "OITM" ON 
	"OIBQ"."ItemCode" = "OITM"."ItemCode" INNER JOIN "OITB" ON 
	"OITM"."ItmsGrpCod" = "OITB"."ItmsGrpCod" WHERE  
	"OIBQ"."BinAbs" IN (SELECT "AbsEntry" FROM "OBIN" WHERE "OBIN"."ItmRtrictT" = '
	|| :ITM_RTRICT_SNG_ITM_GROUP || ') AND "OIBQ"."OnHandQty" > 0 GROUP BY "OIBQ"."BinAbs", 
	"OITM"."ItmsGrpCod" HAVING COUNT("OIBQ"."BinAbs") < 2) AS "ITM_RTRICT" ';
	
	T_SQL_BATCH_RTRICT :=
	' (SELECT "OBBQ"."BinAbs" AS "BinAbs", Min("OBTN"."DistNumber") AS "Batch" FROM 
	"OBBQ" INNER JOIN "OBTN" ON "OBBQ"."SnBMDAbs" = "OBTN"."AbsEntry" WHERE 
	"OBBQ"."BinAbs" IN (SELECT "AbsEntry" FROM "OBIN" WHERE "OBIN"."SngBatch" = ''Y'') AND  
	"OBBQ"."OnHandQty" <> 0 GROUP BY "OBBQ"."BinAbs", "OBBQ"."SnBMDAbs" HAVING COUNT(*) < 2) AS "BATCH_RTRICT" ';
	
	T_SQL_REPLENISH_QTY :=
	' (SELECT "OIBQ"."BinAbs", SUM("OIBQ"."OnHandQty") AS "ReplenishOnHandQty" FROM "OIBQ"
	INNER JOIN "OBIN" ON "OIBQ"."BinAbs" = "OBIN"."AbsEntry" AND "OIBQ"."ItemCode" = 
	"OBIN"."SpcItmCode" AND "OBIN"."ItmRtrictT" = ' || :ITM_RTRICT_SPC_ITM ||
	' WHERE "OIBQ"."OnHandQty" <> 0 GROUP BY "OIBQ"."BinAbs" '
	|| ' UNION ALL ' ||
	' SELECT "OIBQ"."BinAbs", SUM("OIBQ"."OnHandQty") AS "ReplenishOnHandQty" FROM "OIBQ"
	WHERE "OIBQ"."OnHandQty" > 0 AND "OIBQ"."BinAbs" IN (SELECT "AbsEntry" FROM "OBIN"
	WHERE "OBIN"."ItmRtrictT" = ' || :ITM_RTRICT_SNG_ITM  || ') GROUP BY "OIBQ"."BinAbs" ) AS REPLENISH_QTY ';
	
	IF BITAND(:flag,4) = 4 THEN
		displayInactiveBin := 'Y';
	ELSE
		displayInactiveBin := 'N';
	END IF;
	
	T_SQL :=
	'SELECT ' || :BIN_LOC_TABLE_BASE_FIELD ||
		' "OBIN"."ItmRtrictT", "ITM_RTRICT"."RestrictedTo", "OBIN"."SngBatch", "BATCH_RTRICT"."Batch", 
		CASE "B"."ItemCount" WHEN 0 THEN NULL ELSE "B"."ItemCount" END AS "ItemCount",
		CASE "B"."OnHandQty" WHEN 0 THEN NULL ELSE "B"."OnHandQty" END AS "OnHandQty",
		CASE "B"."SnBCount" WHEN 0 THEN NULL ELSE "B"."SnBCount" END AS "SnBCount",
		CASE WHEN "B"."ItmCountNeg" = 1 AND "OBIN"."ItmRtrictT" <> ' || :ITM_RTRICT_SPC_ITM ||
		' THEN CASE WHEN "OBIN"."MinLevel" <> IFNULL("B"."OnHandQty", 0) THEN "OBIN"."MinLevel" - IFNULL("B"."OnHandQty", 0) 
		ELSE NULL END WHEN "OBIN"."ItmRtrictT" = ' || :ITM_RTRICT_SPC_ITM || ' AND "OBIN"."Disabled" = ''N'' THEN
		CASE WHEN "OBIN"."MinLevel" <> IFNULL("REPLENISH_QTY"."ReplenishOnHandQty", 0) THEN 
		"OBIN"."MinLevel" - IFNULL("REPLENISH_QTY"."ReplenishOnHandQty", 0) ELSE NULL END
		WHEN "B"."ItmCountNeg" <> 1 AND "OBIN"."ItmRtrictT" = ' || :ITM_RTRICT_SNG_ITM || 
		' THEN CASE WHEN "OBIN"."MinLevel" <> "REPLENISH_QTY"."ReplenishOnHandQty" THEN
		"OBIN"."MinLevel" - "REPLENISH_QTY"."ReplenishOnHandQty" ELSE NULL END ELSE NULL END AS "QtyBelowMin",
		CASE WHEN "B"."ItmCountNeg" = 1 AND "OBIN"."ItmRtrictT" <> ' || :ITM_RTRICT_SPC_ITM || 
		' AND "OBIN"."MaxLevel" <> 0 AND "OBIN"."MaxLevel" > IFNULL("B"."OnHandQty", 0)
		THEN ("OBIN"."MaxLevel" - IFNULL("B"."OnHandQty", 0)) WHEN "OBIN"."ItmRtrictT" = ' || :ITM_RTRICT_SPC_ITM ||
		' AND "OBIN"."Disabled" = ''N'' AND "OBIN"."MaxLevel" <> 0 AND "OBIN"."MaxLevel" > 
		IFNULL("REPLENISH_QTY"."ReplenishOnHandQty", 0) THEN "OBIN"."MaxLevel" - IFNULL("REPLENISH_QTY"."ReplenishOnHandQty", 0)
		WHEN "B"."ItmCountNeg" <> 1 AND "OBIN"."ItmRtrictT" = ' || :ITM_RTRICT_SNG_ITM ||
		' AND "OBIN"."MaxLevel" <> 0 AND "OBIN"."MaxLevel" > "REPLENISH_QTY"."ReplenishOnHandQty"
		THEN "OBIN"."MaxLevel" - "REPLENISH_QTY"."ReplenishOnHandQty" ELSE NULL END AS ReplenishmentQty 
	FROM OBIN LEFT OUTER JOIN
	(SELECT	MAX("A"."BinAbs") AS "BinAbs", SUM("A"."ItemCount") AS "ItemCount",
		SUM("A"."OnHandQty") AS "OnHandQty", SUM("A"."SnBCount") AS "SnBCount",
		SUM("A"."ItmCountNeg") AS "ItmCountNeg" FROM 
		(SELECT "OIBQ"."BinAbs"	AS "BinAbs", Count(DISTINCT "OIBQ"."ItemCode") AS "ItemCount", 
			Max("OIBQ"."OnHandQty")	AS "OnHandQty", Count(DISTINCT "OBBQ"."SnBMDAbs") + Count(DISTINCT "OSBQ"."SnBMDAbs") AS "SnBCount",  
			Count(DISTINCT "OIBQ"."ItemCode") AS "ItmCountNeg"
		FROM "OIBQ" LEFT OUTER JOIN "OBBQ" ON  "OIBQ"."BinAbs" = "OBBQ"."BinAbs" AND "OIBQ"."ItemCode" = "OBBQ"."ItemCode" AND "OBBQ"."OnHandQty" <> 0
			LEFT OUTER JOIN "OSBQ" ON "OIBQ"."BinAbs" = "OSBQ"."BinAbs" AND "OIBQ"."ItemCode" = "OSBQ"."ItemCode" AND "OSBQ"."OnHandQty" <> 0
		WHERE "OIBQ"."OnHandQty" > 0 GROUP BY "OIBQ"."BinAbs", "OIBQ"."AbsEntry"
		UNION ALL
		SELECT "OIBQ"."BinAbs" AS "BinAbs", 0 AS "ItemCount", Max("OIBQ"."OnHandQty") AS "OnHandQty", 
			Count(DISTINCT "OBBQ"."SnBMDAbs") + Count(DISTINCT "OSBQ"."SnBMDAbs") AS "SnBCount", 
			Count("OIBQ"."ItemCode") AS "ItmContNeg" 
		FROM "OIBQ" LEFT OUTER JOIN "OBBQ" ON "OIBQ"."BinAbs" = "OBBQ"."BinAbs" AND "OIBQ"."ItemCode" = "OBBQ"."ItemCode" AND "OBBQ"."OnHandQty" <> 0 
			LEFT OUTER JOIN "OSBQ" ON "OIBQ"."BinAbs" = "OSBQ"."BinAbs" AND "OIBQ"."ItemCode" = "OSBQ"."ItemCode" AND "OSBQ"."OnHandQty" <> 0
		WHERE "OIBQ"."OnHandQty" < 0 GROUP BY "OIBQ"."BinAbs", "OIBQ"."AbsEntry") AS "A" GROUP BY "A"."BinAbs") AS "B" ON "OBIN"."AbsEntry" = "B"."BinAbs"
	LEFT OUTER JOIN ' || :T_SQL_ITEM_RTRICT || ' ON "OBIN"."AbsEntry" = "ITM_RTRICT"."BinAbs"
	LEFT OUTER JOIN ' || :T_SQL_BATCH_RTRICT || ' ON "OBIN"."AbsEntry" = "BATCH_RTRICT"."BinAbs"
	LEFT OUTER JOIN ' || :T_SQL_REPLENISH_QTY || ' ON "OBIN"."AbsEntry" = "REPLENISH_QTY"."BinAbs"
	WHERE "OBIN"."AbsEntry" > 0 ';
	--AND "OBIN"."WhsCode" IN (SELECT "WhsCode" FROM "OWHS" WHERE "Inactive" = ''N'') ';
	
	IF :binAbsSet <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."AbsEntry" IN (SELECT "BinAbs" FROM #TMP_TABLE_BIN_ABS3) ';
	END IF;
	
	IF :whsIncludingFrom <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."WhsCode" >= ''' || :whsIncludingFrom || '''';
	END IF;
		
	IF :whsIncludingTo <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."WhsCode" <= ''' || :whsIncludingTo || '''';
	END IF;
	
	IF :whsExcludingFrom <> '' OR  :whsExcludingTo <> '' THEN
		T_SQL := :T_SQL || ' AND ( ';
		IF :whsExcludingFrom <> '' THEN
			T_SQL := :T_SQL || ' "OBIN"."WhsCode" < ''' || :whsExcludingFrom || '''';
		END IF;
	
		IF :whsExcludingTo <> '' THEN
			IF :whsExcludingFrom <> '' THEN
				T_SQL := :T_SQL || ' OR ';
			ELSE
				T_SQL := :T_SQL || ' "OBIN"."WhsCode" > ''' || :whsExcludingTo || '''';
			END IF;
		END IF;
		T_SQL := :T_SQL || ' ) ';
	END IF;
	
	IF :binSbl1From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."SL1Code" >= ''' || :binSbl1From || '''';
	END IF;
	IF :binSbl1To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."SL1Code" <= ''' || :binSbl1To || '''';
	END IF;
	
	IF :binSbl2From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."SL2Code" >= ''' || :binSbl2From || '''';
	END IF;
	IF :binSbl2To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."SL2Code" <= ''' || :binSbl2To || '''';
	END IF;
	
	IF :binSbl3From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."SL3Code" >= ''' || :binSbl3From || '''';
	END IF;
	IF :binSbl3To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."SL3Code" <= ''' || :binSbl3To || '''';
	END IF;
	
	IF :binSbl4From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."SL4Code" >= ''' || :binSbl4From || '''';
	END IF;
	IF :binSbl4To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."SL4Code" <= ''' || :binSbl4To || '''';
	END IF;
	
	IF :binAttr1From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr1Val" >= ''' || :binAttr1From || '''';
	END IF;
	IF :binAttr1To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr1Val" <= ''' || :binAttr1To || '''';
	END IF;

	IF :binAttr2From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr2Val" >= ''' || :binAttr2From || '''';
	END IF;
	IF :binAttr2To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr2Val" <= ''' || :binAttr2To || '''';
	END IF;
	
	IF :binAttr3From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr3Val" >= ''' || :binAttr3From || '''';
	END IF;
	IF :binAttr3To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr3Val" <= ''' || :binAttr3To || '''';
	END IF;
	
	IF :binAttr4From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr4Val" >= ''' || :binAttr4From || '''';
	END IF;
	IF :binAttr4To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr4Val" <= ''' || :binAttr4To || '''';
	END IF;

	IF :binAttr5From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr5Val" >= ''' || :binAttr5From || '''';
	END IF;
	IF :binAttr5To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr5Val" <= ''' || :binAttr5To || '''';
	END IF;
	
	IF :binAttr6From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr6Val" >= ''' || :binAttr6From || '''';
	END IF;
	IF :binAttr6To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr6Val" <= ''' || :binAttr6To || '''';
	END IF;
	
	IF :binAttr7From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr7Val" >= ''' || :binAttr7From || '''';
	END IF;
	IF :binAttr7To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr7Val" <= ''' || :binAttr7To || '''';
	END IF;
	
	IF :binAttr8From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr8Val" >= ''' || :binAttr8From || '''';
	END IF;
	IF :binAttr8To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr8Val" <= ''' || :binAttr8To || '''';
	END IF;
	
	IF :binAttr9From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr9Val" >= ''' || :binAttr9From || '''';
	END IF;
	IF :binAttr9To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr9Val" <= ''' || :binAttr9To || '''';
	END IF;
	
	IF :binAttr10From <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr10Val" >= ''' || :binAttr10From || '''';
	END IF;
	IF :binAttr10To <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Attr10Val" <= ''' || :binAttr10To || '''';
	END IF;
	
	IF :binLocCodeFrom <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."BinCode" >= ''' || :binLocCodeFrom || '''';
	END IF;
	IF :binLocCodeTo <> '' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."BinCode" <= ''' || :binLocCodeTo || '''';
	END IF;
	
	IF :displayInactiveBin = 'N' THEN
		T_SQL := :T_SQL || ' AND "OBIN"."Disabled" = ''N'' ';
	END IF;
	
	T_SQL := :T_SQL || 'ORDER BY "OBIN"."BinCode" ';
	
	--SELECT :T_SQL FROM DUMMY;
	EXECUTE IMMEDIATE :T_SQL;
	
END;


