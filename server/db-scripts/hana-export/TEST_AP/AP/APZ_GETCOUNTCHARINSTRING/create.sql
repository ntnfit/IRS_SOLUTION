
CREATE FUNCTION APZ_GetCountCharInString(
	RowData	nvarchar(5000),
	CharData nvarchar(50)
)
RETURNS result int
AS
BEGIN
	
	result = (LENGTH(:RowData) - LENGTH(REPLACE(:RowData, :CharData, '')))/ LENGTH(:CharData);
	
END




