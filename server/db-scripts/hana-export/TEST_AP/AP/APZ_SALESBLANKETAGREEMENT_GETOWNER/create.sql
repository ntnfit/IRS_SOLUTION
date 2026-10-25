
CREATE PROCEDURE APZ_SalesBlanketAgreement_GetOwner (

)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	SELECT "empID" AS "ID", "lastName" || ',' || "firstName" AS "Name" FROM OHEM;
END;




