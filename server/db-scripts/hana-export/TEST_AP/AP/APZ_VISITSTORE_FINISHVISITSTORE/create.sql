CREATE PROCEDURE APZ_VisitStore_FinishVisitStore
AS
BEGIN
	declare docEntry int;
	declare guidCheck nvarchar(200);
	declare offset int = 0;
	declare userID int;
	declare storeName nvarchar(100);
	declare storeAddress nvarchar(500);
	create local temporary table #DocLocal as (select top 0 *, cast('' as nvarchar(8)) "UserCode", '' as "GeoLocationSave" from APZ_D_VISIT);
	exec 'drop table #DocLocal';
	exec 'create local temporary table #DocLocal as (select *,"GeoLocation" as "GeoLocationSave" from "#tempDoc")';

	SELECT TOP 1 "CardName" into storeName FROM OCRD WHERE "CardCode" = (SELECT TOP 1 "StoreCode" FROM #DocLocal);
	SELECT TOP 1 "Address" into storeAddress FROM OCRD WHERE "CardCode" = (SELECT TOP 1 "StoreCode" FROM #DocLocal);
	SELECT USERID into userID FROM OUSR WHERE USER_CODE = (SELECT "UserCode" FROM #DocLocal);

	create local temporary table #LineLocal as (select top 0 * from APZ_D_VISIT1);
	exec 'drop table #LineLocal';
	exec 'create local temporary table #LineLocal as (select * from "#tempLine")';

	create local temporary table #DocCompetitorLocal as (select top 0 *, 0 "NumOfItem" from APZ_D_OCPA);
	exec 'drop table #DocCompetitorLocal';
	exec 'create local temporary table #DocCompetitorLocal as (select * from "#tempDocCompetitor")';

	create local temporary table #LineCompetitorLocal as (select top 0 * from APZ_D_CPA1);
	exec 'drop table #LineCompetitorLocal';
	exec 'create local temporary table #LineCompetitorLocal as (select * from "#tempLineCompetitor")';

	create local temporary table #UpdateInventoryLocal ("ItemCode" nvarchar(20), "ItemName" nvarchar(100), "LineNum" int, "Quantity" nvarchar(100));
	exec 'insert into #UpdateInventoryLocal select "ItemCode","ItemName","LineNum","Quantity" from "#tempUpdateInventory"';

	guidCheck := CAST((select "Guid" from (select "Guid", row_number() over(order by "Guid") row_id from APZ_D_VISIT where "Guid" = (SELECT "Guid" FROM #DocLocal)) vs where row_id = 1)
	AS nvarchar(200));
	SELECT IFNULL(MAX("DocEntry") + 1, 1) into docEntry FROM Apz_D_visit;

	if :guidCheck <> '' THEN
	select '1470000161' as "ObjectType",403 as "Code",'Duplicate Guid '|| ifnull(:guidCheck,'') as "Message" from dummy;
	return;
	end if;

	INSERT INTO Apz_D_Visit ("DocEntry", "Guid", "UserSign", "StoreCode", "StoreName", "Address", "CreateDate", "CreateTime", "LogInstance", "StartDate", "StartTime", "FinishedDate", "FinishedTime", "Geolocation")
	SELECT :docEntry, "Guid", :userID, "StoreCode", :storeName, :storeAddress, CURRENT_DATE, CURRENT_TIME, NULL, "StartDate", CAST("StartDate" AS time), "FinishedDate", CAST("FinishedDate" AS time), "GeoLocationSave" FROM #DocLocal;

	if ::ROWCOUNT<=0 then

	select '1470000161' as "ObjectType",406 as "Code",'Insert table header fail' as "Message" from dummy;
	return;
	else
	select '1470000161' as "ObjectType",200 as "Code",'' as "Message" from dummy;

	UPDATE #LineLocal SET "DocEntry" = :docEntry, "Guid" = (SELECT "Guid" FROM #DocLocal), "CreateDate" = CURRENT_DATE, "CreateTime" = CURRENT_TIME;
	UPDATE #LineLocal SET "Geolocation" = (SELECT "GeoLocationSave" FROM #DocLocal) WHERE "StepCode" <> -1;
	insert into APZ_D_VISIT1 select * from #LineLocal;

	if ::ROWCOUNT<=0 THEN
	select '1470000161' as "ObjectType",406 as "Code",'Insert table line fail' as "Message" FROM DUMMY;
	else
	if exists (select 1 from #UpdateInventoryLocal) THEN

	-- cập nhật giá trị bảng apz_d_ostc (phần update inventory)
	INSERT INTO APZ_D_OSTC ("Code", "CountDate", "CountTime", "StatusCode", "CreateDate", "CreateTime", "UserSign", "UserSign2", "UpdateDate", "UpdateTime")
	SELECT "Guid", "StartDate", CAST("StartDate" AS time), 'O', CURRENT_DATE, CURRENT_TIME, :userID, NULL, NULL, NULL from #DocLocal;

	if ::ROWCOUNT<=0 then
	select '1470000161' as "ObjectType",406 as "Code",'Insert data to table APZ_D_OSTC fail' as "Message" from dummy;
	return;
	end if;
	-- cập nhật giá trị bảng apz_d_stc1 (phần update inventory)
	INSERT INTO APZ_D_STC1 ("Code", "LineNum", "ItemCode", "Dscription", "Quantity")
	SELECT (SELECT "Guid" FROM #DocLocal) AS "Code", "LineNum", "ItemCode", "ItemName" AS "Dscription", 
	cast(replace(ifnull(nullif("Quantity",''),'0'),',','') as int) FROM #UpdateInventoryLocal;
	if ::ROWCOUNT<=0 THEN
	select '1470000161' as "ObjectType",406 as "Code",'Insert data to table APZ_D_STC1 fail' as "Message" FROM DUMMY;
	return;
	end IF;
	end if;

	if exists (select 1 from #DocCompetitorLocal) THEN

	-- cập nhật bảng apz_d_ocpa (phần competitor)
	INSERT INTO APZ_D_OCPA ("Code", "VisitCode", "StoreCode", "CountDate", "CountTime", "StatusCode", "CreateDate", "CreateTime", "UserSign", "UserSign2", "UpdateDate", "UpdateTime", "CompetitorName", "Notes")
	SELECT "Code", "VisitCode", "StoreCode", "CountDate", CAST("CountDate" AS time), "StatusCode", CURRENT_DATE, CURRENT_TIME,
	"UserSign", "UserSign2", "UpdateDate", CAST("UpdateDate" AS time), "CompetitorName", "Notes" FROM #DocCompetitorLocal;
	if ::ROWCOUNT<=0 THEN
	select '1470000161' as "ObjectType",406 as "Code",'Insert data to table APZ_D_OCPA fail' as "Message" FROM DUMMY;
	return;
	end IF;
	-- cập nhật bảng apz_d_ocpa (phần competitor)
	INSERT INTO APZ_D_CPA1 ("Code", "VisitCode", "LineNum", "ItemName", "IsAvailable") (SELECT "Code", "VisitCode", "LineNum", "ItemName", "IsAvailable" FROM #LineCompetitorLocal);
	/*if ::ROWCOUNT<=0 THEN

	SELECT '1470000161' AS "ObjectType", 406 AS "Code", 'Insert data to table APZ_D_CPA1 fail' AS "Message" FROM DUMMY;
	RETURN;
	end IF;*/
	end IF;

	SELECT '1470000161' AS "ObjectType", 200 AS "Code", '' AS "Message" FROM DUMMY;
	end IF;
	end if;
	
END



