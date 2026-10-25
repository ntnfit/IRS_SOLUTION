
CREATE PROCEDURE APZ_Integrated_GetListConfig
as 
BEGIN
	declare checkCount int = 0;
	select IFNULL((SELECT 1 FROM schemas WHERE SCHEMA_NAME = N'ASYNC'  OR SCHEMA_NAME =  N'ASYNC' ),0) into checkCount from DUMMY;
	IF checkCount = 1
	then
	
		checkCount = 0;
		select IFNULL((SELECT 1 FROM  TABLES WHERE SCHEMA_NAME = current_schema  AND TABLE_NAME = N'CONF'), 0) into checkCount from DUMMY;
		IF checkCount = 1
		then
			execute IMMEDIATE 'Select * From ASYNC.CONF Where "Valid"=''Y''';
		END if;
	
	END if;
END




