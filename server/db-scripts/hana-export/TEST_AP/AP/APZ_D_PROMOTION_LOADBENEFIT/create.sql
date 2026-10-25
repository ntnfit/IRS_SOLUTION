
CREATE PROCEDURE APZ_D_Promotion_LoadBenefit(in Code nvarchar(100))
AS
BEGIN

	SELECT * FROM APZ_D_PMO2 WHERE "Code"=:Code;
END



