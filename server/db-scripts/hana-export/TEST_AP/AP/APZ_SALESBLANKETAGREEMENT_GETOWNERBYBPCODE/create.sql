
CREATE PROCEDURE APZ_SalesBlanketAgreement_GetOwnerByBPCode (
	IN cardcode nvarchar(50)
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	SELECT IFNULL("OwnerCode",-1) AS "OwnerCode" FROM OCRD WHERE "CardCode" = :cardcode;
END;




