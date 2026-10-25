
create procedure Apz_ProductionOrder_GetListItemProductionOrders(in tranType nvarchar(2))
As
Begin
SELECT IFNULL(a."DocEntry", -1) AS "DocEntry", 
IFNULL(a."Series", '') AS "Series", IFNULL("Type", '') AS "Type",
 IFNULL(a."PlannedQty", 0) - IFNULL(a."CmpltQty", 0) AS "PlaQty",
  IFNULL("DueDate", '') AS "DueDate", IFNULL(a."ItemCode", '') AS "ItemCode", 
  IFNULL("ItemName", 'N') AS "ItemName", IFNULL(a."Comments", 3) AS "Comments",
   IFNULL(a."Warehouse", '') AS "DefaultWarehouse", IFNULL(b."InvntryUom", '') AS "UoMCode", (SELECT SUM(IFNULL("OnHand", 0))
    FROM oitw WHERE "ItemCode" = b."ItemCode") AS "QuantityOnStock", (SELECT SUM(IFNULL("OnHand", 0)) 
    FROM oitw WHERE "ItemCode" = b."ItemCode" AND "WhsCode" = b."DfltWH") AS "QuantityOnWareHouse" 
    FROM owor a LEFT OUTER JOIN oitm b ON a."ItemCode" = b."ItemCode" 
    WHERE (:tranType = 'DI' AND a."Type" = 'D' AND a."Status" = 'R' AND IFNULL(a."CmpltQty", 0) < IFNULL(a."PlannedQty", 0)) 
    OR (:tranType = 'SI' AND (a."Type" = 'S' AND a."Status" = 'R' AND a."DocEntry" IN (SELECT "DocEntry" FROM wor1
    WHERE "IssueType" = 'M' AND "IssuedQty" < "PlannedQty") OR a."Type" = 'D' AND a."Status" = 'R' 
    AND a."DocEntry" IN (SELECT "DocEntry" FROM wor1 WHERE "IssueType" = 'M' AND "IssuedQty" >= "PlannedQty")));

End;




