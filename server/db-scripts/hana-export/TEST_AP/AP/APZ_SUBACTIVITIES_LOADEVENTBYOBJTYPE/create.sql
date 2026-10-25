
CREATE  PROCEDURE  APZ_SubActivities_LoadEventByObjType(
	in UserName nvarchar(50),
	in ObjTypeEvent NVARCHAR(20),
	in ObjType nvarchar(254),
	in ObjId nvarchar(254))
as
BEGIN

	declare userId int;
	declare empId int;
	select ifnull((select USERID from OUSR where USER_CODE = :userName), -1) into userId from DUMMY;
	select ifnull((select "empID" from OHEM where "userId" = :userId), -1) into empId from DUMMY;
	--------------------------------- GET DATA OWNER SHIP -------------------------------------------------------
	CALL apz_func_OwnerOverShip(:UserName,:ObjTypeEvent, OwnerView);
	
	SELECT a.*,d."Name" as "ProjectName",wbs."TaskName" as "WbsName", ifnull(b."Authorize", case when ifnull(a."Owner", 0) = 0 or a."Owner" = -1 then 'F' else 'R' end) "OwnerAuth" FROM APZ_OCLG a
		left join APZ_OPRG d on a."ProjectEntry" = d."AbsEntry"
					left join APZ_PRG1 wbs on a."ProjectWBS" = wbs."TaskCode" and a."ProjectEntry"=wbs."AbsEntry"
					LEFT join :OwnerView b ON ifnull(a."Owner", -1) = ifnull(b."OwnerId",-2) 
	 WHERE "Action" = 'V' AND "IsRemoved" = 'N' AND ("SeriesNum" IS NULL OR "SeriesNum" = 0)
	 AND (
	 			:ObjType = '1470000132' AND cast("ProjectEntry" as nvarchar) = :ObjId
	 		 OR :ObjType = '112' AND "DocType" = '112' AND "DocEntry" = :ObjId 
	 		 OR :ObjType <> '1470000132' AND :ObjType <> '112' AND "DocType" = :ObjType AND "DocEntry" = :ObjId
	 		 OR :ObjType = '97' AND cast("OprId" as nvarchar) = :ObjId
	 	 )
	 AND (ifnull(a."Owner", 0) = 0 or a."Owner" = -1 or ifnull(b."Authorize", 'N') <> 'N'
	 		---- trong truường hợp được assign thì cũng có thể xem note
				 	or ifnull(a."GroupEmployee", '') <> '' and compareSplit(:empId, a."GroupEmployee", ';') = 1
				 	or ifnull(a."GroupUser", '') <> '' and compareSplit(:userId, a."GroupUser", ';') = 1
				 	or ifnull(a."UserSharing", '') <> '' and compareSplit(:empId, a."UserSharing", ';') = 1
				 	or ifnull(a."EmplSharing", '') <> '' and compareSplit(:userId, a."EmplSharing", ';') = 1
				 	OR a."AllUserInvite" = 'Y' or a."AllUserShare" = 'Y' or a."AllEmplShare" ='Y' or a."AllEmplInvite" = 'Y')
		 order by "ClgCode" DESC;
END;




