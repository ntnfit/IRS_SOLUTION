
CREATE PROCEDURE APZ_Schedule_GetBarCodeManual(in BarCode nvarchar(50))
AS
BEGIN
	declare objCode nvarchar(20) = '';
	select ifnull((select "ObjType"  from apz_POBarCode where "Barcode" = :BarCode),'') into objCode from DUMMY;
	if :objCode = '202'
	THEN

		SELECT a."Barcode", a."ProductNo" AS "ItemCode", a."ProductDesc" AS "ItemName", a.ESEQ AS "Order", a."TotalSEQ" AS "TotalLine", IFNULL(a."PackageQTY", 0) AS "PackQty", IFNULL(a."Qty", 0) AS "BarQty"
			, IFNULL(a."CompleteQTY", 0) AS "CmpQty", a."DueDate", a."TargetWhse" AS "WhsCode", IFNULL(a."Qty", 0) - IFNULL(a."CompleteQTY", 0) AS "Quantity", a."PONo" AS "DocEntry", t1."Series"
			, IFNULL(c."BPLId", -1) AS "BPLId", a."ObjType", b."DocNum" AS "PONum", IFNULL((SELECT SUM(IFNULL(a1."AvgPrice", 0) * IFNULL(a2."PlannedQty", 0)) 
		FROM OITW a1 INNER JOIN WOR1 a2 ON a1."ItemCode" = a2."ItemCode" AND a1."WhsCode" = a2."wareHouse" WHERE a2."DocEntry" = b."DocEntry"), 0) / b."PlannedQty" AS "Price" FROM apz_POBarCode a 
					 INNER JOIN OWOR b ON a."PONo" = b."DocEntry" INNER JOIN NNM1 c ON b."Series" = c."Series" AND c."ObjectCode" = '202' 
					 INNER JOIN NNM1 t1 ON IFNULL(t1."BPLId", -1) = IFNULL(c."BPLId", -1) AND t1."ObjectCode" = '59' WHERE a."Barcode" = :BarCode AND b."Status" = 'R';-- and isnull(a.Qty,0) - isnull(a.CompleteQTY,0) > 0

	elseif :objCode = '22'
	THEN

		SELECT a."Barcode", a."ProductNo" AS "ItemCode", a."ProductDesc" AS "ItemName", a.ESEQ AS "Order", a."TotalSEQ" AS "TotalLine", IFNULL(a."PackageQTY", 0) AS "PackQty", IFNULL(a."Qty", 0) AS "BarQty"
			, IFNULL(a."CompleteQTY", 0) AS "CmpQty", a."DueDate", a."TargetWhse" AS "WhsCode", IFNULL(a."Qty", 0) - IFNULL(a."CompleteQTY", 0) AS "Quantity", a."PONo" AS "DocEntry", b."Series"
			, IFNULL(c."BPLId", -1) AS "BPLId", a."ObjType", b."DocNum" AS "PONum" 
		FROM apz_POBarCode a INNER JOIN OPOR b ON a."PONo" = b."DocEntry" INNER JOIN NNM1 c ON b."Series" = c."Series" AND c."ObjectCode" = '22' 
			WHERE a."Barcode" = :BarCode;-- and isnull(a.Qty,0) - isnull(a.CompleteQTY,0) > 0
	end IF;
	
END



