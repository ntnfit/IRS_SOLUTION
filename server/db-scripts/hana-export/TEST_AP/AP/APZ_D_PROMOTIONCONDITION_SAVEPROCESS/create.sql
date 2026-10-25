
Create PROCEDURE Apz_D_PromotionCondition_SaveProcess(in typeName char(1) default '')
as
BEGIN 

	-- kiểm tra bảng tạm đã khởi tạo chưa
	DECLARE checkCount INT;
	DECLARE code NVARCHAR(20) = '';
	DECLARE docType NVARCHAR(20) = '';
	
	create local temporary table #APZ_D_OPCSDATA as (select top 0 * from APZ_D_OPCS);
	exec 'insert into #APZ_D_OPCSDATA select * from #APZ_D_OPCS';
	
	create local temporary table #APZ_D_PCS1Data as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_PCS1 b ON 1 = 1);
	create local temporary table #APZ_D_PCS2Data as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_PCS2 b ON 1 = 1);
	create local temporary table #APZ_D_PCS3Data as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_PCS3 b ON 1 = 1);
	create local temporary table #APZ_D_PCS4Data as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_PCS4 b ON 1 = 1);
	create local temporary table #APZ_D_PCS5Data as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_PCS5 b ON 1 = 1);
	create local temporary table #APZ_D_PCS6Data as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_PCS6 b ON 1 = 1);
	create local temporary table #APZ_D_PCS7Data as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_PCS7 b ON 1 = 1);
	create local temporary table #APZ_D_PCS9Data as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_PCS9 b ON 1 = 1);
	create local temporary table #APZ_D_PCS10Data as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_PCS10 b ON 1 = 1);
	create local temporary table #APZ_D_PCS11Data as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_PCS11 b ON 1 = 1);
	
	SELECT COUNT(1) into checkCount FROM #APZ_D_OPCSData;
	IF :checkCount <= 0
	THEN
		SELECT 404, ' Temp data is not exist' FROM DUMMY;
		RETURN;
	END IF;

	SELECT TOP 1 "Code", "Type" into code, docType FROM #APZ_D_OPCSData;
	IF :code = ''
	THEN
		SELECT 405, 'Code cannot be empty!' FROM DUMMY;
		RETURN;
	END IF;

	-- Nếu delete
	IF :typeName = 'D'
	THEN
		-- todo: check condition có nam trong condition rule nào ko
		
		-- nếu ko thì xóa
		DELETE FROM APZ_D_OPCS WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS1 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS2 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS3 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS4 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS5 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS6 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS7 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS9 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS10 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS11 WHERE "Code" = :code;
		
		SELECT 0, '' from DUMMY;
		RETURN; 
	ELSE
		IF :docType = '1'
		THEN
			IF exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_D_PCS1' and schema_name = current_schema) --OBJECT_ID('tempdb..#APZ_D_PCS1') IS NOT NULL
			THEN
				exec 'insert into #APZ_D_PCS1DATA select * from #APZ_D_PCS1';
				UPDATE #APZ_D_PCS1Data SET "Code" = :code;
			END if;
		ELSEIF :docType = '2'
		THEN
			IF exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_D_PCS2' and schema_name = current_schema) --OBJECT_ID('tempdb..#APZ_D_PCS2') IS NOT NULL
			THEN
				exec 'insert into #APZ_D_PCS2DATA select * from #APZ_D_PCS2';
				UPDATE #APZ_D_PCS2Data SET "Code" = :code;
			END IF;
		ELSEIF :docType = '3'
		THEN
			IF exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_D_PCS3' and schema_name = current_schema) --OBJECT_ID('tempdb..#APZ_D_PCS3') IS NOT NULL
			THEN
				exec 'insert into #APZ_D_PCS3DATA select * from #APZ_D_PCS3';
				UPDATE #APZ_D_PCS3Data SET "Code" = :code;
			END if;
		ELSEIF :docType = '4'
		THEN
			IF exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_D_PCS4' and schema_name = current_schema) --OBJECT_ID('tempdb..#APZ_D_PCS4') IS NOT NULL
			THEN
				exec 'insert into #APZ_D_PCS4DATA select * from #APZ_D_PCS4';
				UPDATE #APZ_D_PCS4Data SET "Code" = :code;
			END IF;
		ELSEIF :docType = '5'
		THEN
			IF exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_D_PCS5' and schema_name = current_schema) --OBJECT_ID('tempdb..#APZ_D_PCS5') IS NOT NULL
			THEN
				exec 'insert into #APZ_D_PCS5DATA select * from #APZ_D_PCS5';
				UPDATE #APZ_D_PCS5Data SET "Code" = :code;
			END IF;
		ELSEIF :docType = '6'
		THEN
			IF exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_D_PCS6' and schema_name = current_schema) --OBJECT_ID('tempdb..#APZ_D_PCS6') IS NOT NULL
			THEN
				exec 'insert into #APZ_D_PCS6DATA select * from #APZ_D_PCS6';
				UPDATE #APZ_D_PCS6Data SET "Code" = :code;
			END IF;
		ELSEIF :docType = '7'
		THEN
			IF exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_D_PCS7' and schema_name = current_schema) --OBJECT_ID('tempdb..#APZ_D_PCS7') IS NOT NULL
			THEN
				exec 'insert into #APZ_D_PCS7DATA select * from #APZ_D_PCS7';
				UPDATE #APZ_D_PCS7Data SET "Code" = :code;
			END IF;
		--ELSE IF @type = '8'
		ELSEIF :docType = '9'
		THEN
			IF exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_D_PCS9' and schema_name = current_schema) --OBJECT_ID('tempdb..#APZ_D_PCS9') IS NOT NULL
			THEN
				exec 'insert into #APZ_D_PCS9DATA select * from #APZ_D_PCS9';
				UPDATE #APZ_D_PCS9Data SET "Code" = :code;
			END IF;
		ELSEIF :docType = '10'
		THEN
			IF exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_D_PCS10' and schema_name = current_schema) --OBJECT_ID('tempdb..#APZ_D_PCS10') IS NOT NULL
			THEN
				exec 'insert into #APZ_D_PCS10DATA select * from #APZ_D_PCS10';
				UPDATE #APZ_D_PCS10Data	SET "Code" = :code;
			END IF;
		ELSEIF exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_D_PCS11' and schema_name = current_schema) --OBJECT_ID('tempdb..#APZ_D_PCS11') IS NOT NULL
		THEN
			BEGIN
				declare lineNum int = 1;
				exec 'insert into #APZ_D_PCS11DATA select * from #APZ_D_PCS11';
				UPDATE #APZ_D_PCS11Data set "Code" = :code,"LineNum"= null;
				while exists(select 1 from #APZ_D_PCS11Data where "LineNum" is null)
				DO
				
					update top 1 #APZ_D_PCS11Data set "LineNum" = :lineNum where "LineNum" is null;
					lineNum = :lineNum + 1;
				
				END WHILE;
			END;
		END	IF;
		
		DELETE FROM APZ_D_OPCS WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS1 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS2 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS3 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS4 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS5 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS6 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS7 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS9 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS10 WHERE "Code" = :code;
		DELETE FROM APZ_D_PCS11 WHERE "Code" = :code;
	END IF;
	
	IF EXISTS (SELECT 1 FROM APZ_D_OPCS a INNER JOIN #APZ_D_OPCSData b ON b."Code" = a."Code")
	THEN
		SELECT 123, 'Object is overlapping with an existing Object' FROM DUMMY;
		RETURN;
	END IF;

	IF :typeName = 'A'
	THEN
		UPDATE #APZ_D_OPCSData SET "UpdateDate" = Current_date ,"CreateDate" = Current_date;
	ELSEIF :typeName = 'U'
	THEN
		UPDATE #APZ_D_OPCSData SET "UpdateDate" = Current_date;
	END IF;
	
	INSERT INTO APZ_D_OPCS
	  SELECT * FROM #APZ_D_OPCSData;

	INSERT INTO APZ_D_PCS1
			SELECT * FROM #APZ_D_PCS1Data;
	INSERT INTO APZ_D_PCS2
		  SELECT * FROM #APZ_D_PCS2Data;
	INSERT INTO APZ_D_PCS3
			SELECT * FROM #APZ_D_PCS3Data;
	INSERT INTO APZ_D_PCS4
		  SELECT * FROM #APZ_D_PCS4Data;
	INSERT INTO APZ_D_PCS5
			SELECT * FROM #APZ_D_PCS5Data;
	INSERT INTO APZ_D_PCS6
		  SELECT * FROM #APZ_D_PCS6Data;
	INSERT INTO APZ_D_PCS7
			SELECT * FROM #APZ_D_PCS7Data;
	INSERT INTO APZ_D_PCS9
		  SELECT * FROM #APZ_D_PCS9Data;
	INSERT INTO APZ_D_PCS10
			SELECT * FROM #APZ_D_PCS10Data;
	INSERT INTO APZ_D_PCS11
			SELECT * FROM #APZ_D_PCS11Data;
	SELECT 0, '' from DUMMY;
END




