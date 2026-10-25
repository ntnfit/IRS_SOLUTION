
CREATE PROCEDURE APZ_D_Store_GetListAddress(in cardCode nvarchar(100) default '')
AS
BEGIN
	select a.*,a."Street" as "AddressDetail" from crd1 a Where :cardCode = '' or "CardCode"=:cardCode and "AdresType" = 'S';
END



