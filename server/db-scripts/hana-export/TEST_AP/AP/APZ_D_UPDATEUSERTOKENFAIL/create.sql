

CREATE PROCEDURE APZ_D_UpdateUserTokenFail (IN Token nvarchar(500), IN AppName nvarchar(100))
AS
BEGIN
	UPDATE APZ_D_OADT SET "FailCount" = IFNULL("FailCount", 0) + 1 WHERE "DeviceToken" = :Token AND "AppCode" = :AppName AND "AppType" = 'A';
	DELETE FROM APZ_D_OADT WHERE "FailCount" > 5 AND "AppCode" = :AppName AND "AppType" = 'A';
END



