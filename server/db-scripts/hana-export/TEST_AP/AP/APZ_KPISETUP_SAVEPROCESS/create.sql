
Create PROCEDURE Apz_KPISetup_SaveProcess(in typeName char(1) default '')
AS
BEGIN 
	DECLARE Id INT = 0;
	DECLARE code NVARCHAR(50) = '';
	-- kiểm tra bảng tạm đã khởi tạo chưa
	IF not exists (select 1 from M_TEMPORARY_TABLES where schema_Name = current_schema and Table_Name = '#APZ_OKSP')
	THEN
		SELECT 404, ' Temp data is not exist' from dummy;
		RETURN;
	END IF;
	
	create local temporary table #APZ_OKSPData as (select top 0 * from APZ_OKSP);
	exec 'insert into #APZ_OKSPData select * from #APZ_OKSP';
	
	select ifnull((SELECT TOP 1 "Name" FROM #APZ_OKSPData), '') into code from DUMMY;
	IF :code = ''
	THEN
		SELECT 405, 'Name cannot be empty!' from DUMMY;
		RETURN;
	END if;
	
	select ifnull((SELECT TOP 1 "DocEntry" FROM #APZ_OKSPData), 0) into Id from DUMMY;

	-- Nếu delete
	IF :typeName = 'D'
	THEN
		-- todo: check KPI Plan có nằm trong nghiệp vụ # nào ko
		
		-- nếu ko thì xóa
		DELETE from APZ_OKSP WHERE "DocEntry" = :Id;
		DELETE from APZ_KSP1 WHERE "DocEntry" = :Id;
		
		SELECT 0, '' from DUMMY;
		RETURN;
	END IF;

	IF :typeName = 'A'
	THEN
		select IFNULL((SELECT MAX("DocEntry") FROM APZ_OKSP), 0) + 1 into Id from DUMMY;
		UPDATE #APZ_OKSPData SET "UpdateDate" = current_date ,"CreateDate" = current_date ,"DocEntry"=:Id;
	ELSEIF :typeName = 'U'
	THEN
        DELETE from APZ_OKSP WHERE "DocEntry"=:Id;
        DELETE from APZ_KSP1 WHERE "DocEntry"=:Id;
		UPDATE #APZ_OKSPData SET "UpdateDate" = CUrrent_date;
	END IF;

	IF EXISTS (SELECT 1 FROM APZ_OKSP a INNER JOIN #APZ_OKSPData b ON b."Name" = a."Name")
	THEN
		SELECT 123, 'Object is overlapping with an existing Object' FROM DUMMY;
		RETURN;
	END IF;
	
	INSERT INTO APZ_OKSP 
	  SELECT * FROM #APZ_OKSPData;

	IF exists (select 1 from M_TEMPORARY_TABLES where schema_Name = current_schema and Table_Name = '#APZ_KSP1')
	THEN
		create local temporary table #APZ_KSP1DATA as (select top 0 b.* from (select 1 tmp from DUMMY) a left join APZ_KSP1 b on 1=2);
		exec 'insert into #APZ_KSP1DATA select * from #APZ_KSP1';
		-------------update linenum
		BEGIN
			declare lineNum int = 0;
			update #APZ_KSP1DATA set "DocEntry"=:Id, "LineId" = null;
			WHILE exists(select 1 from #APZ_KSP1DATA where "LineId" is null)
			DO
				UPDATE top 1 #APZ_KSP1DATA SET "LineId"=:lineNum where "LineId" is null;
				lineNum = :lineNum + 1;
			END WHILE;
		END;
		
		INSERT INTO APZ_KSP1
			SELECT * FROM #APZ_KSP1Data;
	END if;
	
	SELECT 0, '' from DUMMY;
END




