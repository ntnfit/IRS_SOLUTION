-- B1 DEPENDS: BEFORE:PT:PROCESS_START

create procedure _TmSp_AlterTaskColumnAtPMG1
language sqlscript
SQL SECURITY INVOKER
as

cnt int;
begin
	cnt := 0;

	select count(1) into cnt from CINF where "Version" = 920003;
	
	if (:cnt = 1)
	then
		exec('update PMG1 set "Task" = NULL where "Task" = '''' ');
		exec('alter Table PMG1 alter ("Task" nvarchar(1) NULL DEFAULT NULL)');
	end if;
end;


