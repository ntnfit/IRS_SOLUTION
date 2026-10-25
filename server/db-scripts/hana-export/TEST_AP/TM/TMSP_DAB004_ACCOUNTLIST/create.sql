-- B1 DEPENDS: AFTER:SP:TmSp_DAB004_AccountList_QRYSTR AFTER:PT:PROCESS_END
--DROP PROCEDURE TmSp_DAB004_AccountList;
CREATE PROCEDURE TmSp_DAB004_AccountList(in Indexer int)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
--QryString nvarchar(max);
QryString nvarchar(8000);
v_TempInt INTEGER;

BEGIN

	-- DROP TEMP TABLE
	/*SELECT COUNT (*) INTO v_TempInt FROM "PUBLIC"."M_TEMPORARY_TABLES" WHERE TABLE_NAME = '#CASHFLOW_TMPACCTTOSHOW' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection;
	IF :v_TempInt > 0 THEN
		DROP TABLE #CASHFLOW_TmpAcctToShow;
	END IF;
	
	--SELECT QryString FROM DUMMY;
	IF :Indexer <> 0 THEN
		CREATE LOCAL TEMPORARY TABLE #CASHFLOW_TmpAcctToShow  ("AcctCode" nvarChar(30), "AcctName" nvarChar(200), "Balance" DECIMAL(21,6));
	ELSE
		CREATE LOCAL TEMPORARY TABLE #CASHFLOW_TmpAcctToShow  ("AcctCode" nvarChar(30), "AcctName" nvarChar(200), "AcctType" nvarchar(1));
	END IF;
	*/
	CALL TmSp_DAB004_AccountList_QRYSTR (Indexer, QryString);
	--QryString := 'Insert INTO #CASHFLOW_TmpAcctToShow (' || :QryString || ')';
	--EXEC :QryString;
	--SELECT * FROM #CASHFLOW_TmpAcctToShow;
	execute immediate (:QryString);
		
END;


