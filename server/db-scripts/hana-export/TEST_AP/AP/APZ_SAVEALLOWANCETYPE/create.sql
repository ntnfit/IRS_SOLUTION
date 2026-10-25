
CREATE PROCEDURE APZ_SaveAllowanceType
AS
BEGIN
	declare code int = 0;
	
	--Create Temp table
	create local temporary table #APZ_OHATLocal as (select top 0 * from APZ_OHAT);
	exec 'DROP TABLE #APZ_OHATLocal;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OHATLocal as (SELECT * from #APZ_OHAT);';

	delete from APZ_OHAT;
	select ifnull((select max("Code") from #APZ_OHATLocal),0) into code from DUMMY;
	
	while exists(select 1 from #APZ_OHATLocal where ifnull("Code",0) = 0)
	DO
		code = :code + 1;
		update top 1 #APZ_OHATLocal set "Code" = :Code where ifnull("Code",0) = 0;
	END WHILE;
	
	insert into APZ_OHAT
		select * from #APZ_OHATLocal where ifnull("Code",0)<>0;
	
END;




