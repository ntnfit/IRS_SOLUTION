
Create Procedure APZ_WareHouse_DeleteWarehouse
(in whsCode nvarchar(50) default '')
As
Begin
Declare isExist int;
create local temporary table #tbl 
(
	"ObjType" VARCHAR(20)
);
	INSERT INTO #tbl 
	        ( "ObjType" )
SELECT DISTINCT "TransType" FROM OILM WHERE "LocCode" = :whsCode 
UNION SELECT TOP 1 '1250000025' FROM OAT2 T0 
INNER JOIN OAT1 T1 ON T1."AgrNo" = T0."AgrNo" AND T1."AgrLineNum" = t0."AgrLnNum" WHERE T0."WhsCode" = :whsCode 
UNION SELECT TOP 1 '202' FROM owor WHERE "Warehouse" = :whsCode 
UNION SELECT TOP 1 '202' FROM WOR1 WHERE "wareHouse" = :whsCode 
UNION SELECT TOP 1 '23' FROM QUT1 WHERE "WhsCode" = :whsCode 
UNION SELECT TOP 1 '540000006' FROM PQT1 WHERE "WhsCode" = :whsCode 
UNION SELECT TOP 1 '112' FROM DRF1 WHERE "WhsCode" = :whsCode OR "FromWhsCod" = :whsCode 
UNION SELECT TOP 1 '10000044' FROM OBTQ WHERE "WhsCode" = :whsCode 
UNION SELECT TOP 1 '10000045' FROM OSRQ WHERE "WhsCode" = :whsCode 
UNION SELECT TOP 1 '10000206' FROM OBIN WHERE "WhsCode" = :whsCode 
UNION SELECT TOP 1 '4' FROM oitw WHERE "WhsCode" = :whsCode AND "OnHand" > 0 
UNION SELECT TOP 1 '4' FROM oitm WHERE "DfltWH" = :whsCode 
UNION SELECT TOP 1 '700007' FROM OBPL WHERE "DflWhs" = :whsCode OR "DfltResWhs" = :whsCode 
UNION SELECT TOP 1 '1470000065' FROM INC1 WHERE "WhsCode" = :whsCode 
UNION SELECT TOP 1 '93' FROM OUDG WHERE "Warehouse" = :whsCode;
SELECT * FROM #tbl;


 Select count(1) into isExist from #tbl;
IF (isExist>0)
then
	Select 'Cannot remove warehouse because it has been used in transactions' ErrorMsg from dummy;
	Return;
END if;


DELETE from  OWHS Where "WhsCode" = :whsCode;
DELETE from  OITW Where "WhsCode" = :whsCode;
End;




