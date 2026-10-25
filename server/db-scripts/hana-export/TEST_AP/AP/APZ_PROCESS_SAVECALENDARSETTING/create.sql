
CREATE PROCEDURE apz_process_SaveCalendarSetting
AS
BEGIN
	declare UserSign nvarchar(20);
	declare maxCode integer;
	
	create local temporary table #temp_OACS as (select top 0 * from APZ_OACS);
	execute immediate 'insert into #temp_OACS select * from #APZ_OACS';
	
	create local temporary table #temp_ACS1 as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_ACS1 b ON 1=1);
	execute immediate 'insert into #temp_ACS1 select * from #APZ_ACS1';
	
	
select IFNULL(NULLIF((SELECT TOP 1 "UserSign" FROM #temp_ACS1), 'manager'), '') into UserSign from DUMMY;
DELETE FROM APZ_OACS WHERE "UserSign" = :UserSign;
UPDATE #temp_OACS SET "Color" = -1 WHERE "Color" IS NULL OR "Color" = 0;
UPDATE #temp_OACS SET "UserSign" = :UserSign;
INSERT INTO APZ_OACS (SELECT * FROM #temp_OACS);
DELETE FROM APZ_ACS1 WHERE "UserSign" = :UserSign;

select IFNULL((SELECT MAX("Code") FROM APZ_ACS1), 0) into MaxCode from DUMMY;
UPDATE #temp_ACS1 SET "Code" = :maxCode + 1 WHERE "Code" = 0 OR "Code" IS NULL;
UPDATE #temp_ACS1 SET "UserSign" = :UserSign WHERE "UserSign" IS NULL;
INSERT INTO APZ_ACS1 (SELECT * FROM #temp_ACS1);

END



