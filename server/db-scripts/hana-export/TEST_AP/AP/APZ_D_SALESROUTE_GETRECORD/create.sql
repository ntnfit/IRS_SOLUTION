

CREATE PROCEDURE APZ_D_SalesRoute_GetRecord(in posValue nvarchar(1), in routeCode nvarchar(50))
AS
BEGIN

	declare minRowNum int = -1;
	declare maxRowNum int = -1;
	declare currentRowNum int = -1;

	create local temporary table #dtTemp
	(
		"RowNum" int,
		"RouteCode" nvarchar(50)  not null,
		"RouteName" nvarchar(254) null,		
		"CountryCode" nvarchar(50) null,
		"Remarks" nvarchar(254) null,
		"Status" varchar(1) null,
		"ChannelCode" varchar(50) null,
		"UserSign" smallint,	
		"CreateDate" date null,
		"CreateTime" int null,
		"UpdateDate" date null,
		"UpdateTime" int
	);
	
	
	insert into #dtTemp("RowNum", "RouteCode", "RouteName", "CountryCode", "Remarks", "Status", "ChannelCode", "UserSign", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime") 
	select ROW_NUMBER() OVER(ORDER BY "RouteCode"), "RouteCode", "RouteName", "CountryCode", "Remarks", "Status", "ChannelCode", "UserSign", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime"
	from APZ_D_OSTR;
	
	currentRowNum := cast((select "RowNum" from #dtTemp where "RouteCode" = :routeCode) as int);
	
	/* Check postition type is valid */
	if (:posValue <> 'F' and :posValue <> 'L' and :posValue <> 'P' and :posValue <> 'N')
	then
		select '01' as "ErrorCode", 'Invalid Navigation type' as "ErrorMessage" from dummy;
		return;
	end if;

	--First
	if :posValue = 'F'
	then
		select top 1 * from #dtTemp order by "RowNum" asc;
	--Previous
	elseif :posValue = 'P'
	then
		
		select (select IFNULL(MIN("RowNum"), 0) from #dtTemp) into minRowNum from dummy;
		if :currentRowNum <= :minRowNum
		then
			select top 1 * from #dtTemp order by "RowNum" desc;
		else
			select top 1 * from #dtTemp where "RowNum" < :currentRowNum order by "RowNum" DESC;
		end if;
	--Next	
	elseif :posValue = 'N'
	then
		select (select IFNULL(MAX("RowNum"), 0) from #dtTemp) into maxRowNum from dummy;
		if :currentRowNum >= :maxRowNum
		then
			select top 1 * from #dtTemp  order by "RowNum" ASC;
		else
			select top 1 * from #dtTemp  where "RowNum" > :currentRowNum order by "RowNum" ASC;
		end if;
	--Last
	else
		select top 1 * from #dtTemp order by "RowNum" DESC;
	end if;

END




