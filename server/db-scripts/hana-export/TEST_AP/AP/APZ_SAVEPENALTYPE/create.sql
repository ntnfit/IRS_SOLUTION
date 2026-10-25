
CREATE PROCEDURE APZ_SavePenalType
AS
BEGIN

create local temporary table #temp_OHPT as (select top 0 * from APZ_OHPT);
execute immediate 'insert into #temp_OHPT select * from  #APZ_OHPT;';
UPSERT APZ_OHPT SELECT * FROM #temp_OHPT;
	
END




