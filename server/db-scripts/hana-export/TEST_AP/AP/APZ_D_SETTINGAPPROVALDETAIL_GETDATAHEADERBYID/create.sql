
Create PROCEDURE APZ_D_SettingApprovalDetail_GetDataHeaderById(in Id int default 0)
AS
BEGIN
	select * from APZ_D_SAD1 where "DocEntry"=:Id order by "OrderId";
end





