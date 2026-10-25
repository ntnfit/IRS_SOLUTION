
CREATE PROCEDURE  APZ_SubActivities_LoadNote(in UserName nvarchar(50), in ObjType NVARCHAR(20),in ClgCode int default 0,in StartDate timestamp default null,in EndDate timestamp default null, in ProjectCode int default -1)
as
BEGIN
	declare maxdate timestamp;
	declare userId int;
	declare empId int;
	
	select ifnull((select max("endDate") from APZ_OCLG),'1999-12-31') into maxdate from DUMMY;
	select ifnull((select USERID from OUSR where USER_CODE = :userName), -1) into userId from DUMMY;
	select ifnull((select "empID" from OHEM where "userId" = :userId), -1) into empId from DUMMY;
	
	call apz_func_OwnerOverShip(:UserName,:ObjType, OwnerView);
	
	SELECT a.*,d."Name" as "ProjectName",wbs."TaskName", ifnull(b."Authorize", case when ifnull(a."Owner", 0) = 0 or a."Owner" = -1 then 'F' else 'R' end) "OwnerAuth" FROM APZ_OCLG a
		left join APZ_OPRG d on a."ProjectEntry" = d."AbsEntry"
					left join APZ_PRG1 wbs on a."ProjectWBS" = wbs."TaskCode" and a."ProjectEntry"=wbs."AbsEntry"
					LEFT join :OwnerView b ON ifnull(a."Owner", -1) = ifnull(b."OwnerId",-2)
		WHERE "Action"='E' and ifnull("IsRemoved",'N') = 'N'
	  	and (ifnull(:ProjectCode,-1) = -1 or (ifnull(:ProjectCode,-1) != -1 and a."ProjectEntry" = :ProjectCode))
	
	  	and (a."Recontact" is null or :startDate is null or :endDate is null or a."Recontact" between :StartDate and :EndDate)
	  	--a."Recontact" between :StartDate and ifnull(:EndDate,:maxdate) and ifnull(a."endDate",a."Recontact") between :StartDate and ifnull(:EndDate,:maxdate)) and :StartDate is not null) 
	  	--			OR (:StartDate is null and a."Recontact" < ifnull(:EndDate,:maxdate)))
	
		 AND (ifnull(a."Owner", 0) = 0 or a."Owner" = -1 or ifnull(b."Authorize", 'N') <> 'N'
		 	---- trong truường hợp được assign thì cũng có thể xem note
		 	or ifnull(a."GroupEmployee", '') <> '' and compareSplit(:empId, a."GroupEmployee", ';') = 1
		 	or ifnull(a."GroupUser", '') <> '' and compareSplit(:userId, a."GroupUser", ';') = 1
		 );
  --ORDER BY ClgCode ASC thanhnv
  END




