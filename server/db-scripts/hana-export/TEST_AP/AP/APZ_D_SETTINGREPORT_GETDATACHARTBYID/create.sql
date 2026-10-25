
Create PROCEDURE APZ_D_SettingReport_GetDataChartById(in Id int default 0)
AS
BEGIN
	select * from APZ_D_SRT1 where "DocEntry"=:Id order by "OrderId";
end



