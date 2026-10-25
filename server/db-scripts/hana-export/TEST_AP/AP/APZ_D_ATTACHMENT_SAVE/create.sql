
--use for form attachment upload in IRS
CREATE PROCEDURE APZ_D_Attachment_Save
AS
BEGIN

	CREATE LOCAL TEMPORARY TABLE #UUPIMLocal as (select top 0 *,-1 as "SaveType" from APZ_D_UUPIM);
	exec 'drop table #UUPIMLocal';
	exec 'create local temporary table #UUPIMLocal as (select * from #APZ_D_UUPIM)';
	
	delete from APZ_D_UUPIM where ifnull("Code",'') != '' and "Code" in (select "Code" from #UUPIMLocal where "SaveType" = 2);
	
	INSERT INTO APZ_D_UUPIM ("Code", "CreateDate", "CreateTime", "UserSign", "FileId", "FileName", "LinkDownload", "LinkThumbnails", "MimeType", "Path", "Size", "WebUrl", 
		"UploadDate", "UploadTime", "AlbumId", "FileNameDrive", "ObjectType", "ObjectCode", "Extension", "PathThumbnail", "Domain", "DateCreated", "TimestampCreated")
	SELECT "Code", CURRENT_DATE, CURRENT_TIME, "UserSign", "FileId", "FileName", "LinkDownload", "LinkThumbnails", "MimeType", "Path", "Size", "WebUrl", 
		"UploadDate", CAST("UploadDate" AS time), "AlbumId", "FileNameDrive", "ObjectType", "ObjectCode", "Extension", 
		--save type (0 - new file is file add new, 1 - database is file already exists, 2 - deleted)
		"PathThumbnail", "Domain", "DateCreated", "TimestampCreated" FROM #UUPIMLocal where "SaveType" = 0;
		
	if ::ROWCOUNT <= 0 THEN
		select 406 as "Code",'Insert table APZ_D_UUPIM fail' as "Message" from dummy;
		return;
	else
		select 200 as "Code",'' as "Message" from dummy;
		return;
	end if;
END




