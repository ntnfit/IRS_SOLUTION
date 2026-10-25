
CALL APZ_EXISTSPROC('Apz_Auth_GetAllAuthForm');

GO

create PROCEDURE Apz_Auth_GetAllAuthForm(in userName NVARCHAR(50), in lngCode nvarchar(50), in superUser nvarchar(50))
AS
 BEGIN
	if :userName=:superUser then
		SELECT 	T0."Id",T0."FatherId",T0."Code",T0."Code" "menucode",'F' "authoriza",T0."Name","MenuType",T0."LanguageKey",'F',T0."Name" "MenuName"
		FROM APZ_OMNI T0
		WHERE T0."Code"='frm_Authorization' OR T0."Code"='frm_LicenseManager';
		return;
	end if;
	
	create local temporary table #tempAuth as
	(SELECT top 0 "Id","Code","Name","FatherId","menucode","authoriza","Name" "MenuName","MenuType","LanguageKey", cast(null as int) "TempId"
	FROM apz_omni a join oprm b on a."Code"=b."menucode");
	
	insert into #tempAuth
	select "Id","Code","Name","FatherId",ifnull("menucode","Code"),ifnull("authoriza",'N') "authoriza","Name","MenuType","LanguageKey",
	CASE when a."MenuType" in ('N','F','A') then a."Id" else IFNULL(NULLIF(a."FatherId",-1),a."Id") end
	from apz_omni a left join oprm b on a."Code"=b."menucode" and b.USER_CODE=:userName;
	
	UPDATE a set a."MenuName" = b."FunctionName", a."Name" = b."FunctionName" 
	from #tempAuth a join (select "ColumnId" "FunctionCode", "ColumnName" "FunctionName" from APZ_FORMSETTING 
			where "FormId" = N'oMain' and "ItemId" = 'oMain' and "UserSign" = '-' and "LngCode" = :lngCode) b
			ON a."LanguageKey" = b."FunctionCode";
												
	select * from #tempAuth order by "TempId", "FatherId", "Name";
	
	drop table #tempAuth;
 END;