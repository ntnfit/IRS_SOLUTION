
create procedure APZ_PackingManagement_UpdateTraceability
as
begin
	
	declare CaseCode int;
	
	create local temporary table #APZ_DMC as (select top 0 * from APZ_DMC);
	exec 'insert into #APZ_DMC select * from #DMCT;';
	create local temporary table #APZ_DMB as (select top 0 * from APZ_DMB);
	exec 'insert into #APZ_DMB select * from #DMIT;';
	
	select (select top 1 "CaseCode" from #APZ_DMC) into CaseCode from dummy;
	
	delete from APZ_DMC where "CaseCode" = :CaseCode;
	insert into APZ_DMC select * from #APZ_DMC;
	
	delete from APZ_DMB where "CaseCode" = :CaseCode;
	insert into APZ_DMB select * from #APZ_DMB;
	
end;




