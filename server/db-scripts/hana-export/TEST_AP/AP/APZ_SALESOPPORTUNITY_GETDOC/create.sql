
CREATE  PROCEDURE APZ_SalesOpportunity_GetDoc (OpprId int DEFAULT 0, UserName NVARCHAR(50))
AS
BEGIN
	call apz_func_OwnerOverShip(:UserName,'97',OwnerView);

	 SELECT a.*, 
	 	(SELECT IFNULL("lastName", '') || ', ' || IFNULL("firstName", '') FROM OHEM WHERE "empID" = "Owner") AS "OwnerName", 
	 	IFNULL("Owner", -1) AS "OwnerCode", b."MailAddres", CAST('VND' AS nvarchar(3)) AS "DocCurr" 
	 	FROM OOPR a LEFT OUTER JOIN OCRD b ON a."CardCode" = b."CardCode" 
	 	WHERE "OpprId" = :OpprId 
	 	AND ((IFNULL("Owner", -1) = -1 OR ("Owner" IN (SELECT DISTINCT x."OwnerId" FROM :OwnerView x))) OR :UserName = 'manager');

END




