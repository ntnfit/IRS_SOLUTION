
CREATE  PROCEDURE  APZ_SubActivities_LoadNoteByObjType(
in UserName nvarchar(50),
in ObjTypeNote NVARCHAR(20),
in ObjType nvarchar(254),
in ObjId nvarchar(254))
as
BEGIN

	declare userId int;
	declare empId int;
	select ifnull((select USERID from OUSR where USER_CODE = :userName), -1) into userId from DUMMY;
	select ifnull((select "empID" from OHEM where "userId" = :userId), -1) into empId from DUMMY;

 	------------------------------ GET DATA OWNER SHIP ----------------------------------------
	call apz_func_OwnerOverShip(:UserName,:ObjTypeNote, OwnerView);
	
	SELECT a.*, ifnull(b."Authorize", case when ifnull(a."Owner", 0) = 0 or a."Owner" = -1 then 'F' else 'R' end) "OwnerAuth" FROM APZ_OCLG a LEFT join :OwnerView b ON ifnull(a."Owner", -1) = ifnull(b."OwnerId",-2)
	
	WHERE "Action"='E'
		 AND (
			 		:ObjType='1470000132' AND "ProjectEntry"=:ObjId
			 	 OR :ObjType='112' AND "DocType"='112' AND "DocEntry"=:ObjId
				 OR :ObjType!='1470000132' AND :ObjType!='112' AND "DocType"=:ObjType AND "DocEntry"=:ObjId
				 OR :ObjType='97' AND "OprId"=:ObjId
			 ) 
		 AND (ifnull(a."Owner", 0) = 0 or a."Owner" = -1 or ifnull(b."Authorize", 'N') <> 'N'
		 		---- trong truường hợp được assign thì cũng có thể xem note
		 	or ifnull(a."GroupEmployee", '') <> '' and compareSplit(:empId, a."GroupEmployee", ';') = 1
		 	or ifnull(a."GroupUser", '') <> '' and compareSplit(:userId, a."GroupUser", ';') = 1)
	 order by "ClgCode" DESC;
END;




