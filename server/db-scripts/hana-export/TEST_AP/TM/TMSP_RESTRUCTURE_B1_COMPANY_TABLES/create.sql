-- B1 DEPENDS: BEFORE:PT:PROCESS_START

create procedure TmSp_Restructure_B1_Company_Tables (schema_name_in varchar(260))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as
cursor cur_cs_tab for 
select * from M_CS_TABLES mct where 1=1 and upper(schema_name) = upper(:schema_name_in) 
and exists (select * from tables t where mct.schema_name = t.schema_name and mct.table_name = t.table_name and is_temporary = 'FALSE')
;
v_num integer;
v_start_time timestamp;
begin
v_num := 0;
for v_idx as cur_cs_tab
do
    exec 'load "'|| v_idx.SCHEMA_NAME ||'"."' || v_idx.TABLE_NAME ||'" all';
    exec 'MERGE DELTA OF "'|| v_idx.SCHEMA_NAME ||'"."' || v_idx.TABLE_NAME ||'" WITH PARAMETERS (''FORCED_MERGE'' = ''ON'')';
    exec 'UPDATE "'|| v_idx.SCHEMA_NAME ||'"."' || v_idx.TABLE_NAME ||'" WITH PARAMETERS (''OPTIMIZE_COMPRESSION''=''YES'')';
   
END FOR;
end;


