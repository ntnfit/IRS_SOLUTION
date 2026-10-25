
create procedure Apz_ProductionOrder_GetListStandardOrders
As
Begin
SELECT IFNULL(a."DocEntry", -1) AS "DocEntry",
 IFNULL(a."Series", '') AS "Series",
  IFNULL("Type", '') AS "Type", 
  IFNULL("DueDate", '') AS "DueDate",
   IFNULL(a."ItemCode", '') AS "ItemCode", 
   IFNULL("ItemName", 'N') AS "ItemName",
    IFNULL(a."Comments", 3) AS "Comments" 
    FROM owor a LEFT OUTER JOIN oitm b ON a."ItemCode" = b."ItemCode" WHERE a."Type" = 'S' AND a."Status" = 'R';

End;





