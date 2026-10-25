
CREATE PROCEDURE SP_APZ_OUGP_SAVE
AS
BEGIN
	 create local temporary table #temp_OUGP as (select top 0 * from OUGP);
	 execute immediate 'insert into #temp_OUGP select * from #OUGP';
	DELETE from OUGP;
	 
	INSERT INTO OUGP ("UgpEntry", "UgpCode", "UgpName", "BaseUom", "DataSource", "UserSign", "LogInstanc", "UserSign2", "UpdateDate", "CreateDate", "Locked") 
	(SELECT a."UgpEntry", a."UgpCode", a."UgpName", a."BaseUom", a."DataSource", a."UserSign", a."LogInstanc", a."UserSign2", a."UpdateDate", a."CreateDate", a."Locked" 
	FROM #temp_OUGP a WHERE a."UgpEntry" <> '0' AND a."UgpCode" <> '' AND a."UgpName" <> '' AND (SELECT COUNT( b."UgpEntry") FROM UGP1 b WHERE b."UgpEntry" = a."UgpEntry") >0);
DELETE FROM UGP1 WHERE "UgpEntry" NOT IN (SELECT a."UgpEntry" FROM OUGP a);
UPDATE ONNM SET "AutoKey" = (SELECT MAX("UgpEntry") FROM OUGP) + 1 WHERE "ObjectCode" = '10000197';

END




