

CREATE PROCEDURE APZ_D_Delivery_GetListDeliveryOrder
(
	IN userCode NVARCHAR(50) DEFAULT '',
	IN date DATETIME,
	IN status NVARCHAR(10) DEFAULT 'A'	
)
AS
BEGIN
	SELECT a.*, T1."FileName", IFNULL(T1."LinkThumbnails", IFNULL(T1."LinkDownload", (SELECT TOP 1 "DefaultAvatar" FROM APZ_D_OAPC WHERE "AppName" = 'i-DMS'))) AS "LinkThumbnails", 
	(select "ConfirmTime" from (SELECT "ConfirmTime", row_number() over(order by "ConfirmTime" desc) as row_id FROM APZ_D_ODLO WHERE "UserSign" = :userCode AND "CardCode" = a."CardCode" AND "Status" = 'Done') where row_id = 1) AS "LastDelivery" 
	FROM APZ_D_ODLO a LEFT JOIN APZ_D_UPIM T1 ON a."CardCode" = T1."CardCode" 
	WHERE a."UserSign" = :userCode AND CAST(a."ScheduledDate" AS date) = CAST(:date AS date) AND (:status = 'A' OR (:status <> 'A' AND "Status" = :status));
END




