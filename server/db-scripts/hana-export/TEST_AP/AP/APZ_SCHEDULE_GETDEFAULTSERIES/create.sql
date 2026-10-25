

CREATE PROCEDURE APZ_Schedule_GetDefaultSeries(in branch int)
AS
BEGIN
	SELECT IFNULL("Series", 0) AS "Series" FROM nnm1 WHERE "ObjectCode" = '202' AND "BPLId" = :branch;
END



