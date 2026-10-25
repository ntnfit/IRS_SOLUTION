

CREATE PROCEDURE APZ_D_VisitSchedule_GetData(in userName nvarchar(50) default '', dateValue timestamp)
AS
BEGIN

	declare listOfDateSplt nvarchar(1000);
	declare dayOfDate varchar(3) = '';
	declare dateName varchar(10) = '';
	
	dayOfDate := cast((select extract(day from :dateValue) from dummy) as varchar(3));
	dateName := cast((select dayname(:dateValue) from dummy) as varchar(10));
	
	create local temporary table #tmpStore ("CardCode" nvarchar(50), "CardName" nvarchar(254));
	call APZ_D_FilterStores('', '', 'S', :userName, '#tmpStore');
	
	create local temporary table #tblSchedule as 
	(
		select ifnull(a."Code", b."CardCode") as "Code", ifnull(a."Name", b."CardName") as "Name", ifnull(a."Address", b."Address") as "Address", a."Description", a."StartDate", 
		ifnull(a."Finished", 'N') as "Finished", ifnull(a."Recurring", 'W') AS "Recurring", a."RecurNum", a."RecurNote", ifnull(a."EveryDay", 'N') as "EveryDay", 
		ifnull(a."DayOfWeek", 'N') as "DayOfWeek", IFNULL(a."Mon", 'N') as "Mon", ifnull(a."Tue", 'N') as "Tue", ifnull(a."Wed", 'N') as "Wed", ifnull(a."Thu", 'N') as "Thu", 
		ifnull(a."Fri", 'N') as "Fri", ifnull(a."Sat", 'N') as "Sat", ifnull(a."Sun", 'N') as "Sun", ifnull(a."DayOfMonth", 'N') as "DayOfMonth", ifnull(a."ListOfDate", '') as "ListOfDate", 
		b."U_Latitude", b."U_Longitude" 
		from APZ_D_OOVF a right outer join OCRD b on b."CardCode" = a."Code" 
		where a."Code" in (select "CardCode" from #tmpStore)
	);
	
	select STRING_AGG("ListOfDate", ',') into listOfDateSplt from #tblSchedule;
	
	select "Code" as "CardCode", "Name" as "CardName", "U_Latitude", "U_Longitude" 
	from #tblSchedule
	where "StartDate" < :dateValue and "Finished" = 'N' and 
	(
		"EveryDay" = 'Y'	
		or "DayOfMonth" = 'Y' and :dayOfDate in (select DATA from SPLIT(:listOfDateSplt, ','))
		or ("DayOfWeek" = 'Y' and 
		(case :dateName 
			when 'MONDAY' 	 then 'Mon'
			when 'TUESDAY' 	 then 'Tue'
			when 'WEDNESDAY' then 'Wed'
			when 'THURSDAY'  then 'Thu'
			when 'FRIDAY'	 then 'Fri'
			when 'SATURDAY'  then 'Sat'
			when 'SUNDAY' 	 then 'Sun' end) = 'Y')
	);
		
END



