CREATE PROCEDURE APZ_TimeSheetingHour_LoadCopyFrom(
	in BaseEntry INT,
	in OvertimeType varchar(20))
AS
BEGIN
	DECLARE err_message nvarchar(254) = '';
	DECLARE StartDate timestamp;
	DECLARE EndDate timestamp;
	DECLARE daterage VARCHAR(400000)='';
	declare differenceDate int = 0;
	declare i int = 0;
	DECLARE sqlString VARCHAR(400000)='';
	DECLARE Dept VARCHAR(254)='';
	DECLARE SStatus VARCHAR(254)='';
	DECLARE Branch INT = -1;
	DECLARE Period VARCHAR(20) = '';
	
	--Get Branch, Period, SStatus, Dept, OvertimeType by BaseEntry
	Select a."Branch" into Branch From APZ_OIOD a Where a."AbsEntry" = :BaseEntry;
	Select a."Period" Into Period From APZ_OIOD a Where a."AbsEntry" = :BaseEntry;
	--SStatus
	CREATE local temporary table #temp_status AS (SELECT Top 0 Distinct(a."status") As "SStatus" FROM OHEM a LEFT OUTER JOIN OUDP b ON a."dept" = b."Code");
	Insert into #temp_status SELECT Distinct(a."status") As "SStatus" FROM OHEM a LEFT OUTER JOIN OUDP b ON a."dept" = b."Code" WHERE a."branch" = :Branch;
	Select STRING_AGG(a."SStatus",',') Into SStatus From #temp_status a;
	--Dept
	CREATE local temporary table #temp_dept AS (SELECT Top 0 Distinct(a."dept") As "Dept" FROM OHEM a LEFT OUTER JOIN OUDP b ON a."dept" = b."Code");
	Insert into #temp_dept SELECT Distinct(a."dept") As "Dept" FROM OHEM a LEFT OUTER JOIN OUDP b ON a."dept" = b."Code" WHERE a."branch" = :Branch;
	Select STRING_AGG(a."Dept",',') Into Dept From #temp_dept a;
	Drop Table #temp_status;
	Drop Table #temp_dept;
	--OvertimeType
	--Select Top 1 a."OvertimeType" Into OvertimeType From APZ_IOD2 a Where a."AbsEntry" = :BaseEntry;
	
 	CREATE local temporary TABLE #temp_NhanVien AS (SELECT top 0 a."empID", a."ExtEmpNo", a."firstName", a."lastName", a."dept", b."Name" AS "Department" FROM OHEM a LEFT OUTER JOIN OUDP b ON a."dept" = b."Code");
	
	SELECT IFNULL((SELECT a."FromDate" FROM APZ_OHWP a WHERE a."PeriodCode"=:Period AND (a."Branch"=:Branch)),'1900-01-01') into StartDate FROM DUMMY;
	select IFNULL((SELECT a."ToDate" FROM APZ_OHWP a WHERE a."PeriodCode"=:Period  AND (a."Branch"=:Branch)),'1900-01-01') into EndDate FROM DUMMY;
	
	insert into #temp_NhanVien
		SELECT a."empID", a."ExtEmpNo", a."firstName", a."lastName", a."dept", b."Name" AS "Department" FROM OHEM a LEFT OUTER JOIN OUDP b ON a."dept" = b."Code"
			WHERE 
			( a."branch"=:Branch)
			AND (:Dept='' OR cast(a."dept" as nvarchar) IN (SELECT Data From Split(:Dept,',')))
			AND (:SStatus='' OR cast(a."status" as nvarchar) IN (SELECT Data From Split(:SStatus,',')))
			AND a."Active"='Y';
	
	create local temporary table #DateData ("ddate" timestamp);
	-- a string for pivot
	select DAYS_BETWEEN(:StartDate,:EndDate) into differenceDate from dummy;
	if :differenceDate >= 0
	THEN
		daterage = '(select MAX("WorkCode") FROM #tbl where "EmpId" = a."EmpId" and "WorkDay" = ''' || TO_VARCHAR(:StartDate, 'yyyy-MM-dd') || ''') "' || TO_VARCHAR(:StartDate, 'yyyy.MM.dd') || '"';
		insert into #DateData values (:StartDate);
	
	END IF;
	
   	FOR i in 1..:differenceDate
   	DO
   
   		daterage = :daterage || ',(select MAX("WorkCode") FROM #tbl where "EmpId" = a."EmpId" and "WorkDay" = ''' || TO_VARCHAR(ADD_DAYS(:StartDate, :i), 'yyyy-MM-dd') || ''') "' || TO_VARCHAR(ADD_DAYS(:StartDate, :i), 'yyyy.MM.dd') || '"';
   		insert into #dateData values (ADD_DAYS(:StartDate, :i));
   
   	END FOR;
   	
	IF IFNULL(:daterage,'')<>''
	THEN
		--Tổng hợp thông tin OT theo nhân viên từ APZ_IOD2
		Create local temporary table #APZ_TSH1 as (Select Top 0 b."Period", b."EmpId", b."WorkDay",b."OvertimeType",b."OvertimeHour" From APZ_TSH1 b);
		Insert Into #APZ_TSH1 ("Period","EmpId","WorkDay","OvertimeType","OvertimeHour")
		Select  a."Period", a."empID" As "EmpId", a."WorkDay",a."OvertimeType",a."OvertimeHour"
		From APZ_IOD2 a Where IFNULL(a."OvertimeHour",0) != 0 And a."AbsEntry" = :BaseEntry And "OvertimeType" = :OvertimeType;
	
		--Tổng hợp thông tin tổng giờ OT từ APZ_IOD2
		Create local temporary table #APZ_OTSH as(Select Top 0 * From APZ_OTSH b);
		Insert Into #APZ_OTSH ("Period","EmpId","OvertimeType","TotalBefore","WorkTotal")
		Select a."Period", a."empID" As "EmpId",a."OvertimeType", SUM(a."OvertimeHour") as "TotalBefore",SUM(a."OvertimeHour") as "WorkTotal" 
		From APZ_IOD2 a Where IFNULL(a."OvertimeHour",0) != 0 And a."AbsEntry" = :BaseEntry And "OvertimeType" = :OvertimeType
		Group By a."Period", a."empID", a."OvertimeType";
		
		create local temporary table #tbl as (
			SELECT TOP 0 a."empID" AS "EmpId", a."ExtEmpNo", a."lastName" AS "EmpName", a."Department", cast(null as nvarchar(254)) AS "Period", cast(null as int) AS "Branch", 
				b."TotalBefore", b."DocStatus", b."Extra", b."WorkTotal", b."Comments", b."UserSign2", b."UpdateDate", cast(null as timestamp) "WorkDay", cast(null as numeric(19,6)) AS "WorkCode" 
			FROM #temp_NhanVien a LEFT OUTER JOIN APZ_OTSH b ON b."EmpId" = a."empID");
			
		insert INTO #tbl SELECT a."empID" AS "EmpId", a."ExtEmpNo", a."lastName" || ', ' || a."firstName" AS "EmpName", a."Department", :Period AS "Period", :Branch AS "Branch", 
				b."TotalBefore", b."DocStatus", b."Extra", b."WorkTotal", b."Comments", b."UserSign2", b."UpdateDate", t0."ddate" AS "WorkDay",
				(SELECT x."OvertimeHour" FROM (select aa."OvertimeHour", row_number() over(order by aa."OvertimeHour") rn 
												from #APZ_TSH1 aa 
												WHERE DAYS_BETWEEN(aa."WorkDay", c."WorkDay") = 0 AND aa."EmpId" = a."empID" AND aa."Period" = :Period AND aa."OvertimeType" = :OvertimeType) x 
											where rn = 1) AS "WorkCode" 
		FROM #temp_NhanVien a LEFT OUTER JOIN #DateData T0 ON T0."ddate" IS NOT NULL LEFT OUTER JOIN #APZ_OTSH b ON b."EmpId" = a."empID" AND b."Period" = :Period AND b."OvertimeType" = :OvertimeType 
								  LEFT OUTER JOIN #APZ_TSH1 c ON b."Period" = c."Period" AND a."empID" = c."EmpId" AND c."WorkDay" = T0."ddate" AND c."OvertimeType" = :OvertimeType
		GROUP BY c."EmpId",a."ExtEmpNo",a."firstName",a."lastName",a."Department",b."TotalBefore",b."DocStatus",b."Extra",b."WorkTotal",b."Comments",c."WorkDay",a."empID",t0."ddate",b."UserSign2",b."UpdateDate";
		
		sqlString = N'SELECT distinct "EmpId","ExtEmpNo","EmpName","Department","Period","Branch","TotalBefore","DocStatus","Extra","WorkTotal","Comments","UserSign2","UpdateDate", '
					 || cast(:BaseEntry as nvarchar) || ' As "BaseEntry" '
					 || case when ifnull(:daterage,'') = '' then '' else ',' || :daterage end || '	FROM #tbl a Order By "EmpId";';
		
		--PRINT @sql
		EXECUTE IMMEDIATE :sqlString;
		
		Select :Branch As "Branch", :Period As "Period", :Dept As "Dept", :SStatus As "SStatus", :OvertimeType AS "OvertimeType" From Dummy; 
		
		DROP TABLE #APZ_TSH1;
		DROP TABLE #APZ_OTSH;
		DROP TABLE #DateData;
		DROP TABLE #tbl;
	
	END IF;
		
	DROP TABLE #temp_NhanVien;
END
