
CREATE FUNCTION APZ_D_F_Notification_GetStringRoute(in routeId int, in paramsString nvarchar(50000))
	RETURNS routeString nvarchar(50000)
AS
BEGIN
--'{"navigationParams":{"parentRoute": "Delivery","routeName": "ListDelivery","routeParams": {} }}' 
	declare parentRoute nvarchar(100) = '';
	declare routeName nvarchar(100) = '';
	declare checkcount int;
	
	routeString = N'"navigationParams":{';
	parentRoute = ifnull((select ParentRoute from APZ_D_ONAR where "Id"=:routeId), '');
	
	routeName = ifnull((select RouteName from APZ_D_ONAR where "Id"=:routeId), '');

	routeString = :routeString || '"parentRoute":"' || :parentRoute || '","routeName":"' || :routeName || '","routeParams":{';

	checkCount = ifnull((select count(1) from APZ_D_NAR1 where "Id" = :routeId),0);
	
	while checkcount>0
	DO
		routeString = :routeString || '"' || ifnull((select "Name" from (select ROW_NUMBER() over (order by a."OrderId" asc) as "RowNum",a.* from APZ_D_NAR1 a) b where b."RowNum" = :checkCount),'')  || '":';
		if ifnull((select "DataType" from (select ROW_NUMBER() over (order by a."OrderId" asc) as "RowNum",a.* from APZ_D_NAR1 a) b where b."RowNum" = :checkcount),'string') = 'string'
		THEN
			routeString = :routeString || '"';
		end IF;
		
		routeString = :routeString || ifnull((select "part" from (select ROW_NUMBER() over (order by a."id" asc) as "RowNum",a.* from (select * from SplitString(:paramsString,'-apz-') where IFNULL("part",'') <> '') a) b where b."RowNum" = :checkCount),'');
		if ifnull((select "DataType" from (select ROW_NUMBER() over (order by a."OrderId" asc) as "RowNum",a.* from APZ_D_NAR1 a) b where b."RowNum" = :checkCount),'string') = 'string'
		THEN
			routeString = :routeString || '"';
		end IF;
		
		if :checkCount <> 1
		THEN
			routeString = :routeString || ',';
		end IF;
		
		checkCount = :checkCount - 1;
	end WHILE;
	
	routeString = :routeString || '}}';
END




