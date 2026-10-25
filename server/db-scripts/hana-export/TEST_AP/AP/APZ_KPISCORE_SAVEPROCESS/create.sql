
Create PROCEDURE Apz_KPIScore_SaveProcess(in typeName char(1) default '')
AS
BEGIN 

	DECLARE Id INT=0;
	DECLARE code NVARCHAR(50) = '';
	
	-- kiểm tra bảng tạm đã khởi tạo chưa
	IF not exists (select 1 from M_TEMPORARY_TABLES where schema_Name = current_schema and Table_Name = '#APZ_OKSC')
	THEN
		SELECT 404, ' Temp data is not exist' from DUMMY;
		RETURN;
	END IF;
	
	create local temporary table #APZ_OKSCData as(select top 0 * from APZ_OKSC);
	exec 'insert into #APZ_OKSCData select * from #APZ_OKSC';
	
	select ifnull((SELECT TOP 1 "Name" FROM #APZ_OKSCData), '') into code from DUMMY;
	IF :code = ''
	THEN
		SELECT 405, 'Name cannot be empty!' from DUMMY;
		RETURN;
	END IF;
	
	select ifnull((SELECT TOP 1 "DocEntry" FROM #APZ_OKSCData),0) into Id from DUMMY;

	-- Nếu delete
	IF :typeName = 'D'
	THEN
		-- todo: check KPI Plan có nằm trong nghiệp vụ # nào ko
		
		-- nếu ko thì xóa
		DELETE from APZ_OKSC WHERE "DocEntry" = :Id;
		DELETE from APZ_KSC1 WHERE "DocEntry" = :Id;
		
		SELECT 0, '' from DUMMY;
		RETURN;
	END IF;

	IF :typeName = 'A'
	THEN
		select ifnull((SELECT MAX("DocEntry") FROM APZ_OKSC), 0) + 1 into Id from DUMMY;
		UPDATE #APZ_OKSCData
			SET "UpdateDate" = current_date ,"CreateDate" = current_date  ,"DocEntry"=:Id;
	ELSEIF :typeName = 'U'
	THEN
        DELETE from APZ_OKSC WHERE "DocEntry"=:Id;
        DELETE from APZ_KSC1 WHERE "DocEntry"=:Id;
		UPDATE #APZ_OKSCData SET "UpdateDate" = Current_Date;
	END IF;

	IF EXISTS (SELECT 1 FROM APZ_OKSC a INNER JOIN #APZ_OKSCData b ON b."Name" = a."Name")
	THEN
		SELECT 123, 'Object is overlapping with an existing Object' from DUMMY;
		RETURN;
	END IF;
	
	INSERT INTO APZ_OKSC
	  SELECT * FROM #APZ_OKSCData;

	IF exists (select 1 from M_TEMPORARY_TABLES where schema_Name = current_schema and Table_Name = '#APZ_KSC1')
	THEN
	
		create local temporary table #APZ_KSC1Data as (select top 0 b.* from (select 1 tmp from DUMMY) a left join APZ_KSC1 b on 1=1);
		exec 'insert into #APZ_KSC1Data select * from #APZ_KSC1';
		BEGIN
			declare lineNum int = 0;
			update #APZ_KSC1Data set "DocEntry"=:Id, "LineId" = null;
			WHILE exists(select 1 from #APZ_KSC1Data where "LineId" is null)
			DO
				UPDATE top 1 #APZ_KSC1Data SET "LineId"=:lineNum where "LineId" is null;
				lineNum = :lineNum + 1;
			END WHILE;
		END;
		INSERT INTO APZ_KSC1
			SELECT * FROM #APZ_KSC1Data;
	END IF;
	
	SELECT 0, '' from DUMMY;
END




