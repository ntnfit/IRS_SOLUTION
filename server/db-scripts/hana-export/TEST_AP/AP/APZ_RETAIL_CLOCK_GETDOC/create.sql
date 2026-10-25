
-- APZ_Retail_CLOCK_GetDoc 'POS1',-1
CREATE PROCEDURE APZ_Retail_CLOCK_GetDoc
	(StoreCode NVARCHAR(50),
	PosID int)
AS
BEGIN
 declare Lineid integer;
select IFNULL((SELECT MAX("LineId") FROM APZ_CLOCK WHERE "StoreCode" = :StoreCode AND "PosID" = :PosID AND "ClockType" = 'I'), 0) into Lineid from DUMMY;
IF (:Lineid = 0) THEN SELECT 'N' AS "ClockIn", 'N' AS "ClockOut" FROM DUMMY;

ELSE SELECT 'Y' AS "ClockIn", CASE WHEN T1."LineId" IS NOT NULL THEN 'Y' ELSE 'N' END AS "ClockOut" FROM APZ_CLOCK T0 LEFT OUTER JOIN APZ_CLOCK T1 ON T0."StoreCode" = T1."StoreCode" AND T0."PosID" = T1."PosID" AND T0."LineId" < T1."LineId" WHERE T0."StoreCode" = :StoreCode AND T0."PosID" = :PosID AND T0."LineId" = :Lineid;
END IF;
 
 END
	




