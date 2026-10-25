
CREATE PROCEDURE APZ_ActivitiesForOpportunity_GetMax
AS
BEGIN

	SELECT IFNULL( MAX("ClgCode"),0) as "ClgCode" FROM OCLG;

END



