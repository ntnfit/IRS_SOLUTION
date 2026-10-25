
CREATE PROCEDURE SP_APZ_OCRP_SAVE
AS
BEGIN
	 create local temporary table #temp_OCRP as (select top 0 * from OCRP);
	 execute immediate 'insert into #temp_OCRP select * from #OCRP';
	DELETE FROM OCRP;
INSERT INTO OCRP ("CrTypeCode", "CrTypeName", "CreditCard", "DueTerms", "MinCredit", "MinToPay", "MaxValid", "InstalMent", "Locked", "DataSource", "UserSign") 
(SELECT a."CrTypeCode", a."CrTypeName", a."CreditCard", a."DueTerms", a."MinCredit", a."MinToPay", a."MaxValid", a."InstalMent", a."Locked", a."DataSource", a."UserSign" FROM #temp_OCRP a WHERE a."CrTypeCode" <> '0' AND a."CrTypeName" <> '' AND a."CreditCard" <> '0' AND a."DueTerms" <> '');
UPDATE ONNM SET "AutoKey" = (SELECT MAX("CrTypeCode") FROM OCRP) + 1 WHERE "ObjectCode" = '70';

END




