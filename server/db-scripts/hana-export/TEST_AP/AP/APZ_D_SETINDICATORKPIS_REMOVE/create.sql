
Create PROCEDURE APZ_D_SetIndicatorKPIs_Remove(in code nvarchar(50)default '')
AS
BEGIN
	if :code IN (select "Code" from APZ_D_OKPM) or :code IN (select "KPI_Code" from APZ_D_KPIS1 where "Type" = 'S')
	THEN
		 select 1, 'Cannot remove code because it has been used' from dummy;
		 return;
	
	end if;
	
	Delete from APZ_D_KPIS where "Code"=:code;
	Delete from APZ_D_KPIS1 where "Code"=:code;
	select 0, '' from DUMMY;
end




