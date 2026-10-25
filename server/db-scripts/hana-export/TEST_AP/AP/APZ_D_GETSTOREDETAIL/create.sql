
CREATE PROCEDURE APZ_D_GetStoreDetail(in cardCode nvarchar(15), in userCode nvarchar(50))
AS
BEGIN

	declare userSign int = -1;
	declare currentDate timestamp = CURRENT_DATE;
	
	create local temporary table #tblBusinessType 
	(
		"CardCode" nvarchar(50),
		"CardName" nvarchar(254),
		"YesNo" char(1),
		"PropertiesName" nvarchar(4000),
		"PropertiesCode" nvarchar(100)
	);
	
	create local temporary table #tblCardCode
	(
		"CardCode" nvarchar(50),
	    "CardName" nvarchar(254), 
	    "QryGroup1" char(1), "QryGroup2" char(1), "QryGroup3" char(1), "QryGroup4" char(1), "QryGroup5" char(1), "QryGroup6" char(1), "QryGroup7" char(1), "QryGroup8" char(1)
	);
	
	insert into #tblCardCode("CardCode", "CardName", "QryGroup1", "QryGroup2", "QryGroup3", "QryGroup4", "QryGroup5", "QryGroup6", "QryGroup7", "QryGroup8")
	select "CardCode", "CardName", "QryGroup1", "QryGroup2", "QryGroup3", "QryGroup4", "QryGroup5", "QryGroup6", "QryGroup7", "QryGroup8"
	from OCRD where "U_SubType" = 'S' and "CardCode" = :cardCode;
	
	create local temporary table #tbl as 
	(
		select "CardCode", "CardName",
		MAP(element_number,
		    1, "QryGroup1",
		    2, "QryGroup2",
		    3, "QryGroup3",
		    4, "QryGroup4",
		    5, "QryGroup5",
		    6, "QryGroup6",
		    7, "QryGroup7",
		    8, "QryGroup8") as "YesNo",
		ROW_NUMBER() OVER(Partition by "CardCode" order by "CardCode" asc) as "OrderId"
		from #tblCardCode, SERIES_GENERATE_INTEGER(1, 1, 9) 
		order by element_number
	);
	
	create local temporary table #tblYes as (select * from #tbl where "YesNo" = 'Y');
	
	create local temporary table #ret as
	(	
		select distinct T0."CardCode", T0."CardName", T0."YesNo",
		(select STRING_AGG(t1."GroupName", ', ') from #tbl x inner join OCQG t1 on x."OrderId" = t1."GroupCode" where x."CardCode" = t0."CardCode" and "YesNo" = 'Y') as "Name",
		(select STRING_AGG(CAST(x."OrderId" as NVARCHAR), ', ')  from #tbl x  where x."CardCode" = t0."CardCode" and "YesNo" = 'Y') as "Code"
	 	from #tblYes t0   
 	);
 	
 	select (select top 1 USERID from OUSR where USER_CODE = :userCode) into userSign from dummy;
 	--ghi chú vì dùng STRING_AGG nên k có dấu , ở cuối nên k cần xóa kí tự cuối cùng giống bên sql
 	insert into #tblBusinessType select "CardCode", "CardName", "YesNo", "Name" as "PropertiesName",
 	"Code" as "PropertiesCode" 
 	from #ret;
	
	drop table #tbl;
	drop table #ret;
	drop table #tblYes;
	
	create local temporary table #tbl1
	(
		"CardCode" nvarchar(15) primary key,
		"CardName" nvarchar(100),
		"Address" nvarchar(100),
		"CardType" char(1),
		"GroupCode" smallint,
		"GroupName" nvarchar(100),
		"Currency" nvarchar(50),
		"E_Mail" nvarchar(100),
		"Cellular" nvarchar(50),
		-- bi loai bo trong ban moi {
		"OrderNumber" int,
		"VisitNumber" int,
		"OrderNotDelivered" int,
		"OrderNotPay" int,
		"Liabilities" numeric(19,6),
		"AccumulatedSales" numeric(19,6),
		-- }
		"U_Latitude" nvarchar(50),
		"U_Longitude" nvarchar(50),
		"PropertiesCode" nvarchar(100),
		"PropertiesName" nvarchar(254),
		"Facebook" nvarchar(254),
		"CntPrsName" nvarchar(50),
		"CntctCode" int,
		"ShipToAddress" nvarchar(500),
		"BillToAddress" nvarchar(500),
		"BillToDef" nvarchar(50),
		"ShipToDef" nvarchar(50),
		"SlpCode" int
	);
	
	insert into #tbl1("CardCode", "CardName", "Address", "CardType", "GroupCode", "GroupName", "Currency", "E_Mail", "Cellular", "OrderNumber", "VisitNumber", "OrderNotDelivered", "OrderNotPay", "Liabilities", "AccumulatedSales", "U_Latitude", "U_Longitude", "PropertiesCode", "PropertiesName", "Facebook"
	,"CntPrsName","CntctCode","ShipToAddress","BillToAddress","BillToDef","ShipToDef","SlpCode") 
	select a."CardCode", a."CardName", a."Address", a."CardType", a."GroupCode", b."GroupName", a."Currency", a."E_Mail", a."Cellular",
	-- number order (van order + sale order)
	IFNULL((select count(*) from ORDR where a."CardCode" = :cardCode and year("DocDate") = year(:currentDate) and month("DocDate") = month(:currentDate) and "U_GUID" is not null and "UserSign" = :userSign), 0) +
	IFNULL((select count(*) from OINV where a."CardCode" = :cardCode and year("DocDate") = year(:currentDate) and month("DocDate") = month(:currentDate) and "U_GUID" is not null and "UserSign" = :userSign), 0) as "OrderNumber",
	-- visit number
	(select count("Guid") from APZ_D_VISIT where "StoreCode" = :cardCode and year("FinishedDate") = year(:currentDate) and month("FinishedDate") = month(:currentDate)) as "VisitNumber", 0, 0, 0, 0,
	"U_Latitude", "U_Longitude", d."PropertiesCode", d."PropertiesName", a."IntrntSite"
	,E."Name" as "CntPrsName",E."CntctCode",IFNULL(F."Street",'') || ' ' || IFNULL(F."City",'') || ' ' ||IFNULL(F."Country",'') as "ShipToAddress",
	IFNULL(G."Street",'') || ' ' || IFNULL(G."City",'') || ' ' || IFNULL(G."Country",'') as "BillToAddress",A."BillToDef" ,A."ShipToDef", A."SlpCode"

	from OCRD a 
	left join OCRG b on a."GroupCode" = b."GroupCode"
	left join #tblBusinessType d on a."CardCode" = d."CardCode"
	 LEFT JOIN OCPR E ON A."CardCode" = E."CardCode" AND A."CntctPrsn" = E."Name"
	LEFT JOIN CRD1 F ON A."CardCode" = F."CardCode" AND A."ShipToDef" = F."Address" AND F."AdresType" = 'S'
	LEFT JOIN CRD1 G ON A."CardCode" = G."CardCode" AND A."BillToDef" = G."Address" AND G."AdresType" = 'B'
	where a."CardCode" = :cardCode;
	
	select * from #tbl1;
	
END




