
CREATE PROCEDURE APZ_CycleCountDeterminationSubtable_Save(in type char(1))
AS 
BEGIN
create local temporary table #templineSubLocal as (select top 0 * from ccs1);
execute immediate 'insert into #templineSubLocal select * from "#templine"';
	if :type = 'U'
	then
	delete  from ccs1 a  where ( a."WhsCode",a."Entry") in (select "WhsCode","Entry" from #templineSubLocal);
	end if;
	insert into ccs1
	select * from #templineSubLocal ;
	Drop table #templineSubLocal;
	
END




