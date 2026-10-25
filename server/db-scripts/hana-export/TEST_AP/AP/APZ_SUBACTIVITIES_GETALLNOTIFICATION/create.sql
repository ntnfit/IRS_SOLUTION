
CREATE PROCEDURE APZ_SubActivities_GetAllNotification(
	in userId int default -1
)

AS
BEGIN

	declare userCode nvarchar(50);
	declare i int;
	declare sqlcmd nvarchar(5000);
	declare timeIndex int;
	declare employee int;
	declare specify date;
	declare checkTypeSpecify nchar(1);
	create local temporary table #temp 
            (
             "Priority"		char(1)				NULL
            ,"Attachment"		char(1)				NULL
            ,"Code"			int					NULL
			,"Name"			VARCHAR(254)		NULL			
            ,"UserSign"		INT					NULL 
            ,"FromUser"		nvarchar(50)		NULL
            ,"QString"		nvarchar(5000)		NULL
            ,"Opened"			char(1)				NULL 
            ,"WasRead"		char(1)				NULL
            ,"Deleted"		char(1)				NULL
            ,"Failed"			char(1)				NULL
            ,"Received"		timestamp			NULL
			);
			
	select (select USER_CODE from OUSR where USERID = :userId) into userCode from DUMMY;
	
	insert into #temp select * from apz_fun_alert_getbyuser(:userCode,0);
	
	create local temporary table #tempDate as (select top 0 "Code", "Name", "UserSign", "FromUser", "QString", 0 "Id", 0 "ResultValue" from #temp);

	insert into #tempDate
	select "Code", "Name", "UserSign", "FromUser", "QString", ROW_NUMBER() OVER(ORDER BY "Code" ASC), 0 from #temp;

	select ifnull((select max("Id") from #tempDate),0) into i from DUMMY;
	
	while i > 0
	DO

		select (select string_agg('update #tempDate set "ResultValue" = (select count(1) from (' || a."QString" || ') a)' || ' where "Id" = ' || CAST(:i as nvarchar(20))) from #tempDate a  where "Id" = :i) into sqlcmd from DUMMY;
		--print(@sqlcmd)
		BEGIN
			declare EXIT HANDLER FOR SQLEXCEPTION
			BEGIN
				exec :sqlcmd;
			END;
		END;
		
		i = :i - 1;

	end while;

	--select * from #tempDate

	timeIndex = cast(to_varchar(current_date, 'hh') as int) * 100 + cast(to_varchar(current_date, 'mm') as int);
	
	select ifnull((select "empID" from OHEM where "userId" = :userId),0) into employee from DUMMY;
	
	create local temporary table #tempSetting ("CreateUserCode" nvarchar(50), "AssignUser" nvarchar(4000), "DueDate" timestamp, "DueTime" int, "Details" nvarchar(4000), "NotifiedId" int, "Location" int
		,"ObjectTypes" nvarchar(20),"CreateUserName" nvarchar(254), "SumResult" int, "TranType" nchar(1), "CmtCode" int, "CntctType" nvarchar(50),"RemType" nvarchar(50),"RemQty" numeric(19,6),"RecurrenceInfo" nclob);
	
	insert into #tempSetting
	select b.USER_CODE "CreateUserCode", "GroupUser" "AssignUser", ifnull(case when "Action" = 'T' then "endDate" else "Recontact" end, '9999-12-30') "DueDate", case when "Action" = 'T' then "ENDTime" else "BeginTime" end "DueTime", 
		   "Details", "ClgCode" "NotifiedId", "Location", case when "Action" = 'T' then '1470000128' else '1470000129' end "ObjectTypes", b."U_NAME" "CreateUserName", 0 "SumResult", 'A' "TranType",0 "CmtCode",
		   "CntctType","RemType","RemQty","RecurrenceInfo"
	 from APZ_OCLG a left join OUSR b on a."AssignedBy" = b."USERID" 
		WHERE IFNULL(a."parentTask",0)=0 and "Action" in ('T', 'V') and "status" = 1 and ("Action" = 'T' or a."Recontact" > current_date or (a."Recontact" = current_date and a."BeginTime" >= :timeIndex)) and
			((ifnull("GroupUser",'') <> '' and comparesplit(:userId, "GroupUser", ';') = 1) or (ifnull("GroupEmployee",'') <> '' and comparesplit(:employee, "GroupEmployee", ';') = 1))
	UNION ALL
	SELECT cast(b.USER_CODE as nvarchar) "CreateUserCode", cast(:userId as nvarchar) "AssignUser", ifnull(case when a."ResponseDate" is null then "IssueDate" else "ResponseDate" end, '9999-12-30') "DueDate", 0 "DueTime", 
		   a."Note" "Details", a."Id" "NotifiedId", null "Location", '1470000130' "ObjectTypes", b."U_NAME" "CreateUserName", 0, 'A',0 "CmtCode",null,null,null,null
		from APZ_TGIE a
			LEFT JOIN OHEM c ON a."Reporter" = c."empID"
			LEFT JOIN OUSR b ON c."userId" = b."USERID" where cast(a."Status" as int) <= 4 and :employee = a."HandleBy"
	UNION ALL 
	select null, null, null, 0, "Name", "Code", null, '1200004', null, "ResultValue", 'A',0 "CmtCode",null,null,null,null from #tempDate where "ResultValue" > 0 
	UNION ALL
	SELECT c."USER_CODE", cast(:userId as nvarchar), a."CreatedTime", cast(TO_VARCHAR(a."CreatedTime", 'HH') as int) * 100 + cast(TO_VARCHAR(a."CreatedTime", 'mm') as int), a."Content", a."ID", null
		, a."ObjType", c."U_NAME", 0, 'M',a."CmtCode" "CmtCode",null,null,null,null
			 from APZ_CLG1 a inner join (select "CmtCode" from APZ_CLG3 where "UserSign" = :userId and "Unread" = 'Y') b ON a."CmtCode" = b."CmtCode"
							 INNER JOIN OUSR c On a."UserSign" = c."USERID"
	order by "DueDate" desc;
	
	select ifnull((SELECT "TypeSpecify" from APZ_NOSE where "UserCode"=:userCode),'') INTO checkTypeSpecify from DUMMY;
	if :checkTypeSpecify !='A'
	then
		if :checkTypeSpecify='D'
		then
			select (select add_days("TimeSetting",-"SpecifyNotification") from apz_nose where "UserCode"=:userCode) into specify from DUMMY;
		
		elseif :checkTypeSpecify='M'
		then
			select (select add_months("TimeSetting",-"SpecifyNotification") from apz_nose where "UserCode"=:userCode) into specify from DUMMY;
		
		elseif :checkTypeSpecify='W'
		then
			select (select add_days("TimeSetting",-"SpecifyNotification" * 7) from apz_nose where "UserCode"=:userCode) into specify from DUMMY;
		
		elseif :checkTypeSpecify='Y'
		then
			select (select add_years("TimeSetting",-"SpecifyNotification") from apz_nose where "UserCode"=:userCode) into specify from DUMMY;
		
		else
			specify = '1753-01-01';
		end if;
	
	else
			specify ='1753-01-01';
		end if;
	select * from #tempSetting where "DueDate" >= :specify;
	--print @specify
END

--delete from apz_nose where usercode='Namnx'
--select * from apz_nose
--select GETDATE()




