
CREATE PROCEDURE Apz_Commons_FormulaSearch_ByTable (
	IN 	TableName NVARCHAR(50)
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	SELECT "U_FieldName" AS "FieldName", "U_TableName" AS "TableName", "U_Formular" AS "Formula", "U_FormularType" AS "FormulaType", "U_DependTable" AS "DependTable", "U_DependField" AS "DependField" 
		FROM "@FORMULAR" 
		WHERE "U_FormularType" IN ('CBX','CFL','MSCFL') 
			AND "U_TableName" = :TableName 
			AND SUBSTRING("U_FieldName", 1, 2) <> 'U_';
	
END;

