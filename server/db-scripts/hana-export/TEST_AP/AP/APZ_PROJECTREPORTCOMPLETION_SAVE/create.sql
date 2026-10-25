

create procedure APZ_ProjectReportCompletion_Save(in type char(1))
as
begin

	Declare Id int =0;
	create local temporary table #APZ_ORTC_Local as (select top 0 * from APZ_ORTC);
	exec 'drop table #APZ_ORTC_Local';
	exec 'create local temporary table #APZ_ORTC_Local as (select * from #APZ_ORTC)';
	
	create local temporary table #APZ_RTC1_Local as (select top 0 * from APZ_RTC1);
	exec 'drop table #APZ_RTC1_Local';
	exec 'create local temporary table #APZ_RTC1_Local as (select * from #APZ_RTC1)';
	
	if :type = 'U' then
		select top 1 "DocEntry" into Id from #APZ_ORTC_Local;
		delete from APZ_RTC1 where "DocEntry" = :Id;
		delete from APZ_ORTC where "DocEntry" = :Id;
	else
		select ifnull(max("DocEntry"), 0) + 1 into Id from APZ_ORTC;
		update #APZ_ORTC_Local set "DocEntry" = :Id;
	end if;
				
	UPDATE #APZ_RTC1_Local set "DocEntry" = :Id, "LineNum"= (SELECT ROW_NUMBER() OVER( ORDER BY "DocEntry") FROM #APZ_RTC1_Local);
	insert into APZ_RTC1 select * from #APZ_RTC1_Local;
	insert into APZ_ORTC select * from #APZ_ORTC_Local;
	
	select :Id,'' from dummy;
	
end;




