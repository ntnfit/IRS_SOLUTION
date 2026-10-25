
CALL APZ_EXISTSPROC('APZ_D_StoreImageAlbum_GetList');

GO

Create procedure APZ_D_StoreImageAlbum_GetList(
	in userSign int default -1,
	in fromDate nvarchar(100),
	in toDate nvarchar(100),
	in pageIndex int default 1,
	--#region namnx update 15/1/2021 lấy tất cả file của DMS
	in typeAttach NVARCHAR(100) default 'A',
	in empId int default -1)
	--#endregion)
as
begin
	declare lowLimit int;
	declare upLimit int;
	declare empUserId int;
    declare empUserCode nvarchar(50);
    declare existsSelectCardCode nvarchar(5);
	
	select ifnull((select top 1 "userId" from ohem where "empID" = :empId),-1) into empUserId from DUMMY;
    select ifnull((select top 1 "USER_CODE" from ousr where "USERID" = :empUserId),'') into empUserCode from DUMMY;
	
    create local temporary table #tmpStore ("CardCode" nvarchar(50), "CardName" nvarchar(254));
	CALL APZ_D_FilterStores('','','S',:empUserCode, N'#tmpStore');

	create local temporary table #tblAlbumdata ("Id" int);
	exec 'drop table #tblAlbumdata';
	exec 'create local temporary table #tblAlbumdata as (select * from "#tblAlbum");';
	
	create local temporary table #tblObjectTypeData ("ObjectType" nvarchar(20));
	exec 'drop table #tblObjectTypeData';
	exec 'create local temporary table #tblObjectTypeData as (select * from "#tblObjectType");';
	
	create local temporary table #tbldata ("CardCode" nvarchar(50));
	exec 'drop table #tbldata';
	exec 'create local temporary table #tbldata as (select * from "#tbl");';

    if exists(select 1 from #tbldata) then
        existsSelectCardCode:= 'Y';
    else 
        existsSelectCardCode:= 'N';
    end if;
	
	lowLimit = 20 * :pageIndex - 20;
	upLimit = :pageIndex * 20;
	--#region namnx update 15/1/2021 lấy tất cả file của DMS
	-- nếu type là attach file
	IF :typeAttach='O'
	THEN
		select * from (select ROW_NUMBER() OVER (ORDER BY A."UploadDate" desc) "RowNum",A.* from APZ_D_UUPIM A 
			where 
			((not exists (select 1 from #tblAlbumData)) or cast(A."AlbumId" as nvarchar(100)) in (select "Id" from #tblAlbumData)) and
			((ifnull(:fromDate,'') <> '' and :fromDate = :toDate and cast(A."UploadDate" as date)=:fromDate)
			or ((ifnull(:fromDate,'') = '' or cast(A."UploadDate" as date)>=:fromDate) 
			and (ifnull(:toDate,'') = '' or  cast(A."UploadDate" as date)<=:toDate)))
			and ((:empUserId is null or :empUserId =-1) or A."UserSign"=:empUserId)
			and ((not exists (select 1 from #tblObjectTypeData))
            or (A."ObjectType" = '20600017' and ((:existsSelectCardCode = 'Y' and A."ObjectCode" in (select "CardCode" from #tbldata))
            or (:existsSelectCardCode <> 'Y' and A."ObjectCode" in (select "CardCode" from #tmpStore))))
             or (A."ObjectType" not in ('20600017') and A."ObjectType" in (select "ObjectType" from #tblObjectTypeData)))) B
			where (B."RowNum" > :lowLimit AND B."RowNum" <=:upLimit);
	-- nếu type là ảnh store
	ELSEIF :typeAttach = 'S'
	THEN
		select * from (select ROW_NUMBER() OVER (ORDER BY A."UploadDate" desc) "RowNum",A.* from APZ_D_UPIM A 
			where 
			((not exists (select 1 from #tblAlbumData)) or cast(A."AlbumId" as nvarchar(100)) in (select "Id" from #tblAlbumData)) and
			((ifnull(:fromDate,'') <> '' and :fromDate = :toDate and cast(A."UploadDate" as date)=:fromDate)
			or ((ifnull(:fromDate,'') = '' or cast(A."UploadDate" as date)>=:fromDate) 
			and (ifnull(:toDate,'') = '' or  cast(A."UploadDate" as date)<=:toDate)))
			and ((:empUserId is null or :empUserId =-1) or A."UserSign"=:empUserId)
			and ((not exists (select 1 from #tbldata)) or A."CardCode" in (select "CardCode" from #tbldata))) B
			where (B."RowNum" > :lowLimit AND B."RowNum" <=:upLimit);
	-- lấy tất cả
	ELSE
		select * from (SELECT ROW_NUMBER() OVER (ORDER BY C."UploadDate" desc) "RowNum",* FROM (SELECT
		--
		"Code",
		"CardCode",
		"VisitCode",
		"CreateDate",
		"CreateTime",
		"UserSign",
		"FileId",
		"FileName",
		"LinkDownload",
		"LinkThumbnails",
		"MimeType",
		"Path",
		"Size",
		"WebUrl",
		"UploadDate",
		"UploadTime",
		"IsAvatar",
		"AlbumId",
		"FileNameDrive",
		"Extension",
		"PathThumbnail",
		"Domain",
		"DateCreated",
		"TimestampCreated",
		NULL AS "ObjectType",
		NULL AS "ObjectCode"
		--
		FROM APZ_D_UPIM A 
        where ((not exists (select 1 from #tblAlbumData)) or cast(A."AlbumId" as nvarchar(100)) in (select "Id" from #tblAlbumData)) and
        ((ifnull(:fromDate,'') <> '' and :fromDate = :toDate and cast(A."UploadDate" as date)=:fromDate)
        or ((ifnull(:fromDate,'') = '' or cast(A."UploadDate" as date)>=:fromDate) 
        and (ifnull(:toDate,'') = '' or  cast(A."UploadDate" as date)<=:toDate)))
        and ((:empUserId is null or :empUserId =-1) or A."UserSign"=:empUserId)
        and ((not exists (select 1 from #tbldata)) or A."CardCode" in (select "CardCode" from #tbldata))
			
        UNION ALL SELECT
        "Code",
        NULL AS "CardCode",
        NULL AS "VisitCode",
        "CreateDate",
        "CreateTime",
        "UserSign",
        "FileId",
        "FileName",
        "LinkDownload",
        "LinkThumbnails",
        "MimeType",
        "Path",
        "Size",
        "WebUrl",
        "UploadDate",
        "UploadTime",
        NULL AS "IsAvatar",
        "AlbumId",
        "FileNameDrive",
        "Extension",
        "PathThumbnail",
        "Domain",
        "DateCreated",
        "TimestampCreated",
        "ObjectType",
        "ObjectCode"
        FROM APZ_D_UUPIM B
        where ((not exists (select 1 from #tblAlbumData)) or cast(B."AlbumId" as nvarchar(100)) in (select "Id" from #tblAlbumData)) and
			((ifnull(:fromDate,'') <> '' and :fromDate = :toDate and cast(B."UploadDate" as date)=:fromDate)
			or ((ifnull(:fromDate,'') = '' or cast(B."UploadDate" as date)>=:fromDate) 
			and (ifnull(:toDate,'') = '' or  cast(B."UploadDate" as date)<=:toDate)))
			and ((:empUserId is null or :empUserId =-1) or B."UserSign"=:empUserId)
			and ((not exists (select 1 from #tblObjectTypeData))
            or (B."ObjectType" = '20600017' and ((:existsSelectCardCode = 'Y' and B."ObjectCode" in (select "CardCode" from #tbldata))
            or (:existsSelectCardCode <> 'Y' and B."ObjectCode" in (select "CardCode" from #tmpStore))))
             or (B."ObjectType" not in ('20600017') and B."ObjectType" in (select "ObjectType" from #tblObjectTypeData)))) C) D
    where (D."RowNum" > :lowLimit AND D."RowNum" <=:upLimit);
	END IF;
	--#endregion
END