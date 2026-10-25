-- B1 DEPENDS: BEFORE:PT:PROCESS_START AFTER:SP:_TmSp_ValidateSpParam

CREATE procedure _TmSp_UpgradeEDF
(
	IN ECM2tableName nvarchar(100), 
	IN ECM3tableName nvarchar(100)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS

retValue		int 			:= 0;
IsExists1 	int				:= 0;
IsExists2 	int				:= 0;
IsExists3 	int				:= 0;
ecm2CopyName	nvarchar(100)	:= ECM2tableName || '_COPY';
ecm2TempName	nvarchar(100)	:= 'TEMP_' || ECM2tableName;
ecm3TempName	nvarchar(100)	:= 'TEMP_' || ECM3tableName;
execStr 		nvarchar(4000);

BEGIN
	call _TmSp_ValidateSpParam (:ECM2tableName);
	call _TmSp_ValidateSpParam (:ECM3tableName);
	
	-- create copy of current table if not already exists
	SELECT COUNT(*) INTO IsExists1 FROM OBJECTS 
	WHERE SCHEMA_NAME = CURRENT_SCHEMA AND OBJECT_TYPE = 'TABLE' AND OBJECT_NAME = :ecm2TempName;
	
	SELECT COUNT(*) INTO IsExists2 FROM OBJECTS 
	WHERE SCHEMA_NAME = CURRENT_SCHEMA AND OBJECT_TYPE = 'TABLE' AND OBJECT_NAME = :ecm2CopyName;
	
	IF :IsExists1 > 0 and :IsExists2 = 0 THEN
		execStr := 'CREATE TABLE ' || :ecm2CopyName || ' AS (SELECT * FROM ' || :ecm2TempName || ')';
		exec(:execStr);
		-- do not continue to upgrade (just copy table)
		return;
	END IF;

	-- necessary checks before upgrade
	-- ecm2CopyName (ECM2_COPY/AEC2_COPY) must exist. Copied from TEMP table
	IF :IsExists2 = 0  THEN
		-- do not continue to upgrade, TEMP table does not exist for "non fast upgrade" cases (if main table was empty or same structure, ...)
		return;
	END IF;

	-- test for upgraded ECM2/ECM3 table structure (9.1 -> 9.2)
	-- ECM2: removed field ActData
	SELECT COUNT(*) INTO IsExists1 FROM "PUBLIC"."TABLE_COLUMNS"
	WHERE TABLE_NAME = :ECM2tableName AND SCHEMA_NAME = CURRENT_SCHEMA AND COLUMN_NAME = 'ActData';
	IF :IsExists1 <> 0 THEN
		return;
	END IF;

	-- ECM3: new field LogData
	SELECT COUNT(*) INTO IsExists1 FROM "PUBLIC"."TABLE_COLUMNS"
	WHERE TABLE_NAME = :ECM3tableName AND SCHEMA_NAME = CURRENT_SCHEMA AND COLUMN_NAME = 'LogData';
	IF :IsExists1 = 0 THEN
		return;
	END IF;

	-- if TEMP_ECM3 table does not exist then use ECM3
	SELECT COUNT(*) INTO IsExists3 FROM OBJECTS 
	WHERE SCHEMA_NAME = CURRENT_SCHEMA AND OBJECT_TYPE = 'TABLE' AND OBJECT_NAME = :ecm3TempName;
	IF :IsExists3 = 0 THEN
	  ecm3TempName := ECM3tableName;
	END IF;


	-- upgrade steps

	-- mark old  ecm3 rows as 'Note'
	execStr := 'update ' || :ECM3tableName || ' set "LogType" = ''N''';
	exec(:execStr);
  
	-- copy	ActData to ECM3 as 'Send'
	execStr := 'insert into ' || :ECM3tableName || '("AbsEntry", "LogNum", "LogMessage", "UserSign", "CreateDate", "UserSign2", "UpdateDate", "LogInstanc", "CreateTS", "UpdateTS", "LogType", "LogData", "LogOpDate", "LogOpTS")
		select t0."AbsEntry", 1 + ifNull(t3."NextLogNum",0),
		"ActMessage", "UserSign", "CreateDate", "UserSign2", "UpdateDate", "LogInstanc", "CreateTS", "UpdateTS", ''S'', "ActData", "SendDate", "SendTS"
		from ' || :ecm2CopyName || ' t0 
		left outer join 
		(
			select "AbsEntry", max ("NextLogNum") as "NextLogNum" from
			(select "AbsEntry", ifNull(max("LogNum"),0) as "NextLogNum" from ' || :ecm3TempName || ' group by "AbsEntry"
			union all
			select "AbsEntry", ifNull(max("LogNum"),0) as "NextLogNum" from ' || :ECM3tableName || ' group by "AbsEntry"
			)as nieco
			group by "AbsEntry"
		) 
		t3 on t3."AbsEntry" = t0."AbsEntry" 
		where t0."ActData" is not null or t0."ActData2" is not null';
	exec(:execStr);

	-- copy	ActData2 to ECM3 as 'Receive'
	execStr := 'insert into ' || :ECM3tableName || '("AbsEntry", "LogNum", "LogMessage", "UserSign", "CreateDate", "UserSign2", "UpdateDate", "LogInstanc", "CreateTS", "UpdateTS", "LogType", "LogData", "LogOpDate", "LogOpTS")
		select t0."AbsEntry", 1 + ifNull(t3."NextLogNum",0),
		"ActMessage", "UserSign", "CreateDate", "UserSign2", "UpdateDate", "LogInstanc", "CreateTS", "UpdateTS", ''R'', "ActData2", "ReceivDate", NULL
		from ' || :ecm2CopyName || ' t0
		left outer join
		(
			select "AbsEntry", max ("NextLogNum") as "NextLogNum" from
			(select "AbsEntry", ifNull(max("LogNum"),0) as "NextLogNum" from ' || :ecm3TempName || ' group by "AbsEntry"
			union all
			select "AbsEntry", ifNull(max("LogNum"),0) as "NextLogNum" from ' || :ECM3tableName || ' group by "AbsEntry"
			)as nieco
			group by "AbsEntry"
		) 
		t3 on t3."AbsEntry" = t0."AbsEntry" 
		where t0."ActData" is not null or t0."ActData2" is not null';
	exec(:execStr);
 
	-- drop copy table
	execStr := 'drop table ' || :ecm2CopyName;
	exec(:execStr);

END


