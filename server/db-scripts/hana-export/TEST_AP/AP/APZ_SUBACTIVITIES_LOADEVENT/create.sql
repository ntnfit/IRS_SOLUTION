
CREATE  PROCEDURE  APZ_SubActivities_LoadEvent(
	in UserName nvarchar(50),
	in ObjType NVARCHAR(20),
	in clgCode int default 0,
	in StartDate timestamp  default null,
	in EndDate datetime default null,
	in ProjectCode int default -1,
	in LimitData int default 0,
	in Skip int default 0)
as
BEGIN
	declare maxdate timestamp;
	declare userId int;
	declare empId int;
	select ifnull((select USERID from OUSR where USER_CODE = :userName), -1) into userId from DUMMY;
	select ifnull((select "empID" from OHEM where "userId" = :userId), -1) into empId from DUMMY;
	
	call apz_func_OwnerOverShip(:UserName,:ObjType, OwnerView);
	
	select ifnull((select max("endDate") from APZ_OCLG),'9999-12-31') into maxdate from DUMMY;
	
	create local temporary table #tblEvent as (select top 0 *,cast(null as nvarchar(1)) "OwnerAuth" from APZ_OCLG);
	
	insert into #tblEvent
		SELECT a.*, ifnull(b."Authorize", case when ifnull(a."Owner", 0) = 0 or a."Owner" = -1 then 'F' else 'R' end) "OwnerAuth" FROM APZ_OCLG a LEFT join :OwnerView b ON ifnull(a."Owner", -1) = ifnull(b."OwnerId",-2)
			WHERE "Action"='V' and ifnull("IsRemoved", 'N') = 'N' and ("SeriesNum" is null or "SeriesNum" = 0) AND (ifnull(:clgCode,0) = 0 or a."ClgCode" = :clgCode)
			 and (ifnull(:ProjectCode,-1) = -1 or a."ProjectEntry" = :ProjectCode)
			
			 and (((a."Recontact" between :StartDate and ifnull(:EndDate,:maxdate) and ifnull(a."endDate",a."Recontact") between :StartDate and ifnull(:EndDate,:maxdate)) and :StartDate is not null) OR (:StartDate is null  and a."Recontact" < ifnull(:EndDate,:maxdate)))
			
			 AND (ifnull(a."Owner", 0) = 0 or a."Owner" = -1 or ifnull(b."Authorize", 'N') <> 'N'
			 	---- trong truường hợp được assign thì cũng có thể xem note
				 	or ifnull(a."GroupEmployee", '') <> '' and compareSplit(:empId, a."GroupEmployee", ';') = 1
				 	or ifnull(a."GroupUser", '') <> '' and compareSplit(:userId, a."GroupUser", ';') = 1
				 	or ifnull(a."UserSharing", '') <> '' and compareSplit(:empId, a."UserSharing", ';') = 1
				 	or ifnull(a."EmplSharing", '') <> '' and compareSplit(:userId, a."EmplSharing", ';') = 1
				 	OR a."AllUserInvite" = 'Y' or a."AllUserShare" = 'Y' or a."AllEmplShare" ='Y' or a."AllEmplInvite" = 'Y');
	   
	if ifnull(:LimitData,0) >0
	THEN
		SELECT * FROM (select * , ROW_NUMBER() OVER(order by "ClgCode" DESC) rn from #tblEvent where "Action" = 'V') t WHERE rn> :Skip AND rn<= :Skip + :LimitData;
	
	ELSE 
		SELECT * from #tblEvent Order by "ClgCode" DESC;
	END IF;
END




