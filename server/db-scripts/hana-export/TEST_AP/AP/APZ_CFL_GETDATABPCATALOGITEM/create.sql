
CREATE PROCEDURE APZ_CFL_GetDataBpCatalogItem(
	in bpCode nvarchar(15) default '',
	in subCatNum nvarchar(50) default '',
	in userName NVARCHAR(20) default '',
	in branch NVARCHAR(20) default '',
	in DocDate timestamp default '',
	in itemCode nvarchar(50) default '')

AS
BEGIN

		------------------------------------- VARIABLE -------------------------------------
		DECLARE DsplITMDoc CHAR(1);
		DECLARE WhsDefault VARCHAR(20);
		declare fromDate timestamp;
		declare toDate timestamp;
		declare checkCount int = 0;
		------------------------------------- END VARIABLE ---------------------------------
		-- bỏ lấy theo item
		itemCode = '';
	
		select (SELECT "DspIITMDoc" FROM ADM1) into DsplITMDoc FROM DUMMY;
		
		select IFNULL((SELECT "Warehouse" FROM OUDG WHERE "Code"=(SELECT TOP 1 "DfltsGroup" FROM OUSR WHERE USER_CODE=:userName)),'') into WhsDefault FROM DUMMY;
		
		IF :WhsDefault=''
		THEN
			select IFNULL((SELECT "DflWhs" FROM OBPL WHERE "BPLId"=:branch),'') into WhsDefault FROM DUMMY;
        END	IF;

		create local temporary table #tblPrice("ItemCode" nvarchar(50) null, "Price" numeric(19,6) null, "Currency" nvarchar(3) null, "Discount" numeric(19,6) null, "CardCode" nvarchar(50) null);
		
		insert into #tblPrice
			SELECT a."ItemCode", IFNULL(b."Price", a."Price") AS "Price", a."Currency", a."Discount", a."CardCode" 
					FROM OSPP a LEFT OUTER JOIN SPP1 b ON b."ItemCode" = a."ItemCode" AND b."CardCode" = a."CardCode" AND :docdate BETWEEN b."FromDate" AND IFNULL(b."ToDate", '2099-12-31')
						 WHERE a."CardCode" = :bpCode;
		select ifnull((SELECT top 1 1 FROM #tblPrice),0) into checkCount from dummy;
		
		IF checkCount = 0
		THEN
			INSERT INTO #tblPrice ("ItemCode", "Price", "Currency", "Discount", "CardCode") 
				SELECT a."ItemCode", a."Price", a."Currency", 0 AS "Discount", b."CardCode" FROM ITM1 a INNER JOIN ocrd b ON a."PriceList" = b."ListNum" AND b."CardCode" = :bpCode;

		END IF;

		-- Khai báo bảng dữ liệu combobox (tùy chọn)
		create local temporary table #tblValidValue
		(
			"ColumnName" NVARCHAR(50) NOT NULL
			,"Value" NVARCHAR(254) NOT NULL
			,"Display" NVARCHAR(254) NOT NULL
		);

		-- Khai báo bảng Structure
		Create local temporary table #tblStructure
            (
	             "ColumnName" NVARCHAR(50) NOT NULL
	            ,"Caption" NVARCHAR(100) NULL
	            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
				,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
	            ,"Width" INT NULL DEFAULT 75
	            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
	            ,"Visible" tinyint NULL DEFAULT 1
	            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
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
				--,Parent BIT NULL	-- dùng cho treelist
				--,[Key] BIT NULL		-- dùng cho treelist (identity)
            );
            
       INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType") 
			select n'ItemCode', n'Item Code', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL FROM DUMMY
		UNION ALL
			select n'ItemDescription', n'Item Name', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from dummy
		UNION ALL
			select n'CardCode', n'BP Code', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		UNION ALL
			select n'CardName', n'BP Name', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		UNION ALL
			select n'SubCatNum', n'Bp Catalog Num', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		UNION ALL
			select n'QuantityOnStock', n'QuantityOnStock', 'numeric', 75, -1, 1, 'numeric', '{0:n2}', 1, 0, 1, 1, 1, NULL, NULL, NULL from DUMMY
		UNION ALL
			select n'Currency', n'Currency', 'numeric', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL from dummy
		UNION ALL
			select n'Price', n'Price', 'nvarchar', 75, -1, 0, 'numeric', '{0:n2}', 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		UNION ALL
			select n'PriceList', n'PriceList', 'int', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL from DUMMY
		UNION ALL
			select n'BOMTreeType', n'BOMTreeType', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL from DUMMY
		UNION ALL
			select n'TreeType', n'TreeType', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL from DUMMY
		UNION ALL
			select n'WarehouseCode', n'WarehouseCode', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY;
		  
		SELECT * FROM #tblStructure;

		select ADD_DAYS(current_date,-1) into fromDate FROM DUMMY;
		select ADD_DAYS(current_date,1) into toDate FROM DUMMY;

        SELECT A."ItemCode", A."ItemName" AS "ItemDescription", A."ItemName" AS "ItemName", m."Substitute" AS "SubCatNum", IFNULL(A."FrgnName", '') AS "FrgnName", IFNULL(A."ItmsGrpCod", 0) AS "ItmsGrpCod"
        	, CASE WHEN IFNULL(t1."ECVatGroup", '') = '' THEN A."VatGourpSa" ELSE t1."ECVatGroup" END AS "SalesVATGroup"
        	, CASE WHEN IFNULL(t1."ECVatGroup", '') = '' THEN A."VatGroupPu" ELSE t1."ECVatGroup" END AS "VatGroupPu"
        	, CASE WHEN IFNULL(t1."ECVatGroup", '') = '' THEN A."VatGourpSa" ELSE t1."ECVatGroup" END AS "VatGourpSa"
        	, CASE WHEN :WhsDefault <> '' THEN :WhsDefault ELSE a."DfltWH" END AS "WarehouseCode", CASE WHEN :WhsDefault <> '' THEN :WhsDefault ELSE a."DfltWH" END AS "DefaultWarehouse"
        	, CASE WHEN :WhsDefault <> '' THEN :WhsDefault ELSE a."DfltWH" END AS "dfltWH"
        	, (SELECT SUM(IFNULL("OnHand", 0) - IFNULL("IsCommited", 0) + IFNULL("OnOrder", 0)) FROM OITW WHERE "ItemCode" = A."ItemCode" AND "Locked" = 'N') AS "QuantityOnStock"
        	, t1."ListNum" AS "PriceList"
        	, CASE WHEN IFNULL(t1."ECVatGroup", '') = '' THEN CASE WHEN IFNULL(A."VatGourpSa", '') = '' THEN IFNULL(k."Rate", 0) ELSE IFNULL(k."Rate", 0) END ELSE ovtg."Rate" END AS "RatePu"
        	, CASE WHEN IFNULL(t1."ECVatGroup", '') = '' THEN CASE WHEN IFNULL(A."VatGroupPu", '') = '' THEN IFNULL(g."Rate", 0) ELSE IFNULL(g."Rate", 0) END ELSE ovtg."Rate" END AS "RateSa"
        	, l."TreeType" AS "BOMTreeType", IFNULL(k."Inactive", '') AS "VatPuInActive", IFNULL(g."Inactive", '') AS "VatSaInActive"
        	, CASE IFNULL(l."TreeType", 'N') WHEN 'A' THEN 1 WHEN 'S' THEN 2 WHEN 'P' THEN 3 WHEN 'T' THEN 4 WHEN 'N' THEN CASE IFNULL((SELECT "TreeType" FROM OITT xx WHERE "Code" = a."ItemCode" and exists(SELECT 1 FROM itt1 WHERE "Father" = xx."Code")), 'N') WHEN 'S' THEN 5 ELSE 0 END END AS "TreeType"
        	, g."Rate" AS "RateSa", A."BuyUnitMsr", A."SalUnitMsr", A."NumInBuy", A."NumInSale", A."InvntryUom", a."UgpEntry" AS "UgpCode", IFNULL(a1."AltQty", 1) / IFNULL(a1."BaseQty", 1) AS "NumInInven"
        	, IFNULL(p."Price", 0) AS "Price", IFNULL(p."Currency", '') AS "Currency", IFNULL(p."Discount", 0) AS "Discount", CASE WHEN a."ManBtchNum" = 'Y' THEN 'B' WHEN a."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBatchSerial"
        	, IFNULL(a."OnHand", 0) AS "OnStock", IFNULL(t2."OnHand", 0) AS "OnWhs", IFNULL(a."IsCommited", 0) AS "IsCommited", IFNULL(t2."IsCommited", 0) AS "IsCommitedInWhs", IFNULL(a."OnOrder", 0) AS "OnOrder"
        	, IFNULL(t2."OnOrder", 0) AS "OnOrderInWhs", t1."CardCode", A."IssueMthd", a."SalPackUn", a."PurPackUn" 
        FROM OITM A
					INNER JOIN OSCN M on a."ItemCode" = M."ItemCode"
					INNER JOIN OCRD t1 ON M."CardCode" = t1."CardCode"
					LEFT JOIN OVTG ON t1."ECVatGroup"=OVTG."Code"
		            LEFT JOIN OSHP D ON A."ShipType" = D."TrnspCode"
		            LEFT JOIN OMRC E ON A."FirmCode" = E."FirmCode"
		            LEFT JOIN OITB f ON A."ItmsGrpCod" = f."ItmsGrpCod"
		            LEFT JOIN OVTG g ON A."VatGourpSa" = g."Code"
		            LEFT JOIN OVTG k ON A."VatGroupPu" = k."Code"
		            LEFT JOIN OITT l ON l."Code" = A."ItemCode"
					left join UGP1 a1 on a."UgpEntry" = a1."UgpEntry" and a1."UomEntry" in (select "UomEntry" from OUOM where "UomName" = a."InvntryUom")
					LEFT JOIN #tblPrice p ON a."ItemCode"=p."ItemCode" AND p."CardCode"=:bpCode
					Left Join (select "ItemCode", "WhsCode", "OnHand","IsCommited", "OnOrder" from OITW where ifnull(:itemCode,'') = '' or "ItemCode" = :itemCode) t2 on a."ItemCode" = t2."ItemCode" and (CASE WHEN  a."DfltWH" <>'' THEN a."DfltWH" else :WhsDefault end) = t2."WhsCode"			
	WHERE (:DsplITMDoc='Y' OR (:DsplITMDoc='N' AND ((A."validFor" = 'Y' and ifnull(A."frozenFor",'N') = 'N' and (ifnull(a."validFrom", :fromDate) <= current_date and ifnull(a."validTo", :toDate) >= CURRENT_DATE)) 
		OR (ifnull(A."validFor",'N') = 'N' and A."frozenFor" = 'Y' and (ifnull(a."frozenFrom", :fromDate) > CURRENT_DATE or ifnull(a."frozenTo", :toDate) < CURRENT_DATE))
		or (A."validFor" = 'Y' and A."frozenFor" = 'Y' and (ifnull(a."validFrom", :fromDate) <= CURRENT_DATE and ifnull(a."validTo", :toDate) >= CURRENT_DATE) and
					(ifnull(a."frozenFrom", :fromDate) > CURRENT_DATE or ifnull(a."frozenTo", :toDate) < CURRENT_DATE))
		OR (A."validFor" = 'N' AND A."frozenFor" = 'N')))) 
		And (ifnull(:bpCode,'') = '' or M."CardCode" = :bpCode) and (ifnull(:subCatNum,'') = '' or M."Substitute" = :subCatNum)
		and (ifnull(:itemCode, '') = '' or a."ItemCode" = :itemCode);
END;




