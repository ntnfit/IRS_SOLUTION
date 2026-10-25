

CREATE PROCEDURE APZ_D_RPT_SalesCoverage
AS
BEGIN
	create local temporary table #DATA as 
	(
		select 
		"CardCode", 
		"Address", 
		"U_SubType", 
		"CardName", 
		"Cellular", 
		"U_Latitude", 
		"U_Longitude", 
		"CntctPrsn" 
		from OCRD where "U_SubType" = 'S'
	);
	
	create local temporary table #tblDKCTKM as (select * from (select *, row_number() over(PARTITION BY "CardCode" ORDER BY "CardCode", "CreateDate" DESC) as rn from APZ_D_PMO3) where rn = 1);

	select distinct T0."CardCode", "CardName", T0."Address", "Cellular", "CntctPrsn", "U_Latitude", "U_Longitude", 
	"LinkDownload", "LinkThumbnails", "WebUrl", t3."Tel1", T3."BirthDate" 
	,case when OCCURRENCES_REGEXPR(',' in T0."Address") > 1 then SUBSTR_REGEXPR('(.*),(.*),(.*)' in T0."Address" GROUP 2)
		  else SUBSTR_REGEXPR('(.*),(.*)' in T0."Address" GROUP 1)
	end as "TinhTP"
	,case when N."CreateDate" between add_years(current_date, -1) and current_date then 'red' else 'dodgerblue' end as "MarkerColor",
	t2."RouteCode", t2."RouteName"
	from #DATA T0
	LEFT JOIN APZ_D_STR2 t1
	inner join APZ_D_OSTR t2 on t1."RouteCode" = t2."RouteCode"
	on T0."CardCode" = t1."CardCode"
	LEFT JOIN OCPR T3 on T0."CardCode" = T3."CardCode" and T0."CntctPrsn" = T3."Name"
	INNER JOIN APZ_D_UPIM T4 ON T0."CardCode"=T4."CardCode" and T4."IsAvatar" = 'Y'
	LEFT JOIN #tblDKCTKM N on T0."CardCode" = N."CardCode"
	where T0."U_SubType" = 'S';

END




