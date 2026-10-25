

CREATE PROCEDURE APZ_Process_WorkingProcessSetting
AS
BEGIN
	
	declare maxId int = 0;
	declare rowCount int = 0;

	create local temporary table #APZ_WPS1Local as (select top 0 * from APZ_WPS1);
	exec 'drop table #APZ_WPS1Local';
	exec 'create local temporary table #APZ_WPS1Local as (select * from #APZ_WPS1)';
	
	create local temporary table #APZ_WPS2Local as (select top 0 * from APZ_WPS2);
	exec 'drop table #APZ_WPS2Local';
	exec 'create local temporary table #APZ_WPS2Local as (select * from #APZ_WPS2)';

	----------------------------------------------------Save menu--------------------------------------------------------------

	delete from APZ_WPS1 where "UserId" = (select top 1 "UserId" from #APZ_WPS1Local);

	select (select IFNULL(MAX("Id"), 0) from APZ_WPS1) into maxId from dummy;
	insert into APZ_WPS1("Id", "UserId", "DocType", "MnuType", "Visible", "Remarks")
	select ROW_NUMBER() OVER(ORDER BY "Id") + :maxId, "UserId", "DocType", "MnuType", IFNULL("Visible", 'Y'), "Remarks" from #APZ_WPS1Local;
	
	-------------------------------------------------------------------------------------------------------------------------------------------

	----------------------------------------------------Save tooltips----------------------------------------------------------
	select (select count(1) from #APZ_WPS2Local) into rowCount from dummy;
	if :rowCount > 1
	then
		delete from #APZ_WPS2Local where IFNULL("ColId", '') = '' and IFNULL("DispText", '') = '';
		select (select IFNULL(MAX("Id"), 0) from APZ_WPS2) into maxId from dummy;

		delete from APZ_WPS2 where "UserId" = (select top 1 "UserId" from #APZ_WPS2Local);
		insert into APZ_WPS2("Id", "UserId", "ColId", "DispText", "TableName", "ExecQuery", "VisOrder")
		select ROW_NUMBER() OVER(ORDER BY "Id") + :maxId, "UserId", "ColId", "DispText", "TableName", "ExecQuery", "VisOrder" from #APZ_WPS2Local;
	else
		delete from APZ_WPS2;
	end if;

	-------------------------------------------------------------------------------------------------------------------------------------------
	select '' from dummy;

END




