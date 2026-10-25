
CALL APZ_EXISTSPROC('APZ_D_GetListStore_V2');

GO


CREATE PROCEDURE APZ_D_GetListStore_V2
(
	IN cardType NVARCHAR(50) DEFAULT '',
	IN cardName nvarchar(254) DEFAULT '',
	IN subType char(1),
	IN saleUser nvarchar(50),
	IN type char(1) DEFAULT '',
	--filter type A - All, O - Ordered, NO - No order, V - Visited, NV - no visited, SC - store created, VS - visit schedule
	in filterType nvarchar(10) default '',
    in searchText NVARCHAR(254) default '',
    in filterProperties nvarchar(254) default '',
	in pageIndex int default -1,
	in fromDate nvarchar(100) default '',
    in toDate nvarchar(100) default '',
    in u_LevelDL nvarchar(100) default '',
    in latitude float default 0,
    in longitude float default 0
)
AS
BEGIN
	declare lowLimit int;
	declare upLimit int;
    declare userId int;
	declare dateName nvarchar(10);
    declare dayOfDate nvarchar(3);
    declare totalRecord int;
    declare tblBusinessType table 
    (
        "CardCode" nvarchar(50),
        "CardName" nvarchar(254),
        "YesNo" char(1),
        "PropertiesName" nvarchar(5000),
        "PropertiesCode" nvarchar(100)
    );
    declare tblCardCod table
    (
        "CardCode" nvarchar(50),
        "CardName" nvarchar(254), 
        "QryGroup1" char(1),
        "QryGroup2" char(1),
        "QryGroup3" char(1),
        "QryGroup4" char(1),
        "QryGroup5" char(1),
        "QryGroup6" char(1),
        "QryGroup7" char(1),
        "QryGroup8" char(1)
    );
    declare ret table
    (
        "CardCode" nvarchar(50), 
        "CardName" nvarchar(254), 
        "YesNo" char(1), 
        "Name" nvarchar(5000), 
        "Code" nvarchar(100)
    );
    declare tbl_VisitSchedule table ("CardCode" nvarchar(50));

    dayOfDate := cast((select extract(day from current_date) from dummy) as varchar(3));
	dateName := cast((select dayname(current_date) from dummy) as varchar(10));
    lowLimit = 20 * :pageIndex - 20;
	upLimit = :pageIndex * 20;
    userId :=  ifnull((select top 1 USERID from OUSR where USER_CODE = :saleUser),0);
	
	create local temporary table #tmpStore ("CardCode" nvarchar(50), "CardName" nvarchar(254));
	CALL APZ_D_FilterStores('','',:subType,:saleUser, N'#tmpStore');
	
	IF :type = 'M' THEN
		tblDKCTKM = SELECT * FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY "CardCode" ORDER BY "CardCode", "CreateDate" DESC) AS rn FROM APZ_D_PMO3) where rn = 1;
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
		LEFT JOIN :tblDKCTKM N on a."CardCode" = N."CardCode"
		where A."CardCode" in (select "CardCode" from #tmpStore);
	ELSE
		
		tblCardCod = SELECT "CardCode", "CardName", "QryGroup1", "QryGroup2", "QryGroup3", "QryGroup4", "QryGroup5", "QryGroup6", "QryGroup7", "QryGroup8" 
        FROM OCRD WHERE "CardCode" in (select "CardCode" from #tmpStore);
		
		tbl = SELECT "CardCode", "CardName",
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
			FROM :tblCardCod, SERIES_GENERATE_INTEGER(1, 1, 9) 
			ORDER BY element_number;
		
		tblYes = select * from :tbl where "YesNo" = 'Y';
		ret =
			select distinct T0."CardCode", T0."CardName",T0."YesNo",
			(select string_agg(t1."GroupName", ',') from :tbl x inner join OCQG t1 on x."OrderId"=t1."GroupCode" where x."CardCode"=t0."CardCode" and "YesNo"='Y') as "Name",
			(select string_agg(cast(x."OrderId" as nvarchar),',') from :tbl x  where x."CardCode"=t0."CardCode" and "YesNo"='Y') as "Code"
			from :tblYes T0;
		 
		tblBusinessType = SELECT "CardCode", "CardName", "YesNo", "Name" AS "PropertiesName", "Code" AS "PropertiesCode" FROM :ret;
		if(ifnull(:filterProperties,'') <> '') then
			-- filter properties
	        tblFilterProperties = select * from split(:filterProperties,',');
	        -- tbl filter business type with cardcode
	        tblBusinessTypeSplit = MAP_MERGE(:tblBusinessType, APZ_D_SplitDataWithCode(:tblBusinessType."CardCode", :tblBusinessType."PropertiesCode",',') );
	        -- after filter
	        tblBusinessTypeFilter = select * from :tblBusinessTypeSplit where "DATA" in (select "DATA" from :tblFilterProperties);
	        -- tbl result
	        tblBusinessTypeResult = select *, case when "CardCode" in (select "Code" from :tblBusinessTypeFilter) then 'Y' else 'N' end as "FilterPropertiesCheck" 
	        from :tblBusinessType;
		else
			tblBusinessTypeResult = select *, 'Y' as "FilterPropertiesCheck" 
	        from :tblBusinessType;
		end if;
        
		
		-- tính khoảng cách vị trí hiện tại với cửa hàng
		tbl_OCRD = SELECT "CardCode",cast("U_Latitude" as float) as "U_Latitude",cast("U_Longitude" as float) as "U_Longitude" 
		FROM "OCRD" where "CardCode" in (select "CardCode" from #tmpStore);
		
   		tbl_Distance = MAP_MERGE(:tbl_OCRD, APZ_D_CalcDistance(:tbl_OCRD."CardCode",:tbl_OCRD."U_Latitude" ,:latitude,:tbl_OCRD."U_Longitude" ,:longitude));
   		-- set giá trị mặc định nếu không tính được khoảng cách
    	tbl_DistanceResult = select * from 
    	(select "CardCode",case when ifnull(nullif("Result",0),0) = 0 then 9999 else "Result" end as "Distance" from :tbl_Distance) t 
    	order by t."Distance" asc;
        -- nếu là type lịch ghé thăm cửa hàng
        if(:filterType = 'VS') then
            tbl_dayOfMonth = select * from APZ_D_OOVF t0 
            where to_date(to_varchar(t0."StartDate",'yyyy-MM-dd'),'yyyy-MM-dd') <= to_date(to_varchar(current_date,'yyyy-MM-dd'),'yyyy-MM-dd')
            and t0."Finished" = 'N' 
            and (t0."DayOfMonth" = 'Y' and CompareSplit(:dayOfDate, t0."ListOfDate", ',') = 1)
            and t0."Code" in (SELECT "CardCode" FROM #tmpStore);
            tbl_listDate = MAP_MERGE(:tbl_dayOfMonth, SplitWithCode(:tbl_dayOfMonth."Code",:tbl_dayOfMonth."ListOfDate", ','));
            tbl_VisitSchedule = select "Code" as "CardCode" from APZ_D_OOVF t0 
            where to_date(to_varchar(t0."StartDate",'yyyy-MM-dd'),'yyyy-MM-dd') <= to_date(to_varchar(current_date,'yyyy-MM-dd'),'yyyy-MM-dd')
            and t0."Finished" = 'N' and 
            (
                t0."EveryDay" = 'Y'	
                or (t0."DayOfMonth" = 'Y' and CompareSplit(:dayOfDate, t0."ListOfDate", ',') = 1
                and (ifnull("RecurNum",0)<=0 or (ifnull("Recurring",'') = 'M' and ifnull("RecurNum",0)>0 and
                APZ_D_ConvertDateFormat(current_date) in (select APZ_D_ConvertDateFormat(
                    ADD_MONTHS(APZ_D_GetRecurFirstExecDate(t0."StartDate",t1."part",'M'),
                    FLOOR(DATEDIFF('month',APZ_D_GetRecurFirstExecDate(t0."StartDate",t1."part",'M'),current_date)/t0."RecurNum")*t0."RecurNum"
                    )) from :tbl_listDate t1) 
                ))
                )
                or ("DayOfWeek" = 'Y' and 
                (case :dateName 
                    when 'MONDAY' 	 then "Mon"
                    when 'TUESDAY' 	 then "Tue"
                    when 'WEDNESDAY' then "Wed"
                    when 'THURSDAY'  then "Thu"
                    when 'FRIDAY'	 then "Fri"
                    when 'SATURDAY'  then "Sat"
                    when 'SUNDAY' 	 then "Sun" end) = 'Y'
                and (ifnull("RecurNum",0)<=0 or (ifnull("Recurring",'') = 'W' and ifnull("RecurNum",0)>0 and
                APZ_D_ConvertDateFormat(current_date) = 
                APZ_D_ConvertDateFormat(add_days(APZ_D_GetRecurFirstExecDate(t0."StartDate",:dateName,'W'),
                FLOOR(DATEDIFF('week',APZ_D_GetRecurFirstExecDate(t0."StartDate",:dateName,'W'),current_date)/t0."RecurNum")*t0."RecurNum"*7))))
                )
            )
            and t0."Code" in (SELECT "CardCode" FROM #tmpStore);
        end if;
		
        tbl_Store = select t.*, ifnull(
		(select "EmpId" from (select "EmpId", row_number() over(order by "Id","RouteCode","EmpId","CardCode") row_id from APZ_D_STR2 where "CardCode" = t."CardCode") route where row_id = 1)
		, t."CreateEmployee") as "saleEmployee", case when "NumOfOrder" >0 then 'Y' else 'N' end as "OrderStatus" from 
		(select A."CardCode", A."CardName", IFNULL(A."Balance", 0) AS "Balance", A."Cellular", A."CardType", B."Name" AS "CntPrsName", A."SlpCode"
        , IFNULL(D."Street", '') || ' ' || IFNULL(D."City", '') || ' ' || IFNULL(D."Country", '') AS "ShipToAddress"
        , IFNULL(E."Street", '') || ' ' || IFNULL(E."City", '') || ' ' || IFNULL(E."Country", '') AS "BillToAddress"
        , A."CreateDate", A."CreateTS", A."U_Latitude", A."U_Longitude", A."ListNum", A."BillToDef",A."ShipToDef"
        , "U_Province" as "TinhTP", F."PropertiesCode", F."PropertiesName" AS "BusinessType", F."FilterPropertiesCheck"
        , T1."FileName", IFNULL(T1."LinkThumbnails", IFNULL(T1."LinkDownload", (SELECT TOP 1 "DefaultAvatar" FROM APZ_D_OAPC WHERE "AppName" = 'i-DMS'))) AS "LinkThumbnails"
        , IFNULL(T1."LinkDownload", (SELECT TOP 1 "DefaultAvatar" FROM APZ_D_OAPC WHERE "AppName" = 'i-DMS')) AS "LinkDownload"
		, case when (((ifnull(:fromDate,'') = '' OR CAST(A."CreateDate" AS date) >= :fromDate) 
	        AND (ifnull(:toDate,'') = '' OR CAST(A."CreateDate" AS date) <= :toDate))) then 'Y' else 'N' end as "StoreCreated"
        , (SELECT COUNT("Guid") FROM APZ_D_VISIT WHERE A."CardCode" = "StoreCode" AND ((ifnull(:fromDate,'') = '' OR CAST("FinishedDate" AS date) >= :fromDate) 
	        AND (ifnull(:toDate,'') = '' OR CAST("FinishedDate" AS date) <= :toDate))) AS "NumOfVisit", 
		(IFNULL((SELECT COUNT(*) FROM ORDR WHERE A."CardCode" = "CardCode" 
            AND ((ifnull(:fromDate,'') = '' OR CAST("DocDate" AS date) >= :fromDate) 
	        AND (ifnull(:toDate,'') = '' OR CAST("DocDate" AS date) <= :toDate))), 0) 
        + IFNULL((SELECT COUNT(*) 
		FROM OINV WHERE A."CardCode" = "CardCode" 
            AND ((ifnull(:fromDate,'') = '' OR CAST("DocDate" AS date) >= :fromDate) 
	        AND (ifnull(:toDate,'') = '' OR CAST("DocDate" AS date) <= :toDate))), 0)) AS "NumOfOrder", 
        -- visit
        (IFNULL((SELECT COUNT(*) FROM ORDR WHERE A."CardCode" = "CardCode" 
            AND ((ifnull(:fromDate,'') = '' OR CAST("DocDate" AS date) >= :fromDate) 
	        AND (ifnull(:toDate,'') = '' OR CAST("DocDate" AS date) <= :toDate)) AND U_GUID IS NOT NULL AND "UserSign" = :userId), 0) 
        + IFNULL((SELECT COUNT(*) 
		FROM OINV WHERE A."CardCode" = "CardCode" 
            AND ((ifnull(:fromDate,'') = '' OR CAST("DocDate" AS date) >= :fromDate) 
	        AND (ifnull(:toDate,'') = '' OR CAST("DocDate" AS date) <= :toDate)) 
            AND U_GUID IS NOT NULL AND "UserSign" = :userId), 0)) AS "NumOfOrderVisit", 
		(SELECT COUNT(*) FROM ORDR WHERE A."CardCode" = "CardCode" 
            AND ((ifnull(:fromDate,'') = '' OR CAST("DocDate" AS date) >= :fromDate) 
	        AND (ifnull(:toDate,'') = '' OR CAST("DocDate" AS date) <= :toDate))
            AND U_GUID IS NOT NULL AND "UserSign" = :userId 
		AND "DocStatus" = 'O') AS "NotDeliveredQty", 0 AS "UnPaidQty", M."empID" as "CreateEmployee"
		 ,N."Distance"
		FROM OCRD A LEFT OUTER JOIN OCPR B ON A."CardCode" = B."CardCode" AND A."CntctPrsn" = B."Name" 
		left JOIN :tbl_DistanceResult N on N."CardCode" = A."CardCode"
		LEFT OUTER JOIN CRD1 D ON A."CardCode" = D."CardCode" AND A."ShipToDef" = D."Address" AND d."AdresType" = 'S' 
		LEFT OUTER JOIN CRD1 E ON A."CardCode" = E."CardCode" AND A."BillToDef" = E."Address" AND E."AdresType" = 'B' 
		LEFT OUTER JOIN :tblBusinessTypeResult F ON A."CardCode" = F."CardCode" LEFT OUTER JOIN APZ_D_UPIM T1 ON A."CardCode" = T1."CardCode" AND "IsAvatar" = 'Y'
		LEFT OUTER JOIN OHEM M ON A."UserSign" = M."userId"
		WHERE ((:filterType = 'VS' and A."CardCode" IN (SELECT "CardCode" FROM :tbl_VisitSchedule)) 
        or (:filterType <> 'VS' and A."CardCode" IN (SELECT "CardCode" FROM #tmpStore)))
        ) t where
        (ifnull(:searchText,'') = '' 
                or (lower(t."CardCode") like N'%'||lower(:searchText)||'%' 
                    or lower(t."CardName") like N'%'||lower(:searchText)||'%' 
                    or lower(t."Cellular") like N'%'||lower(:searchText)||'%' 
                    or lower(t."CntPrsName") like N'%'||lower(:searchText)||'%' 
                    or lower(t."BillToAddress") like N'%'||lower(:searchText)||'%' 
                ))
        --and ((ifnull(:fromDate,'') = '' OR CAST(t."CreateDate" AS date) >= :fromDate) 
	        --AND (ifnull(:toDate,'') = '' OR CAST(t."CreateDate" AS date) <= :toDate))
        --check filter type
        and (ifnull(:filterProperties,'') = '' or (t."FilterPropertiesCheck" = 'Y'))
        and (:filterType = 'A' or :filterType = 'VS'
        or (:filterType = 'O' and t."NumOfOrder">0) 
        or (:filterType = 'NO' and t."NumOfOrder"<=0) 
        or (:filterType = 'V' and t."NumOfVisit">0) 
        or (:filterType = 'NV' and t."NumOfVisit"<=0)
		or (:filterType = 'SC' and t."StoreCreated" = 'Y'));

		totalRecord := ifnull((select count (1) from :tbl_Store),0);

        if(:filterType = 'SC') then
            select *,:totalRecord as "TotalRecord" from (select ROW_NUMBER() OVER (ORDER BY t."CreateDate" desc) "RowNum", t.* from :tbl_Store t) Pagination 
            where (:pageIndex<=0 or (Pagination."RowNum" > :lowLimit and Pagination."RowNum" <= :upLimit)) order by Pagination."RowNum" asc;
        else 
            select *,:totalRecord as "TotalRecord" from (select ROW_NUMBER() OVER (ORDER BY t."Distance" asc) "RowNum", t.* from :tbl_Store t) Pagination 
            where (:pageIndex<=0 or (Pagination."RowNum" > :lowLimit and Pagination."RowNum" <= :upLimit)) order by Pagination."RowNum" asc;
        end if;
	END IF;
END
