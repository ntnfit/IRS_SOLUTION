

CREATE PROCEDURE APZ_D_Auth_Update(in userCode nvarchar(50))
AS
BEGIN

	create local temporary table #APZ_D_AUTHLocal as (select top 0 * from APZ_D_OPRM);
	exec 'drop table #APZ_D_AUTHLocal';
	exec 'create local temporary table #APZ_D_AUTHLocal as (select * from "#APZ_D_AUTH")';

	delete from APZ_D_OPRM where USER_CODE = :userCode;
	
	insert into APZ_D_OPRM(USER_CODE, "menucode", "authoriza", "isDefault")
	select USER_CODE, "menucode", "authoriza", "isDefault" from #APZ_D_AUTHLocal;
	
	if ::ROWCOUNT > 0
	then
		select 200 as "Code", '' as "Message" from dummy;
	else
		select 406 as "Code", 'Insert APZ_D_OPRM fail' as "Message" from dummy;
	end if;

END



