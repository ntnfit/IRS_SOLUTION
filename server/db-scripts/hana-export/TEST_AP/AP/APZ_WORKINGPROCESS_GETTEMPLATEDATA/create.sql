

CREATE PROCEDURE APZ_WorkingProcess_GetTemplateData(in userId int, status char(1), requestType char(1) default '')
AS
BEGIN
	
	call APZ_Function_GetApprovalTemplateData(:userid, :status, :requestType, result);
	select * from :result;

END




