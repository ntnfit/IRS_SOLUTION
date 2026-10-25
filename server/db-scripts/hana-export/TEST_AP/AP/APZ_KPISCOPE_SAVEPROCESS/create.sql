
Create PROCEDURE Apz_KPIScope_SaveProcess(in typeName char(1) default '')
AS
BEGIN 

	DECLARE Id INT=0;
	DECLARE code NVARCHAR(50) = '';
	-- kiểm tra bảng tạm đã khởi tạo chưa
	IF not exists (select 1 from M_TEMPORARY_TABLES where schema_Name = current_schema and Table_Name = '#APZ_OKSE')
	THEN
		SELECT 404, ' Temp data is not exist' from DUMMY;
		RETURN;
	END IF;
	
	create local temporary table #APZ_OKSEData as(select top 0 * from APZ_OKSE);
	exec 'insert into #APZ_OKSEData select * from #APZ_OKSE';
	
	select ifnull((SELECT TOP 1 "Code" FROM #APZ_OKSEData), '') into code from DUMMY;
	IF :code = ''
	THEN
		SELECT 405, 'Code cannot be empty!' from DUMMY;
		RETURN;
	END IF;

	-- Nếu delete
	IF :typeName = 'D'
	THEN
		-- todo: check KPIScope có nằm trong nghiệp vụ # nào ko
		
		-- nếu ko thì xóa
		DELETE from APZ_OKSE WHERE "Code" = :code;
		DELETE from APZ_KSE1 WHERE "Code" = :code;
		
		SELECT 0, '' from DUMMY;
		RETURN;
	END IF;

	IF :typeName = 'A'
	THEN
		select IFNULL((SELECT MAX("DocEntry") FROM APZ_OKSE), 0) + 1 into id from DUMMY;
		UPDATE #APZ_OKSEData SET "UpdateDate" = Current_date ,"CreateDate" = Current_date,"DocEntry"=:Id;

	ELSEIF :typeName = 'U'
	THEN
		select ifnull((SELECT TOP 1 "DocEntry" FROM #APZ_OKSEDATA), 0) into Id from DUMMY;
        DELETE from APZ_OKSE WHERE "DocEntry"=:Id;
        DELETE from APZ_KSE1 WHERE "DocEntry"=:Id;
		UPDATE #APZ_OKSEData SET "UpdateDate" = current_date;
	END IF;

	IF EXISTS (SELECT 1 FROM APZ_OKSE a INNER JOIN #APZ_OKSEData b ON b."Code" = a."Code")
	THEN
		SELECT 123, 'Object is overlapping with an existing Object' from DUMMY;
		RETURN;
	END IF;
	
	INSERT INTO APZ_OKSE
	  SELECT * FROM #APZ_OKSEData;

	IF exists (select 1 from M_TEMPORARY_TABLES where schema_Name = current_schema and Table_Name = '#APZ_KSE1')
	THEN	
		create local temporary table #APZ_KSE1Data as (select top 0 b.* from (select 1 tmp from DUMMY) a left join APZ_KSE1 b on 1=2);
		exec 'insert into #APZ_KSE1Data select * from #APZ_KSE1';
		BEGIN
			declare lineNum int = 0;
			update #APZ_KSE1Data set "DocEntry"=:Id, "Code" = :code, "LineId" = null;
			WHILE exists(select 1 from #APZ_KSE1Data where "LineId" is null)
			DO
				UPDATE top 1 #APZ_KSE1Data SET "LineId"=:lineNum where "LineId" is null;
				lineNum = :lineNum + 1;
			END WHILE;
		END;
		
		INSERT INTO APZ_KSE1
			SELECT * FROM #APZ_KSE1Data;
	END IF;

	SELECT 0, '' FROM DUMMY;
END




