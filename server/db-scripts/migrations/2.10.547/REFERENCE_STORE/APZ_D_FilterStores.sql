
CALL APZ_EXISTSPROC('APZ_D_FilterStores');

GO


CREATE PROCEDURE APZ_D_FilterStores
(
	IN cardType NVARCHAR(50) DEFAULT '',
	IN cardName nvarchar(254) DEFAULT '',
	IN subType char(1),
	IN saleUser nvarchar(50),
	IN tempTable nvarchar(10) DEFAULT ''
)
AS
BEGIN

	declare fromDate date;
	declare toDate date;
	declare empID int;
	declare userId int;
	declare doFilter char(1) = ifnull((select "DoFilter" from CINF), 'N');
	declare doMngMth char(1) = ifnull((select "DoMngMth" from OADM), '');
	declare authMode char(1) = ifnull(nullif((select "AuthorizationMode" from APZ_D_OAPC),''), 'R');
	
	userId :=  ifnull((select top 1 USERID  from OUSR where USER_CODE = :saleUser),0);
	create local temporary table #tmpEmployees ("EmpId" int);
	call APZ_D_FilterEmployees(:userId, '#tmpEmployees');
	
	select add_days(current_date, -1) into fromDate from dummy;
	select add_days(current_date, 1) into toDate from dummy;
	empID :=  ifnull((select top 1 a."empID"  from OHEM a left join OUSR b on a."userId" = b.USERID where b.USER_CODE = :saleUser),0);
	
	CREATE LOCAL TEMPORARY TABLE #tbl as (select top 0 * from APZ_D_STR1);
	INSERT INTO #tbl
		SELECT a.* FROM APZ_D_STR1 a inner join APZ_D_OSTR b on a."RouteCode" = b."RouteCode" and b."Status" = 'Y' and a."Status" = 'Y';
	
	create local temporary table #tblVung as (select top 0 * from APZ_D_STR1);
	create local temporary table #temp1 as (select top 0 * from APZ_D_STR1);
	INSERT INTO #temp1
		SELECT * FROM #tbl where "SuperVisor" = :empID or COMPARESPLIT(cast(:empID AS nvarchar), "SalesAdmin", ';') = 1;
	create local temporary table #temp2 as (select top 0 * from APZ_D_STR1);
	
	while exists (select top 1 1 from #temp1) do
		insert into #tblVung select * from #temp1;
		delete from #temp2;
		insert into #temp2 select T0.* FROM #tbl T0 INNER JOIN #temp1 T1 ON t1."Id"=T0."FatherId" AND t0."RouteCode" = T1."RouteCode";
		delete from #temp1;
		insert into #temp1 select * from #temp2;
	end while;
	
	create local temporary table #tblCardCode ("CardCode" nvarchar(15));
	insert into #tblCardCode SELECT b."CardCode" FROM #tblVung a INNER JOIN APZ_D_STR2 b ON a."Id" = b."Id" AND a."RouteCode" = b."RouteCode"  INNER JOIN APZ_D_STR4 c ON b."EmpId" = c."EmpId" AND b."Id" = c."Id" AND a."RouteCode" = c."RouteCode"
	UNION ALL SELECT "CardCode" FROM APZ_D_STR2 a INNER JOIN APZ_D_STR1 b ON a."Id" = b."Id" AND a."RouteCode" = b."RouteCode" INNER JOIN APZ_D_STR4 c ON a."EmpId" = c."EmpId" AND a."Id" = c."Id" AND a."RouteCode" = c."RouteCode" WHERE a."EmpId" = :empID;
	
	CREATE LOCAL TEMPORARY TABLE #result ("CardCode" nvarchar(15), "CardName" nvarchar(100));

	if :authMode = 'O' then 
	call apz_func_OwnerOverShip(:saleUser,'2', OwnerView);
	end if;
	
	INSERT INTO #result
		SELECT A."CardCode", A."CardName"
		FROM OCRD A WHERE (IFNULL(:subType, '') = '' OR (IFNULL(:subType, '') <> '' AND "U_SubType" = :subType)) 
		AND ((A."CardName" LIKE '%' || IFNULL(:cardName, '') || '%' AND IFNULL(:cardName, '') <> '') 
		OR IFNULL(:cardName, '') = '') AND (IFNULL(:cardType, '') = '' OR (IFNULL(:cardType, '') <> '' AND IFNULL(A."CardType", '') = :cardType)) 
		
		AND ((:authMode = 'R' AND (A."CardCode" IN (SELECT DISTINCT ("CardCode") 
		FROM #tblCardCode) OR (:saleUser = '' OR (:saleUser <> '' 
		AND A."UserSign" in (select t0."userId" from OHEM t0 inner join #tmpEmployees t1 on t0."empID" = t1."EmpId") 
		AND A."CardCode" NOT IN (SELECT t0."CardCode" FROM APZ_D_STR2 t0))))) 

		OR (:authMode = 'O' AND (:doFilter='N' OR (:doFilter='Y' AND :doMngMth NOT IN ('A','B')) OR (:doFilter='Y' AND :doMngMth IN ('A','B') AND  (IFNULL(A."OwnerCode",-1)=-1 OR (A."OwnerCode" IN (select distinct x."OwnerId" from :OwnerView x)))))))
		AND (((A."validFor" = 'Y' AND IFNULL(A."frozenFor", 'N') = 'N' AND (IFNULL(a."validFrom", :fromDate) <= current_date AND IFNULL(a."validTo", :toDate) >= current_date)) OR (IFNULL(A."validFor", 'N') = 'N' 
		)
		AND A."frozenFor" = 'Y' AND (IFNULL(a."frozenFrom", :fromDate) > current_date OR IFNULL(a."frozenTo", :toDate) < current_date)) OR (A."validFor" = 'Y' AND A."frozenFor" = 'Y' 
		AND (IFNULL(a."validFrom", :fromDate) <= current_date AND IFNULL(a."validTo", :toDate) >= current_date) AND (IFNULL(a."frozenFrom", :fromDate) > current_date OR IFNULL(a."frozenTo", :toDate) < current_date)) 
		OR (A."validFor" = 'N' AND A."frozenFor" = 'N')) ORDER BY A."CreateDate" DESC, A."CreateTS" DESC;
	if :tempTable <> '' then
		declare query nvarchar(100) = N'insert into ' || :tempTable || ' select * from #result;';
		exec(:query);
	end if;
	
	drop table #tbl;
	drop table #result;
	drop table #tblVung;
	drop table #temp1;
	drop table #temp2;
	drop table #tblCardCode;
	drop table #tmpEmployees;
	
END