

CREATE PROCEDURE Apz_D_KPIs_GetKPIsSetting
AS
BEGIN
	
	SELECT a.*, IFNULL(b."firstName", '') || ', ' || IFNULL(b."lastName", '') AS "empName", b."jobTitle", f."name" AS "PositionName", d."Name" AS "Department", e."Name" AS "BranchName"
			, CASE WHEN b."manager" IS NOT NULL THEN IFNULL(c."firstName", '') || ', ' || IFNULL(c."lastName", '') ELSE '' END AS "ManagerName", b."ExtEmpNo" 
		FROM APZ_D_OKPM a LEFT OUTER JOIN OHEM b ON a."empID" = b."empID" 
						LEFT OUTER JOIN OHEM c ON b."manager" = c."empID" 
						LEFT OUTER JOIN OUDP d ON b."dept" = d."Code" 
						LEFT OUTER JOIN OUBR e ON b."branch" = e."Code" 
						LEFT OUTER JOIN OHPS f ON b."position" = f."posID";
END






