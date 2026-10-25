-- B1 DEPENDS: BEFORE:PT:PROCESS_START

create procedure _TmSp_AlterCONTACTColumnAtOPMG
language sqlscript
SQL SECURITY INVOKER
as

cnt int;
begin
	cnt := 0;

	select count(1) into cnt from CINF where "Version" >= 920000 and "Version" < 920130;
	
	if (:cnt = 1)
	then
		exec('update OPMG set "CONTACT" = NULL where "CONTACT" = '''' ');
		exec('alter Table OPMG alter ("CONTACT" NVARCHAR(90) NULL DEFAULT NULL)');
	end if;
end;


