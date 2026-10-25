
create procedure APZ_BP_GetSearchStructure()
as
BEGIN

	create local temporary table #tblValidValue
		(
			"ColumnName" NVARCHAR(50) NOT NULL
			,"Value" NVARCHAR(254) NOT NULL
			,"Description" NVARCHAR(254) NOT NULL
		);
		
		insert into #tblValidValue select 'GroupCode', "GroupCode", "GroupName" from OCRG;
		
		insert into #tblValidValue values ('CardType', 'S', 'Vendor');
		insert into #tblValidValue values ('CardType', 'C', 'Customer');
		insert into #tblValidValue values ('CardType', 'L', 'Lead');

		-- Khai báo bảng Structure
		create local temporary table #tblStructure
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
			select N'CardCode', n'Card Code', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from DUMMY
		 UNION ALL
			select n'CardName', n'Card Name', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from DUMMY
		 UNION ALL
			select n'CardType', n'Card Type', 'validvalue', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL from DUMMY
		 UNION ALL
			select n'GroupCode', n'Card Group', 'validvalue', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		 UNION ALL
			select n'Address', n'Address', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		 UNION ALL
			select n'LicTradNum', n'Federal Tax ID', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		 UNION ALL
			select n'Phone1', n'Phone', 'numeric', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY;

		SELECT *  FROM #tblStructure;
		
		select * from #tblValidValue;

END;




