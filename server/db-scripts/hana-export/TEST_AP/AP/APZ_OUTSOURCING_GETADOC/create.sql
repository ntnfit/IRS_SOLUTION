CREATE PROCEDURE APZ_Outsourcing_GetADoc
(
	in DocNum int default 0,
	in grid nvarchar(1) default ''
)
AS
BEGIN
	IF :grid = 'I' --issue
	THEN 
		SELECT a."LineNum", a."ItemCode", b."ItemName", a."WhsCode", a."Quantity", a."DocEntry",d."Ref2" ,d."Comments",d."JrnlMemo", a."U_Type",
			CASE b."ManBtchNum" WHEN 'Y' THEN 'B' ELSE (CASE b."ManSerNum" WHEN 'Y' THEN 'S' ELSE 'N' END) END AS "ManBatchSer", 'Goods Issue' AS "DocType" 
		FROM IGE1 a INNER JOIN OITM b ON a."ItemCode" = b."ItemCode" 
				INNER JOIN "@TBL_OUTS_DOCNUM" c ON c."U_issueID" = a."DocEntry"
				INNER JOIN OIGE d ON d."DocEntry" = a."DocEntry" 
		WHERE c."Code" = :DocNum;
	ELSE IF :grid='R' then -- receipt
		SELECT a."LineNum", a."ItemCode", b."ItemName", a."WhsCode", a."Quantity", a."DocEntry", d."Ref2" ,d."Comments",d."JrnlMemo", a."U_Type", -- , "U_OutAmtFactor" AS "AmountFactor"
			CASE b."ManBtchNum" WHEN 'Y' THEN 'B' ELSE (CASE b."ManSerNum" WHEN 'Y' THEN 'S' ELSE 'N' END) END AS "ManBatchSer", 'Goods Receipt' AS "DocType" 
		FROM IGN1 a INNER JOIN OITM b ON a."ItemCode" = b."ItemCode"
					INNER JOIN "@TBL_OUTS_DOCNUM" c ON c."U_receiptID" = a."DocEntry"
					INNER JOIN OIGN d ON d."DocEntry" = a."DocEntry" 
		WHERE c."Code" = :DocNum;
	END IF;
	END IF;
END

