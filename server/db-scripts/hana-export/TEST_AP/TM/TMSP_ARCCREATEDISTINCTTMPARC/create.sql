-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE PROCEDURE TmSp_ArcCreateDistinctTmpArc(in TmpArcName varchar (5000))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER	
	--*With Encryption
AS
temp_var INT;
TmpArc  nvarchar(128);
TmpArcBack  nvarchar(128);
PairsArc  nvarchar(128);
sqltext varchar(1000);
BEGIN

--Parameters validation for security issues
call _TmSp_ValidateSpParam(:TmpArcName);

TmpArc := :TmpArcName;
TmpArcBack := :TmpArc || '_2';
-- TMP tables names have changed due to problems in the code, with the number of characters (5.3.2009)
--	##TMP_ARC_pid to ##TDAR_pid
--	##ARC_MSSQL_PAIRS_pid to ##TDAP_pid

--SET NOCOUNT ON;

--SELECT 1 INTO temp_var FROM  tempdb."INFORMATION_SCHEMA"."TABLES" where TABLE_NAME = :TmpArcBack;
select count(*) into temp_var from "PUBLIC"."TABLES" WHERE "IS_TEMPORARY" = 'TRUE' and table_name = :TmpArcBack;
if temp_var>0 then 
	select  "TABLE_OID" into temp_var from "PUBLIC"."TABLES" WHERE "IS_TEMPORARY" = 'TRUE' and table_name = :TmpArcBack;
	if :temp_var is not null and :temp_var <> 0 then
		exec ('truncate table ' || :TmpArcBack);
		sqltext:=
		'insert into '||:TmpArcBack||' SELECT MIN(Line_ID) Line_ID, DocType,DocNum,DocAbs,Total,RefDate,ClusterId,Remarks,Min(CanArcObj) CanArcObj,CanArcClus,KeySeg1,KeySeg2,KeySeg3,KeySeg4,KeySeg5,KeySeg6,KeySeg7,KeySeg8,KeySeg9,KeySeg10,Series,DocSubType,PIndicator,Instance,Segment,CloseInSim,CardCode from '||:TmpArc||
		' GROUP BY DocType,DocNum,DocAbs,Total,RefDate,ClusterId,Remarks,CanArcClus,KeySeg1,KeySeg2,KeySeg3,KeySeg4,KeySeg5,KeySeg6,KeySeg7,KeySeg8,KeySeg9,KeySeg10,Series,DocSubType,PIndicator,Instance,Segment,CloseInSim,CardCode';
		exec (:sqltext);
		exec ('truncate table ' || :TmpArc);
		exec ('insert into ' || :TmpArc || '  select  from ' || :TmpArcBack );
		exec ('drop table ' || :TmpArcBack);
	end if;
end if;
--select TOP 1 (:tmpStr || :WhereStr) from  AACP;
END;


