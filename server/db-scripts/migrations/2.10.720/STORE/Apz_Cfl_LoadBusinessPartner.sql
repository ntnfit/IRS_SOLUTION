
CALL APZ_EXISTSPROC('Apz_Cfl_LoadBusinessPartner');

GO

--CALL Apz_Cfl_LoadBusinessPartner ('','manager','-1','C')
CREATE PROCEDURE Apz_Cfl_LoadBusinessPartner(
	in cardType NVARCHAR(10) default '',   
	in userName NVARCHAR(20) default '',  
	in branch NVARCHAR(20) default '',
	in bpType NVARCHAR(50) default ''
	)
AS
BEGIN
		DECLARE DspIBPDoc CHAR(1) = ifnull((SELECT "DspIBPDoc" FROM ADM1), 'N');
		declare fromDate timestamp;
		declare toDate timestamp;
		declare dofilter char(1);
		DECLARE DoMngMth CHAR(1);
        -- Khai bao bang Structure
		declare tblStructure table
            (
             "ColumnName" NVARCHAR(50) NOT NULL
            ,"Caption" NVARCHAR(100) NULL
            ,"DataType" VARCHAR(20) NULL  --int smallint decimal money float datetime date numeric
            ,"Width" INT NULL
            ,"GroupIndex" INT NULL	-- dùng cho grid  default -1
            ,"Visible" tinyint NULL
            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime
            ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
            ,"ReadOnly" tinyint NULL
            ,"AllowEdit" tinyint NULL
            ,"AllowMove" tinyint NULL
            ,"AllowSize" tinyint NULL
            ,"AllowSort" tinyint NULL
            ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			-- Summary Item
            ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
			-- Group Summary
            ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
            );
            INSERT INTO :tblStructure 
						("ColumnName",			"Caption",				"DataType",		"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
                		  select N'CardCode',			N'Code',				'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
				union all select N'CardName',			N'Name',				'nvarchar',		200,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
				union all select N'CardType',			N'Type',				'varchar',		75,	    -1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from DUMMY
				union all select N'Balance',			N'Balance',				'money',		75,		-1,			1,			'numeric',	'{0:n2}',		1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				union all select N'CntctPrsn',			N'Contact Person',		'nvarchar',		125,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				union all select N'BillTo',				N'Bill To',				'nvarchar',		175,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				union all select N'ShipTo',				N'Ship To',				'nvarchar',		175,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				union all select N'SlpCode',			N'SlpCode',				'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				union all select N'SalesPersonCode',	N'SalesPersonCode',		'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				union all select N'BillToDef',			N'BillToDef',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				union all select N'Currency',			N'Currency',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				union all select N'ContactPersonCode',	N'ContactPersonCode',	'int',			75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				union all select N'LicTradNum',			N'Lic. Trad. Num.',		'int',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				union all select N'CntPrsName',			N'Contact Person Name',	'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY
				union all select N'TotalInvc',			N'Total Invoiced',		'money',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from DUMMY;

		SELECT *  FROM :tblStructure;

		select ADD_DAYS(current_date, -1) into fromDate from DUMMY;
		select ADD_DAYS(current_date, 1) into toDate FROM DUMMY;

		select ifnull((select "DoFilter" from cinf),'N') into dofilter from DUMMY;
		
		SELECT IFNULL((SELECT "DoMngMth" FROM oadm ),'') into DoMngMth from DUMMY;
		
		call apz_func_OwnerOverShip(:userName,2, OwnnerView);
		

        SELECT A."CardCode", A."CardName", A."CardType", A."LicTradNum", A."Balance", A."CntctPrsn", B."CntctCode" AS "ContactPersonCode", B."CntctCode", A."SlpCode" AS "SlpCode", D."Street" AS "MailAddress"
        	, IFNULL(A."Currency", '##') AS "Currency", B."Name" AS "CntPrsName", CASE A."CardType" WHEN 'S' THEN 0 ELSE IFNULL((SELECT SUM("DocTotal") FROM OINV WHERE "CardCode" = a."CardCode"), 0) END AS "TotalInvc",
        	IFNULL((SELECT x."Street" FROM CRD1 x WHERE A."CardCode" = x."CardCode" AND x."AdresType" = 'B' AND A."BillToDef" = x."Address"), '') as "BillTo", 
        	IFNULL((SELECT x."Street" FROM CRD1 x WHERE A."CardCode" = x."CardCode" AND x."AdresType" = 'S' AND A."ShipToDef" = x."Address"), '') as "ShipTo"
        	FROM OCRD A 
        	LEFT JOIN OCPR B ON A."CardCode" = B."CardCode" AND A."CntctPrsn" = B."Name" 
        	LEFT OUTER JOIN CRD1 D ON A."CardCode" = D."CardCode" AND A."ShipToDef" = D."Address" AND d."AddrType" = 'S' 
        	LEFT OUTER JOIN CRD1 E ON A."CardCode" = E."CardCode" AND A."BillToDef" = E."Address" AND E."AddrType" = 'B'
            WHERE (:DspIBPDoc='Y' OR (:DspIBPDoc='N' AND ((A."validFor" = 'Y' and IFNULL(A."frozenFor",'N') = 'N' and (IFNULL(a."validFrom", :fromDate) <= current_date and IFNULL(a."validTo", :toDate) >= current_date)) 
			OR (IFNULL(A."validFor",'N') = 'N' and A."frozenFor" = 'Y' and (IFNULL(a."frozenFrom", :fromDate) > current_date or IFNULL(a."frozenTo", :toDate) < current_date))
			or (A."validFor" = 'Y' and A."frozenFor" = 'Y' and (IFNULL(a."validFrom", :fromDate) <= current_date and IFNULL(a."validTo", :toDate) >= current_date) and
			(IFNULL(a."frozenFrom", :fromDate) > current_date or IFNULL(a."frozenTo", :toDate) < current_date))
			OR (IFNULL(A."validFor",'N') = 'N' AND IFNULL(A."frozenFor",'N') = 'N')))) AND (IFNULL(:cardType,'') = '' or "CardType" IN (SELECT data FROM Split(:cardType,',')))
			AND (:dofilter='N' OR (:dofilter='Y' AND :DoMngMth NOT IN ('A','B')) OR (:dofilter='Y' AND :DoMngMth IN ('A','B') AND (IFNULL(A."OwnerCode",-1)=-1 OR (A."OwnerCode" IN (select distinct x."OwnerId" from :OwnnerView x)))))
			AND (iFnull(:bpType,'') = '' OR (:bpType <> '' AND A."CmpPrivate" = :bpType)) order by A."CardCode";
    	 	
    END
	