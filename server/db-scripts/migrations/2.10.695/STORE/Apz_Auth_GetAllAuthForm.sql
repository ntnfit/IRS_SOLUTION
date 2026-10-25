
CALL APZ_EXISTSPROC('Apz_Auth_GetAllAuthForm');

GO

create PROCEDURE Apz_Auth_GetAllAuthForm(in userName NVARCHAR(50), in lngCode nvarchar(50), in superUser nvarchar(50))
AS
 BEGIN
	if :userName=:superUser then
		select 1 "Id", 'frm_Authorization' "Code", 'General Authorization' "Name", cast(null as int) "FatherId", 'frm_Authorization' "menucode", 'F' "authoriza", 'General Authorization' "MenuName", 'F' "MenuType", 'Authorization' "LanguageKey" from dummy
		union
		select 2, 'frm_LicenseManager', 'License Manager', null, 'frm_LicenseManager', 'F', 'License Manager', 'F', 'LicenseManager' from dummy
		union
		select 3, 'frm_FormSetting', 'Form Setting', null, 'frm_FormSetting', 'F', 'Form Setting', 'F', 'FormSetting' from dummy;
		
		return;
	end if;
	
	create local temporary table #tempAuth as
	(SELECT top 0 "Id","Code","Name","FatherId","menucode","authoriza","Name" "MenuName","MenuType","LanguageKey", cast(null as int) "TempId"
	FROM apz_omni a join oprm b on a."Code"=b."menucode");
	
	insert into #tempAuth
	select distinct "Id","Code","Name","FatherId",ifnull("menucode","Code"),ifnull("authoriza",'N') "authoriza","Name","MenuType","LanguageKey",
	CASE when a."MenuType" in ('N','F','A') then a."Id" else IFNULL(NULLIF(a."FatherId",-1),a."Id") end
	from apz_omni a left join oprm b on a."Code"=b."menucode" and b.USER_CODE=:userName;
	
	UPDATE a set a."MenuName" = b."FunctionName", a."Name" = b."FunctionName" 
	from #tempAuth a join (select "ColumnId" "FunctionCode", "ColumnName" "FunctionName" from APZ_FORMSETTING 
			where "FormId" = N'oMain' and "ItemId" = 'oMain' and "UserSign" = '-' and "LngCode" = :lngCode) b
			ON a."LanguageKey" = b."FunctionCode";
												
	select * from #tempAuth order by "TempId", "FatherId", "Name";
	
	drop table #tempAuth;
 END;