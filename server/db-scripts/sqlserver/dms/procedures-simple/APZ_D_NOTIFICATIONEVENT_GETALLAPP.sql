USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_NotificationEvent_GetAllApp]
AS
BEGIN
    SET NOCOUNT ON;
select * from APZ_D_OAPC;
END;
GO
