
CREATE PROCEDURE APZ_BarcodeInventoryCounting_GetDataLine(in docEntry int default null)
AS
BEGIN
	
	SELECT a.*, IFNULL(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr", IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) AS "NumInInven" 
		FROM APZ_BIC1 a LEFT OUTER JOIN OITM b ON a."ItemCode" = b."ItemCode" 
		LEFT OUTER JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry" IN (SELECT "UomEntry" FROM OUOM WHERE "UomName" = b."InvntryUom") WHERE A."DocEntry" = :docEntry ORDER BY "VisOrder";
	
END;



