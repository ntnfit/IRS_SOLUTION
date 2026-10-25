
CREATE PROCEDURE APZ_D_KPIs_CopyKpiSetting(in AbsEntry int, in  CopyEmployees VARCHAR(4000), in CopyAll CHAR(1), in UserUpdate INT)
AS
BEGIN
	declare kpiCode nvarchar(50) = '';
	declare viewType varchar(1) = '';
	declare period timestamp = '';
	declare maxAbsEntry int;
	declare currentDate timestamp = cast(current_date as date);
	declare currentTime int = EXTRACT(Hour FROM Current_date) * 100 + EXTRACT(minute FROM Current_date);
	declare checkUpdate int = 0;
	
	select max("AbsEntry") into maxAbsEntry from APZ_D_OKPM;
	
	if exists(select 1 from APZ_D_OKPM where "AbsEntry" = :AbsEntry)
	THEN
		select "Code", "ViewType", "Period" into kpiCode, viewType, period from APZ_D_OKPM where "AbsEntry" = :AbsEntry;
	END if;

	 CREATE local temporary TABLE #tblExisted ("empID" int);
	 insert into #tblExisted SELECT "empID" FROM APZ_D_OKPM WHERE "Code" = :kpiCode AND "ViewType" = :viewType AND "Period" = :period;

	create local temporary table #tblTemp("AbsEntry" int, "Code" nvarchar(50), "Name" nvarchar(254), "empID" int, "Period" nvarchar(50), "Remarks" nvarchar(254), "FromDate" timestamp, "ToDate" timestamp, "ViewType" char(1)
														, "StatusCode" char(1), "CreateDate" timestamp, "CreateTime" int, "UpdateDate" timestamp, "UpdateTime" int, "LogInstance" int, "UserSign" int, "UserSign2" int);
	 
	insert into #tblTemp
	 SELECT 0,"Code","Name", x.data,"Period","Remarks","FromDate","ToDate","ViewType","StatusCode",:currentDate, :currentTime, :currentDate, :currentTime, null,:UserUpdate, :UserUpdate
			FROM APZ_D_OKPM a ,(SELECT Data FROM Split(:CopyEmployees,',')) x
		 WHERE "AbsEntry"=:AbsEntry and x.Data not in (select "empID" from #tblExisted);

	select ifnull((select top 1 1 from #tblTemp where "AbsEntry" = 0), 0) into checkUpdate from DUMMY;
	while :checkUpdate = 1
	DO
	
		maxAbsEntry = :maxAbsEntry + 1;
		update top 1 #tblTemp set "AbsEntry" = :maxAbsEntry where "AbsEntry" = 0;
		select ifnull((select top 1 1 from #tblTemp where "AbsEntry" = 0), 0) into checkUpdate from DUMMY;
	
	END WHILE;
	
	INSERT INTO APZ_D_OKPM
		("AbsEntry", "Code", "Name", "empID", "Period", "Remarks", "FromDate", "ToDate", "ViewType", "StatusCode", "CreateDate", "CreateTime", "UpdateDate", "UpdateTime", "LogInstance", "UserSign", "UserSign2") 
			SELECT * FROM #tblTemp;

	create local temporary table #tblLine ("AbsEntry" int, "KPI_Code" nvarchar(50), "Condition_Code" nvarchar(50), "FromValue" numeric(19,6), "LogInstance" int, "Point" numeric(19,6), "ToValue" numeric(19,6));
	insert into #tblLine
		SELECT b."AbsEntry", a."KPI_Code", a."Condition_Code", a."FromValue", a."LogInstance", a."Point", a."ToValue" FROM APZ_D_KPM1 a, (SELECT "AbsEntry" FROM #tblTemp) AS b WHERE a."AbsEntry" = :AbsEntry;
	INSERT INTO APZ_D_KPM1("AbsEntry", "KPI_Code", "Condition_Code", "FromValue", "LogInstance", "Point", "ToValue") select * from #tblLine;

END




