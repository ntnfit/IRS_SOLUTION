
CREATE PROCEDURE APZ_ItemMaster_ItemMaterInventory (IN UserCode nvarchar(50), IN BPLId integer) 
AS BEGIN 
SELECT DISTINCT IFNULL(a."ManBtchNum", '') AS "ManBtchNum", IFNULL(a."ItemCode", '') AS "ItemCode", IFNULL(a."ItemName", '') AS "ItemName", IFNULL(a."DfltWH", '') AS "DefaultWarehouse", IFNULL(a."VatGourpSa", '') AS "SalesVATGroup", IFNULL(g."Rate", 0) AS "RateSa", (SELECT SUM(IFNULL("OnHand", 0)) FROM oitw WHERE "ItemCode" = a."ItemCode" AND "Locked" = 'N') AS "QuantityOnStock", IFNULL((SELECT SUM(IFNULL("OnHand", 0)) FROM oitw WHERE "ItemCode" = a."ItemCode" AND "Locked" = 'N' AND "WhsCode" = a."DfltWH"), 0) AS "QuantityOnWareHouse"
 FROM oitm a LEFT OUTER JOIN itm1 b ON a."ItemCode" = b."ItemCode" 
 				LEFT OUTER JOIN OVTG g ON a."VatGourpSa" = g."Code" 
 				LEFT OUTER JOIN OVTG k ON a."VatGroupPu" = k."Code" WHERE a."validFor" = 'Y' AND "InvntItem" = 'Y';
END;




