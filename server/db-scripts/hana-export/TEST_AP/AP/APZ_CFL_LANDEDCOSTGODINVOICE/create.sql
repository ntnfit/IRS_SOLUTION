
CREATE PROCEDURE APZ_Cfl_LandedCostGodInvoice
(
	in vendor nvarchar(30),
	in objectType int
)
AS 
BEGIN
	Declare docEntry nvarchar(20)='';
	Create local temporary table #tblStructure
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
				("ColumnName",		"Caption",					"DataType",				"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
        SELECT	 'DocNum',			'#',						'int',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	Union SELECT 'DocDate',			'Doc Date',					'timestamp',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	Union SELECT 'CardName',		'Vendor Name',					'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	Union SELECT 'CardCode',		'Vendor Code',					'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	Union SELECT 'DocDueDate',		'Due Date',					'timestamp',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	Union SELECT 'DocTotal',		'Doc Total',					'decimal',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	Union SELECT 'DocEntry',		'DocEntry',				'int',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	Union SELECT 'Canceled',		'Canceled',				'char',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	 	
	;
		
	SELECT *FROM #tblStructure;
	If :objectType=20
	Then
		SELECT a."DocNum",a."DocEntry",a."DocDate",a."CardCode",b."CardName",a."DocDueDate",a.Canceled,a."DocTotal" 
		FROM OPDN a LEFT JOIN OCRD b on a."CardCode"=b."CardCode" 
		WHERE ("DocStatus"='O' And a."CardCode"=:vendor And a."DocEntry" Not In(SELECT DISTINCT "BaseEntry" from IPF1 where "BaseType"=20))
			Or(CANCELED ='N' And IFNULL("OpenForLaC",'N') = 'Y' And a."CardCode"=:vendor);
	Elseif :objectType=18
	Then
		SELECT a."DocNum",a."DocEntry",a."DocDate",a."CardCode",b."CardName",a."DocDueDate",a.Canceled,a."DocTotal" 
		FROM OPCH a LEFT JOIN OCRD b on a."CardCode"=b."CardCode" 
		WHERE ("DocStatus"='O' And a."CardCode"=:vendor And a."DocEntry" Not In(SELECT DISTINCT "BaseEntry" from IPF1 where "BaseType"=18))
			Or(CANCELED ='N' And IFNULL("OpenForLaC",'N') = 'Y' And a."CardCode"=:vendor);
	Elseif :objectType=69
	Then
		SELECT a."DocNum",a."DocEntry",a."DocDate",a."CardCode",b."CardName",a."DocDueDate",a."Canceled",a."DocTotal" 
		FROM OIPF a LEFT JOIN OCRD b on a."CardCode"=b."CardCode" 
		WHERE "DocStatus"='O' And a."CardCode"=:vendor And a."DocEntry" Not In(SELECT DISTINCT "BaseEntry" from IPF1 where "BaseType"=69);
	End if;
	Drop Table #tblStructure;
End;

