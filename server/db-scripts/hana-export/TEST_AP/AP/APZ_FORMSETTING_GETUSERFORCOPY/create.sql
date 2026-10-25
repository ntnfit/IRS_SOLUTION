
CREATE PROCEDURE Apz_FormSetting_GetUserForCopy (
	IN userName NVARCHAR(50)
,	   idSetting INT
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN		
	-- Khai báo bảng dữ liệu combobox (tùy chọn)
	CREATE LOCAL TEMPORARY COLUMN TABLE #tblValidValue (
		 "ColumnName" NVARCHAR(50) NOT NULL
		 ,"Value" NVARCHAR(254) NOT NULL
		 ,"Description" NVARCHAR(254) NOT NULL
		 ,"BackColor" NVARCHAR(20)
		 ,"ForeColor" NVARCHAR(20)
	 );
	INSERT INTO #tblValidValue("ColumnName", "Value", "Description", "BackColor", "ForeColor") 
		VALUES (N'BaseRef', :idSetting, :idSetting, N'orange', N'');
		
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
	 
	INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType", "BackColor")
  		SELECT   		N'USER_CODE',	N'User name',		'int',			'',		75,			-1,			1,			NULL,		NULL,					1,		 1,			1,			1,			0,		 '',	NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	N'U_NAME',		N'Name',			'int',			'',		75,			-1,			1,			NULL,		NULL,					1,		 1,			1,			1,			0,		 '',	NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	N'Department',	N'Department',		'nvarchar',		'',		75,			-1,			1,			NULL,		NULL,					1,		 1,			1,			1,			0,		 '',	NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	N'BaseRef',		N'BaseRef',			'validcolor',	'',		75,			-1,			0,			NULL,		NULL,					1,		 1,			1,			1,			0,		 '',	NULL,		 NULL,		 NULL FROM DUMMY;	
		
	SELECT * FROM #tblStructure ORDER BY ID;
	
	SELECT USER_CODE, U_NAME, IFNULL(b."Name", '') AS "Department", c."BaseRef" 
		FROM OUSR a 
		LEFT OUTER JOIN OUDP b ON b."Code" = a."Department" 
		LEFT OUTER JOIN APZ_OFST c ON c."UserSign" = a.USER_CODE 
								  AND c."BaseRef" = :idSetting 
		WHERE USER_CODE <> :userName;
	
	SELECT * FROM #tblValidValue;
	
	DROP TABLE #tblValidValue;
	DROP TABLE #tblStructure;
END;




