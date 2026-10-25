
CREATE PROCEDURE APZ_CFL_GetAllOpportunities
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
		
        INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType", "Parent", "Key")
        	select N'OprId',			N'Opr. No',				'int',			75,		0,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL,0,1 from DUMMY
        		UNION ALL
			select N'CardCode',				N'BP Code',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0 from DUMMY
        		UNION ALL
			select N'CardName',				N'BP Name',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0 from DUMMY
        		UNION ALL
			select N'Name',				N'Opr. Name',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0 from DUMMY
        		UNION ALL
			select N'Stage',			N'Stage',			'nvarchar',		75,		-1,			1,			null,		null,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,0,0 from DUMMY
        		UNION ALL
			select N'OprLine',			N'Line Id',			'nvarchar',		75,		-1,			1,			null,		null,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL,1,0 FROM DUMMY;

		SELECT * FROM #tblStructure;

		SELECT a."OpprId" AS "OprId", IFNULL(b."Line", 0) AS "OprLine", a."Name", a."CardCode", a."CardName", c."StepId", c."Descript" AS "Stage"
				 FROM OOPR a LEFT OUTER JOIN OPR1 b ON a."OpprId" = b."OpprId" LEFT OUTER JOIN OOST c ON b."Step_Id" = c."StepId" AND c."Canceled" <> 'Y';
		
END



