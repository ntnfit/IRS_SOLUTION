
CREATE procedure apz_document_GetDocLineUDFNameAndType
as
BEGIN
	SELECT COLUMN_NAME, DATA_TYPE_NAME DATA_TYPE FROM TABLE_COLUMNS WHERE Schema_name = current_Schema and TABLE_NAME = 'RDR1' AND  LEFT(COLUMN_NAME,2)= 'U_';
END




