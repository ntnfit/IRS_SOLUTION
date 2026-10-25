

Create PROCEDURE APZ_D_KpisMasterData_LoadData(in code nvarchar(50) default '',in name nvarchar(254) default '',in status char(1) default '')
AS
BEGIN
	select * from APZ_D_OKPI where (ifnull(:code,'') ='' or "Code" =:code) and
									(ifnull(:name, '') = '' or "Name"=:name) and
									(ifnull(:status,'') = '' or "StatusCode"=:status)
									order by "CreateDate","CreateTime"  Asc;
end




