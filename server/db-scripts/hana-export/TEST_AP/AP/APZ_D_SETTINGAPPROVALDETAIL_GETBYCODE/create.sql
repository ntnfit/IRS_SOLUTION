
Create PROCEDURE APZ_D_SettingApprovalDetail_GetByCode(in code nvarchar(50))
AS
BEGIN
	select * from apz_D_OSAD where "ApprovalObjectType"=:code;
end



