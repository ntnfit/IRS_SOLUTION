
CALL APZ_EXISTSPROC('Apz_Formular_Get');

GO

CREATE PROCEDURE Apz_Formular_Get (IN ObjType NVARCHAR(100))
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
BEGIN
	DECLARE tbl TABLE
	(
		 "Code" nvarchar(50),
		 "Name" nvarchar(50),
		 "Description" nvarchar(254),
		 "ObjType" nvarchar(100),
		 "TableName" nvarchar(100),
		 "FieldName" nvarchar(20),
		 "FieldType" nvarchar(10),
		 "FormularType" nvarchar(10),
		 "DependTable" nvarchar(100),
		 "DependField" nvarchar(1024),
		 "FormularString" nclob
	 );
	 
	 INSERT INTO :tbl ("Code", "Name", "Description", "ObjType", "TableName", "FieldName", "FieldType", "FormularType", "DependTable", "DependField", "FormularString") 
		SELECT "Code", "Name", "U_Description", "U_ObjType", "U_TableName", "U_FieldName", "U_FieldType", "U_FormularType", "U_DependTable", "U_DependField", "U_Formular" 
		FROM "@FORMULAR" 
		WHERE 
			"U_ObjType" = :ObjType 
			AND "U_FormularType" 
			NOT IN ('CFL','CBX', 'MSCFL');
			
	SELECT * FROM :tbl;
END;