
CALL APZ_EXISTSPROC('APZ_Capacity_GetDataResourceEffic');

GO

CREATE PROCEDURE APZ_Capacity_GetDataResourceEffic
(
	In resCode nvarchar(50) default '',
	In routCode nvarchar(50) default '',
	In routLine int default -1
)
AS
BEGIN

	SELECT a."Effic", b."InvQty", CASE WHEN b."UomSetTime" = 'M' THEN IFNULL(b."SetTime", 0) ELSE 60 * IFNULL(b."SetTime", 0) END 
						+ CASE WHEN b."UomRunTime" = 'M' THEN IFNULL(b."RunTime", 0) ELSE 60 * IFNULL(b."RunTime", 0) END 
						+ CASE WHEN b."UomTransTime" = 'M' THEN IFNULL(b."TransTime", 0) ELSE 60 * IFNULL(b."TransTime", 0) END AS "BaseTime" 
	FROM APZ_RTA2 a INNER JOIN APZ_RTA1 b ON a."RoutCode" = b."RoutCode" AND a."FatherLine" = b."LineNum" 
	WHERE a."RoutCode" = :routCode and a."ResCode" = :resCode and a."FatherLine" = :routLine;
 	
END