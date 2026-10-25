

CREATE PROCEDURE APZ_D_MarkAllNotiAsRead
(
	IN userName NVARCHAR(50),	
	IN appName NVARCHAR(50),	
	IN dbName NVARCHAR(50)
)
AS
BEGIN
	Update APZ_D_ELN1 set "Read" = 'Y' WHERE "Method" = :appName  and "DatabaseName" = :dbName and (COMPARESPLIT(:userName, "SendUser", ',') = 1) and "Read" = 'N';
END	

