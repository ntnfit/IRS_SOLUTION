-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_PicknPackCreateTempTable (IN docTable varchar(64))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS	
tempTable NVARCHAR(128);	
tempname nvarchar(100);
cnt int;
i int;
stmt NVARCHAR(2000);
tablePrefix NVARCHAR(128);
SQL_STR VARCHAR(1000);
BEGIN
	SELECT 'temp_' || CURRENT_CONNECTION into tempname FROM DUMMY;
	--ins_msg_proc (:tempname);
	
	tablePrefix := '#TMP_DOC1_';
	cnt := 1;
	tempTable := '';
	i := -1;
	
	WHILE :cnt <> 0 DO
		i := :i + 1;
		tempTable := :tablePrefix || :i;
		SELECT COUNT(*) INTO cnt FROM "PUBLIC"."M_TEMPORARY_TABLES" where "TABLE_NAME" = :tempTable;		
	END WHILE;

				
	IF :docTable = 'ORDR' THEN				
		stmt := 'CREATE LOCAL TEMPORARY TABLE "' || :tempTable ||'" AS (SELECT T1."DocEntry", T1."ItemCode", T1."WhsCode", T1."DropShip", T1."FromWhsCod", T1."LineStatus", '||
				'T1."TreeType", T1."ShipDate", T1."Dscription",T1."Quantity", T1."PickOty", T1."OpenCreQty", T1."DelivrdQty", '||
				'T1."PickStatus", T1."PickIdNo", T1."BackOrdr", T1."ReleasQtty", T1."LineNum",T1."ShipToCode", '||
				'T1."TrnsCode", T1."ShipToDesc", T1."Price",  T1."Currency", T1."LineTotal", T1."FreeTxt", '||
				'T1."ObjType", T1."UseBaseUn",T1."unitMsr", T1."NumPerMsr", T1."unitMsr2", T1."NumPerMsr2", '||
				'T1."VisOrder", T1."SlpCode", T1."Project", T1."OcrCode", T1."OcrCode2", T1."OcrCode3", T1."OcrCode4", '||
				'T1."OcrCode5", T1."SubCatNum", T1."CodeBars", '||
				't1."Length1", t1."length2", t1."Len1Unit", t1."Len2Unit", t1."Width1", t1."Width2", '||
				't1."Wdth1Unit", t1."Wdth2Unit", t1."Height1", t1."Height2", t1."Hght1Unit", '||
				't1."Hght2Unit", t1."Weight1", t1."Weight2", t1."Wght1Unit", t1."Wght2Unit", t1."Volume", '||
				't1."VolUnit", t1."SWW", t1."FreeTxt" as "Length", t1."FreeTxt" as "Width", t1."FreeTxt" as "Height", t1."FreeTxt" as "Weight", t1."ChgAsmBoMW", '|| 
				't1."UomEntry", t1."UomEntry2", t1."UomCode", t1."UomCode2", CAST(4 AS int) "ItemType", CAST(NULL as char(1)) "IssueType", '||
				'CAST(NULL AS DATETIME) AS "DocDate", '||
				'CAST(NULL AS nvarchar(50)) AS "OrigItem", '||
				'CAST(NULL AS int) AS "StgSeqNum", '||
				'CAST(NULL AS nvarchar(50)) AS "ShipFromCo", '||
				'CAST(NULL AS nvarchar(100)) AS "BaseAtCard", '||
				'CAST(NULL AS int) AS "BaseType", '||
				'CAST(NULL AS int) AS "StgEntry" '||
				' from ORDR T0 ' || 
				'INNER JOIN RDR1 T1 on T0."DocEntry" = T1."DocEntry" INNER JOIN OITM T2 on T1."ItemCode" = T2."ItemCode" '||
				'WHERE T0."DocStatus" = N''O'' AND T1."LineStatus" = N''O'' and T2."TreeType" <> N''S'')';	
				
		--ins_msg_proc (:stmt);
		
		EXEC (:stmt);
		
	elseif :docTable = 'OINV'  THEN					  
		stmt := 'CREATE LOCAL TEMPORARY TABLE "'||:tempTable||'" AS (SELECT T0."DocEntry", T1."ItemCode", T1."WhsCode", T1."DropShip", T1."FromWhsCod", T1."LineStatus", '||
				'T1."TreeType", T1."ShipDate", T1."Dscription",T1."Quantity", T1."PickOty", T1."OpenCreQty", T1."DelivrdQty", '||
				'T1."PickStatus", T1."PickIdNo", T1."BackOrdr", T1."ReleasQtty", T1."LineNum",T1."ShipToCode", '||
				'T1."TrnsCode", T1."ShipToDesc", T1."Price",  T1."Currency", T1."LineTotal", T1."FreeTxt", '||
				'T1."ObjType", T1."UseBaseUn",T1."unitMsr", T1."NumPerMsr", T1."unitMsr2", T1."NumPerMsr2", '||
				'T1."VisOrder", T1."SlpCode", T1."Project", T1."OcrCode", T1."OcrCode2", T1."OcrCode3", T1."OcrCode4", '||
				'T1."OcrCode5", T1."SubCatNum", T1."CodeBars", '||
				't1."Length1", t1."length2", t1."Len1Unit", t1."Len2Unit", t1."Width1", t1."Width2", '||
				't1."Wdth1Unit", t1."Wdth2Unit", t1."Height1", t1."Height2", t1."Hght1Unit", '||
				't1."Hght2Unit", t1."Weight1", t1."Weight2", t1."Wght1Unit", t1."Wght2Unit", t1."Volume", '||
				't1."VolUnit", t1."SWW", t1."FreeTxt" as "Length", t1."FreeTxt" as "Width", t1."FreeTxt" as "Height", t1."FreeTxt" as "Weight", t1."ChgAsmBoMW", '|| 
				't1."UomEntry", t1."UomEntry2", t1."UomCode", t1."UomCode2", CAST(4 AS int) "ItemType", CAST(NULL as char(1)) "IssueType", '||
				'CAST(NULL AS DATETIME) AS "DocDate", '||
				'CAST(NULL AS nvarchar(50)) AS "OrigItem", '||
				'CAST(NULL AS int) AS "StgSeqNum", '||
				'CAST(NULL AS nvarchar(50)) AS "ShipFromCo", '||
				'CAST(NULL AS nvarchar(100)) AS "BaseAtCard", '||
				'CAST(NULL AS int) AS "BaseType", '||
				'CAST(NULL AS int) AS "StgEntry" '||
				'from OINV T0 '|| 
				'INNER JOIN INV1 T1 on T0."DocEntry" = T1."DocEntry" INNER JOIN OITM T2 on T1."ItemCode" = T2."ItemCode" '||
				'WHERE T0."InvntSttus" = N''O'' AND T0."isIns" = N''Y'' AND T1."LineStatus" = N''O'' and T2."TreeType" <> N''S'')';
		
		--ins_msg_proc (:stmt);
		EXEC (:stmt);
		
	elseif :docTable = 'OWTQ'  THEN					  
		stmt := 'CREATE LOCAL TEMPORARY TABLE "'||:tempTable||'" AS (SELECT T0."DocEntry", T1."ItemCode", T1."FromWhsCod" as "WhsCode", T1."DropShip", T1."WhsCode" as "FromWhsCod", T1."LineStatus", '||
				'T1."TreeType", T1."ShipDate", T1."Dscription",T1."Quantity", T1."PickOty", T1."OpenCreQty", T1."Quantity"-T1."OpenCreQty" as "DelivrdQty", '||
				'T1."PickStatus", T1."PickIdNo", T1."BackOrdr", T1."ReleasQtty", T1."LineNum",T1."ShipToCode", '||
				'T1."TrnsCode", T1."ShipToDesc", T1."Price",  T1."Currency", T1."LineTotal", T1."FreeTxt", '||
				'T1."ObjType", T1."UseBaseUn",T1."unitMsr", T1."NumPerMsr", T1."unitMsr2", T1."NumPerMsr2", '||
				'T1."VisOrder", T0."SlpCode", T1."Project", T1."OcrCode", T1."OcrCode2", T1."OcrCode3", T1."OcrCode4", '||
				'T1."OcrCode5", T1."SubCatNum", T1."CodeBars", '||
				't1."Length1", t1."length2", t1."Len1Unit", t1."Len2Unit", t1."Width1", t1."Width2", '||
				't1."Wdth1Unit", t1."Wdth2Unit", t1."Height1", t1."Height2", t1."Hght1Unit", '||
				't1."Hght2Unit", t1."Weight1", t1."Weight2", t1."Wght1Unit", t1."Wght2Unit", t1."Volume", '||
				't1."VolUnit", t1."SWW", t1."FreeTxt" as "Length", t1."FreeTxt" as "Width", t1."FreeTxt" as "Height", t1."FreeTxt" as "Weight", t1."ChgAsmBoMW", '|| 
				't1."UomEntry", t1."UomEntry2", t1."UomCode", t1."UomCode2", CAST(4 AS int) "ItemType", CAST(NULL as char(1)) "IssueType", '||
				'CAST(NULL AS DATETIME) AS "DocDate", '||
				'CAST(NULL AS nvarchar(50)) AS "OrigItem", '||
				'CAST(NULL AS int) AS "StgSeqNum", '||
				'CAST(NULL AS nvarchar(50)) AS "ShipFromCo", '||
				'CAST(NULL AS nvarchar(100)) AS "BaseAtCard", '||
				'CAST(NULL AS int) AS "BaseType", '||
				'CAST(NULL AS int) AS "StgEntry" '||
				'from OWTQ T0 '|| 
				'INNER JOIN WTQ1 T1 on T0."DocEntry" = T1."DocEntry" INNER JOIN OITM T2 on T1."ItemCode" = T2."ItemCode" '||
				'WHERE T0."DocStatus" = N''O'' AND T1."LineStatus" = N''O'' and T2."TreeType" <> N''S'')';
		
		--ins_msg_proc (:stmt);
		EXEC (:stmt);
		
	elseif :docTable = 'OWOR'  THEN					  
		stmt := 'CREATE LOCAL TEMPORARY TABLE "'||:tempTable||'" AS (SELECT T0."DocEntry", T1."ItemCode", T1."WhsCode", T1."DropShip", T1."FromWhsCod", T1."LineStatus", '||
				'T1."TreeType", T1."ShipDate", T1."Dscription",T1."Quantity", T1."PickOty", T1."OpenCreQty", T1."DelivrdQty", '||
				'T1."PickStatus", T1."PickIdNo", T1."BackOrdr", T1."ReleasQtty", T1."LineNum",T1."ShipToCode", '||
				'T1."TrnsCode", T1."ShipToDesc", T1."Price",  T1."Currency", T1."LineTotal", T1."FreeTxt", '||
				'T1."ObjType", T1."UseBaseUn",T1."unitMsr", T1."NumPerMsr", T1."unitMsr2", T1."NumPerMsr2", '||
				'T1."VisOrder", T1."SlpCode", T1."Project", T1."OcrCode", T1."OcrCode2", T1."OcrCode3", T1."OcrCode4", '||
				'T1."OcrCode5", T1."SubCatNum", T1."CodeBars", '||
				't1."Length1", t1."length2", t1."Len1Unit", t1."Len2Unit", t1."Width1", t1."Width2", '||
				't1."Wdth1Unit", t1."Wdth2Unit", t1."Height1", t1."Height2", t1."Hght1Unit", '||
				't1."Hght2Unit", t1."Weight1", t1."Weight2", t1."Wght1Unit", t1."Wght2Unit", t1."Volume", '||
				't1."VolUnit", t1."SWW", t1."FreeTxt" as "Length", t1."FreeTxt" as "Width", t1."FreeTxt" as "Height", t1."FreeTxt" as "Weight", t1."ChgAsmBoMW", '|| 
				't1."UomEntry", t1."UomEntry2", t1."UomCode", t1."UomCode2", CAST(4 AS int) "ItemType", CAST(NULL as char(1)) "IssueType", '||
				'CAST(NULL AS DATETIME) AS "DocDate", '||
				'CAST(NULL AS nvarchar(50)) AS "OrigItem", '||
				'CAST(NULL AS int) AS "StgSeqNum", '||
				'CAST(NULL AS nvarchar(50)) AS "ShipFromCo", '||
				'CAST(NULL AS nvarchar(100)) AS "BaseAtCard", '||
				'CAST(NULL AS int) AS "BaseType", '||
				'CAST(NULL AS int) AS "StgEntry" '||
				'from OINV T0 '|| 
				'INNER JOIN INV1 T1 on T0."DocEntry" = T1."DocEntry" INNER JOIN OITM T2 on T1."ItemCode" = T2."ItemCode" '||
				'WHERE 1<>1 )';
		
		--ins_msg_proc (:stmt);
		EXEC (:stmt);
		stmt := 'INSERT INTO "' || :tempTable ||
			'" (SELECT T0."DocEntry", T1."ItemCode", T1."wareHouse", N''N'', N'''', N''O'', N''N'', '||
				'T1."EndDate", T1."ItemName", T1."PlannedQty", T1."PickQty", T1."PlannedQty" - T1."IssuedQty", T1."IssuedQty", '||
				'T1."PickStatus", T1."PickIdNo", N''Y'', T1."ReleaseQty", T1."LineNum", null, '||
				'-1, N'''', 0, N'''', 0, N'''', '||
				'202, N''Y'',T2."InvntryUom", 1, T2."InvntryUom", 1, '||
				'T1."VisOrder", -1, T1."Project", T1."OcrCode", T1."OcrCode2", T1."OcrCode3", T1."OcrCode4", '||
				'T1."OcrCode5", N'''', null, '||
				'0, 0, 0, 0, 0, 0, '||
				'0, 0, 0, 0, 0, '||
				'0, 0, 0, 0, 0, 0, '||
				'0, t2."SWW", N'''', N'''', N'''', N'''', N''N'', '|| 
				't1."UomEntry", t1."UomEntry", t1."UomCode", t1."UomCode", t1."ItemType", t1."IssueType", '||
				't1."StartDate", t0."ItemCode", t3."SeqNum", t4."Code", t3."Name", t0."Priority", t1."StageId" '||
				'from OWOR T0 '|| 
				'INNER JOIN WOR1 T1 on T0."DocEntry" = T1."DocEntry" INNER JOIN OITM T2 on T1."ItemCode" = T2."ItemCode" '||
				'LEFT OUTER JOIN WOR4 T3 on T1."DocEntry" = T3."DocEntry" and T1."StageId" = T3."StageId" '||
				'LEFT OUTER JOIN ORST T4 on T3."StgEntry" = T4."AbsEntry" '||
				'WHERE (T0."Status" = N''P'' OR T0."Status" = N''R'') AND (T0."Type" = N''S'' OR T0."Type" = N''P'') '||
				'AND T1."ItemType" = 4 and T2."TreeType" <> N''S'')';
		
		--ins_msg_proc (:stmt);
		
		EXEC(:stmt);
		
		stmt := 'INSERT INTO "' || :tempTable ||
			'" (SELECT T0."DocEntry", T1."ItemCode", T1."wareHouse", N''N'', N'''', N''O'', N''N'', '||
				'T1."EndDate", T1."ItemName", T1."PlannedQty", T1."PickQty", T1."PlannedQty" - T1."IssuedQty", T1."IssuedQty", '||
				'T1."PickStatus", T1."PickIdNo", N''Y'', T1."ReleaseQty", T1."LineNum", null, '||
				'-1, N'''', 0, N'''', 0, N'''', '||
				'202, N''Y'',null, 1, null, 1, '||
				'T1."VisOrder", -1, T1."Project", T1."OcrCode", T1."OcrCode2", T1."OcrCode3", T1."OcrCode4", '||
				'T1."OcrCode5", N'''', null, '||
				'0, 0, 0, 0, 0, 0, '||
				'0, 0, 0, 0, 0, '||
				'0, 0, 0, 0, 0, 0, '||
				'0, null, N'''', N'''', N'''', N'''', N''N'', '|| 
				't1."UomEntry", t1."UomEntry", t1."UomCode", t1."UomCode", t1."ItemType", t1."IssueType", '||
				't1."StartDate", t0."ItemCode", t3."SeqNum", t4."Code", t3."Name", t0."Priority", t1."StageId" '||
				'from OWOR T0 '|| 
				'INNER JOIN WOR1 T1 on T0."DocEntry" = T1."DocEntry" INNER JOIN ORSC T2 on T1."ItemCode" = T2."ResCode" '||
				'LEFT OUTER JOIN WOR4 T3 on T1."DocEntry" = T3."DocEntry" and T1."StageId" = T3."StageId" '||
				'LEFT OUTER JOIN ORST T4 on T3."StgEntry" = T4."AbsEntry" '||
				'WHERE (T0."Status" = N''P'' OR T0."Status" = N''R'') AND (T0."Type" = N''S'' OR T0."Type" = N''P'') '||
				'AND T1."ItemType" = 290)';
		
		--ins_msg_proc (:stmt);
		
		EXEC(:stmt);
				
	END IF;	
	
	SELECT :tempTable FROM DUMMY;
	
END;


