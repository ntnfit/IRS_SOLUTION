
CREATE PROCEDURE Util_ReportTemplate_GetColumReport
AS
BEGIN
	create local temporary table #tempReport as (Select top 0 *, cast(null as char(1)) "IsExists" from APZ_ART);
	INSERT INTO #tempReport ("RepCode") values ('');
	select * from #tempReport;

	DROP TABLE #tempReport;
END



