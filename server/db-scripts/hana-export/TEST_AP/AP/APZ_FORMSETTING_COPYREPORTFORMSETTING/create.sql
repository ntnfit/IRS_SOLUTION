
CREATE PROCEDURE Apz_FormSetting_CopyReportFormSetting (
	IN listUser NVARCHAR(5000)
,	   idSetting INT
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
BEGIN
	-- khai báo biến	
	DECLARE formid nvarchar(50);
	DECLARE itemid nvarchar(50);
	DECLARE count INT := 0;
	DECLARE idx INT := 1;
	DECLARE userName nvarchar(50);
	DECLARE id INT := 0;
	DECLARE isExistOFST INT := 0;
	DECLARE isExistDf INT := 0;
	DECLARE df TINYINT := 0;
	DECLARE isExistFST2 INT := 0;
	DECLARE nextId INT := 0;
	
	CREATE LOCAL TEMPORARY COLUMN TABLE #tblUser(
		"Id" INT 
	  , "UserName" nvarchar(100)
	  );
	INSERT INTO #tblUser ("Id", "UserName")
	  SELECT ROW_NUMBER() OVER ( ORDER BY a.DATA), a.DATA
		  FROM Split(:listUser, ',') a;	
	--lấy ra formid và itemId của setting
	SELECT "FormId" INTO formid FROM APZ_OFST WHERE ID = :idSetting;
	SELECT "ItemId" INTO itemid FROM APZ_OFST WHERE ID = :idSetting;

	SELECT COUNT(*) INTO count FROM #tblUser;
	WHILE(idx <= :count) DO
		-- lấy ra username
		SELECT "UserName" INTO userName FROM #tblUser WHERE "Id" = :idx;
		SELECT COUNT(*) INTO isExistOFST FROM APZ_OFST WHERE "UserSign" = :userName AND "BaseRef" = :idSetting;
		--nếu đã có -> cập nhật
		IF isExistOFST > 0 THEN
			SELECT ID INTO id FROM APZ_OFST WHERE "UserSign" = :userName AND "BaseRef" = :idSetting;
			-- xóa bản ghi đã có
			DELETE FROM APZ_OFST WHERE ID = :id;
			DELETE FROM APZ_FST1 WHERE ID = :id;
			DELETE FROM APZ_FST2 WHERE ID = :id;
			
			-- nếu itemID này chưa có defaul setting thì lấy bản ghi mới làm default		
			SELECT COUNT(*) INTO isExistDf 
				FROM APZ_OFST 
				WHERE "FormId" = :formid 
					AND "ItemId" = :itemid 
					AND "UserSign" = :userName 
					AND "SetDefault" = 1;
			IF isExistDf = 0 THEN
				df := 1;
			ELSE
				df := 0;
			END IF;
			-- thêm mới lại
			INSERT INTO APZ_OFST (ID, "Name", "FormId", "ItemId", "SetDefault", "UserSign", "CreateDate", "UpdateTime", "Usersign2", "BaseRef") 
				SELECT :id, "Name", "FormId", "ItemId", :df, :userName, NOW(), NULL, "UserSign", :idSetting 
					FROM APZ_OFST 
					WHERE ID = :idSetting;
					
			INSERT INTO APZ_FST1 (ID, "VisIndex", "ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "FilterString", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "BackColor", "ForeColor", "SummaryType", "GroupSummaryType", "GroupSummaryShowInFooter", "SummaryFormat", "Expression", "Parent", "Key", "PivotArea") 
				SELECT :id, "VisIndex", "ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "FilterString", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "BackColor", "ForeColor", "SummaryType", "GroupSummaryType", "GroupSummaryShowInFooter", "SummaryFormat", "Expression", "Parent", "Key", "PivotArea" 
					FROM APZ_FST1 
					WHERE ID = :idSetting;
					
			SELECT COUNT(*) INTO isExistFST2 FROM APZ_FST2 WHERE ID = :idSetting;
			IF isExistFST2 > 0 THEN
				INSERT INTO APZ_FST2 (ID, "ColumnName", "Description", "Value", "BackColor", "ForeColor") 
					SELECT :id, "ColumnName", "Description", "Value", "BackColor", "ForeColor" 
						FROM APZ_FST2 
						WHERE ID = :idSetting;
			END IF;
		-- chưa có -> thêm mới
		ELSE
			-- lấy ra idmax
			SELECT MAX(ID) + 1 INTO nextId FROM APZ_OFST;
			-- nếu itemID này chưa có defaul setting thì lấy bản ghi mới làm default
			SELECT COUNT(*) INTO isExistDf 
				FROM APZ_OFST 
				WHERE "FormId" = :formid 
					AND "ItemId" = :itemid 
					AND "UserSign" = :userName 
					AND "SetDefault" = 1;
			IF isExistDf = 0 THEN
				df := 1;
			ELSE
				df := 0;
			END IF;
			
			INSERT INTO APZ_OFST (ID, "Name", "FormId", "ItemId", "SetDefault", "UserSign", "CreateDate", "UpdateTime", "Usersign2", "BaseRef") 
				SELECT :nextId, "Name", "FormId", "ItemId", :df, :userName, NOW(), NULL, "UserSign", :idSetting 
					FROM APZ_OFST 
					WHERE ID = :idSetting;
					
			INSERT INTO APZ_FST1 (ID, "VisIndex", "ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "FilterString", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "BackColor", "ForeColor", "SummaryType", "GroupSummaryType", "GroupSummaryShowInFooter", "SummaryFormat", "Expression", "Parent", "Key", "PivotArea") 
				SELECT :nextId, "VisIndex", "ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "FilterString", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "BackColor", "ForeColor", "SummaryType", "GroupSummaryType", "GroupSummaryShowInFooter", "SummaryFormat", "Expression", "Parent", "Key", "PivotArea" 
					FROM APZ_FST1 
					WHERE ID = :idSetting;
					
			SELECT COUNT(*) INTO isExistFST2 FROM APZ_FST2 WHERE ID = :idSetting;
			IF isExistFST2 > 0 THEN
				INSERT INTO APZ_FST2 (ID, "ColumnName", "Description", "Value", "BackColor", "ForeColor") 
					SELECT :nextId, "ColumnName", "Description", "Value", "BackColor", "ForeColor" 
						FROM APZ_FST2 
						WHERE ID = :idSetting;
			END IF;
		END IF;
		idx := :idx + 1;
	END WHILE;
	DROP TABLE #tblUser;
END;




