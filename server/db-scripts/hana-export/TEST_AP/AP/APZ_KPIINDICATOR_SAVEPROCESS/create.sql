
Create PROCEDURE Apz_KPIIndicator_SaveProcess(in typeName char(1) default '')
AS
BEGIN 
	
	DECLARE Id INT=0;
	DECLARE code NVARCHAR(50) = '';
	-- kiểm tra bảng tạm đã khởi tạo chưa
	IF not exists (select 1 from M_TEMPORARY_TABLES where schema_Name = current_schema and Table_Name = '#APZ_OKIR')
	THEN
		SELECT 404, ' Temp data is not exist' FROM DUMMY;
		RETURN;
	END IF;
	
	create local temporary table #APZ_OKIRdata as (select top 0 * from APZ_OKIR);
	exec 'insert into #APZ_OKIRdata select * from #APZ_OKIR';
	
	select ifnull((SELECT TOP 1 "Code" FROM #APZ_OKIRdata), '') into code from DUMMY;
	IF :code = ''
	THEN
		SELECT 405, 'Code cannot be empty!' from DUMMY;
		RETURN;
	END IF;
	
	select ifnull((SELECT TOP 1 "DocEntry" FROM #APZ_OKIRdata),0) into Id from DUMMY;

	-- Nếu delete
	IF :typeName = 'D'
	THEN
		-- todo: check KPI Indicator có nằm trong nghiệp vụ # nào ko
		
		-- nếu ko thì xóa
		DELETE from APZ_OKIR WHERE "DocEntry" = :id;
		DELETE from APZ_KIR1 WHERE "DocEntry" = :Id;
		
		SELECT 0, '' from DUMMY;
		RETURN;
	END IF; 

	IF :typeName = 'A'
	THEN
		select ifNULL((SELECT MAX("DocEntry") FROM APZ_OKIR), 0) + 1 into Id from DUMMY;
		UPDATE #APZ_OKIRData
			SET "UpdateDate" = current_date
			   ,"CreateDate" = current_date
			   ,"DocEntry"=:Id;
			   
	ELSEIF :typeName = 'U'
	THEN
        DELETE from APZ_OKIR WHERE "DocEntry"=:Id;
        DELETE from APZ_KIR1 WHERE "DocEntry"=:Id;
		UPDATE #APZ_OKIRdata SET "UpdateDate" = current_date;
	END IF;

	IF EXISTS (SELECT 1 FROM APZ_OKIR a INNER JOIN #APZ_OKIRdata b ON b."Name" = a."Name")
	THEN
		SELECT 123, 'Object is overlapping with an existing Object' from DUMMY;
		RETURN;
	END IF;
	
	INSERT INTO APZ_OKIR
	  SELECT * FROM #APZ_OKIRdata;

	IF exists (select 1 from M_TEMPORARY_TABLES where schema_Name = current_schema and Table_Name = '#APZ_KIR1')
	THEN
	
		create local temporary table #APZ_KIR1Data as (select top 0 b.* from (select 1 tmp from DUMMY) a left join APZ_KIR1 b on 1=2);
		exec 'insert into #APZ_KIR1Data select * from #APZ_KIR1';
		BEGIN
			declare lineNum int = 0;
			update #APZ_KIR1Data set "DocEntry"=:Id, "Code" = :code, "LineId" = null;
			WHILE exists(select 1 from #APZ_KIR1Data where "LineId" is null)
			DO
				UPDATE top 1 #APZ_KIR1Data SET "LineId"=:lineNum where "LineId" is null;
				lineNum = :lineNum + 1;
			END WHILE;
		END;

		INSERT INTO APZ_KIR1
			SELECT * FROM #APZ_KIR1Data;
	END if;
	
	SELECT 0, '' from DUMMY;
END




