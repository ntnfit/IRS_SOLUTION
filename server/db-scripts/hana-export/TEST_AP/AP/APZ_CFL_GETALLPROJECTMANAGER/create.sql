
CREATE PROCEDURE APZ_CFL_GetAllProjectManager
AS
BEGIN
	create local temporary table #tblStructure
            (
	             "ColumnName" NVARCHAR(50) NOT NULL
	            ,"Caption" NVARCHAR(100) NULL
	            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
				,"SubType" VARCHAR(20) NULL			--None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time
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
				,"Parent" smallint NULL	-- dùng cho treelist
				,"Key" smallint NULL		-- dùng cho treelist (identity)
			);
			
        INSERT INTO #tblStructure("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType", "Parent", "Key")
        	select N'AbsEntry',			N'Id',				'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL,0,1 from DUMMY
        		UNION ALL
			select N'Code',				N'Code',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0 FROM DUMMY
				UNION ALL
			SELECT N'Name',				N'Name',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0  FROM DUMMY
				UNION ALL
			SELECT N'PrgStatus',			N'Status',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0  FROM DUMMY
				UNION ALL
			SELECT N'StartDate',			N'Start Date',		'datetime',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0  FROM DUMMY
				UNION ALL
			SELECT N'Priority',			N'Priority',		'nvarchar',			75,		-1,			0,			null,		null,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0  FROM DUMMY
				UNION ALL
			SELECT N'CardCode',			N'BP Code',			'nvarchar',		75,		-1,			1,			null,		null,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0  FROM DUMMY
				UNION ALL
			SELECT N'CardName',			N'BP Name',			'nvarchar',		75,		-1,			1,			null,		null,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0  FROM DUMMY
				UNION ALL 
			SELECT N'FatherId',			N'FatherId',			'nvarchar',		75,		-1,			1,			null,		null,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,1,0 FROM DUMMY;			 

		SELECT * FROM #tblStructure;

		select *,"MasterEntry" as "FatherId" from APZ_OPRG;
END




