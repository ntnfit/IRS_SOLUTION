USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingReport_GetDataDashboardById]
    @docEntry int = -1,
    @id int = 0
AS
BEGIN
    SET NOCOUNT ON;
select * from APZ_D_SRT3 where ((@docEntry = -1) or [DocEntry]=@docEntry) and ((@id = -1) or [DashboardId] = @id) order by [DashboardId];
END;
GO
