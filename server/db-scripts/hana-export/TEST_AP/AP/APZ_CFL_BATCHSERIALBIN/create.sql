
CREATE PROCEDURE Apz_CFL_BatchSerialBin (
	IN 	ItemCode VARCHAR(50)
,		WhsCode VARCHAR(50)
,		ObjType VARCHAR(50)
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	DECLARE ManBtchNum char(1) := '';
	DECLARE ManSerNum char(1) := '';
	
	CREATE LOCAL TEMPORARY COLUMN TABLE #tblStructure (
		 "ID" INT
		,"ColumnName" NVARCHAR(50) NOT NULL
		,"Caption" NVARCHAR(100)
		,"DataType" NVARCHAR(50) DEFAULT 'nvarchar'	--int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox) LinkedTable LinkedUDO* LinkedObject cfl(choosefromlist) LinkedCflObject
	                                        		-- color, validcolor (combobox+color),	linkedStore, LinkedCrystal
		,"SubType" NVARCHAR(500)					-- None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time, UDT dùng cho LinkedTable, (udo code : dùng cho LinkedUDO)*	 , ObjectType(số): dùng cho LinkedObject
	                            					-- type of cfl - số của type hoặc tên store(storename @value, @userName, @branch) lấy dữ liệu cfl-store phải có structure, sub - sub form
		,"Width" INT DEFAULT 75
		,"GroupIndex" INT DEFAULT -1				-- dùng cho grid  default -1
		,"Visible" TINYINT DEFAULT 1
		,"FormatType" NVARCHAR(20)					-- numeric, datetime/ 
		,"DisplayFormat" NVARCHAR(20) NULL			-- '{0:n2}'
		,"FilterString" NVARCHAR(5000)
		,"ReadOnly" TINYINT DEFAULT 0
		,"AllowEdit" TINYINT DEFAULT 1
		,"AllowMove" TINYINT DEFAULT 1
		,"AllowSize" TINYINT DEFAULT 1
		,"AllowSort" TINYINT DEFAULT 1
		,"Fixed" VARCHAR(1)							-- L:Left / R:Right
		,"BackColor" NVARCHAR(20)					-- color column //list color https://msdn.microsoft.com/en-us/library/aa358802(v=vs.85).aspx
		,"ForeColor" NVARCHAR(20)
		,"SummaryType" NVARCHAR(10)					-- sum,average...
		,"GroupSummaryType" NVARCHAR(10)
		,"GroupSummaryShowInFooter" TINYINT
		,"SummaryFormat" NVARCHAR(20)				-- (AVR={0:#.##})/ Count ({0})/ Custom format/ Max (MAX={0})/ Min (MIN={0})/ Sum (SUM={0:#.##})
		,"Expression" NVARCHAR(5000)
		,"Parent" TINYINT							-- dùng cho treelist
		,"Key" TINYINT
		,"PivotArea" NVARCHAR(20)
		);	
	 
	INSERT INTO #tblStructure 
					("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
  		SELECT   		N'DistNumber',	N'Batch/Serial No',		'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY
		UNION SELECT	N'Instock',		N'Instock',				'numeric',		150,	-1,			1,			NULL,		'{0:n5}',		1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	N'CommitQty',	N'Committed Quantity',	'numeric',		150,	-1,			1,			NULL,		'{0:n5}',		1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	N'BinCode',		N'BinCode',				'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	N'SnBAbsEntry',	N'Serial/Batch Entry',	'int',			150,	-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY;
		
	SELECT * FROM #tblStructure;
	
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
			SELECT T4.*, T0."BinAbs", T1."BinCode", T2."OnHandQty" AS "Instock", T1."WhsCode", IFNULL(T1."Descr", T1."BinCode") AS "BinDescription", CAST(0 AS DECIMAL(19, 6)) AS "LogQty", CAST(0 AS DECIMAL(19, 6)) AS "CommitQty", T4."AbsEntry" AS "SnBAbsEntry" 
				FROM OIBQ T0
				INNER JOIN OBIN T1 ON T0."BinAbs" = T1."AbsEntry" AND T0."OnHandQty" <> 0 
				LEFT OUTER JOIN OBBQ T2 ON T0."BinAbs" = T2."BinAbs" AND T0."ItemCode" = T2."ItemCode" AND T2."OnHandQty" <> 0 
				LEFT OUTER JOIN OBTN T4 ON T2."SnBMDAbs" = T4."AbsEntry" AND T2."ItemCode" = T4."ItemCode" 
				LEFT OUTER JOIN OITM T6 ON T0."ItemCode" = T6."ItemCode" AND T6."validFor" = 'Y' 
				WHERE T2."OnHandQty" > 0 
					AND T0."ItemCode" = :ItemCode 
					AND T1."WhsCode" = :WhsCode 
			UNION ALL SELECT b.*, 0, '', A."Quantity" AS "InStock", A."WhsCode", '', CAST(0 AS DECIMAL(19, 6)) AS "LogQty", a."CommitQty", b."AbsEntry" AS "SnBAbsEntry" 
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
			SELECT T5.*, T0."BinAbs", T1."BinCode", T3."OnHandQty" AS "Instock", T1."WhsCode", IFNULL(T1."Descr", T1."BinCode") AS "BinDescription", CAST(0 AS DECIMAL(19, 6)) AS "LogQty", CAST(0 AS DECIMAL(19, 6)) AS "CommitQty", T5."AbsEntry" AS "SnBAbsEntry" 
				FROM OIBQ T0 
				INNER JOIN OBIN T1 ON T0."BinAbs" = T1."AbsEntry" AND T0."OnHandQty" <> 0 
				LEFT OUTER JOIN OSBQ T3 ON T0."BinAbs" = T3."BinAbs" AND T0."ItemCode" = T3."ItemCode" AND T3."OnHandQty" <> 0 
				LEFT OUTER JOIN OSRN T5 ON T3."SnBMDAbs" = T5."AbsEntry" AND T3."ItemCode" = T5."ItemCode" 
				INNER JOIN OITM T6 ON T0."ItemCode" = T6."ItemCode" AND T6."validFor" = 'Y' 
				WHERE T3."OnHandQty" > 0 
					AND T0."ItemCode" = :ItemCode 
					AND T1."WhsCode" = :WhsCode 
			UNION ALL SELECT b.*, 0, '', A."Quantity" AS "InStock", A."WhsCode", '', CAST(0 AS DECIMAL(19, 6)) AS "LogQty", A."CommitQty", b."AbsEntry" AS "SnBAbsEntry" 
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
		SELECT TOP 0 '' AS "DistNumber", 0 AS "Instock", '' AS "BinCode" FROM DUMMY;
	END IF;
	DROP TABLE #tblStructure;

END;




