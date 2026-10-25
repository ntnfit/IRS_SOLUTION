
CREATE PROCEDURE Util_AdvanceReportTemplate_GetColumnListOfIndicator
AS
BEGIN
	create local temporary table #tbl as (select top 0 *,cast(null as int) "Color1", cast(null as int) "BColor1", cast(null as nvarchar(1)) "IsExists" from APZ_ARTI);
	insert into #tbl ("IndCode", "RepCode") values ('', '');
	select * from #tbl;
	drop table #tbl;
END



