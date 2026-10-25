
CREATE PROCEDURE APZ_BCTK_Commons_GetListProject(
	in docDate datetime default '',
	in docType nvarchar(20) default '',
	in cardCode nvarchar(50) default '',
	in typeCode int default 1)
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
				select n'PrjCode', n'Project Code', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from dummy
					UNION ALL
				select N'PrjName',N'Project Name',	'nvarchar',	200,-1,1,NULL,NULL,	1,	0,	1,	1,	1, 'L',	NULL, NULL from DUMMY;
					   
					   
		SELECT * FROM #tblStructure;

	--if @type = 1 AND exists (select top 1 1 from OPRJ where U_BPOwner = @cardCode)
	--Select PrjCode, PrjName from OPRJ where U_BPOwner = @cardCode
	--UNION ALL
	--Select PrjCode, PrjName from OPRJ where isnull(U_BPOwner,'') = '' 
	--UNION ALL
	--Select PrjCode, PrjName from OPRJ where U_TypeHM = 'Y'
	--order by PrjCode
	--else 
	--Select PrjCode, PrjName from OPRJ order by PrjCode

	Select "PrjCode", "PrjName" /*, "U_BPOwner", "U_BPOwnerName", "U_D4Owner", "U_D4OwnerName", "U_D3Owner", "U_D3OwnerName" */ from OPRJ /* where ("U_BPOwner" NOT LIKE N'A0000%')*/ order by "PrjCode";

END



