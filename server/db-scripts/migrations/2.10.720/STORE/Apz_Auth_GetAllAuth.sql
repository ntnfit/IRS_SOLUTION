
CALL APZ_EXISTSPROC('Apz_Auth_GetAllAuth');

GO

CREATE PROCEDURE Apz_Auth_GetAllAuth(in userName NVARCHAR(50))
AS
 BEGIN
 	declare tbl table
 	(
 		"Id" int,
 		"FMenuName" int,
 		"MenuName" nvarchar(254),
 		"authoriza" char(1),
 		"menucode" nvarchar(254),
 		USER_CODE nvarchar(25),
 		"MenuType" char(1),
 		"isDefault" char(1),
 		"TempId" int
 	);
 	
 	insert into :tbl
	select "Id","FatherId","Name",ifnull("authoriza",'N'),"Code",:userName,"MenuType","isDefault",CASE when a."MenuType" in ('N','F','A') then a."Id" else IFNULL(NULLIF(a."FatherId",-1),a."Id") end
	from apz_omni a left join oprm b on a."Code"=b."menucode" and b.USER_CODE=:userName;
	
	select * from :tbl order by "TempId";
 END;