
CREATE PROCEDURE Apz_PromotionCondition_CFLCustomerGroup
AS
BEGIN
	SELECT "GroupCode", "GroupName" FROM OCRG WHERE "GroupType" = 'C';
END




