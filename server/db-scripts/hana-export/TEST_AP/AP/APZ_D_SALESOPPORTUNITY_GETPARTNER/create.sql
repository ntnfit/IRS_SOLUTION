
CREATE PROCEDURE APZ_D_SalesOpportunity_GetPartner (OpprId int DEFAULT 0)
AS
BEGIN

	SELECT a.*, b."OrlDesc", c."Name" FROM OPR2 a 
	LEFT JOIN OORL b ON a."OrlCode" = b."OrlCode" 
	LEFT JOIN OPRT c ON a."ParterId" = c."PrtId" 
	WHERE a."OpportId" = :OpprId;
	
END




