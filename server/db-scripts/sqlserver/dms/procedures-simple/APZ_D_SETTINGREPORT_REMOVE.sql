USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingReport_Remove]
    @id int = 0
AS
BEGIN
    SET NOCOUNT ON;
delete from APZ_D_SRT2 where [DocEntry] = @id;
	delete from APZ_D_SRT1 where [DocEntry] = @id;
	delete from APZ_D_OSRT where [DocEntry] = @id;
	select 0, '';
END;
GO
