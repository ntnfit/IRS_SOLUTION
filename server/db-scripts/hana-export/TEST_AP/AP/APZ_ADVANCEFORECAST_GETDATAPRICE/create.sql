
CREATE PROCEDURE APZ_AdvanceForecast_GetDataPrice(in priceList int  default -1)
AS
BEGIN
	SELECT "ItemCode", "Price" FROM ITM1 WHERE "PriceList" = :priceList;
END;



