
CREATE  PROCEDURE APZ_WorkScheduling_GetColumnLine
AS
BEGIN
	create local temporary table #Temptable as (select top 0 * from APZ_WSH1);
	insert into #Temptable ("ID", "WorkDate") values (0, '');
	select * from #Temptable;
	Drop table #Temptable;
END



