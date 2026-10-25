

CREATE PROCEDURE APZ_D_GetUserToken (IN AppCode nvarchar(50))
AS
BEGIN
	SELECT * FROM APZ_D_OADT WHERE "AppCode" = :AppCode;
END




