
CALL APZ_EXISTSPROC('Apz_Cfl_GetListItem');

GO

CREATE PROCEDURE Apz_Cfl_GetListItem(
	in itemCode NVARCHAR(50) default ''
,   in itemType nVARCHAR(1) default ''
,   in userName NVARCHAR(20) default ''
,   in branch NVARCHAR(20) default ''
,   in CardCode nVARCHAR(50) default '',
	in DocDate timestamp default null,
	in Quantity NUMERIC(19,6) default 1,
	in subCatNum nvarchar(50) default '',
	in listField nvarchar(2000) default ''
)
AS
BEGIN
		------------------------------------------- VARIABLE -------------------------------------------------------------
		declare infoPriceList int = 0;
		declare isGrossPrc char(1);
		DECLARE DfSVatItem nVARCHAR(50)='';
		DECLARE DfPVatItem nVARCHAR(50)='';
		DECLARE DsplITMDoc nvarCHAR(1);
		DECLARE WhsDefault nVARCHAR(20);
		declare fromDate timestamp;
		declare toDate timestamp;
		declare ecVatGroup nvarchar(8);
		
		------------------------------------------- END VARIABLE ---------------------------------------------------------
		SELECT TOP 1 "ListNum", "ECVatGroup" into infoPriceList, ecVatGroup default 0, null FROM OCRD WHERE "CardCode"=:CardCode;
		select ifnull((select "IsGrossPrc" from OPLN where "ListNum" = :infoPriceList), 'N') into isGrossPrc from DUMMY;
		select "DfSVatItem", "DfPVatItem" into DfSVatItem, DfPVatItem from OADM;
		select ifnull("DspIITMDoc", 'N') into DsplITMDoc from ADM1;
		
		select ADD_DAYS(current_Date, -1) into fromDate from DUMMY;
		select ADD_DAYS(current_date, 1) into toDate from DUMMY;
		
		if not exists(select 1 from OSCN where "CardCode" = :CardCode and "ItemCode" = :itemCode and :subCatNum = "Substitute") then
			subCatNum = '';
		end if;

		----------------------------------- LẤY DỮ LIỆU DEFAULT WAREHOUSR THEO THỨ TỰ ƯU TIÊN -----------------------------------------------------------------
		----------------------------------- DỮ LIỆU CONFIG DEFAULT WAREHOUSE THEO USER ------------------------------------------------------------------------
		select IFNULL((SELECT "Warehouse" FROM OUDG WHERE "Code"=(SELECT TOP 1 "DfltsGroup" FROM OUSR WHERE USER_CODE=:userName)),'') into WhsDefault from DUMMY;
		----------------------------------- LẤY DỮ LIỆU THEO BRANCH -------------------------------------------------------------------------------------------
		IF :WhsDefault=''
		THEN
			select IFNULL((SELECT "DflWhs" FROM OBPL WHERE "BPLId"=:branch),'') into WhsDefault from DUMMY;
        END	IF;
        ----------------------------------- LẤY THEO CONFIG TRÊN GENERAL --------------------------------------------------------------------------------------
		IF :WhsDefault=''
		then
			SELECT ifNULL((SELECT "DfltWhs" FROM OADM),'') INTO WhsDefault FROM DUMMY;
        END	iF;
		------------------------------------ END LẤY DỮ LIỆU DEFAULT WAREHOUSE -------------------------------------------------------------------------------
		IF :DocDate IS NULL
		then
			 DocDate= current_date;
		END IF;

		if ifnull(:itemCode,'') <> '' then
			
			----------------------------------- GET DATA SPECIAL PRICE ----------------------------------------------------------------
			CREATE local temporary TABLE #tblPrice ("ItemCode" nvarchar(50) null, "Price" numeric(21,6) null, "Currency" nvarchar(30) null, "Discount" numeric(21,6) null, "CardCode" nvarchar(50) null, "IsGrossPrc" nvarchar(50) null);
			INSERT INTO #tblPrice
			SELECT a."ItemCode", IFNULL(b."Price", a."Price") AS "Price", a."Currency", a."Discount", a."CardCode", :isGrossPrc 
										FROM OSPP a LEFT JOIN SPP1 b ON b."ItemCode" = a."ItemCode" AND b."CardCode" = a."CardCode" AND :docdate BETWEEN b."FromDate" AND IFNULL(b."ToDate", '2099-12-31')
													LEFT JOIN opln c ON a."ListNum" = c."ListNum" WHERE a."CardCode" = :CardCode and a."ItemCode"=:itemCode;
			----------------------------------- END GET DATA PRICE ------------------------------------------------------------
			
			----------------------------------- GET DATA PRICE FROM PRICE LIST WHEN NOT CONFIG SPECIAL PRICE ------------------
			if not exists(SELECT 1 FROM #tblPrice) THEN
				INSERT INTO #tblPrice ("ItemCode", "Price", "Currency", "Discount", "CardCode", "IsGrossPrc") 
				SELECT a."ItemCode", a."Price", a."Currency", 0 AS "Discount", b."CardCode", :isGrossPrc 
					FROM ITM1 a INNER JOIN ocrd b ON a."PriceList" = b."ListNum" AND b."CardCode" = :CardCode INNER JOIN opln c ON a."PriceList" = c."ListNum"
					where a."ItemCode"=:itemCode;
			END IF;
			
			----------------------------------- END GET DATA PRICE FROM PRICE LIST ---------------------------------------------

			create local temporary table #APZ_D_UUPIM as (select *, ROW_NUMBER() over(partition by "ObjectCode" order by "UploadDate", "UploadTime" DESC) as Row_Num from APZ_D_UUPIM where "ObjectType" = '810039');
		
			select a."ItemCode", A."ItemName" "ItemDescription", a."ItemName", '' "Substitute", a."FrgnName", a."ItmsGrpCod", f."ItmsGrpNam" "ItmsGrpNam",
			CASE WHEN IFNULL(:ecVatGroup, '') = '' THEN CASE WHEN IFNULL(A."VatGourpSa", '') = '' THEN :DfSVatItem ELSE A."VatGourpSa" END ELSE :ecVatGroup END "SalesVATGroup", CASE WHEN IFNULL(:ecVatGroup, '') = '' THEN CASE WHEN IFNULL(A."VatGroupPu", '') = '' THEN :DfSVatItem ELSE A."VatGroupPu" END ELSE :ecVatGroup END "VatGroupPu",
			CASE WHEN IFNULL(:ecVatGroup, '') = '' THEN CASE WHEN IFNULL(A."VatGourpSa", '') = '' THEN :DfSVatItem ELSE A."VatGourpSa" END ELSE :ecVatGroup END "VatGourpSa", CASE WHEN a."DfltWH" <> '' THEN a."DfltWH" ELSE :WhsDefault END "WarehouseCode",
			CASE WHEN a."DfltWH" <> '' THEN a."DfltWH" ELSE :WhsDefault END "DefaultWarehouse", CASE WHEN a."DfltWH" <> '' THEN a."DfltWH" ELSE :WhsDefault END "dfltWH", a."OnHand" "QuantityOnStock", :infoPriceList "PriceList"
			, CASE WHEN IFNULL(:ecVatGroup, '') = '' THEN CASE WHEN IFNULL(A."VatGourpSa", '') = '' THEN IFNULL((select "Rate" from OVTG where "Code"=:DfPVatItem), 0) ELSE IFNULL(k."Rate", 0) END ELSE (select "Rate" from OVTG where "Code"=:ecVatGroup) END "RatePu"
			, CASE WHEN IFNULL(:ecVatGroup, '') = '' THEN CASE WHEN IFNULL(A."VatGroupPu", '') = '' THEN IFNULL((select "Rate" from OVTG where "Code"=:DfSVatItem), 0) ELSE IFNULL(g."Rate", 0) END ELSE (select "Rate" from OVTG where "Code"=:ecVatGroup) END "RateSa"
			, l."TreeType" "BOMTreeType", IFNULL(k."Inactive", '') "VatPuInActive", IFNULL(g."Inactive", '') "VatSaInActive", a."BuyUnitMsr", a."SalUnitMsr", a."NumInBuy", a."NumInSale", a."InvntryUom", a."UgpEntry" "UgpCode"
			, case when a."UgpEntry"=-1 then 'Manual' else (select "UomCode" from OUOM where "UomEntry"=A."PUoMEntry") end "PUomCode"
			, case when a."UgpEntry"=-1 then 'Manual' else (select "UomCode" from OUOM where "UomEntry"=A."SUoMEntry") end "SUomCode"
			, case when a."UgpEntry"=-1 then 'Manual' else (select "UomCode" from OUOM where "UomEntry"=A."IUoMEntry") end "IUomCode"
			, IFNULL(a1."AltQty", 1) / IFNULL(a1."BaseQty", 1) "NumInInven", IFNULL(p."Price", 0) "Price", IFNULL(p."IsGrossPrc", 'N') "IsGrossPrc", p."Currency" "Currency", IFNULL(p."Discount", 0) "Discount"
			, CASE WHEN a."ManBtchNum" = 'Y' THEN 'B' WHEN a."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END "ManBatchSerial", IFNULL(a."OnHand", 0) "OnStock", IFNULL(t1."OnHand", 0) "OnWhs", a."IsCommited", IFNULL(t1."IsCommited", 0) "IsCommitedInWhs", a."OnOrder"
			, IFNULL(t1."OnOrder", 0) "OnOrderInWhs", a."CardCode"
			, (SELECT x."Substitute" FROM (select "Substitute","ItemCode","CardCode", row_number() over(order by "Substitute") rowentry  from oscn WHERE oscn."ItemCode" = A."ItemCode" AND oscn."CardCode" = :CardCode) x where x.rowentry = 1) "SubCatNum", a."IssueMthd", a."SalPackUn", a."PurPackUn"
			, ifnull((select t."LinkThumbnails" from #APZ_D_UUPIM t where t."ObjectCode" = A."ItemCode" and t.Row_Num = 1),'') "LinkThumbnails"
			, CASE IFNULL(l."TreeType", 'N') WHEN 'A' THEN 1 WHEN 'S' THEN 2 WHEN 'P' THEN 3 WHEN 'T' THEN 4 WHEN 'N' THEN CASE IFNULL((SELECT MAX("TreeType") FROM OITT x6 WHERE exists(SELECT 1 FROM itt1 WHERE "Code" = a."ItemCode" and x6."Code" = "Father")), 'N') WHEN 'S' THEN 5 ELSE 0 END END "TreeType"
			--, v."AgrNo" "AgrNo", v."AgrLineNum" "AgrLnNum"
        	FROM OITM A 
        	LEFT JOIN OITB f ON A."ItmsGrpCod" = f."ItmsGrpCod" 
        	LEFT JOIN OVTG g ON A."VatGourpSa" = g."Code" 
        	LEFT JOIN OVTG k ON A."VatGroupPu" = k."Code"
            LEFT JOIN OITT l ON l."Code" = A."ItemCode"
			left join UGP1 a1 on a."UgpEntry" = a1."UgpEntry" and a1."UomEntry" = a."IUoMEntry"
			LEFT JOIN #tblPrice p ON a."ItemCode"=p."ItemCode" AND p."CardCode"=:CardCode
			Left Join (SELECT "ItemCode", "WhsCode", "OnHand", "IsCommited", "OnOrder" FROM OITW WHERE "ItemCode" = :itemCode) t1 on a."ItemCode" = t1."ItemCode" and (CASE WHEN  a."DfltWH" <>'' THEN a."DfltWH" else :WhsDefault end) = t1."WhsCode"
			--LEFT JOIN (SELECT top 1 m."BpCode", n."ItemCode", n."AgrNo", n."AgrLineNum" FROM OOAT m INNER JOIN OAT1 n on m."AbsID" = n."AgrNo" where m."BpCode" = :CardCode order by m."StartDate") v on v."ItemCode" = a."ItemCode"
			WHERE(
				:DsplITMDoc='Y' 
				 OR (
				 		:DsplITMDoc='N' AND A."validFor" = 'Y' and ifnull(A."frozenFor",'N') = 'N' and ifnull(a."validFrom", :fromDate) <= current_date and ifnull(a."validTo", :toDate) >= current_date
				 	)
				 OR (
				 		ifnull(A."validFor",'N') = 'N' and A."frozenFor" = 'Y' and (ifnull(a."frozenFrom", :fromDate) > current_date or ifnull(a."frozenTo", :toDate) < current_date)
				 	)
				or (
						A."validFor" = 'Y' and A."frozenFor" = 'Y' and (ifnull(a."validFrom", :fromDate) <= current_date and ifnull(a."validTo", :toDate) >= current_date) and
							(ifnull(a."frozenFrom", :fromDate) > current_date or ifnull(a."frozenTo", :toDate) < current_date)
					)
				OR (
						ifnull(A."validFor",'N') = 'N' AND Ifnull(A."frozenFor",'N') = 'N'
					)
			)
			AND A."ItemCode" = :itemCode
			AND (
					:itemType = '1' AND A."InvntItem" = 'Y'
				OR :itemType = '2' AND A."SellItem" = 'Y'
				OR :itemType = '3' AND A."PrchseItem" = 'Y'
				OR IFNULL(:itemType,0) = 0
				);
				
			drop table #tblPrice;
			drop table #APZ_D_UUPIM;
		else
			declare fromQuery nvarchar(5000);
			declare query nvarchar(5000);
			declare whereQuery nvarchar(2000);
		
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
			drop table #tblStructure;
			
			-- ItemCode là trường bắt buộc
			if ifnull(:listField,'')='' then
				listField := 'ItemCode,ItemDescription';
			else
				listField := :listField || ',ItemCode';
			end if;
			
			whereQuery := '
			WHERE(
					''' || :DsplITMDoc || '''=''Y'' 
					 OR (
					 		''' || :DsplITMDoc || '''=''N'' AND A."validFor" = ''Y'' and ifnull(A."frozenFor",''N'') = ''N'' and ifnull(a."validFrom", ''' || to_varchar(:fromDate,'yyyy-MM-dd') || ''') <= current_date and ifnull(a."validTo", ''' || to_varchar(:toDate,'yyyy-MM-dd') || ''') >= current_date
					 	)
					 OR (
					 		ifnull(A."validFor",''N'') = ''N'' and A."frozenFor" = ''Y'' and (ifnull(a."frozenFrom", ''' || to_varchar(:fromDate,'yyyy-MM-dd') || ''') > current_date or ifnull(a."frozenTo", ''' || to_varchar(:toDate,'yyyy-MM-dd') || ''') < current_date)
					 	)
					or (
							A."validFor" = ''Y'' and A."frozenFor" = ''Y'' and (ifnull(a."validFrom", ''' || to_varchar(:fromDate,'yyyy-MM-dd') || ''') <= current_date and ifnull(a."validTo", ''' || to_varchar(:toDate,'yyyy-MM-dd') || ''') >= current_date) and
								(ifnull(a."frozenFrom", ''' || to_varchar(:fromDate,'yyyy-MM-dd') || ''') > current_date or ifnull(a."frozenTo", ''' || to_varchar(:toDate,'yyyy-MM-dd') || ''') < current_date)
						)
					OR (
							ifnull(A."validFor",''N'') = ''N'' AND Ifnull(A."frozenFor",''N'') = ''N''
						)
				)
				AND (
						''' || :itemType || '''= ''1'' AND A."InvntItem" = ''Y''
					OR ''' || :itemType || ''' = ''2'' AND A."SellItem" = ''Y''
					OR ''' || :itemType || ''' = ''3'' AND A."PrchseItem" = ''Y''
					OR IFNULL(''' || :itemType || ''',0) = 0
					);';
			
			select string_agg("JoinQuery",'
			') into fromQuery from APZ_Item_CFLJoinQuery where "Alias" in (select "JoinAlias" from APZ_Item_ListCFLField where "FieldName" in (select Data from Split(:listField,',')));
			
			fromQuery := ' from OITM a ' || ifnull(:fromQuery,'');
			
			select string_agg(case when "QueryField" is null then 'a."' || "FieldName" || '"' else "QueryField" || ' "' || "FieldName" || '"' end,', ') into query
			from APZ_Item_ListCFLField where "FieldName" in (select Data from Split(:listField,','));
			
			query := 'select ' || :query || :fromQuery || :whereQuery;
			query := replace(:query,':CardCode',case when :CardCode is null then 'null' else '''' || :CardCode || '''' end);
			query := replace(:query,':ecVatGroup',case when :ecVatGroup is null then 'null' else '''' || :ecVatGroup || '''' end);
			query := replace(:query,':DfSVatItem',case when :DfSVatItem is null then 'null' else '''' || :DfSVatItem || '''' end);
			query := replace(:query,':DfPVatItem',case when :DfPVatItem is null then 'null' else '''' || :DfPVatItem || '''' end);
			query := replace(:query,':WhsDefault',case when :WhsDefault is null then 'null' else '''' || :WhsDefault || '''' end);
			query := replace(:query,':infoPriceList',case when :infoPriceList is null then 'null' else '''' || :infoPriceList || '''' end);
			
			if exists(select 1 from APZ_Item_ListCFLField where "FieldName" in (select Data from Split(:listField,',')) and "JoinAlias"='p') then
				----------------------------------- GET DATA SPECIAL PRICE ----------------------------------------------------------------
				CREATE local temporary TABLE #tblPrice ("ItemCode" nvarchar(50) null, "Price" numeric(21,6) null, "Currency" nvarchar(30) null, "Discount" numeric(21,6) null, "CardCode" nvarchar(50) null, "IsGrossPrc" nvarchar(50) null);
				INSERT INTO #tblPrice
				SELECT a."ItemCode", IFNULL(b."Price", a."Price") AS "Price", a."Currency", a."Discount", a."CardCode", :isGrossPrc 
											FROM OSPP a LEFT JOIN SPP1 b ON b."ItemCode" = a."ItemCode" AND b."CardCode" = a."CardCode" AND :docdate BETWEEN b."FromDate" AND IFNULL(b."ToDate", '2099-12-31')
														LEFT JOIN opln c ON a."ListNum" = c."ListNum" WHERE a."CardCode" = :CardCode;
				----------------------------------- END GET DATA PRICE ------------------------------------------------------------
				
				----------------------------------- GET DATA PRICE FROM PRICE LIST WHEN NOT CONFIG SPECIAL PRICE ------------------
				if not exists(SELECT 1 FROM #tblPrice) THEN
					INSERT INTO #tblPrice ("ItemCode", "Price", "Currency", "Discount", "CardCode", "IsGrossPrc") 
					SELECT a."ItemCode", a."Price", a."Currency", 0 AS "Discount", b."CardCode", :isGrossPrc 
						FROM ITM1 a INNER JOIN ocrd b ON a."PriceList" = b."ListNum" AND b."CardCode" = :CardCode INNER JOIN opln c ON a."PriceList" = c."ListNum";
				END IF;
				
				----------------------------------- END GET DATA PRICE FROM PRICE LIST ---------------------------------------------
			end if;
			
			execute immediate :query;
		end if;
END;