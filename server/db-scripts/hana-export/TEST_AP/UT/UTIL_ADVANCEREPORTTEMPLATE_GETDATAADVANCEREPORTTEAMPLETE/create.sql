
CREATE PROCEDURE Util_AdvanceReportTemplate_GetDataAdvanceReportTeamplete
AS
BEGIN
	SELECT *, 'Y' AS "IsExists" FROM APZ_ART ORDER BY "TCode";
END



