
CREATE PROCEDURE apz_calendarsetting_get(in userName NVARCHAR(50),in ObjTypeTask NVARCHAR(20), in ObjTypeEvent NVARCHAR(20), in ObjTypeNote NVARCHAR(20))
AS
BEGIN
	declare userSignId smallint;
	declare ResourceSign nvarchar(100);
	
	select ifnull(cast((select TOP 1 USERID from OUSR where USER_CODE = :userName) as smallint),0) into userSignId FROM DUMMY;
	select 'U' || cast(:userSignId as NVARCHAR) into ResourceSign FROM DUMMY;
	
	create local temporary table #tbl
	(
		"ResourceId" NVARCHAR(200) ,
		"ResName" NVARCHAR(254),
		"Color" INT DEFAULT -1 ,
		"Remarks" NVARCHAR(254),
		"Checked" CHAR(1) DEFAULT 'N',
		"CellColor" INT DEFAULT -1,
		"UserSign" NVARCHAR(20)
	);
	
	create local temporary table #tblRes
	(
		"ResourceId" NVARCHAR(200) ,
		"ResName" NVARCHAR(254)
	);
	
	call apz_func_OwnerOverShip(:userName,:ObjTypeTask, ownnerShipTask);
	call apz_func_OwnerOverShip(:userName,:ObjTypeEvent, ownnerShipEvent);
	call apz_func_OwnerOverShip(:userName,:ObjTypeNote, ownnerShipNote);
	 
	INSERT INTO #tbl    
		SELECT * FROM apz_OACS a WHERE "UserSign"=:userName;
	
	INSERT INTO #tbl
		SELECT * FROM apz_OACS a WHERE "UserSign" = '' AND NOT EXISTS(SELECT 1 FROM #tbl);
		
	create local temporary table #tblUSER ("ResourceId" nvarchar(254) null, "ResName" nvarchar(254) null);
	create local temporary table #tblEMPL ("ResourceId" nvarchar(254) null, "ResName" nvarchar(254) null);
	
	insert into #tblUSER 
		select 'U' || cast(USERID as NVARCHAR) as "ResourceId", ifnull(U_NAME,'Unknown') as "ResName"
			from  OUSR where "Locked" = 'N' 
				and "GROUPS" != 99
				and (USERID in (select distinct x."UserId" from  :ownnerShipTask x) OR USERID in (select distinct x."UserId" from  :ownnerShipEvent x) OR USERID in (select distinct x."UserId" from  :ownnerShipNote x));
		
	insert into #tblEMPL 
		select  'E' || cast("empID" as NVARCHAR) as "ResourceId", ifnull("firstName",'') || ' ' || ifnull("middleName", '') || ' ' || ifnull("lastName",'') as "ResName"
			from  OHEM
				where "Active" = 'Y'and ("empID" in (select distinct x."OwnerId" from  :ownnerShipTask x) OR "empID" in (select distinct x."OwnerId" from  :ownnerShipEvent x) OR "empID" in (select distinct x."OwnerId" from  :ownnerShipNote x));
	
	Insert into #tblRes Select * from #tblUSER UNION ALL select * from #tblEMPL;
	
	create local temporary table #tbl1 as (select top 0 a."ResourceId",a."ResName",ifnull(b."Color",-1) as Color,ifnull(b."Remarks",'') as "Remarks",
		'Y' "Checked",ifnull(b."CellColor",-1) as "CellColor",cast(null as nvarchar(50)) as "UserSign" from #tblRes a left join #tbl b on 1=1);
	
	insert into #tbl1
		select distinct(a."ResourceId"),a."ResName",ifnull(b."Color",-1) as Color,ifnull(b."Remarks",'') as "Remarks",
			CASE when (a."ResourceId" = :ResourceSign and b."ResourceId" is null) then 'Y' else ifnull(b."Checked",'N') end as "Checked",ifnull(b."CellColor",-1) as "CellColor",:userName as "UserSign" from #tblRes a left join #tbl b on a."ResourceId" = b."ResourceId";
	
	DELETE from #tbl;
	
	INSERT INTO #tbl    
		SELECT * FROM #tbl1 a;
		
	create local temporary table #tblgen as (SELECT top 0 * FROM apz_ACS1);
	
	insert into #tblgen
		SELECT * FROM apz_ACS1 a WHERE "UserSign"=:userName;
		
	INSERT INTO #tblgen
		SELECT * FROM apz_ACS1 a WHERE "UserSign"='' AND NOT EXISTS(SELECT 1 FROM #tblgen);
	
	
	Select * from #tbl  order by "ResName" ASC;
	Select * from #tblgen;
END



