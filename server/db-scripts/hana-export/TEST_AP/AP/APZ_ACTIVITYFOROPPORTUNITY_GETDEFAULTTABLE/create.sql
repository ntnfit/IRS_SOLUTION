
CREATE procedure APZ_ActivityForOpportunity_GetDefaultTable(in clgCode int default -1)
as
BEGIN
	if(:clgCode=-1)
	THEN
		create local temporary table #tbl as (select top 0 * from OCLG);
		insert into #tbl ("ClgCode") values (0);
		select * from #tbl;
	end IF;

END



