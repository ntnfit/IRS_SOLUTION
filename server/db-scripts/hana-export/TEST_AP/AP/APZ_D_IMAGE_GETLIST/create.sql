

CREATE PROCEDURE APZ_D_Image_GetList(
	in fromDate timestamp,
	in toDate timestamp,
	in empID int default 0
)
AS
BEGIN

	declare userId int = -1;
	declare userName nvarchar(50) = '';
	
	userId := cast((select ifnull("userId", 0) from OHEM where "empID" = :empID) as int);
	userName := cast((select top 1 ifnull(USER_CODE, '')  from OUSR where USERID = :userId) as nvarchar);
	
	--Lấy ra danh sách cửa hàng theo tuyến bán hàng
	create local temporary table #tmpStore ("CardCode" nvarchar(50), "CardName" nvarchar(254));

	call APZ_D_FilterStores('', '', 'S', :userName, '#tmpStore');
	
	select a.*, b."CardName", cast("UploadDate" as date) as "UploadOnlyDate" 
	from APZ_D_UPIM a left join OCRD b on a."CardCode" = b."CardCode"
	where cast("UploadDate" as date) >= cast(:fromDate as date) and  cast("UploadDate" as date) <= cast(:toDate as date)
	and a."CardCode" in (select "CardCode" from #tmpStore) order by "UploadOnlyDate" DESC, "UploadTime" ASC;

END




