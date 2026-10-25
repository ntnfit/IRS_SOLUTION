
-- quandn
CALL APZ_EXISTSPROC('Apz_UDR_GetParamByStore');

GO


--	CALL Apz_UDR_GetParamByStore;
CREATE PROCEDURE Apz_UDR_GetParamByStore (
	IN 	code NVARCHAR(100) DEFAULT ''
,		storeName	NVARCHAR(500) DEFAULT ''
,		userName NVARCHAR(30) DEFAULT ''
,		branch	NVARCHAR(20) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	-- Khai báo bảng dữ liệu combobox (tùy chọn)
	DECLARE tblValidValue TABLE
	(
		 "ColumnName" NVARCHAR(50) NOT NULL
		 ,"Value" NVARCHAR(254) NOT NULL
		 ,"Description" NVARCHAR(254) NOT NULL
		 ,"BackColor" NVARCHAR(20)
		 ,"ForeColor" NVARCHAR(20)
	 );
	 DECLARE tblStructure TABLE
	 (
		 "ID" INT
		,"ColumnName" NVARCHAR(50) NOT NULL
		,"Caption" NVARCHAR(100)
		,"DataType" NVARCHAR(50)	--int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox) LinkedTable LinkedUDO* LinkedObject cfl(choosefromlist) LinkedCflObject
	                                        		-- color, validcolor (combobox+color),	linkedStore, LinkedCrystal
		,"SubType" NVARCHAR(500)					-- None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time, UDT dùng cho LinkedTable, (udo code : dùng cho LinkedUDO)*	 , ObjectType(số): dùng cho LinkedObject
	                            					-- type of cfl - số của type hoặc tên store(storename @value, @userName, @branch) lấy dữ liệu cfl-store phải có structure, sub - sub form
		,"Width" INT
		,"GroupIndex" INT				-- dùng cho grid  default -1
		,"Visible" TINYINT
		,"FormatType" NVARCHAR(20)					-- numeric, datetime/ 
		,"DisplayFormat" NVARCHAR(20) NULL			-- '{0:n2}'
		,"FilterString" NVARCHAR(5000)
		,"ReadOnly" TINYINT
		,"AllowEdit" TINYINT
		,"AllowMove" TINYINT
		,"AllowSize" TINYINT
		,"AllowSort" TINYINT
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
	DECLARE tbl TABLE
	(
		 "Code" NVARCHAR(60) NOT NULL,
		 "LineNum" INT,
		 "ParamName" NVARCHAR(50),
		 "Label" NVARCHAR(100),
		 "Type" NVARCHAR(20),
		 "DataType" NVARCHAR(50),
		 "FormatString" NVARCHAR(20),
		 "SubType" NVARCHAR(100),
		 "DefaultValue" NVARCHAR(254),
		 "ToolTip" NVARCHAR(254),
		 "AllowMultiple" TINYINT,
		 "Visible" TINYINT,
		 "AllowEdit" TINYINT
	 );
	 
	INSERT INTO :tblValidValue  ("ColumnName", "Value", "Description")
		SELECT 			N'DataType',N'nvarchar' ,	N'Nvarchar' FROM DUMMY
		UNION SELECT 	N'DataType',N'char' ,		N'Char' 	FROM DUMMY	
		UNION SELECT 	N'DataType',N'int' ,		N'Int' 		FROM DUMMY
		UNION SELECT 	N'DataType',N'bit' ,		N'Checked' 	FROM DUMMY
		UNION SELECT 	N'DataType',N'numeric' ,	N'Numeric' 	FROM DUMMY
		UNION SELECT 	N'DataType',N'datetime' ,	N'Datetime' FROM DUMMY
		UNION SELECT 	N'DataType',N'time' ,		N'Time' 	FROM DUMMY
		UNION SELECT 	N'DataType',N'validvalue' ,	N'Combobox'	FROM DUMMY
		UNION SELECT 	N'DataType',N'cfl' ,		N'Choose From List' FROM DUMMY
		UNION SELECT 	N'DataType',N'queryparam' ,	N'Query Parameter' 	FROM DUMMY
		UNION SELECT 	N'DataType',N'barcode' ,	N'Scan QR/Barcode' 	FROM DUMMY;
	
	INSERT INTO :tblStructure ("ID", "ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
  		SELECT   		1, N'Code',			N'Code',			'nvarchar',		'',			75,			-1,			0,			NULL,		NULL,					1,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY
		UNION SELECT	2, N'LineNum',		N'LineNum',			'nvarchar',		'',			75,			-1,			0,			NULL,		NULL,					1,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	3, N'ParamName',	N'ParamName',		'nvarchar',		'',			100,		-1,			1,			NULL,		NULL,					1,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY			
		UNION SELECT	4, N'Label',		N'Label',			'nvarchar',		'',			100,		-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	5, N'Type',			N'Type',			'nvarchar',		'',			75,			-1,			1,			NULL,		NULL,					1,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY	
		UNION SELECT	6, N'DataType',		N'DataType',		'ValidValue',	'',			75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	7, N'FormatString',	N'FormatString',	'nvarchar',		'',			75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY	
		UNION SELECT	8, N'SubType',		N'SubType',			'cfl',			'Apz_Cfl_GetUserProcedureFullParam',			75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	9, N'DefaultValue',	N'DefaultValue',	'nvarchar',		'',			75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY	
		UNION SELECT	10,N'ToolTip',		N'ToolTip',			'memo',			'',			75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	11,N'AllowMultiple',N'AllowMultiple',	'bit',			'',			75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY	
		UNION SELECT	12,N'Visible',		N'Visible',			'bit',			'',			75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	13,N'AllowEdit',	N'AllowEdit',		'bit',			'',			75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			1,		 '',	NULL,		 NULL FROM DUMMY;	
		
	SELECT * FROM :tblStructure ORDER BY ID;
	
	-- kiểm tra report có phải query không, nếu là query ko lấy từ INFORMATION_SCHEMA.PARAMETERS
	IF exists(SELECT 1 FROM APZ_OUDR WHERE "Code" = :code AND "StoreName" = :storeName AND "QueryType" = 'U') THEN
		INSERT INTO :tbl ("Code", "LineNum", "ParamName", "DataType", "SubType", "FormatString", "DefaultValue", "AllowMultiple", "Label", "ToolTip", "Type", "Visible", "AllowEdit")
		SELECT b."Code", b."LineNum", b."ParamName", b."DataType", b."SubType", b."FormatString", b."DefaultValue", b."AllowMultiple", b."Label", b."ToolTip", b."Type", b."Visible", b."AllowEdit" 
			FROM APZ_UDR1 b WHERE b."Code" = :code;
	ELSEIF exists(SELECT 1 FROM APZ_OUDR WHERE "Code" = :code AND "StoreName" = :storeName) THEN
		INSERT INTO :tbl ("Code", "LineNum", "ParamName", "DataType", "SubType", "FormatString", "DefaultValue", "AllowMultiple", "Label", "ToolTip", "Type", "Visible", "AllowEdit") 
		SELECT IFNULL(b."Code", :code), b."LineNum", IFNULL(b."ParamName",'@' || a.PARAMETER_NAME), IFNULL(b."DataType", a.DATA_TYPE_NAME), IFNULL(b."SubType", ''), IFNULL(b."FormatString", ''), IFNULL(b."DefaultValue", ''), IFNULL(b."AllowMultiple", 1), IFNULL(b."Label", REPLACE(a.PARAMETER_NAME, '@', '')), IFNULL(b."ToolTip", ''), IFNULL(b."Type", a.DATA_TYPE_NAME), IFNULL(b."Visible", 1), IFNULL(b."AllowEdit", 1) 
			FROM SYS.PROCEDURE_PARAMETERS a 
			LEFT OUTER JOIN APZ_UDR1 b ON b."Code" = :code AND '@' || a.PARAMETER_NAME = b."ParamName" 
			WHERE a.SCHEMA_NAME = CURRENT_SCHEMA 
				AND a.PROCEDURE_NAME = :storeName 
				AND a.PARAMETER_TYPE = 'IN'
			ORDER BY a.POSITION;
		/* bỏ ko sắp xếp theo linenum
		SELECT MAX("LineNum") INTO maxLine FROM #tbl;
		
		UPDATE #tbl SET "LineNum" = IFNULL(:maxLine, 0) + IFNULL(t.RN, 0) 
			FROM (SELECT a."ParamName", ROW_NUMBER() OVER ( ORDER BY a."ParamName") AS "RN" FROM #tbl a WHERE "LineNum" IS NULL) AS t 
			WHERE t."ParamName" = #tbl."ParamName" AND #tbl."LineNum" IS NULL;
		*/
	ELSE
		INSERT INTO :tbl ("Code", "LineNum", "ParamName", "DataType", "SubType", "FormatString", "DefaultValue", "AllowMultiple", "Label", "ToolTip", "Type", "Visible", "AllowEdit") 
		SELECT :code, ROW_NUMBER() OVER ( ORDER BY b.POSITION), '@' || b.PARAMETER_NAME, b.DATA_TYPE_NAME, '', '', '', 1, REPLACE(b.PARAMETER_NAME, '@', ''), '', b.DATA_TYPE_NAME, 1, 1 
			FROM SYS.PROCEDURE_PARAMETERS b 
			WHERE b.SCHEMA_NAME = CURRENT_SCHEMA 
				AND b.PROCEDURE_NAME = :storeName
				AND b.PARAMETER_TYPE = 'IN';
	END IF;
	
	SELECT * FROM :tbl;
	
	SELECT * FROM :tblValidValue;
	
	SELECT * FROM APZ_UDR2 WHERE "Code" = :code;

END;