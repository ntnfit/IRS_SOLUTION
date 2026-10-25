

CREATE PROCEDURE APZ_BusinessPartnerCatalogNumbers_GetBPByItemCode(
	in itemcode nvarchar(50)
)
AS
BEGIN
	select a.*, b."CardName", c."ItemName" from oscn a inner join ocrd b on a."CardCode" = b."CardCode" inner join OITM c on a."ItemCode" = c."ItemCode" where a."ItemCode" = :itemcode;
END




