
CREATE PROCEDURE APZ_Inventory_Serial_SerialTransaction_GetDocs (
	IN 	distNumber NVARCHAR(30) DEFAULT ''
,		ItemFrom NVARCHAR(50) DEFAULT ''
,		ItemTo NVARCHAR(50) DEFAULT ''
,		Status INT DEFAULT -1
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	DECLARE maxitem nvarchar(50);  
	DECLARE minitem nvarchar(50);
	SELECT MAX("ItemCode") INTO maxitem FROM OITM;
	SELECT MIN("ItemCode") INTO minitem FROM OITM;
	SELECT *, "CommitQty" AS "Allocated", "WhsCode" AS "LocCode" 
		FROM OSRQ a 
		INNER JOIN OSRN b ON a."MdAbsEntry" = b."AbsEntry" 
		WHERE (b."DistNumber" = :distNumber OR :distNumber = '') 
			AND (:Status = -1 OR b."Status" = :Status) 
			AND a."ItemCode" BETWEEN IFNULL(NULLIF(:ItemFrom, ''), :minitem) AND IFNULL(NULLIF(:ItemTo, ''), :maxitem);
			
			 
END;




