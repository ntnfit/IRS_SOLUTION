
CALL APZ_EXISTSPROC('Apz_ProductionOrder_GetTransferComponents');

GO

CREATE PROCEDURE Apz_ProductionOrder_GetTransferComponents(in docEntry INT default 0, in ParentQty numeric(19,6) default 0)
AS
begin

	--lấy dữ liệu cardcode = null
	select top 0 cast(null as nvarchar(50)) "CardCode" From DUMMY;

	--lấy cấu trúc table header
	select top 0 a.*, b."Name" as "CntctName", a."DocCur" "Currency" from OWTQ a left join OCPR b on a."CntctCode" = b."CntctCode";

	----------------------------------------------------------- Lấy cấu trúc của table line ------------------------------------------------------------------
	create local temporary table #ret as (
	SELECT TOP 0 a.*, CAST(NULL AS nvarchar(10)) AS "ManBtchSerial", CAST(NULL AS decimal(19, 6)) AS "OnHandItem", CAST(NULL AS decimal(19, 6)) AS "IsCommittedItem", CAST(NULL AS decimal(19, 6)) AS "OnOrderedItem"
		, CAST(NULL AS decimal(19, 6)) AS "OnHandWhs", CAST(NULL AS decimal(19, 6)) AS "IsCommittedWhs", CAST(NULL AS decimal(19, 6)) AS "OnOrderedWhs", CAST(NULL AS nvarchar(10)) AS "SalUnitMsr"
		, CAST(NULL AS decimal(19, 6)) AS "NumInSale", CAST(NULL AS nvarchar(10)) AS "BuyUnitMsr", CAST(NULL AS decimal(19, 6)) AS "NumInBuy", CAST(NULL AS integer) AS "UgpCode", CAST(NULL AS nvarchar(10)) AS "InvUnitMsr"
		, CAST(NULL AS decimal(19, 6)) AS "NumInInven", CAST(NULL AS nvarchar(254)) AS "Rules", CAST(NULL AS decimal(19, 6)) AS "AvgPrice", cast(null as nvarchar(20)) "ProjectHeader"
		FROM (SELECT -1 AS "DocEntry" FROM DUMMY) AS b LEFT OUTER JOIN WTR1 A ON 1 = 2 
		WHERE A."DocEntry" = 0);

	----------------------------------------------------------- Insert dữ liệu copy vào table line ----------------------------------------------------------
	INSERT INTO #ret ("ItemType", "DocEntry", "LineNum", "BaseType", "ItemCode", "Dscription", "Quantity", "WhsCode", "FromWhsCod", "Project", "OcrCode", "OcrCode2", "OcrCode3", "OcrCode4", "OcrCode5", "ManBtchSerial"
		, "OnHandItem", "IsCommittedItem", "OnOrderedItem", "OnHandWhs", "IsCommittedWhs", "OnOrderedWhs", "SalUnitMsr", "NumInSale", "BuyUnitMsr", "NumInBuy", "UgpCode", "InvUnitMsr", "NumInInven", "Rules", "AvgPrice", "ProjectHeader") 
	SELECT a."ItemType", 0, ROW_NUMBER() OVER ( ORDER BY t1."DocNum"), -1, a."ItemCode", b."ItemName"
		, CASE WHEN :ParentQty = 0 THEN a."PlannedQty"-a."IssuedQty" else a."BaseQty" * :ParentQty END, a."wareHouse", a."wareHouse", a."Project", a."OcrCode", a."OcrCode2", a."OcrCode3", a."OcrCode4", a."OcrCode5"
		, CASE WHEN b."ManBtchNum" = 'Y' THEN 'B' WHEN b."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", IFNULL(b."OnHand", 0) AS "OnHandItem", IFNULL(b."IsCommited", 0) AS "IsCommittedItem"
		, IFNULL(b."OnOrder", 0) AS "OnOrderedItem", IFNULL(c."OnHand", 0) AS "OnHandWhs", IFNULL(c."IsCommited", 0) AS "IsCommittedWhs", IFNULL(c."OnOrder", 0) AS "OnOrderedWhs", b."SalUnitMsr" AS "SalUnitMsr"
		, IFNULL(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 0) AS "NumInBuy", IFNULL(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr"
		, IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) AS "NumInInven"
		, CASE WHEN IFNULL(a."OcrCode", '') <> '' AND IFNULL(a."OcrCode2", '') <> '' AND IFNULL(a."OcrCode3", '') <> '' AND IFNULL(a."OcrCode4", '') <> '' AND IFNULL(a."OcrCode5", '') <> '' 
		THEN NULL ELSE IFNULL(a."OcrCode", '') || ';' || IFNULL(a."OcrCode2", '') || ';' || IFNULL(a."OcrCode3", '') || ';' || IFNULL(a."OcrCode4", '') || ';' || IFNULL(a."OcrCode5", '') END AS "Rules"
		, c."AvgPrice", T1."Project"
	FROM WOR1 a INNER JOIN OWOR T1 ON a."DocEntry" = T1."DocEntry" INNER JOIN OITM b ON a."ItemCode" = b."ItemCode" LEFT OUTER JOIN OITW c ON a."ItemCode" = c."ItemCode" AND a."wareHouse" = c."WhsCode" 
		LEFT OUTER JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry"=b."IUoMEntry" WHERE a."ItemType" = 4 AND b."InvntItem" = 'Y' AND T1."DocEntry" = :docEntry AND T1."Status" in ('R', 'P') AND a."PlannedQty" > 0; 
	-------------------------------------------------------- End lấy dữ liệu line ----------------------------------------------------------------------------

	---------------------------------------- Update các thông tin liên quan đến đơn vị tính ------------------------------------------------------
	update #ret set "InvQty" = "Quantity", "unitMsr" = "InvUnitMsr", "UseBaseUn" = 'Y', "NumPerMsr" = 1;

	--------------------- Select dữ liệu lines sắp xếp theo baseentry và base line -------------------
	SELECT * FROM #ret ORDER BY "BaseEntry","BaseLine";
	
	DROP TABLE #ret;
	
	----------------------------------------------------------- Cấu trúc table batch/serial ----------------------------------------------------------------------------------------------------------
	SELECT TOP 0 T0.*, CAST(0 AS integer) AS "BinAbs", CAST('' AS nvarchar(228)) AS "BinCode", CAST(0 AS decimal(19, 6)) AS "SelectedQty", CAST('' AS nvarchar(8)) AS "WhsCode", CAST('' AS nvarchar(228)) AS "BinDescription"
		, CAST(0 AS integer) AS "BaseLineNumber", CAST(0 AS integer) AS "BaseDocEntry", 'B' AS "ManBatchSerial", CAST(0 AS decimal(19, 6)) AS "LogQty", 'Y' AS "IsExist", 1 AS "TranType", 'N' AS "IsBase", 'N' AS "IsExits" 
	FROM OBTN T0;

	call apz_fun_document_getBinAllowcationLine_byDocEntry(-1, '202', bin);
	select * from :bin a left join OBTN b on 1 = 0;
end