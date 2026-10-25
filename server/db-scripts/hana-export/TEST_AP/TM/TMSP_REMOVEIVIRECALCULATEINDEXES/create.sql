-- B1 DEPENDS: AFTER:PT:PROCESS_END

--It is about index migration, so we use follow codes instead.
CREATE   procedure TmSp_RemoveIVIRecalculateIndexes
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as

begin
EXEC('SELECT * FROM DUMMY');
end;


