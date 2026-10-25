
CREATE PROCEDURE apz_retail_cfl_customer(
	UserName NVARCHAR(50) DEFAULT '',
	StoreCode NVARCHAR(50) DEFAULT '',
	CardCode NVARCHAR(50) DEFAULT '')
AS
    BEGIN
    DECLARE fromDate Date;
	DECLARE toDate Date;
	select ADD_DAYS(CURRENT_DATE,-1) into fromDate from Dummy;
	select ADD_DAYS(CURRENT_DATE,1) into toDate from Dummy;
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
                SELECT	N'CardCode',			N'Customer Code',		'nvarchar',		250,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy 
					   union select N'CardName',			N'Customer Name',		'nvarchar',		300,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy 
					   union select N'Balance',			N'Balance',				'money',		250,		-1,			1,			'numeric',	'{0:n2}',		1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy 
					   union select N'CntctPrsn',			N'Contact Person',		'nvarchar',		250,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy 
					   union select N'SlpCode',			N'Sales Employee',		'nvarchar',		250,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy 
					   union select N'SalesPersonCode',	N'SalesPersonCode',		'nvarchar',		250,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy 
					   union select N'BillToDef',			N'Bill-to Default',			'nvarchar',		250,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy 
					   union select N'Currency',			N'Currency',			'nvarchar',		250,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy 
					   union select N'ContactPersonCode',	N'ContactPersonCode',	'int',			250,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy 
					   union select N'LicTradNum',			N'Federal Tax ID',			'int',			250,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy 
					   union select N'CntPrsName',			N'Contact Person',	'nvarchar',			250,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy 
					   union select N'Cellular',			N'Mobile Phone',	'nvarchar',			250,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy 
					   union select N'AddID',			N'ID Number',	'nvarchar',			250,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy;

		SELECT *
            FROM #tblStructure;

		

        SELECT A."CardCode", A."CardName",A."LicTradNum", A."Balance", A."CntctPrsn", B."CntctCode" AS "ContactPersonCode",
                        A."SlpCode" AS "SlpCode", D."Street" AS "Street", IFNULL(A."Currency", '##') AS "Currency"
						,B."Name" as "CntPrsName",A."Address",A."Cellular",A."MailAddres" ,A."Balance",A."AddID"
                    FROM OCRD A
                    LEFT JOIN OCPR B ON A."CardCode" = B."CardCode" AND A."CntctPrsn" = B."Name"
                    LEFT JOIN CRD1 D ON A."CardCode" = D."CardCode" AND A."ShipToDef" = D."Address" AND d."AddrType"='S'
                    LEFT JOIN CRD1 E ON A."CardCode" = E."CardCode" AND A."BillToDef" = E."Address" AND E."AddrType"='B'
                    WHERE ((A."validFor" = 'Y' and IFNULL(A."frozenFor",'N') = 'N' and (IFNULL(a."validFrom", :fromDate) <= CURRENT_DATE and IFNULL(a."validTo", :toDate) >= CURRENT_DATE)) 
					OR (IFNULL(A."validFor",'N') = 'N' and A."frozenFor" = 'Y' and (IFNULL(a."frozenFrom", :fromDate) > CURRENT_DATE or IFNULL(a."frozenTo", :toDate) < CURRENT_DATE))
					or (A."validFor" = 'Y' and A."frozenFor" = 'Y' and (IFNULL(a."validFrom", :fromDate) <= CURRENT_DATE and IFNULL(a."validTo", :toDate) >= CURRENT_DATE) and
								(IFNULL(a."frozenFrom", :fromDate) > CURRENT_DATE or IFNULL(a."frozenTo", :toDate) < CURRENT_DATE))
					OR (A."validFor" = 'N' AND A."frozenFor" = 'N')) AND  "CardType" = 'C' AND ((IFNULL(:CardCode,'') = '' or (:CardCode <> ''and A."CardCode" like '%'|| :CardCode || '%')));
					
					drop table #tblStructure;
    END
	




