
CALL APZ_EXISTSPROC('APZ_D_Employees_SearchData');

GO

--call APZ_D_Employees_SearchData (0,0,'',-1, 1)
CREATE PROCEDURE APZ_D_Employees_SearchData(
	in lowLimit int default 0,
	in upLimit int default 0,
	in searchKey nvarchar(100) default '',
	in empID int default -1,
	in userId int default -1
)
AS
BEGIN
	declare tblOrder table ("DocEntry" int, "DocTotal" decimal(28, 6), "U_Status" nvarchar(10),"U_TypeDH" nvarchar(10), "UserSign" int);
    declare tblDelivery table ("DocEntry" int, "DocTotal" decimal(28, 6), "U_Status" nvarchar(10),"UserSign" int);

	INSERT INTO :tblDelivery SELECT "DocEntry", "DocTotal", "U_Status","UserSign" FROM ODLN WHERE "U_Status" in ('A') and "CANCELED" = 'N';
    INSERT INTO :tblOrder SELECT "DocEntry", "DocTotal", "U_Status", "U_TypeDH","UserSign" FROM ORDR WHERE "U_Status" in ('A','D','C') and "CANCELED" = 'N';

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
		"liabilities" decimal(28, 6), 
		"numOfOrder" integer,
        "salesGNG" decimal(28, 6),
        "salesTNO" decimal(28, 6),
        "salesDAT" decimal(28, 6),
        "totalActual" decimal(28, 6),
        "totalPlan" decimal(28, 6)
	);

	insert into #tblData("row_num", "empID", "userId", "userCode", "lastName", "firstName", "middleName", "sex", "workStreet", "email", "mobile", "birthDate", "FileName", "LinkDownload",
	"LinkThumbnails", "Size", "UploadDate", "U_fullName", "route", "online", "numStoreManage" 
	,"numStoreVisitToday"
	, "nearestVisit"
	, "nearestVisitTime"
	, "liabilities"
	, "numOfOrder"
    , "salesGNG"
    , "salesTNO"
    , "salesDAT"
    , "totalActual"
    , "totalPlan"
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
		select str1."RouteCode" from APZ_D_STR1 str1 inner join OHEM t0 
		on str1."SuperVisor" = t0."empID" or CompareSplit(cast(t0."empID" as nvarchar),str1."SalesAdmin",';') = 1
		where t0."empID" = a."empID" group by str1."RouteCode" 
		union all select str4."RouteCode" from APZ_D_STR4 str4 inner join OHEM t0 on str4."EmpId" = t0."empID" where t0."empID" = a."empID" group by str4."RouteCode"
	))), ''),
	'N', 0
    -- near visit today
	,(select count(*) from APZ_D_VISIT where "UserSign" = a."userId" and cast("FinishedDate" as date) = cast(current_date as date))
    -- nearest visit
	,(select FIRST_VALUE("StoreName" ORDER BY "FinishedDate" DESC) 
	from APZ_D_VISIT where "UserSign" = a."userId" and TO_VARCHAR("StartDate",'yyyy-MM-dd') = TO_VARCHAR(current_date,'yyyy-MM-dd'))
    -- nearest visit time
	,(select FIRST_VALUE(TO_VARCHAR("FinishedDate",'HH:mm dd/MM/yyyy') ORDER BY "FinishedDate" DESC) 
	from APZ_D_VISIT where "UserSign" = a."userId" and TO_VARCHAR("StartDate",'yyyy-MM-dd') = TO_VARCHAR(current_date,'yyyy-MM-dd'))
    -- công nợ
	, 0
    -- số lượng đơn hàng
	,ifnull(cast((select count(*) from ORDR where "UserSign" = a."userId") as int) + cast((select count(*) from OINV where "UserSign" = a."userId") as int), 0)
    -- doanh số giao ngay
    ,ifnull(cast((select sum(t0."DocTotal") from :tblOrder t0 where t0."U_TypeDH" = 'GNG' and t0."UserSign" = a."userId") as decimal(28, 6)),0)
    -- doanh số trả nợ
    ,(ifnull(cast((select sum(t0."DocTotal") from :tblOrder t0 where t0."U_TypeDH" = 'TNO' and t0."UserSign" = a."userId") as decimal(28, 6)),0) 
    + ifnull(cast((select sum(t0."DocTotal") from :tblDelivery t0 where t0."UserSign" = a."userId") as decimal(28, 6)),0))
    -- doanh số đơn hàng đặt
    ,ifnull(cast((select sum(t0."DocTotal") from :tblOrder t0 where t0."U_TypeDH" = 'DAT' and t0."UserSign" = a."userId") as decimal(28, 6)),0)
    -- tổng doanh số 
    ,ifnull((select sum("DocTotal") from ORDR where "U_Status" in ('A','D','C') and "CANCELED" = 'N' and "U_TypeDH" in ('DAT','GNG') and "CardCode" in 
    (select "CardCode" from apz_d_str2 where "RouteCode" in (select "RouteCode" from (select distinct "RouteCode" from 
	(
		select str1."RouteCode" from APZ_D_STR1 str1 inner join OHEM t0 
		on str1."SuperVisor" = t0."empID" or CompareSplit(cast(t0."empID" as nvarchar),str1."SalesAdmin",';') = 1
		where t0."empID" = a."empID" group by str1."RouteCode" 
		union all select str4."RouteCode" from APZ_D_STR4 str4 inner join OHEM t0 on str4."EmpId" = t0."empID" where t0."empID" = a."empID" group by str4."RouteCode"
	))))),0)
    -- tổng kế hoạch (planning) 
	,ifnull((select sum(t1."U_Actual") from "@SLDL" t0, "@SLDL_1" t1
	where t0."DocEntry"=t1."DocEntry" and t0."U_Type"='M' 
	and t1."U_RouteCode" in 
	(select "RouteCode" from (select distinct "RouteCode" from 
	(
		select str1."RouteCode" from APZ_D_STR1 str1 inner join OHEM t2 
        on str1."SuperVisor" = t2."empID" or CompareSplit(cast(t2."empID" as nvarchar),str1."SalesAdmin",';') = 1
        where t2."empID" = a."empID" group by str1."RouteCode" 
		union all select str4."RouteCode" from APZ_D_STR4 str4 inner join OHEM t2 on str4."EmpId" = t2."empID" where t2."empID" = a."empID" group by str4."RouteCode"
	)))
	and YEAR(t0."U_ToDate")=YEAR(Current_date)),0)
	from OHEM a 
	where (:empID = -1 or a."empID" = :empID)
	and (a."empID" in (select distinct "EmpId" from #tmpEmployees))
	and
	(((ifnull(:searchKey, '') = '' and (a."empID" is null or lower(cast(a."empID" as nvarchar)) like N'%' || lower(ifnull(:searchKey, '')) || '%')) 
	or (ifnull(:searchKey, '') <> '' and (lower(cast(a."empID" as nvarchar)) like N'%' || lower(:searchKey) || '%'))) or
	((ifnull(:searchKey, '') = '' and 
	((ifnull(a."firstName", '') || ' ' || ifnull(a."middleName", '') || ' ' || ifnull(a."lastName", ''))  is null 
	or lower(ifnull(a."firstName", '') || ' ' || ifnull(a."middleName", '') || ' ' || ifnull(a."lastName", '')) like N'%' || lower(ifnull(:searchKey, '')) || '%')) 
	or (ifnull(:searchKey, '') <> '' and 
	(lower(ifnull(a."firstName", '') || ' ' || ifnull(a."middleName", '') || ' ' || ifnull(a."lastName", '')) like N'%' || lower(ifnull(:searchKey, '')) || '%'))))
	group by a."empID", a."userId", a."lastName", a."firstName", a."middleName", a."sex", a."workStreet", a."email", a."mobile", a."birthDate";
	
	select * from #tblData order by "empID";
	
END