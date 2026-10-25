
-- quandn
CALL APZ_EXISTSPROC('APZ_InventoryTransaction_GoodIssueIT_GetLines');

GO

CREATE PROCEDURE APZ_InventoryTransaction_GoodIssueIT_GetLines (
	IN 	docEntry INT DEFAULT 0
)
AS
BEGIN
	-------------------------------------- GET DATA LINES --------------------------------------------------------------
	SELECT A.*, CASE WHEN b."ManBtchNum" = 'Y' THEN 'B' WHEN b."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", b."OnHand" AS "QuantityOnStock", c."OnHand" AS "QuantityOnWareHouse", b."SalUnitMsr" AS "SalUnitMsr", IFNULL(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 0) AS "NumInBuy", IFNULL(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr", IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) AS "NumInInven" 
		, case when ifnull(a."OcrCode", '') = '' and ifnull(a."OcrCode2", '') = '' and ifnull(a."OcrCode3", '') = '' and ifnull(a."OcrCode4", '') = '' and ifnull(a."OcrCode5", '') = '' then null 
				else ifnull(a."OcrCode", '') || ';' || ifnull(a."OcrCode2", '') || ';' || ifnull(a."OcrCode3", '') || ';' || ifnull(a."OcrCode4", '') || ';' || ifnull(a."OcrCode5", '') end "Rules", c."AvgPrice"
		FROM IGE1 A 
		INNER JOIN OITM b ON A."ItemCode" = b."ItemCode" 
		INNER JOIN OITW c ON A."ItemCode" = c."ItemCode" AND A."WhsCode" = c."WhsCode" 
		LEFT JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry"=b."IUoMEntry"
		WHERE A."DocEntry" = :docEntry;
	------------------------------------- END GET DATA LINES --------------------------------------------------------
	
	----------------------------------------- GET DATA BATCH AND SERIAL -------------------------------------------------------------------------
	create local temporary table #tempBatchSerial (docentry int);
	call apz_fun_getbatchserial_byDocEntry(:docEntry, '60', '#tempBatchSerial');
	select * from #tempBatchSerial;
	drop table #tempBatchSerial;
	----------------------------------------- END GET DATA BATCH AND SERIAL -------------------------------------------------------------------

	----------------------------------------- GET DATA BIN ------------------------------------------------------------------------------------
	create local temporary table #tempBinAllowcation (docentry int);
	call apz_fun_document_getBinAllowcationLine_byDocEntry(:docentry, '60', '#tempBinAllowcation');
	select * from #tempBinAllowcation;
	drop table #tempBinAllowcation;
	----------------------------------------- END GET DATA BIN -------------------------------------------------------------------------------
	
END;