USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingReport_GetAll]
    @userCode nvarchar(50) = ''
AS
BEGIN
    SET NOCOUNT ON;
SELECT a.*, b.[Name], b.[StoreName] FROM apz_D_OSRT a 
	INNER JOIN APZ_OUDR b ON b.[Code] = a.[ReportId] 
	INNER JOIN OPRM C ON C.USER_CODE = @userCode AND REPLACE(c.[menucode], 'frm_', '') = a.[ReportId] 
	AND c.[authoriza] = 'F' WHERE a.[Active] = 'Y' order by [ReportName] asc;
END;
GO
