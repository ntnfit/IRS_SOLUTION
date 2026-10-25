
CREATE PROCEDURE APZ_SubActivities_LoadTask(in UserName nvarchar(40000), in TaskStatus nvarchar(40000) default '')
AS
BEGIN
		
	---------------------------- lấy dữ liệu onner ship ---------------------------------
	call apz_func_OwnerOverShip(:UserName,'1470000128', OwnerView);
	
	--SELECT a.* FROM APZ_OCLG a
	--INNER JOIN #tmpTaskList b ON a.ClgCode=b.ClgCode
	
	SELECT a.*, ifnull(b."Authorize", 'F') "OwnerAuth" FROM APZ_OCLG a
				LEFT join (select distinct "OwnerId", "Authorize" from :OwnerView) b ON ifnull(a."Owner", -1) = ifnull(b."OwnerId",-2)
				 where (ifnull(a."Owner", 0) = 0 or a."Owner" = -1 or ifnull(b."Authorize", 'N') <> 'N') 
				 		and a."Action"='T' AND a."parentTask" IS NULL AND (ifnull(:TaskStatus,'')='' OR "Closed" = :TaskStatus) order by a."ClgCode" DESC;

END




