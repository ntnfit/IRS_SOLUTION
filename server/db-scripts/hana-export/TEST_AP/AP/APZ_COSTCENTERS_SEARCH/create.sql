
CREATE PROCEDURE APZ_CostCenters_Search
(
PrcCode nvarchar(50) default '',
PrcName nvarchar(50) default '',
GrpCode nvarchar(50) default '',
DimCode int default 0,
CCTypeCode nvarchar(50) default '',
ValidFrom TimeStamp default '',
ValidTo TimeStamp default '',
Active char(1) default ''
)
as
BEGIN
	SELECT * FROM OPRC WHERE 
	((:PrcCode <> '' AND "PrcCode" LIKE '%' || :PrcCode || '%') OR (:PrcCode = '')) 
	AND ((:PrcName <> '' AND "PrcName" LIKE '%' || :PrcName || '%') OR (:PrcName = '')) 
	AND ((:GrpCode <> '' AND "GrpCode" LIKE '%' || :GrpCode || '%') OR (:GrpCode = '')) 
	AND ((:DimCode > 0 AND "DimCode" = :DimCode) OR (:DimCode <= 0)) 
	AND ((ifnull(:CCTypeCode,'') <> '' AND "CCTypeCode" = :CCTypeCode) OR (ifnull(:CCTypeCode,'') = '')) 
	AND ((YEAR(:ValidFrom) > 1990 AND "ValidFrom" >= :ValidFrom) OR (YEAR(:ValidFrom) <= 1990)) 
	AND ((YEAR(:ValidTo) > 1990 AND "ValidTo" >= :ValidTo) OR (YEAR(:ValidTo) <= 1990)) 
	AND ((:Active <> '' AND "Active" = :Active) OR (:Active = '')) ORDER BY "PrcCode";
END;




