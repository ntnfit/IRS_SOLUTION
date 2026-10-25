
CREATE  PROCEDURE Util_AdvanceReportTemplate_GetColumnLinkAccount
AS
BEGIN
	create local temporary table #tbl as (select top 0 *, cast(null as nvarchar(254)) "Description" from APZ_ARTA);
	insert into #tbl ("RepCode", "IndCode") values ('','');
	select * from #tbl;
	drop table #tbl;
END



