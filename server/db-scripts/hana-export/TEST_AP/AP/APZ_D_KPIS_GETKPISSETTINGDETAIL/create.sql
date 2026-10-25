
CREATE PROCEDURE Apz_D_KPIs_GetKPIsSettingDetail(in absEntry int)
AS
BEGIN
	
	SELECT a.*, b."Name" AS "Condition_Name", c."Name" FROM APZ_D_KPM1 a INNER JOIN APZ_D_OPCS b ON a."Condition_Code" = b."Code" LEFT OUTER JOIN APZ_D_OKPI c ON a."KPI_Code" = c."Code" WHERE "AbsEntry" = :absEntry;

END




