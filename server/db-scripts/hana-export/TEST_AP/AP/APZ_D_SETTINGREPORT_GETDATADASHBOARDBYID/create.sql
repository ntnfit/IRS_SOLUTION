Create PROCEDURE APZ_D_SettingReport_GetDataDashboardById(
	in docEntry int default -1,
	in id int default 0
)
AS
BEGIN
	select * from APZ_D_SRT3 where ((:docEntry = -1) or "DocEntry"=:docEntry) and ((:id = -1) or "DashboardId" = :id) order by "DashboardId";
end



