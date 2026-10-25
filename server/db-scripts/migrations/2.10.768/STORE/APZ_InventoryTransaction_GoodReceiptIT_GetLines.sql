
CALL APZ_EXISTSPROC('APZ_InventoryTransaction_GoodReceiptIT_GetLines');

GO

CREATE PROCEDURE APZ_InventoryTransaction_GoodReceiptIT_GetLines (
	IN 	docEntry INT DEFAULT 0
)
AS
BEGIN
	-------------------------------------------- GET DATA LINES ----------------------------------------------------------------------------------
	SELECT A.*, CASE WHEN b."ManBtchNum" = 'Y' THEN 'B' WHEN b."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", b."OnHand" AS "QuantityOnStock", c."OnHand" AS "QuantityOnWareHouse", b."SalUnitMsr" AS "SalUnitMsr", IFNULL(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 0) AS "NumInBuy", IFNULL(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr", IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) AS "NumInInven" 
		, case when ifnull(a."OcrCode", '') = '' and ifnull(a."OcrCode2", '') = '' and ifnull(a."OcrCode3", '') = '' and ifnull(a."OcrCode4", '') = '' and ifnull(a."OcrCode5", '') = '' then null 
				else ifnull(a."OcrCode", '') || ';' || ifnull(a."OcrCode2", '') || ';' || ifnull(a."OcrCode3", '') || ';' || ifnull(a."OcrCode4", '') || ';' || ifnull(a."OcrCode5", '') end "Rules", c."AvgPrice"
		FROM IGN1 A 
		INNER JOIN OITM b ON A."ItemCode" = b."ItemCode" 
		INNER JOIN OITW c ON A."ItemCode" = c."ItemCode" AND A."WhsCode" = c."WhsCode" 
		LEFT JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry"=b."IUoMEntry"
		WHERE A."DocEntry" = :docEntry 
		ORDER BY A."VisOrder";
	--------------------------------------------- END GET DATA LINES ----------------------------------------------------------------------------
	
	----------------------------------------- GET DATA BATCH AND SERIAL -------------------------------------------------------------------------
	call apz_fun_getbatchserial_byDocEntry(:docEntry, '59', batchSerial);
	select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
	from :batchSerial a join OBTN b on a."ManBatchSerial"='B' and a."AbsEntry"=b."AbsEntry"
	union all
	select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
	from :batchSerial a join OSRN b on a."ManBatchSerial"='S' and a."AbsEntry"=b."AbsEntry";
	----------------------------------------- END GET DATA BATCH AND SERIAL -------------------------------------------------------------------

	----------------------------------------- GET DATA BIN ------------------------------------------------------------------------------------
	call apz_fun_document_getBinAllowcationLine_byDocEntry(:docentry, '59', bin);
	select * from :bin a left join OBTN b on 1 = 0;
	----------------------------------------- END GET DATA BIN -------------------------------------------------------------------------------
END