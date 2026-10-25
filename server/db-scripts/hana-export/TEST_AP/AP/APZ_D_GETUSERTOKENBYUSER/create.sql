

CREATE PROCEDURE APZ_D_GetUserTokenByUser (IN AppCode nvarchar(50), IN UserName nvarchar(50))
AS
BEGIN
	SELECT * FROM APZ_D_OADT WHERE "AppCode" = :AppCode AND "UserName" = :UserName;
END




