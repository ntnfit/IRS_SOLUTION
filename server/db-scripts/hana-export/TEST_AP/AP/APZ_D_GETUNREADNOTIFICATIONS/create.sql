

CREATE PROCEDURE APZ_D_GetUnreadNotifications
(
	IN userName NVARCHAR(50),	
	IN appName NVARCHAR(50),	
	IN dbName NVARCHAR(50)
)
AS
BEGIN
	SELECT * FROM APZ_D_ELN1 WHERE "Method" = :appName  and "DatabaseName" = :dbName and (COMPARESPLIT(:userName, "SendUser", ',') = 1) and "Read" = 'N' and "IsRemove" = 'N';
END	


