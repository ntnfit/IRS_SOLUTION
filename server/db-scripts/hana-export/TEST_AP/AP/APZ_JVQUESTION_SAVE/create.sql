

CREATE PROCEDURE APZ_JVQuestion_Save
AS
BEGIN
	declare QuesCode int=0;
	declare AnsCode int=0;
		
	--Create temp table #dtAPZ_OQLQLocal 	
	create local temporary table #dtAPZ_OQLQLocal as (select top 0 * from APZ_OQLQ);
	exec 'drop table #dtAPZ_OQLQLocal;';
	exec 'create local temporary table #dtAPZ_OQLQLocal as (select * from "#dtAPZ_OQLQ");';
	
	--Create temp table #dtAPZ_OQLALocal
	create local temporary table #dtAPZ_OQLALocal as (select top 0 * from APZ_OQLA);
	exec 'drop table #dtAPZ_OQLALocal;';
	exec 'create local temporary table #dtAPZ_OQLALocal as (select * from "#dtAPZ_OQLA");';
	
	
	If Exists(Select 1 From #dtAPZ_OQLQLocal Where Ifnull("QuesCode",0) = 0)
	Then 
		Select ifnull(max("QuesCode"),0) Into QuesCode From APZ_OQLQ;
		QuesCode = :QuesCode + 1;
		--CONFIG DATA TABLE APZ_OQLQ
		Update #dtAPZ_OQLQLocal Set "QuesCode" = :QuesCode;
		--CONFIG DATA TABLE APZ_OQLA
		Update #dtAPZ_OQLALocal Set "QuesCode" = :QuesCode;
	Else
		Select "QuesCode" Into QuesCode From #dtAPZ_OQLQLocal;
		--CONFIG DATA TABLE APZ_OQLQ
		Delete From APZ_OQLQ Where "QuesCode" = :QuesCode;
		--CONFIG DATA TABLE APZ_OQLA
		Update #dtAPZ_OQLALocal Set "QuesCode" = :QuesCode;
		Delete From APZ_OQLA Where "QuesCode" = :QuesCode;
	End if;
	
	Insert into APZ_OQLQ select * From #dtAPZ_OQLQLocal;
	Insert into APZ_OQLA select * from #dtAPZ_OQLALocal;
	
	select Cast(:QuesCode AS Integer) from APZ_OQLQ;
END;




