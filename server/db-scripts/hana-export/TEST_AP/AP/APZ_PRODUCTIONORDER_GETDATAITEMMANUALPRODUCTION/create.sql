
create procedure Apz_ProductionOrder_GetDataItemManualProduction(in docEntry nvarchar(5000))
As
Begin
--declare sqlQuery nvarchar(5000) = '';
--select '
SELECT IFNULL(a."DocEntry", -1) AS "DocEntry", 
IFNULL(a."IssueType", '') AS "Type",
 abs(IFNULL(a."PlannedQty", 0) - IFNULL(a."IssuedQty", 0)) AS "PlaQty", 
 IFNULL(a."ItemCode", '') AS "ItemCode",
  IFNULL(b."ItemName", '') AS "ItemName", 
  IFNULL(a."wareHouse", '') AS "DefaultWarehouse", 
  IFNULL(b."InvntryUom", '') AS "UoMCode",
(select sum(IFNULL("OnHand",0)) from oitw where "ItemCode" = b."ItemCode") as "QuantityOnStock",
(select sum(IFNULL("OnHand",0)) from oitw where "ItemCode" = b."ItemCode" and "WhsCode" = b."DfltWH") as "QuantityOnWareHouse"
   FROM wor1 a left join oitm b on a."ItemCode"=b."ItemCode"
   where a."DocEntry" in (select Data from split(:docEntry,',')) and a."IssueType" = 'M' and 
   (((select "Type" from owor where "DocEntry" = a."DocEntry") = 'S' and ifnull(a."IssuedQty",0) < IFnull(a."PlannedQty",0)) or 
   ((select "Type" from owor where "DocEntry" = a."DocEntry") = 'D' and ifnull(a."IssuedQty",0) >= IFnull(a."PlannedQty",0)));
   --' into sqlQuery from dummy;
 
--execute immediate ':sqlQuery';
End;





