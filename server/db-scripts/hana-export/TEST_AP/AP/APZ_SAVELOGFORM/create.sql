create procedure APZ_SaveLogForm()
as
begin			
		create local temporary table #dtAPZ_LogDrafReportData as (select top 0 * from APZ_LOGFORM);
		exec 'drop table #dtAPZ_LogDrafReportData;';
		exec 'create local temporary table #dtAPZ_LogDrafReportData as (select * from "#TEMPTB");';		
		
		delete from APZ_LOGFORM where ("FormId","LogType","UserCode") in (select "FormId","LogType","UserCode" from #dtAPZ_LogDrafReportData);
		
		insert into APZ_LOGFORM select * from #dtAPZ_LogDrafReportData;
		drop table #dtAPZ_LogDrafReportData;
		select 1 from DUMMY;	
end



