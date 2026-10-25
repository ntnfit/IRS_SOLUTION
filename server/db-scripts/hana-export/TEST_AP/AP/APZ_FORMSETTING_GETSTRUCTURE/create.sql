
CREATE PROCEDURE Apz_FormSetting_GetStructure (
	IN 	id INT DEFAULT 0
,		formId	NVARCHAR(20) DEFAULT ''
,		itemId	NVARCHAR(50) DEFAULT ''
,		typeName	VARCHAR(1) DEFAULT '1'	-- 1: full, 0: data only
,		userName NVARCHAR(30) DEFAULT ''
,		branch	NVARCHAR(20) DEFAULT ''	
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	-- khai báo biến
	DECLARE idSelect INT := 0;
	
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
		,"SummaryType" NVARCHAR(10)				-- sum type: all, selected or unselected rows
		,"SummaryMode" VARCHAR(1)
		,"GroupSummaryType" NVARCHAR(10)
		,"GroupSummaryShowInFooter" TINYINT
		,"SummaryFormat" NVARCHAR(20)				-- (AVR={0:#.##})/ Count ({0})/ Custom format/ Max (MAX={0})/ Min (MIN={0})/ Sum (SUM={0:#.##})
		,"Expression" NVARCHAR(5000)
		,"Parent" TINYINT							-- dùng cho treelist
		,"Key" TINYINT
		,"PivotArea" NVARCHAR(20)					-- sum,average...
	);
	 DECLARE tbl TABLE
	(
		 "ID" INT
		,"VisIndex" INT
		,"ColumnName" NVARCHAR(50) NOT NULL
		,"Caption" NVARCHAR(100)
		,"DataType" NVARCHAR(50) 
		,"SubType" NVARCHAR(500)	
		,"Width" INT
		,"GroupIndex" INT
		,"Visible" TINYINT
		,"FormatType" NVARCHAR(20)	
		,"DisplayFormat" NVARCHAR(20) NULL
		,"FilterString" NVARCHAR(5000)
		,"ReadOnly" TINYINT
		,"AllowEdit" TINYINT
		,"AllowMove" TINYINT
		,"AllowSize" TINYINT
		,"AllowSort" TINYINT
		,"Fixed" VARCHAR(1)			
		,"BackColor" NVARCHAR(20)					
		,"ForeColor" NVARCHAR(20)
		,"SummaryType" NVARCHAR(10)		
		,"SummaryMode" VARCHAR(1)
		,"GroupSummaryType" NVARCHAR(10)
		,"GroupSummaryShowInFooter" TINYINT
		,"SummaryFormat" NVARCHAR(20)				
		,"Expression" NVARCHAR(5000)
		,"Parent" TINYINT							
		,"Key" TINYINT
		,"PivotArea" NVARCHAR(20)		
	 );
	INSERT INTO :tblValidValue  ("ColumnName", "Value", "Description")
		SELECT 			N'DataType',N'nvarchar',		N'Nvarchar' 		FROM DUMMY
		UNION SELECT 	N'DataType',N'int',				N'Int' 				FROM DUMMY	
		UNION SELECT 	N'DataType',N'bit',				N'Checked' 			FROM DUMMY
		UNION SELECT 	N'DataType',N'numeric',			N'Numeric' 			FROM DUMMY
		UNION SELECT 	N'DataType',N'datetime',		N'Datetime' 		FROM DUMMY
		UNION SELECT 	N'DataType',N'memo',			N'Memo' 			FROM DUMMY
		UNION SELECT	N'DataType',N'memowrap',		N'Memo Auto Height'	FROM DUMMY
		UNION SELECT 	N'DataType',N'validvalue',		N'Combobox' 		FROM DUMMY
		UNION SELECT 	N'DataType',N'cfl',				N'Choose from list'	FROM DUMMY
		UNION SELECT 	N'DataType',N'sub',				N'Sub-Form' 		FROM DUMMY
		UNION SELECT 	N'DataType',N'linkedtable',		N'Linked Table' 	FROM DUMMY	
		UNION SELECT 	N'DataType',N'linkedudo',		N'Linked UDO' 		FROM DUMMY	
		UNION SELECT 	N'DataType',N'linkedobject',	N'Linked Object' 	FROM DUMMY
		UNION SELECT 	N'DataType',N'linkedCflobject', N'LinkedCflObject' 	FROM DUMMY
		UNION SELECT 	N'DataType',N'color',			N'Color' 			FROM DUMMY
		UNION SELECT 	N'DataType',N'validcolor',		N'Valid Color' 		FROM DUMMY
		UNION SELECT 	N'DataType',N'linkedstore',		N'Linked Store' 	FROM DUMMY
		UNION SELECT 	N'DataType',N'linkedcrystal',	N'Linked Crystal'	FROM DUMMY
		UNION SELECT 	N'DataType',N'link',			N'Attachment' 		FROM DUMMY
		UNION SELECT 	N'DataType',N'webex',			N'Web External' 					FROM DUMMY
		UNION SELECT 	N'DataType',N'web',				N'Web Embed with Url' 				FROM DUMMY
		UNION SELECT 	N'DataType',N'webhtml',			N'Web Embed with Html' 				FROM DUMMY
		UNION SELECT 	N'DataType',N'webtimer',		N'Web Embed with Url auto load'		FROM DUMMY
		UNION SELECT 	N'DataType',N'webhtmltimer',	N'Web Embed with Html auto load'	FROM DUMMY
		UNION SELECT 	N'DataType',N'',				N''	FROM DUMMY
						----------------------------------------
		UNION SELECT 	N'FormatType',N'numeric',		N'Numeric' 	FROM DUMMY
		UNION SELECT 	N'FormatType',N'datetime' ,		N'Datetime'	FROM DUMMY
		UNION SELECT 	N'FormatType',N'' ,				N'' 		FROM DUMMY
						----------------------------------------
		UNION SELECT	N'DisplayFormat',N'1' ,N'Qty'		FROM DUMMY
		UNION SELECT	N'DisplayFormat',N'2' ,N'Price'		FROM DUMMY
		UNION SELECT	N'DisplayFormat',N'3' ,N'Amount'	FROM DUMMY
		UNION SELECT	N'DisplayFormat',N'4' ,N'Rate'		FROM DUMMY
		UNION SELECT	N'DisplayFormat',N'5' ,N'Percent'	FROM DUMMY
		UNION SELECT	N'DisplayFormat',N'6' ,N'Unit'		FROM DUMMY
		UNION SELECT	N'DisplayFormat',N'7' ,N'DateTime'	FROM DUMMY
		UNION SELECT	N'DisplayFormat',N'8' ,N'Time'		FROM DUMMY
		UNION SELECT	N'DisplayFormat',N'9' ,N'Integer'	FROM DUMMY
		UNION SELECT	N'DisplayFormat',N'' , N''			FROM DUMMY
						----------------------------------------
		UNION SELECT	N'SummaryType',	N'sum' ,	N'Sum'  	FROM DUMMY
		UNION SELECT	N'SummaryType',	N'average' ,N'Average'  FROM DUMMY
		UNION SELECT	N'SummaryType',	N'count' ,	N'Count'  	FROM DUMMY
		UNION SELECT	N'SummaryType',	N'max' ,	N'Max'  	FROM DUMMY
		UNION SELECT	N'SummaryType',	N'min' ,	N'Min'  	FROM DUMMY
		UNION SELECT	N'SummaryType',	N'custom' ,	N'Custom'  	FROM DUMMY	-- tính sumary value theo giá trị 1 cột khác (lấy giá trị dòng đầu tiên) ,tạm lưu tên cột ở SubType
		UNION SELECT	N'SummaryType', N'' ,		N''  		FROM DUMMY
						----------------------------------------
		UNION SELECT 	N'SummaryMode', 'A',	N'All Rows'		FROM DUMMY
		UNION SELECT 	N'SummaryMode', 'S',	N'Selection'	FROM DUMMY
		UNION SELECT 	N'SummaryMode', 'M',	N'Mixed'		FROM DUMMY
		UNION SELECT 	N'SummaryMode',	'',		N''				FROM DUMMY
						----------------------------------------
		UNION SELECT	N'GroupSummaryType',N'sum' ,	N'Sum'  	FROM DUMMY
		UNION SELECT	N'GroupSummaryType',N'average' ,N'Average'  FROM DUMMY
		UNION SELECT	N'GroupSummaryType',N'count' ,	N'Count'  	FROM DUMMY
		UNION SELECT	N'GroupSummaryType',N'max' ,	N'Max'  	FROM DUMMY
		UNION SELECT	N'GroupSummaryType',N'min' ,	N'Min'  	FROM DUMMY
		UNION SELECT	N'GroupSummaryType',N'' ,		N''  		FROM DUMMY
		  			----------------------------------------
		UNION SELECT	N'Fixed',N'L'	,N'Left'  			FROM DUMMY
		UNION SELECT	N'Fixed',N'R'	,N'Right'  			FROM DUMMY
		UNION SELECT	N'Fixed',N'H'	,N'Excel Header'  	FROM DUMMY
		UNION SELECT	N'Fixed',N''	,N''  				FROM DUMMY
					----------------------------------------
		UNION SELECT	N'PivotArea',N'row' ,	N'Row Area'  	FROM DUMMY
		UNION SELECT	N'PivotArea',N'column' ,N'Column Area'  FROM DUMMY
		UNION SELECT	N'PivotArea',N'data' ,	N'Data Area'  	FROM DUMMY
		UNION SELECT	N'PivotArea',N'filter' ,N'Filter Area'  FROM DUMMY
		UNION SELECT 	N'PivotArea',N'',		N''				FROM DUMMY;
 
	INSERT INTO :tblStructure ("ID", "ColumnName", "Caption", 	"DataType", 	"SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType", "BackColor")
		SELECT   		1, 	N'ID',				N'ID',				'int',			'',		45,			-1,			0,			NULL,		NULL,					1,		 1,			1,			1,			0,		 	'L',	NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	2, 	N'VisIndex',		N'VisIndex',		'int',			'',		45,			-1,			0,			NULL,		NULL,					1,		 1,			1,			1,			0,		 	'L',	NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	3, 	N'ColumnName',		N'Column Name',		'nvarchar',		'',		75,			-1,			1,			NULL,		NULL,					1,		 1,			1,			1,			0,		 	'L',	NULL,		 NULL,		 'silver' FROM DUMMY
		UNION SELECT	4, 	N'Caption',			N'Caption',			'nvarchar',		'',		75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	5, 	N'DataType',		N'Data Type',		'ValidValue',	'',		75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	6, 	N'SubType',			N'SubType',			'cfl',			'Apz_Cfl_GetSubType @DataType',		120,		-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 '',	NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	7, 	N'Width',			N'Width',			'int',			'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	8, 	N'GroupIndex',		N'Group Index',		'int',			'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	9, 	N'Visible',			N'Visible',			'bit',			'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	10,	N'FormatType',		N'Format Type',		'ValidValue',	'',		75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	11, N'DisplayFormat',	N'Display Format',	'ValidValue',	'',		75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	12, N'FilterString',	N'Filter String',	'nvarchar',		'',		75,			-1,			1,			NULL,		NULL,					1,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	13, N'ReadOnly',		N'Read Only',		'bit',			'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	14, N'AllowEdit',		N'Allow Edit',		'bit',			'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	15, N'AllowMove',		N'Allow Move',		'bit',			'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	16, N'AllowSize',		N'Allow Size',		'bit',			'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	17, N'AllowSort',		N'Allow Sort',		'bit',			'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	18, N'Fixed',			N'Fixed',			'ValidValue',	'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	19, N'BackColor',		N'Back Color',		'nvarchar',		'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	20, N'ForeColor',		N'Fore Color',		'nvarchar',		'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	21, N'SummaryType',		N'Summary Type',	'ValidValue',	'',		75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	21, N'SummaryMode',		N'Summary Mode',	'ValidValue',	'',		75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	23, N'GroupSummaryType',N'Group Summary Type','ValidValue',	'',		75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	24, N'SummaryFormat',	N'Summary Format',	'nvarchar',		'',		75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	25, N'Expression',		N'Expression',		'nvarchar',		'',		75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	26, N'Parent',			N'Parent',			'bit',			'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	27, N'Key',				N'Key',				'bit',			'',		45,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY
		UNION SELECT	28, N'PivotArea',		N'PivotArea',		'ValidValue',	'',		75,			-1,			1,			NULL,		NULL,					0,		 1,			1,			1,			0,		 	'',		NULL,		 NULL,		 NULL FROM DUMMY;
	IF :typeName = '1' THEN		
		SELECT * FROM :tblStructure ORDER BY ID;
	END IF;
	
	IF :id = 0 THEN
		--SELECT COUNT(*) INTO isExist FROM APZ_OFST a WHERE a."FormId" = :formId AND a."ItemId" = :itemId AND a."UserSign" = :userName;
		IF EXISTS(SELECT 1 FROM APZ_OFST a WHERE a."FormId" = :formId AND a."ItemId" = :itemId AND a."UserSign" = :userName) THEN
			SELECT TOP 1 a.ID INTO id 
				FROM APZ_OFST a 
				WHERE a."FormId" = :formId 
					AND a."ItemId" = :itemId 
					AND a."UserSign" = :userName 
					ORDER BY a."SetDefault" DESC, ID DESC;
			idSelect := :id;
		ELSE
			--SELECT COUNT(*) INTO isExist FROM APZ_OFST a WHERE a."FormId" = :formId AND a."ItemId" = :itemId AND a."UserSign" = 'manager';
			IF EXISTS(SELECT 1 FROM APZ_OFST a WHERE a."FormId" = :formId AND a."ItemId" = :itemId AND a."UserSign" = 'manager') THEN
				SELECT TOP 1 a.ID INTO idSelect  
					FROM APZ_OFST a 
					WHERE a."FormId" = :formId 
						AND a."ItemId" = :itemId 
						AND a."UserSign" = 'manager' 
						ORDER BY a."SetDefault" DESC, ID DESC;
			END IF;
		END IF;
	ELSE
		idSelect := :id;
	END IF;

	INSERT INTO :tbl (ID, "VisIndex", "ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "FilterString", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "BackColor", "ForeColor", "SummaryType", "SummaryMode", "GroupSummaryType", "GroupSummaryShowInFooter", "SummaryFormat", "Expression", "Parent", "Key", "PivotArea") 
		SELECT :id, "VisIndex", "ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "FilterString", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "BackColor", "ForeColor", "SummaryType","SummaryMode", "GroupSummaryType", "GroupSummaryShowInFooter", "SummaryFormat", "Expression", "Parent", "Key", "PivotArea" 
		FROM APZ_FST1 
			WHERE ID = :idSelect 
			ORDER BY "VisIndex" ASC;
	
	SELECT * FROM :tbl ORDER BY "VisIndex" ASC;
	
	IF :typeName = '1' THEN 
		SELECT * FROM :tblValidValue;
	ELSEIF :typeName = '0' THEN 
		SELECT ID, "ColumnName", "Value", "Description", "BackColor", "ForeColor" 
			FROM APZ_FST2 
			WHERE ID = :idSelect;
	END IF;
	
END;

