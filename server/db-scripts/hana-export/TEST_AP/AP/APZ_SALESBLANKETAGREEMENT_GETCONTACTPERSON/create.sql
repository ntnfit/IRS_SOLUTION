
CREATE PROCEDURE APZ_SalesBlanketAgreement_GetContactPerson (
	IN CardCode nvarchar(15) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	SELECT "CntctCode", "Tel1", "E_MailL", "Name", "CardCode" FROM OCPR WHERE "CardCode" = :CardCode;
END;




