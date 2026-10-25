
CREATE PROCEDURE APZ_SalesOpportunity_GetListCompetitor
AS
BEGIN
	SELECT IFNULL("CompetId", -1) AS "CompetId", IFNULL("Name", '') AS "Name", IFNULL("ThreatLevl", -1) AS "ThreatLevl", IFNULL("Memo", '') AS "Memo", IFNULL("DataSource", 'N') AS "DataSource"
	, IFNULL("UserSign", 3) AS "UserSign" FROM OCMT;
END;



