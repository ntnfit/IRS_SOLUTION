
CREATE PROCEDURE APZ_SubActivities_RemoveEvent
(type int default 1)
AS
BEGIN
declare ClgCode integer;
	declare SeriesNum integer;
	
	create local temporary table #temp_Event as (select top 0 * from APZ_OCLG);
	execute immediate 'insert into #temp_Event select * from "#APZ_Event"';
	
	 select IFNULL((SELECT TOP 1 IFNULL("ClgCode", 0) FROM #temp_Event), 0) into ClgCode from DUMMY;
select IFNULL((SELECT TOP 1 IFNULL("SeriesNum", 0) FROM #temp_Event), 0) into SeriesNum from DUMMY;
--Remove All
IF (:type = 1) THEN IF (:SeriesNum > 0) THEN DELETE FROM APZ_OCLG WHERE "ClgCode" = :SeriesNum OR "SeriesNum" = :SeriesNum;
ELSE DELETE FROM APZ_OCLG WHERE "ClgCode" = :ClgCode OR "SeriesNum" = :ClgCode;
END IF;
--Remove Unchanged
ELSE IF (:SeriesNum > 0) THEN DELETE FROM APZ_OCLG WHERE "ClgCode" = :SeriesNum OR ("IsRemoved" = 'Y' AND "SeriesNum" = :SeriesNum);
UPDATE APZ_OCLG SET "SeriesNum" = NULL, "OrigDate" = NULL, "RecurrenceInfo" = NULL, "RecurPat" = 'N', "CntctType" = 0 WHERE "SeriesNum" = :SeriesNum;
ELSE DELETE FROM APZ_OCLG WHERE "ClgCode" = :ClgCode OR ("IsRemoved" = 'Y' AND "SeriesNum" = :ClgCode);
UPDATE APZ_OCLG SET "SeriesNum" = NULL, "OrigDate" = NULL, "RecurrenceInfo" = NULL, "RecurPat" = 'N', "CntctType" = 0 WHERE "SeriesNum" = :ClgCode;
END IF;
END IF;
	
END




