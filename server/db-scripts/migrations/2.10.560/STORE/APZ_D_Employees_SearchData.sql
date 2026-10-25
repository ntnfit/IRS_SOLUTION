
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE type='p' and id = OBJECT_ID('APZ_D_Employees_SearchData'))
BEGIN
    DROP PROCEDURE APZ_D_Employees_SearchData
END

GO

CREATE PROCEDURE APZ_D_Employees_SearchData
	@lowLimit int=0,
	@upLimit int=0,
	@searchKey nvarchar(100)='',
	@empID int =-1,
	@userId int=-1
AS
BEGIN
	declare @tbl table(
		row_num int,
		empID int primary key,
		userId int,
		userCode nvarchar(100),
		lastName nvarchar(50),
		firstName nvarchar(50),
		middleName nvarchar(50),
		sex char(1),
		workStreet nvarchar(100),
		email nvarchar(100),
		mobile nvarchar(20),
		birthDate datetime,
		FileName nvarchar(100),
		LinkDownload nvarchar(500),
		LinkThumbnails nvarchar(500),
		Size float,
		UploadDate datetime,
		U_fullName nvarchar(100),
		route nvarchar(max),
		online char(1),
		numStoreManage int,
		numStoreVisitToday int,
		nearestVisit nvarchar(100),
		nearestVisitTime nvarchar(100),
		liabilities numeric(16,9),
		numOfOrder int
	)

declare @tmpEmployees table (empID int)
INSERT INTO  @tmpEmployees (empID)
exec APZ_D_FilterEmployees @userId
	-- 
	insert into @tbl(row_num,empID,userId,userCode,lastName,firstName,middleName,sex,workStreet,email,mobile,birthDate,FileName,LinkDownload,LinkThumbnails,Size,UploadDate,U_fullName,route,online,numStoreManage,numStoreVisitToday,nearestVisit,nearestVisitTime,liabilities,numOfOrder)
	SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY empID ) row_num,
					empID,userId,(select top 1 USER_CODE from OUSR tblOUSR where tblOUSR.USERID=A.userId) as userCode,lastName,firstName,middleName,sex,workStreet,email,mobile,birthDate,B.FileName,B.LinkDownload,B.LinkThumbnails,B.Size,B.UploadDate,
					CONCAT(ISNULL(CONVERT(nvarchar(50),firstName),''),' ',ISNULL(CONVERT(nvarchar(50),middleName),''),' ',ISNULL(CONVERT(nvarchar(50),lastName),'')) as U_fullName ,
					(select distinct Convert(nvarchar,tblRouteCode.RouteCode)+', ' from 
		(select str1.RouteCode from APZ_D_STR1 str1 where str1.SuperVisor = A.empId or convert(nvarchar(50),A.empID) in (select Data from dbo.Split(str1.SalesAdmin,';'))
		union all select str4.RouteCode from APZ_D_STR4 str4 where str4.EmpId = A.empId) tblRouteCode FOR XML PATH ('')) as route,
					'N' as online,
					0 as numStoreManage,
					(select count(*) from APZ_D_VISIT where Usersign=userId and cast(FinishedDate as date) = cast(GETDATE() as date)) as numStoreVisitToday,
					(select top 1 StoreName from APZ_D_VISIT where Usersign=userId and StartDate=GETDATE() order by CreateTime desc) as nearestVisit,
					CONVERT(VARCHAR,(select top 1 FinishedDate from APZ_D_VISIT where Usersign=userId and StartDate=GETDATE() order by CreateTime desc)) as nearestVisitTime,
					0 as liabilities,
					(ISNULL((CONVERT(int,(select count(*) from ORDR where UserSign = userId))),0)+(ISNULL((CONVERT(int,(select count(*) from OINV where UserSign = userId))),0))) as numOfOrder
					FROM OHEM A left join APZ_D_UUPIM B on (userId=B.UserSign and ObjectType='1470000171')
					WHERE (@empID = -1 or empID=@empID) 
					and (
					--(manager=@userEmpId) 
					--or 
					 empID in (select distinct empID from @tmpEmployees))
					and
					(((@searchKey='' and (empID is null or CONVERT(nvarchar(50), empID) like N'%'+@searchKey+'%')) or (@searchKey<>'' and (CONVERT(nvarchar(50), empID) like N'%'+@searchKey+'%'))) or
					((@searchKey='' and 
					(CONCAT(ISNULL(CONVERT(nvarchar(50),firstName),''),' ',ISNULL(CONVERT(nvarchar(50),middleName),''),' ',ISNULL(CONVERT(nvarchar(50),lastName),'')) is null 
					or CONCAT(ISNULL(CONVERT(nvarchar(50),firstName),''),' ',ISNULL(CONVERT(nvarchar(50),middleName),''),' ',ISNULL(CONVERT(nvarchar(50),lastName),'')) like N'%'+@searchKey+'%')) 
					or (@searchKey<>'' and 
					(CONCAT(ISNULL(CONVERT(nvarchar(50),firstName),''),' ',ISNULL(CONVERT(nvarchar(50),middleName),''),' ',ISNULL(CONVERT(nvarchar(50),lastName),'')) like N'%'+@searchKey+'%'))))) a 
                    --WHERE row_num > @lowLimit AND row_num <=@upLimit; select all employee
	select* from @tbl
END

--exec APZ_D_Employees_SearchData 0,null,N'',-1,29