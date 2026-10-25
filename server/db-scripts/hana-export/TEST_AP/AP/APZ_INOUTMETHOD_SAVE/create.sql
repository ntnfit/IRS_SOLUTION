
CREATE PROCEDURE APZ_InOutMethod_Save
AS
BEGIN
	declare id int=0;
	
	--Create Temp Table
	create local temporary table #APZ_OIOMLocal as (select top 0 * from APZ_OIOM);
	exec 'DROP TABLE #APZ_OIOMLocal;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OIOMLocal as (select * from #APZ_OIOM);';
	
	--Delete current record in APZ_OIOM
	select ifnull((select top 1 "MethodId" from APZ_OIOM),0) into id from DUMMY;
	delete from APZ_OIOM where "MethodId"=:id;
	
	--Update MethodId in Temp table
	select ifnull((select max("MethodId") from APZ_OIOM),0) + 1 into id from DUMMY;
	update #APZ_OIOMLocal set "MethodId"=:id where ifnull("MethodId",0)=0;
	
	--Insert data from Temp table
	insert into APZ_OIOM("MethodId", "Code", "Dscription", "InOutMethod", "MaxInterval", "MinInterval", "BetweenInterval")
	select * from #APZ_OIOMLocal;
	
	--Return Id after inserted
	select :id,'' from DUMMY;
END;




