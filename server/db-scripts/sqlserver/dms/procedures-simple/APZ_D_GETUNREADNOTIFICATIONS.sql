USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_GetUnreadNotifications]
    @userName NVARCHAR(50),
    @appName NVARCHAR(50),
    @dbName NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
SELECT * FROM APZ_D_ELN1 WHERE [Method] = @appName  and [DatabaseName] = @dbName and (dbo.CompareSplit(@userName, [SendUser], ',') = 1) and [Read] = 'N' and [IsRemove] = 'N';
END;
GO
