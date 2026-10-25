
CALL APZ_EXISTSPROC('APZ_D_Promotion_GetList');

GO


CREATE  PROCEDURE APZ_D_Promotion_GetList
(
	promotionType int DEFAULT 0, -- 0 view, 1 list promotion by store code, 2 list store register promotion by promotion code, 3 get all promotion apz_d_opmo
	cardCode nvarchar(100) DEFAULT '',
	promotionCode nvarchar(100) DEFAULT '',
	groupCode nvarchar(100) DEFAULT '',
	userCode nvarchar(50) DEFAULT ''
)
AS
BEGIN
	DECLARE userId int;
	declare empID int;
	create local temporary table #tmpEmployees ("empID" int);
	
	userId := IFNULL((SELECT USERID FROM OUSR WHERE USER_CODE = :userCode), 0);
	empID := IFNULL((SELECT TOP 1 a."empID" FROM OHEM a LEFT OUTER JOIN OUSR b ON a."userId" = b.USERID WHERE b.USER_CODE = :userCode), 0);
	
	call APZ_D_FilterEmployees(:userId, '#tmpEmployees');
	insert into #tmpEmployees values(:empID);
	
	create local temporary table #tmpStore ("CardCode" nvarchar(50), "CardName" nvarchar(254));
	CALL APZ_D_FilterStores ('', '', 'S', :userCode, N'#tmpStore');

	create local temporary table #tbl as 
	(
		SELECT * FROM (SELECT ROW_NUMBER() OVER (PARTITION BY "DocEntry","PakageNo" ORDER BY "LineId") AS "RowNumber", 
		"DocEntry", "LineId", "RowId", "ColId", "Code", "PakageNo", "BenefitGroup", "BenefitCode" FROM APZ_PPR1) AS GR WHERE GR."RowNumber" = 1
	);
	
	create local temporary table #tblRouteOfEmployee ("Code" nvarchar(50));
	-- ctkm ctkm theo tuyến của nhân viên
	insert into #tblRouteOfEmployee
		select distinct T2."Code" from APZ_PCN1 T0 
									INNER JOIN APZ_OPCN T1 ON T0."DocEntry"=T1."DocEntry" and T1."Type"=8
									INNER JOIN APZ_PPR2 T2 on T0."Code"=T2."ConditionCode"
									INNER JOIN APZ_D_OSTR T3 ON T0."TypeCode"=T3."RouteCode"
									INNER JOIN APZ_D_STR1 T4 ON T3."RouteCode"=T4."RouteCode"
									INNER JOIN APZ_D_STR4 T5 ON T4."RouteCode"=T5."RouteCode" and T4."Id"=T5."Id"
									INNER JOIN OHEM T6 ON T5."EmpId"=T6."empID" 
				where T5."EmpId" in (select  "empID" from #tmpEmployees);
	--select * from #tblRouteOfEmployee
	-- các ctkm theo tuyến
	create local temporary table #tblRoute as (select Distinct "Code" from APZ_PPR2 WHERE "Code" NOT IN(SELECT "Code" FROM #tblRouteOfEmployee) AND "ConditionCode" IN 
													(SELECT T0."Code" FROM APZ_PCN1 T0 INNER JOIN APZ_OPCN T1 ON T0."DocEntry"=T1."DocEntry" AND T1."Type"=8));
	 --select * from #tblRoute
	
	
	if :promotionType = 0 then
		SELECT A.*, 
		CASE WHEN IFNULL(A."PakageSplit", 'N') = 'Y' THEN B."PakageNo" ELSE NULL END AS "PakageNo", 
		CASE WHEN IFNULL(A."PakageSplit", 'N') = 'Y' THEN (SELECT COUNT(t0."CardCode") FROM APZ_D_PMO3 t0 WHERE t0."Code" = A."Code" AND t0."StatusCode" = 'Y' AND t0."GroupCode" = B."PakageNo" AND (t0."CardCode" IN (SELECT "CardCode" FROM #tmpStore)))
		ELSE (SELECT COUNT(t0."CardCode") FROM APZ_D_PMO3 t0 WHERE t0."Code" = A."Code" AND t0."StatusCode" = 'Y' AND (t0."CardCode" IN (SELECT "CardCode" FROM #tmpStore)))
		END AS "NumStoreRegister"
		FROM APZ_OPPR A INNER JOIN #tbl B ON A."DocEntry" = B."DocEntry" AND (A."PakageSplit" = 'Y' OR IFNULL(A."PakageSplit", 'N') = 'N' AND 
		B."LineId" = (SELECT max("LineId") FROM #tbl B WHERE B."DocEntry" = A."DocEntry"))
		WHERE A."Active" = 'Y' 
		AND (A."EndDate" is null or A."EndDate" = '' or A."EndDate" >= current_date)
		and (:empID=0 or A."Code" in (select "Code" from #tblRouteOfEmployee) or A."Code" not in (select "Code" from #tblRoute));
		
	elseif :promotionType = 1 then
		SELECT A.*, 
			    CASE 
			        WHEN IFNULL(A."PakageSplit", 'N') = 'Y' THEN B."PakageNo" 
			        ELSE NULL 
			    END AS "PakageNo", 
			    CASE 
			        WHEN IFNULL(A."PakageSplit", 'N') = 'Y' THEN 
			        (SELECT COUNT(t0."CardCode") 
			        FROM APZ_D_PMO3 t0 
			        WHERE t0."Code" = A."Code" AND t0."StatusCode" = 'Y' AND t0."GroupCode" = B."PakageNo" AND
			             (t0."CardCode" IN (SELECT "CardCode" 
			            FROM #tmpStore))) 
			        ELSE 
			        (SELECT COUNT(t0."CardCode") 
			        FROM APZ_D_PMO3 t0 
			        WHERE t0."Code" = A."Code" AND t0."StatusCode" = 'Y' AND (t0."CardCode" IN (SELECT "CardCode" 
			            FROM #tmpStore))) 
			    END AS "NumStoreRegister", 
			    CASE 
			        WHEN IFNULL(A."PakageSplit", 'N') = 'Y' THEN (IFNULL((
			        select "StatusCode" from (SELECT t0."StatusCode", row_number() over() as row_id 
			        FROM APZ_D_PMO3 t0 
			        WHERE A."Code" = t0."Code" AND t0."CardCode" = :cardCode AND t0."GroupCode" = B."PakageNo") where row_id = 1), 'N')) 
			        ELSE (IFNULL((
			        select "StatusCode" from (SELECT t0."StatusCode", row_number() over() as row_id
			        FROM APZ_D_PMO3 t0 
			        WHERE A."Code" = t0."Code" AND t0."CardCode" = :cardCode) where row_id = 1), 'N')) 
			    END AS "StatusCode", 
			    CASE 
			        WHEN IFNULL(A."PakageSplit", 'N') = 'Y' THEN (IFNULL((
			        select "Quantity" from (SELECT t0."Quantity", row_number() over() as row_id
			        FROM APZ_D_PMO3 t0 
			        WHERE A."Code" = t0."Code" AND t0."CardCode" = :cardCode AND t0."GroupCode" = B."PakageNo") where row_id = 1), 0)) 
			    ELSE (IFNULL((
			        select "Quantity" from (SELECT t0."Quantity", row_number() over() as row_id
			        FROM APZ_D_PMO3 t0 
			        WHERE A."Code" = t0."Code" AND t0."CardCode" = :cardCode) where row_id = 1), 0)) 
			    END AS "Quantity",
			    case when ifnull(A."PakageSplit",'N') = 'Y' 
			    		then (ifnull((select "RegisterRemarks" from (select t0."RegisterRemarks", row_number() over(order by t0."Code") id from APZ_D_PMO3 t0 where A."Code"=t0."Code" and t0."CardCode" = :cardCode and t0."GroupCode" = B."PakageNo") x where id = 1),null)) 
					else (ifnull((select "RegisterRemarks" from (select t0."RegisterRemarks", row_number() over(order by t0."Code") id from APZ_D_PMO3 t0 where A."Code"=t0."Code" and t0."CardCode" = :cardCode) x where id = 1),null)) end as "RegisterRemarks"
			    
				FROM APZ_OPPR A 
			    INNER JOIN #tbl B ON A."Code" = B."Code" AND 
			        CASE 
			            WHEN (A."PakageSplit" = 'Y') OR (IFNULL(A."PakageSplit", 'N') = 'N' AND B."LineId" = 
			            (SELECT MAX("LineId") 
			            FROM #tbl 
			            WHERE B."DocEntry" = A."DocEntry")) THEN 1 
			            ELSE 0 
			        END = 1 
					WHERE A."Active" = 'Y' AND (A."EndDate" IS NULL OR A."EndDate" = '' OR A."EndDate" >= current_date)
							and (:empID=0 or A."Code" in (select "Code" from #tblRouteOfEmployee) or A."Code" not in (select "Code" from #tblRoute))
					ORDER BY "StatusCode" DESC;
		
	elseif :promotionType = 2 then

		declare splitGroup nvarchar(10) = IFNULL((SELECT TOP 1 "PakageSplit" FROM APZ_OPPR WHERE "Code" = :promotionCode), 'N');
		IF :splitGroup = 'Y' THEN SELECT * FROM APZ_D_PMO3 
			WHERE "Code" = :promotionCode AND "GroupCode" = :groupCode AND 
			("CardCode" IN (SELECT "CardCode" FROM #tmpStore) OR "CardCode" IN (SELECT "CardCode" FROM OCRD WHERE "UserSign" = :userId));
		ELSE 
			SELECT * FROM APZ_D_PMO3 
			WHERE "Code" = :promotionCode AND ("CardCode" IN (SELECT "CardCode" FROM #tmpStore) 
			OR "CardCode" IN (SELECT "CardCode" FROM OCRD WHERE "UserSign" = :userId));
		END IF;
		
	elseif :promotionType = 3 then 

		select * from APZ_OPPR;
	elseif :promotionType = 4 then

		SELECT * 
		FROM (SELECT A.*, 
        CASE 
            WHEN IFNULL(A."PakageSplit", 'N') = 'Y' THEN B."PakageNo" 
            ELSE NULL 
        END AS "PakageNo", 
        CASE 
            WHEN IFNULL(A."PakageSplit", 'N') = 'Y' THEN 
            (SELECT COUNT(t0."CardCode") 
            FROM APZ_D_PMO3 t0 
            WHERE t0."Code" = A."Code" AND t0."StatusCode" = 'Y' AND t0."GroupCode" = B."PakageNo" AND
                 (t0."CardCode" IN (SELECT "CardCode" 
                FROM #tmpStore))) 
            ELSE 
            (SELECT COUNT(t0."CardCode") 
            FROM APZ_D_PMO3 t0 
            WHERE t0."Code" = A."Code" AND t0."StatusCode" = 'Y' AND (t0."CardCode" IN (SELECT "CardCode" 
                FROM #tmpStore))) 
        END AS "NumStoreRegister", 
        CASE 
            WHEN IFNULL(A."PakageSplit", 'N') = 'Y' THEN (IFNULL((
            select "StatusCode" from (SELECT t0."StatusCode", row_number() over() as row_id 
            FROM APZ_D_PMO3 t0 
            WHERE A."Code" = t0."Code" AND t0."CardCode" = :cardCode AND t0."GroupCode" = B."PakageNo") where row_id = 1), 'N')) 
            ELSE (IFNULL((
            select "StatusCode" from (SELECT t0."StatusCode", row_number() over() as row_id
            FROM APZ_D_PMO3 t0 
            WHERE A."Code" = t0."Code" AND t0."CardCode" = :cardCode) where row_id = 1), 'N')) 
        END AS "StatusCode", 
        CASE 
            WHEN IFNULL(A."PakageSplit", 'N') = 'Y' THEN (IFNULL((
            select "Quantity" from (SELECT t0."Quantity", row_number() over() as row_id
            FROM APZ_D_PMO3 t0 
            WHERE A."Code" = t0."Code" AND t0."CardCode" = :cardCode AND t0."GroupCode" = B."PakageNo") where row_id = 1), 0)) 
            ELSE (IFNULL((
            select "Quantity" from (SELECT t0."Quantity", row_number() over() as row_id
            FROM APZ_D_PMO3 t0 
            WHERE A."Code" = t0."Code" AND t0."CardCode" = :cardCode) where row_id = 1), 0)) 
        END AS "Quantity",
        case when ifnull(A."PakageSplit",'N') = 'Y' 
        		then (ifnull((select "RegisterRemarks" from (select t0."RegisterRemarks", row_number() over(order by t0."Code") id from APZ_D_PMO3 t0 where A."Code"=t0."Code" and t0."CardCode" = :cardCode and t0."GroupCode" = B."PakageNo") x where id = 1),null)) 
			else (ifnull((select "RegisterRemarks" from (select t0."RegisterRemarks", row_number() over(order by t0."Code") id from APZ_D_PMO3 t0 where A."Code"=t0."Code" and t0."CardCode" = :cardCode) x where id = 1),null)) end as "RegisterRemarks"
        
    FROM APZ_OPPR A 
        INNER JOIN #tbl B ON A."Code" = B."Code" AND 
            CASE 
                WHEN (A."PakageSplit" = 'Y') OR (IFNULL(A."PakageSplit", 'N') = 'N' AND B."LineId" = 
                (SELECT MAX("LineId") 
                FROM #tbl 
                WHERE B."DocEntry" = A."DocEntry")) THEN 1 
                ELSE 0 
            END = 1 
    WHERE A."Active" = 'Y') AS B 
WHERE B."StatusCode" = 'Y';

	end if;

END
