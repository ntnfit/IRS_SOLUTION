

CREATE PROCEDURE APZ_D_Delivery_GetLocationNote (IN shipCode NVARCHAR(50))
AS
BEGIN

	SELECT * FROM APZ_D_DLO4 WHERE "ShipCode" = :shipCode;

END




