
CREATE PROCEDURE APZ_LicenseManager_GetListUser
AS
BEGIN
	select LOWER(USER_CODE) as "User", 'N' "Use" from OUSR order by USER_CODE;
END




