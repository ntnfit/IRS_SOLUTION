
CALL APZ_EXISTSPROC('APZ_InventoryTransferRequest_GetDataLine');

GO

CREATE PROCEDURE APZ_InventoryTransferRequest_GetDataLine(in docEntry int)
AS
BEGIN
	---------------------------------------------- GET DATA LINES --------------------------------------------------------------------------------
	SELECT a.*, CASE WHEN b."ManBtchNum" = 'Y' THEN 'B' WHEN b."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", IFNULL(b."OnHand", 0) AS "OnHandItem", IFNULL(b."IsCommited", 0) AS "IsCommittedItem"
			, IFNULL(b."OnOrder", 0) AS "OnOrderedItem", IFNULL(c."OnHand", 0) AS "OnHandWhs", IFNULL(c."IsCommited", 0) AS "IsCommittedWhs", IFNULL(c."OnOrder", 0) AS "OnOrderedWhs", b."SalUnitMsr" AS "SalUnitMsr"
			, IFNULL(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 0) AS "NumInBuy", IFNULL(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr"
			, IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) AS "NumInInven"
			, CASE WHEN IFNULL(a."OcrCode", '') = '' AND IFNULL(a."OcrCode2", '') = '' AND IFNULL(a."OcrCode3", '') = '' AND IFNULL(a."OcrCode4", '') = '' AND IFNULL(a."OcrCode5", '') = '' 
					THEN NULL ELSE IFNULL(a."OcrCode", '') || ';' || IFNULL(a."OcrCode2", '') || ';' || IFNULL(a."OcrCode3", '') || ';' || IFNULL(a."OcrCode4", '') || ';' || IFNULL(a."OcrCode5", '') END AS "Rules"
			, c."AvgPrice" FROM WTQ1 a INNER JOIN OITM b ON a."ItemCode" = b."ItemCode"
						    inner join OITW c on a."ItemCode" = c."ItemCode" and a."FromWhsCod" = c."WhsCode"
							left join UGP1 d on b."UgpEntry" = d."UgpEntry" and d."UomEntry"=b."IUoMEntry"
	where a."DocEntry" = :docEntry order by "VisOrder";
	---------------------------------------------- END GET DATA LINES --------------------------------------------------------------------------

	----------------------------------------- GET DATA BATCH AND SERIAL -------------------------------------------------------------------------
	create local temporary table #tempBatchSerial (docentry int);
	call apz_fun_getbatchserial_byDocEntry(:docEntry, '1250000001', '#tempBatchSerial');
	select * from #tempBatchSerial;
	--drop table #tempBatchSerial;
	----------------------------------------- END GET DATA BATCH AND SERIAL -------------------------------------------------------------------

	----------------------------------------- GET DATA BIN ------------------------------------------------------------------------------------
	--create local temporary table #tempBinAllowcation (docentry int);
	call apz_fun_document_getBinAllowcationLine_byDocEntry(:docentry, '1250000001', '#tempBatchSerial');
	select * from #tempBatchSerial;
	drop table #tempBatchSerial;
	----------------------------------------- END GET DATA BIN -------------------------------------------------------------------------------
	----------------------------------------- GET DATA REFERENCED ------------------------------------------------------------------------------------
	Select * From WTQ21 Where "DocEntry" = :docEntry;
	----------------------------------------- END GET DATA REFERENCED -------------------------------------------------------------------------------
END