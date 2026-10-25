
CREATE PROCEDURE Apz_KPIPlan_CFLBusinessPartner
AS
BEGIN
	select "CardCode", "CardName" from OCRD	WHERE "validFor" = 'Y';
END




