
CREATE PROCEDURE SP_APZ_UGP1_SAVE
AS
BEGIN
	create local temporary table #temp_UGP1 as (select top 0 * from UGP1);
	execute immediate 'insert into #temp_UGP1 select * from #UGP1';
	DELETE FROM UGP1 WHERE "UgpEntry" IN (SELECT "UgpEntry" FROM #temp_UGP1);
	INSERT INTO UGP1 ("UgpEntry", "UomEntry", "AltQty", "BaseQty", "LogInstanc", "LineNum", "WghtFactor", "UdfFactor") 
	(SELECT a."UgpEntry", a."UomEntry", a."AltQty", a."BaseQty", a."LogInstanc", a."LineNum", a."WghtFactor", a."UdfFactor" 
	FROM #temp_UGP1 a 
	WHERE a."UgpEntry" <> '0' AND a."UomEntry" <> '0' AND a."LineNum" <> '0');
END






