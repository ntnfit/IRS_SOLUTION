
CALL APZ_EXISTSPROC('APZ_D_OutletVisitingFrequency_GetData');

GO


CREATE PROCEDURE APZ_D_OutletVisitingFrequency_GetData(in userName nvarchar(50) default '', in branch int default -1)
as
BEGIN

	create local temporary table #tmpStore("CardCode" nvarchar(50), "CardName" nvarchar(254));
	call APZ_D_FilterStores('','','S','','#tmpStore');
	
	tbl = (select ifnull(a."Code", b."CardCode") as "Code", ifnull(a."Name", b."CardName") as "Name", ifnull(a."Address", b."Address") as "Address", a."Description", 
		a."StartDate", ifnull(a."Finished",'N') as "Finished", ifnull(a."Recurring", 'W') as "Recurring", a."RecurNum", a."RecurNote", ifnull(a."EveryDay",'N') as "EveryDay", ifnull(a."DayOfWeek", 'N') as "DayOfWeek",
		ifnull(a."Mon",'N') as "Mon", ifnull(a."Tue",'N') as "Tue", ifnull(a."Wed", 'N') as "Wed", ifnull(a."Thu",'N') as "Thu", ifnull(a."Fri",'N') as "Fri"
		, ifnull(a."Sat", 'N') as "Sat", ifnull(a."Sun", 'N') as "Sun", ifnull(a."DayOfMonth", 'N') as "DayOfMonth", ifnull(a."ListOfDate", '') as "ListOfDate",a."NumberOfVisits"
	from APZ_D_OOVF a 
	right join OCRD b on b."CardCode" = a."Code");
	
	select * from :tbl where "Code" in (select "CardCode" from #tmpStore);

END
