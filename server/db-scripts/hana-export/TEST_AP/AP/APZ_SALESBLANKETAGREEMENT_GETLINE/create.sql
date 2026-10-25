
CREATE PROCEDURE APZ_SalesBlanketAgreement_GetLine (
	IN 	AgrNo INT
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	SELECT OAT1.*, OITM."InvntryUom", OITM."UgpEntry", OITM."SalUnitMsr" AS "SalUnitMsr", IFNULL(OITM."NumInSale", 0) AS "NumInSale", OITM."BuyUnitMsr", IFNULL(OITM."NumInBuy", 0) AS "NumInBuy", IFNULL(OITM."UgpEntry", -1) AS "UgpCode", OITM."InvntryUom" AS "InvUnitMsr", IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) AS "NumInInven", CAST(1 AS NUMERIC(19, 6)) AS "Rate", 
		cast(-1 as bigint) as "OpenAmntLC", cast(-1 as bigint) as "OpenAmntFC"
		FROM OAT1, OITM 
		LEFT OUTER JOIN UGP1 d ON OITM."UgpEntry" = d."UgpEntry" AND d."UomEntry" IN (SELECT "UomEntry" FROM OUOM WHERE "UomName" = OITM."InvntryUom") 
		WHERE OAT1."AgrNo" = :AgrNo 
			AND OAT1."ItemCode" = OITM."ItemCode"
		ORDER BY OAT1."VisOrder";
END;




