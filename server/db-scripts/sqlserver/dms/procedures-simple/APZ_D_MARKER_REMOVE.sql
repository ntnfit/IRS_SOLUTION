USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_Marker_Remove]
    @code nvarchar(50) = ''
AS
BEGIN
    SET NOCOUNT ON;
delete from APZ_D_MARKER where [Code] = @code; 
	select 0, '';
END;
GO
