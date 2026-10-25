USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_SalesRoute_GetDistributors]
    @id int,
    @routeCode varchar(50),
    @empId int
AS
BEGIN
    SET NOCOUNT ON;
select a.[Id], a.[RouteCode], a.[EmpId], a.[CardCode], b.[CardName], b.[CardType], b.[Address], a.[Remarks] 
	from APZ_D_STR3 a 
	inner join OCRD b on a.[CardCode] = b.[CardCode] 
	where a.[Id] = @id and a.[RouteCode] = @routeCode and a.[EmpId] = @empId;
END;
GO
