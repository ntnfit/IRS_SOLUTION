
--CALL apz_ItemGroups_CheckExistsTrans (100)
CREATE PROCEDURE apz_ItemGroups_CheckExistsTrans
(ItmsGrpCod int default 0)
AS
BEGIN
declare IsExists nvarchar(10)='';
--tạo bảng có cột tên TblName và type là nvarchar(10)
create local temporary table #tbl as (SELECT TOP 0 CAST('' AS nvarchar(10)) AS TblName FROM DUMMY);
	 --delete from #tbl;
	 insert into #tbl select TOP 1 'OITM' as TblName FROM OITM where "ItmsGrpCod" = :ItmsGrpCod 
	 UNION SELECT TOP 1 'OBIN' as TblName FROM OBIN WHERE "SpcItmGrpC" = :ItmsGrpCod
	 UNION SELECT TOP 1 'OAT1' as TblName FROM OAT1 WHERE "ItemGroup" = :ItmsGrpCod
	 UNION SELECT TOP 1 'OINS' as TblName FROM OINS WHERE "itemGroup" = :ItmsGrpCod
	 UNION SELECT TOP 1 'OGAR' as TblName FROM OGAR WHERE "ItmsGrpCod" = :ItmsGrpCod
	 UNION SELECT TOP 1 'GPA2' as TblName FROM GPA2 WHERE "ItmsGrpCod" = :ItmsGrpCod
	 UNION SELECT TOP 1 'IQI1' as TblName FROM IQI1 WHERE "ItmsGrpCod" = :ItmsGrpCod
	 UNION SELECT TOP 1 'IOD1' as TblName FROM IOD1 WHERE "ItmsGrpCod" = :ItmsGrpCod
	UNION SELECT TOP 1 'OBIN' as TblName FROM OBIN WHERE "SpcItmGrpC" = :ItmsGrpCod
	UNION SELECT TOP 1 'OMSN' as TblName FROM OMSN WHERE "ItmGrp" = :ItmsGrpCod
	UNION SELECT TOP 1 'OFLT' as TblName FROM OFLT WHERE "ItmGroup" = :ItmsGrpCod
	UNION SELECT TOP 1 'OSCL' as TblName FROM OSCL WHERE "itemGroup" = :ItmsGrpCod
	UNION SELECT TOP 1 'CTR1' as TblName FROM CTR1 WHERE "ItemGroup" = :ItmsGrpCod;
	
	select Top 1 TblName into IsExists from #tbl;
	drop table #tbl;
if	IsExists <> ''
then
	select :IsExists from DUMMY;
	end if;
	END
	




