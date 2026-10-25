

CREATE PROCEDURE APZ_D_GetListStore
(
	IN cardType NVARCHAR(50) DEFAULT '',
	IN cardName nvarchar(254) DEFAULT '',
	IN subType char(1),
	IN saleUser nvarchar(50),
	IN type char(1) DEFAULT ''
)
AS
BEGIN
	
	create local temporary table #tmpStore ("CardCode" nvarchar(50), "CardName" nvarchar(254));
	CALL APZ_D_FilterStores('','',:subType,:saleUser, N'#tmpStore');
	
	create local temporary table #tblDKCTKM as (SELECT * FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY "CardCode" ORDER BY "CardCode", "CreateDate" DESC) AS rn FROM APZ_D_PMO3) where rn = 1);
	
	IF :type = 'M' THEN
	
		SELECT A."CardCode", A."CardName", A."Cellular",A."CardType", B."Name" AS "CntPrsName",B."CntctCode", IFNULL(D."Street", '') || ' ' || IFNULL(D."City", '') || ' ' || IFNULL(D."Country", '') AS "ShipToAddress", A."BillToDef", A."ShipToDef",
		IFNULL(E."Street", '') || ' ' || IFNULL(E."City", '') || ' ' || IFNULL(E."Country", '') AS "BillToAddress", A."U_Latitude", A."U_Longitude", 
		case when N."CreateDate" between add_years(current_date, -1) and current_date then 'red' else 'dodgerblue' end AS "MarkerColor", '' AS "MarkerIcon"
		,"U_Province" as "TinhTP",ifnull((select string_agg(cast("EmpId" as nvarchar), ',') from APZ_D_STR2 where "CardCode" = a."CardCode"), cast(M."empID" as nvarchar)) as "saleEmployees",
		A."SlpCode"
		from  OCRD A
		LEFT JOIN OCPR B ON A."CardCode" = B."CardCode" AND A."CntctPrsn" = B."Name" 
		LEFT JOIN CRD1 D ON A."CardCode" = D."CardCode" AND A."ShipToDef" = D."Address" AND d."AdresType" = 'S'
		LEFT JOIN CRD1 E ON A."CardCode" = E."CardCode" AND A."BillToDef" = E."Address" AND E."AdresType" = 'B'
		LEFT JOIN OHEM M ON A."UserSign" = M."userId"
		LEFT JOIN #tblDKCTKM N on a."CardCode" = N."CardCode"
		where A."CardCode" in (select "CardCode" from #tmpStore);
		
	ELSE
		
		declare fromDate date;
		declare toDate date; 
		declare userId int;
		
		select add_days(current_date, -1) into fromDate from dummy;
		select add_days(current_date, 1) into toDate from dummy;
		userId :=  ifnull((select top 1 USERID from OUSR where USER_CODE = :saleUser),0);
		
		CREATE LOCAL TEMPORARY TABLE #tblBusinessType
		(
			"CardCode" nvarchar(50),
			"CardName" nvarchar(254),
			"YesNo" char(1),
			"PropertiesName" nvarchar(4000),
			"PropertiesCode" nvarchar(100)
		);
		
		CREATE LOCAL TEMPORARY TABLE #tblCardCod
		(
		    "CardCode" nvarchar(50),
		    "CardName" nvarchar(254), 
		    "QryGroup1" char(1),"QryGroup2" char(1),"QryGroup3" char(1),"QryGroup4" char(1),"QryGroup5" char(1),"QryGroup6" char(1),"QryGroup7" char(1),"QryGroup8" char(1)
		);
		
		INSERT INTO #tblCardCod SELECT "CardCode", "CardName", "QryGroup1", "QryGroup2", "QryGroup3", "QryGroup4", "QryGroup5", "QryGroup6", "QryGroup7", "QryGroup8" FROM OCRD WHERE "U_SubType" = 'S';
		
		CREATE LOCAL TEMPORARY TABLE #tbl AS
		--unpivot Hana
		( 
			SELECT "CardCode", "CardName",
							MAP(element_number,
							 	1, 'QryGroup1',
							 	2, 'QryGroup2',
							 	3, 'QryGroup3',
							 	4, 'QryGroup4',
							 	5, 'QryGroup5',
							 	6, 'QryGroup6',
							 	7, 'QryGroup7',
							 	8, 'QryGroup8') AS "Properties",
							 MAP(element_number,
							 	1, "QryGroup1",
							 	2, "QryGroup2",
							 	3, "QryGroup3",
							 	4, "QryGroup4",
							 	5, "QryGroup5",
							 	6, "QryGroup6",
							 	7, "QryGroup7",
							 	8, "QryGroup8") AS "YesNo",
							 ROW_NUMBER() over (Partition by "CardCode" order by "CardCode" asc) AS "OrderId"
			FROM #tblCardCod, SERIES_GENERATE_INTEGER(1, 1, 9) 
			ORDER BY element_number
		);
		
		create local temporary table #tblYes as (select * from #tbl where "YesNo" = 'Y');
		create local temporary table #ret ("CardCode" nvarchar(15), "CardName" nvarchar(100), "YesNo" char(1), "Name" nvarchar(1000), "Code" nvarchar(254));
		
		insert into #ret
			select distinct T0."CardCode", T0."CardName",T0."YesNo",
			(select string_agg(t1."GroupName", ',') from #tbl x inner join OCQG t1 on x."OrderId"=t1."GroupCode" where x."CardCode"=t0."CardCode" and "YesNo"='Y') as "Name",
			(select string_agg(cast(x."OrderId" as nvarchar),',') from #tbl x  where x."CardCode"=t0."CardCode" and "YesNo"='Y') as "Code"
			from #tblYes T0;
		 
		INSERT INTO #tblBusinessType SELECT "CardCode", "CardName", "YesNo", "Name" AS "PropertiesName", "Code" AS "PropertiesCode" FROM #ret;
		drop table #tbl;
		drop table #ret;
		drop table #tblYes;
		
		select t.*, ifnull(
		(select "EmpId" from (select "EmpId", row_number() over(order by "Id","RouteCode","EmpId","CardCode") row_id from APZ_D_STR2 where "CardCode" = t."CardCode") route where row_id = 1)
		, t."CreateEmployee") as "saleEmployee", case when "NumOfOrder" >0 then 'Y' else 'N' end as "OrderStatus" from 
	
		(SELECT A."CardCode", A."CardName", IFNULL(A."Balance", 0) AS "Balance", A."Cellular", A."CardType", B."Name" AS "CntPrsName", A."SlpCode",
		IFNULL(D."Street", '') || ' ' || IFNULL(D."City", '') || ' ' || IFNULL(D."Country", '') AS "ShipToAddress", 
		IFNULL(E."Street", '') || ' ' || IFNULL(E."City", '') || ' ' || IFNULL(E."Country", '') AS "BillToAddress", A."CreateDate", A."CreateTS", A."U_Latitude", A."U_Longitude", A."ListNum", A."BillToDef",A."ShipToDef", 
		"U_Province" as "TinhTP", F."PropertiesCode", F."PropertiesName" AS "BusinessType", 
		T1."FileName", IFNULL(T1."LinkThumbnails", IFNULL(T1."LinkDownload", (SELECT TOP 1 "DefaultAvatar" FROM APZ_D_OAPC WHERE "AppName" = 'i-DMS'))) AS "LinkThumbnails", IFNULL(T1."LinkDownload", 
		(SELECT TOP 1 "DefaultAvatar" FROM APZ_D_OAPC WHERE "AppName" = 'i-DMS')) AS "LinkDownload", (SELECT COUNT("Guid") FROM APZ_D_VISIT 
		WHERE A."CardCode" = "StoreCode" AND YEAR("FinishedDate") = YEAR(current_date) AND MONTH("FinishedDate") = MONTH(current_date)) AS "NumOfVisit", 
		IFNULL((SELECT COUNT(*) FROM ORDR WHERE A."CardCode" = "CardCode" AND YEAR("DocDate") = YEAR(current_date) AND MONTH("DocDate") = MONTH(current_date) AND U_GUID IS NOT NULL AND "UserSign" = :userId), 0) + IFNULL((SELECT COUNT(*) 
		FROM OINV WHERE A."CardCode" = "CardCode" AND YEAR("DocDate") = YEAR(current_date) AND MONTH("DocDate") = MONTH(current_date) AND U_GUID IS NOT NULL AND "UserSign" = :userId), 0) AS "NumOfOrder", 
		(SELECT COUNT(*) FROM ORDR WHERE A."CardCode" = "CardCode" AND YEAR("DocDate") = YEAR(current_date) AND MONTH("DocDate") = MONTH(current_date) AND U_GUID IS NOT NULL AND "UserSign" = :userId 
		AND "DocStatus" = 'O') AS "NotDeliveredQty", 0 AS "UnPaidQty", M."empID" as "CreateEmployee"
		 
		FROM OCRD A LEFT OUTER JOIN OCPR B ON A."CardCode" = B."CardCode" AND A."CntctPrsn" = B."Name" 
		LEFT OUTER JOIN CRD1 D ON A."CardCode" = D."CardCode" AND A."ShipToDef" = D."Address" AND d."AdresType" = 'S' 
		LEFT OUTER JOIN CRD1 E ON A."CardCode" = E."CardCode" AND A."BillToDef" = E."Address" AND E."AdresType" = 'B' 
		LEFT OUTER JOIN #tblBusinessType F ON A."CardCode" = F."CardCode" LEFT OUTER JOIN APZ_D_UPIM T1 ON A."CardCode" = T1."CardCode" AND "IsAvatar" = 'Y'
		LEFT OUTER JOIN OHEM M ON A."UserSign" = M."userId"
		WHERE A."CardCode" IN (SELECT "CardCode" FROM #tmpStore)) t;
	END IF;

END




