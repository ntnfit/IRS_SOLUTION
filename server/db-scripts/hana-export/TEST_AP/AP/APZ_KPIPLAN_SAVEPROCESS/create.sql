
Create PROCEDURE Apz_KPIPlan_SaveProcess(in typeName char(1) default '')
AS
BEGIN 

	DECLARE Id INT=0;
	DECLARE code NVARCHAR(50) = '';
	-- kiểm tra bảng tạm đã khởi tạo chưa
	IF not exists (select 1 from M_TEMPORARY_TABLES where schema_Name = current_schema and Table_Name = '#APZ_OKPN')
	THEN
		SELECT 404, ' Temp data is not exist' from DUMMY;
		RETURN;
	END IF;
	
	create local temporary table #APZ_OKPNData as (select top 0 * from APZ_OKPN);
	exec 'insert into #APZ_OKPNData select * from #APZ_OKPN';
	
	select ifnull((SELECT TOP 1 "Name" FROM #APZ_OKPNData), '') into code from DUMMY;
	IF :code = ''
	THEN
		SELECT 405, 'Name cannot be empty!' from DUMMY;
		RETURN;
	END IF;
	select ifnull((SELECT TOP 1 "DocEntry" FROM #APZ_OKPNDATA), 0) into Id from DUMMY;

	-- Nếu delete
	IF :typeName = 'D'
	THEN
		-- todo: check KPI Plan có nằm trong nghiệp vụ # nào ko
		
		-- nếu ko thì xóa
		DELETE from APZ_OKPN WHERE "DocEntry" = :Id;
		DELETE from APZ_KPN1 WHERE "DocEntry" = :Id;
		
		SELECT 0, '' from DUMMY;
		RETURN;
	END IF;

	IF :typeName = 'A'
	THEN
		SELECT IFNULL((SELECT MAX("DocEntry") FROM APZ_OKPN), 0) + 1 into id from DUMMY;
		UPDATE #APZ_OKPNData
			SET "UpdateDate" = current_date
			   ,"CreateDate" = current_date
			   ,"DocEntry"=:Id;
	ELSEIF :typeName = 'U'
	THEN
        DELETE from APZ_OKPN WHERE "DocEntry"=:Id;
        DELETE from APZ_KPN1 WHERE "DocEntry"=:Id;
		UPDATE #APZ_OKPNData SET "UpdateDate" = current_date;
	END IF;

	IF EXISTS (SELECT 1 FROM APZ_OKPN a INNER JOIN #APZ_OKPNData b ON b."Name" = a."Name")
	THEN
		SELECT 123, 'Object is overlapping with an existing Object' FROM DUMMY;
		RETURN;
	END IF;
	
	INSERT INTO APZ_OKPN
	  SELECT * FROM #APZ_OKPNData;

	IF exists (select 1 from M_TEMPORARY_TABLES where schema_Name = current_schema and Table_Name = '#APZ_KPN1')
	THEN
	
		create local temporary table #APZ_KPN1Data as (select top 0 b.* from (select 1 ff from DUMMY) a left join APZ_KPN1 b on 1=2);
		exec 'insert into #APZ_KPN1Data select * from #APZ_KPN1';
		BEGIN
			declare lineNum int = 0;
			update #APZ_KPN1Data set "DocEntry"=:Id, "LineId" = null;
			WHILE exists(select 1 from #APZ_KPN1Data where "LineId" is null)
			DO
				UPDATE top 1 #APZ_KPN1Data SET "LineId"=:lineNum where "LineId" is null;
				lineNum = :lineNum + 1;
			END WHILE;
		END;
		
		INSERT INTO APZ_KPN1
			SELECT * FROM #APZ_KPN1Data;
	END IF;
	
	SELECT 0, '' from DUMMY;
END




