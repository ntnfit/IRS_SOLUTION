

CREATE PROCEDURE APZ_D_ProjectManager_GetDataTask(IN projectEntry int DEFAULT -1)
AS
BEGIN
	create local temporary table #tblUSER as (select top 0 a.* , cast(null as nvarchar(100))  as "startDateTime" ,cast(null as nvarchar(100)) as "endDateTime"
		,cast(null as nvarchar(254)) as "LocationCode" ,cast(null as nvarchar(100)) as "ProjectCode", cast(null as nvarchar(100)) as "CardName", 
		cast(null as nvarchar(254)) as "EmployeeName", cast(null as nvarchar(500)) "LinkThumbnails", cast(null as nvarchar(1000)) "FullName" from APZ_oclg a);
		
	insert into #tblUSER
		select a.* , cast(cast(a."Recontact" as date) as NVARCHAR) || ' ' || LEFT(RIGHT('0000' || cast(ifnull(a."BeginTime",0) as varchar(4)), 4), 2) || cast(STUFF(RIGHT('0000' || cast(ifnull(a."BeginTime",0) as varchar(4)), 4) , 3, 0, ':') as NVARCHAR)  as "startDateTime"
			,cast(cast(ifnull(a."endDate",a."Recontact") as Date) as NVARCHAR) || ' ' || LEFT(RIGHT('0000' || cast(ifnull(a."ENDTime",ifnull(a."BeginTime",0) +100) as varchar(4)), 4),2) || cast(STUFF(RIGHT('0000' || cast(ifnull(a."ENDTime",ifnull(a."BeginTime",0) +100) as varchar(4)), 4) , 3, 0, ':') as NVARCHAR) as "endDateTime"
			,c."Name" as "LocationCode" ,d."Code" as "ProjectCode", OCRD."CardName",ifnull(OHEM."firstName", '')  || ' ' || ifnull(OHEM."middleName", '')  || ' ' || ifnull(OHEM."lastName", '') as "EmployeeName", 
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
		WHERE "Action" = 'T' AND "parentTask" IS NULL AND "ProjectEntry" = :projectEntry;
				
	SELECT * FROM #tblUSER;
	
	drop table #tblUSER;
END



