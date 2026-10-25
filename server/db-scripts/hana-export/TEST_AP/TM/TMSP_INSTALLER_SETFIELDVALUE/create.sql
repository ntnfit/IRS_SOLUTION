-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

create procedure TmSp_Installer_SetFieldValue (
	in tableName nvarchar (128),
	in field nvarchar (128),
	in fieldValue nvarchar (128))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS

sqlStr nvarchar (512);
exists1 integer;

begin

--Parameters validation for security issues
	call _TmSp_ValidateSpParam(:tableName);
	call _TmSp_ValidateSpParam(:field);
	call _TmSp_ValidateSpParam(:fieldValue);

	select record_count into exists1 from "PUBLIC".M_TABLES
	where table_name = :tableName and schema_name = CURRENT_SCHEMA;
	
	if exists1 <> 0 then
		exec(' update ' || :tableName ||' set "'|| :field||'" = ''' || :fieldValue ||''' ');
	else
		exec(' insert into '|| :tableName || '("' || :field ||'") values(''' || :fieldValue ||''') ');
	end if;
		
end;


