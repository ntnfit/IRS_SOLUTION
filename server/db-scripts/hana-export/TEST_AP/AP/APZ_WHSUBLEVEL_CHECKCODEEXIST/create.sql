
CREATE PROCEDURE APZ_WhSublevel_CheckCodeExist
AS
BEGIN
	create local temporary table #tempTbl("FldAbs" int, "SLCode" nvarchar(50), "Error" nvarchar(50));
	exec 'drop table #tempTbl';
	exec 'create local temporary table #tempTbl as (select * from "#dtOBSL");';
	update a set "Error" = 'Sublevel code already exists' from #tempTbl a inner join OBSL b on a."FldAbs" = b."FldAbs" and a."SLCode" = b."SLCode";
	select * from #tempTbl order by "SLCode";
END



