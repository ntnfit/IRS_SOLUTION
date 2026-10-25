
CREATE PROCEDURE Util_AdvanceReportTemplate_GetColNum(in indCode nvarchar(10), in repCode nvarchar(10))
AS
BEGIN
	SELECT (SELECT COUNT(1) FROM APZ_ARTA WHERE "IndCode" = :indCode AND "RepCode" = :repCode) / (SELECT DISTINCT COUNT(1) FROM APZ_ARTA WHERE "IndCode" = :indCode AND "RepCode" = :repCode) AS "ColNum" FROM DUMMY;
END



