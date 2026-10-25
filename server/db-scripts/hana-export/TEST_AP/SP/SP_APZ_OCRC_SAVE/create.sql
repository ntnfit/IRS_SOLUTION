
CREATE PROCEDURE SP_APZ_OCRC_SAVE
AS
BEGIN
create local temporary table #temp_OCRC as (select top 0 * from OCRC);
execute immediate 'insert into #temp_OCRC select * from #OCRC';
	DELETE FROM OCRC WHERE "CreditCard" NOT IN (SELECT "CreditCard" FROM OCRP);
INSERT INTO OCRC ("CreditCard", "CardName", "AcctCode", "Phone", "CompanyId", "Locked", "DataSource", "UserSign", "LogInstanc", "UpdateDate", "IntTaxCode", "UserSign2", "Country") 
(SELECT a."CreditCard", a."CardName", a."AcctCode", a."Phone", a."CompanyId", a."Locked", a."DataSource", a."UserSign", a."LogInstanc", a."UpdateDate", a."IntTaxCode", a."UserSign2", a."Country" FROM #temp_OCRC a WHERE a."CreditCard" <> '0' AND a."CardName" <> '');
--DELETE dbo.UGP1 WHERE UgpEntry NOT IN (SELECT a.UgpEntry FROM OUGP a)
UPDATE ONNM SET "AutoKey" = (SELECT MAX("CreditCard") FROM OCRC) + 1 WHERE "ObjectCode" = '36';
END




