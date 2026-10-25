
CREATE PROCEDURE APZ_SalaryCalculation_View(in empNo nvarchar(40000) default null, in branch int, in period nvarchar(50))
as
BEGIN

	DECLARE sqlString nvarchar(4000000)= '';
	DECLARE allowanceRange NVARCHAR(400000)='';
	DECLARE deductionRange NVARCHAR(400000)='';
	DECLARE OverTimeRange NVARCHAR(400000)='';
	DECLARE cols NVARCHAR(400000) = '';
	declare ohemQry nvarchar(400000) = '';
	DECLARE SalInfoUdfCols NVARCHAR(40000);

	create local temporary table #tbl as (select top 0 b.*, cast(null as nvarchar(254)) "DeptRemarks", cast(null as numeric(19,6)) "HourPerDay", cast(null as timestamp) "FromDate"
					, cast(null as timestamp) "ToDate", cast(null as char(1)) "IsLastPeriod" from OADM a left join APZ_OSAL b ON 1 = 1);
	insert into #tbl
		SELECT a.*, p."Remarks" AS "DeptRemarks", h."HourPerDay", h."FromDate", h."ToDate", h."IsLastPeriod" 
			FROM OHEM d INNER JOIN APZ_OSAL a ON a."empID" = d."empID" AND a."Period" = :period 
						LEFT JOIN OUDP p ON d."dept" = p."Code" 
						LEFT JOIN APZ_OHWP h ON h."PeriodCode" = :period AND h."Branch" = a."branch"		
			where  d."branch"=:branch and ((ifnull(:empNo,'') = '' or (:empNo <> '' and d."empID" in (select data from split(:empNo,',')))));
			
	insert into #tbl ("empID", "DeptRemarks", "HourPerDay","FromDate", "ToDate", "IsLastPeriod")
		SELECT d."empID", p."Remarks", h."HourPerDay", h."FromDate", h."ToDate", h."IsLastPeriod" 
			FROM OHEM d	LEFT JOIN OUDP p ON d."dept" = p."Code" 
						LEFT JOIN APZ_OHWP h ON h."PeriodCode" = :period AND h."Branch" = d."branch"		
			where  d."branch"=:branch and ((ifnull(:empNo,'') = '' or (:empNo <> '' and d."empID" in (select data from split(:empNo,','))))) 
					and not exists(select 1 from #tbl where "empID" = d."empID");
		
	update #tbl set "Period" = :period where ifnull("Period",'') ='';
	update #tbl set "branch" = :branch where ifnull("branch", 0) = 0;

	create local temporary table #tbl2 as (SELECT TOP 0 a."Code", a."Name", ifnull(b."CompRat", 0) AS "CompRat", ifnull(b."EmpRat", 0) AS "EmpRat", ifnull(b."EmpAmount", 0) AS "EmpAmount"
										, ifnull(b."CompAmount", 0) AS "CompAmount", b."empID", b."Period", b."CalFrom", b."CalTo" FROM APZ_SAL2 b LEFT OUTER JOIN APZ_OHDT a ON a."Code" = b."DeducCode");
	insert into #tbl2
		SELECT a."Code", a."Name", ifnull(b."CompRat", 0) AS "CompRat", ifnull(b."EmpRat", 0) AS "EmpRat", ifnull(b."EmpAmount", 0) AS "EmpAmount"
					, ifnull(b."CompAmount", 0) AS "CompAmount", b."empID", b."Period", b."CalFrom", b."CalTo" FROM APZ_SAL2 b LEFT OUTER JOIN APZ_OHDT a ON a."Code" = b."DeducCode" and b."Period" = :period;
							
	
	create local temporary table #tbl3 ("new_col" nvarchar(254), "value" numeric(19,6), "empID" int, "Period" nvarchar(50), "CalFrom" timestamp, "CalTo" timestamp);
	insert into #tbl3
		select b.Data || '_' || cast("Code" as NVARCHAR)
			,ifnull(case when b.data = 'EmpRat' then a."EmpRat" when b.data = 'EmpDeducAmt' then a."EmpAmount" when b.Data = 'CompRat' then a."CompRat" else a."CompAmount" end,0)
			, "empID", "Period","CalFrom","CalTo"
				from #tbl2 a inner join (select data from split('EmpRat,EmpDeducAmt,CompRat,CompDeducAmt', ',')) b on 1=1;
	 
	create local temporary table #tbl4 ("new_col1" nvarchar(254), "value1" numeric(19,6), "OTWorkTotal" numeric(19,6), "OTAmount" numeric(19,6), "OTRate" numeric(19,6), "empID" int, "Period" nvarchar(50), "CalFrom" timestamp, "CalTo" timestamp);
	insert into #tbl4
		select y.Data || '_' || cast("OTCode" as nvarchar)
			,ifnull(case when y.data = 'OTWTotal' then x."OTWorkTotal" when y.data = 'OTRate' then x."OTRate" else x."OTAmount" end,0),ifnull("OTWorkTotal",0),"OTAmount","OTRate","empID","Period","CalFrom","CalTo"
				from (SELECT "OTWorkTotal", "OTAmount", CAST(g."Code" AS nvarchar) AS "OTCode", g."Name" AS "OTName", f."empID", f."Period", ifnull(f."OTRate", g."Rate") AS "OTRate", f."CalFrom", f."CalTo" 
							FROM APZ_OHOT g INNER JOIN APZ_SAL3 f ON g."Code" = f."OTCode") x inner join (select data from split('OTWTotal,OTRate,OTAmt', ',')) y on 1=1;

	create local temporary table #tbl5 ("new_col2" nvarchar(254), "value2" numeric(19,6), "empID" int, "Period" nvarchar(50), "CalFrom" timestamp, "CalTo" timestamp);
	insert into #tbl5 
		select 'AllowQta_' || cast("Code" as nvarchar),ifnull(x."QuotaAmt",0), "empID", "Period","CalFrom","CalTo"
			from (select "QuotaAmt",e."Code",e."Name",d."empID" ,d."Period",d."CalFrom",d."CalTo" from APZ_SAL1 d JOIN APZ_OHAT e ON d."AllowanceType" = e."Code") as x;

	-- AllowanceType arrays for pivot
	select ifnull((select string_agg('(select MAX("value2") from #tbl5 where "empID" = a."empID" and "new_col2" = n''' || cast("new_col2" as nvarchar) || ''') "' || cast("new_col2" as nvarchar) || '"',',')
		from (select 'AllowQta_' || cast("Code" as nvarchar) "new_col2" from (select e."Code",e."Name" from APZ_OHAT e) x)),'') into allowanceRange from DUMMY;

	---- OverTime arrays for pivot
	select ifnull((select string_agg('(select MAX("value") from #tbl3 where "empID" = a."empID" and "new_col" = n''' || cast("new_col" as nvarchar) || ''') "' || cast("new_col" as nvarchar) || '"',',')  from (select y.Data || '_' || cast("Code" as nvarchar) "new_col"
			from (select a."Code",a."Name" from APZ_OHDT a) x inner join (select data from split('EmpRat,EmpDeducAmt,CompRat,CompDeducAmt', ',')) y on 1=1)),'') into deductionRange from DUMMY;
	
	-- Deduction arrays for pivot
	 --SET @OverTimeRange =(SELECT  '[OT_'+CONVERT(NVARCHAR,code) +'_'+CONVERT(nvarchar,Name)+'],'  from APZ_OHOT where branch = @branch FOR XML PATH(''))
	select ifnull((select string_agg('(select max("value1") from #tbl4 where "empID" = a."empID" and "new_col1" = n''' || cast("new_col1" as nvarchar) ||  ''') "' || cast("new_col1" as nvarchar) || '"',',')  from (select  y.Data || '_' || cast("OTCode" as nvarchar) "new_col1"
			from (select  cast(g."Code" as nvarchar) as "OTCode",cast(g."Name" as nvarchar) as "OTName", g."Rate" as "OTRate"
		from APZ_OHOT g where g."Branch" = :branch) as x inner join (select data from split('OTWTotal,OTRate,OTAmt', ',')) y on 1=1)),'') into OverTimeRange from DUMMY;

	select ifnull((SELECT string_agg('a."' || COLUMN_NAME || '"',',')
                   from TABLE_COLUMNS where SCHEMA_NAME = current_schema and TABLE_NAME = 'OHEM' AND COLUMN_NAME not in ('empID','branch','CreateDate','UpdateDate','UserSign','UserSign2','LogInstanc')),'') into cols from DUMMY;
	--PIVOT DATA TO SHOW
	--OVER TIME
	create local temporary table #tblOHEM as (select top 0 * from OHEM);
	exec 'drop table #tblOHEM';
	
	create local temporary table #tempStructure as (select distinct COLUMN_NAME,DATA_TYPE_NAME, "LENGTH", "POSITION" from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = 'OHEM' order by "POSITION");
	select ifnull((select string_agg('"' || COLUMN_NAME || '" ' || 
					case when DATA_TYPE_NAME = N'NCLOB' or DATA_TYPE_NAME = N'CLOB' or COLUMN_NAME in ('remark', 'attachment','WorkBuild','HomeBuild') then n'nvarchar(5000)'
					when DATA_TYPE_NAME = 'INTEGER' or DATA_TYPE_NAME = 'SMALLINT' or DATA_TYPE_NAME = 'TIMESTAMP' or DATA_TYPE_NAME = 'DATE' or DATA_TYPE_NAME = 'BIGINT' or DATA_TYPE_NAME = 'TINYINT' then DATA_TYPE_NAME
					when DATA_TYPE_NAME = 'DECIMAL' then n'numeric(19,6)'
					else DATA_TYPE_NAME || '(' || "LENGTH" || ')' end, ',') from #tempStructure),'') into ohemQry from DUMMY;
	exec 'create local temporary table #tblOHEM (' || :ohemQry || ')';
	
	insert into #tblOHEM select * from OHEM;
	
	select ifnull((select string_agg('a."' || COLUMN_NAME || '"', ',') from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = 'APZ_OSAL'),'') into ohemQry from DUMMY;
	
	SELECT string_agg('f."' || COLUMN_NAME || '"', ',') into SalInfoUdfCols
           from TABLE_COLUMNS
				where TABLE_NAME = 'APZ_OHSI' AND COLUMN_NAME like 'U__%' ESCAPE '_'  and schema_name = current_schema;
	
	sqlString = 'create local temporary table #tblResult as (SELECT distinct ' || :ohemQry || n', "DeptRemarks", "HourPerDay","FromDate", "ToDate", "IsLastPeriod"'
				|| (case when ifnull(:overtimeRange,'') = '' then '' else ',' || :overtimeRange end) || (case when ifnull(:deductionRange,'') = '' then '' else ',' || :deductionRange end)
				|| (case when ifnull(:allowanceRange,'') = '' then '' else ',' || :allowanceRange end) 
				|| (case when ifnull(:SalInfoUdfCols,'') = '' then '' else ',' || :SalInfoUdfCols end) 
				|| ' from #tbl a left join #tbl4 b on a."empID" = b."empID" and a."Period" = b."Period"  and a."CalFrom" = b."CalFrom" and a."CalTo" = b."CalTo"
								left join #tbl3 c on a."empID" = c."empID" and a."Period" = c."Period"  and a."CalFrom" = c."CalFrom" and a."CalTo" = c."CalTo"
								left join #tbl5 d on a."empID" = d."empID" and a."Period" = d."Period"  and a."CalFrom" = d."CalFrom" and a."CalTo" = d."CalTo"
								LEFT JOIN APZ_OHSI f ON a."empID" = f."EmpId" AND ((f."ValidFrom" <= a."FromDate" AND IFNULL(f."ValidTo", a."ToDate") >= a."ToDate") OR (f."ValidFrom" BETWEEN a."FromDate" AND a."ToDate")));';
	
	exec :sqlString;
	    
	sqlString = N'SELECT Ifnull(a."lastName",'''') || 
							Case When Ifnull(a."middleName",'''') != '''' Then '' '' ||  Ifnull(a."middleName",'''') Else '''' End ||
							Case When Ifnull(a."firstName",'''') != '''' Then '', '' ||  Ifnull(a."firstName",'''') Else '''' End as "empName",b.*,' || :cols || n' 
		from #tblResult b left join #tblOHEM a on a."empID" = b."empID" order by "empID" ASC;';
	
	EXECUTE immediate :sqlString;
END




