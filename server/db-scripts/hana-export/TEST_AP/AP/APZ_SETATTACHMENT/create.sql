
CREATE PROCEDURE APZ_SetAttachment(out AbsEntry INT)
AS
BEGIN
	
	DECLARE maxAbsEntry int = 0;
	declare checkCount int = 0;
	
	select ifnull((select top 1 1 from M_TEMPORARY_TABLES where SCHEMA_NAME = current_Schema and Table_name = N'#APZ_Attachment'),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		create local temporary table #APZ_AttachmentData as (select top 0 * from ATC1);
		exec 'drop table #APZ_AttachmentData;';
		exec 'create local temporary table #APZ_AttachmentData as (select * from "#APZ_Attachment");';
		
		select IFNULL((select top 1 1 from #APZ_AttachmentData),0) into checkCount from dummy;
		IF :checkCount = 1
		THEN
			IF ifnull(:AbsEntry,0) <= 0
			THEN /* Add New */
				-- Set new attachment absEntry to series table
				--get max absentry from OATC
				--DECLARE @maxAbsEntry int = (select isnull(max(absEntry),0)+1 From OATC);
				--if no autokey available
				--start with absentry = 1
				select (select ifnull(max("AbsEntry"),0)+1 From OATC) into maxAbsEntry FROM DUMMY;
				--DECLARE @maxAbsEntry int = ISNULL((SELECT TOP 1 AutoKey FROM ONNM WHERE ObjectCode='221'),1);
				--update OATC
				insert into OATC ("AbsEntry") VALUES (:maxAbsEntry);
				--update AutoKey
                Update ONNM set "AutoKey" = :maxAbsEntry+1 where "ObjectCode"='221';
                AbsEntry = :maxAbsEntry; -- Get new absEntry

				--IF @AbsEntry > 0 -- new AbsEntry is valid
				--BEGIN
				-- update new AbsEntry to attachment temp table and EmpRelatives temp table
				Update #APZ_AttachmentData SET "AbsEntry"=:AbsEntry;
				-- Insert new file entry
				INSERT INTO ATC1 ("AbsEntry", "Line", "srcPath", "trgtPath", "FileName", "FileExt", "Date", "UsrID", "Copied", "Override", "subPath") 
					SELECT a."AbsEntry", a."Line", a."srcPath", a."trgtPath", a."FileName", a."FileExt", a."Date", a."UsrID", a."Copied", a."Override", a."subPath" FROM #APZ_AttachmentData a;
				--END
			--if(@AbsEntry >0)
			ELSE
				 /* Update */
				-- delete all file entry of this document
				--for safety
				--#APZ_Attachment AbsEntry might be different from @AbsEntry
				--=> data insert into ATC1 from #APZ_Attachment may not link to @AbsEntry
				--=>update #APZ_Attachment with @AbsEntry for safety
				Update #APZ_AttachmentData SET "AbsEntry"=:AbsEntry;
				DELETE FROM ATC1 WHERE "AbsEntry"=:AbsEntry;
				-- Insert new file entry
				INSERT INTO ATC1 ("AbsEntry", "Line", "srcPath", "trgtPath", "FileName", "FileExt", "Date", "UsrID", "Copied", "Override", "subPath") 
					SELECT a."AbsEntry", a."Line", a."srcPath", a."trgtPath", a."FileName", a."FileExt", a."Date", a."UsrID", a."Copied", a."Override", a."subPath" FROM #APZ_AttachmentData a;
			END IF;
		--if exists #APZ_Attachment
		--but not exists (Select 1 from #APZ_Attachment))
		--=> only the structure from gv_File is sent
		--=>data from gv_File has been deleted
		--=>remove all
		--possibly another case
		--where no attachment is browsed 
		--then absentry will be 0
		--=>delete all from ATC1 where absentry = 0 won't affect data
		ELSE
			 /* No file in documents delete all file entry of this document */
			DELETE FROM ATC1 WHERE "AbsEntry"=:AbsEntry;
		END IF;
	END IF;

	  --if @AbsEntry !=0
	  --Update #dtAPZ_OCLG SET AtcEntry=@AbsEntry
END




