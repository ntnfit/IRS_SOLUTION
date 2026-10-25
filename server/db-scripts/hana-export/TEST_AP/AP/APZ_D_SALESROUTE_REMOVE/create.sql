

CREATE PROCEDURE APZ_D_SalesRoute_Remove(in routeCode varchar(50))
AS
BEGIN
	
	delete from APZ_D_STR4 where "RouteCode" = :routeCode;
	delete from APZ_D_STR3 where "RouteCode" = :routeCode;
	delete from APZ_D_STR2 where "RouteCode" = :routeCode;
	delete from APZ_D_STR1 where "RouteCode" = :routeCode;
	delete from APZ_D_OSTR where "RouteCode" = :routeCode;

	select '' from dummy;	
END

