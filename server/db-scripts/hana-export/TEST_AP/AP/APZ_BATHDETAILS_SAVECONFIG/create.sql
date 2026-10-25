
CREATE PROCEDURE APZ_BathDetails_SaveConfig(in userName nvarchar(50))
AS
BEGIN

	declare checkCount integer = 0;
	
	------------------------- Create and insert data to temp table -----------------------
	create local temporary table #tempConfigData as (select top 0 * from APZ_OABC);
	exec 'drop table #tempConfigData;';
	exec 'create local temporary table #tempConfigData as (select * from "#tempConfig");';
	-------------------------- END create temp table -------------------------------------
	
	select IFNULL((select count(1) from #tempConfigData),0) into checkCount from DUMMY;
	if :checkCount = 0
	then
		select 100021, 'Data config is empty' from DUMMY;
		return;
	END IF;
	
	checkCount = 0;
	select IFNULL((select count(1) from #tempConfigData group by "ColumnId" having count(1) > 1),0) into checkCount from DUMMY;
	if :checkCount > 0
	then
		select 100022, 'Can not dupplicate column config' from DUMMY;
		return;
	END IF;

	DELETE FROM APZ_OABC;
	update #tempConfigData set "UserSign" = :userName;
	INSERT INTO APZ_OABC select * from #tempConfigData;

	select 0, '' from DUMMY;

END




