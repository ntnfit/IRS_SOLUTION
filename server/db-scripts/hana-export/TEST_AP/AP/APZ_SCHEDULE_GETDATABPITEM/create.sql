

CREATE PROCEDURE APZ_Schedule_GetdataBpItem
(
	IN bpCode nvarchar(15) default '',
	itemCode nvarchar(50) default '',
	shipDate date default ''
)
AS
BEGIN
	SELECT a.*, CASE WHEN t1."Price" <> NULL THEN t1."Price" WHEN t."Price" <> NULL THEN t."Price" ELSE IFNULL(t2."Price", 0) END AS "Price", 
			CASE WHEN t1."Price" <> NULL THEN t1."Price" WHEN t."Price" <> NULL THEN t."Price" ELSE IFNULL(t2."Price", 0) END AS "PriceBefDi", 
			CASE WHEN t1."Price" <> NULL THEN t1."Currency" WHEN t."Price" <> NULL THEN t."Currency" ELSE t2."Currency" END AS "Currency", 
			CASE WHEN t1."Price" <> NULL THEN t1."Discount" WHEN t."Price" <> NULL THEN t."Discount" ELSE 0 END AS "DiscPrcnt" 
	FROM (SELECT "CardCode", "CardName", "ListNum" FROM OCRD WHERE "CardCode" = :bpCode) AS a 
	LEFT OUTER JOIN OSPP t ON t."CardCode" = a."CardCode" AND t."ItemCode" = :itemCode 
	LEFT OUTER JOIN SPP1 t1 ON t1."CardCode" = a."CardCode" AND t1."ItemCode" = :itemCode AND :shipDate >= t1."FromDate" AND (t1."ToDate" = NULL OR t1."ToDate" = '' OR :shipDate <= t1."ToDate") 
	LEFT OUTER JOIN ITM1 t2 ON t2."PriceList" = a."ListNum" AND t2."ItemCode" = :itemCode;
END



