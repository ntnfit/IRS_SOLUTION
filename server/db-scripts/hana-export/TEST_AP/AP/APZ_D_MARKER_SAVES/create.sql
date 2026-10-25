

CREATE PROCEDURE APZ_D_Marker_Saves
AS
BEGIN
	
	create local temporary table #APZ_D_MARKERLocal as (select top 0 * from APZ_D_MARKER);
	exec 'drop table #APZ_D_MARKERLocal;';
	exec 'create local temporary table #APZ_D_MARKERLocal as (select * from "#APZ_D_MARKER");';

	delete from  APZ_D_MARKER;
	insert into APZ_D_MARKER("Code", "Name", "SvgData", "DfltColorHex", "DfltSize") select * from #APZ_D_MARKERLocal;
	select 0, '' from dummy;

END




