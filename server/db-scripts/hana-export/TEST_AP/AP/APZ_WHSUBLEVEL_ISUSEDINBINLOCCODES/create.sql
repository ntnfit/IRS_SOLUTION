
CREATE PROCEDURE APZ_WhSublevel_IsUsedInBinLocCodes
AS
BEGIN
	declare query nvarchar(200);
	create local temporary table #tempTbl("FldAbs" int, "SLCode" nvarchar(50), "Error" nvarchar(50));
	exec 'drop table #tempTbl';
	exec 'create local temporary table #tempTbl as (select * from "#dtOBSL");';
	query := 'update a set "Error" = ''Sublevel code is already in use'' from #tempTbl a inner join OBIN b on
	b."SL' || (select top 1 cast("FldAbs" as nvarchar) from #tempTbl)  || 'Code" = a."SLCode";';
	exec(:query);
	select * from #tempTbl order by "SLCode";
END



