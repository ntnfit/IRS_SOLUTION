
CALL APZ_EXISTSPROC('APZ_D_Employees_SearchData');

GO

CREATE PROCEDURE APZ_D_Employees_SearchData(
	in lowLimit int default 0,
	in upLimit int default 0,
	in searchKey nvarchar(100) default '',
	in empID int default -1,
	in userId int default -1
)
AS
BEGIN
	
	
	create local temporary table #tmpEmployees ("EmpId" int);
	call APZ_D_FilterEmployees(:userId, '#tmpEmployees');
	
	create local temporary table #tblData
	(
		"row_num" integer, 
		"empID" integer, 
		"userId" integer, 
		"userCode" nvarchar(100), 
		"lastName" nvarchar(50), 
		"firstName" nvarchar(50), 
		"middleName" nvarchar(50), 
		"sex" char(1),
		"workStreet" nvarchar(100), 
		"email" nvarchar(100), 
		"mobile" nvarchar(20), 
		"birthDate" timestamp, 
		"FileName" nvarchar(100), 
		"LinkDownload" nvarchar(500), 
		"LinkThumbnails" nvarchar(500), 
		"Size" float, 
		"UploadDate" timestamp, 
		"U_fullName" nvarchar(100), 
		"route" nvarchar(5000), 
		"online" char(1), 
		"numStoreManage" integer, 
		"numStoreVisitToday" integer, 
		"nearestVisit" nvarchar(100), 
		"nearestVisitTime" nvarchar(100), 
		"liabilities" decimal(16, 9), 
		"numOfOrder" integer
	);

	insert into #tblData("row_num", "empID", "userId", "userCode", "lastName", "firstName", "middleName", "sex", "workStreet", "email", "mobile", "birthDate", "FileName", "LinkDownload",
	"LinkThumbnails", "Size", "UploadDate", "U_fullName", "route", "online", "numStoreManage" 
	,"numStoreVisitToday"
	, "nearestVisit"
	, "nearestVisitTime"
	, "liabilities"
	, "numOfOrder"
	)
	select ROW_NUMBER() OVER (order by a."empID") as "row_num", a."empID", a."userId", (select USER_CODE from OUSR where USERID = a."userId") as "userCode", a."lastName", a."firstName", a."middleName", a."sex", a."workStreet", a."email", a."mobile",
	a."birthDate", 
	(select "FileName" from APZ_D_UUPIM where "UserSign" = a."userId" and "ObjectType" = '1470000171') as "FileName", 
	(select "LinkDownload" from APZ_D_UUPIM where "UserSign" = a."userId" and "ObjectType" = '1470000171') as "LinkDownload", 
	(select "LinkThumbnails" from APZ_D_UUPIM where "UserSign" = a."userId" and "ObjectType" = '1470000171') as "LinkThumbnails", 
	(select "Size" from APZ_D_UUPIM where "UserSign" = a."userId" and "ObjectType" = '1470000171') as "Size", 
	(select "UploadDate" from APZ_D_UUPIM where "UserSign" = a."userId" and "ObjectType" = '1470000171') as "UploadDate", 
	ifnull(a."firstName", '') || ' ' || ifnull(a."middleName", '') || ' ' || ifnull(a."lastName", ''),
	ifnull((select STRING_AGG("RouteCode", ',') from (select distinct "RouteCode" from 
	(
		select str1."RouteCode" from APZ_D_STR1 str1 inner join OHEM t0 on str1."SuperVisor" = t0."empID" where t0."empID" = a."empID" group by str1."RouteCode" 
		union all select str4."RouteCode" from APZ_D_STR4 str4 inner join OHEM t0 on str4."EmpId" = t0."empID" where t0."empID" = a."empID" group by str4."RouteCode"
	))), ''),
	'N', 0
	,(select count(*) from APZ_D_VISIT where "UserSign" = a."userId" and cast("FinishedDate" as date) = cast(current_date as date))
	,(select FIRST_VALUE("StoreName" ORDER BY "FinishedDate" DESC) 
	from APZ_D_VISIT where "UserSign" = a."userId" and TO_VARCHAR("StartDate",'yyyy-MM-dd') = TO_VARCHAR(current_date,'yyyy-MM-dd'))
	,(select FIRST_VALUE(TO_VARCHAR("FinishedDate",'HH:mm dd/MM/yyyy') ORDER BY "FinishedDate" DESC) 
	from APZ_D_VISIT where "UserSign" = a."userId" and TO_VARCHAR("StartDate",'yyyy-MM-dd') = TO_VARCHAR(current_date,'yyyy-MM-dd'))
	, 0
	,ifnull(cast((select count(*) from ORDR where "UserSign" = a."userId") as int) + cast((select count(*) from OINV where "UserSign" = a."userId") as int), 0)
	from OHEM a 
	where (:empID = -1 or a."empID" = :empID)
	and (a."empID" in (select distinct "EmpId" from #tmpEmployees))
	and
	(((ifnull(:searchKey, '') = '' and (a."empID" is null or cast(a."empID" as nvarchar) like N'%' || ifnull(:searchKey, '') || '%')) 
	or (ifnull(:searchKey, '') <> '' and (cast(a."empID" as nvarchar) like N'%' || :searchKey || '%'))) or
	((ifnull(:searchKey, '') = '' and 
	((ifnull(a."firstName", '') || ' ' || ifnull(a."middleName", '') || ' ' || ifnull(a."lastName", ''))  is null 
	or (ifnull(a."firstName", '') || ' ' || ifnull(a."middleName", '') || ' ' || ifnull(a."lastName", '')) like N'%' || ifnull(:searchKey, '') || '%')) 
	or (ifnull(:searchKey, '') <> '' and 
	((ifnull(a."firstName", '') || ' ' || ifnull(a."middleName", '') || ' ' || ifnull(a."lastName", '')) like N'%' || ifnull(:searchKey, '') || '%'))))
	group by a."empID", a."userId", a."lastName", a."firstName", a."middleName", a."sex", a."workStreet", a."email", a."mobile", a."birthDate";
	
	select * from #tblData order by "empID";
	
END