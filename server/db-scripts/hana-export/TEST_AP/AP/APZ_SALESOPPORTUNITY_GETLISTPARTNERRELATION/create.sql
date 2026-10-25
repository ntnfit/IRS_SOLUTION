
CREATE PROCEDURE APZ_SalesOpportunity_GetListPartnerRelation
AS
BEGIN
SELECT 
IFNULL("OrlCode",-1) as "OrlCode",
IFNULL("OrlDesc",'') as "OrlDesc"
   FROM OORL;
   END




