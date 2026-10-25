
CREATE PROCEDURE APZ_CFL_GetDataSalesOder 
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
                SELECT	N'DocNum',					'Doc number',			'int',			75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'DocEntry',			N'DocEntry',			'int',			75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'CardCode',			N'Card Code',			'nvarchar',		75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'CardName',			N'Card Name',			'nvarchar',		75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
				union select N'Name',				N'Contact Person',		'nvarchar',		75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
				union select N'DocStatus',			N'Status',				'char',			75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'DocDate',			N'Posting Date',		'timestamp',	75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'DocDueDate',			N'Delivery date',		'timestamp',	75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
				union select N'TaxDate',			N'Doccument Date',		'timestamp',	75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
				union select N'SlpName',			N'Sale Employee',		'nvarchar',		75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'Owner',				N'Owner',				'nvarchar',		75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
				union select N'NumAtCard',			N'NumAtCard',			'nvarchar',		75,				-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
		;
		
		
		
		
		SELECT * FROM #tblStructure; 
		SELECT a."DocEntry", a."DocNum", a."CardCode", a."CardName", b."Name", a."DocStatus", a."DocDate", a."DocDueDate", a."TaxDate", c."SlpName", d."lastName" || d."middleName" || d."firstName" AS "Owner", "NumAtCard" 
		FROM ORDR a LEFT OUTER JOIN OCPR b ON a."CntctCode" = b."CntctCode" LEFT OUTER JOIN OSLP c ON a."SlpCode" = c."SlpCode" LEFT OUTER JOIN OHEM d ON a."OwnerCode" = d."empID" WHERE IFNULL(a.CANCELED, 'N') = 'N' AND "DocStatus" = 'O' ORDER BY "DocEntry" DESC;

			
			drop table #tblStructure;
	end
			




