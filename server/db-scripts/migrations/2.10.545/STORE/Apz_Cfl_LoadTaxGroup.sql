
CALL APZ_EXISTSPROC('Apz_Cfl_LoadTaxGroup');

GO

CREATE PROCEDURE Apz_Cfl_LoadTaxGroup(
	in cardType NVARCHAR(10) default '',   
	in vatGroup NVARCHAR(10) default ''
	)
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
            ,"Visible" tinyint NULL DEFAULT 1
            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime
            ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
            ,"ReadOnly" tinyint NULL DEFAULT 0
            ,"AllowEdit" tinyint NULL DEFAULT 1
            ,"AllowMove" tinyint NULL DEFAULT 1
            ,"AllowSize" tinyint NULL DEFAULT 1
            ,"AllowSort" tinyint NULL DEFAULT 1
            ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			-- Summary Item
            ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
			-- Group Summary
            ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
            );
		INSERT INTO #tblStructure 
			("ColumnName",			"Caption",				"DataType",		"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
        		Select N'Code',			N'Code',				'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
		Union All Select N'Name',			N'Name',				'nvarchar',		200,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
		Union All Select N'Rate',			N'Rate %',				'double',		75,	    -1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY;

		SELECT * FROM #tblStructure;

		IF IFNULL(:cardType,'') = 'C' Then
			IF IFNULL(:vatGroup,'N') = 'Y' Then
				Select "Code","Name","Rate" From OVTG Where "Category" = 'O' AND IFNULL("Inactive",'N') = 'N' AND IFNULL("Account",'') LIKE '33311%';
			Else
				Select "Code","Name","Rate" From OVTG Where "Category" = 'O' AND IFNULL("Inactive",'N') = 'N' And IFNULL("Rate",0) = 0 AND IFNULL("Account",'') LIKE '33311%';
			End If;
		Else
			IF IFNULL(:vatGroup,'') = 'N' Then
				Select "Code","Name","Rate" From OVTG Where "Category" = 'I' AND IFNULL("Inactive",'N') = 'N' And IFNULL("Account",'') LIKE '1331%';
			Else
				Select "Code","Name","Rate" From OVTG Where "Category" = 'I' AND IFNULL("Inactive",'N') = 'N' And IFNULL("Rate",0) = 0 And IFNULL("Account",'') LIKE '1331%';
			End If;
		End If;
    	 	
    	drop table #tblStructure;
END;