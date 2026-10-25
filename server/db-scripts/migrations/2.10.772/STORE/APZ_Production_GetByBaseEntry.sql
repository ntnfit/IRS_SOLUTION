
CALL APZ_EXISTSPROC('APZ_Production_GetByBaseEntry');

GO

CREATE PROCEDURE APZ_Production_GetByBaseEntry(in toType int, docEntry int)
AS
BEGIN
	if :toType = 59 then
		SELECT T0."ItemCode", T0."DocNum", -1 AS "BaseLine", T0."PlannedQty", T0."CmpltQty", T0."Warehouse", b."ItemName", IFNULL(T2."BPLId", -1) AS "BplId", b."UgpEntry", b."InvntryUom", 
            CASE WHEN b."ManBtchNum" = 'Y' THEN 'B' WHEN b."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", b."OnHand" AS "QuantityOnStock", c."OnHand" AS "QuantityOnWareHouse", 
            b."SalUnitMsr" AS "SalUnitMsr", IFNULL(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 0) AS "NumInBuy", IFNULL(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr", 
            IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) AS "NumInInven", T0."Warehouse" AS "WhsCode", T0."Project", T0."OcrCode", T0."OcrCode2", T0."OcrCode3", T0."OcrCode4", T0."OcrCode5",
            c."AvgPrice",'N' "IsByPrdct"
        FROM OWOR T0 
        INNER JOIN OITM b ON T0."ItemCode" = b."ItemCode" 
        INNER JOIN OITW c ON T0."ItemCode" = c."ItemCode" AND T0."Warehouse" = c."WhsCode" 
        LEFT JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry"=b."IUoMEntry"
        LEFT JOIN NNM1 T2 ON T0."Series" = T2."Series" AND T2."ObjectCode" = '202'
        WHERE T0."DocEntry"= :docEntry AND "Type" IN ('S','P')
        UNION 
    	SELECT T0."ItemCode", T1."DocNum", T0."LineNum", -T0."PlannedQty", -T0."IssuedQty", T0."wareHouse", b."ItemName", -1, b."UgpEntry", b."InvntryUom", 
            CASE WHEN b."ManBtchNum" = 'Y' THEN 'B' WHEN b."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", b."OnHand" AS "QuantityOnStock", c."OnHand" AS "QuantityOnWareHouse", 
            b."SalUnitMsr" AS "SalUnitMsr", IFNULL(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 0) AS "NumInBuy", IFNULL(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr", 
            IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) AS "NumInInven", T0."wareHouse" AS "WhsCode", T0."Project", T0."OcrCode", T0."OcrCode2", T0."OcrCode3", T0."OcrCode4", T0."OcrCode5",
            c."AvgPrice",case when T1."Type"<>'D' then 'Y' else 'N' end
        FROM WOR1 T0 
        INNER JOIN OWOR T1 ON T0."DocEntry" = T1."DocEntry" 
        INNER JOIN OITM b ON T0."ItemCode" = b."ItemCode" 
        INNER JOIN OITW c ON T0."ItemCode" = c."ItemCode" AND T0."wareHouse" = c."WhsCode" 
        LEFT JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry"=b."IUoMEntry"
        WHERE T1."DocEntry" = :docEntry AND T0."IssueType" = 'M'  AND (T1."Type" ='D' OR T0."PlannedQty" < 0);
        
	elseif :toType = 60 then
		SELECT T0."ItemCode", T0."DocNum", -1 AS "BaseLine", T0."PlannedQty", T0."CmpltQty", T0."Warehouse", b."ItemName", IFNULL(T2."BPLId", -1) AS "BplId", b."UgpEntry", b."InvntryUom", 
            CASE WHEN b."ManBtchNum" = 'Y' THEN 'B' WHEN b."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", b."OnHand" AS "QuantityOnStock", c."OnHand" AS "QuantityOnWareHouse", 
            b."SalUnitMsr" AS "SalUnitMsr", IFNULL(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 0) AS "NumInBuy", IFNULL(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr", 
            IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) AS "NumInInven", T0."Warehouse" AS "WhsCode", T0."Project", T0."OcrCode", T0."OcrCode2", T0."OcrCode3", T0."OcrCode4", T0."OcrCode5",
            c."AvgPrice"
        FROM OWOR T0 
        INNER JOIN OITM b ON T0."ItemCode" = b."ItemCode" 
        INNER JOIN OITW c ON T0."ItemCode" = c."ItemCode" AND T0."Warehouse" = c."WhsCode" 
        LEFT JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry"=b."IUoMEntry"
        LEFT JOIN NNM1 T2 ON T0."Series" = T2."Series" AND T2."ObjectCode" = '202'
        WHERE T0."DocEntry"= :docEntry AND "Type" ='D'
        UNION
    	SELECT T0."ItemCode", T1."DocNum", T0."LineNum", -T0."PlannedQty", -T0."IssuedQty", T0."wareHouse", b."ItemName", -1, b."UgpEntry", b."InvntryUom", 
            CASE WHEN b."ManBtchNum" = 'Y' THEN 'B' WHEN b."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", b."OnHand" AS "QuantityOnStock", c."OnHand" AS "QuantityOnWareHouse", 
            b."SalUnitMsr" AS "SalUnitMsr", IFNULL(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", IFNULL(b."NumInBuy", 0) AS "NumInBuy", IFNULL(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr", 
            IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) AS "NumInInven", T0."wareHouse" AS "WhsCode", T0."Project", T0."OcrCode", T0."OcrCode2", T0."OcrCode3", T0."OcrCode4", T0."OcrCode5",
            c."AvgPrice"
        FROM WOR1 T0 
        INNER JOIN OWOR T1 ON T0."DocEntry" = T1."DocEntry" 
        INNER JOIN OITM b ON T0."ItemCode" = b."ItemCode" 
        INNER JOIN OITW c ON T0."ItemCode" = c."ItemCode" AND T0."wareHouse" = c."WhsCode" 
        LEFT JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry"=b."IUoMEntry"
        WHERE T1."DocEntry" = :docEntry AND T0."IssueType" = 'M'  AND (T1."Type" IN('S','P') AND T0."PlannedQty" > 0);
	end if;
END