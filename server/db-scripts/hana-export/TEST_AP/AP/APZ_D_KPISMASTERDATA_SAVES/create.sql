
Create PROCEDURE APZ_D_KpisMasterData_Saves
AS
BEGIN
	delete from  APZ_D_OKPI;
	exec 'insert into APZ_D_OKPI select * from #APZ_D_OKPI';
	select 0, '' from DUMMY;
END



