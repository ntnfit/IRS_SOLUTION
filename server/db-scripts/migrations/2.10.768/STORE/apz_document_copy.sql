
CALL APZ_EXISTSPROC('apz_document_copy');

GO

CREATE PROCEDURE apz_document_copy(
-- **** FROM DOCUMENT ****
	in fromtype NVARCHAR(20),
	in docentry NVARCHAR(1024), -- list of docentry
	
-- **** TO DOCUMENT ****
	in totype NVARCHAR(20)
)
AS
BEGIN
	
	DECLARE closedQuote nvarchar(1);
	DECLARE basetype NVARCHAR(50);
	DECLARE tblname NVARCHAR(10)='';
	DECLARE tblline NVARCHAR(10)='';
	DECLARE tbl10 NVARCHAR(10)='';
	DECLARE sqlQuery NVARCHAR(400000)='';
	DECLARE n INT;
	DECLARE p INT=1;
	DECLARE shipdoc INT;
	DECLARE tblTo nvarchar(10);
	DECLARE WhsDefault VARCHAR(20);
	DECLARE i0 TINYINT=1;
	DECLARE EnblExpns CHAR(1) ='N';
	DECLARE sql_expns NVARCHAR(500000)='';
	DECLARE tbl_expns CHAR(4);
	DECLARE i TINYINT=1;
	DECLARE dpmInvoice char(1) = 'N'; -- Nếu là Down payment thì có phải Invoice không
	
	if :toType = '203000' then
		toType = '203';
		dpmInvoice = 'Y';
	elseif :toType = '204000' then
		toType = '204';
		dpmInvoice = 'Y';
	end if;
	
	select ifnull((select top 1 "ClosedQuot" from oadm),'') into closedQuote from DUMMY;
	IF :fromtype='700021'
	then
		tblname='ORDR';
		tblline='RDR1';
		basetype='17';
		tbl10='RDR10';

		create local temporary table #temp_ship ("TransType" nvarchar(20) null,"TransNo" int null,"TransRow" int null,"ItemCode" nvarchar(50) null,"Quantity" numeric(19,6) null,"VisOrder" int null ,"TreeType" varchar(1) null);
		
		INSERT INTO #temp_ship SELECT T0."TransType", T0."TransNo", T0."TransRow", T0."ItemCode", SUM(T0."Quantity" * IFNULL(T0."NumPerMsr", 1)), T2."VisOrder", T2."TreeType" 
			FROM APZ_SHIP1 T0 INNER JOIN APZ_SHIP T1 ON T0."DocEntry" = T1."DocEntry" INNER JOIN RDR1 T2 ON T2."LineNum" = T0."TransRow" 
				INNER JOIN ORDR T3 ON T0."TransNo" = T3."DocNum" AND T2."DocEntry" = T3."DocEntry"
				 WHERE T0."DocEntry" IN (SELECT Data FROM split(:docentry,',')) AND T1."DocStatus"='O'
		 			GROUP BY T0."TransType",T0."TransNo",T0."TransRow",T0."ItemCode",T2."VisOrder",T2."TreeType";

		 create local temporary table #temp_bom as(SELECT ROW_NUMBER() OVER (ORDER BY "TransNo") ID,"VisOrder" FROM #temp_ship  WHERE "TreeType"='I');
		 
		 select ifnull((SELECT COUNT(1) FROM #temp_ship WHERE "TreeType"='I'),0) into n from DUMMY;
		 select ifnull((SELECT TOP 1 "TransNo" FROM #temp_ship),0) into shipdoc from dummy;
		 WHILE p<=n
		 DO
			 INSERT INTO #temp_ship ("TransType", "TransNo", "TransRow", "ItemCode", "Quantity", "VisOrder", "TreeType") 
			 	SELECT TOP 1 '17', T1."DocNum", T0."LineNum", T0."ItemCode", T0."Quantity", T0."VisOrder", T0."TreeType" FROM rdr1 T0 INNER JOIN ORDR T1 ON t0."DocEntry" = T1."DocEntry" 
			 			WHERE T0."VisOrder" < (SELECT TOP 1 "VisOrder" FROM #temp_bom WHERE ID = :p) AND T0."TreeType" = 'S' AND T1."DocNum" = :shipdoc ORDER BY T0."VisOrder" DESC;	      
			 p=:p+1;
		 END while;

		  create local temporary table #ret_ship as (SELECT DISTINCT * FROM #temp_ship);
		  CREATE local temporary TABLE #tblship AS (SELECT T0."TransType", T0."TransNo", T0."TransRow", T0."ItemCode", "Quantity" FROM #ret_ship T0 ORDER BY T0."VisOrder" ASC);

	ELSE
		select ifnull((SELECT TOP 1  "TblName" FROM apz_tblobject a WHERE a."ObjType"=:fromtype),'') into tblname from dummy;
		select ifnull((SELECT TOP 1  "TblLine" FROM apz_tblobject a WHERE a."ObjType"=:fromtype),'') into tblline from dummy;
		tbl10=SUBSTRING(:tblLine,0,LENGTH(:tblLine) - 1) || '10';
		basetype=:fromtype;
	END	if;
	
	select ifnull((SELECT TOP 1 "TblName" FROM apz_tblobject a WHERE a."ObjType"=:totype),'') into tblTo from dummy;
	
	IF :fromtype='1250000025' or :fromtype='1250000026'	-- Blanket Agreement
	then
		
		sql_expns := ' 
			LEFT JOIN                       
					(SELECT "ExtTaxSum" "ExtTaxSum_F1","AnalysRpt" "AnalysRpt_F1","Stock" "Stock_F1","lineVat" "lineVat_F1","VatAppldSC" "VatAppldSC_F1","TaxAmtSrc" "TaxAmtSrc_F1","TaxCode" "TaxCode_F1","VatGroup" "VatGroup_F1","ExpnsCode" "ExpnsCode_F1","PaidToDate" "PaidToDate_F1","VatSumFrgn" "VatSumFrgn_F1","VatSum" "VatSum_F1","VatSumSy" "VatSumSy_F1","ObjType" "ObjType_F1","VisOrder" "VisOrder_F1","RetReqSC" "RetReqSC_F1","WtLiable" "WtLiable_F1","TaxStatus" "TaxStatus_F1","OcrCode2" "OcrCode2_F1","EquVatPer" "EquVatPer_F1","DedVatSum" "DedVatSum_F1","TotalSumSy" "TotalSumSy_F1","IsAcquistn" "IsAcquistn_F1","BaseMethod" "BaseMethod_F1","DrawnSC" "DrawnSC_F1","VatAppldFC" "VatAppldFC_F1","ExtTaxSumF" "ExtTaxSumF_F1","VatGrpSrc" "VatGrpSrc_F1","Project" "Project_F1","ExtTaxRate" "ExtTaxRate_F1","VatApplied" "VatApplied_F1","ExtTaxSumS" "ExtTaxSumS_F1","RetReqLC" "RetReqLC_F1","TaxType" "TaxType_F1","TrgGroup" "TrgGroup_F1","DrawnTotal" "DrawnTotal_F1","VatPrcnt" "VatPrcnt_F1","LineNum" "LineNum_F1","lineVatlF" "lineVatlF_F1","BaseGroup" "BaseGroup_F1","EquVatSumF" "EquVatSumF_F1","TotalFrgn" "TotalFrgn_F1","LstPchPrce" "LstPchPrce_F1","LineTotal" "LineTotal_F1","FixCurr" "FixCurr_F1","DocEntry" "DocEntry_F1","Status" "Status_F1","EquVatSumS" "EquVatSumS_F1","LogInstanc" "LogInstanc_F1","OcrCode" "OcrCode_F1","EquVatSum" "EquVatSum_F1","RetReqFC" "RetReqFC_F1","VatDscntPr" "VatDscntPr_F1","PaidSys" "PaidSys_F1","GroupNum" "GroupNum_F1","DedVatSumF" "DedVatSumF_F1","OcrCode3" "OcrCode3_F1","OcrCode4" "OcrCode4_F1","OcrCode5" "OcrCode5_F1","lineVatS" "lineVatS_F1","DrawnFC" "DrawnFC_F1","DedVatSumS" "DedVatSumS_F1","PaidFC" "PaidFC_F1" FROM QUT2 WHERE "GroupNum"=0)
						FG1 ON A."AgrNo"=FG1."DocEntry_F1" AND FG1."LineNum_F1" = A."AgrLineNum" 
			LEFT JOIN                       
					(SELECT "ExtTaxSum" "ExtTaxSum_F2","AnalysRpt" "AnalysRpt_F2","Stock" "Stock_F2","lineVat" "lineVat_F2","VatAppldSC" "VatAppldSC_F2","TaxAmtSrc" "TaxAmtSrc_F2","TaxCode" "TaxCode_F2","VatGroup" "VatGroup_F2","ExpnsCode" "ExpnsCode_F2","PaidToDate" "PaidToDate_F2","VatSumFrgn" "VatSumFrgn_F2","VatSum" "VatSum_F2","VatSumSy" "VatSumSy_F2","ObjType" "ObjType_F2","VisOrder" "VisOrder_F2","RetReqSC" "RetReqSC_F2","WtLiable" "WtLiable_F2","TaxStatus" "TaxStatus_F2","OcrCode2" "OcrCode2_F2","EquVatPer" "EquVatPer_F2","DedVatSum" "DedVatSum_F2","TotalSumSy" "TotalSumSy_F2","IsAcquistn" "IsAcquistn_F2","BaseMethod" "BaseMethod_F2","DrawnSC" "DrawnSC_F2","VatAppldFC" "VatAppldFC_F2","ExtTaxSumF" "ExtTaxSumF_F2","VatGrpSrc" "VatGrpSrc_F2","Project" "Project_F2","ExtTaxRate" "ExtTaxRate_F2","VatApplied" "VatApplied_F2","ExtTaxSumS" "ExtTaxSumS_F2","RetReqLC" "RetReqLC_F2","TaxType" "TaxType_F2","TrgGroup" "TrgGroup_F2","DrawnTotal" "DrawnTotal_F2","VatPrcnt" "VatPrcnt_F2","LineNum" "LineNum_F2","lineVatlF" "lineVatlF_F2","BaseGroup" "BaseGroup_F2","EquVatSumF" "EquVatSumF_F2","TotalFrgn" "TotalFrgn_F2","LstPchPrce" "LstPchPrce_F2","LineTotal" "LineTotal_F2","FixCurr" "FixCurr_F2","DocEntry" "DocEntry_F2","Status" "Status_F2","EquVatSumS" "EquVatSumS_F2","LogInstanc" "LogInstanc_F2","OcrCode" "OcrCode_F2","EquVatSum" "EquVatSum_F2","RetReqFC" "RetReqFC_F2","VatDscntPr" "VatDscntPr_F2","PaidSys" "PaidSys_F2","GroupNum" "GroupNum_F2","DedVatSumF" "DedVatSumF_F2","OcrCode3" "OcrCode3_F2","OcrCode4" "OcrCode4_F2","OcrCode5" "OcrCode5_F2","lineVatS" "lineVatS_F2","DrawnFC" "DrawnFC_F2","DedVatSumS" "DedVatSumS_F2","PaidFC" "PaidFC_F2" FROM QUT2 WHERE "GroupNum"=1)
						FG2 ON A."AgrNo"=FG2."DocEntry_F2" AND FG2."LineNum_F2" = A."AgrLineNum" 
			LEFT JOIN                       
					(SELECT "ExtTaxSum" "ExtTaxSum_F3","AnalysRpt" "AnalysRpt_F3","Stock" "Stock_F3","lineVat" "lineVat_F3","VatAppldSC" "VatAppldSC_F3","TaxAmtSrc" "TaxAmtSrc_F3","TaxCode" "TaxCode_F3","VatGroup" "VatGroup_F3","ExpnsCode" "ExpnsCode_F3","PaidToDate" "PaidToDate_F3","VatSumFrgn" "VatSumFrgn_F3","VatSum" "VatSum_F3","VatSumSy" "VatSumSy_F3","ObjType" "ObjType_F3","VisOrder" "VisOrder_F3","RetReqSC" "RetReqSC_F3","WtLiable" "WtLiable_F3","TaxStatus" "TaxStatus_F3","OcrCode2" "OcrCode2_F3","EquVatPer" "EquVatPer_F3","DedVatSum" "DedVatSum_F3","TotalSumSy" "TotalSumSy_F3","IsAcquistn" "IsAcquistn_F3","BaseMethod" "BaseMethod_F3","DrawnSC" "DrawnSC_F3","VatAppldFC" "VatAppldFC_F3","ExtTaxSumF" "ExtTaxSumF_F3","VatGrpSrc" "VatGrpSrc_F3","Project" "Project_F3","ExtTaxRate" "ExtTaxRate_F3","VatApplied" "VatApplied_F3","ExtTaxSumS" "ExtTaxSumS_F3","RetReqLC" "RetReqLC_F3","TaxType" "TaxType_F3","TrgGroup" "TrgGroup_F3","DrawnTotal" "DrawnTotal_F3","VatPrcnt" "VatPrcnt_F3","LineNum" "LineNum_F3","lineVatlF" "lineVatlF_F3","BaseGroup" "BaseGroup_F3","EquVatSumF" "EquVatSumF_F3","TotalFrgn" "TotalFrgn_F3","LstPchPrce" "LstPchPrce_F3","LineTotal" "LineTotal_F3","FixCurr" "FixCurr_F3","DocEntry" "DocEntry_F3","Status" "Status_F3","EquVatSumS" "EquVatSumS_F3","LogInstanc" "LogInstanc_F3","OcrCode" "OcrCode_F3","EquVatSum" "EquVatSum_F3","RetReqFC" "RetReqFC_F3","VatDscntPr" "VatDscntPr_F3","PaidSys" "PaidSys_F3","GroupNum" "GroupNum_F3","DedVatSumF" "DedVatSumF_F3","OcrCode3" "OcrCode3_F3","OcrCode4" "OcrCode4_F3","OcrCode5" "OcrCode5_F3","lineVatS" "lineVatS_F3","DrawnFC" "DrawnFC_F3","DedVatSumS" "DedVatSumS_F3","PaidFC" "PaidFC_F3" FROM QUT2 WHERE "GroupNum"=2)
						FG3 ON A."AgrNo"=FG3."DocEntry_F3" AND FG3."LineNum_F3" = A."AgrLineNum"';
		select ifnull((SELECT "DfltWhs" FROM OADM),'') into WhsDefault from dummy;
		sqlQuery = '
			SELECT
					CASE
						WHEN b."ManBtchNum"=''Y'' THEN ''B''
						WHEN b."ManSerNum"=''Y'' THEN ''S''
						ELSE ''N'' END AS "ManBatchSerial"
				  , CASE
						WHEN b."ManBtchNum"=''Y'' THEN ''B''
						WHEN b."ManSerNum"=''Y'' THEN ''S''
						ELSE ''N'' END AS "ManBtchSerial"
				  , ifnull(C."Rate", 1) "TaxPercentagePerRow"
				  , ifnull(C."Rate", 1) "RateVat"
				  , '''' "BOMTreeType"
				  , A."AgrNo" "AgrNo"
				  , D."Number" "AgrNumber"
				  , A."AgrLineNum" "AgrLnNum"
				  , A."PlanQty" AS "Quantity"
				  , A."PlanQty" AS "OpenQty"
				  , CASE WHEN D."Type" <> ''G'' THEN a."UnitPrice" ELSE f."Price" END  AS "PriceBefDi"
				  , CASE WHEN D."Type" <> ''G'' THEN a."UnitPrice" ELSE f."Price" END  AS "Price"
				  , f."Currency" AS "Currency"
				  , 0 AS "DiscPrcnt"
				  , CASE WHEN D."Type" <> ''G'' THEN (A."PlanQty" * A."UnitPrice") ELSE (A."PlanQty" * f."Price") END AS "LineTotal"
				  , 4 "ItemType" 
				  , 0 "LineTotal" -- set bằng 0 để tính lại trên form
				  , ''R'' "LineType"
				  , '
				  
				  || (case when :fromtype='1250000025' then 'case when ifnull(b."SalUnitMsr", '''') = ifnull(b."InvntryUom", '''') then ''Y'' else ''N'' end'
				  		else 'case when ifnull(b."BuyUnitMsr", '''') = ifnull(b."InvntryUom", '''') then ''Y'' else ''N'' end' end)
				  ||
				  
				  ' "UseBaseUn"
				  , A."ItemCode" AS "ItemCode"
				  , A."ItemName" AS "Dscription"
				  , A."UomEntry"
				  , A."UomCode"
				  , A."NumPerMsr"
				  , A."InvntryUom"
				  , b."VatGroupPu"
				  , b."VatGourpSa"
				  , CASE WHEN b."DfltWH" <> '''' THEN b."DfltWH" ELSE ''' || :WhsDefault || ''' END AS "WhsCode"
				  ,''' || :basetype || ''' "TempBaseType"
				  , ROW_NUMBER() OVER ( ORDER BY A."AgrNo" ASC) - 1 AS "LineNum"
				  , ifnull(b."SUoMEntry", -1) AS "SUoMEntry"
				  , (select "UomCode" from OUOM where "UomEntry" = b."SUoMEntry") "SUoMCode"
				  , b."SalUnitMsr" AS "SalUnitMsr"
				  , ifnull(b."NumInSale", 1) AS "NumInSale"
				  , ifnull(b."PUoMEntry", -1) AS "PUoMEntry"
				  , (select "UomCode" from OUOM where "UomEntry" = b."PUoMEntry") "PUoMCode"
				  , b."BuyUnitMsr" AS "BuyUnitMsr"
				  , ifnull(b."NumInBuy", 1) AS "NumInBuy"
				  , ifnull(b."UgpEntry", -1) AS "UgpCode"
				  , b."InvntryUom" AS "InvUnitMsr"
				  , ifnull(T4."AltQty", 1) / ifnull(T4."BaseQty", 1) AS "NumInInven"
				  , cast(null as integer) AS "LineSeq"
				  , ifnull(b."OnHand", null) AS "QuantityOnStock"
				  , ifnull(b."IsCommited", null) AS "IsCommited"
				  , ifnull(b."OnOrder", null) AS "OnOrder"
				  , ifnull(b1."OnHand", null) AS "QuantityOnWareHouse"
				  , ifnull(b."IsCommited", null) AS "IsCommited"
				  , ifnull(b1."IsCommited", null) AS "IsCommitedInWareHouse"
				  , ifnull(b."OnOrder", null) AS "OnOrder"
				  , ifnull(b1."OnOrder", null) AS "OnOrderInWareHouse"
				  , b."PurPackUn", b."SalPackUn", b1."AvgPrice"
				  , C.*, FG1.*, FG2.*, FG3.*
				FROM OAT1 A
					LEFT JOIN QUT1 C ON C."DocEntry"=0
					INNER JOIN OITM b ON A."ItemCode"=b."ItemCode"
					LEFT JOIN UGP1 T4 ON b."UgpEntry"=T4."UgpEntry" AND T4."UomEntry"=b."IUoMEntry"
					INNER JOIN OOAT d ON d."AbsID" = a."AgrNo"
					INNER JOIN OCRD e ON e."CardCode" = d."BpCode"
					INNER JOIN ITM1 f ON f."ItemCode" = a."ItemCode" AND f."PriceList" = e."ListNum"
					LEFT JOIN OITW b1 on a."ItemCode" = b1."ItemCode" and (CASE WHEN  b."DfltWH" <>'''' THEN b."DfltWH" else ''' || :WhsDefault || ''' end) = b1."WhsCode" 
					' || :sql_expns || '
				WHERE  A."AgrNo" IN (SELECT Data FROM split(''' || :docentry || ''','',''))
					AND (
					(A."LineStatus"=''O'' AND A."PlanQty">0)AND d."Status"=''A'')
				ORDER BY A."VisOrder";';
		--PRINT @SQL
		EXECUTE immediate :sqlQuery;
		RETURN;
	END if;

	EnblExpns='Y';--ifnull((SELECT EnblExpns FROM cinf),'N')
	IF	:EnblExpns='Y'
	then
			
		tbl_expns=SUBSTRING(:tblLine,0,LENGTH(:tblLine) - 1) || '2';

		sql_expns := 'LEFT JOIN                       
					(SELECT "ExtTaxSum" "ExtTaxSum_F1","AnalysRpt" "AnalysRpt_F1","Stock" "Stock_F1","lineVat" "lineVat_F1","VatAppldSC" "VatAppldSC_F1","TaxAmtSrc" "TaxAmtSrc_F1","TaxCode" "TaxCode_F1","VatGroup" "VatGroup_F1","ExpnsCode" "ExpnsCode_F1","PaidToDate" "PaidToDate_F1","VatSumFrgn" "VatSumFrgn_F1","VatSum" "VatSum_F1","VatSumSy" "VatSumSy_F1","ObjType" "ObjType_F1","VisOrder" "VisOrder_F1","RetReqSC" "RetReqSC_F1","WtLiable" "WtLiable_F1","TaxStatus" "TaxStatus_F1","OcrCode2" "OcrCode2_F1","EquVatPer" "EquVatPer_F1","DedVatSum" "DedVatSum_F1","TotalSumSy" "TotalSumSy_F1","IsAcquistn" "IsAcquistn_F1","BaseMethod" "BaseMethod_F1","DrawnSC" "DrawnSC_F1","VatAppldFC" "VatAppldFC_F1","ExtTaxSumF" "ExtTaxSumF_F1","VatGrpSrc" "VatGrpSrc_F1","Project" "Project_F1","ExtTaxRate" "ExtTaxRate_F1","VatApplied" "VatApplied_F1","ExtTaxSumS" "ExtTaxSumS_F1","RetReqLC" "RetReqLC_F1","TaxType" "TaxType_F1","TrgGroup" "TrgGroup_F1","DrawnTotal" "DrawnTotal_F1","VatPrcnt" "VatPrcnt_F1","LineNum" "LineNum_F1","lineVatlF" "lineVatlF_F1","BaseGroup" "BaseGroup_F1","EquVatSumF" "EquVatSumF_F1","TotalFrgn" "TotalFrgn_F1","LstPchPrce" "LstPchPrce_F1","LineTotal" "LineTotal_F1","FixCurr" "FixCurr_F1","DocEntry" "DocEntry_F1","Status" "Status_F1","EquVatSumS" "EquVatSumS_F1","LogInstanc" "LogInstanc_F1","OcrCode" "OcrCode_F1","EquVatSum" "EquVatSum_F1","RetReqFC" "RetReqFC_F1","VatDscntPr" "VatDscntPr_F1","PaidSys" "PaidSys_F1","GroupNum" "GroupNum_F1","DedVatSumF" "DedVatSumF_F1","OcrCode3" "OcrCode3_F1","OcrCode4" "OcrCode4_F1","OcrCode5" "OcrCode5_F1","lineVatS" "lineVatS_F1","DrawnFC" "DrawnFC_F1","DedVatSumS" "DedVatSumS_F1","PaidFC" "PaidFC_F1" FROM ' || :tbl_expns || ' WHERE "GroupNum"=0)
					 FG1 ON A."DocEntry"=FG1."DocEntry_F1" AND FG1."LineNum_F1" = A."LineNum" 
			LEFT JOIN                       
					(SELECT "ExtTaxSum" "ExtTaxSum_F2","AnalysRpt" "AnalysRpt_F2","Stock" "Stock_F2","lineVat" "lineVat_F2","VatAppldSC" "VatAppldSC_F2","TaxAmtSrc" "TaxAmtSrc_F2","TaxCode" "TaxCode_F2","VatGroup" "VatGroup_F2","ExpnsCode" "ExpnsCode_F2","PaidToDate" "PaidToDate_F2","VatSumFrgn" "VatSumFrgn_F2","VatSum" "VatSum_F2","VatSumSy" "VatSumSy_F2","ObjType" "ObjType_F2","VisOrder" "VisOrder_F2","RetReqSC" "RetReqSC_F2","WtLiable" "WtLiable_F2","TaxStatus" "TaxStatus_F2","OcrCode2" "OcrCode2_F2","EquVatPer" "EquVatPer_F2","DedVatSum" "DedVatSum_F2","TotalSumSy" "TotalSumSy_F2","IsAcquistn" "IsAcquistn_F2","BaseMethod" "BaseMethod_F2","DrawnSC" "DrawnSC_F2","VatAppldFC" "VatAppldFC_F2","ExtTaxSumF" "ExtTaxSumF_F2","VatGrpSrc" "VatGrpSrc_F2","Project" "Project_F2","ExtTaxRate" "ExtTaxRate_F2","VatApplied" "VatApplied_F2","ExtTaxSumS" "ExtTaxSumS_F2","RetReqLC" "RetReqLC_F2","TaxType" "TaxType_F2","TrgGroup" "TrgGroup_F2","DrawnTotal" "DrawnTotal_F2","VatPrcnt" "VatPrcnt_F2","LineNum" "LineNum_F2","lineVatlF" "lineVatlF_F2","BaseGroup" "BaseGroup_F2","EquVatSumF" "EquVatSumF_F2","TotalFrgn" "TotalFrgn_F2","LstPchPrce" "LstPchPrce_F2","LineTotal" "LineTotal_F2","FixCurr" "FixCurr_F2","DocEntry" "DocEntry_F2","Status" "Status_F2","EquVatSumS" "EquVatSumS_F2","LogInstanc" "LogInstanc_F2","OcrCode" "OcrCode_F2","EquVatSum" "EquVatSum_F2","RetReqFC" "RetReqFC_F2","VatDscntPr" "VatDscntPr_F2","PaidSys" "PaidSys_F2","GroupNum" "GroupNum_F2","DedVatSumF" "DedVatSumF_F2","OcrCode3" "OcrCode3_F2","OcrCode4" "OcrCode4_F2","OcrCode5" "OcrCode5_F2","lineVatS" "lineVatS_F2","DrawnFC" "DrawnFC_F2","DedVatSumS" "DedVatSumS_F2","PaidFC" "PaidFC_F2" FROM ' || :tbl_expns || ' WHERE "GroupNum"=1)
					 FG2 ON A."DocEntry"=FG2."DocEntry_F2" AND FG2."LineNum_F2" = A."LineNum" 
			LEFT JOIN                       
					(SELECT "ExtTaxSum" "ExtTaxSum_F3","AnalysRpt" "AnalysRpt_F3","Stock" "Stock_F3","lineVat" "lineVat_F3","VatAppldSC" "VatAppldSC_F3","TaxAmtSrc" "TaxAmtSrc_F3","TaxCode" "TaxCode_F3","VatGroup" "VatGroup_F3","ExpnsCode" "ExpnsCode_F3","PaidToDate" "PaidToDate_F3","VatSumFrgn" "VatSumFrgn_F3","VatSum" "VatSum_F3","VatSumSy" "VatSumSy_F3","ObjType" "ObjType_F3","VisOrder" "VisOrder_F3","RetReqSC" "RetReqSC_F3","WtLiable" "WtLiable_F3","TaxStatus" "TaxStatus_F3","OcrCode2" "OcrCode2_F3","EquVatPer" "EquVatPer_F3","DedVatSum" "DedVatSum_F3","TotalSumSy" "TotalSumSy_F3","IsAcquistn" "IsAcquistn_F3","BaseMethod" "BaseMethod_F3","DrawnSC" "DrawnSC_F3","VatAppldFC" "VatAppldFC_F3","ExtTaxSumF" "ExtTaxSumF_F3","VatGrpSrc" "VatGrpSrc_F3","Project" "Project_F3","ExtTaxRate" "ExtTaxRate_F3","VatApplied" "VatApplied_F3","ExtTaxSumS" "ExtTaxSumS_F3","RetReqLC" "RetReqLC_F3","TaxType" "TaxType_F3","TrgGroup" "TrgGroup_F3","DrawnTotal" "DrawnTotal_F3","VatPrcnt" "VatPrcnt_F3","LineNum" "LineNum_F3","lineVatlF" "lineVatlF_F3","BaseGroup" "BaseGroup_F3","EquVatSumF" "EquVatSumF_F3","TotalFrgn" "TotalFrgn_F3","LstPchPrce" "LstPchPrce_F3","LineTotal" "LineTotal_F3","FixCurr" "FixCurr_F3","DocEntry" "DocEntry_F3","Status" "Status_F3","EquVatSumS" "EquVatSumS_F3","LogInstanc" "LogInstanc_F3","OcrCode" "OcrCode_F3","EquVatSum" "EquVatSum_F3","RetReqFC" "RetReqFC_F3","VatDscntPr" "VatDscntPr_F3","PaidSys" "PaidSys_F3","GroupNum" "GroupNum_F3","DedVatSumF" "DedVatSumF_F3","OcrCode3" "OcrCode3_F3","OcrCode4" "OcrCode4_F3","OcrCode5" "OcrCode5_F3","lineVatS" "lineVatS_F3","DrawnFC" "DrawnFC_F3","DedVatSumS" "DedVatSumS_F3","PaidFC" "PaidFC_F3" FROM ' || :tbl_expns || ' WHERE "GroupNum"=2)
					 FG3 ON A."DocEntry"=FG3."DocEntry_F3" AND FG3."LineNum_F3" = A."LineNum"';
	END if;

	sqlQuery = 'DO
				BEGIN
					create local temporary table #tbl as (SELECT	 A.*
						,(select cast('''' as ntext) from dummy) "LineText"
						,case when b."ManBtchNum"=''Y'' then ''B'' when b."ManSerNum"=''Y'' then ''S'' else ''N'' END as "ManBatchSerial"
						,case when b."ManBtchNum"=''Y'' then ''B'' when b."ManSerNum"=''Y'' then ''S'' else ''N'' END as "ManBtchSerial"
						,ifnull(C."Rate",null) "TaxPercentagePerRow"
						,ifnull(C."Rate",null) "RateVat"
						,ifnull(A."TreeType", null) AS "BOMTreeType"
						, ifnull(A."LineNum",null) AS "TempBaseLine"
						, ifnull(A."DocEntry",null) AS "TempBaseEntry"
						, ifnull(Header."DocNum", null) AS "TempBaseRef"
						,''' || :basetype || ''' "TempBaseType"
						,ROW_NUMBER() OVER ( ORDER BY A."DocEntry",A."LineNum" ASC) AS "TempLineNum"
						,ROW_NUMBER() OVER ( ORDER BY A."DocEntry",A."VisOrder" ASC) AS "TempVisOrder"
						, b."SalUnitMsr" AS "SalUnitMsr"
						, ifnull(b."NumInSale", null) AS "NumInSale"
						, b."BuyUnitMsr" AS "BuyUnitMsr"
						, ifnull(b."NumInBuy", null) AS "NumInBuy"
						, ifnull(b."UgpEntry", -1) AS "UgpCode"
						, b."InvntryUom" AS "InvUnitMsr"
						, ifnull(T4."AltQty", 1) / ifnull(T4."BaseQty", 1) AS "NumInInven"
						, cast(null as integer) AS "LineSeq"
						, ifnull(b."OnHand", null) AS "QuantityOnStock"
						, ifnull(b1."OnHand", null) AS "QuantityOnWareHouse"
						, ifnull(b."IsCommited", null) AS "IsCommited"
						, ifnull(b1."IsCommited", null) AS "IsCommitedInWareHouse"
						, ifnull(b."OnOrder", null) AS "OnOrder"
						, ifnull(b1."OnOrder", null) AS "OnOrderInWareHouse"
						, b."PurPackUn", b."SalPackUn", b1."AvgPrice"
						, (1 - ((1 - A."DiscPrcnt" / 100) * (1 - Header."DiscPrcnt" / 100))) * 100 AS "TempDisc"';
	IF :fromtype='700021'
	then
		sqlQuery= :sqlQuery || ' ,T5."Quantity" "ShipQuantity" ';
	END if;
	
	sqlQuery= :sqlQuery || ',FG1.*,FG2.*,FG3.* ';
	sqlQuery= :sqlQuery || '
				FROM ' || :tblline || ' A INNER JOIN ' || :tblname || ' Header on A."DocEntry"=Header."DocEntry"';
				
	IF :fromtype='700021'
	then
		sqlQuery= :sqlQuery || ' INNER JOIN ' || :tblname || ' X ON A."DocEntry"=X."DocEntry" INNER JOIN #tblship T5 ON T5."TransType"=A."ObjType" and T5."TransNo"=X."DocNum" and T5."TransRow"=A."LineNum"';
	END	if;

	sqlQuery= :sqlQuery || ' LEFT JOIN OVTG C on C."Code"=A."VatGroup"  
					INNER JOIN OITM b on a."ItemCode" = b."ItemCode"
					LEFT JOIN OITW b1 on a."ItemCode" = b1."ItemCode" and a."WhsCode" = b1."WhsCode"
					LEFT JOIN UGP1 T4 on b."UgpEntry" = T4."UgpEntry" and T4."UomEntry"=b."IUoMEntry" ';
	sqlQuery= :sqlQuery || :sql_expns;
	IF :fromtype<>'700021'
	then
		sqlQuery= :sqlQuery || '
			WHERE  A."DocEntry" IN (SELECT Data FROM split(''' || :docentry || ''','',''))
			-- Các Line còn Open
				AND (
				(A."LineStatus"=''O'' AND (A."OpenQty" <> 0 OR A."Quantity"=0))
				or (''' || :closedQuote || '''=''Y'' And ''23''=' || :fromtype || ' ) 
				OR (''' || :fromtype || '''=''112'')
				));
			-- Số lượng open >0';
	ELSE
		sqlQuery= :sqlQuery || ';';
	END	if;
		
	IF :fromtype<>'112'
	then
		--Nếu line chưa copy to lần nào (Quantity=OpenQty) thì giữ nguyên LineTotal, nếu ko thì tính lại LineTotal dưới form
		sqlQuery= :sqlQuery || '
		Update #tbl set "LineTotal"=0 where "Quantity"<>"OpenQty";
		Update #tbl Set 
						"Quantity"="OpenQty"
						Where (select "DocStatus" From ' || :tblname || ' where "DocEntry"=#tbl."DocEntry")<>''C'';';
		sqlQuery= :sqlQuery || ' Update #tbl Set  "LineStatus"=''O'';
						update #tbl set "BaseType"="TempBaseType"
								,"BaseEntry"="TempBaseEntry"
								,"BaseRef" ="TempBaseRef"
								,"BaseLine"="TempBaseLine"
								,"LineNum"="TempLineNum"-1
								,"VisOrder"="TempVisOrder"-1
								,"TargetType"=-1
								,"TrgetEntry"=0; ';
								
		IF (select count(*) from split(:docentry,',')) > 1
		then
	
			sqlQuery= :sqlQuery || ' 
				update #tbl set "DiscPrcnt"="TempDisc","LineTotal"=0 where ifnull("DiscPrcnt",0)<>ifnull("TempDisc",0); ';
			
		end	if;    	
	END	 if;
	
	IF :fromtype='700021'
	then
		sqlQuery= :sqlQuery || ' update #tbl set "Quantity"="ShipQuantity","OpenQty"="ShipQuantity"; 
			UPDATE T0 SET T0."InvQty"=T0."Quantity", T0."NumPerMsr"=1 FROM #tbl  T0 WHERE T0."UseBaseUn"=''Y'';
			UPDATE T0 SET T0."InvQty"=T0."Quantity" * T0."NumPerMsr" FROM #tbl  T0 WHERE T0."UseBaseUn="''N'';';
	END if;
	
	if :toType = '203' or :toType = '204' then
		sqlQuery= :sqlQuery || '
			update #tbl set "CogsAcct"=null';
		if :dpmInvoice = 'Y' then
			sqlQuery= :sqlQuery || ', "AcctCode"=null';
		end if;
		sqlQuery= :sqlQuery || ';';
	end if;
	
	sqlQuery= :sqlQuery || ' 
			Update #tbl SET "PQTReqQty"="Quantity","Quantity"=0 WHERE ''' || :fromtype || ''' = ''1470000113'' AND ''' || :totype || '''=''540000006'';
			Update #tbl SET "LineVendor"=null WHERE ''' || :fromtype || ''' = ''1470000113'' AND ''' || :totype || '''<>''1470000113'';
			Update #tbl Set "Rate"=1 where "Rate"=0;
			--alter table #tbl
							--alter column itemcode ntext
			INSERT INTO #tbl ("BOMTreeType", "LineStatus", "DocEntry", "LineNum", "ItemCode", "LineText", "VisOrder", "Dscription", "SalUnitMsr", "NumInSale", "BuyUnitMsr", "NumInBuy", "UgpCode", "InvUnitMsr", "NumInInven", "ManBatchSerial", "ManBtchSerial", "LineType", "TaxPercentagePerRow", "RateVat", "TempBaseLine", "TempBaseEntry", "TempBaseType", "TempLineNum", "LineSeq", "QuantityOnStock", "QuantityOnWareHouse", "IsCommited", "IsCommitedInWareHouse", "OnOrder", "OnOrderInWareHouse", "ItemType")
							SELECT '''', ''O'',"DocEntry","AftLineNum",'''', "LineText","AftLineNum",CASE WHEN "LineType"=''S'' THEN ''SubTotal'' ELSE '''' END,'''',0,'''',0,-1,'''',0,''N'', ''N'',"LineType",0,0,0,0,-1,0,"LineSeq",0,0,0,0,0,0,-18
							FROM ' || :tbl10 || ' WHERE "DocEntry" IN (SELECT Data FROM split(''' || :docentry || ''','',''));
			update #tbl set "ShipToDesc" = NULL WHERE ifnull("ShipToCode",'''')='''';
			update #tbl set "InvQty" = round("Quantity" * "NumPerMsr",' || (select cast("QtyDec" as nvarchar) from OADM) || ');
		END;';
	--PRINT @sql
	--select :sqlQuery from DUMMy;
	--return;
	EXEC :sqlQuery;
	execute immediate 'SELECT  * FROM #tbl order by "VisOrder","LineSeq";';
	exec 'DROP TABLE #tbl;';
END