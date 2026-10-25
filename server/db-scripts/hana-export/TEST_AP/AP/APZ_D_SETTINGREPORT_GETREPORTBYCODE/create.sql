

CREATE PROCEDURE APZ_D_SettingReport_GetReportByCode(in code nvarchar(50))
AS
BEGIN

	select a.*, b."Name",b."StoreName" from apz_D_OSRT as a
	left join APZ_OUDR as b on b."Code" = a."ReportId" where a."ReportId" = :code;

END



