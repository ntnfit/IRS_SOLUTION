

CREATE PROCEDURE APZ_D_SubActivities_SearchByDetails
(
	IN UserName nvarchar(50), 
	IN ObjTypeTask NVARCHAR(20),
	IN ObjTypeEvent NVARCHAR(20),
	IN ObjTypeNote NVARCHAR(20),
	IN Action char(1) DEFAULT '',
	IN SeriesNum int DEFAULT 0,
	IN Details nvarchar(50)
)
as
BEGIN
	declare maxdate datetime = iFnull((select max("endDate") from APZ_OCLG), CURRENT_DATE);
	create local temporary table #tblUSER as (select top 0 a.* , cast(null as nvarchar(100))  as "startDateTime" ,cast(null as nvarchar(100)) as "endDateTime",
	cast(null as nvarchar(254)) as "LocationCode" ,cast(null as nvarchar(100)) as "ProjectCode" from APZ_oclg a);

	insert into #tblUSER 
		SELECT a.*, CAST(CAST(a."Recontact" AS date) AS nvarchar) || ' ' || CAST(STUFF(RIGHT('0000' + CAST(IFNULL(a."BeginTime", 0) AS varchar(4)), 4), 3, 0, ':') AS nvarchar), 
		CAST(CAST(IFNULL(a."endDate", a."Recontact") AS date) AS nvarchar) || ' ' || CAST(STUFF(RIGHT('0000' + CAST(IFNULL(a."ENDTime", IFNULL(a."BeginTime", 0) + 100) AS varchar(4)), 4), 3, 0, ':') AS nvarchar), 
		c."Name", d."Code"
		FROM APZ_OCLG a LEFT OUTER JOIN OCLO c ON a."Location" = c."Code" LEFT OUTER JOIN APZ_OPRG d ON a."ProjectEntry" = d."AbsEntry" 
		WHERE "Details" LIKE '%' || :Details || '%' AND (:Action = '' OR (:Action <> '' AND "Action" = :Action)) AND (:SeriesNum <= 0 OR (:SeriesNum > 0 AND a."SeriesNum" = :SeriesNum));
	
	call apz_func_OwnerOverShip(:UserName, :ObjTypeTask, ownerShipTask);
	call apz_func_OwnerOverShip(:UserName, :ObjTypeEvent, ownerShipEvent);
	call apz_func_OwnerOverShip(:UserName, :ObjTypeNote, ownerShipNote);
		
	select * from #tblUSER a where ((
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

END




