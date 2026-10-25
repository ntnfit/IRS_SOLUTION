-- HANA: APZ_SalesBlanketAgreement_GetBlanketAgreementDetails
CREATE OR ALTER PROCEDURE dbo.APZ_SalesBlanketAgreement_GetBlanketAgreementDetails
    @AgrNo int,
    @AgrLnNum int
AS
BEGIN
    SET NOCOUNT ON;
    IF @AgrLnNum > 0
        SELECT a.* FROM OAT2 a WHERE a.AgrNo = @AgrNo AND a.AgrLnNum = @AgrLnNum;
    ELSE
        SELECT a.* FROM OAT2 a WHERE a.AgrNo = @AgrNo;
END;
