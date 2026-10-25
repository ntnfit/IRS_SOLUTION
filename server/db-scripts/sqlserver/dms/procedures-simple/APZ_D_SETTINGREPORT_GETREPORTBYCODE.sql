USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingReport_GetReportByCode]
    @code nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
select a.*, b.[Name],b.[StoreName] from apz_D_OSRT as a
	left join APZ_OUDR as b on b.[Code] = a.[ReportId] where a.[ReportId] = @code;
END;
GO
