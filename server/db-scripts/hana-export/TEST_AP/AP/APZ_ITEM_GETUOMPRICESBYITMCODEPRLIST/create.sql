
CREATE PROCEDURE apz_item_GetUoMPricesByItmCodePrList(
	in PriceList int default 0,
	in ItemCode nvarchar(50),
	in ugpEntry int)
AS
BEGIN

	create local temporary table #tempPrice as (SELECT TOP 0 b.*, b."Price" AS "BasePrice", b."Currency" AS "BaseCurrency", b."Price" AS "BasePrice1", b."Currency" AS "BaseCurrency1", b."Price" AS "BasePrice2"
				, b."Currency" AS "BaseCurrency2", b."Price" AS "ItemPerUnit", b."ItemCode" AS "UomName", b."ItemCode" AS "IsBase" FROM ITM9 b);

	IF  :ugpEntry>0
    THEN
    
		INSERT INTO #tempPrice ("ItemCode", "PriceList", "UomEntry", "Price", "Currency", "AutoUpdate", "AddPrice1", "Currency1", "AddPrice2", "Currency2", "ObjType", "ItemPerUnit", "UomName", "IsBase", "BaseCurrency",
									 "BaseCurrency1", "BaseCurrency2", "BasePrice", "BasePrice1", "BasePrice2") 
			SELECT :ItemCode AS "ItemCode", a."PriceList" AS "PriceList", b."UomEntry" AS "UomEntry", "Price", "Currency", 'Y' AS "AutoUpdate", "AddPrice1", "Currency1", "AddPrice2", "Currency2", "ObjType"
					, b."BaseQty" / b."AltQty", (SELECT "UomName" FROM OUOM WHERE "UomEntry" = (SELECT "BaseUom" FROM OUGP WHERE "UgpEntry" = :UgpEntry)) AS "UomName", 'Y', a."Currency", a."Currency1", a."Currency2",
					 a."Price", a."AddPrice1", a."AddPrice2" FROM ITM1 a INNER JOIN (SELECT * FROM UGP1 WHERE "UgpEntry" = :UgpEntry) AS b ON a."UomEntry" = b."UomEntry" 
					 			WHERE (IFNULL(:PriceList, 0) = 0 OR "PriceList" = :PriceList) AND "ItemCode" = :ItemCode;

     END IF;

	INSERT INTO #tempPrice
		SELECT a.*, b."Price", b."Currency", b."AddPrice1", b."Currency1", b."AddPrice2", b."Currency2", c."BaseQty" / c."AltQty", (SELECT "UomName" FROM OUOM WHERE "UomEntry" = a."UomEntry") AS "UomName", 
				'N' AS "IsBase" FROM #tempPrice b INNER JOIN ITM9 a ON a."PriceList" = b."PriceList" 
									INNER JOIN (SELECT * FROM UGP1 WHERE "UgpEntry" = :UgpEntry) AS c ON a."UomEntry" = c."UomEntry" 
												WHERE a."ItemCode" = :ItemCode AND (IFNULL(:PriceList, 0) = 0 OR a."PriceList" = :PriceList);

	UPDATE a SET "BasePrice" = a."BasePrice" * a."ItemPerUnit" / b."ItemPerUnit", "BasePrice1" = a."BasePrice1" * a."ItemPerUnit" / b."ItemPerUnit", "BasePrice2" = a."BasePrice2" * a."ItemPerUnit" / b."ItemPerUnit"
					 FROM #tempPrice a INNER JOIN (SELECT * FROM #tempPrice WHERE "IsBase" = 'Y') AS b ON a."PriceList" = b."PriceList" WHERE a."IsBase" = 'N';

	select * from #tempPrice;
	Drop table #tempPrice;
END




