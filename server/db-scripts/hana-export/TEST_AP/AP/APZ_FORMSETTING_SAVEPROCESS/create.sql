
CREATE PROCEDURE Apz_FormSetting_SaveProcess (
)

AS
BEGIN
	-- khai báo biến
	DECLARE code NVARCHAR(20);
	DECLARE isExistOFST INT := 0;
	DECLARE isExistFST1 INT := 0;
	DECLARE isExistFST2 INT := 0;
	DECLARE id INT := 0;
	DECLARE df TINYINT := 0;
	DECLARE maxId INT := 0;
	DECLARE formid NVARCHAR(50);
	DECLARE itemid NVARCHAR(50);
	DECLARE user NVARCHAR(50);
	
	-- Temp_OFST = "#APZ_OFST", Temp_FST1 = "#APZ_FST1", Temp_FST2 = "#APZ_FST2";
	-- kiểm tra tồn tại các bảng tạm tạo từ C#
	SELECT COUNT(*) INTO isExistOFST FROM SYS.M_TEMPORARY_TABLES 
		WHERE SCHEMA_NAME = CURRENT_SCHEMA 
			AND IS_TEMPORARY = 'TRUE' 
			AND TEMPORARY_TABLE_TYPE = 'LOCAL'
			AND TABLE_NAME = '#APZ_OFST';
	SELECT COUNT(*) INTO isExistFST1 FROM SYS.M_TEMPORARY_TABLES 
		WHERE SCHEMA_NAME = CURRENT_SCHEMA 
			AND IS_TEMPORARY = 'TRUE' 
			AND TEMPORARY_TABLE_TYPE = 'LOCAL'
			AND TABLE_NAME = '#APZ_FST1';
	SELECT COUNT(*) INTO isExistFST2 FROM SYS.M_TEMPORARY_TABLES 
		WHERE SCHEMA_NAME = CURRENT_SCHEMA 
			AND IS_TEMPORARY = 'TRUE' 
			AND TEMPORARY_TABLE_TYPE = 'LOCAL'
			AND TABLE_NAME = '#APZ_FST2';
	-- khởi tạo bảng tạm theo bảng vật lý	
	CREATE LOCAL TEMPORARY TABLE #Tmp_OFST AS (SELECT TOP 0 * FROM APZ_OFST);
	CREATE LOCAL TEMPORARY TABLE #Tmp_FST1 AS (SELECT TOP 0 * FROM APZ_FST1);
	CREATE LOCAL TEMPORARY TABLE #Tmp_FST2 AS (SELECT TOP 0 * FROM APZ_FST2);
	
	IF isExistOFST = 0 THEN
		SELECT 0, 'Temp data is not exist!' FROM DUMMY;
		RETURN;
	END IF;
	EXEC 'DROP TABLE #Tmp_OFST';
	EXEC 'CREATE LOCAL TEMPORARY TABLE #Tmp_OFST AS (SELECT * FROM #APZ_OFST)';
	IF isExistFST1 > 0 THEN
		EXEC 'DROP TABLE #Tmp_FST1';
		EXEC 'CREATE LOCAL TEMPORARY TABLE #Tmp_FST1 AS (SELECT * FROM #APZ_FST1)';
	END IF;
	IF isExistFST2 > 0 THEN
	EXEC 'DROP TABLE #Tmp_FST2';
	EXEC 'CREATE LOCAL TEMPORARY TABLE #Tmp_FST2 AS (SELECT * FROM #APZ_FST2)';
	END IF;
	
	SELECT TOP 1 "ID" INTO id FROM #Tmp_OFST;
	SELECT TOP 1 "SetDefault" INTO df FROM #Tmp_OFST;
	
	-- Nếu update
	IF :id > 0 THEN 
		UPDATE #Tmp_OFST SET "UpdateTime" = NOW();
		IF isExistFST1 > 0 THEN 
			UPDATE #Tmp_FST1 SET "ID" = :id;
		END IF;
		IF isExistFST2 > 0 THEN
		 	UPDATE #Tmp_FST2 SET "ID" = :id;
		END IF;
		DELETE FROM APZ_OFST WHERE "ID" = :id;
		DELETE FROM APZ_FST1 WHERE "ID" = :id;
		DELETE FROM APZ_FST2 WHERE "ID" = :id;
	-- Nếu add new
	ELSE
		SELECT IFNULL(MAX(ID),0) INTO maxId FROM APZ_OFST;
		id := maxId + 1;
		UPDATE #Tmp_OFST 
			SET ID = :id, 
				"CreateDate" = NOW(), 
				"UpdateTime" = NULL, 
				"Usersign2" = NULL, 
				"BaseRef" = NULL, 
				"SetDefault" = :df;		
		IF isExistFST1 > 0 THEN 
			UPDATE #Tmp_FST1 SET "ID" = :id;
		END IF;
		IF isExistFST2 > 0 THEN
		 	UPDATE #Tmp_FST2 SET "ID" = :id;
		END IF;
	END IF;
	-- nếu chọn default cho bản ghi cần save
	IF :df = 1 THEN
		SELECT TOP 1 "FormId" INTO formid FROM #Tmp_OFST;
		SELECT TOP 1 "ItemId" INTO itemid FROM #Tmp_OFST;
		SELECT TOP 1 "UserSign" INTO user FROM #Tmp_OFST;
		UPDATE APZ_OFST 
			SET "SetDefault" = 0 
			WHERE "FormId" = :formid 
				AND "ItemId" = :itemid 
				AND "UserSign" = :user;
	END IF;
	
	-- inser dữ liệu vào bảng vật lý
	INSERT INTO APZ_OFST
	  SELECT * FROM #Tmp_OFST;
  	IF isExistFST1 > 0 THEN 
		INSERT INTO APZ_FST1 (ID, "VisIndex", "ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "FilterString", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "BackColor", "ForeColor", "SummaryType", "SummaryMode", "GroupSummaryType", "GroupSummaryShowInFooter", "SummaryFormat", "Expression", "Parent", "Key", "PivotArea") 
					   SELECT ID, "VisIndex", "ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "FilterString", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "BackColor", "ForeColor", "SummaryType", "SummaryMode", "GroupSummaryType", "GroupSummaryShowInFooter", "SummaryFormat", "Expression", "Parent", "Key", "PivotArea" 
		FROM #Tmp_FST1;
	END IF;
	IF isExistFST2 > 0 THEN
	 	INSERT INTO APZ_FST2
		  	SELECT * FROM #Tmp_FST2;
	END IF;
	
	--xóa các bảng tạm
	DROP TABLE #Tmp_OFST;
	DROP TABLE #Tmp_FST1;
	DROP TABLE #Tmp_FST2;
	SELECT 0, :id FROM DUMMY;
END;




