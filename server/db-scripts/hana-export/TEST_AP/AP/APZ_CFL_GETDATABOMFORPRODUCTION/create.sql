
CREATE PROCEDURE APZ_Cfl_GetDataBOMForProduction
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
                SELECT	N'Code',				N'Code',					'nvarchar',			75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'ItemName',		N'Item Description',		'nvarchar',			75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'OnHand',			N'In Stock',				'decimal',			75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'ToWH',			N'To WH',					'nvarchar',			75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
			
					;
		
		
		
		
		SELECT * FROM #tblStructure; 
	
	CREATE local temporary TABLE "#tmpLocal" AS (SELECT "ItemCode", SUM("OnHand") AS "OnHand" FROM OITW GROUP BY "ItemCode");
SELECT a."Code", b."ItemName", c."OnHand", a."ToWH", a."OcrCode", a."OcrCode2", a."OcrCode3", a."OcrCode4", a."OcrCode5" FROM OITT a INNER JOIN OITM b ON a."Code" = b."ItemCode" LEFT OUTER JOIN "#tmpLocal" c ON a."Code" = c."ItemCode" WHERE a."TreeType" = 'P' ORDER BY a."Code";
drop table "#tmpLocal";
			drop table #tblStructure;
			
	end
			




