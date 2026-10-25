
CREATE  procedure SP_APZ_BOM_GetChildItemsInBom(in Father nvarchar(50))
AS
BEGIN

	declare maxId int = 0;
	declare i int = 0;
	declare tempItemCode nvarchar(50) = '';
	declare tempRn nvarchar(50) = '';
	
	create local temporary table #cte as (SELECT TOP 0 IFNULL(a."Code", '') AS "ItemCode", IFNULL(a."Quantity", 0) AS "OnHand", IFNULL(a."ChildNum", 0) AS "ChildNum", a."Project", a."Warehouse", a."OcrCode"
				, a."OcrCode2", a."OcrCode3", a."OcrCode4", a."OcrCode5", a."Comment", CAST(ROW_NUMBER() OVER ( ORDER BY a."ChildNum") AS nvarchar(10)) AS "rn", cast(null as int) TempId FROM itt1 a);
	
	insert into #cte 
		SELECT IFNULL(a."Code", '') AS "ItemCode", IFNULL(a."Quantity", 0) AS "OnHand", IFNULL(a."ChildNum", 0) AS "ChildNum", a."Project", a."Warehouse", a."OcrCode", a."OcrCode2", a."OcrCode3", a."OcrCode4"
										, a."OcrCode5", a."Comment", CAST(ROW_NUMBER() OVER ( ORDER BY a."ChildNum") AS nvarchar(10)) AS "rn", ROW_NUMBER() OVER ( ORDER BY a."ChildNum") FROM itt1 a where a."Father"=:Father;
										
	select ifnull((select max(TempId) from #cte), -1) into maxId from DUMMY;
	
	WHILE :i <= :maxId
	DO
	
		select ifnull((select "ItemCode" from #cte where tempId = :i), '') into tempItemCode from DUMMY;
		select ifnull((select "rn" from #cte where tempId = :i), '') into tempRn from DUMMY;
		insert into #cte 
			SELECT IFNULL(a."Code", '') AS "ItemCode", IFNULL(a."Quantity", 0) AS "OnHand", IFNULL(a."ChildNum", 0) AS "ChildNum", a."Project", a."Warehouse", a."OcrCode", a."OcrCode2", a."OcrCode3", a."OcrCode4"
				, a."OcrCode5", a."Comment", CAST(CAST(:tempRn AS nvarchar(10)) || CAST(a."ChildNum" AS nvarchar(10)) AS nvarchar(10)) AS "rn", :maxId + ROW_NUMBER() OVER ( ORDER BY a."ChildNum") FROM itt1 a
						inner join OITT c ON a."Father" = c."Code" where a."Father"= :tempItemCode and c."TreeType" = 'S';
		select ifnull((select max(TempId) from #cte), -1) into maxId from DUMMY;
		i = :i + 1;
	
	END WHILE;
	
	SELECT IFNULL(a."ItemCode", '') AS "ItemCode", IFNULL(b."ItemName", '') AS "ItemName", IFNULL(b."DfltWH", '') AS "DefaultWarehouse", IFNULL(b."VatGourpSa", '') AS "SalesVATGroup", IFNULL(c."Price", 0) AS "Price"
			, IFNULL(d."Rate", 0) AS "RateSa", IFNULL(c."Currency", '') AS "Currency", IFNULL(a."OnHand", 0) AS "OnHand", IFNULL(a."ChildNum", 0) AS "ChildNum", IFNULL(b."SalUnitMsr", '') AS "SalUnitMsr"
			, IFNULL(b."BuyUnitMsr", '') AS "BuyUnitMsr", b."NumInBuy", b."NumInSale", b."UgpEntry" AS "UgpCode", b."InvntryUom"
			, CASE IFNULL(e."BaseQty", 0) WHEN 0 THEN 0 ELSE IFNULL(e."AltQty", 0) / IFNULL(e."BaseQty", 0) END AS "NumInInven", a."Project", a."Warehouse", a."OcrCode", a."OcrCode2", a."OcrCode3", a."OcrCode4"
			, a."OcrCode5", a."Comment", b."SalPackUn", b."PurPackUn"
		FROM (SELECT DISTINCT * FROM #cte) AS a 
				LEFT OUTER JOIN oitm b ON a."ItemCode" = b."ItemCode" 
				LEFT OUTER JOIN itm1 c ON a."ItemCode" = c."ItemCode" AND c."PriceList" = -4 
				LEFT OUTER JOIN OVTG D ON B."VatGourpSa" = d."Code" 
				LEFT OUTER JOIN UGP1 e ON b."UgpEntry" = e."UgpEntry" AND e."UomEntry" IN (SELECT "UomEntry" FROM OUOM WHERE "UomName" = b."InvntryUom")
	WHERE (SELECT COUNT(1) FROM ITT1 T0 INNER JOIN OITT T1 ON T0."Father"=T1."Code" AND T1."TreeType"='S' WHERE "Father"=a."ItemCode")=0;
END




