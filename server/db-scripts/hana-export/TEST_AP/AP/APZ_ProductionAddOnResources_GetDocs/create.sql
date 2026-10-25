
CREATE PROCEDURE "APZ_ProductionAddOnResources_GetDocs" 
AS 
BEGIN 
	SELECT a."ResCode", 
		IFNULL(a."ResName", '') AS "ResName", 
		IFNULL(a."ResType", 'O') AS "ResType", 
		IFNULL(a."ResGrpCod", -1) AS "ResGrpCod", 
		IFNULL(a."ValidComm", 'Y') AS "Status", 
		IFNULL(a."validFrom", '') AS "StartTime", 
		IFNULL(a."validTo", '') AS "FinishTime", 
		IFNULL(a."CodeBars", '') AS "CodeBars", 
		IFNULL(a."UnitOfMsr", '') AS "UnitOfMsr", 
		IFNULL(a."UserText", '') AS "Remark" 
	FROM ORSC A LEFT OUTER JOIN RSC6 B ON A."ResCode" = B."ResCode";
END;

