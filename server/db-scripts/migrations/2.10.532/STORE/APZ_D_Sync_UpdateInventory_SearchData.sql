
CALL APZ_EXISTSPROC('APZ_D_Sync_UpdateInventory_SearchData');

GO


CREATE PROCEDURE APZ_D_Sync_UpdateInventory_SearchData()
AS
BEGIN
	declare inventoryType nvarchar(5) = 'O';
	--Create temp table #tempLocal
	create local temporary table #tempLocal as (select '' as "CardCode" from dummy);
	exec 'drop table #tempLocal;';
	exec 'create local temporary table #tempLocal as (select * from "#temp");';
	
	select (select FIRST_VALUE("WarehouseInventoryType" order by "WarehouseInventoryType") from APZ_D_OAPC) into inventoryType from DUMMY;
	if(:inventoryType = 'O') then
		SELECT Cast(T0."ItemCode" as nvarchar) "ItemCode", T2."ItemName",T1."CardCode" FROM RDR1 T0 INNER JOIN ORDR T1 ON T0."DocEntry" = T1."DocEntry" INNER JOIN OITM T2 ON T0."ItemCode" = T2."ItemCode" 
		WHERE (T1."CardCode" in (select "CardCode" from #tempLocal) OR not exists (select "CardCode" from #tempLocal))
		AND (
	    (t2."validFor" = 'Y' and ifnull(t2."frozenFor",'N') = 'N' and (ifnull(t2."validFrom", current_date) <= current_date and ifnull(t2."validTo", current_date) >= current_date)) 
		OR (ifnull(t2."validFor",'N') = 'N' and t2."frozenFor" = 'Y' and (ifnull(t2."frozenFrom", current_date) > current_date or ifnull(t2."frozenTo", current_date) < current_date))
			or (t2."validFor" = 'Y' and t2."frozenFor" = 'Y' and 
		        (ifnull(t2."validFrom", current_date) <= current_date and ifnull(t2."validTo", current_date) >= current_date) 
		        and (ifnull(t2."frozenFrom", current_date) > current_date or ifnull(t2."frozenTo", current_date) < current_date))
			OR (ifnull(t2."validFor",'N') = 'N' AND ifnull(t2."frozenFor",'N') = 'N'));
	elseif(:inventoryType = 'A') then
		SELECT DISTINCT cast(T0."ItemCode" as nvarchar) "ItemCode", T2."ItemName" FROM RDR1 T0 INNER JOIN ORDR T1 ON T0."DocEntry" = T1."DocEntry" INNER JOIN OITM T2 ON T0."ItemCode" = T2."ItemCode" 
		WHERE (
	    (t2."validFor" = 'Y' and ifnull(t2."frozenFor",'N') = 'N' and (ifnull(t2."validFrom", current_date) <= current_date and ifnull(t2."validTo", current_date) >= current_date)) 
		OR (ifnull(t2."validFor",'N') = 'N' and t2."frozenFor" = 'Y' and (ifnull(t2."frozenFrom", current_date) > current_date or ifnull(t2."frozenTo", current_date) < current_date))
			or (t2."validFor" = 'Y' and t2."frozenFor" = 'Y' and 
		        (ifnull(t2."validFrom", current_date) <= current_date and ifnull(t2."validTo", current_date) >= current_date) 
		        and (ifnull(t2."frozenFrom", current_date) > current_date or ifnull(t2."frozenTo", current_date) < current_date))
			OR (ifnull(t2."validFor",'N') = 'N' AND ifnull(t2."frozenFor",'N') = 'N'));
	else
		select cast(t0."ItmsGrpCod" as nvarchar) as "ItemCode",t0."ItmsGrpNam" as "ItemName" from OITB t0;
	end if;
END