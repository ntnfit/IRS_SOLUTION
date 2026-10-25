

CREATE PROCEDURE APZ_PromotionSetup_Save

AS
BEGIN

	declare id int;

	CREATE LOCAL TEMPORARY TABLE #PRCLocal as (select top 0 * from APZ_D_OPRC);
	exec 'drop table #PRCLocal';
	exec 'create local temporary table #PRCLocal as (select * from #APZ_D_OPRC)';
	
	CREATE LOCAL TEMPORARY TABLE #PRTLocal as (select top 0 * from APZ_D_OPRT);
	exec 'drop table #PRTLocal';
	exec 'create local temporary table #PRTLocal as (select * from #APZ_D_OPRT)';
	
	CREATE LOCAL TEMPORARY TABLE #PRGLocal as (select top 0 * from APZ_D_OPRG);
	exec 'drop table #PRGLocal';
	exec 'create local temporary table #PRGLocal as (select * from #APZ_D_OPRG)';
	
	-------------------------Category-----------------------
	--Delete empty row
	delete from #PRCLocal where ifnull(ltrim("Name"),'') = '' and IFNULL(ltrim("Descr"),'') = '';

	--Check empty
	if exists (select 1 from #PRCLocal where ifnull(ltrim("Name"),'') = '' and ifnull(lTRIM("Descr"),'') <> '') then
		select 1, 'Invalid name' from dummy;
		return;
	end if;

	--Check duplicate name
	if exists(select 1 from #PRCLocal group by LOWER("Name") having COUNT(1) > 1) then
		select 1, 'Duplicate name' from dummy;
		return;
	end if;

	-------------------Type------------------------

	--Delete empty row
	delete from #PRTLocal where ifnull(ltrim("Name"),'') = '' and ifnull(ltrim("Descr"),'') = '';

	if exists(select 1 from #PRTLocal where ifnull(ltrim("Name"),'') = '' and ifnull(lTRIM("Descr"),'') <> '') then
		select 2, 'Invalid name' from dummy;
		return;
	end if;

	--Check duplicate name
	if exists(select 1 from #PRTLocal group by LOWER("Name") having COUNT(1) > 1) then
		select 2, 'Duplicate name' from dummy;
		return;
	end if;


	---------------------Group---------------

	--Delete empty row
	delete from #PRGLocal where ifnull(ltrim("Name"),'') = '' and ifnull(ltrim("Descr"),'') = '';

	if exists(select 1 from #PRGLocal where ifnull(ltrim("Name"),'') = '' and ifnull(lTRIM("Descr"),'') <> '') then
		select 3, 'Invalid name' from dummy;
		return;
	end if;

	--Check duplicate name
	if exists(select 1 from #PRGLocal group by LOWER("Name") having COUNT(1) > 1) then
		select 3, 'Duplicate name' from dummy;
		return;
	end if;


	----------------update data---------------

	create local temporary table #temp (ID int, "Name" nvarchar(250), "Descr" ntext, row_num int);
	insert into #temp select ID, "Name", "Descr", row_number() over(partition by ifnull(ID, 0)) row_num from #PRCLocal;
	id := ifnull((select MAX(ID) from #PRCLocal), 0);
	update #temp set ID = :id + row_num where ifnull(ID, 0) = 0;

 	delete from APZ_D_OPRC;
	insert into APZ_D_OPRC(ID, "Name", "Descr") select ID, "Name", "Descr" from #temp;

	
	delete from #temp;
	
	insert into #temp select ID, "Name", "Descr", row_number() over(partition by ifnull(ID, 0)) row_num from #PRGLocal;
	id := ifnull((select MAX(ID) from #PRGLocal), 0);
	update #temp set ID = :id + row_num where ifnull(ID, 0) = 0;

 	delete from APZ_D_OPRG;
	insert into APZ_D_OPRG(ID, "Name", "Descr") select ID, "Name", "Descr" from #temp;
	
	delete from #temp;
	
	insert into #temp select ID, "Name", "Descr", row_number() over(partition by ifnull(ID, 0)) row_num from #PRTLocal;
	id := ifnull((select MAX(ID) from #PRTLocal), 0);
	update #temp set ID = :id + row_num where ifnull(ID, 0) = 0;

 	delete from APZ_D_OPRT;
	insert into APZ_D_OPRT(ID, "Name", "Descr") select ID, "Name", "Descr" from #temp;

	select 0,'' from dummy;

END




