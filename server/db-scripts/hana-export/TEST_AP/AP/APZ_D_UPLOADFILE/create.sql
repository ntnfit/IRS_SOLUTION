

CREATE PROCEDURE APZ_D_UploadFile
AS
BEGIN

	CREATE LOCAL TEMPORARY TABLE #UUPIMLocal as (select top 0 * from APZ_D_UUPIM);
	exec 'drop table #UUPIMLocal';
	exec 'create local temporary table #UUPIMLocal as (select * from #APZ_D_UUPIM)';
		
	delete from APZ_D_UUPIM where "Code" in (select "Code" from #UUPIMLocal);
	
	INSERT INTO APZ_D_UUPIM ("Code", "CreateDate", "CreateTime", "UserSign", "FileId", "FileName", "LinkDownload", "LinkThumbnails", "MimeType", "Path", "Size", "WebUrl", 
		"UploadDate", "UploadTime", "AlbumId", "FileNameDrive", "ObjectType", "ObjectCode", "Extension", "PathThumbnail", "Domain", "DateCreated", "TimestampCreated")
	SELECT "Code", CURRENT_DATE, CURRENT_TIME, "UserSign", "FileId", "FileName", "LinkDownload", "LinkThumbnails", "MimeType", "Path", "Size", "WebUrl", 
		"UploadDate", CAST("UploadDate" AS time), "AlbumId", "FileNameDrive", "ObjectType", "ObjectCode", "Extension", 
		"PathThumbnail", "Domain", "DateCreated", "TimestampCreated" FROM #UUPIMLocal;
		
	if ::ROWCOUNT <= 0 THEN
		select '1470000169' as "ObjectType",406 as "Code",'Insert table APZ_D_UUPIM fail' as "Message" from dummy;
		return;
	else
		select '1470000169' as "ObjectType",200 as "Code",'' as "Message" from dummy;
		return;
	end if;
END




