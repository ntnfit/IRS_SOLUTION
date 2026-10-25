
CREATE PROCEDURE Apz_MenuStructure_GetListCustomizeMenu 

AS
BEGIN

		SELECT a.* FROM APZ_OMNI a INNER JOIN APZ_OMNI b ON a."FatherId" = b."Id" AND a."LanguageKey" = b."Code" WHERE a."MenuType" IN ('A','T','O') ORDER BY a."FatherId", a."Id";

END






