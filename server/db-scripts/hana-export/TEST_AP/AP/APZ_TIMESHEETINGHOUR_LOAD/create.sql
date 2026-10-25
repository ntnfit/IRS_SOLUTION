
CREATE PROCEDURE APZ_TimeSheetingHour_Load(
	in Dept VARCHAR(254) default '',
	in Emp VARCHAR(254) default '',
	in EmpName NVARCHAR(254) default '',
	in SStatus varchar(254) default '',
	in Period VARCHAR(20),
	in Branch INT,
	in OvertimeType VARCHAR(20),
	in BaseEntry INT default 0)
AS
BEGIN
	DECLARE err_message nvarchar(254) = '';
	DECLARE StartDate timestamp;
	DECLARE EndDate timestamp;
	DECLARE daterage VARCHAR(400000)='';
	declare differenceDate int = 0;
	declare i int = 0;
	DECLARE sqlString VARCHAR(400000)='';
	
 	CREATE local temporary TABLE #temp_NhanVien AS (SELECT top 0 a."empID", a."ExtEmpNo", a."firstName", a."lastName", a."dept", b."Name" AS "Department", a."startDate", a."termDate" 
 													FROM OHEM a LEFT OUTER JOIN OUDP b ON a."dept" = b."Code");
	
	SELECT IFNULL((SELECT a."FromDate" FROM APZ_OHWP a WHERE a."PeriodCode"=:Period AND (a."Branch"=:Branch)),'1900-01-01') into StartDate FROM DUMMY;
	select IFNULL((SELECT a."ToDate" FROM APZ_OHWP a WHERE a."PeriodCode"=:Period  AND (a."Branch"=:Branch)),'1900-01-01') into EndDate FROM DUMMY;
	
	insert into #temp_NhanVien
		SELECT a."empID", a."ExtEmpNo", a."firstName", a."lastName", a."dept", b."Name" AS "Department", a."startDate", a."termDate" 
			FROM OHEM a LEFT OUTER JOIN OUDP b ON a."dept" = b."Code"
			WHERE ( a."branch"=:Branch)
			AND (cast(a."dept" as nvarchar) IN (SELECT Data From Split(:Dept,',')) OR :Dept='')
			AND (a."empID" =:Emp OR :Emp='' OR :Emp=0)
			AND (a."firstName" LIKE '%' || :EmpName || '%' OR a."lastName" LIKE '%' || :EmpName || '%' OR :EmpName='')
			AND (cast(a."status" as nvarchar) IN (SELECT Data From Split(:SStatus,',')) OR :SStatus='')
			AND a."Active"='Y';
	
	create local temporary table #DateData ("ddate" timestamp);
	-- a string for pivot
	select DAYS_BETWEEN(:StartDate,:EndDate) into differenceDate from dummy;
	if :differenceDate >= 0
	THEN
		daterage = '(select MAX("WorkCode") FROM #tbl where "EmpId" = a."EmpId" and "WorkDay" = ''' || TO_VARCHAR(:StartDate, 'yyyy-MM-dd') || ''') "' || TO_VARCHAR(:StartDate, 'yyyy.MM.dd') || '"';
		insert into #dateData values (:StartDate);
	
	END IF;
	
   	FOR i in 1..:differenceDate
   	DO
   
   		daterage = :daterage || ',(select MAX("WorkCode") FROM #tbl where "EmpId" = a."EmpId" and "WorkDay" = ''' || TO_VARCHAR(ADD_DAYS(:StartDate, :i), 'yyyy-MM-dd') || ''') "' || TO_VARCHAR(ADD_DAYS(:StartDate, :i), 'yyyy.MM.dd') || '"';
   		insert into #dateData values (ADD_DAYS(:StartDate, :i));
   
   	END FOR;
	IF IFNULL(:daterage,'')<>''
	THEN
		create local temporary table #tbl as 
			(SELECT TOP 0 a."empID" AS "EmpId", a."ExtEmpNo", a."lastName" AS "EmpName", a."Department", a."startDate", a."termDate"
				, cast(null as nvarchar(254)) AS "Period", cast(null as int) AS "Branch", b."TotalBefore"
				, b."DocStatus", b."Extra", b."WorkTotal", b."Comments", b."UserSign2", b."UpdateDate", cast(null as timestamp) "WorkDay", cast(null as numeric(19,6)) AS "WorkCode",b."BaseEntry" AS "BaseEntry" 
			FROM #temp_NhanVien a LEFT OUTER JOIN APZ_OTSH b ON b."EmpId" = a."empID");
		
		IF :BaseEntry > 0 
		THEN
			--Tổng hợp thông tin OT APZ_IOD2
			Create local temporary table #APZ_TSH1 as (Select Top 0 b."Period", b."EmpId", b."WorkDay",b."OvertimeType",b."OvertimeHour" From APZ_TSH1 b);
			Insert Into #APZ_TSH1 ("Period","EmpId","WorkDay","OvertimeType","OvertimeHour")
			Select  a."Period", a."empID" As "EmpId", a."WorkDay",a."OvertimeType",a."OvertimeHour"
			From APZ_IOD2 a Where IFNULL(a."OvertimeHour",0) != 0 And a."AbsEntry" = :BaseEntry;
			
			--Tổng hợp thông tin tổng giờ OT từ APZ_IOD2
			Create local temporary table #APZ_OTSH as(Select Top 0 * From APZ_OTSH b);
			Insert Into #APZ_OTSH ("Period","EmpId","OvertimeType","TotalBefore","WorkTotal")
			Select a."Period", a."empID" As "EmpId",a."OvertimeType", SUM(a."OvertimeHour") as "TotalBefore",SUM(a."OvertimeHour") as "WorkTotal" 
			From APZ_IOD2 a Where IFNULL(a."OvertimeHour",0) != 0 And a."AbsEntry" = :BaseEntry
			Group By a."Period", a."empID", a."OvertimeType";
			
			insert INTO #tbl SELECT a."empID" AS "EmpId", a."ExtEmpNo", a."lastName" || ', ' || a."firstName" AS "EmpName", a."Department", a."startDate", a."termDate",
					 :Period AS "Period", :Branch AS "Branch", 
					b."TotalBefore", b."DocStatus", b."Extra", b."WorkTotal", b."Comments", b."UserSign2", b."UpdateDate", t0."ddate" AS "WorkDay",
					(SELECT x."OvertimeHour" FROM (select aa."OvertimeHour", row_number() over(order by aa."OvertimeHour") rn 
												from #APZ_TSH1 aa 
												WHERE DAYS_BETWEEN(aa."WorkDay", c."WorkDay") = 0 AND aa."EmpId" = a."empID" AND aa."Period" = :Period AND aa."OvertimeType" = :OvertimeType) x 
											where rn = 1) AS "WorkCode", cast(:BaseEntry as nvarchar) As "BaseEntry"
			FROM #temp_NhanVien a LEFT OUTER JOIN #DateData T0 ON T0."ddate" IS NOT NULL LEFT OUTER JOIN #APZ_OTSH b ON b."EmpId" = a."empID" AND b."Period" = :Period AND b."OvertimeType" = :OvertimeType 
								  LEFT OUTER JOIN #APZ_TSH1 c ON b."Period" = c."Period" AND a."empID" = c."EmpId" AND c."WorkDay" = T0."ddate" AND c."OvertimeType" = :OvertimeType
			GROUP BY c."EmpId",a."ExtEmpNo",a."firstName",a."lastName",a."Department",a."startDate", a."termDate",
					b."TotalBefore",b."DocStatus",b."Extra",b."WorkTotal",b."Comments",c."WorkDay",a."empID",t0."ddate",b."UserSign2",b."UpdateDate";
		
			--select * from #tbl
			sqlString = N'SELECT distinct "EmpId","ExtEmpNo","EmpName","Department","Period","Branch","TotalBefore","DocStatus","Extra","WorkTotal","Comments","UserSign2","UpdateDate", "BaseEntry"'
					 || case when ifnull(:daterage,'') = '' then '' else ',' || :daterage end || '	FROM #tbl a;';
		ELSE
			insert INTO #tbl
				SELECT a."empID" AS "EmpId", a."ExtEmpNo", a."lastName" || ', ' || a."firstName" AS "EmpName", a."Department", a."startDate", a."termDate",
					:Period AS "Period", :Branch AS "Branch", b."TotalBefore", b."DocStatus", b."Extra", b."WorkTotal"
					, b."Comments", b."UserSign2", b."UpdateDate", t0."ddate" AS "WorkDay"
					, (SELECT x."OvertimeHour" FROM (select "OvertimeHour", row_number() over(order by "OvertimeHour") rn from APZ_TSH1 aa WHERE DAYS_BETWEEN(aa."WorkDay", c."WorkDay") = 0 AND aa."EmpId" = a."empID"
										 AND aa."Period" = :Period AND aa."OvertimeType" = :OvertimeType) x where rn = 1) AS "WorkCode" , b."BaseEntry" AS "BaseEntry"
				FROM #temp_NhanVien a LEFT OUTER JOIN #DateData T0 ON T0."ddate" IS NOT NULL LEFT OUTER JOIN APZ_OTSH b ON b."EmpId" = a."empID" AND b."Period" = :Period AND b."OvertimeType" = :OvertimeType 
									  LEFT OUTER JOIN APZ_TSH1 c ON b."Period" = c."Period" AND a."empID" = c."EmpId" AND c."WorkDay" = T0."ddate" AND c."OvertimeType" = :OvertimeType
				GROUP BY c."EmpId",a."ExtEmpNo",a."firstName",a."lastName",a."Department",a."startDate", a."termDate",
				b."TotalBefore",b."DocStatus",b."Extra",b."WorkTotal",b."Comments",c."WorkDay",a."empID",t0."ddate",b."UserSign2",b."UpdateDate",b."BaseEntry";
			
			--Cập nhật thông tin WorkCode cho nhân viên có startdate và termdate không nằm trong period
			Update a Set "WorkCode" = 0
			From #tbl a
			Where (a."startDate" is not null AND a."WorkDay" < a."startDate") Or (a."termDate" is not null AND a."WorkDay" > a."termDate");
			--Cập nhật lại WorkToTal, TotalBefore
			Update x Set "WorkTotal" = Ifnull(y."SumWC",0), "TotalBefore" = Ifnull(y."SumWC",0)
			From #tbl AS x JOIN (SELECT "EmpId", SUM("WorkCode") "SumWC" FROM #tbl GROUP BY "EmpId") y ON x."EmpId" = y."EmpId";
			
			--select * from #tbl
			sqlString = N'SELECT distinct "EmpId","ExtEmpNo","EmpName","Department","Period","Branch","TotalBefore","DocStatus","Extra","WorkTotal","Comments","UserSign2","UpdateDate", "BaseEntry"'
					 || case when ifnull(:daterage,'') = '' then '' else ',' || :daterage end || '	FROM #tbl a;';
		End IF;

		--PRINT @sql
		EXECUTE IMMEDIATE :sqlString;
	
		DROP TABLE #DateData;
		DROP TABLE #tbl;
	
	END IF;
		
	DROP TABLE #temp_NhanVien;
END




