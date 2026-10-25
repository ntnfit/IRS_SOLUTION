

CREATE PROCEDURE APZ_D_ProjectManager_ListTrackingById (projectEntry int DEFAULT 0)
AS
BEGIN
	SELECT a.*, f."LinkThumbnails", c."Color" AS "ColorStatus", c."Name" AS "StatusName", 
	IFNULL(d."firstName", '') || ' ' || IFNULL(d."middleName", '') || ' ' || IFNULL(d."lastName", '') AS "ReporterName", 
	IFNULL(e."firstName", '') || ' ' || IFNULL(e."middleName", '') || ' ' || IFNULL(e."lastName", '') AS "HandlerName", 
	d."firstName" AS "RepFName", d."lastName" AS "RepLName" 
	FROM APZ_TGIE a LEFT OUTER JOIN APZ_OTIS c ON CAST(a."Status" AS INT) = c."Code" 
	LEFT OUTER JOIN OHEM d ON a."Reporter" = d."empID" 
	LEFT OUTER JOIN OHEM e ON a."HandleBy" = e."empID" 
	LEFT OUTER JOIN APZ_D_UUPIM f ON d."userId" = f."UserSign" AND F."ObjectType" = '1470000171' 
	WHERE "ProjectCode" = :projectEntry;
END



