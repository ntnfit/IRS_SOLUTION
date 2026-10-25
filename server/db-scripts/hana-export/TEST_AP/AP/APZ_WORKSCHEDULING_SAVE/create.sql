
CREATE  PROCEDURE APZ_WorkScheduling_Save(in typeSave char(1))
AS
BEGIN
	
	declare checkCount int = 0;
	Declare branch int = 0;
	declare id int = 0;
	
	create local temporary table #tempDocData as (select top 0 * from APZ_OWSH);
	exec 'drop table #tempDocData;';
	exec 'create local temporary table #tempDocData as (select * from "#TempDoc");';
	
	create local temporary table #tempLineData as (select top 0 * from APZ_WSH1);
	exec 'drop table #tempLineData;';
	exec 'create local temporary table #tempLineData as (select * from "#TempLine");';
	
	select IFNUll((Select "BplID" from #TempDocData), -1) into branch from DUMMY;
	
	select IFNULL((SELECT count(a."Shift") FROM #TempLineData a 
	LEFT JOIN (select "Code" from apz_OSHP where IFNULL("BplId",-1) = :branch ) b ON a."Shift" = b."Code" 
	WHERE IFNULL(a."Shift", '') <> '' and IFNULL(b."Code", '') = '' ), 0) 
	into checkCount from DUMMY;
	
	IF :checkCount = 1
	THEN

		select 110002, 'Shift does not belong branch' from DUMMY;
		return;

	END IF;

	if :typeSave = 'A'
	THEN

		select ifnull((select max("Id") from APZ_OWSH), 0) + 1 into id from DUMMY;
		update #TempDocData set "Id" = :id;
		update #TempLineData set "ID" = :id;

	else

		Delete from APZ_WSH1 a where exists(select 1 from #TempDocData b where a."ID" = b."Id");
		Delete from APZ_OWSH a where exists(select 1 from #TempDocData b where a."Id" = b."Id");

	end IF;

	select IFNULL((select top 1 1 from #TempLineData a inner join apz_WSH1 b on a."WorkDate" = b."WorkDate"
													inner join APZ_OWSH c on b."ID" = c."Id" and ifnull(c."BplID",-1) = ifnull((select top 1 "BplID" from #TempDocData),-1)), 0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN

		select 110003, 'There have been working schedule for workday' from DUMMY;

	else

		insert into APZ_OWSH("Id", "Name", "FromDate", "ToDate", "Notes", "Year", "BplID") select * from #TempDocData;
		insert into APZ_WSH1("WorkDate", "ID", "Shift", "Remark", "LinID") select * from #TempLineData;

		select 0,'' from DUMMY;

	end IF;

	Drop table #TempDocdata;
	Drop table #TempLinedata;
END




