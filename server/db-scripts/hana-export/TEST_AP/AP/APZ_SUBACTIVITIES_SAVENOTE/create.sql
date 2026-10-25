
CREATE PROCEDURE APZ_SubActivities_SaveNote
AS
BEGIN

	declare checkCount int = 0;
 	DECLARE Code int;
 	declare AbsEntry int = 0;
 	declare ClgCode int = 0;
 	
 	create local temporary table #APZ_NoteData as (select top 0 * from APZ_OCLG);
 	exec 'DROP TABLE #APZ_NoteData;';
	exec 'create local temporary table #APZ_NoteData as (select * from "#APZ_Note");';
 	
	SELECT IFNULL((select top 1 "ClgCode" from #APZ_NoteData),0) into Code FROM DUMMY;
	

	 /* Attachment Files */
	SELECT IFnull((SELECT ifnull("AtcEntry",0) FROM APZ_OCLG  WHERE  "ClgCode"= :Code),0) into AbsEntry from DUMMY;
		
	select IFNULL((select top 1 1 from M_TEMPORARY_TABLES where SCHEMA_NAME = current_schema and Table_Name = '#APZ_Attachment'), 0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		call APZ_SetAttachment(:AbsEntry);
		
	ELSE
		/* No file in documents delete all file entry of this document */
		DELETE FROM ATC1 WHERE "AbsEntry"=:AbsEntry;
		DELETE FROM OATC WHERE "AbsEntry"=:AbsEntry;
		AbsEntry = null;
	END IF;
	
	update #APZ_NoteData set "AtcEntry" = :AbsEntry;
	select ifnull((Select max("ClgCode") from APZ_OCLG),0) +1 into ClgCode from DUMMY;
	
	UPDATE #APZ_NoteData SET "ClgCode" = :ClgCode where ifnull("ClgCode",0) =0;

	delete from APZ_OCLG where "ClgCode" =:Code;
	insert into APZ_OCLG select top 1 * from #APZ_NoteData;
	
	select cast(:ClgCode as nvarchar) || ',' || ifnull(cast(:AbsEntry as nvarchar),0) from DUMMY;
END




