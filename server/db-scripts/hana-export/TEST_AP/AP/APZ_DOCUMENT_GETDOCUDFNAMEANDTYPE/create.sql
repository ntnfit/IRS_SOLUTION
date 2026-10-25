
CREATE procedure apz_document_GetDocUDFNameAndType
as
BEGIN
	SELECT COLUMN_NAME,DATA_TYPE_NAME DATA_TYPE FROM TABLE_COLUMNS WHERE schema_name = current_Schema and TABLE_NAME = 'ORDR' AND LEFT(COLUMN_NAME,2)= 'U_';
END;




