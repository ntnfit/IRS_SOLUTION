
CREATE PROCEDURE APZ_TimeSheeting_Export(
	in Branch INT,
	in Dept VARCHAR(254) default '', 
	in Emp VARCHAR(254) default '', 
	in EmpName NVARCHAR(254) default '', 
	in SStatus varchar(254) default '', 
	in Period VARCHAR(20)
	)
AS
BEGIN
	DECLARE err_message nvarchar(255);
	DECLARE ot VARCHAR(400000)='';
	DECLARE StartDate timestamp;
	DECLARE EndDate timestamp;
	declare dateIndex timestamp;
	DECLARE sqlQuery VARCHAR(40000)='';
	
	create local temporary table #temp_NhanVien ("empID" int ,"ExtEmpNo" nvarchar(50), "firstName" nvarchar(50), "lastName" nvarchar(50), "dept" int,"Department" nvarchar(254));
	
	insert into #temp_NhanVien
		SELECT a."empID",a."ExtEmpNo",a."firstName", a."lastName",a."dept",b."Name" "Department"
		  FROM OHEM a 
			INNER JOIN OUDP b ON a."dept"=b."Code"
			WHERE (a."branch"=:Branch)
				AND (cast(a."dept" as nvarchar) IN (SELECT Data From Split(:Dept,',')) OR :Dept='')
				AND (a."empID" =:Emp OR :Emp='' OR :Emp=0)
				AND (a."firstName" LIKE '%' || :EmpName || '%' OR a."lastName" LIKE '%' || :EmpName || '%' OR :EmpName='')
				AND (cast(a."status" as nvarchar) IN (SELECT Data From Split(:SStatus,',')) OR :SStatus='')
				AND a."Active"='Y';
	
		select string_agg('(select max("OvertimeHour") 
							from #dtTSH1 where "EmpId" = a."EmpId" and "OvertimeType" = a."OvertimeType" and "WorkDay" = a."WorkDay" and "WorkCode" = a."WorkCode") "'  || "Code" || '"', ', ') into ot 
		from APZ_OHOT where "Branch"=:Branch;
	
		select ifNULL((SELECT a."FromDate" FROM APZ_OHWP a WHERE a."PeriodCode"=:Period AND (a."Branch"=:Branch)),'1900-01-01') into StartDate from DUMMY;
		select ifNULL((SELECT a."ToDate" FROM APZ_OHWP a WHERE a."PeriodCode"=:Period  AND (a."Branch"=:Branch)),'1900-01-01') into EndDate from DUMMY;

		--if @daterange has value and ot has value
	IF :StartDate <= :EndDate
	THEN

		create local temporary table #tempDate ("ddate" timestamp);
		dateIndex = :startDate;
		while :dateIndex <= :enddate
		DO
			insert into #tempDate values (:dateIndex);
			dateIndex = ADD_DAYS(:dateIndex, 1);
		END while;
	
		create local temporary table #dtTSH1("Period" nvarchar(50), "EmpId" int, "ExtEmpNo" nvarchar(50), "EmpName" nvarchar(254), "WorkDay" DATE, "InTime" int, "OutTime" int, "FreeTxt" nvarchar(254)
											, "WorkCode" nvarchar(50), "OvertimeType" nvarchar(50), "OvertimeHour" numeric(19,6));
		insert into #dtTSH1
			select :Period "Period",b."empID",b."ExtEmpNo",b."lastName" || ', ' || b."firstName" as "EmpName",TO_DATE(TO_VARCHAR(t0."ddate", 'DD/MM/YYYY'), 'DD/MM/YYYY') AS "WorkDay",c."InTime",c."OutTime",'' "FreeTxt",
				IFNULL((select string_agg(d."WorkCode", ',') from APZ_TSL1 d where d."WorkDay"=c."WorkDay" AND  d."EmpId"=b."empID"), '') AS "WorkCode",
				(SELECT max("OvertimeType") from APZ_TSH1 aa where t0."ddate"=aa."WorkDay" and a."EmpId"=aa."EmpId" and a."LineNum"=aa."LineNum") "OvertimeType",
				a."OvertimeHour" "OvertimeHour" 
				from #temp_NhanVien b  LEFT JOIN #tempDate T0 ON  t0."ddate" IS NOT NULL 
				 LEFT JOIN APZ_TSH1 a ON a."EmpId"=b."empID" and a."Period"=:Period and a."WorkDay"=t0."ddate"
				 LEFT JOIN APZ_TSL1 c  ON c."EmpId"=b."empID" and c."Period"=:Period and c."WorkDay"=t0."ddate";

		--select * from #dtTSH1 
		--if have OT codes in OHOT, try to pivot columns
		if IFNULL(:ot,'')<>''
		THEN
			select distinct "Period", "EmpId", "ExtEmpNo", "EmpName", "WorkDay", "InTime", "OutTime", "FreeTxt", "WorkCode", "OvertimeType" from #dtTSH1;
			Return;
			sqlQuery = 'SELECT "Period", "EmpId", "ExtEmpNo", "EmpName", "WorkDay", "InTime", "OutTime", "FreeTxt", "WorkCode", ' || :ot ||' 
					FROM (select distinct "Period", "EmpId", "ExtEmpNo", "EmpName", "WorkDay", "InTime", "OutTime", "FreeTxt", "WorkCode", "OvertimeType" from #dtTSH1) a ORDER BY "EmpId","WorkDay";';
		--if there aren't any OT codes in OHOT, skip pivoting.
		ELSE
			Select 2 From Dummy;
			Return;
			sqlQuery = 'SELECT distinct "Period","EmpId","ExtEmpNo","EmpName","WorkDay","FreeTxt","WorkCode" FROM #dtTSH1 ORDER BY "EmpId","WorkDay";';
		END IF;
	
		EXECUTE immediate :sqlQuery;
	
		drop table #dtTSH1;
		drop table #tempDate;
	END IF;
	
	DROP TABLE #temp_NhanVien;
END


