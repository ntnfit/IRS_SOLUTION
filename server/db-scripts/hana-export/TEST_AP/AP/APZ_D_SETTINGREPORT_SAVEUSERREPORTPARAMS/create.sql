Create PROCEDURE APZ_D_SettingReport_SaveUserReportParams()
AS
BEGIN
	create local temporary table #temp as (select top 0 * from APZ_D_SRT4);
	exec 'drop table #temp';
	exec 'create local temporary table #temp as (select * from "#APZ_D_SRT4")';
	delete from APZ_D_SRT4 where "DocEntry" = (select top 1 "DocEntry" from #temp) and "UserCode" = (select top 1 "UserCode" from #temp);
	insert into APZ_D_SRT4 ("DocEntry","UserCode","Name","Value","CreateDate","CreateTime") 
	select  "DocEntry","UserCode","Name","Value",Current_Date,Current_Time from #temp;
	select '200' as "MessageCode", '' as "Message" from dummy;
end



