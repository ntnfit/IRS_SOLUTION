
Create PROCEDURE APZ_Cfl_GetItemWarrantyTemplates
(in CntrctType nvarchar(1) default '') -- C Customer  G Item Group  S Serial Number
as
BEGIN
	create local temporary table #tblStructure
	    (
	     "ColumnName" NVARCHAR(50) NOT NULL
	    ,"Caption" NVARCHAR(100) NULL
	    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
		,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
	    ,"Width" INT NULL DEFAULT 75
	    ,"GroupIndex" INT NULL DEFAULT -1	-- dng cho grid  default -1
	    ,"Visible" char(1) NULL DEFAULT '1'
	    ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
	    ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
	    ,"ReadOnly" char(1) NULL DEFAULT '0'
	    ,"AllowEdit" char(1) NULL DEFAULT '1'
	    ,"AllowMove" char(1) NULL DEFAULT '1'
	    ,"AllowSize" char(1) NULL DEFAULT '1'
	    ,"AllowSort" char(1) NULL DEFAULT '1'
	    ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
		-- Summary Item
	    ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
		-- Group Summary
	    ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
		--,Parent BIT NULL	-- dng cho treelist
		--,[Key] BIT NULL		-- dng cho treelist (identity)
	    );
	 INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
	    select n'TmpltName', n'Template Name', 'nvarchar', 100, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL from DUMMY
union all select n'CntrctType', n'Contract Type', 'nvarchar', 200, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL from DUMMY;
			
	SELECT * FROM #tblStructure;
	
	
	SELECT "TmpltName", CASE "CntrctType" WHEN 'C' THEN 'Customer' WHEN 'G' THEN 'Item Group' WHEN 'S' THEN 'Serial Number' END AS "CntrctType" FROM OCTT WHERE "CntrctType" = :CntrctType;
end;




