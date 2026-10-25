
CREATE PROCEDURE APZ_BCTK_Commons_GetDataDimCode(
	in dimcode nvarchar(8) default '',
	in itemCode nvarchar(50) default '',
	in docDate timestamp default '',
	in donViNhan nvarchar(10) default '',
	in department nvarchar(50) default '',
	in workType2 nvarchar(50) default '',
	in nhaMay nvarchar(50) default '',
	in dvsc nvarchar(50) default '',
	in pmNum nvarchar(50) default '',
	in storeRoom nvarchar(50) default '',
	in comodityGroup nvarchar(50) default '',
	in docType nvarchar(20) default '')
AS
BEGIN

	-- Khai bao bang Structure
		create local temporary table #tblStructure
            (
             "ColumnName" NVARCHAR(50) NOT NULL
            ,"Caption" NVARCHAR(100) NULL
            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar'  --int smallint decimal money float datetime date numeric
            ,"Width" INT NULL DEFAULT 75
            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
            ,"Visible" smallint NULL DEFAULT 1
            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime
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
            );
            
            INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType") 
			select n'PrcCode', n'Dim ' || CAST(:dimcode AS nvarchar) || ' Code', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from DUMMY
				UNION ALL
			select N'U_PrcNameVN',N'Dim ' || cast(:dimcode as nvarchar) ||' Name','nvarchar',200,	-1,	1,	NULL,NULL,	1,	0,	1,	1,	1,	'L',NULL, NULL from DUMMY;
					   
					   
		SELECT * FROM #tblStructure;

	Select "PrcCode"/*, "U_PrcNameVN" */ from OPRC where ("PrcCode" NOT LIKE N'Centr_z%') AND ("PrcCode" NOT LIKE N'A00000%') order by "PrcCode";

END



