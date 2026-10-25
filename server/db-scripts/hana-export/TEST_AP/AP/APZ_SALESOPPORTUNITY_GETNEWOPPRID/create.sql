
CREATE PROCEDURE APZ_SalesOpportunity_GetNewOpprId

AS
BEGIN
	SELECT max("OpprId") "OpprId" FROM OOPR;
END



