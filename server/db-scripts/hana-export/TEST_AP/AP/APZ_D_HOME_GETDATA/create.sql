

CREATE PROCEDURE APZ_D_Home_GetData(IN userCode nvarchar(100))
AS
BEGIN

	declare storelist ParaStrListType;
	declare linkThumb nvarchar(2000);
	declare linkDownload nvarchar(2000);
	declare userId int = -1;
	declare tblValue table 
	 (
		 "ItemTotal" nvarchar(30), "ItemPerOrder" nvarchar(30), "RevenuePerOrder" nvarchar(30), "TotalRevenue" nvarchar(30), 
		 "NumberStore" nvarchar(30), "NumberStoreNew" nvarchar(30), "NumberVisit" nvarchar(30), "NumberVisitHaveOrder" nvarchar(30), 
		 "NumberOrder" nvarchar(30), "NumberOrderOpen" nvarchar(30), "DateType" char(1)
	 );

	--SET DATEFIRST 1;
	create local temporary table #tblUserInfo 
	(
		USERID int, U_NAME nvarchar(155), "Branch" smallint, "salesPrson" int, "empID" int, 
		"U_fullName" nvarchar(155), "LinkThumbnails" nvarchar(500), "LinkDownload" nvarchar(500)
	);
	
	userId := cast((select USERID from OUSR where USER_CODE = :userCode) as int);
	
	linkThumb := cast((select top 1 "LinkThumbnails" from APZ_D_UUPIM where "UserSign" = :userId and "ObjectType"='1470000171' order by "TimestampCreated" desc) as nvarchar);
	linkDownload := cast((select top 1 "LinkDownload" from APZ_D_UUPIM where "UserSign" = :userId and "ObjectType"='1470000171' order by "TimestampCreated" desc) as nvarchar);
	
	insert into #tblUserInfo
		select A.USERID,A.U_NAME,A."Branch", B."salesPrson" ,B."empID",
		ifnull(b."firstName", '') || ' ' || ifnull(b."middleName", '') || ' ' || ifnull(b."lastName", '') as "U_fullName",
		:linkThumb as "LinkThumbnails", :linkDownload as "LinkDownload"
		from OUSR A left join OHEM B on A.USERID = B."userId"
		where USER_CODE=:userCode;
	
	create local temporary table #tmpStore ("CardCode" nvarchar(50), "CardName" nvarchar(254));
	
	CALL APZ_D_FilterStores ('','','S',:userCode, '#tmpStore');
	
	INSERT INTO :storelist("Code") select "CardCode" from #tmpStore;
	
	INSERT INTO :tblValue ("ItemTotal", "ItemPerOrder", "RevenuePerOrder", "TotalRevenue", "NumberStore", "NumberStoreNew", "NumberVisit", "NumberVisitHaveOrder", "NumberOrder", "NumberOrderOpen", "DateType")
	SELECT * from apz_d_home_summary(:userCode,'W',:storelist);
	
	INSERT INTO :tblValue ("ItemTotal", "ItemPerOrder", "RevenuePerOrder", "TotalRevenue", "NumberStore", "NumberStoreNew", "NumberVisit", "NumberVisitHaveOrder", "NumberOrder", "NumberOrderOpen", "DateType")
	SELECT * from apz_d_home_summary(:userCode,'M',:storelist);
	
	INSERT INTO :tblValue ("ItemTotal", "ItemPerOrder", "RevenuePerOrder", "TotalRevenue", "NumberStore", "NumberStoreNew", "NumberVisit", "NumberVisitHaveOrder", "NumberOrder", "NumberOrderOpen", "DateType")
	SELECT * from apz_d_home_summary(:userCode,'Y',:storelist);

	select t0.*,t1.* from #tblUserInfo t0,:tblValue t1;

END




