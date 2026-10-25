
CREATE PROCEDURE APZ_AdvanceForecast_GetColumnsDoc
AS
BEGIN
	create local temporary table #tempOFCT as (select top 0 * from APZ_OFCT);
	
	insert into #tempOFCT ("Code","Name", "FormView", "StartDate","EndDate") 
	values (0,'', 'D', current_date, current_date);
	
	select * from #tempOFCT;
	Drop table #tempOFCT;
END




