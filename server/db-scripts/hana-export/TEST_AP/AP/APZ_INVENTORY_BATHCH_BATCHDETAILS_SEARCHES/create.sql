
CREATE PROCEDURE APZ_Inventory_Bathch_BatchDetails_Searches (
	IN 	itemCode NVARCHAR(50) DEFAULT ''
,		itemName NVARCHAR(100) DEFAULT ''
,		distNumber NVARCHAR(36) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	SELECT * 
		FROM OBTN 
		WHERE ((LTRIM(RTRIM(IFNULL(:itemCode, ''))) = '') OR ("ItemCode" LIKE '%' || :itemCode || '%')) 
			AND ((LTRIM(RTRIM(IFNULL(:itemName, ''))) = '') OR ("itemName" LIKE '%' || :itemName || '%')) 
			AND ((LTRIM(RTRIM(IFNULL(:distNumber, ''))) = '') OR ("DistNumber" LIKE '%' || :distNumber || '%'));
END;




