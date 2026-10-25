
CREATE PROCEDURE Apz_Cfl_GetUserProcedure (
	IN 	type NVARCHAR(50) DEFAULT ''
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
	IF :type = 'U' THEN
		INSERT INTO #tblStructure 
						("ID", "ColumnName",	"Caption",		"DataType",		"Width","GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
	  		SELECT   		1, N'SPECIFIC_NAME',N'Name',		'nvarchar',		60,		-1,			  1,		 NULL,		   NULL,			1,		    0,			 1,			  1,			1,		    'L',	  NULL,		     NULL FROM DUMMY
			UNION SELECT	2, N'CatName',		N'Category',	'nvarchar',		200,	0,			  1,		 NULL,		   NULL,			1,		    0,			 1,			  1,			1,		    'L',	  NULL,		     NULL FROM DUMMY		
			UNION SELECT	3, N'QName',		N'Name',		'nvarchar',		300,	-1,			  1,		 NULL,		   NULL,			1,		    0,			 1,			  1,			1,		    'L',	  NULL,		     NULL FROM DUMMY	
			UNION SELECT	3, N'QString',		N'QString',		'nvarchar',		500,	-1,			  1,		 NULL,		   NULL,			1,		    0,			 1,			  1,			1,		    'L',	  NULL,		     NULL FROM DUMMY;			
		
		SELECT * FROM #tblStructure ORDER BY ID;
		
		SELECT a."IntrnalKey" AS "SPECIFIC_NAME", b."CatName", IFNULL(a."QName",N'No queries belong to this group') AS "QName", IFNULL(a."QString",'') AS "QString" 
			FROM OUQR a 
			RIGHT OUTER JOIN OQCN b ON a."QCategory" = b."CategoryId" 
			WHERE LOCATE("PermMask", 'Y') <> 0 
			ORDER BY b."CategoryId", a."QName";
	ELSE
		INSERT INTO #tblStructure 
						("ID", "ColumnName",	"Caption",		"DataType",		"Width","GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
	  		SELECT   		1, N'SPECIFIC_NAME',N'Name',		'nvarchar',		500,	-1,			  1,		 NULL,		   NULL,			1,		    0,			1,			 1,				1,		    'L',	  NULL,		     NULL FROM DUMMY
			UNION SELECT	2, N'CREATED',		N'Create date',	'datetime',		100,	-1,			  0,		 'datetime',	'dd/MM/yyyy HH:mm',			1,		    0,			 1,			  1,			1,		    'L',	  NULL,		     NULL FROM DUMMY		
			UNION SELECT	3, N'LAST_ALTERED',	N'Last update',	'datetime',		100,	-1,			  0,		 'datetime',	'dd/MM/yyyy HH:mm',			1,		    0,			 1,			  1,			1,		    'L',	  NULL,		     NULL FROM DUMMY;	
		
		SELECT * FROM #tblStructure ORDER BY ID;
		SELECT PROCEDURE_NAME AS "SPECIFIC_NAME" 
			FROM SYS.PROCEDURES 
			WHERE SCHEMA_NAME = CURRENT_SCHEMA 
			ORDER BY SCHEMA_NAME;
	END IF;
	
	DROP TABLE #tblStructure;
END;




