

CREATE PROCEDURE APZ_D_SalesRoute_GetDistributors(
	in id int, 
	in routeCode varchar(50), 
	in empId int
)
AS
BEGIN

	select a."Id", a."RouteCode", a."EmpId", a."CardCode", b."CardName", b."CardType", b."Address", a."Remarks" 
	from APZ_D_STR3 a 
	inner join OCRD b on a."CardCode" = b."CardCode" 
	where a."Id" = :id and a."RouteCode" = :routeCode and a."EmpId" = :empId;

END




