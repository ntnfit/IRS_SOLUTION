

Create PROCEDURE APZ_D_SetIndicatorKPIs_LoadData(in code nvarchar(50) default '',in name nvarchar(254) default '')
AS
BEGIN
	select * from APZ_D_KPIS where (ifnull(:code,'') ='' or "Code"=:code) and	(ifnull(:name,'') ='' or "Name"=:name);
end




