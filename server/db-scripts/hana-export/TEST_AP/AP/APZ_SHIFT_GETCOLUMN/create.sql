
CREATE PROCEDURE APZ_Shift_GetColumn

AS
BEGIN
	create local temporary table #Apztemp as (select top 0 * from APZ_OSHP);
	insert into #Apztemp ("SpID","Code") values (0,'');
	select * from #Apztemp;
	Drop table #Apztemp;
END;



