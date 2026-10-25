
CREATE  PROCEDURE APZ_D_Customers_CheckPotential(in cardCode nvarchar(50) default '')
AS
BEGIN
	select top 1 "CardType" from OCRD where "CardCode" = :cardCode;
END



