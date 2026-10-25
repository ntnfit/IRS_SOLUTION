
CREATE PROCEDURE APZ_KPIGroup_GetLinesIndicatorByCode
(
	in code NVARCHAR(50),	
	in type NVARCHAR(10) default ''		-- M: nhập tay / F: Công thức
	)	
AS
BEGIN
	SELECT a.*, b."Name" "IndicatorName"
		FROM APZ_KGP2 a
			LEFT JOIN APZ_OKIR b ON b."Code" = a."IndicatorCode"
			WHERE a."Code" = :code
			AND (IFNULL(:type,'') = '' OR (:type<>'' AND b."Type" = :type))
			ORDER BY a."LineId";
END




