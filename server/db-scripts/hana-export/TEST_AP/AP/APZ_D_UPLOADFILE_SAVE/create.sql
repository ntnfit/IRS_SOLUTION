

CREATE PROCEDURE APZ_D_UploadFile_Save
AS
BEGIN

	declare objCode varchar(50) = '';
	declare objType varchar(50) = '';
	
	--Create local temporary table
	create local temporary table #APZ_D_UUPIMLocal as (select top 0 * from APZ_D_UUPIM);
	exec 'drop table #APZ_D_UUPIMLocal';
	exec 'create local temporary table #APZ_D_UUPIMLocal as (select * from #APZ_D_UUPIM)';
	
	--Get object code temp
	select (select top 1 "ObjectCode" from #APZ_D_UUPIMLocal) into objCode from dummy;
	--Get object type temp
	select (select top 1 "ObjectType" from #APZ_D_UUPIMLocal) into objType from dummy;
	
	--Delete record exists
	delete from APZ_D_UUPIM where "ObjectCode" = :objCode and "ObjectType" = :objType;
	
	--Add new temp record
	insert into APZ_D_UUPIM("Code", "UserSign", "CreateDate", "CreateTime", "UploadDate", "UploadTime", "ObjectType", "ObjectCode", "AlbumId", "FileId", "FileName",
	"FileNameDrive", "Domain", "Path", "PathThumbnail", "LinkDownload", "LinkThumbnails", "WebUrl", "MimeType", "Size", "Extension", "DateCreated", "TimestampCreated")
	select * from #APZ_D_UUPIMLocal;
	
	select :objCode, '' from dummy;
	
END



