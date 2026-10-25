
CALL APZ_EXISTSPROC('APZ_D_SubActivities_GetByCode');

GO


CREATE  PROCEDURE  APZ_D_SubActivities_GetByCode(
	in UserName nvarchar(50)
	, in ObjTypeTask NVARCHAR(20)
	, in ObjTypeEvent NVARCHAR(20)
	, in ObjTypeNote NVARCHAR(20)
	, in ActionType char(1) default ''
	, in ClgCode int default 0
	, in SeriesNum int default 0
 	, in StartDate timestamp default null
 	, in EndDate timestamp default null
 	, in tempTable nvarchar(10) default '')
as
BEGIN
	declare maxdate timestamp;
	declare authMode NVARCHAR(10) = ifnull(ifnull((select "AuthorizationMode" from APZ_D_OAPC),''), 'R');
    DECLARE userId INT = ifnull((SELECT TOP 1 "USERID" FROM OUSR WHERE "USER_CODE" = :UserName), -1);
    declare empId int = ifnull((SELECT TOP 1 "empID" FROM OHEM WHERE "userId" = :userId), -1);
	if :ActionType is null
	THEN
	
		ActionType = '';
	
	END IF;
	
	if :ClgCode is null
	THEN
	
		ClgCode = 0;
	
	END IF;
	
	if :SeriesNum is null
	THEN
	
		SeriesNum = 0;
	
	END IF;
	
	select ifnull((select max("endDate") from APZ_OCLG),current_date) into maxdate from DUMMY;

	tblUSER = select 
	case when :ClgCode <> 0 then a."Notes" else '' end as "Notes"
	,a."Action"
    ,a."AddressComplete"
    ,a."AddressExecution"
    ,a."AllDay"
    ,a."AllDayEvent"
    ,a."AllEmplInvite"
    ,a."AllEmplShare"
    ,a."AllUserInvite"
    ,a."AllUserShare"
    ,a."AssignedBy"
    ,a."AtcEntry"
    ,a."Attachment"
    ,a."AttendEmpl"
    ,a."AttendUser"
    ,a."BaseCode"
    ,a."BaseEntry"
    ,a."BaseLine"
    ,a."BaseType"
    ,a."BeginTime"
    ,a."CardCode"
    ,a."ClgCode",a."CloseDate",a."Closed",a."CntctCode",a."CntctDate",a."CntctSbjct",a."CntctTime",a."CntctType",a."CreateTime",a."DataSource",a."DayInMonth",a."DayOfWeek",a."Details",a."DocEntry",a."DocNum",a."DocType",a."DurType",a."Duration",a."ENDTime",a."EmplSharing",a."EndType",a."Friday",a."GroupEmployee",a."GroupUser"
    ,a."HtmlNotes",a."Interval",a."IsRemoved",a."LastRemind",a."LatitudeComplete",a."LatitudeExecution",a."Location"
    ,a."LogInstanc",a."LongitudeComplete",a."LongitudeExecution",a."MaxOccur",a."Monday",a."Month",a."NextDate",a."NextTime"
    ,a."OcrCode",a."OcrCode2",a."OcrCode3",a."OcrCode4",a."OcrCode5",a."OprId",a."OprLine",a."OrderLine",a."OrigDate",a."Owner",a."Predecessors"
    ,a."Priority",a."PrjCode",a."ProjectEntry",a."ProjectWBS",a."Recontact",a."RecurPat",a."RecurrenceInfo",a."RemDate",a."RemQty",a."RemSented",a."RemTime",a."RemType"
    ,a."Reminder",a."ReminderInfo",a."ResourceIdsInfo",a."Saturday",a."SeEndDat",a."SeStartDat",a."SeriesNum",a."SubOption",a."Sunday",a."TaskMode",a."Thursday",a."Transfered"
    ,a."Tuesday",a."UpdateDate",a."UpdateTime",a."UserSharing",a."UserSign",a."Wednesday",a."Week",a."endDate",a."inactive",a."parentId",a."parentTask"
    ,a."parentType",a."personal",a."prevActvty",a."status",a."tentative" , 
        cast(cast(a."Recontact" as date) as NVARCHAR) || ' ' || cast(STUFF(RIGHT('0000' || cast(ifnull(a."BeginTime",0) as varchar(4)), 4) , 3, 0, ':') as NVARCHAR)  as "startDateTime"
		,cast(cast(ifnull(a."endDate",a."Recontact") as Date) as NVARCHAR) || ' ' || cast(STUFF(RIGHT('0000' || cast(ifnull(a."ENDTime",ifnull(a."BeginTime",0) +100) as varchar(4)), 4) , 3, 0, ':') as NVARCHAR) as "endDateTime"
		,c."Name" as "LocationCode" 
        ,d."Code" as "ProjectCode"
        ,OCRD."CardName"
        ,ifnull(OHEM."firstName", '')  || ' ' || ifnull(OHEM."middleName", '')  || ' ' || ifnull(OHEM."lastName", '') as "EmployeeName"
        ,OUSR.U_NAME
        ,(select "LinkThumbnails" from APZ_D_UUPIM where "UserSign" = (select USERID from (select USERID, row_number() over() as row_Id 
            from (
                select USERID from OUSR where CompareSplit(cast(USERID as nvarchar), a."GroupUser", ';') = 1 
                union all 
                select "userId" from OHEM where CompareSplit(cast("empID" as nvarchar), a."GroupEmployee", ';') = 1)
                    ) where row_Id = 1
            ) 
		    and "ObjectType"='1470000171') as "LinkThumbnails"
        ,d."Name" as "ProjectName"
        ,wbs."TaskName" as "WbsName"
        from APZ_oclg a
            left join OCLO c on a."Location" = c."Code"
            left join APZ_OPRG d on a."ProjectEntry" = d."AbsEntry"
            left join APZ_PRG1 wbs on a."ProjectWBS" = wbs."TaskCode" and a."ProjectEntry"=wbs."AbsEntry"
            left join OCRD on a."CardCode" = OCRD."CardCode"
            left join OHEM on a."UserSign"=OHEM."userId"
            left join OUSR  on a."UserSign"=OUSR.USERID
        where (:ClgCode <= 0 or (:ClgCode > 0 and (a."ClgCode" = :ClgCode or a."SeriesNum" = :ClgCode))) 
			and (:ActionType = '' or (:ActionType != '' and "Action" = :ActionType)) 
			and (:SeriesNum <= 0 or (:SeriesNum > 0 and a."SeriesNum" = :SeriesNum)) 
			--AND (a.GroupEmployee is not null or a.GroupUser is not null) Tạm thời bỏ đoạn này, vì có thể select Sharing thay vì assign trong trường hợp ko assign cho ai mà chỉ share
		
			--Lẩy ra những record trong 1 khoảng thời gian nhất định
			and (((
			a."Recontact" between :StartDate and ifnull(:EndDate,:maxdate) or ifnull(a."endDate",a."Recontact") between :StartDate and ifnull(:EndDate,:maxdate)  
			--Trường hợp lấy ra các task có thời điểm bắt đầu hoặc kết thúc nằm trong khoảng tìm kiếm
			or :StartDate between a."Recontact" and ifnull(a."endDate",a."Recontact") or ifnull(:EndDate,:maxdate)  between a."Recontact" and ifnull(a."endDate",a."Recontact")
			-- *Xử lí lấy record trong các trường hợp recurring Event* 
				--	TH1: Lấy ra các Recurring event (mà tính được ngày kết thúc) 
			or (a."RecurPat" !='N' AND a."EndType" != 'N' and a."SeEndDat" is not null 
					and (
						--Ngày kết thúc của khoảng tìm kiếm nằm giữa ngày bắt đầu và kết thúc của chuỗi Recurring
					(ifnull(:EndDate,:maxdate) between a."SeStartDat" and a."SeEndDat") 
								--Đối với các sự kiến REcurring gốc, thì sẽ kiểm tra ngày kết thúc của chuỗi có nằm trong khoảng tìm kiếm hay không
							or ((a."SeriesNum"=0 or a."SeriesNum" is null) and a."SeEndDat" between :StartDate and ifnull(:EndDate,:maxdate)))) 
			    --  TH2: Lấy ra ChangedOccurence hay DeleteOccurence mà có OrigDate trong khoảng tìm kiếm
			or ((a."CntctType" =3 or a."CntctType"=4) and a."OrigDate" between :StartDate and ifnull(:EndDate,:maxdate)) 
			    --  TH3: Lấy ra các Recurring event(mà không có ngày kết thúc) có ngày bắt đầu trước hoặc nằm trong khoảng tìm kiếm
			or (a."EndType" = 'N' AND a."RecurPat" != 'N' AND (a."SeStartDat" < :StartDate or (a."SeStartDat" between :StartDate and ifnull(:EndDate,:maxdate))))
			-- **
			) and :StartDate is not null) 
			OR (:StartDate is null));

    --check authorization mode
    if :authMode = 'R'
    then
        --get data employee by route
		create local temporary table #tmpEmployees ("EmpId" int);
		call APZ_D_FilterEmployees(:userId, '#tmpEmployees');
        tmpUsers = select "userId" as "UserId" from OHEM where "empID" in (select "EmpId" from #tmpEmployees); 
        result = 
        select * from :tblUSER a where ((
        (
        "Action" = 'T'
        AND
        Exists (select 1 from #tmpEmployees where CompareSplit("EmpId", a."GroupEmployee",';') = 1) 
        OR ifnull(a."GroupEmployee",'')='' 
        or CompareSplit(:empId, a."GroupEmployee",';') = 1)
        AND 
        (Exists (select 1 from :tmpUsers where CompareSplit("UserId", a."GroupUser",';') = 1) 
        OR ifnull(a."GroupUser",'')=''
        or CompareSplit(:userId, a."GroupUser",';') = 1)
        )
        OR
        (
        "Action" = 'V'
        AND(
        (Exists (select 1 from #tmpEmployees where CompareSplit("EmpId", a."GroupEmployee",';') = 1) 
        OR ifnull(a."GroupEmployee",'')=''
        or CompareSplit(:empId, a."GroupEmployee",';') = 1)
        AND 
        (Exists (select 1 from :tmpUsers where CompareSplit("UserId", a."GroupUser",';') = 1) 
        OR ifnull(a."GroupUser",'')=''
        or CompareSplit(:userId, a."GroupUser",';') = 1)
        )
        --Kiểm tra xem có Sharing hay không
        OR (
        (ifnull(a."UserSharing",'')!='' OR ifnull(a."EmplSharing",'')!='') AND
        (Exists (select 1 from #tmpEmployees where CompareSplit("EmpId", a."EmplSharing",';') = 1) 
        OR ifnull(a."EmplSharing",'')=''
        or CompareSplit(:empId, a."EmplSharing",';') = 1)
        AND
        (Exists (select 1 from :tmpUsers where CompareSplit("UserId", a."UserSharing",';') = 1) 
        OR ifnull(a."UserSharing",'')=''
        or CompareSplit(:userId, a."UserSharing",';') = 1)
        )
        )
        OR
        (
        "Action" = 'E'
        AND
        (Exists (select 1 from #tmpEmployees where CompareSplit("EmpId", a."GroupEmployee",';') = 1) 
        OR ifnull(a."GroupEmployee",'')=''
        or CompareSplit(:empId, a."GroupEmployee",';') = 1)
        AND 
        (Exists (select 1 from :tmpUsers where CompareSplit("UserId", a."GroupUser",';') = 1) 
        OR ifnull(a."GroupUser",'')=''
        or CompareSplit(:userId, a."GroupUser",';') = 1)
        
        ))
        AND ((ifnull(a."GroupUser",'') != '' or ifnull(a."GroupEmployee",'') != '') or(((ifnull(a."GroupUser",'') = '' and ifnull(a."GroupEmployee",'') = '')) and (a."AllUserInvite" = 'Y' or a."AllUserShare" = 'Y' or a."AllEmplShare" ='Y' or a."AllEmplInvite" = 'Y')) );
            --Order by ClgCode ASC  
    else 
        call apz_func_OwnerOverShip(:UserName,:ObjTypeTask, ownerShipTask);
        call apz_func_OwnerOverShip(:UserName,:ObjTypeEvent, ownerShipEvent);
        call apz_func_OwnerOverShip(:UserName,:ObjTypeNote, ownerShipNote);
        
        result = 
        select * from :tblUSER a where ((
        (
        "Action" = 'T'
        AND
        Exists (select 1 from :ownerShipTask where CompareSplit("OwnerId", a."GroupEmployee",';') = 1) OR ifnull(a."GroupEmployee",'')='')
        AND 
        (Exists (select 1 from :ownerShipTask where CompareSplit("UserId", a."GroupUser",';') = 1) OR ifnull(a."GroupUser",'')='')
        
        )
        OR
        (
        "Action" = 'V'
        AND(
        (Exists (select 1 from :ownerShipEvent where CompareSplit("OwnerId", a."GroupEmployee",';') = 1) OR ifnull(a."GroupEmployee",'')='')
        AND 
        (Exists (select 1 from :ownerShipEvent where CompareSplit("UserId", a."GroupUser",';') = 1) OR ifnull(a."GroupUser",'')='')
        )
        --Kiểm tra xem có Sharing hay không
        OR (
        (ifnull(a."UserSharing",'')!='' OR ifnull(a."EmplSharing",'')!='') AND
        (Exists (select 1 from :ownerShipEvent where CompareSplit("OwnerId", a."EmplSharing",';') = 1) OR ifnull(a."EmplSharing",'')='')
        AND
        (Exists (select 1 from :ownerShipEvent where CompareSplit("UserId", a."UserSharing",';') = 1) OR ifnull(a."UserSharing",'')='')
        )
        )
        OR
        (
        "Action" = 'E'
        AND
        (Exists (select 1 from :ownerShipNote where CompareSplit("OwnerId", a."GroupEmployee",';') = 1) OR ifnull(a."GroupEmployee",'')='')
        AND 
        (Exists (select 1 from :ownerShipNote where CompareSplit("UserId", a."GroupUser",';') = 1) OR ifnull(a."GroupUser",'')='')
        
        ))
        AND ((ifnull(a."GroupUser",'') != '' or ifnull(a."GroupEmployee",'') != '') or(((ifnull(a."GroupUser",'') = '' and ifnull(a."GroupEmployee",'') = '')) and (a."AllUserInvite" = 'Y' or a."AllUserShare" = 'Y' or a."AllEmplShare" ='Y' or a."AllEmplInvite" = 'Y')) );
            --Order by ClgCode ASC                                                              
    end if;                 

    
	
	
	select * from :result;
	
	if ifnull(:tempTable, '') <> '' then
		declare query nvarchar(100) = N'insert into ' || :tempTable || ' select * from :result;';
		exec(:query);
	end if;
END