

CREATE PROCEDURE APZ_ProjectManager_GetById (IN Id integer) 
AS 
BEGIN

	SELECT * FROM APZ_OPRG WHERE "AbsEntry" = :Id;
	
	create local temporary table #tbl as (select top 0 *, cast(null as nvarchar(4000)) "Dependent", cast(null as nvarchar(50)) "StatusName",  cast(null as nvarchar(4000)) "Resource", cast(null as char(1)) "IsExist", cast(null as nvarchar(4000)) "tempvis",0 "CommentsCount" from APZ_PRG1);
	
	insert into #tbl
	select a.*, (select string_agg(cast(b."Dependent" as nvarchar), ',') from APZ_PRG2 b where a."AbsEntry" = b."AbsEntry" and a."LineNum" = b."LineNum") as "Dependent"
	,
	CASE
	WHEN a."Status"='P' THEN N'Chờ xử lý'
	WHEN a."Status"='S' THEN N'Đang thực hiện'
	WHEN a."Status"='F' THEN N'Hoàn thành'
	WHEN a."Status"='C' THEN N'Hủy bỏ'
	WHEN a."Status"='L' THEN N'Kết thúc'
	Else N'Kế hoạch'
	END AS "StatusName",
	(select string_agg(cast(c."Resource" as nvarchar), ',') from APZ_PRG3 c where a."AbsEntry" = c."AbsEntry" and a."LineNum" = c."LineNum") as "Resource"
	,'Y' as "IsExist", ROW_NUMBER() OVER (ORDER BY a."Wbs") "tempvis",(select count(1) from APZ_COMMENTS c where a."AbsEntry"=c.OBJECT_CODE and c.DETAIL_CODE=a."Wbs" and c.OBJTYPE='1470000132WBS') "CommentsCount"
	from APZ_PRG1 a
	where a."AbsEntry" = :Id;
	
	UPDATE #tbl SET "VisOrder" = "tempvis" WHERE "VisOrder" IS NULL;
	SELECT * FROM #tbl ORDER BY "Wbs";

	--Dêpnt
	select * from APZ_PRG2 where "AbsEntry"=:Id;
	--Team
	SELECT a.*, IFNULL(b."firstName", '') || ' ' || IFNULL(b."middleName", '') || ' ' || IFNULL(b."lastName", '') AS "EmployeeName", 
	b."branch" AS "Branch", b."dept" AS "Department", b."firstName" AS "EFNameAvata", b."lastName" AS "ELNameAvata",d."Name" AS "jobTitle", 
	(SELECT "LinkThumbnails" FROM (SELECT "LinkThumbnails", ROW_NUMBER() OVER() AS row_id FROM APZ_D_UUPIM WHERE "ObjectType" = '1470000171' AND b."userId" = "UserSign") WHERE row_id = 1) AS "LinkThumbnails", c."name" AS "RoleName" 
	FROM APZ_PRG3 a 
	LEFT OUTER JOIN OHEM b ON a."Resource" = b."empID" 
	LEFT OUTER JOIN OHTY c ON a."Role" = c."typeID" 
	LEFT OUTER JOIN OUDP d ON a."Resource" = b."empID" and d."Code"=b."dept"
	WHERE "AbsEntry" = :Id;
	select *,(select count(1) from APZ_COMMENTS c where a."AbsEntry"=c.OBJECT_CODE and ifnull(c.DETAIL_CODE,'')=a."LineNum" and c.OBJTYPE='1470000132BUDGET') "CommentsCount" from APZ_PRG4 a where a."AbsEntry"=:Id ORDER BY "VisOrder";

	--Document
	select * from APZ_PRG5 where "AbsEntry"=:Id; 
	
	--Sub
	SELECT * FROM APZ_OPRG WHERE "MasterEntry" = :Id;
	--Payment
	SELECT * FROM APZ_PRG6 WHERE "AbsEntry" = :Id ORDER BY "VisOrder";
	--Issue
	SELECT a.*, f."LinkThumbnails",
	c."Color" AS "ColorStatus", c."Name" AS "StatusName", IFNULL(d."firstName", '') || ' ' || IFNULL(d."middleName", '') || ' ' || IFNULL(d."lastName", '') AS "ReporterName", 
	IFNULL(e."firstName", '') || ' ' || IFNULL(e."middleName", '') || ' ' || IFNULL(e."lastName", '') AS "HandlerName", d."firstName" AS "RepFName", d."lastName" AS "RepLName" 
	FROM APZ_TGIE a 
	LEFT OUTER JOIN APZ_OTIS c ON a."Status" = cast(c."Code" as nvarchar) 
	LEFT OUTER JOIN OHEM d ON a."Reporter" = d."empID" 
	LEFT OUTER JOIN OHEM e ON a."HandleBy" = e."empID" 
	LEFT OUTER JOIN APZ_D_UUPIM f on d."userId" = f."UserSign" and f."ObjectType" = '1470000171'
	WHERE "ProjectCode" = :Id;
	

	create local temporary table #tblUSER as (select top 0 a.* , cast(null as nvarchar(100))  as "startDateTime" ,cast(null as nvarchar(100)) as "endDateTime"
		,cast(null as nvarchar(254)) as "LocationCode" ,cast(null as nvarchar(100)) as "ProjectCode", cast(null as nvarchar(100)) as "CardName", cast(null as nvarchar(254)) as "EmployeeName", cast(null as nvarchar(254)) as U_NAME, cast(null as nvarchar(500)) "LinkThumbnails", cast(null as nvarchar(1000)) "FullName" from APZ_oclg a);
	
	insert into #tblUSER
		select a.* , cast(cast(a."Recontact" as date) as NVARCHAR) || ' ' || LEFT(RIGHT('0000' || cast(ifnull(a."BeginTime",0) as varchar(4)), 4), 2) || cast(STUFF(RIGHT('0000' || cast(ifnull(a."BeginTime",0) as varchar(4)), 4) , 3, 0, ':') as NVARCHAR)  as "startDateTime"
			,cast(cast(ifnull(a."endDate",a."Recontact") as Date) as NVARCHAR) || ' ' || LEFT(RIGHT('0000' || cast(ifnull(a."ENDTime",ifnull(a."BeginTime",0) +100) as varchar(4)), 4),2) || cast(STUFF(RIGHT('0000' || cast(ifnull(a."ENDTime",ifnull(a."BeginTime",0) +100) as varchar(4)), 4) , 3, 0, ':') as NVARCHAR) as "endDateTime"
			,c."Name" as "LocationCode" ,d."Code" as "ProjectCode", OCRD."CardName",ifnull(OHEM."firstName", '')  || ' ' || ifnull(OHEM."middleName", '')  || ' ' || ifnull(OHEM."lastName", '') as "EmployeeName", OUSR.U_NAME,
			(select "LinkThumbnails" from APZ_D_UUPIM where "UserSign" = (select USERID from (select USERID, row_number() over() as row_Id 
																		  from (
																			    select USERID from OUSR where CompareSplit(cast(USERID as nvarchar), a."GroupUser", ';') = 1 
																				union all 
																				select "userId" from OHEM where CompareSplit(cast("empID" as nvarchar), a."GroupEmployee", ';') = 1)
																				 ) where row_Id = 1
																			) 
														and "ObjectType"='1470000171') as "LinkThumbnails",
			(select string_agg("FullName", ';') from 
				(
					select case when x."firstName" is not null or x."middleName" is not null or x."lastName" is not null then (ifnull(x."firstName", '') || ' ' || ifnull(x."middleName", '') || ' ' || ifnull(x."lastName", '')) else y.U_NAME end as "FullName" from OUSR y left join OHEM x on x."userId" = y.USERID where CompareSplit(y.USERID, a."GroupUser", ';') = 1
					union all
					select (ifnull(x."firstName", '') || ' ' || ifnull(x."middleName", '') || ' ' || ifnull(x."lastName", '')) as "FullName" from OHEM x inner join OUSR y on x."userId" = y.USERID and CompareSplit(x."empID", a."GroupEmployee", ';') = 1
				)
			) as "FullName"
			
		from APZ_oclg a
			left join OCLO c on a."Location" = c."Code"
			left join APZ_OPRG d on a."ProjectEntry" = d."AbsEntry"
			left join OCRD on a."CardCode" = OCRD."CardCode"
			left join OHEM on a."UserSign"=OHEM."userId"
			left join OUSR  on a."UserSign"=OUSR.USERID
		WHERE "Action" = 'T' AND "parentTask" IS NULL AND "ProjectEntry" = :Id;
				
	SELECT * FROM #tblUSER;

end;




