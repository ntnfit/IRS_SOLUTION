
CREATE PROCEDURE APZ_SubActivities_LoadTaskByObjectType(in UserName nvarchar(254), in ObjType nvarchar(254), in ObjId nvarchar(254))
AS
BEGIN

	if ifnull(:ObjId,'') = '' THEN
		ObjId = '-1';
	END IF;
	--1470000128 is Object Type for Task Management	
	call apz_func_OwnerOverShip(:UserName,'1470000128', OwnerView);
	
	create local temporary table #tmpObjectType as (SELECT TOP 0 * FROM APZ_OCLG);
	--1470000132 is Object Type for ProjectManager
	IF :ObjType='1470000132'
	THEN
		INSERT INTO #tmpObjectType 
			SELECT * FROM APZ_OCLG a WHERE  
				--put condition that is faster to check to before condition that is slower to check
				"Action"='T' AND "parentTask" IS NULL AND "ProjectEntry"= cast(:ObjId as int);

		--opportunities
	ELSEIF :ObjType='97'
	THEN
			INSERT INTO #tmpObjectType 
				SELECT * FROM APZ_OCLG a WHERE  
					--put condition that is faster to check to before condition that is slower to check
					"Action"='T' AND "parentTask" IS NULL AND "OprId"= cast(:ObjId as int);
	--other object type
	--=> documents
		
	ELSE

		INSERT INTO #tmpObjectType 
			SELECT * FROM APZ_OCLG a WHERE  
				--put condition that is faster to check to before condition that is slower to check
				"Action"='T' AND "parentTask" IS NULL AND "DocType"=:ObjType AND "DocEntry"= :ObjId;			
	END IF;
	
	--filter ownership
	create local temporary table #tmpList as 
	(
		SELECT distinct a."ClgCode", ifnull(b."Authorize", 'F') "OwnerAuth"
			FROM #tmpObjectType a LEFT join :OwnerView b ON ifnull(a."Owner", -1) = ifnull(b."OwnerId",-2)
				 where ifnull(a."Owner", 0) = 0 or a."Owner" = -1 or ifnull(b."Authorize", 'N') <> 'N'
	);
	
	SELECT a.*,b."OwnerAuth" FROM APZ_OCLG a inner join #tmpList b On a."ClgCode" = b."ClgCode" order by "ClgCode" DESC;
	
	DROP TABLE #tmpObjectType;

END



