
CREATE PROCEDURE APZ_Inventory_SerialNumberDetails_LoadAnEntry (
	IN 	AbsEntry INT DEFAULT 0
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	SELECT a.*, CASE WHEN b."CommitQty" = 1 THEN '2' ELSE CASE WHEN b."Quantity" = 1 THEN '0' ELSE '1' END END AS "Status1", b."WhsCode", b."Quantity", b."CommitQty" 
		FROM OSRN a 
		LEFT OUTER JOIN OSRQ b ON a."ItemCode" = b."ItemCode" AND a."SysNumber" = b."SysNumber" 
		WHERE a."AbsEntry" = :AbsEntry;

END;




