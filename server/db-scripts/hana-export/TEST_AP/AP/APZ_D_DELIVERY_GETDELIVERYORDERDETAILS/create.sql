

CREATE PROCEDURE APZ_D_Delivery_GetDeliveryOrderDetails (IN docEntry INT, IN cardCode NVARCHAR(50))
AS
BEGIN
	
	SELECT a.*, T1."FileName", IFNULL(T1."LinkThumbnails", IFNULL(T1."LinkDownload", (SELECT "DefaultAvatar" FROM APZ_D_OAPC WHERE "AppName" = 'i-DMS' LIMIT 1))) AS "LinkThumbnails" 
	FROM APZ_D_ODLO a LEFT JOIN APZ_D_UPIM T1 ON a."CardCode" = T1."CardCode" WHERE a."DocEntry" = :docEntry AND a."CardCode" = :cardCode;
	
	SELECT * FROM APZ_D_DLO1 WHERE "DocEntry" = :docEntry AND "CardCode" = :cardCode;

END



