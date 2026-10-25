
CREATE PROCEDURE Apz_PromotionCondition_CFLItemGroup
AS
BEGIN
	SELECT "ItmsGrpCod" "Code", "ItmsGrpNam" "Name" FROM OITB WHERE "Locked" = 'N';
END





