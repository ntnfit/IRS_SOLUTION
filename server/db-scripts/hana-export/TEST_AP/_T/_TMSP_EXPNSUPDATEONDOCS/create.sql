-- B1 DEPENDS: BEFORE:PT:PROCESS_START

create procedure _TmSp_ExpnsUpdateOnDocs
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
begin
-------------------------------------------------------------------
	EXEC ('UPDATE OINV ' 
	||'SET "TotalExpns" = 0 WHERE "TotalExpns" is NULL');
	
	EXEC ('UPDATE OINV '
	||'SET "TotalExpFC" = 0 WHERE "TotalExpFC" is NULL');
	
	EXEC ('UPDATE OINV '
	||'SET "TotalExpSC" = 0 WHERE "TotalExpSC" is NULL');

	EXEC ('UPDATE ORIN '
	||'SET "TotalExpns" = 0 WHERE "TotalExpns" is NULL');
	
	EXEC ('UPDATE ORIN '
	||'SET "TotalExpFC" = 0 WHERE "TotalExpFC" is NULL');
	
	EXEC ('UPDATE ORIN '
	||'SET "TotalExpSC" = 0 WHERE "TotalExpSC" is NULL');
	
	EXEC ('UPDATE ODLN '
	||'SET "TotalExpns" = 0 WHERE "TotalExpns" is NULL');
	
	EXEC ('UPDATE ODLN '
	||'SET "TotalExpFC" = 0 WHERE "TotalExpFC" is NULL');
	
	EXEC ('UPDATE ODLN '
	||'SET "TotalExpSC" = 0 WHERE "TotalExpSC" is NULL');

	EXEC ('UPDATE ORDN '
	||'SET "TotalExpns" = 0 WHERE "TotalExpns" is NULL');
	
	EXEC ('UPDATE ORDN '
	||'SET "TotalExpFC" = 0 WHERE "TotalExpFC" is NULL');
	
	EXEC ('UPDATE ORDN '
	||'SET "TotalExpSC" = 0 WHERE "TotalExpSC" is NULL');

	EXEC ('UPDATE ORDR '
	||'SET "TotalExpns" = 0 WHERE "TotalExpns" is NULL');
	
	EXEC ('UPDATE ORDR '
	||'SET "TotalExpFC" = 0 WHERE "TotalExpFC" is NULL');
	
	EXEC ('UPDATE ORDR '
	||'SET "TotalExpSC" = 0 WHERE "TotalExpSC" is NULL');

	EXEC ('UPDATE OQUT '
	||'SET "TotalExpns" = 0 WHERE "TotalExpns" is NULL');
	
	EXEC ('UPDATE OQUT '
	||'SET "TotalExpFC" = 0 WHERE "TotalExpFC" is NULL');
	
	EXEC ('UPDATE OQUT '
	||'SET "TotalExpSC" = 0 WHERE "TotalExpSC" is NULL');

	EXEC ('UPDATE OPCH '
	||'SET "TotalExpns" = 0 WHERE "TotalExpns" is NULL');
	
	EXEC ('UPDATE OPCH '
	||'SET "TotalExpFC" = 0 WHERE "TotalExpFC" is NULL');
	
	EXEC ('UPDATE OPCH '
	||'SET "TotalExpSC" = 0 WHERE "TotalExpSC" is NULL');

	EXEC ('UPDATE ORPC '
	||'SET "TotalExpns" = 0 WHERE "TotalExpns" is NULL');
	
	EXEC ('UPDATE ORPC '
	||'SET "TotalExpFC" = 0 WHERE "TotalExpFC" is NULL');
	
	EXEC ('UPDATE ORPC '
	||'SET "TotalExpSC" = 0 WHERE "TotalExpSC" is NULL');

	EXEC ('UPDATE OPDN '
	||'SET "TotalExpns" = 0 WHERE "TotalExpns" is NULL');
	
	EXEC ('UPDATE OPDN '
	||'SET "TotalExpFC" = 0 WHERE "TotalExpFC" is NULL');
	
	EXEC ('UPDATE OPDN '
	||'SET "TotalExpSC" = 0 WHERE "TotalExpSC" is NULL');

	EXEC ('UPDATE ORPD '
	||'SET "TotalExpns" = 0 WHERE "TotalExpns" is NULL');
	
	EXEC ('UPDATE ORPD '
	||'SET "TotalExpFC" = 0 WHERE "TotalExpFC" is NULL');
	
	EXEC ('UPDATE ORPD '
	||'SET "TotalExpSC" = 0 WHERE "TotalExpSC" is NULL');

	EXEC ('UPDATE OPOR '
	||'SET "TotalExpns" = 0 WHERE "TotalExpns" is NULL');
	
	EXEC ('UPDATE OPOR '
	||'SET "TotalExpFC" = 0 WHERE "TotalExpFC" is NULL');
	
	EXEC ('UPDATE OPOR '
	||'SET "TotalExpSC" = 0 WHERE "TotalExpSC" is NULL');
----------------------------------------------------------------------
end;


