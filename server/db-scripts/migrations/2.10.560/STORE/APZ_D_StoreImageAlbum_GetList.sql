
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE type='p' and id = OBJECT_ID('APZ_D_StoreImageAlbum_GetList'))
BEGIN
    DROP PROCEDURE APZ_D_StoreImageAlbum_GetList
END

GO

Create proc APZ_D_StoreImageAlbum_GetList
	@userSign int=-1,
	@fromDate nvarchar(100),
	@toDate nvarchar(100),
	@pageIndex int = 1,
	--#region namnx update 15/1/2021 lấy tất cả file của DMS
	@typeAttach NVARCHAR(100)='A',
	@empId int = -1
	--#endregion
as
begin
	declare @empUserId int = isnull((select top 1 userId from ohem where empID = @empId),-1)
	declare @lowLimit int
	set @lowLimit = 20 * @pageIndex - 20
	declare @upLimit int
	set @upLimit = @pageIndex * 20
	--#region namnx update 15/1/2021 lấy tất cả file của DMS
	-- nếu type là attach file
	IF @typeAttach='O'
	BEGIN
		select * from (select ROW_NUMBER() OVER (ORDER BY A.UploadDate desc) RowNum,A.* from dbo.APZ_D_UUPIM A 
		where 
		((not exists (select 1 from #tblAlbum)) or convert(nvarchar(100),A.AlbumId) in (select Id from #tblAlbum)) and
		((isnull(@fromDate,'') <> '' and @fromDate = @toDate and cast(A.UploadDate as date)=@fromDate)
		or ((isnull(@fromDate,'') = '' or cast(A.UploadDate as date)>=@fromDate) 
		and (isnull(@toDate,'') = '' or  cast(A.UploadDate as date)<=@toDate)))
		and ((@empUserId is null or @empUserId=-1) or A.UserSign=@empUserId)
		and ((not exists (select 1 from #tblObjectType)) or A.ObjectType in (select ObjectType from #tblObjectType))) B
		where (B.RowNum > @lowLimit AND B.RowNum <=@upLimit)
	END
	-- nếu type là ảnh store
	ELSE IF @typeAttach = 'S'
	BEGIN
		select * from (select ROW_NUMBER() OVER (ORDER BY A.UploadDate desc) RowNum,A.* from APZ_D_UPIM A 
		where 
		((not exists (select 1 from #tblAlbum)) or convert(nvarchar(100),A.AlbumId) in (select Id from #tblAlbum)) and
		((isnull(@fromDate,'') <> '' and @fromDate = @toDate and cast(A.UploadDate as date)=@fromDate)
		or ((isnull(@fromDate,'') = '' or cast(A.UploadDate as date)>=@fromDate) 
		and (isnull(@toDate,'') = '' or  cast(A.UploadDate as date)<=@toDate)))
		and ((@empUserId is null or @empUserId=-1) or A.UserSign=@empUserId)
		and ((not exists (select 1 from #tbl)) or A.CardCode in (select CardCode from #tbl))) B
		where (B.RowNum > @lowLimit AND B.RowNum <=@upLimit)
	END
	-- lấy tất cả
	ELSE
    BEGIN
		select * from (SELECT ROW_NUMBER() OVER (ORDER BY C.UploadDate desc) RowNum,* FROM (SELECT
		--
		Code,
		CardCode,
		VisitCode,
		CreateDate,
		CreateTime,
		UserSign,
		FileId,
		FileName,
		LinkDownload,
		LinkThumbnails,
		MimeType,
		Path,
		Size,
		WebUrl,
		UploadDate,
		UploadTime,
		IsAvatar,
		AlbumId,
		FileNameDrive,
		Extension,
		PathThumbnail,
		Domain,
		DateCreated,
		TimestampCreated,
		NULL AS ObjectType,
		NULL AS ObjectCode
		--
		FROM APZ_D_UPIM A 
		UNION ALL SELECT
		Code,
		NULL AS CardCode,
		NULL AS VisitCode,
		CreateDate,
		CreateTime,
		UserSign,
		FileId,
		FileName,
		LinkDownload,
		LinkThumbnails,
		MimeType,
		Path,
		Size,
		WebUrl,
		UploadDate,
		UploadTime,
		NULL AS IsAvatar,
		AlbumId,
		FileNameDrive,
		Extension,
		PathThumbnail,
		Domain,
		DateCreated,
		TimestampCreated,
		ObjectType,
		ObjectCode
		FROM APZ_D_UUPIM B) C
		where 
		((not exists (select 1 from #tblAlbum)) or convert(nvarchar(100),C.AlbumId) in (select Id from #tblAlbum)) and
		((isnull(@fromDate,'') <> '' and @fromDate = @toDate and cast(C.UploadDate as date)=@fromDate)
		or ((isnull(@fromDate,'') = '' or cast(C.UploadDate as date)>=@fromDate) 
		and (isnull(@toDate,'') = '' or  cast(C.UploadDate as date)<=@toDate)))
		and ((@empUserId is null or @empUserId=-1) or C.UserSign=@empUserId)
		and ((not exists (select 1 from #tbl)) or C.CardCode in (select CardCode from #tbl))
		and ((not exists (select 1 from #tblObjectType)) or C.ObjectType in (select ObjectType from #tblObjectType))) D
		where (D.RowNum > @lowLimit AND D.RowNum <=@upLimit)
	END
	--#endregion
END