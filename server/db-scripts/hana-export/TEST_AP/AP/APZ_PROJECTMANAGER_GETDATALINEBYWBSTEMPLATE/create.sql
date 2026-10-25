
Create PROCEDURE APZ_ProjectManager_GetDataLineByWbsTemplate(in code nvarchar(50) default '')
AS
BEGIN
	select *  from APZ_WBS1 where "Code"=:code;
-- and [Object]='APZ_OWBS'
	select *  from APZ_TASK a where a."Code"=:code;
	 --and a.[Object]='APZ_OWBS'
end




