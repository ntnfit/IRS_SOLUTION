
CREATE PROCEDURE APZ_InventoryTransaction_GoodIssueIT_GetItemsProdOrder (
	IN 	docentry int
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	SELECT IFNULL(a."DocEntry", 0) AS "DocEntry", IFNULL(a."Series", 0) AS "Series", 0 AS "LineNum", IFNULL(b."ItemType", '') AS "ItemType", IFNULL(a."ItemCode", '') AS "ItemCode", b."ItemName" AS "Dscription", 0 AS "Quantity"
			, IFNULL(a."Warehouse", '') AS "WhsCode", IFNULL(c."OnHand", 0) AS "OnHand", IFNULL(b."InvntryUom", '') AS "UomCode"
			, (SELECT SUM(IFNULL("OnHand", 0)) FROM OITW WHERE "ItemCode" = b."ItemCode") AS "QuantityOnStock"
			, (SELECT SUM(IFNULL("OnHand", 0)) FROM OITW WHERE "ItemCode" = b."ItemCode" AND "WhsCode" = b."DfltWH") AS "QuantityOnWareHouse" 
		FROM OWOR a 
		LEFT OUTER JOIN OITM b ON a."ItemCode" = b."ItemCode" 
		LEFT OUTER JOIN OITW c ON (c."ItemCode" = a."ItemCode" AND c."WhsCode" = a."Warehouse") 
		WHERE a."DocEntry" = :docentry;
END;




