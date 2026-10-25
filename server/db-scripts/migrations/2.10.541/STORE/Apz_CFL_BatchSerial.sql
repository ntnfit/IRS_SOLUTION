
CALL APZ_EXISTSPROC('Apz_CFL_BatchSerial');

GO

CREATE PROCEDURE Apz_CFL_BatchSerial (
	IN 	ItemCode VARCHAR(50)
	,in WhsCode VARCHAR(50)
	,in ObjType VARCHAR(50)
	,in bpCode nvarchar(50) default ''
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
		--UNION SELECT	N'BinCode',		N'BinCode',				'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	N'SnBAbsEntry',	N'Serial/Batch Entry',	'int',			150,	-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY;
		
	SELECT * FROM #tblStructure;
	
	SELECT IFNULL((SELECT "ManBtchNum" FROM OITM WHERE "ItemCode" = :ItemCode), '') INTO ManBtchNum FROM DUMMY;
	SELECT IFNULL((SELECT "ManSerNum" FROM OITM WHERE "ItemCode" = :ItemCode), '') INTO ManSerNum FROM DUMMY;
	
	if ifnull(:bpCode, '') <> ''
	THEN
		create local temporary table #tempDelivery ("MdAbsEntry" int null);
		INSERT INTO #tempDelivery 
			SELECT distinct a."MdAbsEntry" FROM ITL1 a INNER JOIN OITL b ON a."LogEntry" = b."LogEntry"
			WHERE /*b."DocType" = '15' AND b."CardCode" = :bpCode AND */ b."ItemCode" = :ItemCode and b."StockEff"=1
					and (:ObjType in ('234000031','16') or b."LocCode"=:WhsCode)
					and case when :ObjType in ('234000031','16') then b."DocQty" else -b."DocQty" end < 0
					GROUP BY a."MdAbsEntry";
		if :ManBtchNum = 'Y'
		THEN
			SELECT a.*, b."Instock", CAST(NULL AS nvarchar) AS "WhsCode"
					, b."Quantity" AS "LogQty", b."CommitQty", a."AbsEntry" AS "SnBAbsEntry", 'B' AS "ManBatchSerial" FROM OBTN a 
							INNER JOIN (SELECT x.*, SUM(IFNULL(y."Quantity", 0) - IFNULL(y."CommitQty", 0)) AS "Quantity", SUM(IFNULL(y."Quantity", 0)) AS "Instock", SUM(IFNULL(y."CommitQty", 0)) AS "CommitQty" 
								FROM #tempDelivery x INNER JOIN OBTQ y ON x."MdAbsEntry" = y."MdAbsEntry"
								WHERE :ObjType in ('234000031','16') or y."WhsCode"=:WhsCode
								GROUP BY x."MdAbsEntry") AS b ON a."AbsEntry" = b."MdAbsEntry"
								WHERE :ObjType in ('234000031','16') or b."Instock" > 0
								ORDER BY "InDate";
		else
			SELECT a.*, b."Instock", CAST(NULL AS nvarchar) AS "WhsCode", 1 AS "LogQty"
					, b."CommitQty", a."AbsEntry" AS "SnBAbsEntry", 'S' AS "ManBatchSerial" FROM OSRN a 
							INNER JOIN (SELECT x.*, SUM(IFNULL(y."Quantity", 0) - IFNULL(y."CommitQty", 0)) AS "Quantity", SUM(IFNULL(y."Quantity", 0)) AS "Instock", SUM(IFNULL(y."CommitQty", 0)) AS "CommitQty" 
								FROM #tempDelivery x INNER JOIN OBTQ y ON x."MdAbsEntry" = y."MdAbsEntry"
								WHERE :ObjType in ('234000031','16') or y."WhsCode"=:WhsCode
								GROUP BY x."MdAbsEntry") AS b ON a."AbsEntry" = b."MdAbsEntry"
								WHERE :ObjType in ('234000031','16') or b."Instock" > 0
								ORDER BY "InDate";
		END IF;
		return;
	END IF;
	
	IF :ManBtchNum = 'Y' THEN
		SELECT b.*, IFNULL(A."Quantity",0) "InStock", IFNULL(A."WhsCode",'') "WhsCode", IFNULL(A."Quantity",0) - IFNULL(A."CommitQty",0) "LogQty", IFNULL(A."CommitQty",0) "CommitQty", b."AbsEntry" "SnBAbsEntry", 'B' "ManBatchSerial"
				FROM OBTQ A 
				INNER JOIN OBTN b ON A."MdAbsEntry" = b."AbsEntry"
				INNER JOIN OITM c ON a."ItemCode" = c."ItemCode" AND c."validFor" = 'Y' 
				WHERE A."Quantity" > 0 
					AND a."ItemCode" = :ItemCode 
					AND A."WhsCode" = :WhsCode;
	ELSEIF :ManSerNum = 'Y' THEN
		SELECT b.*, IFNULL(A."Quantity",0) "InStock", IFNULL(A."WhsCode",'') "WhsCode", IFNULL(A."Quantity",0) - IFNULL(A."CommitQty",0) "LogQty", IFNULL(A."CommitQty",0) "CommitQty", b."AbsEntry" "SnBAbsEntry", 'S' "ManBatchSerial"
				FROM OSRQ A 
				INNER JOIN OSRN b ON A."ItemCode" = b."ItemCode" AND A."SysNumber" = b."SysNumber" 
				INNER JOIN OITM c ON A."ItemCode" = c."ItemCode" AND c."validFor" = 'Y' 
				WHERE A."Quantity" > 0 
					AND a."ItemCode" = :ItemCode 
					AND A."WhsCode" = :WhsCode;
	ELSE
		SELECT TOP 0 '' AS "DistNumber", 0 AS "Instock", '' AS "BinCode" FROM DUMMY;
	END IF;
	DROP TABLE #tblStructure;

END;