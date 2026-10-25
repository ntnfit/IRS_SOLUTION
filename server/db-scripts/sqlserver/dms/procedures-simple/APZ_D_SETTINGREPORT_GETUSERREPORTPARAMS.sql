USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingReport_GetUserReportParams]
    @docEntry int,
    @userCode nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
select * from APZ_D_SRT4
	where [DocEntry] = @docEntry and [UserCode] = @userCode;
END;
GO
