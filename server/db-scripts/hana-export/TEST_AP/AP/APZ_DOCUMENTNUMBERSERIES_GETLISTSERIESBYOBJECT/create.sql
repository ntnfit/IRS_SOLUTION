
CREATE  PROCEDURE APZ_DocumentNumberSeries_GetListSeriesByObject(
in ObjectCode nvarchar(30) default '',
in BPLId int,
in DocSubType nvarchar(2) default '--')
as
BEGIN
	DECLARE MultiBranch NVARCHAR(1);
	select IFNULL((SELECT TOP 1 "MltpBrnchs" FROM OADM), 'N') into MultiBranch from DUMMY;
	SELECT ifnull("SeriesName", '') AS "SeriesName", ifnull("Series", 0) AS "Series"
			, ifnull("NextNumber", 0) AS "NextNumber"
			, ifnull("LastNum", 0) AS "LastNum"
			, ifnull("BeginStr", '') AS "BeginStr"
			, ifnull("EndStr", '') AS "EndStr"
			, ifnull("NumSize", 0) AS "NumSize", "IsManual", "SeriesType" 
		FROM nnm1 WHERE "ObjectCode" = :ObjectCode AND "DocSubType" = :DocSubType AND (:MultiBranch = 'N' OR (:MultiBranch = 'Y' AND ("BPLId" = :BPLId OR "BPLId" IS NULL)));

END




