
CREATE PROCEDURE Apz_D_PromotionCondition_CFLItem
AS
BEGIN
	SELECT "ItemCode", "ItemName", b."ItmsGrpNam" "ItemGroup", 'Y' "Active"
		FROM OITM a	LEFT JOIN OITB b ON b."ItmsGrpCod" = a."ItmsGrpCod";
END






