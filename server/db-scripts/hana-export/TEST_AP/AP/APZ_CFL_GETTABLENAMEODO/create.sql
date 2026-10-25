

CREATE PROCEDURE APZ_CFL_GetTableNameODO(IN type nvarchar(50))
AS
BEGIN
	create local temporary table #tblStructure
	    (
	     "ColumnName" NVARCHAR(50) NOT NULL
	    ,"Caption" NVARCHAR(100) NULL
	    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
		,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
	    ,"Width" INT NULL DEFAULT 75
	    ,"GroupIndex" INT NULL DEFAULT -1	-- dng cho grid  default -1
	    ,"Visible" TINYINT NULL DEFAULT '1'
	    ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
	    ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
	    ,"ReadOnly" TINYINT NULL DEFAULT '0'
	    ,"AllowEdit" TINYINT NULL DEFAULT '1'
	    ,"AllowMove" TINYINT NULL DEFAULT '1'
	    ,"AllowSize" TINYINT NULL DEFAULT '1'
	    ,"AllowSort" TINYINT NULL DEFAULT '1'
	    ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
		-- Summary Item
	    ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
		-- Group Summary
	    ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
		--,Parent BIT NULL	-- dng cho treelist
		--,[Key] BIT NULL		-- dng cho treelist (identity)
	    );
   INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
                SELECT	N'TableName',					N'Table Name Number',				'nvarchar',			150,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
		union select	N'Descr',					N'Table Description',				'nvarchar',			150,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY;
	
SELECT * FROM #tblStructure;
select "TableName", "Descr" from OUTB where "ObjectType" = :type and ifnull("UsedInObj",'') = '';
END




