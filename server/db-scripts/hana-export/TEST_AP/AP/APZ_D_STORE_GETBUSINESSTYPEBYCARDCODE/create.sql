
CREATE  PROCEDURE APZ_D_Store_GetBusinessTypeByCardCode(in cardCode nvarchar(50) default '')
AS
BEGIN
	-- business type
	declare tblBusinessType table
	(
		"CardCode" nvarchar(50),
		"CardName" nvarchar(254),
		"YesNo" char(1),
		"PropertiesName" nvarchar(40000),
		"PropertiesCode" nvarchar(100)
	);
	
	declare tblCardCode table
	(
		"CardCode" nvarchar(50),
		"CardName" nvarchar(254), 
		"QryGroup1" char(1),"QryGroup2" char(1),"QryGroup3" char(1),"QryGroup4" char(1),"QryGroup5" char(1),"QryGroup6" char(1),"QryGroup7" char(1),"QryGroup8" char(1)
	);
	
	insert into :tblCardCode
		SELECT "CardCode", "CardName", "QryGroup1", "QryGroup2", "QryGroup3", "QryGroup4", "QryGroup5", "QryGroup6", "QryGroup7", "QryGroup8" 
					FROM OCRD WHERE "U_SubType" = 'S' AND "CardCode" = :cardCode;

	create local temporary table #tbl("CardCode" nvarchar(50), "CardName" nvarchar(254),"YesNo" char(1),"OrderId" int);
	
	insert into #tbl
		select "CardCode", "CardName","QryGroup1",1 from :tblCardCode
			UNION ALL
		select "CardCode", "CardName","QryGroup2",2 from :tblCardCode
			UNION ALL
		select "CardCode", "CardName","QryGroup3",3 from :tblCardCode
			UNION ALL
		select "CardCode", "CardName","QryGroup4",4 from :tblCardCode
			UNION ALL
		select "CardCode", "CardName","QryGroup5",5 from :tblCardCode
			UNION ALL
		select "CardCode", "CardName","QryGroup6",6 from :tblCardCode
			UNION ALL
		select "CardCode", "CardName","QryGroup7",7 from :tblCardCode
			UNION ALL
		select "CardCode", "CardName","QryGroup8",8 from :tblCardCode;

	create local temporary table #tblYes as (select * from #tbl where "YesNo"='Y');
	
	select distinct  T0."CardCode", T0."CardName",T0."YesNo",
		(select string_agg(t1."GroupName",', ')  from #tbl x  inner join OCQG t1 on x."OrderId"=t1."GroupCode" where x."CardCode"=t0."CardCode" and "YesNo"='Y') "PropertiesName",
		(select string_agg(cast(x."OrderId" as nvarchar(11)),', ') from #tbl x  where x."CardCode"=t0."CardCode" and "YesNo"='Y') "PropertiesCode" from #tblYes t0;
	 
	drop table #tbl;
	drop table #tblYes;
END



