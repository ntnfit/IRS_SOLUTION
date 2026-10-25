
CREATE PROCEDURE APZ_SalesOpportunity_GetListStage(in OpprId int default 0)
AS
BEGIN

	SELECT IFNULL("Num", -1) AS "Num", IFNULL("Descript", '') AS "Descript", IFNULL("StepId", -1) AS "Step_Id", IFNULL("CloPrcnt", -1) AS "CloPrcnt", IFNULL("Canceled", 'N') AS "Canceled"
		, IFNULL("UserSign", 3) AS "UserSign", IFNULL("SalesStage", 'Y') AS "SalesStage", IFNULL("PurStage", 'N') AS "PurStage" FROM OOST order by "StepId";

END;




