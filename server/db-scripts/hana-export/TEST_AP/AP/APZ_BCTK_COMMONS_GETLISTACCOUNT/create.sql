
CREATE PROCEDURE APZ_BCTK_Commons_GetListAccount(in workType2 nvarchar(50) default '', in docType nvarchar(20) default '')
AS
BEGIN

	declare fromDate timestamp;
	declare toDate timestamp;
	
	select ADD_DAYS(CURRENT_DATE,-1) into fromDate from DUMMY;
	select ADD_DAYS(CURRENT_DATE,1) into toDate FROM DUMMY;

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
            	SELECT n'AcctCode', n'Account Code', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL FROM DUMMY
            		UNION ALL
				select N'AcctName',	N'Account Name','nvarchar',	200,-1,	1,	NULL,NULL,	1,	0,	1,	1,	1,	'L',NULL,	 NULL FROM DUMMY
					UNION ALL
				SELECT N'Active',N'Active',	'nvarchar',	200,-1,	1,	NULL,	NULL,1,	0,	1,	1,	1,	'L',NULL, NULL FROM DUMMY;
					   
					   
		SELECT * FROM #tblStructure;
	
	Select "AcctCode", "AcctName", "Postable" AS "Active" from OACT a where "AcctCode" NOT LIKE N'%00000000000000' order by "AcctCode";
	
	--Postable = 'Y' and ((A.validFor = 'Y' and isnull(A.frozenFor,'N') = 'N' and (isnull(a.validFrom, @fromDate) <= GETDATE() and isnull(a.validTo, @toDate) >= GETDATE())) 
	--	OR (isnull(A.validFor,'N') = 'N' and A.frozenFor = 'Y' and (isnull(a.frozenFrom, @fromDate) > GETDATE() or isnull(a.frozenTo, @toDate) < GETDATE()))
	--	or (A.validFor = 'Y' and A.frozenFor = 'Y' and (isnull(a.validFrom, @fromDate) <= GETDATE() and isnull(a.validTo, @toDate) >= GETDATE()) and
	--				(isnull(a.frozenFrom, @fromDate) > GETDATE() or isnull(a.frozenTo, @toDate) < GETDATE()))
	--	OR (A.validFor = 'N' AND A.frozenFor = 'N')) order by AcctCode 

END



