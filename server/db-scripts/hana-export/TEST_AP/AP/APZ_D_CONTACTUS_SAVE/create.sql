

Create PROCEDURE APZ_D_ContactUs_Save
AS
BEGIN
	DECLARE id int;
	declare maxCode int = IFNULL((select max(IFNULL("Id",0)) from APZ_D_CTUS), 0);
	
	create local temporary table #CTUSLocal as (select top 0 * from APZ_D_CTUS);
	exec 'drop table #CTUSLocal';
	exec 'create local temporary table #CTUSLocal as (select * from #APZ_D_CTUS)';
	
	update #CTUSLocal set "Id"= :maxCode+1;
	insert into APZ_D_CTUS select * from #CTUSLocal;
	select :id,'' FROM DUMMY;
END



