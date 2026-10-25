CREATE PROCEDURE Apz_Auth_GetAllAuth(in userName NVARCHAR(50), lngCode nvarchar(10) default '')
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
 		"TempId" int,
 		"LngKey" nvarchar(50)
 	);
 	
 	insert into :tbl
	select "Id","FatherId","Name",ifnull("authoriza",'N'),"Code",:userName,"MenuType","isDefault",
	CASE when a."MenuType" in ('N','F','A') then a."Id" else IFNULL(NULLIF(a."FatherId",-1),a."Id") end,
	a."LanguageKey"
	from apz_omni a left join oprm b on a."Code"=b."menucode" and b.USER_CODE=:userName;
	
	if :userName not in ('HR0001','HR0002','HR0003','HR0004') then
		delete from :tbl where HR_GetRootMenuCode("menucode") = 'barSub_HRM';
	end if;
	
	if ifnull(:lngCode,'')<>'' then
		update a set "MenuName"=b."ColumnName"
		from :tbl a join apz_formsetting b
		on b."FormId"='oMain' and b."ItemId"='oMain' and b."ColumnId"=a."LngKey" and b."UserSign"='-' and a."MenuType" in ('F','N','L') and b."LngCode"=:lngCode;
	end if;
	
	update :tbl set "MenuName"=replace("MenuName",' && ',' & ');
	
	select * from :tbl order by "TempId";
 END;
