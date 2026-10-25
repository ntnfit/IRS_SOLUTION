
CREATE PROCEDURE APZ_SalesOpportunity_GetListReason
AS
BEGIN
	SELECT IFNULL("Num", -1) AS "Num", IFNULL("Descript", '') AS "Descript", IFNULL("SortOrder", -1) AS "SortOrder", IFNULL("UserSign", -1) AS "UserSign" FROM OOFR;
END;



