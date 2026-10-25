
CREATE PROCEDURE APZ_AdvanceRouting_Save(in trantype char(1) default 'A')
AS
BEGIN	
	declare checkCount int = 0;
	declare efficDuplicate nvarchar(50) = '';
	declare code nvarchar(50) = '';
	declare maxDate timestamp;
	declare mindate timestamp;
	
	create local temporary table #tempDocData as (select top 0 * from APZ_ORTA);
	exec 'drop table #tempDocData;';
	exec 'create local temporary table #tempDocData as (select * from "#tempDoc");';
	
	create local temporary table #tempLineData as (select top 0 * from APZ_RTA1);
	exec 'drop table #tempLineData;';
	exec 'create local temporary table #tempLineData as (select * from "#tempLine");';
	
	create local temporary table #tempEfficData as (select top 0 * from APZ_RTA2);
	exec 'drop table #tempEfficData;';
	exec 'create local temporary table #tempEfficData as (select * from "#tempEffic");';
	
	select ifnull((select count(1) from #tempDocData),0) into checkCount from DUMMY;
	if :checkCount <> 1
	THEN
		select 1470014001, 'Data header is empty, or mutil line' FROM DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempDocData where ifnull("RoutCode",'') = ''),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014002, 'Route No is empty' FROM DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempDocData a INNER JOIN APZ_ORTA b ON a."RoutCode" = b."RoutCode"),0) into checkCount from DUMMY;
	if :trantype = 'A' and :checkCount = 1
	THEN
		select 1470014006, 'Route exists' from DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempDocData a INNER JOIN APZ_ORTA b ON a."RoutCode" = b."RoutCode"),0) into checkCount from DUMMY;
	if :trantype = 'U' and :checkCount = 0
	THEN
		select 1470014011, 'Route does not exists' FROM DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempDocData a LEFT JOIN OITT b ON a."ProdCode" = b."Code" where b."Code" is null),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014009, 'Header Production does not exists' from DUMMY;
		return;
	END IF;

	select ifnull((select TOP 1 1 from #tempDocData where ifnull("FromDate",'') <> '' and ifnull("ToDate",'') = '' and "FromDate" > "ToDate"),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014003, 'To date must be greater than From Date' FROM DUMMY;
		return;
	END IF;

	delete from #tempLineData where "WorkOrder" = 'N' and ifnull("OpCode",'') = '';
	delete from #tempLineData where "WorkOrder" = 'Y' and ifnull("ProdCode",'') = '';

	select ifnull((select top 1 1 from #tempLinedata),0) into checkCount from DUMMY;
	if :checkCount = 0
	THEN
		select 1470014004, 'Data line is empty' FROM DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempLineData where "WorkOrder" = 'N' and ifnull("OpCode",'') = ''),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014005, 'Operation Code is empty' FROM DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempLineData a LEFT JOIN APZ_OOMD b ON a."OpCode" = b."OpeCode" where a."WorkOrder" = 'N' and b."OpeCode" is null),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014007, 'Operation Code does not exists' FROM DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempLineData a LEFT JOIN ORSC b ON a."ResCode" = b."ResCode" where a."WorkOrder" = 'N' and b."ResCode" is null and ifnull(a."ResCode",'') <> ''),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014008, 'Resource Code does not exists' FROM DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempLineData a LEFT JOIN OITT b ON a."ProdCode" = b."Code" where b."Code" is null and ifnull(a."ProdCode",'') <> ''),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014010, 'Line Production does not exists' FROM DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempLineData where "WorkOrder" = 'N' and (ifnull("RunTime",0) = 0 or ifnull("InvQty",0) = 0)),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014016, 'Run time or quantity of Operation must be different 0' FROM DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempEfficData a LEFT JOIN ORSC b ON a."ResCode" = b."ResCode" where b."ResCode" is null and ifnull(a."ResCode",'') <> ''),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014011, 'Resource efficiency. Resource Code does not exists' FROM DUMMY;
		return;
	END IF;

	delete from #tempEfficData a where exists(select 1 from #tempLineData b where a."FatherLine" = b."LineNum" and b."WorkOrder" = 'Y');
	select ifnull((select top 1 "ResCode" from #tempEfficData group by "FatherLine", "ResCode" having count(1) > 1),'') into efficDuplicate from DUMMY;
	
	if :efficDuplicate <> ''
	THEN
		select 1470014012, 'Resource efficiency. Can not duplicate resouce "' || efficDuplicate || '"' FROM DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempLineData where "WorkOrder" = 'N' and ifnull("OpCode",'') = ''),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014013, 'Operation can not be empty when WorkOrder be No' FROM DUMMY;
		return;
	END IF;

	select ifnull((select top 1 1 from #tempLineData where "WorkOrder" = 'N' and ifnull("ResCode",'') = ''),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014014, 'Resource can not be empty when WorkOrder be No' FROM DUMMY;
		return;
	END IF;

	select ifnull((select TOP 1 1 from #tempLineData where "WorkOrder" = 'Y' and ifnull("SubRoutCode",'') = ''),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014016, 'Sub routing can not be empty when WorkOrder be Yes' FROM DUMMY;
		return;
	END IF;

	select ifnull((select 1 from #tempLineData a left join APZ_ORTA b On a."SubRoutCode" = b."RoutCode" where "WorkOrder" = 'Y' and b."RoutCode" is null),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014017, 'Sub routing does not exists' FROM DUMMY;
		return;
	END IF;

	select ifnull((select "RoutCode" from #tempDocData),'') into code from DUMMY;

	update #tempLineData set "RoutCode" = :code;
	update #tempLinedata Set "UomSetTime" = 'M' where "UomSetTime" is null;
	update #tempLinedata Set "UomRunTime" = 'M' where "UomRunTime" is null;
	update #tempLinedata set "InvQty" = "BaseQty" * "NumPerMsr";
	delete from #tempEfficData where ifnull("ResCode", '') = '';
	update #tempEfficData set "Effic" = 100 where ifnull("Effic", 0) = 0;

	create local temporary table #tempInsert as (SELECT "ResCode", "ResName", "RoutCode", "LineNum" FROM #tempLineData a WHERE "WorkOrder" = 'N' AND NOT EXISTS (SELECT 1 FROM #tempEfficData WHERE "FatherLine" = a."LineNum" AND "ResCode" = a."ResCode"));
	INSERT INTO #tempEfficData ("RoutCode", "ResCode", "FatherLine", "ResName", "Effic") (SELECT "RoutCode", "ResCode", "LineNum", "ResName", 100 FROM #tempInsert);
	
	if :trantype = 'A' THEN
		update #tempDocData set "CreateDate" = current_date;
	else
		update #tempDocData set "UpdateDate" = current_date;
	END IF;

	delete from APZ_RTA1 where "RoutCode" = :code;
	delete from APZ_ORTA where "RoutCode" = :code;
	delete from APZ_RTA2 where "RoutCode" = :code;

	select ADD_YEARS(ifnull((select max("ToDate") from APZ_ORTA), current_date), 100) into maxDate from DUMMY;
	select ADD_YEARS(ifnull((select min("FromDate") from APZ_ORTA), current_date), -100) into mindate from DUMMY;

	select ifnull((select top 1 1 from #tempDocData a INNER JOIN APZ_ORTA b ON a."ProdCode" = b."ProdCode" and (ifnull(a."FromDate", :mindate) between ifnull(b."FromDate", :mindate) and ifnull(b."ToDate", :maxDate)
																								or ifnull(a."ToDate", :maxDate) between ifnull(b."FromDate", :mindate) and ifnull(b."ToDate", :maxDate)
																								or ifnull(b."FromDate", :mindate) between ifnull(a."FromDate", :mindate) and ifnull(a."ToDate", :maxDate)
																								or ifnull(b."ToDate", :maxDate) between ifnull(a."FromDate", :mindate) and ifnull(a."ToDate", :maxDate))),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		select 1470014015, 'Date Ranges Overlap!' FROM DUMMY;
		return;
	END IF;

	insert into APZ_ORTA select * from #tempDocData;
	insert into APZ_RTA1 select * from #tempLineData;
	insert into APZ_RTA2 select * from #tempEfficData;

	select 0, '' FROM DUMMY;

END	




