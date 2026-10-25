
CALL APZ_EXISTSPROC('Apz_Commons_FormulaSearch');

GO

CREATE PROCEDURE Apz_Commons_FormulaSearch (
	IN FieldName NVARCHAR(50)
,	   TableName NVARCHAR(50)
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN		
	SELECT REPLACE("U_Formular", '(newline)', CHAR(13) || CHAR(10)) AS "Formula", "U_FormularType" AS "FormulaType", "U_DependTable" AS "DependTable", "U_DependField" AS "DependField","U_FieldName" "FieldName" 
		FROM "@FORMULAR" 
		WHERE "U_FormularType" IN ('CBX','CFL','MSCFL','LKO','LKU') 
			AND (ifnull(:FieldName, '') = '' or "U_FieldName" = :FieldName)
			AND "U_TableName" = :TableName;
END;