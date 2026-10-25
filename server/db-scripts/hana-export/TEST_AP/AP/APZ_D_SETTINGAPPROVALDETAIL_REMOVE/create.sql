
Create PROCEDURE APZ_D_SettingApprovalDetail_Remove(in Id int default 0)
AS
BEGIN
	Delete from APZ_D_OSAD where "DocEntry"=:Id;
	Delete from APZ_D_SAD1 where "DocEntry"=:Id;
	Delete from APZ_D_SAD2 where "DocEntry"=:Id;
	select 0, '' from dummy;
end



