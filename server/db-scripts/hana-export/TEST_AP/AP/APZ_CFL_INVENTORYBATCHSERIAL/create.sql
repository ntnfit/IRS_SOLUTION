
CREATE PROCEDURE APZ_CFl_InventoryBatchSerial
(
in ItemCode VARCHAR(50),
in WhsCode VARCHAR(50)
)
AS 
BEGIN
DECLARE ManBtchNum CHAR(1)='';
DECLARE ManSerNum CHAR(1)='';

create local temporary table #tblStructure
    (
     "ColumnName" NVARCHAR(50) NOT NULL
    ,"Caption" NVARCHAR(100) NULL
    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
	,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
    ,"Width" INT NULL DEFAULT 75
    ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
    ,"Visible" tinyint NULL DEFAULT 1
    ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
    ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
    ,"ReadOnly" tinyint NULL DEFAULT 0
    ,"AllowEdit" tinyint NULL DEFAULT 1
    ,"AllowMove" tinyint NULL DEFAULT 1
    ,"AllowSize" tinyint NULL DEFAULT 1
    ,"AllowSort" tinyint NULL DEFAULT 1
    ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
	-- Summary Item
    ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
	-- Group Summary
    ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
	--,"Parent" tinyint NULL	-- dùng cho treelist
	--,"Key" tinyint NULL		-- dùng cho treelist (identity)
    );
    
    INSERT INTO #tblStructure
				("ColumnName",		"Caption",				"DataType",				"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
        SELECT	 'DistNumber',		'Batch/Serial No',			'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'Instock',			'Instock',					'decimal',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'CommitQty',		'Committed Quantity',		'decimal',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'BinCode',			'BinCode',					'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'SnBAbsEntry',		'Serial/Batch Entry',		'int',				150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	;
		
SELECT *FROM #tblStructure;
	SELECT IFNULL((SELECT "ManBtchNum" FROM OITM WHERE "ItemCode" = :ItemCode), '') INTO ManBtchNum FROM DUMMY;
	SELECT IFNULL((SELECT "ManSerNum" FROM OITM WHERE "ItemCode" = :ItemCode), '') INTO ManSerNum FROM DUMMY;
	IF :ManBtchNum = 'Y' THEN
		CREATE LOCAL TEMPORARY TABLE #tblBATCH AS 
			(SELECT TOP 0 T4.*, T0."BinAbs", T1."BinCode", T2."OnHandQty" AS "Instock", T1."WhsCode", IFNULL(T1."Descr", T1."BinCode") AS "BinDescription", CAST(0 AS DECIMAL(19, 6)) AS "LogQty", CAST(0 AS DECIMAL(19, 6)) AS "CommitQty", T4."AbsEntry" AS "SnBAbsEntry" 
				FROM OIBQ T0
				INNER JOIN OBIN T1 ON T0."BinAbs" = T1."AbsEntry" AND T0."OnHandQty" <> 0 
				LEFT OUTER JOIN OBBQ T2 ON T0."BinAbs" = T2."BinAbs" AND T0."ItemCode" = T2."ItemCode" AND T2."OnHandQty" <> 0 
				LEFT OUTER JOIN OBTN T4 ON T2."SnBMDAbs" = T4."AbsEntry" AND T2."ItemCode" = T4."ItemCode" 
				LEFT OUTER JOIN OITM T6 ON T0."ItemCode" = T6."ItemCode" AND T6."validFor" = 'Y');
		INSERT INTO #tblBATCH
		SELECT T4.*, IFNULL(T0."BinAbs",0), IFNULL(T1."BinCode",''), IFNULL(T2."OnHandQty",0) AS "Instock", IFNULL(T1."WhsCode",''), IFNULL(T1."Descr", T1."BinCode") AS "BinDescription", CAST(0 AS DECIMAL(19, 6)) AS "LogQty", CAST(0 AS DECIMAL(19, 6)) AS "CommitQty", T4."AbsEntry" AS "SnBAbsEntry" 
				FROM OIBQ T0
				INNER JOIN OBIN T1 ON T0."BinAbs" = T1."AbsEntry" AND T0."OnHandQty" <> 0 
				LEFT OUTER JOIN OBBQ T2 ON T0."BinAbs" = T2."BinAbs" AND T0."ItemCode" = T2."ItemCode" AND T2."OnHandQty" <> 0 
				LEFT OUTER JOIN OBTN T4 ON T2."SnBMDAbs" = T4."AbsEntry" AND T2."ItemCode" = T4."ItemCode" 
				LEFT OUTER JOIN OITM T6 ON T0."ItemCode" = T6."ItemCode" AND T6."validFor" = 'Y' 
				WHERE T2."OnHandQty" > 0 
					AND T0."ItemCode" = :ItemCode 
					AND T1."WhsCode" = :WhsCode;
		INSERT INTO #tblBATCH 
		SELECT b.*, 0, '', IFNULL(A."Quantity",0) AS "InStock", IFNULL(A."WhsCode",''), '', CAST(0 AS DECIMAL(19, 6)) AS "LogQty", IFNULL(A."CommitQty",0), b."AbsEntry" AS "SnBAbsEntry" 
				FROM OBTQ A 
				INNER JOIN OBTN b ON A."ItemCode" = b."ItemCode" AND A."SysNumber" = b."SysNumber" 
				INNER JOIN OITM c ON a."ItemCode" = c."ItemCode" AND c."validFor" = 'Y' 
				WHERE A."Quantity" > 0 
					AND B."ItemCode" = :ItemCode 
					AND A."WhsCode" = :WhsCode 
					AND A."WhsCode" NOT IN (SELECT "WhsCode" FROM OBIN);
		UPDATE #tblBATCH SET "LogQty" = "Instock";
		SELECT *, 'B' AS "ManBatchSerial" FROM #tblBATCH;
		DROP TABLE #tblBATCH;
	ELSEIF :ManSerNum = 'Y' THEN
		CREATE LOCAL TEMPORARY TABLE #tblSERIAL AS
			(SELECT TOP 0 T5.*, T0."BinAbs", T1."BinCode", T3."OnHandQty" AS "Instock", T1."WhsCode", IFNULL(T1."Descr", T1."BinCode") AS "BinDescription", CAST(0 AS DECIMAL(19, 6)) AS "LogQty", CAST(0 AS DECIMAL(19, 6)) AS "CommitQty", T5."AbsEntry" AS "SnBAbsEntry" 
				FROM OIBQ T0 
				INNER JOIN OBIN T1 ON T0."BinAbs" = T1."AbsEntry" AND T0."OnHandQty" <> 0 
				LEFT OUTER JOIN OSBQ T3 ON T0."BinAbs" = T3."BinAbs" AND T0."ItemCode" = T3."ItemCode" AND T3."OnHandQty" <> 0 
				LEFT OUTER JOIN OSRN T5 ON T3."SnBMDAbs" = T5."AbsEntry" AND T3."ItemCode" = T5."ItemCode" 
				INNER JOIN OITM T6 ON T0."ItemCode" = T6."ItemCode" AND T6."validFor" = 'Y');
		INSERT INTO #tblSERIAL
			SELECT T5.*, IFNULL(T0."BinAbs",0), IFNULL(T1."BinCode",''), IFNULL(T3."OnHandQty",0) AS "Instock", IFNULL(T1."WhsCode",''), IFNULL(T1."Descr", T1."BinCode") AS "BinDescription", CAST(0 AS DECIMAL(19, 6)) AS "LogQty", CAST(0 AS DECIMAL(19, 6)) AS "CommitQty", T5."AbsEntry" AS "SnBAbsEntry" 
				FROM OIBQ T0 
				INNER JOIN OBIN T1 ON T0."BinAbs" = T1."AbsEntry" AND T0."OnHandQty" <> 0 
				LEFT OUTER JOIN OSBQ T3 ON T0."BinAbs" = T3."BinAbs" AND T0."ItemCode" = T3."ItemCode" AND T3."OnHandQty" <> 0 
				LEFT OUTER JOIN OSRN T5 ON T3."SnBMDAbs" = T5."AbsEntry" AND T3."ItemCode" = T5."ItemCode" 
				INNER JOIN OITM T6 ON T0."ItemCode" = T6."ItemCode" AND T6."validFor" = 'Y' 
				WHERE T3."OnHandQty" > 0 
					AND T0."ItemCode" = :ItemCode 
					AND T1."WhsCode" = :WhsCode;
		INSERT INTO #tblSERIAL
				SELECT b.*, 0, '', IFNULL(A."Quantity",0) AS "InStock", IFNULL(A."WhsCode",''), '', CAST(0 AS DECIMAL(19, 6)) AS "LogQty", IFNULL(A."CommitQty",0), b."AbsEntry" AS "SnBAbsEntry" 
				FROM OSRQ A 
				INNER JOIN OSRN b ON A."ItemCode" = b."ItemCode" AND A."SysNumber" = b."SysNumber" 
				INNER JOIN OITM c ON A."ItemCode" = c."ItemCode" AND c."validFor" = 'Y' 
				WHERE A."Quantity" > 0 
					AND b."ItemCode" = :ItemCode 
					AND A."WhsCode" = :WhsCode 
					AND A."WhsCode" NOT IN (SELECT "WhsCode" FROM OBIN);
		UPDATE #tblSERIAL SET "LogQty" = "Instock";
		SELECT *, 'S' AS "ManBatchSerial" FROM #tblSERIAL;
		DROP TABLE #tblSERIAL;
	ELSE
		SELECT TOP 1 '' As "DistNumber", 0 As "Instock",0 As "CommitQty",'' As "BinCode",0 As "SnBAbsEntry" from DUMMY;
	END IF;
	DROP TABLE #tblStructure;
 end;




