
CREATE PROCEDURE APZ_CFL_GetListOperation
AS
BEGIN	
	-- Khai báo bảng Structure
		Create local temporary table #tblStructure
        (
             "ColumnName" NVARCHAR(50) NOT NULL
            ,"Caption" NVARCHAR(100) NULL
            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
			,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
            ,"Width" INT NULL DEFAULT 75
            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
            ,"Visible" smallint NULL DEFAULT 1
            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
            ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
            ,"ReadOnly" smallint NULL DEFAULT 0
            ,"AllowEdit" smallint NULL DEFAULT 1
            ,"AllowMove" smallint NULL DEFAULT 1
            ,"AllowSize" smallint NULL DEFAULT 1
            ,"AllowSort" smallint NULL DEFAULT 1
            ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			-- Summary Item
            ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
			-- Group Summary
            ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
			--,Parent BIT NULL	-- dùng cho treelist
			--,[Key] BIT NULL		-- dùng cho treelist (identity)
        );
        INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
        	select N'OpeCode',		N'Operation Code',		'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
        		UNION ALL
			select N'OpeName',		N'Operation Name',		'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
        		UNION ALL
			select N'Remarks',		N'Chú thích',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY;
		
		SELECT * FROM #tblStructure;
	select * from APZ_OOMD where "Status" = 'A';
END	



