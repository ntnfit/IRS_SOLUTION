
CREATE PROCEDURE APZ_AdvanceForecast_GetColumnLine
AS
BEGIN
	create local temporary table #tblLine as (select top 0 *, cast('' as nvarchar(254)) as "ItemName", cast(null as nvarchar(50)) as "InvntryUom" from APZ_FCT1);
	insert into #tblLine ("Code", "LineID", "ItemCode", "Date", "Quantity", "WhsCode", "ItemName")
	values
	(0,0,'','',0,'', '');
	
	select * from #tblLine;
	drop table #tblLine;
END




