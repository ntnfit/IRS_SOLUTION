
CALL APZ_EXISTSPROC('Apz_UDR_SaveProcess');

GO


CREATE PROCEDURE Apz_UDR_SaveProcess (
	IN 	type CHAR(1) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
BEGIN
	-- khai báo biến
	DECLARE code NVARCHAR(60);
	DECLARE isExistOUDR INT := 0;
	DECLARE isExistUDR1 INT := 0;
	DECLARE isExistUDR2 INT := 0;
	DECLARE fatherid INT := 0;
	DECLARE maxid INT := 0;
	
	-- Temp_OUDR = "#APZ_OUDR", Temp_UDR1 = "#APZ_UDR1", Temp_UDR2 = "#APZ_UDR2"
	-- kiểm tra tồn tại các bảng tạm tạo từ C#
	SELECT COUNT(*) INTO isExistOUDR FROM SYS.M_TEMPORARY_TABLES 
		WHERE SCHEMA_NAME = CURRENT_SCHEMA 
			AND IS_TEMPORARY = 'TRUE' 
			AND TEMPORARY_TABLE_TYPE = 'LOCAL'
			AND TABLE_NAME = '#APZ_OUDR';
	SELECT COUNT(*) INTO isExistUDR1 FROM SYS.M_TEMPORARY_TABLES 
		WHERE SCHEMA_NAME = CURRENT_SCHEMA 
			AND IS_TEMPORARY = 'TRUE' 
			AND TEMPORARY_TABLE_TYPE = 'LOCAL'
			AND TABLE_NAME = '#APZ_UDR1';
	SELECT COUNT(*) INTO isExistUDR2 FROM SYS.M_TEMPORARY_TABLES 
		WHERE SCHEMA_NAME = CURRENT_SCHEMA 
			AND IS_TEMPORARY = 'TRUE' 
			AND TEMPORARY_TABLE_TYPE = 'LOCAL'
			AND TABLE_NAME = '#APZ_UDR2';
	-- khởi tạo bảng tạm theo bảng vật lý	
	CREATE LOCAL TEMPORARY TABLE #Tmp_OUDR AS (SELECT TOP 0 * FROM APZ_OUDR);
	CREATE LOCAL TEMPORARY TABLE #Tmp_UDR1 AS (SELECT TOP 0 * FROM APZ_UDR1);
	CREATE LOCAL TEMPORARY TABLE #Tmp_UDR2 AS (SELECT TOP 0 * FROM APZ_UDR2);
	
	IF isExistOUDR = 0 THEN
		SELECT 0, 'Temp data is not exist!' FROM DUMMY;
		RETURN;
	END IF;
	EXEC 'INSERT INTO #Tmp_OUDR (SELECT * FROM #APZ_OUDR)';
	IF isExistUDR1 > 0 THEN
		EXEC 'INSERT INTO #Tmp_UDR1 (SELECT * FROM #APZ_UDR1)';
	END IF;
	IF isExistUDR2 > 0 THEN
	EXEC 'INSERT INTO #Tmp_UDR2 (SELECT * FROM #APZ_UDR2)';
	END IF;
	
	SELECT TOP 1 trim("Code") INTO code FROM #Tmp_OUDR;
	
	if ifnull(:code,'')='' then
		select 1, 'Unique Id not found' from dummy;
		return;
	end if;
	
	-- Nếu delete
	IF :type = 'D' THEN 
		DELETE FROM APZ_OUDR WHERE "Code" = :code;
		DELETE FROM APZ_UDR1 WHERE "Code" = :code;
		DELETE FROM APZ_UDR2 WHERE "Code" = :code;
		--xóa ở phân quyền
		DELETE FROM APZ_OMNI WHERE "Code" = 'frm_' || :code;
		DELETE FROM OPRM WHERE "menucode" = 'frm_' || :code;
		-- xóa ở formsetting
		DELETE FROM APZ_FST1 WHERE ID IN (SELECT ID FROM APZ_OFST WHERE "FormId" = N'oUserDefinedReports' AND "ItemId" = :code);
		DELETE FROM APZ_FST2 WHERE ID IN (SELECT ID FROM APZ_OFST WHERE "FormId" = N'oUserDefinedReports' AND "ItemId" = :code);
		DELETE FROM APZ_OFST WHERE ID IN (SELECT ID FROM APZ_OFST WHERE "FormId" = N'oUserDefinedReports' AND "ItemId" = :code);
		
		SELECT 0, '' FROM DUMMY;
		RETURN;	
	-- Nếu update
	ELSEIF :type = 'U' THEN 
		UPDATE #Tmp_OUDR SET "LstUpdDate" = NOW();
		IF isExistUDR1 > 0 THEN 
			UPDATE #Tmp_UDR1 SET "Code" = :code;
		END IF;
		IF isExistUDR2 > 0 THEN
		 	UPDATE #Tmp_UDR2 SET "Code" = :code;
		END IF;
		DELETE FROM APZ_OUDR WHERE "Code" = :code;
		DELETE FROM APZ_UDR1 WHERE "Code" = :code;
		DELETE FROM APZ_UDR2 WHERE "Code" = :code;
		-- update report ở bảng phân quyền
		SELECT top 1 "Id" INTO fatherid 
			FROM APZ_OMNI
			WHERE "Code" = (SELECT TOP 1 "FatherMenu" FROM #Tmp_OUDR);
		UPDATE a SET a."Name" = "MenuCapt", a."FatherId" = :fatherid 
			FROM APZ_OMNI a INNER JOIN #Tmp_OUDR b ON 1=1 where a."Code" = 'frm_' || :code; 
	-- Nếu add new
	ELSE
		
		UPDATE #Tmp_OUDR SET "LstUpdDate" = NOW();
		IF isExistUDR1 > 0 THEN 
			UPDATE #Tmp_UDR1 SET "Code" = :code;
		END IF;
		IF isExistUDR2 > 0 THEN
		 	UPDATE #Tmp_UDR2 SET "Code" = :code;
		END IF;
		
		-- Kiểm tra code có trùng trong bảng phân quyền ko
		IF EXISTS (SELECT 1 FROM APZ_OMNI a INNER JOIN #Tmp_OUDR b ON 'frm_' || b."Code" = a."Code") THEN
			SELECT 123, 'Object is overlapping with an existing Object (APZ_OMNI)' FROM DUMMY;
			RETURN;
		END IF;
		
		SELECT MAX("Id") INTO maxid FROM APZ_OMNI;
		select ifnull((SELECT top 1 "Id" 
			FROM APZ_OMNI
			WHERE "Code" = (SELECT TOP 1 "FatherMenu" FROM #Tmp_OUDR)), -1) into fatherid from dummy;
			
		if :fatherid = -1 then
			SELECT 100100, 'Please choose menu first' FROM DUMMY;
			return;
		END IF;
			
		INSERT INTO APZ_OMNI ("Id", "FatherId", "Code", "Name", "LanguageKey", "MenuType") 
			SELECT TOP 1 :maxid + 1, :fatherid, 'frm_' || "Code", "Name", 'EN-en', 'C' FROM #Tmp_OUDR; 
	END IF;
	
	-- inser dữ liệu vào bảng vật lý
	INSERT INTO APZ_OUDR
	  SELECT * FROM #Tmp_OUDR;
  	IF isExistUDR1 > 0 THEN 
		INSERT INTO APZ_UDR1
			SELECT * FROM #Tmp_UDR1;
	END IF;
	IF isExistUDR2 > 0 THEN
	 	INSERT INTO APZ_UDR2
		  	SELECT * FROM #Tmp_UDR2;
	END IF;
	
	--xóa các bảng tạm
	DROP TABLE #Tmp_OUDR;
	DROP TABLE #Tmp_UDR1;
	DROP TABLE #Tmp_UDR2;
	SELECT 0, '' FROM DUMMY;
END;