
CALL APZ_EXISTSPROC('Apz_Auth_GetAllAuthForm');

GO

create PROCEDURE Apz_Auth_GetAllAuthForm(in userName NVARCHAR(50), in lngCode nvarchar(50), in superUser nvarchar(50))
AS
 BEGIN
	if :userName <> :superUser then
		declare tempAuth table
		(
			"Id" int,
			"Code" nvarchar(100),
			"Name" nvarchar(254),
			"FatherId" int,
			"menucode" nvarchar(100),
			"authoriza" nvarchar(2),
			"MenuName" nvarchar(254),
			"MenuType" char(1),
			"LanguageKey" nvarchar(100),
			"TempId" int
		);
		
		insert into :tempAuth
		select distinct "Id","Code","Name","FatherId",ifnull("menucode","Code"),ifnull("authoriza",'N') "authoriza","Name","MenuType","LanguageKey",
		CASE when a."MenuType" in ('N','F','A') then a."Id" else IFNULL(NULLIF(a."FatherId",-1),a."Id") end
		from apz_omni a left join oprm b on a."Code"=b."menucode" and b.USER_CODE=:userName;
		
		UPDATE a set a."MenuName" = b."ColumnName", a."Name" = b."ColumnName" 
		FROM :tempAuth a join APZ_FORMSETTING b
				on b."FormId" = 'oMain' and b."ItemId" = 'oMain' and b."ColumnId"=a."LanguageKey"
				and b."UserSign" = '-' and a."MenuType" in ('F','N','L') and b."LngCode" = :lngCode;
													
		select * from :tempAuth order by "TempId", "FatherId", "Name";
	else
		select 1 "Id", 'frm_Authorization' "Code", 'General Authorization' "Name", cast(null as int) "FatherId", 'frm_Authorization' "menucode", 'F' "authoriza", 'General Authorization' "MenuName", 'F' "MenuType", 'Authorization' "LanguageKey" from dummy
		union
		select 2, 'frm_LicenseManager', 'License Manager', null, 'frm_LicenseManager', 'F', 'License Manager', 'F', 'LicenseManager' from dummy
		union
		select 3, 'frm_FormSetting', 'Form Setting', null, 'frm_FormSetting', 'F', 'Form Setting', 'F', 'FormSetting' from dummy;
	end if;
 END;