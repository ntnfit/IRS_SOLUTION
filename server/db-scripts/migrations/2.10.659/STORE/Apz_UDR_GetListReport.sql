
CALL APZ_EXISTSPROC('Apz_UDR_GetListReport');

GO


CREATE PROCEDURE Apz_UDR_GetListReport (
	IN 	userName NVARCHAR(30) DEFAULT ''
,		branch	NVARCHAR(20) DEFAULT ''
,		type varchar(1) default 'N'
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
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
					("ID", "ColumnName",	"Caption",		"DataType",		"Width","GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
  		SELECT   		1, N'Code',			N'Code',		'nvarchar',		100,	-1,			  1,		 NULL,		   NULL,			1,		    0,			 1,			  1,			1,		    'L',	  NULL,		     NULL FROM DUMMY
		UNION SELECT	2, N'Name',			N'Name',		'nvarchar',		200,	-1,			  1,		 NULL,		   NULL,			1,		    0,			 1,			  1,			1,		    'L',	  NULL,		     NULL FROM DUMMY		
		UNION SELECT	3, N'TableName',	N'TableName',	'nvarchar',		100,	-1,			  1,		 NULL,		   NULL,			1,		    0,			 1,			  1,			1,		    'L',	  NULL,		     NULL FROM DUMMY
		UNION SELECT	3, N'MenuCapt',		N'Menu Caption','nvarchar',		200,	-1,			  1,		 NULL,		   NULL,			1,		    0,			 1,			  1,			1,		    '',		  NULL,		     NULL FROM DUMMY;		
		
	SELECT * FROM #tblStructure ORDER BY ID;
	
	SELECT * FROM APZ_OUDR WHERE IFNULL("SystemReport",'N') = :type order by lower(trim("MenuCapt"));
	
	DROP TABLE #tblStructure;

END;