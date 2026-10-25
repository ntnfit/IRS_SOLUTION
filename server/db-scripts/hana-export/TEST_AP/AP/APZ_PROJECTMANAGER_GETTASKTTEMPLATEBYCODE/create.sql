
CREATE PROCEDURE APZ_ProjectManager_GetTaskTtemplateByCode(in code nvarchar(50) default '')
AS
BEGIN
select *  from APZ_TASK where  (ifnull(:code, '') = '' or "Code"=:code);
END



