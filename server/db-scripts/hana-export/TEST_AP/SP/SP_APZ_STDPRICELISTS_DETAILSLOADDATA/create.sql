
CREATE PROCEDURE SP_APZ_STDPriceLists_DetailsLoadData(in ListNum int default -1)
AS
BEGIN
	SELECT a.*, b."ItemName", b."InvntryUom", c."Price" AS "BasePrice", c."Currency" AS "BaseCurrency", c."AddPrice1" AS "BasePrice1", c."Currency1" AS "BaseCurrency1", c."AddPrice2" AS "BasePrice2"
			, c."Currency2" AS "BaseCurrency2" FROM ITM1 a LEFT OUTER JOIN OITM b ON a."ItemCode" = b."ItemCode" 
							LEFT OUTER JOIN ITM1 c ON c."PriceList" = a."BasePLNum" AND a."ItemCode" = c."ItemCode" WHERE a."PriceList" = :ListNum ORDER BY a."ItemCode";
END;



