
CREATE  PROCEDURE APZ_WorkScheduling_GetColumnDoc
AS
BEGIN
	create local temporary table #Temptable as (select top 0 * from APZ_OWSH);
	insert into #Temptable ("Id", "Year") values (0, YEAR(current_date));
	select * from #Temptable;
	Drop table #Temptable;
END



