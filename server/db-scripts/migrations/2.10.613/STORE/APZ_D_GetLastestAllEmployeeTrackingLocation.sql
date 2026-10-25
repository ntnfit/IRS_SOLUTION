
CALL APZ_EXISTSPROC('APZ_D_GetLastestAllEmployeeTrackingLocation');

GO

CREATE PROCEDURE APZ_D_GetLastestAllEmployeeTrackingLocation (IN userName nvarchar(100))
AS
BEGIN
	declare userId int;
	userId := IFNULL((select TOP 1 UserId from OUSR where USER_CODE = :userName), -1);
	create local temporary table #tmpEmployees ("EmpId" int);
	call APZ_D_FilterEmployees(:userId, '#tmpEmployees');
	
	tblData = 
	(SELECT t0.*
	,t2."empID"
	,(ifnull("firstName", '') || ' ' || IFNULL("middleName", '') || ' ' || IFNULL("lastName", '')) as "empName"
    , t1."LinkDownload",t1."LinkThumbnails"
    , ROW_NUMBER() OVER (PARTITION BY t0."UserSign" ORDER BY t0."TrackingDate" DESC,t0."TrackingTime" DESC) as "rn"
    FROM APZ_D_TKLC t0
    left join APZ_D_UUPIM t1 on (t0."UserSign"=t1."UserSign" and "ObjectType"='1470000171')
    left join OHEM t2 on (t0."UserSign" = t2."userId")
    where cast(t0."TrackingDate" as date) = cast(current_date as date)
    AND t0."UserSign" IN (SELECT "userId" FROM OHEM WHERE "empID" IN (SELECT DISTINCT ("EmpId") FROM #tmpEmployees)));
    
	SELECT * FROM :tblData WHERE "rn" = 1;
END