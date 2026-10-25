
CREATE PROCEDURE apz_users_getall

AS
BEGIN
	create local temporary table #tblStructure
				(
				 "ColumnName" NVARCHAR(50) NOT NULL
				,"Caption" NVARCHAR(100) NULL
				,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
	   			,"SubType" VARCHAR(100) NULL   --None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time
				,"Width" INT NULL DEFAULT 75
				,"GroupIndex" INT NULL DEFAULT -1 -- dùng cho grid  default -1
				,"Visible" smallint NULL DEFAULT 1
				,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
				,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
				,"ReadOnly" smallint NULL DEFAULT 0
				,"AllowEdit" smallint NULL DEFAULT 1
				,"AllowMove" smallint NULL DEFAULT 1
				,"AllowSize" smallint NULL DEFAULT 1
				,"AllowSort" smallint NULL DEFAULT 1
				,"Fixed" VARCHAR(1) NULL -- L:Left / R:Right
	   			-- Summary Item
				,"SummaryType" VARCHAR(10) NULL -- sum,average...
	   			-- Group Summary
				,"GroupSummaryType" VARCHAR(10) NULL -- sum,average...
			    ,"Parent" smallint NULL -- dùng cho treelist
			    ,"Key" smallint NULL -- dùng cho treelist (identity)
				);
	INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "FormatType", "DisplayFormat", "SummaryType", "Fixed", "SubType") 
		select n'USER_CODE', 'UserCode', 'nvarchar', 100, -1, 1, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from DUMMY
	UNION ALL
		select n'U_NAME', 'UserName', 'nvarchar', 100, -1, 1, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL FROM DUMMY;
		
	select * from #tblStructure;

	 select USER_CODE,U_NAME from OUSR;
END



