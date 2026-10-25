
CREATE PROCEDURE apz_itemmaster_exists_trans_itemwhs
(
ItemCode NVARCHAR(50),
WhsCode NVARCHAR(50))
as
BEGIN
		CREATE LOCAL TEMPORARY TABLE #tbl(
		ObjType VARCHAR(20)
	);

	INSERT INTO #tbl (ObjType) 
	(SELECT DISTINCT "TransType" FROM oilm WHERE "ItemCode" = :ItemCode AND "LocCode" = :WhsCode 
	UNION SELECT DISTINCT '1250000025' FROM OAT2 T0 INNER JOIN OAT1 T1 ON T1."AgrNo" = T0."AgrNo" AND T1."AgrLineNum" = t0."AgrLnNum" WHERE T0."WhsCode" = :WhsCode AND T1."ItemCode" = :ItemCode 
	UNION SELECT DISTINCT '202' FROM owor WHERE "ItemCode" = :ItemCode AND "Warehouse" = :WhsCode
	 UNION SELECT DISTINCT '202' FROM WOR1 WHERE "ItemCode" = :ItemCode AND "wareHouse" = :WhsCode 
	 UNION SELECT DISTINCT '23' FROM QUT1 WHERE "ItemCode" = :ItemCode AND "WhsCode" = :WhsCode 
	 UNION SELECT DISTINCT '540000006' FROM PQT1 WHERE "ItemCode" = :ItemCode AND "WhsCode" = :WhsCode 
	 UNION SELECT DISTINCT '112' FROM DRF1 WHERE "ItemCode" = :ItemCode AND ("WhsCode" = :WhsCode OR "FromWhsCod" = :WhsCode) 
	 UNION SELECT TOP 1 '1470000065' FROM INC1 WHERE "ItemCode" = :ItemCode AND "WhsCode" = :WhsCode);
SELECT * FROM #tbl;

END;



