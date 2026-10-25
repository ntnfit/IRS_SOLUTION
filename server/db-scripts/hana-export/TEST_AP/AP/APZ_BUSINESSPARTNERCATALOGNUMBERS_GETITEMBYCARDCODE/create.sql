
CREATE PROCEDURE APZ_BusinessPartnerCatalogNumbers_GetItemByCardCode(
	in cardcode nvarchar(50) default ''
)
AS
BEGIN
	select a.*, b."CardName", c."ItemName" from oscn a inner join ocrd b on a."CardCode" = b."CardCode" 
		inner join OITM c on a."ItemCode" = c."ItemCode" where a."CardCode" = :cardcode;
END




