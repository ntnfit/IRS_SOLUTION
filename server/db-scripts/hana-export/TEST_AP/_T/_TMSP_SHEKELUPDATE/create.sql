-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE procedure _TmSp_ShekelUpdate
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
----------------------------------------------------------------
tableName nvarchar(256);

-- this file should be removed but because of some bugs in TA 
-- we should keep the file just remove content
begin
	tableName := 'test';
end;


