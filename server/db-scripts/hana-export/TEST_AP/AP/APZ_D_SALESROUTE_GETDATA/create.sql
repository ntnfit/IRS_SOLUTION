

CREATE PROCEDURE APZ_D_SalesRoute_GetData(
	in routeCode nvarchar(50), 
	in routeName nvarchar(50), 
	in status varchar(1)
)
AS
BEGIN

	declare query varchar(5000) = '';
	
	if :routeCode <> '' or :routeName <> '' or :status <> ''
	then
		--Search data
		query := 'select * from APZ_D_OSTR where 
		("RouteCode" = '''' or "RouteCode" like ''' || :routeCode || '%''' || ')
		and ("RouteName" = '''' or "RouteName" like ''' || :routeName || '%''' || ')
		and ("Status" = '''' or "Status" like ''' || :status || '%''' ||')';
		execute immediate :query;
	else
		--Get data
		select * from APZ_D_OSTR;
	end if;

END




