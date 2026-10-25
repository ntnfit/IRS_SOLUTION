USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SettingReport_GetDetailByCode]
    @code nvarchar(250) = ''
AS
BEGIN
    SET NOCOUNT ON;
select * from APZ_UDR1 where [Code] = @code;
	select * from APZ_UDR2 where [Code] = @code;
END;
GO
