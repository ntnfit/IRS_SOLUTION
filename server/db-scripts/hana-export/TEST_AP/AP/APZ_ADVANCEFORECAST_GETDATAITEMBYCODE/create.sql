
CREATE PROCEDURE APZ_AdvanceForecast_GetDataItemByCode(IN itemCode nvarchar(50))
AS
BEGIN
	Select "ItemName", "DfltWH" from OITM where "ItemCode" = :itemCode;
END;




