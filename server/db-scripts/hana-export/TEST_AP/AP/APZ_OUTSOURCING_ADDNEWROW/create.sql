CREATE PROCEDURE APZ_Outsourcing_AddNewRow(in grid char(1))
AS
BEGIN
	IF :grid='I' THEN -- issue
		SELECT top 0 
			0 "LineNum"
			,a."ItemCode"
			,b."ItemName"
			,a."WhsCode"
			,a."Quantity"
			,a."DocEntry"
			,'N' "ManBatchSer"
			,'' "Father"
			,0 "ChildNum",'' "OcrCode",'' "OcrCode2",'' "OcrCode3",'' "OcrCode4",'' "OcrCode5",'' "Project"
			,c."Ref2" ,c."Comments",c."JrnlMemo", a."U_Type"
		FROM IGE1 a 
			INNER JOIN OITM b on a."ItemCode"=b."ItemCode"
			INNER JOIN OIGE c on a."DocEntry"=c."DocEntry";
		
	ELSE IF :grid='R' THEN -- receipt
		SELECT top 0 
			0 "LineNum"
			,a."ItemCode"
			,b."ItemName"
			,a."WhsCode"
			,a."Quantity"
			,1 "AmountFactor"
			,a."DocEntry"
			,'N' "ManBatchSer",'' "OcrCode",'' "OcrCode2",'' "OcrCode3",'' "OcrCode4",'' "OcrCode5",'' "Project"
			,c."Ref2" ,c."Comments",c."JrnlMemo",a."U_Type"
		FROM IGN1 a 
			INNER JOIN OITM b on a."ItemCode"=b."ItemCode"
			INNER JOIN OIGN c on a."DocEntry"=c."DocEntry";
		END IF;
	
	END IF;
END

