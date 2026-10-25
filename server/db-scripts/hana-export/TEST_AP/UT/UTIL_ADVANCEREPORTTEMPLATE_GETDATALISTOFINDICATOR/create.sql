
CREATE PROCEDURE Util_AdvanceReportTemplate_GetDataListOfIndicator(in repCode nvarchar(20))
AS
BEGIN
	SELECT *, "Color" AS "Color1" FROM APZ_ARTI WHERE "RepCode" = :repCode ORDER BY "TCode";
END



