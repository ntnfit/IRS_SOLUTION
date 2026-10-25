CREATE PROCEDURE "_TmSp_BinLocationContentDetailListCRPrint"
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
	
	BIN_LOC_CONTENT_DETAIL_LIST integer := 3;
	
	COMMA nvarchar(1) := ',';
	V_INT integer;
	V_START integer;
	V_LENGTH integer;
		
	T_SQL nclob;
	BinAbs nvarchar(10);
	displayInactiveBin nvarchar(1);
	snbOnly nvarchar(1);
	includeNonBinWhs nvarchar(1);
	displayInactiveItem nvarchar(1);
	SNB_FILTER NVARCHAR(1);
	
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
	
	FROM_TABLE_SQL nvarchar(5000);
	BATCH_NOT_DEFINED_SQL nvarchar(5000);
	SERIAL_NOT_DEFINED_SQL nvarchar(5000);
	BATCH_DEFINED_SQL nvarchar(5000);
	SERIAL_DEFINED_SQL nvarchar(5000);
	NOT_SNB_SQL nvarchar(5000);
	SELECT_BASE_SQL_4_NON_BIN_WHS nvarchar(5000);
	FROM_TABLE_SQL_4_NON_BIN_WHS nvarchar(5000);
	BATCH_NOT_DEFINED_SQL_4_NON_BIN_WHS nvarchar(5000);
	SERIAL_NOT_DEFINED_SQL_4_NON_BIN_WHS nvarchar(5000);
	BATCH_DEFINED_SQL_4_NON_BIN_WHS nvarchar(5000);
	SERIAL_DEFINED_SQL_4_NON_BIN_WHS nvarchar(5000);
	NOT_SNB_SQL_4_NON_BIN_WHS nvarchar(5000);
	
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
		
		select count (*) into V_INT from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#TMP_TABLE_BIN_ABS' AND SCHEMA_NAME = CURRENT_SCHEMA AND CONNECTION_ID = CURRENT_CONNECTION;
		IF :V_INT > 0 THEN
			DROP TABLE #TMP_TABLE_BIN_ABS;
		END IF;
		
		V_INT := 0;
		CREATE LOCAL TEMPORARY TABLE #TMP_TABLE_BIN_ABS ("BinAbs" INTEGER);
		DELETE FROM #TMP_TABLE_BIN_ABS WHERE 1=1;
		V_INT := LOCATE(:binAbsSet, :COMMA);
					
		WHILE :V_INT <> 0 DO
			V_LENGTH := :V_INT - :V_START;
			BinAbs := SUBSTRING(:binAbsSet, :V_START, :V_LENGTH);
						
			INSERT INTO #TMP_TABLE_BIN_ABS VALUES(:BinAbs);
						
			V_START := :V_INT + 1;
			V_INT := LOCATE(:binAbsSet, :COMMA, :V_START);
		END WHILE;
		
		IF :V_INT = 0 THEN
			V_LENGTH := LENGTH(:binAbsSet) + 1 - :V_START;
			BinAbs := SUBSTRING(:binAbsSet, :V_START, :V_LENGTH);
			INSERT INTO #TMP_TABLE_BIN_ABS VALUES(:BinAbs);
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
	
	IF :type = :BIN_LOC_CONTENT_DETAIL_LIST THEN
		IF BITAND(:flag, 2) = 2 THEN
			snbOnly := 'Y';
		ELSE
			snbOnly := 'N';
		END IF;
		
		IF BITAND(:flag, 8) = 8 THEN
			includeNonBinWhs := 'Y';
		ELSE
			includeNonBinWhs := 'N';
		END IF;
		
		FROM_TABLE_SQL := 
		' FROM "OIBQ" LEFT OUTER JOIN "OBBQ" ON "OIBQ"."BinAbs" = "OBBQ"."BinAbs" AND "OIBQ"."ItemCode" = "OBBQ"."ItemCode" AND "OBBQ"."OnHandQty" <> 0 
		LEFT OUTER JOIN "OSBQ" ON "OIBQ"."BinAbs" = "OSBQ"."BinAbs" AND "OIBQ"."ItemCode" = "OSBQ"."ItemCode" AND "OSBQ"."OnHandQty" <> 0 
		LEFT OUTER JOIN "OBTN" ON "OBBQ"."SnBMDAbs" = "OBTN"."AbsEntry" AND "OBBQ"."ItemCode" = "OBTN"."ItemCode"
		LEFT OUTER JOIN "OSRN" ON "OSBQ"."SnBMDAbs" = "OSRN"."AbsEntry" AND "OSBQ"."ItemCode" = "OSRN"."ItemCode" ';
					
		BATCH_NOT_DEFINED_SQL :=
		' SELECT "OIBQ"."BinAbs", "OIBQ"."ItemCode", MAX("OIBQ"."OnHandQty") - SUM("OBBQ"."OnHandQty") AS "OnHandQty",
			NULL AS "BatchNumber", NULL AS "BatchAttr1", NULL AS "BatchAttr2", NULL AS "SerialNumber",
			NULL AS "MnfSerial", NULL AS "LotNumber", NULL AS "BatchAbs", NULL AS "SerialAbs" ' ||
			:FROM_TABLE_SQL ||
		' WHERE "OIBQ"."OnHandQty" <> 0 AND "OBBQ"."AbsEntry" IS NOT NULL
			GROUP BY "OIBQ"."BinAbs", "OIBQ"."ItemCode" 
			HAVING MAX("OIBQ"."OnHandQty") > SUM("OBBQ"."OnHandQty") ';
				
		SERIAL_NOT_DEFINED_SQL := 
		' SELECT "OIBQ"."BinAbs", "OIBQ"."ItemCode", MAX("OIBQ"."OnHandQty") - SUM("OSBQ"."OnHandQty") AS "OnHandQty",
			NULL AS "BatchNumber", NULL AS "BatchAttr1", NULL AS "BatchAttr2", NULL AS "SerialNumber", NULL AS "MnfSerial",
			NULL AS "LotNumber", NULL AS "BatchAbs", NULL AS "SerialAbs" '
		|| :FROM_TABLE_SQL ||
			' WHERE "OIBQ"."OnHandQty" <> 0 AND "OSBQ"."AbsEntry" IS NOT NULL
			GROUP BY "OIBQ"."BinAbs", "OIBQ"."ItemCode" 
			HAVING MAX("OIBQ"."OnHandQty") > SUM("OSBQ"."OnHandQty") ';
					
		BATCH_DEFINED_SQL :=
		' SELECT "OIBQ"."BinAbs", "OIBQ"."ItemCode", "OBBQ"."OnHandQty", "OBTN"."DistNumber" AS "BatchNumber",
			"OBTN"."MnfSerial" AS "BatchAttr1", "OBTN"."LotNumber" AS "BatchAttr2", "OSRN"."DistNumber" AS "SerialNumber", 
			"OSRN"."MnfSerial" AS "MnfSerial", "OSRN"."LotNumber" AS "LotNumber", "OBTN"."AbsEntry" AS "BatchAbs",
			"OSRN"."AbsEntry" AS "SerialAbs" '
		|| :FROM_TABLE_SQL ||
			' WHERE "OIBQ"."OnHandQty" <> 0 AND "OBBQ"."AbsEntry" IS NOT NULL ';
					
		SERIAL_DEFINED_SQL := 
		' SELECT "OIBQ"."BinAbs", "OIBQ"."ItemCode", "OSBQ"."OnHandQty", "OBTN"."DistNumber" AS "BatchNumber",
			"OBTN"."MnfSerial" AS "BatchAttr1", "OBTN"."LotNumber" AS "BatchAttr2", "OSRN"."DistNumber" AS "SerialNumber",
			"OSRN"."MnfSerial" AS "MnfSerial", "OSRN"."LotNumber" AS "LotNumber", "OBTN"."AbsEntry" AS "BatchAbs",
			"OSRN"."AbsEntry" AS "SerialAbs" '
		|| :FROM_TABLE_SQL ||
			' WHERE "OIBQ"."OnHandQty" <> 0 AND "OSBQ"."AbsEntry" IS NOT NULL ';
					
		NOT_SNB_SQL := 
		' SELECT "OIBQ"."BinAbs", "OIBQ"."ItemCode", "OIBQ"."OnHandQty", "OBTN"."DistNumber" AS "BatchNumber",
			"OBTN"."MnfSerial" AS "BatchAttr1", "OBTN"."LotNumber" AS "BatchAttr2", "OSRN"."DistNumber" AS "SerialNumber",
			"OSRN"."MnfSerial" AS "MnfSerial", "OSRN"."LotNumber" AS "LotNumber", "OBTN"."AbsEntry" AS "BatchAbs",
			"OSRN"."AbsEntry" AS "SerialAbs" '
		|| :FROM_TABLE_SQL ||
			' WHERE "OIBQ"."OnHandQty" <> 0 AND "OBBQ"."AbsEntry" IS NULL AND "OSBQ"."AbsEntry" IS NULL ';
			
		SELECT_BASE_SQL_4_NON_BIN_WHS := 
		' SELECT NULL AS "BinCode", "T_QTY_NON_BIN"."WhsCode" AS "WhsCode",
			NULL AS "SL1Code", NULL AS "SL2Code", NULL AS "SL3Code", NULL AS "SL4Code",
			NULL AS "Disabled", NULL AS "ReceiveBin", NULL AS "Descr", NULL AS "AltSortCod",
			NULL AS "BarCode", NULL AS "MinLevel", NULL AS "MaxLevel", NULL AS "Attr1Val", NULL AS "Attr2Val",
			NULL AS "Attr3Val", NULL AS "Attr4Val", NULL AS "Attr5Val", NULL AS "Attr6Val", NULL AS "Attr7Val",
			NULL AS "Attr8Val", NULL AS "Attr9Val", NULL AS "Attr10Val", NULL AS "RtrictType", NULL AS "RtrictResn",
			NULL AS "RtrictDate", NULL AS "ItmRtrictT", NULL AS "RestrictedTo", NULL AS "SngBatch", NULL AS "Batch",
			"OITM"."ItemCode" AS "ItemCode", "OITM"."ItemName" AS "ItemName",
			(SELECT "ItmsGrpNam" FROM "OITB" WHERE "OITB"."ItmsGrpCod" = "OITM"."ItmsGrpCod" ) AS "ItmsGrpNam",
			"OITM"."CodeBars" AS "CodeBars", "OITM"."InvntryUom" AS "InvntryUom", ';
			
		FROM_TABLE_SQL_4_NON_BIN_WHS :=
		' FROM "OITW" INNER JOIN "OWHS" ON "OITW"."WhsCode" = "OWHS"."WhsCode"
			LEFT OUTER JOIN "OBTQ" ON "OBTQ"."WhsCode" = "OITW"."WhsCode" 
			AND "OBTQ"."ItemCode" = "OITW"."ItemCode" 
			LEFT OUTER JOIN "OSRQ" ON "OSRQ"."WhsCode" = "OITW"."WhsCode"
			AND "OSRQ"."ItemCode" = "OITW"."ItemCode"
		WHERE "OWHS"."BinActivat" = ''N'' ';
			
		BATCH_NOT_DEFINED_SQL_4_NON_BIN_WHS := 
		' SELECT "OITW"."ItemCode", "OITW"."WhsCode", MAX("OITW"."OnHand") - SUM("OBTQ"."Quantity") AS "OnHandQty",
			NULL AS "BatchAbs", NULL AS "SerialAbs" '
		|| :FROM_TABLE_SQL_4_NON_BIN_WHS ||
			' AND "OITW"."OnHand" <> 0 AND "OBTQ"."AbsEntry" IS NOT NULL
			GROUP BY "OITW"."WhsCode", "OITW"."ItemCode"
			HAVING MAX("OITW"."OnHand") > SUM("OBTQ"."Quantity") ';
			
		SERIAL_NOT_DEFINED_SQL_4_NON_BIN_WHS := 
		' SELECT "OITW"."ItemCode", "OITW"."WhsCode", MAX("OITW"."OnHand") - SUM("OSRQ"."Quantity") AS "OnHandQty",
			NULL AS "BatchAbs", NULL AS "SerialAbs" '
		|| :FROM_TABLE_SQL_4_NON_BIN_WHS ||
			' AND "OITW"."OnHand" <> 0 AND "OSRQ"."AbsEntry" IS NOT NULL
			GROUP BY "OITW"."WhsCode", "OITW"."ItemCode"
			HAVING MAX("OITW"."OnHand") > SUM("OSRQ"."Quantity") ';
			
		BATCH_DEFINED_SQL_4_NON_BIN_WHS :=
		' SELECT "OITW"."ItemCode", "OITW"."WhsCode", "OBTQ"."Quantity" AS "OnHandQty",
			"OBTQ"."MdAbsEntry" AS "BatchAbs", "OSRQ"."MdAbsEntry" AS "SerialAbs" '
		|| :FROM_TABLE_SQL_4_NON_BIN_WHS ||
			' AND "OBTQ"."Quantity" <> 0 AND "OBTQ"."AbsEntry" IS NOT NULL ';	

		SERIAL_DEFINED_SQL_4_NON_BIN_WHS := 
		' SELECT "OITW"."ItemCode", "OITW"."WhsCode", "OSRQ"."Quantity" AS "OnHandQty", 
			"OBTQ"."MdAbsEntry" AS "BatchAbs", "OSRQ"."MdAbsEntry" AS "SerialAbs" '
		|| :FROM_TABLE_SQL_4_NON_BIN_WHS ||
			' AND "OSRQ"."Quantity" <> 0 AND "OSRQ"."AbsEntry" IS NOT NULL ';

		NOT_SNB_SQL_4_NON_BIN_WHS := 
		' SELECT "OITW"."ItemCode", "OITW"."WhsCode", "OITW"."OnHand" AS "OnHandQty",
			"OBTQ"."MdAbsEntry" AS "BatchAbs", "OSRQ"."MdAbsEntry" AS "SerialAbs" '
		|| :FROM_TABLE_SQL_4_NON_BIN_WHS ||
			' AND "OITW"."OnHand" <> 0 AND "OBTQ"."AbsEntry" IS NULL AND "OSRQ"."AbsEntry" IS NULL ';
			
		IF :includeNonBinWhs = 'Y' THEN
			T_SQL :=
			' SELECT "A"."BinCode", "A"."WhsCode", "A"."SL1Code", "A"."SL2Code", "A"."SL3Code", "A"."SL4Code",
				"A"."Disabled", "A"."ReceiveBin", "A"."Descr", "A"."AltSortCod", "A"."BarCode", "A"."MinLevel",
				"A"."MaxLevel", "A"."Attr1Val", "A"."Attr2Val", "A"."Attr3Val", "A"."Attr4Val", "A"."Attr5Val",
				"A"."Attr6Val", "A"."Attr7Val", "A"."Attr8Val", "A"."Attr9Val", "A"."Attr10Val", "A"."RtrictType",
				"A"."RtrictResn", "A"."RtrictDate", "A"."ItmRtrictT", "A"."RestrictedTo", "A"."SngBatch", "A"."Batch",
				"A"."ItemCode", "A"."ItemName", "A"."ItmsGrpNam", "A"."CodeBars", "A"."InvntryUom", "A"."OnHandQty",
				"A"."BatchNumber", "A"."BatchAttr1", "A"."BatchAttr2", "A"."SerialNumber", "A"."MnfSerial", "A"."LotNumber",
				"A"."QtyBelowMin", "A"."ReplenishmentQty" FROM ( ';
		ELSE
			T_SQL := '';
		END IF;
					
		T_SQL := :T_SQL ||
			' SELECT ' || :BIN_LOC_TABLE_BASE_FIELD ||
				' "OBIN"."ItmRtrictT", "ITM_RTRICT"."RestrictedTo", "OBIN"."SngBatch", 
				"BATCH_RTRICT"."Batch", "OITM"."ItemCode", "OITM"."ItemName",
				(SELECT "ItmsGrpNam" FROM "OITB" WHERE "OITB"."ItmsGrpCod" = "OITM"."ItmsGrpCod" ) AS "ItmsGrpNam",
				"OITM"."CodeBars", "OITM"."InvntryUom", "T_QTY"."OnHandQty", "T_QTY"."BatchNumber",
				"T_QTY"."BatchAttr1", "T_QTY"."BatchAttr2", "T_QTY"."SerialNumber", "T_QTY"."MnfSerial",
				"T_QTY"."LotNumber", NULL AS "QtyBelowMin", NULL AS "ReplenishmentQty" ';
			
		IF :includeNonBinWhs = 'Y' THEN
			T_SQL := :T_SQL || ', "T_QTY"."BatchAbs" AS "BatchAbs", "T_QTY"."SerialAbs" AS "SerialAbs" ';
		END IF;
			
		T_SQL := :T_SQL || ' FROM "OBIN" INNER JOIN ( '
			|| :BATCH_DEFINED_SQL ||
			' UNION ALL '
			|| :SERIAL_DEFINED_SQL;
					
		IF :snbOnly = 'N' THEN
			T_SQL := :T_SQL || ' UNION ALL ' || :NOT_SNB_SQL || ' UNION ALL ' || :BATCH_NOT_DEFINED_SQL 
							|| ' UNION ALL ' || :SERIAL_NOT_DEFINED_SQL;
		END IF;
					
		T_SQL := :T_SQL || 
			') AS "T_QTY" ON"OBIN"."AbsEntry" = "T_QTY"."BinAbs"
			INNER JOIN "OITM" ON "T_QTY"."ItemCode" = "OITM"."ItemCode"
			LEFT OUTER JOIN ' || :T_SQL_ITEM_RTRICT || ' ON "OBIN"."AbsEntry" = "ITM_RTRICT"."BinAbs"
			LEFT OUTER JOIN ' || :T_SQL_BATCH_RTRICT || ' ON "OBIN"."AbsEntry" = "BATCH_RTRICT"."BinAbs"
			WHERE "OBIN"."AbsEntry" > 0 ';
						
		IF :binAbsSet <> '' THEN
			T_SQL := :T_SQL || ' AND "OBIN"."AbsEntry" IN (SELECT "BinAbs" FROM #TMP_TABLE_BIN_ABS) ';
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
		
		IF :itemCodeFrom <> '' THEN
			T_SQL := :T_SQL || ' AND "OITM"."ItemCode" >= ''' || :itemCodeFrom || '''';
		END IF;
		IF :itemCodeTo <> '' THEN
			T_SQL := :T_SQL || ' AND "OITM"."ItemCode" <= ''' || :itemCodeTo || '''';
		END IF;
		
		IF :itemGroups <> '' THEN
			T_SQL := :T_SQL || ' AND "OITM"."ItmsGrpCod" IN ( ' || :itemGroups || ' ) ';
		END IF;
		
		IF :batchNumberFrom <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."DistNumber" >= ''' || :batchNumberFrom || '''';
		END IF;
		IF :batchNumberTo <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."DistNumber" <= ''' || :batchNumberTo || '''';
		END IF;
			
		IF :batchAttr1From <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."MnfSerial" >= ''' || :batchAttr1From || '''';
		END IF;
		IF :batchAttr1To <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."MnfSerial" <= ''' || :batchAttr1To || '''';
		END IF;
			
		IF :batchAttr2From <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."LotNumber" >= ''' || :batchAttr2From || '''';
		END IF;
		IF :batchAttr2To <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."LotNumber" <= ''' || :batchAttr2To || '''';
		END IF;
		
		IF :serialNumberFrom <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."DistNumber" >= ''' || :serialNumberFrom || '''';
		END IF;
		IF :serialNumberTo <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."DistNumber" <= ''' || :serialNumberTo || '''';
		END IF;
			
		IF :mfrSerialNumberFrom <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."MnfSerial" >= ''' || :mfrSerialNumberFrom || '''';
		END IF;
		IF :mfrSerialNumberTo <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."MnfSerial" <= ''' || :mfrSerialNumberTo || '''';
		END IF;
			
		IF :lotNumberFrom <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."LotNumber" >= ''' || :lotNumberFrom || '''';
		END IF;
		IF :lotNumberTo <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."LotNumber" <= ''' || :lotNumberTo || '''';
		END IF;
		
		IF :batchAbs <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."BatchAbs" IN (' || :batchAbs || ')';
		END IF;
		IF :serialAbs <> '' THEN
			T_SQL := :T_SQL || ' AND "T_QTY"."SerialAbs" IN (' || :serialAbs || ')';
		END IF;
		
		SELECT "DspFrznITM" INTO displayInactiveItem FROM "OADM";
		IF :displayInactiveItem = 'N' THEN
			T_SQL := :T_SQL || ' AND ("OITM"."ItemCode" IS NULL OR ("OITM"."validFor" = ''Y'' 
			OR ("OITM"."frozenFor" = ''Y'' AND ("OITM"."frozenFrom" IS NOT NULL OR "OITM"."frozenTo" IS NOT NULL)) 
			OR ("OITM"."validFor" = ''N'' AND "OITM"."frozenFor" = ''N'' ))) ';
		END IF;
		
		IF :includeNonBinWhs = 'N' THEN			
			T_SQL := :T_SQL || ' ORDER BY "OBIN"."BinCode", "OITM"."ItemCode", "T_QTY"."BatchAbs", "T_QTY"."SerialAbs" ';
		END IF;
		
		IF :includeNonBinWhs = 'Y' THEN
			T_SQL := :T_SQL || ' UNION ALL ' || :SELECT_BASE_SQL_4_NON_BIN_WHS ||
				' "T_QTY_NON_BIN"."OnHandQty",
				"OBTN"."DistNumber" AS "BatchNumber", "OBTN"."MnfSerial" AS "BatchAttr1", "OBTN"."LotNumber" AS "BatchAttr2",
				"OSRN"."DistNumber" AS "SerialNumber", "OSRN"."MnfSerial" AS "MnfSerial", "OSRN"."LotNumber" AS "LotNumber",
				NULL AS "QtyBelowMin", NULL AS "ReplenishmentQty", "OBTN"."AbsEntry" AS "BatchAbs", "OSRN"."AbsEntry" AS "SerialAbs"
				FROM ( ' ||  :BATCH_DEFINED_SQL_4_NON_BIN_WHS || ' UNION ALL ' || :SERIAL_DEFINED_SQL_4_NON_BIN_WHS;
				
			IF :snbOnly = 'N' THEN
				T_SQL := :T_SQL || ' UNION ALL ' || :NOT_SNB_SQL_4_NON_BIN_WHS
								|| ' UNION ALL ' || :BATCH_NOT_DEFINED_SQL_4_NON_BIN_WHS
								|| ' UNION ALL ' || :SERIAL_NOT_DEFINED_SQL_4_NON_BIN_WHS;
			END IF;
				
			T_SQL := :T_SQL || ') AS "T_QTY_NON_BIN" INNER JOIN "OITM" ON "T_QTY_NON_BIN"."ItemCode" = "OITM"."ItemCode"
							LEFT OUTER JOIN "OBTN" ON "T_QTY_NON_BIN"."BatchAbs" = "OBTN"."AbsEntry"
							LEFT OUTER JOIN "OSRN" ON "T_QTY_NON_BIN"."SerialAbs" = "OSRN"."AbsEntry"
							WHERE "OITM"."ItemCode" IS NOT NULL ';
							
			IF :whsIncludingFrom <> '' THEN
				T_SQL := :T_SQL || ' AND "T_QTY_NON_BIN"."WhsCode" >= ''' || :whsIncludingFrom || '''';
			END IF;
		
			IF :whsIncludingTo <> '' THEN
				T_SQL := :T_SQL || ' AND "T_QTY_NON_BIN"."WhsCode" <= ''' || :whsIncludingTo || '''';
			END IF;
	
			IF :whsExcludingFrom <> '' OR  :whsExcludingTo <> '' THEN
				T_SQL := :T_SQL || ' AND ( ';
				IF :whsExcludingFrom <> '' THEN
					T_SQL := :T_SQL || ' "T_QTY_NON_BIN"."WhsCode" < ''' || :whsExcludingFrom || '''';
				END IF;
	
				IF :whsExcludingTo <> '' THEN
					IF :whsExcludingFrom <> '' THEN
						T_SQL := :T_SQL || ' OR ';
					ELSE
						T_SQL := :T_SQL || ' "T_QTY_NON_BIN"."WhsCode" > ''' || :whsExcludingTo || '''';
					END IF;
				END IF;
				T_SQL := :T_SQL || ' ) ';
			END IF;
			
			IF :itemCodeFrom <> '' THEN
				T_SQL := :T_SQL || ' AND "OITM"."ItemCode" >= ''' || :itemCodeFrom || '''';
			END IF;
			IF :itemCodeTo <> '' THEN
				T_SQL := :T_SQL || ' AND "OITM"."ItemCode" <= ''' || :itemCodeTo || '''';
			END IF;
		
			IF :itemGroups <> '' THEN
				T_SQL := :T_SQL || ' AND "OITM"."ItmsGrpCod" IN ( ' || :itemGroups || ' ) ';
			END IF;
			
			SELECT "DspFrznITM" INTO displayInactiveItem FROM "OADM";
			IF :displayInactiveItem = 'N' THEN
				T_SQL := :T_SQL || ' AND ("OITM"."ItemCode" IS NULL OR ("OITM"."validFor" = ''Y'' 
				OR ("OITM"."frozenFor" = ''Y'' AND ("OITM"."frozenFrom" IS NOT NULL OR "OITM"."frozenTo" IS NOT NULL)) 
				OR ("OITM"."validFor" = ''N'' AND "OITM"."frozenFor" = ''N'' ))) ';
			END IF;
			
			IF :batchNumberFrom <> '' THEN
				T_SQL := :T_SQL || ' AND "OBTN"."DistNumber" >= ''' || :batchNumberFrom || '''';
			END IF;
			IF :batchNumberTo <> '' THEN
				T_SQL := :T_SQL || ' AND "OBTN"."DistNumber" <= ''' || :batchNumberTo || '''';
			END IF;
			
			IF :batchAttr1From <> '' THEN
				T_SQL := :T_SQL || ' AND "OBTN"."MnfSerial" >= ''' || :batchAttr1From || '''';
			END IF;
			IF :batchAttr1To <> '' THEN
				T_SQL := :T_SQL || ' AND "OBTN"."MnfSerial" <= ''' || :batchAttr1To || '''';
			END IF;
			
			IF :batchAttr2From <> '' THEN
				T_SQL := :T_SQL || ' AND "OBTN"."LotNumber" >= ''' || :batchAttr2From || '''';
			END IF;
			IF :batchAttr2To <> '' THEN
				T_SQL := :T_SQL || ' AND "OBTN"."LotNumber" <= ''' || :batchAttr2To || '''';
			END IF;
		
			IF :serialNumberFrom <> '' THEN
				T_SQL := :T_SQL || ' AND "OSRN"."DistNumber" >= ''' || :serialNumberFrom || '''';
			END IF;
			IF :serialNumberTo <> '' THEN
				T_SQL := :T_SQL || ' AND "OSRN"."DistNumber" <= ''' || :serialNumberTo || '''';
			END IF;
			
			IF :mfrSerialNumberFrom <> '' THEN
				T_SQL := :T_SQL || ' AND "OSRN"."MnfSerial" >= ''' || :mfrSerialNumberFrom || '''';
			END IF;
			IF :mfrSerialNumberTo <> '' THEN
				T_SQL := :T_SQL || ' AND "OSRN"."MnfSerial" <= ''' || :mfrSerialNumberTo || '''';
			END IF;
			
			IF :lotNumberFrom <> '' THEN
				T_SQL := :T_SQL || ' AND "OSRN"."LotNumber" >= ''' || :lotNumberFrom || '''';
			END IF;
			IF :lotNumberTo <> '' THEN
				T_SQL := :T_SQL || ' AND "OSRN"."LotNumber" <= ''' || :lotNumberTo || '''';
			END IF;
			
			T_SQL := :T_SQL || ') AS "A" ORDER BY "A"."WhsCode", "A"."BinCode", "A"."ItemCode", "A"."BatchAbs", "A"."SerialAbs" ';
		END IF;
	ELSE
		T_SQL :=
		'SELECT "BinCode", "WhsCode", "SL1Code", "SL2Code", "SL3Code",
		"SL4Code", "Disabled", "ReceiveBin", "Descr", "AltSortCod", "BarCode",
		"MinLevel", "MaxLevel", "Attr1Val", "Attr2Val", "Attr3Val", "Attr4Val",
		"Attr5Val", "Attr6Val", "Attr7Val", "Attr8Val", "Attr9Val", "Attr10Val",
		"RtrictType", "RtrictResn", "RtrictDate", "ItmRtrictT", NULL AS "RestrictedTo",
		"SngBatch", NULL AS "Batch", NULL AS "ItemCode", NULL AS "ItemName", NULL AS "ItmsGrpNam",
		NULL AS "CodeBars", NULL AS "InvntryUom", NULL AS "OnHandQty", 
		NULL AS "BatchNumber",
		NULL AS "BatchAttr1",
		NULL AS "BatchAttr2",
		NULL AS "SerialNumber",
		NULL AS "MnfSerial",
		NULL AS "LotNumber",
		NULL AS "QtyBelowMin", NULL AS "ReplenishmentQty"
		FROM "OBIN"
		WHERE 1 <> 1 ';
	END IF;
	
	--SELECT :T_SQL FROM DUMMY;
	EXECUTE IMMEDIATE :T_SQL;
END;


