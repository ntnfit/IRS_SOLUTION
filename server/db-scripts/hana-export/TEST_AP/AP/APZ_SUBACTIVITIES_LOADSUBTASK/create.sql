
CREATE PROCEDURE APZ_SubActivities_LoadSubTask(in ClgCode int, in UserName nvarchar(40000), in LoadForNode char(1) default 'N')
AS
BEGIN
		
	call apz_func_OwnerOverShip(:UserName,'1470000128', OwnerView);
	
	--only load few columns for Navigator 
	IF :LoadForNode='Y'
	THEN
		SELECT "ClgCode", "Details", ifnull(b."Authorize", 'F') "OwnerAuth" FROM APZ_OCLG a LEFT join :OwnerView b ON ifnull(a."Owner", -1) = ifnull(b."OwnerId",-2)
				 where (ifnull(a."Owner", 0) = 0 or a."Owner" = -1 or ifnull(b."Authorize", 'N') <> 'N') 
				 		and a."Action"='T' AND a."parentTask" = :ClgCode order by a."ClgCode" DESC;
	ELSEIF :LoadForNode='N'
	THEN
		SELECT a.*, ifnull(b."Authorize", 'F') "OwnerAuth" FROM APZ_OCLG a LEFT join :OwnerView b ON ifnull(a."Owner", -1) = ifnull(b."OwnerId",-2)
				 where (ifnull(a."Owner", 0) = 0 or a."Owner" = -1 or ifnull(b."Authorize", 'N') <> 'N') 
				 		and a."Action"='T' AND a."parentTask" = :ClgCode order by a."ClgCode" DESC;
	END IF;

END




