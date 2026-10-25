
CREATE PROCEDURE APZ_D_GetAllDetailStore (IN saleUser nvarchar(50))
AS
BEGIN

	declare subType char(1) = 'S';
	declare cardName nvarchar(100) = '';
	declare cardType nvarchar(50) = '';
	declare fromDate date;
	declare toDate date;
	declare empID int;
	DECLARE userId INT;
	
	SELECT ADD_DAYS(current_date, -1) into fromDate FROM DUMMY;
	SELECT ADD_DAYS(current_date, 1) into toDate FROM DUMMY;
	empID := IFNULL((SELECT TOP 1 "salesPrson" FROM OHEM a LEFT OUTER JOIN OUSR b ON a."userId" = b.USERID WHERE b.USER_CODE = :saleUser), 0);
	userId := IFNULL((SELECT TOP 1 USERID FROM OUSR WHERE USER_CODE = :saleUser), 0);
	
	CREATE LOCAL TEMPORARY TABLE #tblBusinessType ("CardCode" nvarchar(50), "CardName" nvarchar(254), "YesNo" char(1), "PropertiesName" nvarchar(5000), "PropertiesCode" nvarchar(100));
	
	CREATE LOCAL TEMPORARY TABLE #tblCardCod ("CardCode" nvarchar(50), "CardName" nvarchar(254), "QryGroup1" char(1), "QryGroup2" char(1), "QryGroup3" char(1), "QryGroup4" char(1), "QryGroup5" char(1), "QryGroup6" char(1), "QryGroup7" char(1), "QryGroup8" char(1));
	
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
		(select string_agg(cast(x."OrderId" as nvarchar),',') from #tbl x  where x."CardCode"=t0."CardCode"   and "YesNo"='Y') as "Code"
		from #tblYes t0;
	 
	INSERT INTO #tblBusinessType SELECT "CardCode", "CardName", "YesNo", "Name" AS "PropertiesName","Code" AS "PropertiesCode" FROM #ret;
	drop table #tbl;
	drop table #ret;
	drop table #tblYes;
	
	create local temporary table #tmpStore ("CardCode" nvarchar(15), "CardName" nvarchar(100));
	CALL APZ_D_FilterStores('','','S',:saleUser,N'#tmpStore');
	
	CREATE LOCAL TEMPORARY TABLE #tblReturn
	(
		"CardCode" nvarchar(15) PRIMARY KEY, 
		 "CardName" nvarchar(100), 
		 "Address" nvarchar(100), 
		 "CardType" char(1), 
		 "GroupCode" smallint, 
		 "GroupName" nvarchar(100), 
		 "Currency" nvarchar(50), 
		 "E_Mail" nvarchar(100), 
		 "Cellular" nvarchar(50), 
		 "OrderNumber" int, 
		 "VisitNumber" int, 
		 "OrderNotDelivered" int, 
		 "OrderNotPay" int, 
		 "Liabilities" decimal(19, 6), 
		 "AccumulatedSales" decimal(19, 6),
		 "U_Latitude" nvarchar(50), 
		 "U_Longitude" nvarchar(50), 
		 "PropertiesCode" nvarchar(100), 
		 "PropertiesName" nvarchar(254), 
		 "Facebook" nvarchar(254),
		 "CntPrsName" nvarchar(50),
		"CntctCode" int,
		"ShipToAddress" nvarchar(500),
		"BillToAddress" nvarchar(500),
		"BillToDef" nvarchar(50),
		"ShipToDef" nvarchar(50),
		"SlpCode" int
	  );
		  
		INSERT INTO #tblReturn ("CardCode", "CardName", "Address", "CardType", "GroupCode", "GroupName", "Currency", "E_Mail", "Cellular", "OrderNumber", "VisitNumber", "OrderNotDelivered", "OrderNotPay", "Liabilities", "AccumulatedSales", "U_Latitude", "U_Longitude", "PropertiesCode", "PropertiesName", "Facebook"
		,"CntPrsName","CntctCode","ShipToAddress","BillToAddress","BillToDef","ShipToDef","SlpCode")  
		SELECT a."CardCode", a."CardName", A."Address", "CardType", a."GroupCode", b."GroupName", "Currency", "E_Mail", "Cellular", 0 AS "OrderNumber", 0 AS "VisitNumber", 0 AS "OrderNotDelivered", 0 AS "OrderNotPay", 0 AS "Liabilities", 0 AS "AccumulatedSales", "U_Latitude", "U_Longitude", d."PropertiesCode" AS "PropertiesCode", d."PropertiesName" AS "PropertiesName", a."IntrntSite" AS "Facebook" 
		,E."Name" as "CntPrsName",E."CntctCode",IFNULL(F."Street",'') ||' '|| IFNULL(F."City",'') ||' '||IFNULL(F."Country",'') as "ShipToAddress",IFNULL(G."Street",'') ||' '|| IFNULL(G."City",'') ||' '||IFNULL(G."Country",'') as "BillToAddress",A."BillToDef" ,A."ShipToDef", A."SlpCode"
		FROM OCRD A 
		LEFT OUTER JOIN OCRG B ON A."GroupCode" = B."GroupCode" LEFT OUTER JOIN #tblBusinessType D ON A."CardCode" = D."CardCode" 
		 LEFT JOIN OCPR E ON A."CardCode" = E."CardCode" AND A."CntctPrsn" = E."Name"
	LEFT JOIN CRD1 F ON A."CardCode" = F."CardCode" AND A."ShipToDef" = F."Address" AND F."AdresType" = 'S'
	LEFT JOIN CRD1 G ON A."CardCode" = G."CardCode" AND A."BillToDef" = G."Address" AND G."AdresType" = 'B'
		WHERE (IFNULL(:subType, '') = '' OR (IFNULL(:subType, '') <> '' AND "U_SubType" = :subType)) AND ((A."CardName" LIKE '%' || IFNULL(:cardName, '') || '%' AND IFNULL(:cardName, '') <> '') 
		OR IFNULL(:cardName, '') = '') AND (IFNULL(:cardType, '') = '' OR (IFNULL(:cardType, '') <> '' AND IFNULL(A."CardType", '') = :cardType)) AND (A."CardCode" IN (SELECT "CardCode" FROM #tmpStore)) 
		AND ((A."validFor" = 'Y' AND IFNULL(A."frozenFor", 'N') = 'N' AND (IFNULL(a."validFrom", :fromDate) <= current_date AND IFNULL(a."validTo", :toDate) >= current_date)) OR (IFNULL(A."validFor", 'N') = 'N' 
		AND A."frozenFor" = 'Y' AND (IFNULL(a."frozenFrom", :fromDate) > current_date OR IFNULL(a."frozenTo", :toDate) < current_date)) OR (A."validFor" = 'Y' AND A."frozenFor" = 'Y' 
		AND (IFNULL(a."validFrom", :fromDate) <= current_date AND IFNULL(a."validTo", :toDate) >= current_date) AND (IFNULL(a."frozenFrom", :fromDate) > current_date OR IFNULL(a."frozenTo", :toDate) < current_date)) 
		OR (A."validFor" = 'N' AND A."frozenFor" = 'N')) ORDER BY A."CreateDate" DESC, A."CreateTS" DESC;
		
		select * from #tblReturn;
		select * from apz_oclg where "CardCode" in (select "CardCode" from #tblReturn);
		SELECT * FROM OCPR where "CardCode" in (select "CardCode" from #tblReturn);

END




