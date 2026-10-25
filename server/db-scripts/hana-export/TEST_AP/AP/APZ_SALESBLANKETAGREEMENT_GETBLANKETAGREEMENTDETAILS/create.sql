
CREATE PROCEDURE APZ_SalesBlanketAgreement_GetBlanketAgreementDetails (
	IN 	AgrNo INT
, 		AgrLnNum INT 
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	IF :AgrLnNum > 0 THEN 
		SELECT a.* FROM OAT2 a WHERE a."AgrNo" = :AgrNo AND a."AgrLnNum" = :AgrLnNum;
	ELSE 
		SELECT a.* FROM OAT2 a WHERE a."AgrNo" = :AgrNo;
	END IF;
END;




