
CALL APZ_EXISTSPROC('Apz_ProductionOrder_GetIssueComponents');

GO

CREATE PROCEDURE Apz_ProductionOrder_GetIssueComponents(in docEntry INT default 0)
AS
begin
	----------------------------------------------------------- Lấy cấu trúc của table line ------------------------------------------------------------------
	create local temporary table #ret as (
	SELECT TOP 0 a.*, '' AS "ManBtchSerial", CAST(NULL AS decimal(19, 6)) AS "QuantityOnStock", CAST(NULL AS decimal(19, 6)) AS "QuantityOnWareHouse", CAST(NULL AS nvarchar(100)) AS "SalUnitMsr"
		, CAST(NULL AS decimal(19, 6)) AS "NumInSale", CAST(NULL AS nvarchar(100)) AS "BuyUnitMsr", CAST(NULL AS decimal(19, 6)) AS "NumInBuy", -1 AS "UgpCode", CAST(NULL AS nvarchar(100)) AS "InvUnitMsr", 1 AS "NumInInven"
		, CAST(NULL AS decimal(19, 6)) AS "AvgPrice" FROM (SELECT -1 AS "DocEntry" FROM DUMMY) AS b LEFT OUTER JOIN IGN1 A ON 1 = 2 WHERE A."DocEntry" = 0);

	----------------------------------------------------------- Insert dữ liệu copy vào table line ----------------------------------------------------------
	INSERT INTO #ret ("ItemType", "DocEntry", "LineNum", "BaseRef", "BaseEntry", "BaseLine", "BaseType", "ItemCode", "Dscription", "Quantity", "ManBtchSerial", "QuantityOnStock", "QuantityOnWareHouse", "SalUnitMsr", "NumInSale"
						, "BuyUnitMsr", "NumInBuy", "UgpCode", "InvUnitMsr", "NumInInven", "WhsCode", "Project", "OcrCode", "OcrCode2", "OcrCode3", "OcrCode4", "OcrCode5", "AvgPrice") 
		SELECT a."ItemType", 0, ROW_NUMBER() OVER ( ORDER BY t1."DocNum"), T1."DocNum", T1."DocEntry", a."LineNum", '202', a."ItemCode"
			, CASE WHEN a."ItemType" = 290 THEN (SELECT "ResName" FROM ORSC WHERE "ResCode" = a."ItemCode") ELSE b."ItemName" END AS "ItemName"
			, CASE WHEN a."PlannedQty" - a."IssuedQty" <= 0 THEN a."BaseQty" ELSE a."PlannedQty" - a."IssuedQty" END
			, CASE WHEN IFNULL(b."ManBtchNum", '') = 'Y' THEN 'B' WHEN IFNULL(b."ManSerNum", '') = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", b."OnHand" AS "QuantityOnStock"
			, c."OnHand" AS "QuantityOnWareHouse", b."SalUnitMsr" AS "SalUnitMsr", IFNULL(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 0) AS "NumInBuy", IFNULL(b."UgpEntry", -1) AS "UgpCode"
			, b."InvntryUom" AS "InvUnitMsr", IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) AS "NumInInven", a."wareHouse", a."Project", a."OcrCode", a."OcrCode2", a."OcrCode3", a."OcrCode4", a."OcrCode5", c."AvgPrice" 
		FROM WOR1 a INNER JOIN OWOR T1 ON a."DocEntry" = T1."DocEntry" LEFT OUTER JOIN OITM b ON a."ItemCode" = b."ItemCode" LEFT OUTER JOIN OITW c ON a."ItemCode" = c."ItemCode" AND a."wareHouse" = c."WhsCode" 
					LEFT OUTER JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry"=b."IUoMEntry"
					WHERE T1."DocEntry" = :docEntry AND a."IssueType" = 'M' AND a."PlannedQty" > 0 AND IFNULL(a."IssuedQty", 0) < a."PlannedQty" AND T1."Status" = 'R';

	-------------------------------------------------------- End lấy dữ liệu line ----------------------------------------------------------------------------

	---------------------------------------- Update các thông tin liên quan đến đơn vị tính ------------------------------------------------------
	update #ret set "InvQty" = "Quantity", "unitMsr" = "InvUnitMsr", "UseBaseUn" = 'Y', "NumPerMsr" = 1;

	--------------------- Select dữ liệu lines sắp xếp theo baseentry và base line -------------------
	SELECT * FROM #ret ORDER BY "BaseEntry","BaseLine";

	----------------------------------------------------------- Cấu trúc table batch/serial ----------------------------------------------------------------------------------------------------------
	create local temporary table #TempBatchSerialBase as (
	SELECT TOP 0 T0.*, CAST(0 AS integer) AS "BinAbs", CAST('' AS nvarchar(228)) AS "BinCode", CAST(0 AS decimal(19, 6)) AS "SelectedQty", CAST('' AS nvarchar(8)) AS "WhsCode", CAST('' AS nvarchar(228)) AS "BinDescription"
		, CAST(0 AS integer) AS "BaseLineNumber", CAST(0 AS integer) AS "BaseDocEntry", 'B' AS "ManBatchSerial", CAST(0 AS decimal(19, 6)) AS "LogQty", 'Y' AS "IsExist", 1 AS "TranType", 'N' AS "IsBase" FROM OBTN T0);

	----------------------------------------------------------- Insert dữ liệu vào table batch/serial -------------------------------------------------------------------------------------------------
	
	call apz_fun_getbatchserial_byDocEntry(:docentry, '202', '#TempBatchSerialBase');

	DELETE FROM #TempBatchSerialBase a where not exists(select 1 from #ret b where a."BaseDocEntry" = b."BaseEntry" and a."BaseLineNumber" = b."BaseLine");
	Update a set "BaseLineNumber" = b."LineNum" from #TempBatchSerialBase a INNER JOIN #ret b ON a."BaseDocEntry" = b."BaseEntry" and a."BaseLineNumber" = b."BaseLine";

	delete from #TempBatchSerialBase a where not exists(select 1 from #ret where "LineNum" = a."BaseLineNumber");
	select *, 'N' "IsExits" from #TempBatchSerialBase;
end;