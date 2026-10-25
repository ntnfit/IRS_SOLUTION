
CREATE PROCEDURE APZ_AuthorToken_GetEmps
as
BEGIN
	declare query nvarchar(5000);
	declare checkUdf int;
	
	SELECT COUNT(*) into checkUdf FROM TABLE_COLUMNS WHERE column_name = N'U_OldCode' AND table_name = 'OHEM' and SCHEMA_NAME = CURRENT_SCHEMA;
	
	if(checkUdf>0)
	THEN
	query= N'SELECT U_OldCode,EmpId,ISNULL(firstName,'''') + '' '' + ISNULL(lastName,'''') + '' '' +ISNULL( middleName,'''') AS FullName FROM OHEM';
	ELSE
	query = N'SELECT EmpId,ISNULL(firstName,'''') + '' '' + ISNULL(lastName,'''') + '' '' +ISNULL( middleName,'''') AS FullName FROM OHEM';
	end if;
	execute immediate :query;
END;




