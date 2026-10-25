
CREATE PROCEDURE Apz_Report_GetFileNameReport (
	IN 	Code NVARCHAR(254)
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	SELECT "FileName" 
		FROM APZ_PRINTLAYOUT 
		WHERE "TypeName" = n'User Reports' 
			AND "FileName" IS NOT NULL 
			AND "FileName" <> '' 
			AND "DocCode" = :Code;

END;




