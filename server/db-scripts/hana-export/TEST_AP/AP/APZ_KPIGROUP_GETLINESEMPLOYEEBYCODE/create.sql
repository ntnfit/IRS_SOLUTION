
CREATE PROCEDURE APZ_KPIGroup_GetLinesEmployeeByCode(in code NVARCHAR(50))
AS
BEGIN
	SELECT a.*	FROM APZ_KGP1 a	WHERE a."Code" = :code ORDER BY a."LineId";
END




