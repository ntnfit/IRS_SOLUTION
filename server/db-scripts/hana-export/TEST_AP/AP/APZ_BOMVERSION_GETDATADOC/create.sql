
CREATE PROCEDURE APZ_BomVersion_GetDataDoc(in Code nvarchar(50) default '', in Version nvarchar(50) default '')
AS
BEGIN
	
	---------------------------------------- VARIABLE ---------------------------------------------------
	DECLARE i int = 1;
	DECLARE maxId int;
	DECLARE sqlString NVARCHAR(254);
	---------------------------------------- END VARIABLE ----------------------------------------------
	
	---------------------------------------- THÊM UDF ĐÃ ĐƯỢC TẠO TRÊN BẢNG HEADER CHÍNH NHƯNG CHƯA TẠO Ở BẢNG VERSION ----------------------------------------------
	create local temporary table #tempNewDocUdf as (SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE_NAME DATA_TYPE, "LENGTH" CHARACTER_MAXIMUM_LENGTH, ROW_NUMBER() OVER ( ORDER BY "POSITION" ASC) AS "Id" 
					FROM TABLE_COLUMNS t WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = 'OITT' AND COLUMN_NAME LIKE 'U[_]%' AND NOT EXISTS (SELECT 1 FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = 'APZ_OITT' AND COLUMN_NAME = t.COLUMN_NAME));
	SELECT IFNULL((select MAX("Id") from #tempNewDocUdf), 0) into maxId from DUMMY;
	
	WHILE :i <= :maxId
	DO
		select IFNULL((select  'ALTER TABLE APZ_OITT ADD "' || COLUMN_NAME || '" ' + DATA_TYPE || 
				case when DATA_TYPE = 'NVARCHAR' or DATA_TYPE = 'CHAR' or DATA_TYPE = 'VARCHAR' or DATA_TYPE = 'NCHAR' then '(' ||  cast(CHARACTER_MAXIMUM_LENGTH as nvarchar) || ')'
					WHEN DATA_TYPE = 'NUMERIC' then '(19,6)' else '' end || 
				case when IS_NULLABLE = 'FALSE' then ' NOT NULL' else ' NULL' end	from #tempNewDocUdf where "Id" = :i),'') into sqlString from DUMMY;
		--print @sql
		BEGIN
			declare EXIT HANDLER FOR SQLEXCEPTION
			BEGIN
				exec :sqlString;
			END;
		END;
		i = i + 1;

	END WHILE;
	------------------------------------------ END TẠO THÊM UDF ----------------------------------------------------------------------------------------------

	------------------------------------------ XÓA CÁC UDF ĐÃ XÓA TRÊN BẢNG HEADER CHÍNH NHƯNG CHƯA XÓA Ở BẢNG VERSION ----------------------------------------------
	create local temporary table #tempDeleDocUdf as (select COLUMN_NAME, ROW_NUMBER() OVER(ORDER BY "POSITION" ASC) "Id"
						from TABLE_COLUMNS t where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'APZ_OITT' AND COLUMN_NAME like 'U[_]%' 
	                                 AND NOT EXISTS(select 1 from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'OITT' AND COLUMN_NAME = t.COLUMN_NAME));
	SELECT IFNULL((select MAX("Id") from #tempDeleDocUdf), 0) into maxId FROM DUMMY;
	i = 1;
	
	WHILE :i <= :maxId
	DO
		select IFNULL((select  'ALTER TABLE APZ_OITT
		DROP COLUMN "' || COLUMN_NAME || '"' from #tempDeleDocUdf where "Id" = :i),'') into sqlString from DUMMY;
		BEGIN
			declare EXIT HANDLER FOR SQLEXCEPTION
			BEGIN
				exec :sqlString;
			END;
		END;
		i = i + 1;

	END WHILE;
	--------------------------------------------- END XÓA UDF -----------------------------------------------------------------------------------------------

	--------------------------------------------- THÊM CÁC UDF ĐÃ ĐƯỢC THÊM VÀO BẢNG LINE CHÍNH NHƯNG CHƯA THÊM VÀO BẢNG VERSION ----------------------------
	CREATE LOCAL TEMPORARY TABLE #tempNewLineUdf as (select COLUMN_NAME, IS_NULLABLE, DATA_TYPE_NAME DATA_TYPE, "LENGTH" CHARACTER_MAXIMUM_LENGTH, ROW_NUMBER() OVER(ORDER BY "POSITION" ASC) "Id"
						from TABLE_COLUMNS t where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'ITT1' AND COLUMN_NAME like 'U[_]%' 
	                                 AND NOT EXISTS(select 1 from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'APZ_ITT1' AND COLUMN_NAME = t.COLUMN_NAME));
	                                 
	SELECT IFNULL((select MAX("Id") from #tempNewLineUdf), 0) into maxId from DUMMY;
	i = 1;
	
	WHILE :i <= :maxId
	DO
		select IFNULL((select  'ALTER TABLE APZ_ITT1
		ADD "' || COLUMN_NAME || '" ' || DATA_TYPE || 
				case when DATA_TYPE = 'NVARCHAR' or DATA_TYPE = 'CHAR' or DATA_TYPE = 'VARCHAR' or DATA_TYPE = 'NCHAR' then '('|| CAST(CHARACTER_MAXIMUM_LENGTH as nvarchar) || ')'
					WHEN DATA_TYPE = 'NUMERIC' then '(19,6)' else '' end  || 
				case when IS_NULLABLE = 'FALSE' then ' NOT NULL' else ' NULL' end	from #tempNewLineUdf where "Id" = :i),'') into sqlString from DUMMY;
		
		BEGIN
			declare EXIT HANDLER FOR SQLEXCEPTION
			BEGIN
				exec :sqlString;
			END;
		END;
		i = 1 + 1;

	END WHILE;
	-------------------------------------------- END THÊM UDF -----------------------------------------------------------------------------------------------

	-------------------------------------------- XÓA UDF ĐÃ XÓA TRÊN LINE CHÍNH NHƯNG CHƯA XÓA TRÊN BẢNG VERSION --------------------------------------------
	CREATE LOCAL TEMPORARY TABLE #tempDeleLineUdf as (select COLUMN_NAME, ROW_NUMBER() OVER(ORDER BY "POSITION" ASC) "Id" 
						from TABLE_COLUMNS t where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'APZ_ITT1' AND COLUMN_NAME like 'U[_]%' 
	                                 AND NOT EXISTS(select 1 from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'ITT1' AND COLUMN_NAME = t.COLUMN_NAME));
	SELECT IFNULL((select MAX("Id") from #tempDeleLineUdf), 0) INTO maxId from DUMMY;
	i = 1;
	
	WHILE :i <= :maxId
	DO
		SELECT IFNULL((select  'ALTER TABLE APZ_ITT1
		DROP COLUMN "' || COLUMN_NAME || '"' from #tempDeleLineUdf where "Id" = :i),'') into sqlString from DUMMY;
		BEGIN
			declare EXIT HANDLER FOR SQLEXCEPTION
			BEGIN
				exec :sqlString;
			END;
		END;
		i = i + 1;

	END WHILE;
	------------------------------------------------ END XÓA DỮ LIỆU ------------------------------------------------------------------------------------

	SELECT a.*, b."ItemName" AS "ItemName", CASE WHEN IFNULL(a."OcrCode", '') = '' THEN '' WHEN IFNULL(a."OcrCode2", '') = '' THEN a."OcrCode" WHEN IFNULL(a."OcrCode3", '') = '' THEN a."OcrCode" || ',' || a."OcrCode2" 
							WHEN IFNULL(a."OcrCode4", '') = '' THEN a."OcrCode" || ',' || a."OcrCode2" || ',' || a."OcrCode3" 
							WHEN IFNULL(a."OcrCode5", '') = '' THEN a."OcrCode" || ',' || a."OcrCode2" || ',' || a."OcrCode3" || ',' || a."OcrCode4" 
							ELSE a."OcrCode" || ',' || a."OcrCode2" || ',' || a."OcrCode3" || ',' || a."OcrCode4" || ',' || a."OcrCode5" END AS "DisRule" 
				FROM APZ_OITT a INNER JOIN oitm b ON a."Code" = b."ItemCode" WHERE IFNULL(:Code, '') = '' OR (a."Code" = :Code AND (IFNULL(:Version, '') = '' OR a."Version" = :Version));
END;




