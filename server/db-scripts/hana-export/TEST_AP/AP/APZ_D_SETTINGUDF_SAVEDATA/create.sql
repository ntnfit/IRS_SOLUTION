

CREATE PROCEDURE APZ_D_SettingUDF_SaveData()
AS
BEGIN
	declare menuID int;
	
	create local temporary table #TempLocal as (select top 0 * from APZ_D_SUDF);
	exec 'drop table #TempLocal';
	exec 'create local temporary table #TempLocal as (select * from "#temp")';
	
	select top 1 "MenuID" into menuID from #TempLocal;
	delete from APZ_D_SUDF where "MenuID" = :menuID;
	
	if exists (select 1 from #TempLocal) then
		insert into APZ_D_SUDF("TableID","MenuID","FieldID","IsActive","IsEditable","CreateDate","UserSign")
		select "TableID","MenuID","FieldID","IsActive","IsEditable",current_timestamp,"UserSign" from #TempLocal;
		if ::ROWCOUNT<=0 THEN
 			SELECT 406 AS "MessageCode", 'Insert data to table APZ_D_SUDF failed' AS "Message" FROM DUMMY;
			RETURN;
		else
			SELECT 200 AS "MessageCode", '' AS "Message" FROM DUMMY;
			RETURN;
		end IF;
	else
		SELECT 404 AS "MessageCode", 'Notfound data to save' AS "Message" FROM DUMMY;
		RETURN;
	end if;
END



