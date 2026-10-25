
Create PROCEDURE APZ_D_PromotionBenefit_Remove(in code nvarchar(50) default '')
AS
BEGIN
	Delete from APZ_D_OPBF where "Code"=:code;
	Delete from APZ_D_PBF1 where "Code"=:code;
	Delete from APZ_D_PBF2 where "Code"=:code;
	Delete from APZ_D_PBF3 where "Code"=:code;
	select 0, '' from DUMMY;
end




