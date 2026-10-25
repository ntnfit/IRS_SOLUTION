
CREATE PROCEDURE APZ_SalesOpportunity_GetListPartner
AS
BEGIN
	SELECT IFNULL("PrtId", -1) AS "PrtId", IFNULL("Name", '') AS "Name", IFNULL("RelatnType", '') AS "RelatnType", IFNULL("RelatCard", '') AS "RelatCard", IFNULL("Memo", '') AS "Memo"
			, IFNULL("DataSource", 'N') AS "DataSource", IFNULL("UserSign", 3) AS "UserSign", IFNULL("DefaultORL", 0) AS "DefaultORL" FROM OPRT;
END;



