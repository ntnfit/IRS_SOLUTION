
CREATE PROCEDURE APZ_CFL_GetDataBOMCFLOfProduct 
(
in	val NVARCHAR(5000) DEFAULT '' 
,in   userName NVARCHAR(20) DEFAULT ''
,in   branch NVARCHAR(20) DEFAULT ''
,in   empBranch int DEFAULT 0
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
                SELECT	N'ItemCode',				'Item Code',			'nvarchar',		75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'ItemName',			N'Item Name',			'nvarchar',		150,			-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'InStock',			N'In Stock',			'nvarchar',		75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
			;
		SELECT * FROM #tblStructure; 
		SELECT  
		a."ItemCode"
		,a."ItemName"
		,SUM(b."OnHand") as "InStock"
	 FROM OITM a LEFT JOIN OITW b ON a."ItemCode" = b."ItemCode"
	 GROUP BY a."ItemCode",a."ItemName"
	 ORDER by a."ItemCode";
			drop table #tblStructure;
	end
			




