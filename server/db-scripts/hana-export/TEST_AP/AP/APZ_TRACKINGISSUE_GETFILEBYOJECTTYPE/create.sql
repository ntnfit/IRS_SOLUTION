

CREATE PROCEDURE APZ_TrackingIssue_GetFileByOjectType(in objType varchar(50), objCode varchar(50))
AS
BEGIN

	select cast(m."Code" as nvarchar) as "CodeDisplay", m."FileName" ,m."Size", m."WebUrl", m."Path", m."ObjectCode", m."ObjectType" ,cast('Y' as char(1)) as "IsDMS", cast(0 as int) as "AbsEntry"
	,cast(0 as int) as "Line", m."LinkDownload" as "srcPath", m."WebUrl" as "trgtPath", m."FileName" as "FileNameAtc" ,cast(m."MimeType" as nvarchar(8)) as "FileExt", m."CreateDate" as "Date" 
	,cast(0 as int) as  UsrID ,cast('' as char(1)) as "Copied" ,cast('' as char(1)) as "Override", cast('' as nvarchar(254)) as "subPath", cast('' as nvarchar(100)) as "FreeText",
	cast(m."Code" as nvarchar(500)) as "Code", m."CreateDate", cast(m."CreateTime" as nvarchar(254)) as "CreateTime", m."UserSign", m."FileId", m."FileNameDrive", m."LinkDownload", m."LinkThumbnails", 
	m."MimeType", m."UploadDate", cast(m."UploadTime" as nvarchar(254)) as "UploadTime", m."AlbumId", m."Extension", m."PathThumbnail", m."Domain", m."DateCreated", m."TimestampCreated"
	from APZ_D_UUPIM m 
	where (ifnull(:objType, '') = '' or "ObjectType" = :objType) and (ifnull(:objCode, '') = '' or "ObjectCode" = :objCode)
 	union all
	select cast(a."AbsEntry" as  nvarchar) as "CodeDisplay", a."FileName",
	cast(0 as float) as "Size",
	CASE
    WHEN a."Copied" = 'Y' THEN ifnull(CAST(a."trgtPath" as nvarchar), '') || ifnull(CAST('\' as nvarchar), '') || ifnull(CAST(a."FileName" as nvarchar), '') || ifnull(CAST('.' as nvarchar), '') || ifnull(CAST(a."FileExt" as nvarchar), '')
    ELSE ifnull(CAST(a."srcPath" as nvarchar), '') || ifnull(CAST('\' as nvarchar), '') || ifnull(CAST(a."FileName" as nvarchar), '') || ifnull(CAST('.' as nvarchar), '') || ifnull(CAST(a."FileExt" as nvarchar), '')
	END as "WebUrl",
	ifnull(cast(a."srcPath" as nvarchar), '') || ifnull(cast(a."trgtPath" as nvarchar), '') as "Path", :objCode as "ObjectCode",
	cast('1470000130' as nvarchar(50)) as "ObjectType",
	cast('N' as char(1)) as "IsDMS", a."AbsEntry",
	a."Line", a."srcPath", a."trgtPath", a."FileName" || '.' || ifnull(cast(a."FileExt" as nvarchar), '') as "FileNameAtc", a."FileExt", a."Date", a."UsrID", a."Copied", a."Override", a."subPath", a."FreeText",
	cast('' as nvarchar(500)) as "Code",
	cast('' as datetime) as "CreateDate",
	cast('' as nvarchar(254)) as "CreateTime",
	cast(0 as int) as "UserSign",
	cast('' as nvarchar(100)) as "FileId",
	cast('' as nvarchar(100)) as "FileNameDrive",
	CASE
    WHEN a."Copied" = 'Y' THEN ifnull(CAST(a."trgtPath" as nvarchar), '') || ifnull(CAST('\' as nvarchar), '') || ifnull(CAST(a."FileName" as nvarchar), '') || ifnull(CAST('.' as nvarchar), '') || ifnull(CAST(a."FileExt" as nvarchar), '')
    ELSE ifnull(CAST(a."srcPath" as nvarchar), '') || ifnull(CAST('\' as nvarchar), '') || ifnull(CAST(a."FileName" as nvarchar), '') || ifnull(CAST('.' as nvarchar), '') || ifnull(CAST(a."FileExt" as nvarchar), '')
	END as "LinkDownload",
	cast('' as nvarchar(500)) as "LinkThumbnails",
	cast('' as nvarchar(100)) as "MimeType",
	cast('' as datetime) as "UploadDate",
	cast('' as nvarchar(254)) as "UploadTime",
	cast(0 as int) as "AlbumId",
	cast('' as nvarchar(100)) as "Extension",
	cast('' as nvarchar(500)) as "PathThumbnail",
	cast('' as nvarchar(100)) as "Domain",
	cast('' as datetime) as "DateCreated",
	cast(0 as bigint) as "TimestampCreated"
	from ATC1 a 
	where "AbsEntry" = (select "AttachedFiled" from APZ_TGIE where "Id" = :objCode);

END




