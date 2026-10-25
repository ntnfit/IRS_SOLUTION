USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SalesRoute_GetDetail]
    @routeCode nvarchar(50) = ''
AS
BEGIN
    SET NOCOUNT ON;
select a.*, b.[Id] as [BaseId] 
	from APZ_D_STR1 a
	left join APZ_D_GGP1 b on b.[Code] = a.[Code]
	where [RouteCode] = @routeCode;
END;
GO
