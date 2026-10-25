-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE procedure _TmSp_RemoveUDFsUDTsInconsistencies
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
 exists1 int;
 tables nvarchar(30) array = array('TMP1', 'ABF1', 'ABF2', 'ABTD', 'ABTF', 'OPBU', 'ORPA', 'ARPA', 'ORPR', 'ARPR');
 idx integer;
 statement nvarchar(500);
 table_es_query_template nvarchar(30)  = 'ES_QUERY_TEMPLATE';
BEGIN

-- Fixing query for pre-check nr.11 - space after table name in OUTB.TableName
------------------------------------------------------------------------------
UPDATE OUTB SET "TableName" = LTRIM(RTRIM("TableName")) WHERE "TableName" LIKE '% %';

-- Fixing queries from SAP note 1076082 - UDT and UDF Related Issues:
---------------------------------------------------------------------
-- 7a) UDF contains extra spaces in field TableID in User-fields description CUFD table
update cufd
set "TableID" = REPLACE ("TableID", char(32), char(0)) from cufd
where "TableID"  LIKE '% %';

-- 7b) UDF contains extra spaces in field AliasID in User-fields
update cufd
set "AliasID" = REPLACE ("AliasID", char(32), char(0)) from cufd
where "AliasID"  LIKE '% %';

-- 1) UDT is defined but does not exist
delete from OUTB T0 where not exists (select 1 from sys.tables T1 where T1.schema_name = current_schema and T1.is_system_table = 'FALSE' and T1.table_name = '@' || "TableName");

-- 3) UDF is defined on an unregistered UDT
delete from CUFD T0 
where left(T0."TableID",1)='@' 
and not exists (select 1 from OUTB T1 where '@' || T1."TableName" =T0."TableID" or '@' || 
T1."LogTable"= T0."TableID");

-- 4) UDF is defined but does not exist
delete from CUFD where not exists (
select 1 from public.tables t0 inner join public.table_columns t1 on t0.table_oid = t1.table_oid
where t0.schema_name = current_schema and t0.is_system_table = 'FALSE'
and t0.table_name = "TableID" and t1.column_name='U_' || "AliasID") and "TableID" not in ('BTNT', 'BTNT1', 'OIBT', 'OSRI', 'SRNT', 'SRNT1', 'ODIB', 'ODSR');

-- 5) UDF valid values are defined for non-existing UDF
delete from UFD1 T
where not exists(select 1 from CUFD where "TableID" = T."TableID"
and "FieldID"=T."FieldID");

-- Fixing query for obsolete tables 
-----------------------------------
for idx in 1.. cardinality(:tables) do
	delete from CUFD where "TableID" = :tables[:idx];
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = :tables[:idx] and "SCHEMA_NAME" = CURRENT_SCHEMA;
	if :exists1 <> 0 then
		statement := 'drop table ' || :tables[:idx] || ' CASCADE';
		exec :statement;
	end if;
end for;

-- Fixing query for table ES_QUERY_TEMPLATE 
-------------------------------------------
select count(1) into exists1 from "PUBLIC"."M_TABLES"
where "TABLE_NAME" = :table_es_query_template and "SCHEMA_NAME" = CURRENT_SCHEMA;
if :exists1 <> 0 then
	statement := 'alter table ' || :table_es_query_template || ' row';
	exec :statement;
	statement := 'alter table ' || :table_es_query_template || ' column';
	exec :statement;
end if;

END;


