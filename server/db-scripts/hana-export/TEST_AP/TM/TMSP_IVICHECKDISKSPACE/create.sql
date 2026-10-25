-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:PT:PROCESS_END


CREATE procedure TmSp_IVICheckDiskSpace 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS 
	mdf_disk_free_space_mb BIGINT; 
	TablesSumSpace BIGINT; 
	UTablesSumSpace BIGINT; 
BEGIN 
	select round(sum(T.table_size) / 1048576) into  TablesSumSpace
	from  "PUBLIC".m_tables as T
	where T.table_name IN ( 'OITM', 'OITW', 'OILM', 'OIVL', 
							'IVL1', 'IVL2', 'OIVE', 'OIVQ', 'OIVK', 
							'ILM1', 'ILM2', 'ILM3' )
			and schema_name = CURRENT_SCHEMA;

	select round(sum(T.table_size) / 1048576) into  UTablesSumSpace
	from  "PUBLIC".m_tables as T
	where T.table_name IN ( 'UITM', 'UITW', 'UILM', 'UIVL', 
							'UIVL1', 'UIVL2', 'UIVE', 'UIVQ', 'UIVK', 
							'UILM1', 'UILM2', 'UILM3' )
			and schema_name = CURRENT_SCHEMA;

	select round((T.total_size - T.used_size) / 1048576) into mdf_disk_free_space_mb
	from "PUBLIC".m_disks as T where T.disk_id = 1;

	SELECT ( 3 * ( :TablesSumSpace - :UTablesSumSpace ) - :mdf_disk_free_space_mb ) from dummy;
end;


