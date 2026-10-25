

CREATE PROCEDURE APZ_D_UpdateInventory_SearchData
(
	cardCode nvarchar(15) DEFAULT '',
	searchKey nvarchar(100) DEFAULT ''
)
AS
BEGIN
	declare inventoryType nvarchar(5) = 'O';
	
	select (select FIRST_VALUE("WarehouseInventoryType" order by "WarehouseInventoryType") from APZ_D_OAPC) into inventoryType from DUMMY;
	if(:inventoryType = 'O') then
		SELECT DISTINCT T0."ItemCode", T2."ItemName" FROM RDR1 T0 INNER JOIN ORDR T1 ON T0."DocEntry" = T1."DocEntry" INNER JOIN OITM T2 ON T0."ItemCode" = T2."ItemCode" 
		WHERE (T1."CardCode" = :cardCode OR :cardCode = '') AND ((T0."ItemCode" LIKE N'%' || :searchKey || '%') OR (T2."ItemName" LIKE N'%' || :searchKey || '%'))
		AND (
	    (t2."validFor" = 'Y' and ifnull(t2."frozenFor",'N') = 'N' and (ifnull(t2."validFrom", current_date) <= current_date and ifnull(t2."validTo", current_date) >= current_date)) 
		OR (ifnull(t2."validFor",'N') = 'N' and t2."frozenFor" = 'Y' and (ifnull(t2."frozenFrom", current_date) > current_date or ifnull(t2."frozenTo", current_date) < current_date))
			or (t2."validFor" = 'Y' and t2."frozenFor" = 'Y' and 
		        (ifnull(t2."validFrom", current_date) <= current_date and ifnull(t2."validTo", current_date) >= current_date) 
		        and (ifnull(t2."frozenFrom", current_date) > current_date or ifnull(t2."frozenTo", current_date) < current_date))
			OR (ifnull(t2."validFor",'N') = 'N' AND ifnull(t2."frozenFor",'N') = 'N'))
			order by T0."ItemCode";
	elseif(:inventoryType = 'A') then
		SELECT DISTINCT T0."ItemCode", T2."ItemName" FROM RDR1 T0 INNER JOIN ORDR T1 ON T0."DocEntry" = T1."DocEntry" INNER JOIN OITM T2 ON T0."ItemCode" = T2."ItemCode" 
		WHERE ((T0."ItemCode" LIKE N'%' || :searchKey || '%') OR (T2."ItemName" LIKE N'%' || :searchKey || '%'))
		AND (
	    (t2."validFor" = 'Y' and ifnull(t2."frozenFor",'N') = 'N' and (ifnull(t2."validFrom", current_date) <= current_date and ifnull(t2."validTo", current_date) >= current_date)) 
		OR (ifnull(t2."validFor",'N') = 'N' and t2."frozenFor" = 'Y' and (ifnull(t2."frozenFrom", current_date) > current_date or ifnull(t2."frozenTo", current_date) < current_date))
			or (t2."validFor" = 'Y' and t2."frozenFor" = 'Y' and 
		        (ifnull(t2."validFrom", current_date) <= current_date and ifnull(t2."validTo", current_date) >= current_date) 
		        and (ifnull(t2."frozenFrom", current_date) > current_date or ifnull(t2."frozenTo", current_date) < current_date))
			OR (ifnull(t2."validFor",'N') = 'N' AND ifnull(t2."frozenFor",'N') = 'N'))
			order by T0."ItemCode";
	else
		select t0."ItmsGrpCod" as "ItemCode",t0."ItmsGrpNam" as "ItemName" from OITB t0 where (T0."ItmsGrpCod" LIKE N'%' || :searchKey || '%') OR (T0."ItmsGrpNam" LIKE N'%' || :searchKey || '%') order by T0."ItmsGrpCod";
	end if;
END


