
CREATE PROCEDURE Util_AdvanceReportTemplate_DeleteReport(in repCode nvarchar(20))
AS
BEGIN

	Delete from APZ_ARTA where "RepCode" = :repCode;
	Delete from APZ_ARTI where "RepCode" = :repCode;
	Delete from APZ_ART where "RepCode" = :repCode;
END



