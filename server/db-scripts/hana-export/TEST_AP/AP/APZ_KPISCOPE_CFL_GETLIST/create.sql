
CREATE PROCEDURE APZ_KPIScope_CFL_GetList
AS
BEGIN
-- Khai báo bảng Structure
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
		--,Parent BIT NULL	-- dùng cho treelist
		--,[Key] BIT NULL		-- dùng cho treelist (identity)
    );

    INSERT INTO #tblStructure 
					("ColumnName",	"Caption",		"DataType",			"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
    	select N'Code',		N'Code',			'nvarchar',		85,		-1,			1,				NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
    		UNION ALL
		SELECT N'Name',		N'Name',			'nvarchar',		150,	-1,			1,				NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
    		UNION ALL
		SELECT N'Remark',	    N'Remark',			'nvarchar',		150,	-1,			1,				NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY;
					 
	SELECT * FROM #tblStructure;
	SELECT "Code","Name","Remark" FROM APZ_OKSE WHERE "Active" = 'Y';
END




