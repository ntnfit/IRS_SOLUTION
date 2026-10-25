
CREATE PROCEDURE Apz_CFL_GetDataBanks
AS
BEGIN
    -- Khai bao bang Structure
	CREATE LOCAL TEMPORARY TABLE #tblStructure
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
    	select N'Country',			N'Bank Country',		'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
    		UNION ALL
		select N'BankCode',			N'Bank Code',			'nvarchar',		200,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
    		UNION ALL
		SELECT N'BankName',			N'Bank Name',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
    		UNION ALL
		SELECT N'CountryCod',			N'CountryCod',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
    		UNION ALL
		SELECT N'AbsEntry',			N'AbsEntry',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
    		UNION ALL
		SELECT N'SwiftNum',			N'SwiftNum',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY;
					   
		SELECT * FROM #tblStructure;

		SELECT b."Name" AS "Country", "BankCode", "BankName", a."CountryCod" AS "CountryCod", a."AbsEntry" AS "AbsEntry", a."SwiftNum" AS "SwiftNum" FROM ODSC a INNER JOIN OCRY b ON a."CountryCod" = b."Code";
		
end;		



