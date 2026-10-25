
create PROCEDURE APZ_CycleCountDetermination_Save(in type char(1))
AS 
BEGIN
	create local temporary table #templineLocal as (select top 0 * from OCCS);
	exec 'insert into #templineLocal select * from "#templine"';
if :type = 'U'
then	
	delete from OCCS a where  A."WhsCode" in (select "WhsCode" from #templineLocal);
	end if;
	insert into OCCS select * from #templineLocal ;
	drop table #templineLocal;

END;




