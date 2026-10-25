
CREATE PROCEDURE APZ_Inventory_SerialNumberDetails_Search (
	IN 	itemCode NVARCHAR(50) DEFAULT ''
,		itemName NVARCHAR(100) DEFAULT ''
,		mnfSerial NVARCHAR(36) DEFAULT ''
,		serialNum NVARCHAR(36) DEFAULT ''
,		lotNum NVARCHAR(36) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	SELECT a.*, CASE WHEN b."CommitQty" = 1 THEN '2' ELSE CASE WHEN b."Quantity" = 1 THEN '0' ELSE '1' END END AS "Status1", b."WhsCode", b."Quantity", b."CommitQty" 
		FROM OSRN a 
		LEFT OUTER JOIN OSRQ b ON a."ItemCode" = b."ItemCode" AND a."SysNumber" = b."SysNumber" 
		WHERE ((LTRIM(RTRIM(IFNULL(:itemCode, ''))) = '') OR (a."ItemCode" LIKE '%' || :itemCode || '%')) 
			AND ((LTRIM(RTRIM(IFNULL(:itemName, ''))) = '') OR (a."itemName" LIKE '%' || :itemName || '%')) 
			AND ((LTRIM(RTRIM(IFNULL(:mnfSerial, ''))) = '') OR (a."MnfSerial" LIKE '%' || :mnfSerial || '%')) 
			AND ((LTRIM(RTRIM(IFNULL(:serialNum, ''))) = '') OR (a."DistNumber" LIKE '%' || :serialNum || '%')) 
			AND ((LTRIM(RTRIM(IFNULL(:lotNum, ''))) = '') OR (a."LotNumber" LIKE '%' || :lotNum || '%'));

END;




