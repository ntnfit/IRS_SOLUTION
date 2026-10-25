

CREATE PROCEDURE Util_AdvanceReportTemplete_GetDataLine(in repCode nvarchar(20) default '')
AS
BEGIN

	create local temporary table #tempIndi as(select top 0 *,"Color" "Color1", "BColor" "BColor1", 'Y' "IsExists" from APZ_ARTI);
	
	insert into #tempIndi
		select *,"Color" "Color1", "BColor" "BColor1", 'Y' "IsExists" from APZ_ARTI where "RepCode" = :repCode order by "VisOrder";

	select * from #tempIndi order by  "TCode";

	select a.* from APZ_ARTA a inner join #tempIndi b ON a."IndCode" = b."IndCode" and a."RepCode" = b."RepCode" order by a."TCode";
END



