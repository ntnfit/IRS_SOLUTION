
CREATE PROCEDURE APZ_PriceList_GetPriceForListItem(
	in priceList int default -1)
AS
BEGIN
	
	create local temporary table #tempItemData ("ItemCode" nvarchar(50) null);
	exec 'drop table #tempItemData;';
	exec 'create local temporary table #tempItemData as (select * from "#tempItem");';
	SELECT a."ItemCode", round(a."Price") "Price", a."Currency", b."Rate" FROM (SELECT "ItemCode", "Price", "Currency" FROM ITM1 WHERE :priceList >= 0 AND "PriceList" = :priceList 
																UNION ALL 
															SELECT "ItemCode", "LastPurPrc", "LastPurCur" FROM OITM WHERE :priceList = -1 
																UNION ALL 
															SELECT "ItemCode", "LstEvlPric", '' FROM OITM WHERE :priceList = -2) AS a 
														INNER JOIN #tempItemData c ON a."ItemCode" = c."ItemCode" 
														LEFT OUTER JOIN (SELECT "Rate", "Currency" FROM ORTT WHERE CAST("RateDate" AS date) = CAST(CURRENT_DATE AS date)) AS b ON a."Currency" = b."Currency";

END;

