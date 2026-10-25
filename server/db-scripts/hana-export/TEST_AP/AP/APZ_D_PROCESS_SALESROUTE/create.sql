

CREATE PROCEDURE APZ_D_Process_SalesRoute(in actionType char(1))
AS
BEGIN

	declare routeCode nvarchar(100) = '';
	
	create local temporary table #APZ_D_OSTRLocal as(select top 0 * from APZ_D_OSTR);
	exec 'DROP TABLE #APZ_D_OSTRLocal';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_D_OSTRLocal as (select * from "#APZ_D_OSTR")';
	
	create local temporary table #APZ_D_STR1Local as(select top 0 * from APZ_D_STR1);
	exec 'DROP TABLE #APZ_D_STR1Local';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_D_STR1Local as (select * from "#APZ_D_STR1")';
	
	create local temporary table #APZ_D_STR2Local as(select top 0 * from APZ_D_STR2);
	exec 'DROP TABLE #APZ_D_STR2Local';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_D_STR2Local as (select * from "#APZ_D_STR2")';
	
	create local temporary table #APZ_D_STR3Local as(select top 0 * from APZ_D_STR3);
	exec 'DROP TABLE #APZ_D_STR3Local';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_D_STR3Local as (select * from "#APZ_D_STR3")';
	
	create local temporary table #APZ_D_STR4Local as(select top 0 * from APZ_D_STR4);
	exec 'DROP TABLE #APZ_D_STR4Local';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_D_STR4Local as (select * from "#APZ_D_STR4")';
	
	if :actionType = 'A'
	then 
		update #APZ_D_OSTRLocal set "CreateDate" = current_date, "CreateTime" =  cast((select GetTimeInteger(current_time) from dummy) as int), "UpdateDate" = null, "UpdateTime" = null;
	else
		update #APZ_D_OSTRLocal set "UpdateDate" = current_date, "UpdateTime" = cast((select GetTimeInteger(current_time) from dummy) as int);
	end if;
	
	routeCode := ifnull((select top 1 "RouteCode" from #APZ_D_OSTRLocal), '');
	
	if :routeCode = ''
	then
		select 1, N'Vui lòng nhập mã tuyến' from dummy;
		return;
	end if;	
	
	update #APZ_D_STR1Local set "RouteCode" = :routeCode;
	update #APZ_D_STR2Local set "RouteCode" = :routeCode;
	update #APZ_D_STR3Local set "RouteCode" = :routeCode;
	update #APZ_D_STR4Local set "RouteCode" = :routeCode;
	
	delete from APZ_D_OSTR where "RouteCode" = :routeCode;
	delete from APZ_D_STR1 where "RouteCode" = :routeCode;
	delete from APZ_D_STR2 where "RouteCode" = :routeCode;
	delete from APZ_D_STR3 where "RouteCode" = :routeCode;
	delete from APZ_D_STR4 where "RouteCode" = :routeCode;
	
	insert into APZ_D_OSTR
	select * from #APZ_D_OSTRLocal;

	insert into APZ_D_STR1
	select * from #APZ_D_STR1Local;

	insert into APZ_D_STR2
	select * from #APZ_D_STR2Local where ifnull("CardCode", '') <> '';

	insert into APZ_D_STR3
	select * from #APZ_D_STR3Local where ifnull("CardCode", '') <> '';

	insert into APZ_D_STR4
	select * from #APZ_D_STR4Local where ifnull("EmpId", 0) <> 0;
	
	select 0, '' from dummy;
	
END




