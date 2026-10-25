
CREATE PROCEDURE apz_retail_cfl_item(
in UserName NVARCHAR(50) DEFAULT '',
in StoreCode NVARCHAR(50) DEFAULT '',
in ItemCode NVARCHAR(50) DEFAULT '',
in CardCode NVARCHAR(50) DEFAULT '',
in Codebars NVARCHAR(254) DEFAULT ''
)
AS
BEGIN
	DECLARE WhsDefault VARCHAR(20);
	DECLARE PriceList INT = 1;
	DECLARE fromDate Date;
	DECLARE toDate Date;
	select ADD_DAYS(CURRENT_DATE,-1) into fromDate from Dummy;
	select ADD_DAYS(CURRENT_DATE,1) into toDate from Dummy;
	if(ifnull(:codebars,'')<>'')
 	then
	select ifnull((select top 1 "ItemCode" from OBCD WHERE "BcdCode"=:Codebars),'') into ItemCode from dummy;
	end if;
	
	 select ifnull((SELECT "Warehouse" FROM OUDG WHERE "Code"=(SELECT TOP 1 "DfltsGroup" FROM OUSR WHERE "USER_CODE"=:userName)),'') into WhsDefault from Dummy;
		
		-- Khai báo bảng dữ liệu combobox (tùy chọn)
		create local temporary table #tblValidValue
		(
		 "ColumnName" NVARCHAR(50) NOT NULL
		, "Value" NVARCHAR(254) NOT NULL
		, "Display" NVARCHAR(254) NOT NULL
		);

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
                SELECT	N'ItemCode',		N'Item Code',			'nvarchar',		250,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
					   union select N'ItemDescription',N'Item Name',			'nvarchar',		300,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from Dummy
					   union select N'CodeBars',		N'Bar Code',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		NULL,	NULL,		 NULL from Dummy
					   union select N'FrgnName',		N'FrgnName',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'U_OldCode',		N'Mã cũ',				'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'ItmsGrpCod',		N'ItmsGrpCod',			'int',			75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'SalesVATGroup',	N'SalesVATGroup',		'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'VatGroupPu',		N'VatGroupPu',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'VatGourpSa',		N'VatGourpSa',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'QuantityOnStock',N'QuantityOnStock',		'numeric',		75,		-1,			1,			'numeric',	'{0:n2}',		1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'Currency',		N'Currency',			'numeric',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'Price',			N'Price',				'nvarchar',		75,		-1,			0,			'numeric',	'{0:n2}',		1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'PriceList',		N'PriceList',			'int',			75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'RateSa',			N'RateSa',				'numeric',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'RatePu',			N'RatePu',				'numeric',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'BOMTreeType',	N'BOMTreeType',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'VatPuInActive',	N'VatPuInActive',		'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'VatSaInActive',	N'VatSaInActive',		'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'TreeType',		N'TreeType',			'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'WarehouseCode',	N'WarehouseCode',		'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy
					   union select N'Substitute',		N'Bp Catalog Num',		'nvarchar',		75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL from Dummy;

		SELECT *
            FROM #tblStructure;

		

        SELECT A."ItemCode", 
		A."ItemName" AS "ItemDescription", 
		A."CodeBars",
		A."ItemName" AS "ItemName",
		M."Substitute",
		IFNULL(A."FrgnName", '') AS "FrgnName", 
		IFNULL(A."ItmsGrpCod", 0) AS "ItmsGrpCod",
        CASE WHEN IFNULL(OCRD."ECVatGroup",'') ='' then A."VatGourpSa" ELSE OCRD."ECVatGroup" end  AS "SalesVATGroup",
		CASE WHEN IFNULL(OCRD."ECVatGroup",'') ='' THEN A."VatGroupPu" ELSE OCRD."ECVatGroup" end  AS "VatGroupPu", 
		CASE WHEN IFNULL(OCRD."ECVatGroup",'') ='' THEN A."VatGourpSa" ELSE OCRD."ECVatGroup" end  AS "VatGourpSa",
		CASE WHEN :WhsDefault <>'' THEN :WhsDefault else  a."DfltWH" end AS "WarehouseCode",
		CASE WHEN :WhsDefault <>'' THEN :WhsDefault else  a."DfltWH" end AS "DefaultWarehouse",
		CASE WHEN :WhsDefault <>'' THEN :WhsDefault else  a."DfltWH" end AS "DfltWH",
        (SELECT SUM(IFNULL("OnHand", 0) - IFNULL("IsCommited", 0) + IFNULL("OnOrder", 0))FROM OITW WHERE "ItemCode" = A."ItemCode" AND "Locked" = 'N') AS "QuantityOnStock",  
		OCRD."ListNum" "PriceList",
		CASE WHEN IFNULL(OCRD."ECVatGroup",'') ='' THEN IFNULL(k."Rate", 0) ELSE ovtg."Rate" end  AS "RatePu", 
		CASE WHEN IFNULL(OCRD."ECVatGroup",'') ='' THEN IFNULL(g."Rate", 0) ELSE ovtg."Rate" end  AS "RateSa",
		l."TreeType" AS "BOMTreeType", 
		IFNULL(k."Inactive", '') AS "VatPuInActive",
        IFNULL(g."Inactive", '') AS "VatSaInActive",
		case IFNULL(l."TreeType",'N')
			when 'A' then 1
			when 'S' then 2
			when 'P' then 3
			when 'T' then 4
			when 'N' then
					case IFNULL((select "TreeType" from OITT where "Code" = (select "Father" from (select "Father", row_number () over 
               (partition by itt1."Father" 
                ORDER BY itt1."Father" asc) as RN from itt1 where "Code"=a."ItemCode") where rn = 2)) ,'N')
					when 'S' then 5
					else 0
				End
		end AS "TreeType", 
		A."BuyUnitMsr", 
		A."SalUnitMsr",
		A."NumInBuy",
		A."NumInSale",
		A."InvntryUom",
		a."UgpEntry" as "UgpCode",
		IFNULL(a1."AltQty", 1) / IFNULL(a1."BaseQty", 1) as "NumInInven"
		,T0."Price"
		,T0."Currency"
		,0 "Discount"
            FROM OITM A
			--INNER JOIN OBCD T1 on A.ItemCode = T1.ItemCode and T1.BcdCode = A.CodeBars
            LEFT JOIN OSHP D ON A."ShipType" = D."TrnspCode"
            LEFT JOIN OMRC E ON A."FirmCode" = E."FirmCode"
            LEFT JOIN OITB f ON A."ItmsGrpCod" = f."ItmsGrpCod"
            LEFT JOIN OVTG g ON A."VatGourpSa" = g."Code"
            LEFT JOIN OVTG k ON A."VatGroupPu" = k."Code"
            LEFT JOIN OITT l ON l."Code" = A."ItemCode"
			LEFT JOIN OSCN M on a."ItemCode" = M."ItemCode" and M."CardCode" = :CardCode
			left join UGP1 a1 on a."UgpEntry" = a1."UgpEntry" and a1."UomEntry" in (select "UomEntry" from OUOM where "UomName" = a."InvntryUom")
			LEFT JOIN OCRD 
			LEFT JOIN OVTG ON OCRD."ECVatGroup"=OVTG."Code" ON OCRD."CardCode"=:CardCode
			LEFT JOIN ITM1 T0 ON A."ItemCode"=T0."ItemCode" 
			left join APZ_STORE store on store."ListNum"=T0."PriceList"
			
WHERE ((A."validFor"= 'Y' and IFNULL(A."frozenFor",'N') = 'N' and (IFNULL(a."validFrom", :fromDate) <= CURRENT_DATE and IFNULL(a."validTo", :toDate) >= CURRENT_DATE)) 
		OR (IFNULL(A."validFor",'N') = 'N' and A."frozenFor" = 'Y' and (IFNULL(a."frozenFrom", :fromDate) > CURRENT_DATE or IFNULL(a."frozenTo", :toDate) < CURRENT_DATE))
		or (A."validFor" = 'Y' and A."frozenFor" = 'Y' and (IFNULL(a."validFrom", :fromDate) <= CURRENT_DATE and IFNULL(a."validTo", :toDate) >= CURRENT_DATE) and
					(IFNULL(a."frozenFrom", :fromDate) > CURRENT_DATE or IFNULL(a."frozenTo", :toDate) < CURRENT_DATE))
		OR (A."validFor" = 'N' AND A."frozenFor" = 'N')) AND 
		(IFNULL(:Codebars,'')='' AND (:itemCode = '' OR (:itemCode != '' AND A."ItemCode" = :itemCode))  or (IFNULL(:Codebars,'')<>'' AND A."ItemCode"=:ItemCode)) 
		AND (  A."InvntItem" = 'Y')  and ((IFnull(:StoreCode,'')='') OR (:StoreCode != '' AND store."Code"=:StoreCode));
		
		--AND (@Codebars = '' OR (@Codebars != '' AND .ItemCode = @Codebars))
	Drop table #tblValidValue;
	Drop table #tblStructure;
END




