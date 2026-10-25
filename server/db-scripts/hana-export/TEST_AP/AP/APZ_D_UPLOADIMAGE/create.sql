

CREATE PROCEDURE APZ_D_UploadImage
AS
BEGIN
	
	CREATE LOCAL TEMPORARY TABLE #UPIMLocal as (select top 0 * from APZ_D_UPIM);
	exec 'drop table #UPIMLocal';
	exec 'create local temporary table #UPIMLocal as (select * from #APZ_D_UPIM)';
	
	delete from APZ_D_UPIM where "Code" in (select "Code" from #UPIMLocal);
	
	INSERT INTO APZ_D_UPIM ("Code", "CardCode", "VisitCode", "CreateDate", "CreateTime", "UserSign", "FileId", "FileName", "LinkDownload", "LinkThumbnails", "MimeType", "Path", "Size", 
		"WebUrl", "UploadDate", "UploadTime", "IsAvatar", "AlbumId", "FileNameDrive", "Extension", "PathThumbnail", "Domain", "DateCreated", "TimestampCreated") 
	SELECT "Code", "CardCode", "VisitCode", CURRENT_DATE, CURRENT_TIME, "UserSign", "FileId", "FileName", "LinkDownload", 
		"LinkThumbnails", "MimeType", "Path", "Size", "WebUrl", 
		"UploadDate", CAST("UploadDate" AS time), "IsAvatar", "AlbumId", "FileNameDrive", "Extension", 
		"PathThumbnail", "Domain", "DateCreated", "TimestampCreated" FROM #UPIMLocal;

	if ::ROWCOUNT <= 0 then
		select '1470000169' as "ObjectType", 406 as "Code",'Insert table APZ_D_UPIM fail' as "Message" from dummy;
		return;
	else
		select '1470000169' as "ObjectType", 200 as "Code",'' as "Message" from dummy;
		return;
	end if;
END



