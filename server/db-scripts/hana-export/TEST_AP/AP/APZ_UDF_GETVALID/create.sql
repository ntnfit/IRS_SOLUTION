
CREATE PROCEDURE apz_udf_getvalid(
TableId NVARCHAR(10),
FieldId NVARCHAR(50))
AS
BEGIN
IF :TableId LIKE 'APZ_%' 
THEN 
	SELECT "FldValue" AS "Code", a."Descr" AS "Name" FROM APZ_UFD1 a LEFT OUTER JOIN cufd b ON a."TableID" = b."TableID" AND a."FieldID" = b."FieldID" WHERE a."TableID" = :TableId AND b."AliasID" = :FieldId;
ELSE 
	SELECT "FldValue" AS "Code", a."Descr" AS "Name" FROM UFD1 a LEFT OUTER JOIN cufd b ON a."TableID" = b."TableID" AND a."FieldID" = b."FieldID" WHERE a."TableID" = :TableId AND b."AliasID" = :FieldId;
END IF;
END




