-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE procedure _TmSp_AddIndexTmpMSN4
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
	v_strSQL nclob;
BEGIN

	--v_strSQL := 'ALTER TABLE #TmpMSN4 ADD primary key (MSN4_PRIMARY)';
	--exec (:v_strSQL);
	v_strSQL := 'a'; 
	--CANNOT alter templorary table 
	--CANNOT CREATE INDEX ON HANA TEMP TABLES.

END;


