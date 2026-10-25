
-- apz_itemmaster_linked_trans 'zzzzzz'
CREATE PROCEDURE apz_itemmaster_linked_trans
(
ItemCode NVARCHAR(50)
)
as
BEGIN
		create local temporary TABLE #tbl (
		ObjType VARCHAR(20)
	);

	INSERT INTO #tbl
	        ( ObjType )
SELECT DISTINCT CAST("TransType" AS nvarchar(20)) FROM oilm WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 '1250000025' FROM OAT2 T0 INNER JOIN OAT1 T1 ON T1."AgrNo" = T0."AgrNo" AND T1."AgrLineNum" = t0."AgrLnNum" WHERE T1."ItemCode" = :ItemCode 
UNION SELECT TOP 1 '202' FROM owor WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 '202' FROM WOR1 WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 '66' FROM oitt WHERE "Code" = :ItemCode 
UNION SELECT TOP 1 '66' FROM itt1 WHERE "Code" = :ItemCode OR "Father" = :ItemCode 
UNION SELECT TOP 1 '8000006' FROM apz_oitt WHERE "Code" = :ItemCode 
UNION SELECT TOP 1 '8000006' FROM apz_itt1 WHERE "Code" = :ItemCode 
UNION SELECT TOP 1 '23' FROM QUT1 WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 '540000006' FROM PQT1 WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 '112' FROM DRF1 WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 '10000044' FROM OBTN WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 '10000045' FROM OSRN WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 '1470000065' FROM INC1 WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 '198' FROM fct1 WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 '8000005' FROM apz_fct1 WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 'LINE' FROM APZ_LIN1 WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 'IQA' FROM APZ_OIQA WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 'IQA' FROM APZ_IQA1 WHERE "ItemCode" = :ItemCode 
UNION SELECT TOP 1 '73' FROM OSCN WHERE "ItemCode" = :ItemCode;
	select * from #tbl;
END




