-- B1 DEPENDS: BEFORE:PT:PROCESS_START AFTER:SP:_TmSp_ValidateSpParam

create procedure _TmSp_ChangeColumnToNotNull (
	in tableName nvarchar(100),		
	in columnName nvarchar(100),
	in defaultValue nvarchar(100),
        out retVal int)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as
	colType	nvarchar(100);
	len	integer;
	prec integer;
	scale integer;
begin
--For HANA security issue call procedure "_TmSp_ValidateSpParam"
    call _TmSp_ValidateSpParam(:tableName);    
	call _TmSp_ValidateSpParam(:columnName);   
	call _TmSp_ValidateSpParam(:defaultValue);   
	
	--if :tableName IS NULL or :tableName = '' or :columnName IS NULL or :columnName = ''
	--then
	--	return -1;
	--end if;
	retVal := 0;
	-- replace column NULL values by default value
	if :defaultValue IS NOT NULL and :defaultValue <> ''
	then		
		exec('update "' || :tableName || '" set "' || :columnName || '" = '' ' || :defaultValue || '''' || ' where "' || :columnName || '" IS NULL');
	end if;

	-- get column type, lenght (text types), digits and decimal places (money type)
	-- (used SQL is prepared in MSSQL2000, but it is compatible with MSSQL2005 and MSSQL2008)

	select top 1 tblTypes."TYPE_NAME" into colType
	from "PUBLIC"."TABLE_COLUMNS" tblColumns
	inner join "PUBLIC"."OBJECTS" tblTables
	on tblTables."OBJECT_OID" = tblColumns."TABLE_OID" and tblTables."OBJECT_TYPE" = 'TABLE'
	inner join "PUBLIC"."DATA_TYPES" tblTypes
	on tblTypes."TYPE_ID" = tblColumns."DATA_TYPE_ID" and tblTypes."TYPE_NAME" <> 'sysname'
	where tblTables."OBJECT_NAME" = :tableName and tblColumns."COLUMN_NAME" = :columnName;
	
	select top 1 tblColumns."LENGTH" into len
	from "PUBLIC"."TABLE_COLUMNS" tblColumns
	inner join "PUBLIC"."OBJECTS" tblTables
	on tblTables."OBJECT_OID" = tblColumns."TABLE_OID" and tblTables."OBJECT_TYPE" = 'TABLE'
	inner join "PUBLIC"."DATA_TYPES" tblTypes
	on tblTypes."TYPE_ID" = tblColumns."DATA_TYPE_ID" and tblTypes."TYPE_NAME" <> 'sysname'
	where tblTables."OBJECT_NAME" = :tableName and tblColumns."COLUMN_NAME" = :columnName;
	
	select top 1 tblColumns."SCALE" into scale
	from "PUBLIC"."TABLE_COLUMNS" tblColumns
	inner join "PUBLIC"."OBJECTS" tblTables
	on tblTables."OBJECT_OID" = tblColumns."TABLE_OID" and tblTables."OBJECT_TYPE" = 'TABLE'
	inner join "PUBLIC"."DATA_TYPES" tblTypes
	on tblTypes."TYPE_ID" = tblColumns."DATA_TYPE_ID" and tblTypes."TYPE_NAME" <> 'sysname'
	where tblTables."OBJECT_NAME" = :tableName and tblColumns."COLUMN_NAME" = :columnName;

	-- prepare complete datatype for altering the column
	if :colType = 'nvarchar'
	then
		colType := :colType || '(' || TO_NCHAR(:len/2) || ')';  --(:len/2) cast((:len/2) as varchar(10))  cast((:len/2) as nvarchar(10))
	elseif :colType = 'char'
    then
		colType := :colType || '(' || TO_NCHAR(:len) || ')';   --cast(:len as nvarchar(10))
	elseif :colType = 'numeric'
	then
		colType := :colType || '(' || TO_NCHAR(:len) || ',' || TO_NCHAR(:scale) || ')';  --cast(:len as nvarchar(10))   cast(:scale as nvarchar(10))
	end if;
	
	exec('alter table "' || :tableName || '" alter ("' || :columnName || '" ' || :colType || ' NOT NULL DEFAULT ' || :defaultValue || ' )');
end;


