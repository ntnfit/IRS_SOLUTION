

CREATE PROCEDURE APZ_Service_CreditPaymentMethods_DeleteLine
	(in crTypeCode int)
AS
BEGIN
	DELETE FROM OCRP
       where "CrTypeCode" = :crTypeCode;
END




