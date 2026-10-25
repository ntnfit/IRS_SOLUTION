
create procedure APZ_ProductionOrder_GetSearchStructure()
as
BEGIN

	create local temporary table #tblValidValue
		(
			"ColumnName" NVARCHAR(50) NOT NULL
			,"Value" NVARCHAR(254) NOT NULL
			,"Description" NVARCHAR(254) NOT NULL
		);
		
		insert into #tblValidValue values ('Status', 'P', 'Planned');
		insert into #tblValidValue values ('Status', 'R', 'Released');
		insert into #tblValidValue values ('Status', 'L', 'Closed');
		insert into #tblValidValue values ('Status', 'C', 'Canceled');
		
		
		insert into #tblValidValue values ('Type', 'S', 'Standard');
		insert into #tblValidValue values ('Type', 'P', 'Special');
		insert into #tblValidValue values ('Type', 'D', 'Disassembly');
		
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
			select N'DocEntry', n'Document Entry', 'int', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from DUMMY
		 UNION ALL
		 	select N'DocNum', n'Document Number', 'int', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from DUMMY
		 UNION ALL
			select N'ItemCode', n'Item Code', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from DUMMY
		 UNION ALL
			select n'ItemName', n'Item Name', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from DUMMY
		 UNION ALL
			select n'Type', n'Type', 'validvalue', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL from DUMMY
		 UNION ALL
			select n'Status', n'Status', 'validvalue', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		 UNION ALL
			select n'Comments', n'Comments', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		 UNION ALL
			select n'Warehouse', n'Warehouse', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		UNION ALL
			select n'PlannedQty', n'PlannedQty', 'numeric', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		UNION ALL
			select n'CmpltQty', n'CmpltQty', 'numeric', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		UNION ALL
			select n'RjctQty', n'RjctQty', 'numeric', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		UNION ALL
			select n'DueDate', n'Due Date', 'datetime', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY;

		SELECT *  FROM #tblStructure;
		
		select * from #tblValidValue;

END;




