
CALL APZ_EXISTSPROC('apz_Document_GetDocLine');

GO


CREATE PROCEDURE apz_Document_GetDocLine(
in DocEntry INT,
in tbl VARCHAR(30),
in tblLine NVARCHAR(30))
AS
BEGIN
	DECLARE SQLQuery VARCHAR(5000);
	DECLARE EnblExpns CHAR(1) ='N';
	DECLARE tbl10 NVARCHAR(10)='';
	DECLARE sql_expns NVARCHAR(1000)='';
	DECLARE tbl_expns CHAR(4);
	
	tbl10=SUBSTRING(:tblLine,0,LENGTH(:tblLine) - 1) || '10';
	EnblExpns='Y';--ISNULL((SELECT EnblExpns FROM cinf),'N')

	IF	:EnblExpns='Y'
	then
	
		tbl_expns=SUBSTRING(:tblLine,0,LENGTH(:tblLine) - 1) || '2';

	END if;
	
	SQLQuery='insert into #tbl
		SELECT A.*,'''',(select "Rate" from OVTG where "Code"=A."VatGroup"),'''', b."SalUnitMsr", ifnull(b."NumInSale",0), b."BuyUnitMsr", ifnull(b."NumInBuy",0), 
											ifnull(b."UgpEntry",-1), b."InvntryUom", ifnull(d."AltQty", 1) / IfNULL(d."BaseQty", 1),
											case when b."ManBtchNum"=''Y'' then ''B'' when b."ManSerNum"=''Y'' then ''S'' else ''N'' END,0, Ifnull(b."OnHand",0)
											, ifnull(t1."OnHand",0),Ifnull(b."IsCommited",0), ifnull(t1."IsCommited",0),
											 Ifnull(b."OnOrder",0), ifnull(t1."OnOrder",0), b."PurPackUn", b."SalPackUn", t1."AvgPrice", (select ifnull("Number",0) from OOAT where "AbsID"=A."AgrNo") ';
	IF :EnblExpns='Y'
	then
		SQLQuery=:SQLQuery || ',FG1."lineVatS",FG1."DrawnSC",FG1."RetReqLC",FG1."WtLiable",FG1."DedVatSumF",FG1."ExpnsCode",FG1."OcrCode",FG1."DedVatSumS",FG1."TrgGroup",FG1."VatGroup",FG1."PaidFC",FG1."ObjType",FG1."EquVatSumF",FG1."ExtTaxSum",FG1."VatSumSy",FG1."EquVatSumS",FG1."TaxAmtSrc",FG1."TotalSumSy",FG1."ExtTaxSumS",FG1."VatDscntPr",FG1."PaidToDate",FG1."VatSumFrgn",FG1."RetReqSC",FG1."TaxStatus",FG1."EquVatPer",FG1."DedVatSum",FG1."Project",FG1."VatSum",FG1."IsAcquistn",FG1."BaseMethod",FG1."VatAppldFC",FG1."TaxType",FG1."ExtTaxSumF",FG1."VatGrpSrc",FG1."ExtTaxRate",FG1."DrawnTotal",FG1."VatApplied",FG1."Stock",FG1."LineNum",FG1."VisOrder",FG1."BaseGroup",FG1."AnalysRpt",FG1."VatPrcnt",FG1."LineTotal",FG1."FixCurr",FG1."DocEntry",FG1."lineVatlF",FG1."VatAppldSC",FG1."TotalFrgn",FG1."LstPchPrce",FG1."TaxCode",FG1."Status",FG1."PaidSys",FG1."LogInstanc",FG1."EquVatSum",FG1."RetReqFC",FG1."DrawnFC",FG1."GroupNum",FG1."OcrCode2",FG1."OcrCode3",FG1."OcrCode4",FG1."OcrCode5",FG1."lineVat",
FG2."lineVatS",FG2."DrawnSC",FG2."RetReqLC",FG2."WtLiable",FG2."DedVatSumF",FG2."ExpnsCode",FG2."OcrCode",FG2."DedVatSumS",FG2."TrgGroup",FG2."VatGroup",FG2."PaidFC",FG2."ObjType",FG2."EquVatSumF",FG2."ExtTaxSum",FG2."VatSumSy",FG2."EquVatSumS",FG2."TaxAmtSrc",FG2."TotalSumSy",FG2."ExtTaxSumS",FG2."VatDscntPr",FG2."PaidToDate",FG2."VatSumFrgn",FG2."RetReqSC",FG2."TaxStatus",FG2."EquVatPer",FG2."DedVatSum",FG2."Project",FG2."VatSum",FG2."IsAcquistn",FG2."BaseMethod",FG2."VatAppldFC",FG2."TaxType",FG2."ExtTaxSumF",FG2."VatGrpSrc",FG2."ExtTaxRate",FG2."DrawnTotal",FG2."VatApplied",FG2."Stock",FG2."LineNum",FG2."VisOrder",FG2."BaseGroup",FG2."AnalysRpt",FG2."VatPrcnt",FG2."LineTotal",FG2."FixCurr",FG2."DocEntry",FG2."lineVatlF",FG2."VatAppldSC",FG2."TotalFrgn",FG2."LstPchPrce",FG2."TaxCode",FG2."Status",FG2."PaidSys",FG2."LogInstanc",FG2."EquVatSum",FG2."RetReqFC",FG2."DrawnFC",FG2."GroupNum",FG2."OcrCode2",FG2."OcrCode3",FG2."OcrCode4",FG2."OcrCode5",FG2."lineVat",
FG3."lineVatS",FG3."DrawnSC",FG3."RetReqLC",FG3."WtLiable",FG3."DedVatSumF",FG3."ExpnsCode",FG3."OcrCode",FG3."DedVatSumS",FG3."TrgGroup",FG3."VatGroup",FG3."PaidFC",FG3."ObjType",FG3."EquVatSumF",FG3."ExtTaxSum",FG3."VatSumSy",FG3."EquVatSumS",FG3."TaxAmtSrc",FG3."TotalSumSy",FG3."ExtTaxSumS",FG3."VatDscntPr",FG3."PaidToDate",FG3."VatSumFrgn",FG3."RetReqSC",FG3."TaxStatus",FG3."EquVatPer",FG3."DedVatSum",FG3."Project",FG3."VatSum",FG3."IsAcquistn",FG3."BaseMethod",FG3."VatAppldFC",FG3."TaxType",FG3."ExtTaxSumF",FG3."VatGrpSrc",FG3."ExtTaxRate",FG3."DrawnTotal",FG3."VatApplied",FG3."Stock",FG3."LineNum",FG3."VisOrder",FG3."BaseGroup",FG3."AnalysRpt",FG3."VatPrcnt",FG3."LineTotal",FG3."FixCurr",FG3."DocEntry",FG3."lineVatlF",FG3."VatAppldSC",FG3."TotalFrgn",FG3."LstPchPrce",FG3."TaxCode",FG3."Status",FG3."PaidSys",FG3."LogInstanc",FG3."EquVatSum",FG3."RetReqFC",FG3."DrawnFC",FG3."GroupNum",FG3."OcrCode2",FG3."OcrCode3",FG3."OcrCode4",FG3."OcrCode5",FG3."lineVat" ';
	end if;
	
	SQLQuery=:SQLQuery || '
	FROM ' || :tblLine || ' A
											INNER JOIN OITM b on a."ItemCode" = b."ItemCode"
											LEFT JOIN OITW t1 on a."ItemCode" = t1."ItemCode" and a."WhsCode" = t1."WhsCode"
											LEFT JOIN UGP1 d on b."UgpEntry" = d."UgpEntry" and d."UomEntry" = b."IUoMEntry"';
											
							
	IF :EnblExpns='Y'
	then
		sql_expns := 'LEFT JOIN ' || :tbl_expns || ' FG1 ON FG1."GroupNum"=0 AND A."DocEntry"=FG1."DocEntry" AND FG1."LineNum" = A."LineNum" 
					  LEFT JOIN ' || :tbl_expns || ' FG2 ON FG2."GroupNum"=1 AND A."DocEntry"=FG2."DocEntry" AND FG2."LineNum" = A."LineNum"
					  LEFT JOIN ' || :tbl_expns || ' FG3 ON FG3."GroupNum"=2 AND A."DocEntry"=FG3."DocEntry" AND FG3."LineNum" = A."LineNum"';
		
		SQLQuery=:SQLQuery || :sql_expns;
	end if;
	
	create local temporary table #tbl as 
				(SELECT top 0 *
							,cast(null as ntext) "LineText",
							cast(null as decimal(21,6))"RateVat",
							cast(null as char(1)) "BOMTreeType",
							cast(null as nvarchar(100)) "SalUnitMsr",
							cast(null as decimal(21,6)) "NumInSale",
							cast(null as nvarchar(100)) "BuyUnitMsr",
							cast(null as decimal(21,6)) "NumInBuy", 
							cast(null as int) "UgpCode",
							cast(null as nvarchar(100)) "InvUnitMsr",
							cast(null as decimal(21,6)) "NumInInven",
							'B' "ManBatchSerial",0 "LineSeq",
							cast(null as decimal(21,6)) "QuantityOnStock"
							, cast(null as decimal(21,6)) "QuantityOnWareHouse",
							cast(null as decimal(21,6)) "IsCommited",
							cast(null as decimal(21,6)) "IsCommitedInWareHouse",
							 cast(null as decimal(21,6)) "OnOrder", 
							 cast(null as decimal(21,6)) "OnOrderInWareHouse", 
							 cast(null as decimal(21,6)) "PurPackUn", 
							 cast(null as decimal(21,6)) "SalPackUn", 
							 cast(null as decimal(21,6)) "AvgPrice", 
							 cast(null as int) "AgrNumber",
							cast(null as decimal(21,6)) "lineVatS_F1",cast(null as decimal(21,6)) "DrawnSC_F1",cast(null as decimal(21,6)) "RetReqLC_F1",cast(null as nvarchar(1)) "WtLiable_F1",cast(null as decimal(21,6)) "DedVatSumF_F1",cast(null as int) "ExpnsCode_F1",cast(null as nvarchar(8)) "OcrCode_F1",cast(null as decimal(21,6)) "DedVatSumS_F1",cast(null as int) "TrgGroup_F1",cast(null as nvarchar(8)) "VatGroup_F1",cast(null as decimal(21,6)) "PaidFC_F1",cast(null as nvarchar(20)) "ObjType_F1",cast(null as decimal(21,6)) "EquVatSumF_F1",cast(null as decimal(21,6)) "ExtTaxSum_F1",cast(null as decimal(21,6)) "VatSumSy_F1",cast(null as decimal(21,6)) "EquVatSumS_F1",cast(null as nvarchar(1)) "TaxAmtSrc_F1",cast(null as decimal(21,6)) "TotalSumSy_F1",cast(null as decimal(21,6)) "ExtTaxSumS_F1",cast(null as decimal(21,6)) "VatDscntPr_F1",cast(null as decimal(21,6)) "PaidToDate_F1",cast(null as decimal(21,6)) "VatSumFrgn_F1",cast(null as decimal(21,6)) "RetReqSC_F1",cast(null as nvarchar(1)) "TaxStatus_F1",cast(null as decimal(21,6)) "EquVatPer_F1",cast(null as decimal(21,6)) "DedVatSum_F1",cast(null as nvarchar(20)) "Project_F1",cast(null as decimal(21,6)) "VatSum_F1",cast(null as nvarchar(1)) "IsAcquistn_F1",cast(null as nvarchar(1)) "BaseMethod_F1",cast(null as decimal(21,6)) "VatAppldFC_F1",cast(null as nvarchar(1)) "TaxType_F1",cast(null as decimal(21,6)) "ExtTaxSumF_F1",cast(null as nvarchar(1)) "VatGrpSrc_F1",cast(null as decimal(21,6)) "ExtTaxRate_F1",cast(null as decimal(21,6)) "DrawnTotal_F1",cast(null as decimal(21,6)) "VatApplied_F1",cast(null as nvarchar(1)) "Stock_F1",cast(null as int) "LineNum_F1",cast(null as int) "VisOrder_F1",cast(null as int) "BaseGroup_F1",cast(null as nvarchar(1)) "AnalysRpt_F1",cast(null as decimal(21,6)) "VatPrcnt_F1",cast(null as decimal(21,6)) "LineTotal_F1",cast(null as nvarchar(3)) "FixCurr_F1",cast(null as int) "DocEntry_F1",cast(null as decimal(21,6)) "lineVatlF_F1",cast(null as decimal(21,6)) "VatAppldSC_F1",cast(null as decimal(21,6)) "TotalFrgn_F1",cast(null as nvarchar(1)) "LstPchPrce_F1",cast(null as nvarchar(8)) "TaxCode_F1",cast(null as nvarchar(1)) "Status_F1",cast(null as decimal(21,6)) "PaidSys_F1",cast(null as int) "LogInstanc_F1",cast(null as decimal(21,6)) "EquVatSum_F1",cast(null as decimal(21,6)) "RetReqFC_F1",cast(null as decimal(21,6)) "DrawnFC_F1",cast(null as int) "GroupNum_F1",cast(null as nvarchar(8)) "OcrCode2_F1",cast(null as nvarchar(8)) "OcrCode3_F1",cast(null as nvarchar(8)) "OcrCode4_F1",cast(null as nvarchar(8)) "OcrCode5_F1",cast(null as decimal(21,6)) "lineVat_F1",
							cast(null as decimal(21,6)) "lineVatS_F2",cast(null as decimal(21,6)) "DrawnSC_F2",cast(null as decimal(21,6)) "RetReqLC_F2",cast(null as nvarchar(1)) "WtLiable_F2",cast(null as decimal(21,6)) "DedVatSumF_F2",cast(null as int) "ExpnsCode_F2",cast(null as nvarchar(8)) "OcrCode_F2",cast(null as decimal(21,6)) "DedVatSumS_F2",cast(null as int) "TrgGroup_F2",cast(null as nvarchar(8)) "VatGroup_F2",cast(null as decimal(21,6)) "PaidFC_F2",cast(null as nvarchar(20)) "ObjType_F2",cast(null as decimal(21,6)) "EquVatSumF_F2",cast(null as decimal(21,6)) "ExtTaxSum_F2",cast(null as decimal(21,6)) "VatSumSy_F2",cast(null as decimal(21,6)) "EquVatSumS_F2",cast(null as nvarchar(1)) "TaxAmtSrc_F2",cast(null as decimal(21,6)) "TotalSumSy_F2",cast(null as decimal(21,6)) "ExtTaxSumS_F2",cast(null as decimal(21,6)) "VatDscntPr_F2",cast(null as decimal(21,6)) "PaidToDate_F2",cast(null as decimal(21,6)) "VatSumFrgn_F2",cast(null as decimal(21,6)) "RetReqSC_F2",cast(null as nvarchar(1)) "TaxStatus_F2",cast(null as decimal(21,6)) "EquVatPer_F2",cast(null as decimal(21,6)) "DedVatSum_F2",cast(null as nvarchar(20)) "Project_F2",cast(null as decimal(21,6)) "VatSum_F2",cast(null as nvarchar(1)) "IsAcquistn_F2",cast(null as nvarchar(1)) "BaseMethod_F2",cast(null as decimal(21,6)) "VatAppldFC_F2",cast(null as nvarchar(1)) "TaxType_F2",cast(null as decimal(21,6)) "ExtTaxSumF_F2",cast(null as nvarchar(1)) "VatGrpSrc_F2",cast(null as decimal(21,6)) "ExtTaxRate_F2",cast(null as decimal(21,6)) "DrawnTotal_F2",cast(null as decimal(21,6)) "VatApplied_F2",cast(null as nvarchar(1)) "Stock_F2",cast(null as int) "LineNum_F2",cast(null as int) "VisOrder_F2",cast(null as int) "BaseGroup_F2",cast(null as nvarchar(1)) "AnalysRpt_F2",cast(null as decimal(21,6)) "VatPrcnt_F2",cast(null as decimal(21,6)) "LineTotal_F2",cast(null as nvarchar(3)) "FixCurr_F2",cast(null as int) "DocEntry_F2",cast(null as decimal(21,6)) "lineVatlF_F2",cast(null as decimal(21,6)) "VatAppldSC_F2",cast(null as decimal(21,6)) "TotalFrgn_F2",cast(null as nvarchar(1)) "LstPchPrce_F2",cast(null as nvarchar(8)) "TaxCode_F2",cast(null as nvarchar(1)) "Status_F2",cast(null as decimal(21,6)) "PaidSys_F2",cast(null as int) "LogInstanc_F2",cast(null as decimal(21,6)) "EquVatSum_F2",cast(null as decimal(21,6)) "RetReqFC_F2",cast(null as decimal(21,6)) "DrawnFC_F2",cast(null as int) "GroupNum_F2",cast(null as nvarchar(8)) "OcrCode2_F2",cast(null as nvarchar(8)) "OcrCode3_F2",cast(null as nvarchar(8)) "OcrCode4_F2",cast(null as nvarchar(8)) "OcrCode5_F2",cast(null as decimal(21,6)) "lineVat_F2",
							cast(null as decimal(21,6)) "lineVatS_F3",cast(null as decimal(21,6)) "DrawnSC_F3",cast(null as decimal(21,6)) "RetReqLC_F3",cast(null as nvarchar(1)) "WtLiable_F3",cast(null as decimal(21,6)) "DedVatSumF_F3",cast(null as int) "ExpnsCode_F3",cast(null as nvarchar(8)) "OcrCode_F3",cast(null as decimal(21,6)) "DedVatSumS_F3",cast(null as int) "TrgGroup_F3",cast(null as nvarchar(8)) "VatGroup_F3",cast(null as decimal(21,6)) "PaidFC_F3",cast(null as nvarchar(20)) "ObjType_F3",cast(null as decimal(21,6)) "EquVatSumF_F3",cast(null as decimal(21,6)) "ExtTaxSum_F3",cast(null as decimal(21,6)) "VatSumSy_F3",cast(null as decimal(21,6)) "EquVatSumS_F3",cast(null as nvarchar(1)) "TaxAmtSrc_F3",cast(null as decimal(21,6)) "TotalSumSy_F3",cast(null as decimal(21,6)) "ExtTaxSumS_F3",cast(null as decimal(21,6)) "VatDscntPr_F3",cast(null as decimal(21,6)) "PaidToDate_F3",cast(null as decimal(21,6)) "VatSumFrgn_F3",cast(null as decimal(21,6)) "RetReqSC_F3",cast(null as nvarchar(1)) "TaxStatus_F3",cast(null as decimal(21,6)) "EquVatPer_F3",cast(null as decimal(21,6)) "DedVatSum_F3",cast(null as nvarchar(20)) "Project_F3",cast(null as decimal(21,6)) "VatSum_F3",cast(null as nvarchar(1)) "IsAcquistn_F3",cast(null as nvarchar(1)) "BaseMethod_F3",cast(null as decimal(21,6)) "VatAppldFC_F3",cast(null as nvarchar(1)) "TaxType_F3",cast(null as decimal(21,6)) "ExtTaxSumF_F3",cast(null as nvarchar(1)) "VatGrpSrc_F3",cast(null as decimal(21,6)) "ExtTaxRate_F3",cast(null as decimal(21,6)) "DrawnTotal_F3",cast(null as decimal(21,6)) "VatApplied_F3",cast(null as nvarchar(1)) "Stock_F3",cast(null as int) "LineNum_F3",cast(null as int) "VisOrder_F3",cast(null as int) "BaseGroup_F3",cast(null as nvarchar(1)) "AnalysRpt_F3",cast(null as decimal(21,6)) "VatPrcnt_F3",cast(null as decimal(21,6)) "LineTotal_F3",cast(null as nvarchar(3)) "FixCurr_F3",cast(null as int) "DocEntry_F3",cast(null as decimal(21,6)) "lineVatlF_F3",cast(null as decimal(21,6)) "VatAppldSC_F3",cast(null as decimal(21,6)) "TotalFrgn_F3",cast(null as nvarchar(1)) "LstPchPrce_F3",cast(null as nvarchar(8)) "TaxCode_F3",cast(null as nvarchar(1)) "Status_F3",cast(null as decimal(21,6)) "PaidSys_F3",cast(null as int) "LogInstanc_F3",cast(null as decimal(21,6)) "EquVatSum_F3",cast(null as decimal(21,6)) "RetReqFC_F3",cast(null as decimal(21,6)) "DrawnFC_F3",cast(null as int) "GroupNum_F3",cast(null as nvarchar(8)) "OcrCode2_F3",cast(null as nvarchar(8)) "OcrCode3_F3",cast(null as nvarchar(8)) "OcrCode4_F3",cast(null as nvarchar(8)) "OcrCode5_F3",cast(null as decimal(21,6)) "lineVat_F3"
	from RDR1);
	
	SQLQuery= :SQLQuery || '
				WHERE  A."DocEntry" =' || cast(:DocEntry as nvarchar) || ';';
				exec :SQLQuery;
				--alter table #tbl
				--alter column "ItemCode" nvarchar(5000);
				SQLQuery = 'INSERT INTO #tbl ("BOMTreeType","DocEntry","LineNum","ItemCode","LineText","VisOrder","Dscription","SalUnitMsr","NumInSale","BuyUnitMsr","NumInBuy","UgpCode","InvUnitMsr","NumInInven","ManBatchSerial"
				,"LineType","LineSeq","QuantityOnStock","QuantityOnWareHouse","IsCommited","IsCommitedInWareHouse","OnOrder","OnOrderInWareHouse","ItemType","AgrNumber")
				SELECT '''',"DocEntry","AftLineNum",'''',"LineText","AftLineNum",CASE WHEN "LineType"=''S'' THEN ''SubTotal'' ELSE '''' END,'''',0,'''',0,-1,'''',0,''N'',"LineType","LineSeq",0,0,0,0,0,0,-18,0
				FROM ' || :tbl10 || ' WHERE "DocEntry"=' || cast(:DocEntry as nvarchar) || ';';
				exec :SQLQuery;
							
	update #tbl set "Rate"=1 where "Rate"=0;
	SELECT *,"ManBatchSerial" "ManBtchSerial","IsCommited" "IsCommittedItem", "OnOrder" "OnOrderedItem","QuantityOnStock" "OnHandItem","QuantityOnWareHouse" "OnHandWhs","IsCommitedInWareHouse" "IsCommittedWhs"
		,"OnOrderInWareHouse"  "OnOrderedWhs" FROM #tbl order by "VisOrder","LineSeq";
	
	
	DROP TABLE #tbl;
END	