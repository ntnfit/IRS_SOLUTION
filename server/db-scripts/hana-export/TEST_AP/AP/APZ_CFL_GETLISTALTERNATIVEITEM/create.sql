

CREATE PROCEDURE Apz_Cfl_GetListAlternativeItem(in itemCode NVARCHAR(50),in userName NVARCHAR(20) default '', in branch NVARCHAR(20) default '',in CardCode VARCHAR(50) default '',in DocDate timestamp default null)
AS
BEGIN
	DECLARE DfSVatItem VARCHAR(50)='';
	DECLARE DfPVatItem VARCHAR(50)='';
	DECLARE DsplITMDoc CHAR(1);
	declare checkCount int = 0;
	DECLARE WhsDefault VARCHAR(20);
	declare fromDate timestamp;
	declare toDate timestamp;

	select IFNULL((SELECT "DfSVatItem" FROM oadm),'') into DfSVatItem from DUMMY;
	select IFNULL((SELECT "DfPVatItem" FROM oadm),'') into DfPVatItem from DUMMY;

	select IFNULL((SELECT "DspIITMDoc" FROM ADM1),'') into DsplITMDoc from DUMMY;
	
	CREATE local temporary TABLE #tblPrice ("ItemCode" nvarchar(50) null, "Price" numeric(19,6) null, "Currency" nvarchar(30) null, "Discount" numeric(19,6) null, "CardCode" nvarchar(50) null, "IsGrossPrc" nvarchar(50) null);
		
	insert into #tblPrice
	select a."ItemCode",ifnull(b."Price",a."Price") as "Price",a."Currency",a."Discount",a."CardCode",c."IsGrossPrc"
		from OSPP a
		left join SPP1 b on (b."ItemCode"=a."ItemCode" and b."CardCode"=a."CardCode" and :docdate between b."FromDate" and ifnull(b."ToDate",'2099-12-31'))
		LEFT JOIN opln c  ON a."ListNum"=c."ListNum"
		where a."CardCode"=:CardCode;
		
	select IFNULL((SELECT top 1 1 FROM #tblPrice),0) into checkCount from DUMMY;
	IF :checkCount = 0
	THEN
			INSERT INTO #tblPrice ("ItemCode",
			"Price",
			"Currency",
			"Discount",
			"CardCode",
			"IsGrossPrc")
			SELECT a."ItemCode",a."Price",a."Currency",0 "Discount",b."CardCode",c."IsGrossPrc"  FROM ITM1 a 
			INNER JOIN ocrd b ON a."PriceList"=b."ListNum" AND b."CardCode"=:CardCode
			INNER JOIN opln c ON a."PriceList"=c."ListNum";

	END IF;
	
	select ifnull((SELECT "Warehouse" FROM OUDG WHERE "Code"=(SELECT TOP 1 "DfltsGroup" FROM OUSR WHERE USER_CODE=:userName)),'') into WhsDefault from DUMMY;
	
	IF :WhsDefault=''
	THEN
		SELECT ifnull((SELECT "DflWhs" FROM OBPL WHERE "BPLId"=:branch),'') into WhsDefault FROM DUMMY;
    END	IF;
    
	IF :WhsDefault=''
	THEN
		select ifnull((SELECT "DfltWhs" FROM OADM),'') into WhsDefault FROM DUMMY;
    END	IF;

	IF :DocDate IS NULL
	THEN
		DocDate = CURRENT_DATE;
	END IF;

		-- Khai báo bảng dữ liệu combobox (tùy chọn)
		create local temporary table #tblValidValue
		(
			"ColumnName" NVARCHAR(50) NOT NULL
			,"Value" NVARCHAR(254) NOT NULL
			,"Display" NVARCHAR(254) NOT NULL
		);

		-- Khai báo bảng Structure
		create local temporary table #tblStructure
            (
	             "ColumnName" NVARCHAR(50) NOT NULL
	            ,"Caption" NVARCHAR(100) NULL
	            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
				,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
	            ,"Width" INT NULL DEFAULT 75
	            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
	            ,"Visible" smallint NULL DEFAULT 1
	            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
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
				--,Parent BIT NULL	-- dùng cho treelist
				--,[Key] BIT NULL		-- dùng cho treelist (identity)
            );
         INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType") 
			select n'ItemCode', n'Item Code', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from DUMMY
		 UNION ALL
			select n'ItemDescription', n'Item Name', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from DUMMY
		 UNION ALL
			select n'FrgnName', n'Foreign Name', 'nvarchar', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL from DUMMY
		 UNION ALL
			select n'U_OldCode', n'Mã cũ', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL from DUMMY
		 UNION ALL
			select n'ItmsGrpNam', n'Item Group', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		 UNION ALL
			select n'SalesVATGroup', n'SalesVATGroup', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		 UNION ALL
			select n'VatGroupPu', n'VatGroupPu', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		 UNION ALL
			select n'VatGourpSa', n'VatGourpSa', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		 UNION ALL
			select n'QuantityOnStock', n'InStock', 'numeric', 75, -1, 1, 'numeric', '{0:n3}', 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		 UNION ALL
			select n'Currency', n'Currency', 'numeric', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		 UNION ALL
			select n'Price', n'Price', 'nvarchar', 75, -1, 0, 'numeric', '{0:n3}', 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		 UNION ALL
			select n'PriceList', n'PriceList', 'int', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		 UNION ALL
			select n'RateSa', n'RateSa', 'numeric', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		 UNION ALL
			select n'RatePu', n'RatePu', 'numeric', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		 UNION ALL
			select n'BOMTreeType', n'BOMTreeType', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		 UNION ALL
			select n'VatPuInActive', n'VatPuInActive', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		 UNION ALL
			select n'VatSaInActive', n'VatSaInActive', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL  FROM DUMMY
		 UNION ALL
			select n'TreeType', n'TreeType', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		 UNION ALL
			select n'WarehouseCode', n'WarehouseCode', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY
		 UNION ALL
			select n'Substitute', n'Bp Catalog Num', 'nvarchar', 75, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, NULL, NULL, NULL FROM DUMMY;

		SELECT *  FROM #tblStructure;

		----------------------------------------- GET LIST ALTERNATIVE ITEM ---------------------------------------------------------

		create local temporary table #alterItem ("ItemCode" nvarchar(50) null);

		insert into #alterItem
			SELECT "AltItem" "ItemCode" FROM OALI where "OrigItem" = :itemCode;

		----------------------------------------- END GET LIST ALTERNATIVE ITEM -----------------------------------------------------

		select ADD_DAYS(current_Date, -1) into fromDate from DUMMY;
		select ADD_DAYS(current_date, 1) into toDate from DUMMY;

        SELECT A."ItemCode", 
		A."ItemName" AS "ItemDescription", 
		A."ItemName" AS "ItemName",
		'' "Substitute",
		ifnull(A."FrgnName", '') AS "FrgnName", 
		ifnull(f."ItmsGrpCod", 0) AS "ItmsGrpCod",
		ifnull(f."ItmsGrpNam", '') "ItmsGrpNam",
        CASE WHEN ifnull(OCRD."ECVatGroup",'') ='' THEN CASE WHEN  ifnull(A."VatGourpSa",'')='' THEN :DfSVatItem ELSE A."VatGourpSa" END ELSE OCRD."ECVatGroup" end  AS "SalesVATGroup",
		CASE WHEN ifnull(OCRD."ECVatGroup",'') ='' THEN CASE WHEN  ifnull(A."VatGroupPu",'')='' THEN :DfSVatItem ELSE A."VatGroupPu" END ELSE OCRD."ECVatGroup" end  AS "VatGroupPu", 
		CASE WHEN ifnull(OCRD."ECVatGroup",'') ='' THEN CASE WHEN  ifnull(A."VatGourpSa",'')='' THEN :DfSVatItem ELSE A."VatGourpSa" END ELSE OCRD."ECVatGroup" end  AS "VatGourpSa",
		CASE WHEN  a."DfltWH" <>'' THEN a."DfltWH" else :WhsDefault end AS "WarehouseCode",
		CASE WHEN  a."DfltWH" <>'' THEN a."DfltWH" else :WhsDefault end AS "DefaultWarehouse",
		CASE WHEN  a."DfltWH" <>'' THEN a."DfltWH" else :WhsDefault end AS "dfltWH",
        a."OnHand" "QuantityOnStock",  
		OCRD."ListNum" "PriceList",
		CASE WHEN ifnull(OCRD."ECVatGroup",'') ='' THEN CASE WHEN  ifnull(A."VatGourpSa",'')='' THEN ifnull(k1."Rate",0) ELSE ifnull(k."Rate", 0) END   ELSE ovtg."Rate" end  AS "RatePu", 
		CASE WHEN ifnull(OCRD."ECVatGroup",'') ='' THEN CASE WHEN  ifnull(A."VatGroupPu",'')='' THEN ifnull(g1."Rate",0) ELSE ifnull(g."Rate", 0) END ELSE ovtg."Rate" end  AS "RateSa",
		l."TreeType" AS "BOMTreeType", 
		ifnull(k."Inactive", '') AS "VatPuInActive",
        ifnull(g."Inactive", '') AS "VatSaInActive",
		case ifnull(l."TreeType",'N')
			when 'A' then 1
			when 'S' then 2
			when 'P' then 3
			when 'T' then 4
			when 'N' then
					case ifnull((select "TreeType" from OITT x6 where exists(select 1 from itt1 where "Code"=a."ItemCode" and x6."Code" = "Father")),'N')
					when 'S' then 5
					else 0
				End
		end AS "TreeType", 
		A."BuyUnitMsr", 
		A."SalUnitMsr",
		A."NumInBuy",
		A."NumInSale",
		A."InvntryUom",
		(SELECT x."Substitute" FROM (select "Substitute","ItemCode","CardCode", row_number() over(order by "Substitute") rowentry  from oscn WHERE oscn."ItemCode" = A."ItemCode" AND oscn."CardCode" = :CardCode) x where x.rowentry = 1) AS "SubCatNum",
		a."UgpEntry" as "UgpCode",
		ifnull(a1."AltQty", 1) / ifnull(a1."BaseQty", 1) as "NumInInven",
		ifnull(p."Price",0) "Price",
		ifnull(p."IsGrossPrc",'N') "IsGrossPrc",
		ifnull(p."Currency",'') "Currency",
		ifnull(p."Discount",0) "Discount",
		case when a."ManBtchNum"='Y' then 'B' when a."ManSerNum"='Y' then 'S' else 'N' END as "ManBatchSerial",
		ifnull(a."OnHand",0) "OnStock",
		ifnull(t1."OnHand",0) "OnWhs",
		ifnull(a."IsCommited",0) "IsCommited",
		ifnull(t1."IsCommited",0) "IsCommitedInWhs",
		ifnull(a."OnOrder",0) "OnOrder",
		ifnull(t1."OnOrder",0) "OnOrderInWhs",
		 A."CardCode",
		 '' "SubCatNum",
		A."IssueMthd",A."SalPackUn",a."PurPackUn"
            FROM OITM A inner join #alterItem xx ON a."ItemCode" = xx."ItemCode"
            LEFT JOIN OSHP D ON A."ShipType" = D."TrnspCode"
            LEFT JOIN OMRC E ON A."FirmCode" = E."FirmCode"
            LEFT JOIN OITB f ON A."ItmsGrpCod" = f."ItmsGrpCod"
            LEFT JOIN OVTG g ON A."VatGourpSa" = g."Code"
            LEFT JOIN OVTG k ON A."VatGroupPu" = k."Code"
			LEFT JOIN OVTG g1 ON :DfSVatItem = g1."Code"
            LEFT JOIN OVTG k1 ON :DfPVatItem = k1."Code"

            LEFT JOIN OITT l ON l."Code" = A."ItemCode"
			--LEFT JOIN OSCN M on a.ItemCode = M.ItemCode and M.CardCode = @CardCode
			left join UGP1 a1 on a."UgpEntry" = a1."UgpEntry" and a1."UomEntry" in (select "UomEntry" from OUOM where "UomName" = a."InvntryUom")
			LEFT JOIN OCRD 
			LEFT JOIN OVTG ON OCRD."ECVatGroup"=OVTG."Code" ON OCRD."CardCode"=:CardCode
			LEFT JOIN #tblPrice p ON a."ItemCode"=p."ItemCode" AND p."CardCode"=:CardCode
			Left Join (select "ItemCode", "WhsCode", "OnHand", "IsCommited", "OnOrder" from OITW where ifnull(:itemCode,'') = '' or "ItemCode" = :itemCode) t1 on a."ItemCode" = t1."ItemCode" 
					and (CASE WHEN  a."DfltWH" <>'' THEN a."DfltWH" else :WhsDefault end) = t1."WhsCode"
WHERE(:DsplITMDoc='Y' OR (:DsplITMDoc='N' AND ((A."validFor" = 'Y' and ifnull(A."frozenFor",'N') = 'N' and (ifnull(a."validFrom", :fromDate) <= CURRENT_DATE and ifnull(a."validTo", :toDate) >= CURRENT_DATE)) 
		OR (ifnull(A."validFor",'N') = 'N' and A."frozenFor" = 'Y' and (ifnull(a."frozenFrom", :fromDate) > CURRENT_DATE or ifnull(a."frozenTo", :toDate) < CURRENT_DATE))
		or (A."validFor" = 'Y' and A."frozenFor" = 'Y' and (ifnull(a."validFrom", :fromDate) <= current_date and ifnull(a."validTo", :toDate) >= current_date) and
					(ifnull(a."frozenFrom", :fromDate) > current_date or ifnull(a."frozenTo", :toDate) < CUrrent_date))
		OR (ifnull(A."validFor",'N') = 'N' AND ifnull(A."frozenFor",'N') = 'N')))) --AND (@itemCode = '' OR (@itemCode != '' AND A.ItemCode = @itemCode)) --AND A.OnHand > 0
		ORDER BY A."ItemCode";
END




