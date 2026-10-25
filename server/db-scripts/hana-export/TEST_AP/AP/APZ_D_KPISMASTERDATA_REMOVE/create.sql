
Create PROCEDURE APZ_D_KpisMasterData_Remove(in code nvarchar(50) default '')
AS
BEGIN

	if(:code IN (select "KPI_Code" from APZ_D_KPIS1))
	THEN
		 select 1, 'Cannot remove code because it has been used' from DUMMY;
		 return;
	
	end IF;
	Delete from APZ_D_OKPI where "Code"=:code;
	select 0, '' from DUMMY;

end




