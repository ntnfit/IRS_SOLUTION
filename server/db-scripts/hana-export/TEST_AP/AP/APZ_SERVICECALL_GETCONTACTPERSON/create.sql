
CREATE  PROCEDURE APZ_ServiceCall_GetContactPerson(in cardCode nvarchar(15) default null)
AS
BEGIN
	DECLARE SqlString NVARCHAR(400000); 
	sqlString = 'select "CardCode","Name" as "CntctName","CntctCode","Tel1","E_MailL" as "E_MaiL" from ocpr A where 1 = 1';
	IF :cardCode is not null and :cardCode <> ''
	THEN
		sqlString = :sqlString || ' And A."CardCode"=N''' || :cardCode || '''';
	END IF;
	sqlString = :sqlString || ';';
	--PRINT @SQL
	EXECUTE immediate :sqlString;
END




