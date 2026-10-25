-- B1 DEPENDS: BEFORE:PT:PROCESS_START
-- dependencies does not work in msq sql, and since we are updating fields that will be created during structural upgrade
-- so during creation of this SP, fields does not exists and table exits, so it will fail.  therefore we have to do it via 
-- string execution
-- please also note that doing this query by DBQ, it will fail on HANA, therefore SP creation
CREATE procedure _TmSp_UpgradeClosingDataTimeVersionNum
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
	v_sql nvarchar(4000);	
BEGIN
    v_sql:='UPDATE T0
	SET T0."SysCloseDt" = T1."CreateDate", T0."SysCloseTm" = T1."CreateTime", T0."CloseVerNm" = T1."VersionNum"
	FROM OWOR T0, OJDT T1
	WHERE T0."DocEntry" = T1."CreatedBy"
	AND T1."TransType" = ''202''';
	exec(:v_sql);
END;


