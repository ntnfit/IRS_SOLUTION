
CREATE procedure apz_filter_getfilter(in Category nvarchar(20))
as
begin
	create local temporary table #tblValidValue
		    (
		    	"ColumnName" NVARCHAR(50) NOT NULL
			    ,"Description" NVARCHAR(254) NOT NULL
			    ,"Value" NVARCHAR(254) NOT NULL
		    );
				
 	create local temporary table #tblStructure
            (
	             "ColumnName" NVARCHAR(50) NOT NULL
	            ,"Caption" NVARCHAR(100) NULL
	            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
				,"SubType" VARCHAR(100) NULL			--None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time
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
				,"Key" smallint NULL	-- dùng cho treelist (identity)
            );
	INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "FormatType", "DisplayFormat", "SummaryType", "Fixed", "SubType") 
		select n'Code', 'Code', 'nvarchar', 100, -1, 1, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL FROM DUMMY
	UNION ALL
		select n'Name', 'Name', 'nvarchar', 100, -1, 1, 0, 1, 0, 1, 1, NULL, NULL, NULL, '', NULL FROM DUMMY
	UNION ALL
		select n'FilterText', 'Filter Text', 'nvarchar', 100, -1, 1, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from DUMMY
	UNION ALL
		select n'Valid', 'Valid', 'bit', 35, -1, 1, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL FROM DUMMY
	UNION ALL
		select n'ObjectType', 'Category', 'nvarchar', 100, -1, 0, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL FROM DUMMY;

	select * from #tblStructure;
	
	select * from APZ_FILTER WHERE "ObjType"=:Category;
	
end;



