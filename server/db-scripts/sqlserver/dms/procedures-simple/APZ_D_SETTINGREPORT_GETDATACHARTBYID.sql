USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingReport_GetDataChartById]
    @Id int = 0
AS
BEGIN
    SET NOCOUNT ON;
select * from APZ_D_SRT1 where [DocEntry]=@Id order by [OrderId];
END;
GO
