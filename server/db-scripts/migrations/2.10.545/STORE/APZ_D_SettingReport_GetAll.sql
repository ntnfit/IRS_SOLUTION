
CALL APZ_EXISTSPROC('APZ_D_SettingReport_GetAll');

GO


CREATE PROCEDURE APZ_D_SettingReport_GetAll (IN userCode nvarchar(50) DEFAULT '')
AS
BEGIN

	SELECT a.*, b."Name", b."StoreName" FROM apz_D_OSRT a 
	INNER JOIN APZ_OUDR b ON b."Code" = a."ReportId" 
	INNER JOIN OPRM C ON C.USER_CODE = :userCode AND REPLACE(c."menucode", 'frm_', '') = a."ReportId" 
	AND c."authoriza" = 'F' WHERE a."Active" = 'Y' order by "ReportName" asc;

END