-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE procedure TmSp_Replace_Table(IN SourceDB     nvarchar(128),
									IN SourceTable  nvarchar(128),
									IN UtilityDB    nvarchar(128),
									IN UtilityTable nvarchar(128)) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as
subQuery nvarchar(1000);
begin

call _TmSp_ValidateSpParam (:SourceDB);
call _TmSp_ValidateSpParam (:SourceTable);
call _TmSp_ValidateSpParam (:UtilityDB);
call _TmSp_ValidateSpParam (:UtilityTable);

exec('delete from ' || SourceDB  ||'.' || SourceTable);

subQuery := '(select * from  ' || UtilityDB  || '.' || UtilityTable || ')';

exec('insert into ' || SourceDB  || '.' || SourceTable || subQuery);

end;


