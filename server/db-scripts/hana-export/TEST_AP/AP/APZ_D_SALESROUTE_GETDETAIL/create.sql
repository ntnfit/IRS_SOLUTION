

CREATE PROCEDURE APZ_D_SalesRoute_GetDetail(in routeCode nvarchar(50) default '')
AS
BEGIN

	select a.*, b."Id" as "BaseId" 
	from APZ_D_STR1 a
	left join APZ_D_GGP1 b on b."Code" = a."Code"
	where "RouteCode" = :routeCode;

END




