USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingReport_GetLineById]
    @id int = 0
AS
BEGIN
    SET NOCOUNT ON;
--select * from APZ_D_SRT1 where [DocEntry] = @id;
	select * from APZ_D_SRT2 where [DocEntry] = @id order by [Id] asc;
END;
GO
