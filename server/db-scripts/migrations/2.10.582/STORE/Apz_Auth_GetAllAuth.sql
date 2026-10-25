
CALL APZ_EXISTSPROC('Apz_Auth_GetAllAuth');

GO

CREATE PROCEDURE Apz_Auth_GetAllAuth(in userName NVARCHAR(50))
AS
 BEGIN
 	select "Id","FatherId" "FMenuName","Name" "MenuName",ifnull("authoriza",'N') "authoriza","Code" "menucode",:userName USER_CODE,"MenuType","isDefault"
	from apz_omni a left join oprm b on a."Code"=b."menucode" and b.USER_CODE=:userName;
 END;