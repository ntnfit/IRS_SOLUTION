
CREATE PROCEDURE Util_AdvanceReportTemplete_SaveRep
AS
BEGIN

	declare checkCount int = 0;
	Declare repCode nvarchar(50);
	
	create local temporary table #APZ_ARTData as(select top 0 * from APZ_ART);
	exec 'insert into #APZ_ARTData select * from #APZ_ART;';
	
	select ifnull((select count(1) from #APZ_ARTData), 0) into checkCount from dummy;
	
	if :checkCount = 0
	THEN

		select 10001, 'Data report can not be null' from DUMMY;
		return;

	END IF;

	select ifnull((select top 1 1 from #APZ_ARTData where ifnull("RepCode",'') = ''), 0) into checkCount from dummy;

	if :checkCount > 0
	THEN

		select 10002, 'Report code can not be null' from DUMMY;
		return;

	END if;
	
	-- get report code from temp table
	select ifnull((select top 1 "RepCode" from #APZ_ARTData),'') into repCode from DUMMY;

	-- delete all from report, indicator, account table of this report code
	delete from APZ_ART where "RepCode" = :repCode;
	delete from APZ_ARTI where "RepCode" = :repCode;
	Delete from APZ_ARTA where "RepCode" = :repCode;
	
	create local temporary table #APZ_ARTIData as (select top 0 b.* from (select 1 "Cl" from DUMMY) a left join APZ_ARTI b ON 1 = 1);
	exec 'insert into #APZ_ARTIData select * from #APZ_ARTI;';
	
	create local temporary table #APZ_ARTAData as (select top 0 b.* from (select 1 "Cl" from DUMMY) a left join APZ_ARTA b ON 1 = 1);
	exec 'insert into #APZ_ARTAData select * from #APZ_ARTA;';

	delete from #APZ_ARTIData where ifnull("IndCode", '') = '' or ifnull("RepCode",'') = '';
	delete from #APZ_ARTAData where ifnull("Account", '') = '' or ifnull("RepCode", '') = '';

	delete from #APZ_ARTAData x where not exists(select 1 from #APZ_ARTIData where "IndCode" = x."IndCode");

	update #APZ_ARTIData set "RepCode" = :repCode;
	update #APZ_ARTAData set "RepCode" = :repCode;

	update #APZ_ARTIData set "RepCode" = :repCode;

	-- insert from temp table to report table
	insert into APZ_ART select * from #APZ_ARTData;

	-- insert from temp table to indicator table
	insert into APZ_ARTI  select * from #APZ_ARTIData;

	---- insert from temp table to account table
	insert into APZ_ARTA  select * from #APZ_ARTAData;
	
	select 0, '' from DUMMY;
END



