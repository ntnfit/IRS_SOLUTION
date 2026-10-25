
CREATE PROCEDURE APZ_SubActivities_AddTask
AS
BEGIN

		declare checkCount int = 0;
		DECLARE maxClgCode int = 0;
		DECLARE AtcEntry int = 0;
		DECLARE AbsEntry int = 0;
		
		select IFNULL((SELECT TOP 1 "ClgCode" from APZ_OCLG ORDER BY "ClgCode" DESC),0) into maxClgCode FROM DUMMY;
		
		------------------------------------- Lấy dữ liệu table tạm -------------------------------------------------
		create local temporary table #dtAPZ_OCLGData as (select top 0 * from APZ_OCLG);
		exec 'DROP TABLE #dtAPZ_OCLGData;';
		exec 'create local temporary table #dtAPZ_OCLGData as (select * from "#dtAPZ_OCLG");';
		------------------------------------- End lấy dữ liệu trong table tạm --------------------------------------

		--------------------------- Update lại id theo id trong hệ thống ------------------------------------------
		UPDATE #dtAPZ_OCLGData SET "ClgCode"= :maxClgCode+1, "OrderLine"= :maxClgCode+1;
		
		--when copy task
		--need to copy attachment too
		--and only in copy task
		--atcentry can be > 0
		--in order to copy attachment from old task to new task
		--need to get attachments of old task in ATC1
		--to do so
		--need to create a temp table #APZ_Attachment
		--by selecting data from old task AtcEntry

		--get AtcEntry
		SELECT IFNULL((SELECT TOP 1 "AtcEntry" from #dtAPZ_OCLGData),0) into AtcEntry FROM DUMMY;
		--check if AtcEntry > 0
		IF :AtcEntry >0
		THEN
			--if #APZ_Attachment exists
			--need to drop table before creating new
			--although it is impossible for temp table #APZ_Attachment to exists
			--when copy task
			select ifnull((select top 1 1 from M_TEMPORARY_TABLES where SCHEMA_NAME = current_Schema and Table_name = N'APZ_Attachment'),0) into checkCount from DUMMY;
			if :checkCount = 1
			THEN
				exec 'DROP TABLE #APZ_Attachment;';
			END IF;
			--create #APZ_Attachment
			create local temporary table "#APZ_Attachment" as (SELECT top 0 * FROM ATC1);
			insert into "#APZ_Attachment"
				SELECT * FROM ATC1 WHERE "AbsEntry"=:AtcEntry;

		END IF;

		call APZ_SetAttachment(AbsEntry);

		IF :AbsEntry !=0
		THEN
			UPDATE #dtAPZ_OCLGData SET "AtcEntry" = :AbsEntry;
		END IF;

		INSERT INTO APZ_OCLG
			SELECT * from #dtAPZ_OCLGData;

		--MERGE INTO APZ_OCLG as a
		--USING #dtAPZ_OCLG as b
		--ON a.ClgCode=b.ClgCode
		--WHEN NOT MATCHED BY TARGET THEN
		--INSERT (ClgCode,Details,[Action],Recontact,BeginTime,CntctDate,AssignedBy,AttendUser,OrderLine,parentTask, ResourceIdsInfo
		--, DocType, DocNum, DocEntry,CardCode,  PrjCode, AttendEmpl, endDate, ENDTime, Notes, OcrCode, OcrCode2, OcrCode3, OcrCode4, OcrCode5, [status],ProjectEntry
		--,GroupUser, GroupEmployee, [Priority], ProjectWBS)
		--VALUES (@maxClgCode+1,b.Details,'T',b.Recontact, b.BeginTime,b.CntctDate,b.AssignedBy,b.AttendUser,@maxClgCode+1,b.parentTask, b.ResourceIdsInfo
		--, b.DocType, b.DocNum, b.DocEntry, b.CardCode ,  b.PrjCode, b.AttendEmpl, b.endDate, b.ENDTime, b.Notes, b.OcrCode, b.OcrCode2, b.OcrCode3, b.OcrCode4, b.OcrCode5, b.[status], b.ProjectEntry
		--,b.GroupUser, b.GroupEmployee, b.[Priority], b.ProjectWBS);

		DROP TABLE #dtAPZ_OCLGData;
		SELECT :maxClgCode+1 from DUMMY;
		
	
		
END




