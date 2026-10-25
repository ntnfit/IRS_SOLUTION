
Create PROCEDURE APZ_D_SettingApprovalDetail_GetLineById(in Id int default 0)
AS
BEGIN
	select * from APZ_D_SAD2 where "DocEntry"=:Id;
end



