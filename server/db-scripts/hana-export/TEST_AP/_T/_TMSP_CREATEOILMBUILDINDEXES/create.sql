-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE PROCEDURE _TmSp_CreateOILMBuildIndexes
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
	sqlStr nvarchar(1000);
BEGIN
	sqlStr :='CREATE INDEX "OILM_CARD" ON OILM("BPCardCode" ASC)';
	EXEC sqlStr;
	
	sqlStr :='CREATE INDEX "OILM_DocLine" ON OILM("TransType" ASC,"DocEntry" ASC,"DocLineNum" ASC,"SubLineNum" ASC)';
	EXEC sqlStr;
	sqlStr :='CREATE INDEX "OILM_APPOBJ" ON OILM("ApplObj" ASC,"AppObjAbs" ASC,"AppObjType" ASC,"AppObjLine" ASC)';
	EXEC sqlStr;
	
END;


