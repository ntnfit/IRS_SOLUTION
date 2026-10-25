
Create Procedure apz_whs_linked_type
(in WhsCode nvarchar(50) default '')
As
Begin
create local temporary table #tbl 
(
	"ObjType" VARCHAR(20)
);
	INSERT INTO #tbl 
	        ( "ObjType" )
SELECT DISTINCT "TransType" FROM OILM WHERE "LocCode" = :WhsCode 
UNION SELECT TOP 1 '1250000025' FROM OAT2 T0 
INNER JOIN OAT1 T1 ON T1."AgrNo" = T0."AgrNo" AND T1."AgrLineNum" = t0."AgrLnNum" WHERE T0."WhsCode" = :WhsCode 
UNION SELECT TOP 1 '202' FROM owor WHERE "Warehouse" = :WhsCode 
UNION SELECT TOP 1 '202' FROM WOR1 WHERE "wareHouse" = :WhsCode 
UNION SELECT TOP 1 '23' FROM QUT1 WHERE "WhsCode" = :WhsCode 
UNION SELECT TOP 1 '540000006' FROM PQT1 WHERE "WhsCode" = :WhsCode 
UNION SELECT TOP 1 '112' FROM DRF1 WHERE "WhsCode" = :WhsCode OR "FromWhsCod" = :WhsCode 
UNION SELECT TOP 1 '10000044' FROM OBTQ WHERE "WhsCode" = :WhsCode 
UNION SELECT TOP 1 '10000045' FROM OSRQ WHERE "WhsCode" = :WhsCode 
UNION SELECT TOP 1 '10000206' FROM OBIN WHERE "WhsCode" = :WhsCode 
UNION SELECT TOP 1 '4' FROM oitw WHERE "WhsCode" = :WhsCode AND "OnHand" > 0 
UNION SELECT TOP 1 '4' FROM oitm WHERE "DfltWH" = :WhsCode 
UNION SELECT TOP 1 '700007' FROM OBPL WHERE "DflWhs" = :WhsCode OR "DfltResWhs" = :WhsCode 
UNION SELECT TOP 1 '1470000065' FROM INC1 WHERE "WhsCode" = :WhsCode 
UNION SELECT TOP 1 '93' FROM OUDG WHERE "Warehouse" = :WhsCode;
SELECT * FROM #tbl;
End;




