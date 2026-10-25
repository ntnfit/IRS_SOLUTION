
CREATE PROCEDURE APZ_CFL_GetItemBySaleOrder(in orderId int)
AS 
BEGIN
	Declare DocEntry int=0;
	select ifnull((select "DocEntry" from ORDR where "DocNum"=:orderId), 0) into DocEntry from dummy;
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
				("ColumnName",		"Caption",		"DataType",		"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
        SELECT	 'DocEntry',		'DocEntry',			'int',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'DocNum',			'DocNum',			'int',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'LineNum',			'LineNum',			'int',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'DocType',			'DocType',			'int',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'ItemCode',		'ItemCode',			'nvarchar',		150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'Dscription',		'ItemName',			'nvarchar',		150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'UoM',				'UoM',				'nvarchar',		150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	;
		
SELECT *FROM #tblStructure;

select a.*, b."InvntryUom" as "UoM",b."ItmsGrpCod" from RDR1 a
		left join OITM b on b."ItemCode"=a."ItemCode"
		 WHERE a."DocEntry"=:DocEntry ;
		 
Drop table #tblStructure;		 
 end;




