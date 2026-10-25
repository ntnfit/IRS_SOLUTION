-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE procedure _TmSp_SetCategoryAtOFPR
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS

exists1 int;
exists2 int;
begin

	select count(*) into exists1 from "PUBLIC".objects
	where object_name = 'OFPR' and object_type = 'TABLE' and schema_name = CURRENT_SCHEMA;

	select count(*) into exists2 from "PUBLIC"."TABLE_COLUMNS" c
	inner Join "PUBLIC".objects o on (o.object_oid = c.table_oid
							 and o.schema_name = CURRENT_SCHEMA
							 and c.schema_name = CURRENT_SCHEMA)
	where o.object_name = 'OFPR' And 
		  c.column_name = 'Category';
		  
	IF (:exists1 <> 0) and (:exists2 = 0)
	THEN
		exec('alter table OFPR add("Category9999" nvarchar(10))');
	END IF;

	exec('UPDATE OFPR SET "Category" = "AbsEntry" WHERE IFNULL("Category",'''') = ''''');

end;


