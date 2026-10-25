

CREATE PROCEDURE APZ_D_Auth_GetAllAuth(in userName nvarchar(50))
AS
BEGIN

	declare rowCount int = 0;
	declare rowIndex int = 0;
	declare currentId int = -1;
	declare fatherId int = -1;

	create local temporary table #tbl
	(
		"Id" integer NULL, 
		"FatherId" integer NULL, 
		"Code" nvarchar(254) NULL, 
		"Name" nvarchar(254) NULL, 
		"LanguageKey" nvarchar(254) NULL, 
		"MenuType" char(1),
		USER_CODE nvarchar(100) NOT NULL, 
		"menucode" nvarchar(254) NULL, 
		"authoriza" char(1) NULL, 
		"isDefault" char(1) NULL,
		"MenuName" nvarchar(254) NULL
	);
	
	create local temporary table #tblMenu
	(
		"Id" integer NULL, 
		"FatherId" integer NULL, 
		"Code" nvarchar(254) NULL, 
		"Name" nvarchar(254) NULL, 
		"LanguageKey" nvarchar(254) NULL, 
		"MenuType" char(1),
		USER_CODE nvarchar(100) NULL, 
		"menucode" nvarchar(254) NULL, 
		"authoriza" char(1) NULL, 
		"isDefault" char(1) NULL,
		"MenuName" nvarchar(254) NULL
	);
	
	--Đệ quy
	insert into #tbl("Id", "FatherId", "Code", "Name", "LanguageKey", "MenuType", USER_CODE, "menucode", "authoriza", "isDefault", "MenuName")
	select *, t0."Name" from APZ_D_OMNI t0 LEFT JOIN APZ_D_OPRM T1 ON t0."Code" = t1."menucode" and t1.USER_CODE = :userName and t1."authoriza" = 'F'
    where t1.USER_CODE is not null or ifnull("MenuType", 'N') = 'N';

	rowCount := cast((select count(*) from #tbl) as int);
	while(:rowIndex <= :rowCount)
	do
	
		currentId := cast((select top 1 "Id" from #tbl) as int);
		fatherId := cast((select top 1 "FatherId" from #tbl) as int);
	
		insert into #tblMenu("Id", "FatherId", "Code", "Name", "LanguageKey", "MenuType", USER_CODE, "menucode", "authoriza", "isDefault", "MenuName")
		select t0."Id", t0."FatherId", t0."Code", t0."Name", t0."LanguageKey", t0."MenuType", t1.USER_CODE, t1."menucode", t1."authoriza", t1."isDefault", t0."Name"
		from APZ_D_OMNI t0 left join APZ_D_OPRM t1 on t0."Code" = t1."menucode" and t1.USER_CODE = :userName and t1."authoriza" = 'F'
		where  ifnull("MenuType", 'N') <> 'N' and "Id" = :currentId and "FatherId" = :fatherId;
	
		insert into #tblMenu("Id", "FatherId", "Code", "Name", "LanguageKey", "MenuType", USER_CODE, "menucode", "authoriza", "isDefault", "MenuName")
		select t0."Id", t0."FatherId", t0."Code", t0."Name", t0."LanguageKey", t0."MenuType", t1.USER_CODE, t1."menucode", t1."authoriza", t1."isDefault", t0."Name"
		from APZ_D_OMNI t0 left join APZ_D_OPRM t1 on t0."Code" = t1."menucode" and t1.USER_CODE = :userName and t1."authoriza" = 'F'
		where  ifnull("MenuType", 'N') <> 'N' and "FatherId" = :currentId;

		delete from #tbl where "Id" = :currentId;
		rowIndex := :rowIndex + 1;
		
	end while;
	--Kết thúc đệ quy
   	
   	select distinct t0."Id", t0."FatherId" as "FMenuName", t0."Name" as "MenuName", ifnull(t1."auth", 'N') as "authoriza", t0."Code" as "menucode", :userName as USER_CODE, t0."MenuType", null as "isDefault"
	from APZ_D_OMNI t0 left join 
	(
		select distinct t0."Id", t0."Code", t0."Name", t0."FatherId", 
		case when (select count(1) from #tblMenu where #tblMenu."FatherId" = t0."Id" and #tblMenu."authoriza" = 'N') > 0 then 'N' else T0."authoriza" end as "auth", null as "isDefault"
		FROM #tblMenu t0 
	) t1 on t0."Id" = t1."Id" order by t0."Id";
   	

END




