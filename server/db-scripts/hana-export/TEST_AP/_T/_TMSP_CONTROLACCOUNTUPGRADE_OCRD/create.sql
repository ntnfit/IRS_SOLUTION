-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE procedure _TmSp_ControlAccountUpgrade_OCRD
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
begin
----------------------------------------------------------------
EXEC ('UPDATE OCRD SET "DebPayAcct" = JDT1."Account" '
	||'FROM OCRD '
	||'INNER  JOIN JDT1 ON JDT1."ShortName" = OCRD."CardCode" '  
	||'WHERE OCRD."DebPayAcct" IS NULL OR OCRD."DebPayAcct" = ''''');
end;


