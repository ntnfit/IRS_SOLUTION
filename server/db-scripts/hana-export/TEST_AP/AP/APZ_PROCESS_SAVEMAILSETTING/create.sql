
CREATE PROCEDURE apz_process_SaveMailSetting
AS
BEGIN
declare Code integer;
declare maxCode integer;
create local temporary table #temp_OEMC as (select top 0 * from APZ_OEMC);
execute immediate 'insert into #temp_OEMC select * from #APZ_OEMC';
select IFNULL((SELECT TOP 1 "Code" FROM #temp_OEMC), 0) into Code from DUMMY;
DELETE FROM APZ_OEMC;

select  IFNULL((SELECT MAX("Code") FROM APZ_OEMC), 0) into MaxCode from DUMMY;
UPDATE #temp_OEMC SET "Code" = :maxCode + 1 WHERE "Code" = 0 OR "Code" IS NULL;
INSERT INTO APZ_OEMC (SELECT * FROM #temp_OEMC);
	
END




