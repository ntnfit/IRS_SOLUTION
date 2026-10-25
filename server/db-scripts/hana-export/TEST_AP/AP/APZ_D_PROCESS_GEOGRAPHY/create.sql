

CREATE PROCEDURE APZ_D_Process_Geography(in actionType char(1))
AS
BEGIN

	create local temporary table #APZ_D_OGGPLocal as (select top 0 * from APZ_D_OGGP);
	exec 'drop table #APZ_D_OGGPLocal';
	exec 'create local temporary table #APZ_D_OGGPLocal as (select * from "#APZ_D_OGGP")';
	
	create local temporary table #APZ_D_GGP1Local as (select top 0 * from APZ_D_GGP1);
	exec 'drop table #APZ_D_GGP1Local';
	exec 'create local temporary table #APZ_D_GGP1Local as (select * from "#APZ_D_GGP1");';

	--Update default value
	if :actionType = 'A'
	then
		update #APZ_D_OGGPLocal set "UserSign" = 1, "CreateDate" = current_date, "CreateTime" = cast((select GetTimeInteger(current_time) from dummy) as int), "UpdateDate" = null, "UpdateTime" = null;
		update #APZ_D_GGP1Local set "CreateDate" = current_date, "CreateTime" = cast((select GetTimeInteger(current_time) from dummy) as int), "UpdateDate" = null, "UpdateTime" = null;
	else
		update #APZ_D_OGGPLocal set "UserSign" = 1, "UpdateDate" = current_date, "UpdateTime" = cast((select GetTimeInteger(current_time) from dummy) as int);
		update #APZ_D_GGP1Local set "UpdateDate" = current_date, "UpdateTime" = cast((select GetTimeInteger(current_time) from dummy) as int);
	end if;

	update #APZ_D_OGGPLocal set "Status" = 'Y' where ifnull("Status", '') = '';
	update #APZ_D_GGP1Local set "Status" = 'Y' where ifnull("Status", '') = '';
		
	--Check Country Code empty value
	if exists(select 1 from #APZ_D_OGGPLocal where ifnull("CountryCode", '') = '')
	then
		select '02' as "ErrorCode", 'Country Code is empty' as "ErrorMessage" from dummy;
		return;
	end if;

	--Check Country Name empty value
	if exists(select 1 from #APZ_D_OGGPLocal where ifnull("CountryName", '') = '')
	then
		select '03' as "ErrorCode", 'Country Name is empty' as "ErrorMessage" from dummy;
		return;
	end if;

	--Check Region is empty value
	if exists(select 1 from #APZ_D_OGGPLocal where ifnull("Location", '') = '')
	then
		select '04' as "ErrorCode", 'Region is empty' as "ErrorMessage" from dummy;
		return;
	end if;

	--Check Code is empty value
	if exists(select 1 from #APZ_D_GGP1Local where ifnull("Code", '') = '')
	then
		select '06' as "ErrorCode", 'Code is empty' as "ErrorMessage" from dummy;
		return;
	end if;

	--Check Name is empty value
	if exists(select 1 from #APZ_D_GGP1Local where ifnull("Name", '') = '')
	then
		select '07' as "ErrorCode", 'Name is empty' as "ErrorMessage" from dummy;
		return;
	end if;

	--Check duplicated Geography header code
	if exists(select 1 from #APZ_D_OGGPLocal a inner join APZ_D_OGGP b on a."CountryCode" = b."CountryCode") and :actionType = 'A'
	then
		select '09' as "ErrorCode", 'The country already exists' as "ErrorMessage" from dummy;
		return;
	end if;
	
	--Check duplicated Geography details code
	if exists(select 1 from #APZ_D_GGP1Local group by "Code" having count(1) > 1)
	then
		select '10' as "ErrorCode", 'Duplicated Geography details code' as "ErrorMessage" from dummy;
		return;
	end if;
	
	--Remove empty row
	if exists(select 1 from #APZ_D_GGP1Local where ifnull("Code", '') = '' and ifnull("Name", '') = '')
	then
		delete from APZ_D_GGP1 where "CountryCode" in (select "CountryCode" from #APZ_D_GGP1Local);
		delete from #APZ_D_GGP1Local where ifnull("Code", '') = '' and ifnull("Name", '') = '';
	end if;
	
	/*-------------------------------------------------INSERT AND UPDATE DATA----------------------------------------------------------------*/
		
	if :actionType = 'U'
	then
		delete from APZ_D_OGGP where "CountryCode" in (select a."CountryCode" from APZ_D_OGGP a inner join #APZ_D_OGGPLocal b on a."CountryCode" = b."CountryCode");
		delete from APZ_D_GGP1 where "CountryCode" in (select a."CountryCode" from APZ_D_GGP1 a inner join #APZ_D_GGP1Local b on a."Code" = b."Code" and a."CountryCode" = b."CountryCode");
	end if;
	
	insert into APZ_D_OGGP("CountryCode", "CountryName", "Status", "Remarks", "Location", "UserSign", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime") 
	select "CountryCode", "CountryName", "Status", "Remarks", "Location", "UserSign", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime" from #APZ_D_OGGPLocal;
	
	insert into APZ_D_GGP1("Code", "Name", "CountryCode", "FatherCode", "Status", "Type", "Remarks", "Id", "FatherId", "NameWithType", "NamePath", "NamePathWithType", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime") 
	select * from #APZ_D_GGP1Local; 
		
	/*---------------------------------------------------------------------------------------------------------------------------------------*/

END





