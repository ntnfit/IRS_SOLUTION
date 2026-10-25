
CREATE PROCEDURE APZ_SubActivities_SaveComment(in AdditionNotifyUser nvarchar(40000) default '')
AS
BEGIN

 	DECLARE Code int = 0;
 	DECLARE ID int = 0;
 	DECLARE ObjType nvarchar(40000) = '';
 	DEClARE UserSign nvarchar(40000) = '';
 	declare MaxId int = 0;
	
	create local temporary table #APZ_CommentData as (select top 0 * from APZ_CLG1);
	exec 'drop table #APZ_CommentData;';
	exec 'create local temporary table #APZ_CommentData as (select * from "#APZ_Comment");';
	
	SELECT IFNULL((select top 1 "CmtCode" from #APZ_CommentData),0) into Code from DUMMY;

	SELECT IFNULL((SELECT TOP 1 "ID" FROM #APZ_CommentData),0) INTO ID FROM DUMMY;
 
 	SELECT IFNULL((SELECT TOP 1 "ObjType" FROM #APZ_CommentData),'') into ObjType from DUMMY;

 	SELECT IFNULL((SELECT TOP 1 cast("UserSign" as nvarchar) FROM #APZ_CommentData),'') into UserSign FROM DUMMY;

 	delete from APZ_CLG1 where "CmtCode" = :Code;

	select ifNULL((select Max("CmtCode") from APZ_CLG1),0) +1 into MaxId FROM DUMMY;
	  
	  INSERT INTO APZ_CLG1 (ID, "ObjType", "CmtCode", "UserName", "UserSign", "CreatedTime", "Content") 
	  	SELECT ID, "ObjType", :MaxId, "UserName", "UserSign", "CreatedTime", "Content" FROM #APZ_CommentData; --WHERE IFNULL("CmtCode", 0) = 0;
	
	  --APZ_CLG3 is table to save comment unread status
	  --need to get a list of users will be notify new comment
	  
	  --foreach user
	  --add a new row with Unread='Y' to notify unread status
	  --misunderstood between the source of #UserSignList
	  --should be APZ_CLG1 instead of APZ_CLG3
	  --because APZ_CLG3 only contains users haven't read comments
	create local temporary table #UserSignList ("UserSign" int);
	insert INTO #UserSignList
	 SELECT "UserSign" FROM APZ_CLG1 WHERE ID = :ID AND "ObjType"=:ObjType;
	 
	 --namnx set thêm số người bình luận người giao issue và người thực hiện
	if(:ObjType = '1470000130')
    THEN
    	BEGIN
			declare reporter int;
			declare handle int;
		  	declare reporterUserId int;
		  	declare handleUserId int;
		  	
		  	select ifnull((select top 1 "Reporter" from APZ_TGIE where "Id" = :ID), -1) into reporter from dummy;
		  	select ifnull((select top 1 "HandleBy" from APZ_TGIE where "Id" = :ID), -1) into handle from dummy;
		  	select IFNull((select top 1 "userId" from OHEM where "empID" = :reporter),-1) into reporterUserId from DUMMY;
		  	select ifnull((select top 1 "userId" from OHEM where "empID" = :handle),-1) into handleUserId from DUMMY;
			if :reporterUserId <> -1
			THEN
				insert into #UserSignList("UserSign") values(:reporterUserId);
			end IF;
			if :handleUserId <> -1
			THEN
				insert into #UserSignList("UserSign") values(:handleUserId);
			end IF;
		END;
	 END IF;
	 
	  --insert addition notify users into #userSignList
	IF ifnull(:AdditionNotifyUser, '') !=''
	THEN
	  	INSERT INTO #UserSignList
	 		 SELECT Data FROM Split(:AdditionNotifyUser,';');
	END IF;
	  --need select distinct to prevent duplicate usersign value
	  --because #UserSignList contains all usersign from a conversation
	  --and user can appear in a conversation multi times
	  --=>usersign of that user can be repeated in #UserSignList
	  --=>need to select DISTINCT to prevent duplicate value
	  --when insert into APZ_CLG3
	  --excluded the poster
	  --because the poster always see the comment
	  --=> no need for unread notify
	
	  --need to select distinct before run insert into .. select
	  --because select into would ignore DISTINCT?
	  delete from APZ_CLG3 where "CmtCode" = :maxId;
	  INSERT into APZ_CLG3("CmtCode","UserSign","Unread") select :MaxId "CmtCode",x."UserSign", case when x."UserSign" = :userSign then 'N' else 'Y' end "Unread" from (SELECT DISTINCT "UserSign" FROM #UserSignList) x;
	  
	  SELECT :MaxId from DUMMY;
	
	  DROP TABLE #UserSignList;
 
END




