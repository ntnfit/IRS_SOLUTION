

CREATE PROCEDURE APZ_D_GetEmployeeRoute(in empId int, dateValue timestamp default null)
AS
BEGIN

	declare empName nvarchar(254) = '';
	empName := CAST((select ifnull("firstName", '') || ' ' || ifnull("middleName", '') || ' ' || ifnull("lastName", '') from OHEM where "empID" = :empId) AS NVARCHAR);
	
	create local temporary table #tbl
	(
		"Geolocation" varchar(1000),
		"empId" int,
		"empName" nvarchar(254),
		"CardCode" varchar(10),
		"CardName" nvarchar(254),
		"CreateDate" timestamp,
		"CreateTS" int
	);
	
	insert into #tbl("Geolocation", "empId", "empName", "CardCode", "CardName", "CreateDate", "CreateTS")
	select ('(' || ifnull("U_Latitude", '') || ':' || ifnull("U_Longitude", '') || ')') as "Geolocation", :empId as "empID", :empName as "empName", "CardCode", "CardName", "CreateDate", "CreateTS" 
	from OCRD where "UserSign" = (select top 1 "userId" from OHEM where "empID" = :empId) and "CreateDate" = CAST(ifnull(:dateValue, CURRENT_DATE) as date)
	union all
	select b."Geolocation", :empId as "empID", :empName as "empName", "StoreCode" as "CardCode", "StoreName" as "CardName", b."CreateDate",
	cast(extract(hour from b."CreateTime") as varchar) + 
	cast(extract(minute from b."CreateTime") as varchar) + 
	cast(extract(second from b."CreateTime") as int) as "CreateTS"
	from APZ_D_VISIT a 
	inner join APZ_D_VISIT1 b on a."DocEntry" = b."DocEntry" and b."StepCode" = -1 where a."UserSign" = (select top 1 "userId" from OHEM where "empID" = :empId) 
	and CAST(b."CreateDate" as date) = CAST(ifnull(:dateValue, current_date) as date);

	select *, ROW_NUMBER() OVER (ORDER BY "CreateTS" asc) as "RowNum" from #tbl  order by "CreateTS" asc;

END




