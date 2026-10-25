
CREATE PROCEDURE APZ_SubActivities_LoadSingleTask(in ClgCode int, in UserName nvarchar(40000))
AS
BEGIN
		
	call apz_func_OwnerOverShip(:UserName,'1470000128', OwnerView);	
	
	SELECT a.*, ifnull(b."Authorize", 'F') "OwnerAuth" FROM APZ_OCLG a LEFT join :OwnerView b ON ifnull(a."Owner", -1) = ifnull(b."OwnerId",-2)
				 where (ifnull(a."Owner", 0) = 0 or a."Owner" = -1 or ifnull(b."Authorize", 'N') <> 'N') 
				 		and a."Action"='T' AND "ClgCode" = :ClgCode;

END




