USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_AlertTemplate_Remove]
    @code varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
--Delete Item data by code
	delete from APZ_D_OALT where [Code] = @code;
	select '';
END;
GO
