CREATE PROCEDURE APZ_InOutBaseDoc_Load(in AbsEntry int default 0)
AS 
BEGIN
	declare Period nvarchar(50);
	declare Branch int;
	DECLARE StartDate timestamp;
	DECLARE EndDate timestamp;
	DECLARE HourPerDay int;
	DECLARE sqlString nvarchar(4000000) = '';
	DECLARE tblSplitWC TABLE ("empID" Int, "ddate" Timestamp, "CalcWorkCode" nvarchar(20), "Rate" Decimal(19,6), "HourPerDay" Decimal(19,6));
	
	if(ifnull(AbsEntry,0) = 0) then 
		return;
	end if;
	
	select top 1  "Period","Branch" into Period, Branch from APZ_OTBD where "AbsEntry" = :AbsEntry;

	select IFNULL((SELECT a."FromDate" FROM APZ_OTBD a WHERE a."AbsEntry"=:AbsEntry),'1900-01-01') into StartDate from DUMMY;
	select IFNULL((SELECT a."ToDate" FROM APZ_OTBD a WHERE a."AbsEntry"=:AbsEntry),'1900-01-01') into EndDate from DUMMY;
	select ifnull((select "HourPerDay" from APZ_OHWP where "PeriodCode" = :Period and "Branch" = :Branch limit 1),8) into HourPerDay from DUMMY;

	create local temporary table #temp_Employee("empID" int, "ExtEmpNo" nvarchar(50), "empName" nvarchar(254), "dept" int, "Department" nvarchar(254), "EnrollNumber" int, "MachineNo" int);
	insert into #temp_Employee
		SELECT a."empID",a."ExtEmpNo",a."firstName" || ',' || a."lastName",a."dept",b."Name",c."EnrollNo",c."MachineNo"
			FROM OHEM a LEFT JOIN APZ_OTEM c on a."empID" = c."empNo"
						LEFT JOIN OUDP b ON a."dept"=b."Code"
		WHERE a."branch"= Ifnull(:Branch,0) 
			AND a."Active"='Y'
			AND (ifnull(a."startDate",:EndDate)<=:EndDate and ifnull(a."termDate",:StartDate)>=:StartDate)
			and c."EnrollNo" is not null and c."empNo" is not null;
	
	/*Lấy ra bảng chứa thông tin các ngày trong khoảng kì chấm công*/
	Create local temporary table #Number (GENERATED_PERIOD_START int);
	
	Insert Into #Number
		SELECT GENERATED_PERIOD_START FROM SERIES_GENERATE_DECIMAL(1, 0, DAYS_BETWEEN(:StartDate,:EndDate) + 2);
		
	Create local temporary table #tempDate ("ddate" timestamp null);
	Insert Into #tempDate
		SELECT DISTINCT CAST(ADD_DAYS(:StartDate, a.GENERATED_PERIOD_START) AS timestamp) FROM #Number A;

	create local temporary table #FullDate as(SELECT A.*, mod(WEEKDAY("ddate") + 1, 7) + 1 AS "wkDay", DAYOFMONTH("ddate") AS "mthDay", DAYOFYEAR("ddate") AS "yrDay" FROM #tempDate A);
	
	/*Lấy ra các thông tin liên quan đến lịch trình làm việc như thông tin 
	- CÁCH CHỌN GIỜ VÀO/RA
	- THÔNG TIN NGÀY NGHỈ LỄ
	- CHU KÌ CỦA LỊCH TRÌNH (Theo Tuần, Tháng, Năm)
	*/
	temp_Schedule = select a."ddate", Cast(null as int) "wkDay", Cast(null as int) "mthDay", Cast(null as int) "yrDay", Cast(null as int) "SchID",b.*
		from #FullDate a 
			LEFT JOIN #temp_Employee b on a."ddate" between :StartDate and ADD_DAYS(:EndDate,1)
		order by "EnrollNumber","ddate";
	Create local temporary table #temp_Schedule as (Select * From :temp_Schedule);
	
	Update a Set "wkDay" = b."wkDay","mthDay" = b."mthDay","yrDay" = b."yrDay" From #temp_Schedule a Join #FullDate b On a."ddate" = b."ddate";
	Update a Set "SchID" = b."SchID" 
	From #temp_Schedule a Join APZ_OWSR b On a."empID" = b."empID"
	Where "ddate" between b."ValidFrom" and b."ValidTo";
	
	/*Lấy ra các thông tin liên quan đến lịch trình làm việc như thông tin 
	- CÁCH CHỌN GIỜ VÀO/RA
	- THÔNG TIN NGÀY NGHỈ LỄ
	- CHU KÌ CỦA LỊCH TRÌNH (Theo Tuần, Tháng, Năm)
	*/
	create local temporary table #temp_EmpSchedule as(
		select b.*,e."CycleMode",k."InOutMethod",k."MinInterval",k."MaxInterval",k."BetweenInterval"
			, case when exists(select * from APZ_HLD1 ld1 where (ld1."isCurYear" = 'Y' and "ddate" between ld1."StrDate" and ld1."EndDate") 
							or (ld1."isCurYear" = 'N' and DAYOFYEAR("ddate") between DAYOFYEAR(ld1."StrDate") and DAYOFYEAR(ld1."EndDate"))) then 'Y' else 'N' end "IsHoliday"
			, case when cast(l."WndFrm" as int) <= cast(l."WndTo" as int) then (case when b."wkDay" between cast(l."WndFrm" as int) and cast(l."WndTo" as int) then 'Y' else 'N' end) 
					else (case when b."wkDay" >= cast(l."WndFrm" as int) or b."wkDay" <= cast(l."WndTo" as int) then 'Y' else 'N' end) end "IsWeekend"
			,e."RoundUnit", Cast(null as int) "ShftID", Cast(null as int) "StartTimeIn", Cast(null as int) "EndTimeIn", Cast(null as int) "StartTimeOut", Cast(null as int) "EndTimeOut"
			,Cast(null as nvarchar) "IsNightShift"
		from #temp_Schedule b
			LEFT JOIN APZ_OWSC e on e."SchID" = b."SchID"
			LEFT JOIN APZ_OIOM k on k."MethodId" = e."InOutID"
			LEFT JOIN APZ_OHLD l on l."HldCode" = e."HldCode"
		order by "EnrollNumber","ddate");
	
	Update #temp_EmpSchedule Set "IsWeekend" = 'N'
	Where "SchID" is Not Null;
	Update a Set "IsWeekend" = 'Y'
	From #temp_EmpSchedule a Left Join APZ_OWSC b On :Branch = b."branch" And a."SchID" = b."SchID"
	Where a."SchID" is Null Or IFNULL(b."SchName",'') = 'OFF';	
	
	Update a Set "ShftID" = IFNULL((b."ShiftID"),0)
	From #temp_EmpSchedule a
		JOIN (Select "ShiftID","SchID", ROW_NUMBER() OVER (PARTITION BY "SchID" ORDER BY "DayID") as "rn" From APZ_WSC1) b on a."SchID" = b."SchID"
		Left Join APZ_OWSC c On :Branch = c."branch" And a."SchID" = c."SchID"
	Where a."SchID" is Not Null Or IFNULL(c."SchName",'') != 'OFF' and b."rn" = 1;
	
	Update a Set "StartTimeIn" = IFNULL(b."StartTimeIn",0),
				"EndTimeIn" = IFNULL(b."EndTimeIn",0),
				"StartTimeOut" = IFNULL(b."StartTimeOut",0),
				"EndTimeOut" = IFNULL(b."EndTimeOut",0),
				"IsNightShift" = IFNULL(b."IsNightShift",'N')
	From #temp_EmpSchedule a
		JOIN APZ_OEWS b on a."ShftID" = b."ShiftID";

	--[APZ_InOutBaseDoc_Load] 2
	/*Lấy dữ liệu chấm công*/
	create local temporary table #tblAttendDraft as (
		Select top 0 Cast(Null As Timestamp) As "DateTimeRecord",
				Cast(Null As Timestamp) As "ClockDate",
				Cast(Null As Int) As "OriginType",
				Cast(Null As Int) As "Source",
				T0.*,
				Cast(Null As Char(5)) As "InOutString",
				Cast(Null As INT) As "InOutTime",
				Cast(Null As date) As "DateOnly",
				Cast(Null As Int) As "Increase",
				Cast(Null As int) As "WorkTime"
 		From #temp_EmpSchedule T0
	);
	
	Insert Into #tblAttendDraft
		Select b."DateTimeRecord",b."ClockDate",b."OriginType",b."Source",T0.*, TO_VARCHAR(b."DateTimeRecord", 'HH24:mi')
			, cast(REPLACE(TO_VARCHAR(b."DateTimeRecord", 'HH24:mi'),':','') as int), cast(T0."ddate" as date)
			, 0, cast(REPLACE(TO_VARCHAR(b."DateTimeRecord", 'HH24:mi'),':','') as int)
		From #temp_EmpSchedule T0 left join (select * from APZ_TBD1 where "AbsEntry" = :AbsEntry) b on b."EnrollNo" = T0."EnrollNumber" and T0."ddate" = b."ClockDate" and b."MachineNo" = T0."MachineNo"
			and ifnull((CAST(b."DateTimeRecord" as Date)),T0."ddate") = T0."ddate" 
			and ifnull((CAST(b."DateTimeRecord" as Date)),T0."ddate") between :StartDate and ADD_DAYS(:EndDate,1) 
		Order By Cast(b."DateTimeRecord" as date),b."EnrollNo";
	
	--Khi đã có dữ liệu chấm công ở 1 máy bất kì thì xóa dữ liệu ở các máy có dữ liệu = null
	delete from #tblAttendDraft x where "DateTimeRecord" is null and exists(select 1 from #tblAttendDraft y where y."empID" = x."empID" and y."ddate" = x."ddate" and y."DateTimeRecord" is not null);
	--Khi không có dữ liệu chấm công ở bất kỳ mày chấm công nào thì xóa dữ liệu null chỉ để lại 1 máy bất kì
	delete from #tblAttendDraft x where "DateTimeRecord" is null and "MachineNo" <> (select min("MachineNo") from #tblAttendDraft y where y."empID" = x."empID" and y."ddate" = x."ddate") 
											and ifnull((select count(1) from #tblAttendDraft y where y."empID" = x."empID" and y."ddate" = x."ddate"),0) > 1;
	
	/*Lọc data check in check out*/
	create local temporary table #tblIn as(
		Select *, ROW_NUMBER() OVER (PARTITION BY "empID",Cast("ddate" as Timestamp) ORDER BY IFNULL("InOutTime",0) Asc) As "RowNumber",Count(*) OVER (PARTITION BY "empID",Cast("ddate" As Timestamp) ORDER BY "empID" Asc) As "MaxRowNum"
		From #tblAttendDraft Where 1=0);
	create local temporary table #TblOut as(
		Select *, ROW_NUMBER() OVER (PARTITION BY "empID",Cast("ddate" as Timestamp) ORDER BY IFNULL("InOutTime",0) Asc) As "RowNumber",Count(*) OVER (PARTITION BY "empID",Cast("ddate" As Timestamp) ORDER BY "empID" Asc) As "MaxRowNum"
		From #tblAttendDraft Where 1=0);
	create local temporary table #tblInOut as(
		SELECT CAST(null as int) "empID", CAST(null as nvarchar(150)) "ExtEmpNo", CAST(null as nvarchar(150)) "empName", CAST(null as int) "dept", CAST(null as nvarchar(150)) "Department", CAST(null as TimeStamp) "ddate",
			CAST(null as TimeStamp)"DateOnly", CAST(null as TimeStamp) "DateTimeRecord", CAST(null as  numeric(19,6)) "RoundUnit", CAST(null as int) "SchID", CAST(null as int) "ShftID",CAST(null as numeric(19,6)) "MinInterval",
			CAST(null as numeric(19,6)) "MaxInterval", CAST(null as numeric(19,6)) "BetweenInterval", CAST(null as int)"wkDay", CAST(null as int)"mthDay", CAST(null as int)"yrDay", 
			CAST(null as nvarchar(1)) "IsHoliday", CAST(null as nvarchar(1)) "IsWeekend", CAST(null as int) "InTime", CAST(null as int) "OutTime", CAST(null as int) "InWorkTime", CAST(null as int) "OutWorkTime",
			CAST(null as int) "Late", CAST(null as int) "Early", CAST(null as int) "BeforeOT",CAST(null as int) "AfterOT", CAST(null as numeric(19,6)) "DHour", CAST(null as int) "DMinutes",
			CAST(null as numeric(19,6)) "DWorkHour", CAST(null as int) AS "DWorkMinutes", CAST(null as numeric(19,6)) "Rate", CAST(null as numeric(19,6)) "WorkRate", CAST(null as numeric(19,6)) "TotalRate",
			Cast(null as nvarchar(50)) "DateTimeIn", Cast(null as nvarchar(50)) "DateTimeOut", Cast(null as nvarchar(250)) "CalcWorkCode", Cast(null as nvarchar(20)) "TotalWorkCode" FROM dummy Where 1=0);
	
	NShiftIn = Select *, ROW_NUMBER() OVER (PARTITION BY "empID",Cast("ddate" as Timestamp) ORDER BY IFNULL("InOutTime",0) Asc) As "RowNumber",
			Count(*) OVER (PARTITION BY "empID",Cast("ddate" As Timestamp) ORDER BY "empID" Asc) As "MaxRowNum"
	From #tblAttendDraft 
	Where IFNULL("IsNightShift",'N')='Y' And ("WorkTime" Is null Or (("WorkTime" Between "StartTimeIn"-400 And "EndTimeIn") AND "empID" IS NOT NULL));
	Insert Into #tblIn SELECT * FROM :NShiftIn WHERE "RowNumber" = 1;
	
	NShiftOut = Select a.*, ROW_NUMBER() OVER (PARTITION BY a."empID",Cast(a."ddate" as Timestamp) ORDER BY IFNULL(a."InOutTime",0) Asc) As "RowNumber",
			Count(*) OVER (PARTITION BY a."empID",Cast(a."ddate" As Timestamp) ORDER BY a."empID" Asc) As "MaxRowNum"
	From #tblAttendDraft a Join #TblIn b On a."empID" = b."empID" And a."ddate" = ADD_DAYS(b."ddate",1) WHERE b."WorkTime" Is Not null And a."WorkTime" Is Not null And a."WorkTime" <= b."EndTimeOut" + 400;
	Insert Into #TblOut SELECT a.* FROM :NShiftOut a Where a."RowNumber" = "MaxRowNum";
	
	Insert Into #tblInOut 
		SELECT T1."empID", T1."ExtEmpNo", T1."empName", T1."dept", T1."Department",T1."ddate",T1."DateOnly",T1."DateTimeRecord", IFNULL(T1."RoundUnit",1) as "RoundUnit",T1."SchID"
			,T1."ShftID",T1."MinInterval",T1."MaxInterval",T1."BetweenInterval",T1."wkDay", T1."mthDay", T1."yrDay", T1."IsHoliday", T1."IsWeekend",
			T1."InOutTime" "InTime",
			T2."InOutTime" "OutTime",
			T1."WorkTime" "InWorkTime",
			T2."WorkTime" "OutWorkTime",
			CAST(null as int) "Late",
			CAST(null as int) "Early",
			CAST(null as int) "BeforeOT",
			CAST(null as int) "AfterOT",
			CAST(null as numeric(19,6)) "DHour",
			CAST(null as int) "DMinutes",
			CAST(null as numeric(19,6)) "DWorkHour",
			CAST(null as int) AS "DWorkMinutes",
			CAST(null as numeric(19,6)) "Rate",
			CAST(null as numeric(19,6)) "WorkRate",
			CAST(null as numeric(19,6)) "TotalRate",
			TO_VARCHAR(T1."DateTimeRecord",'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast(T1."WorkTime" as nvarchar),4),2) || ':' || RIGHT(cast(T1."WorkTime" as nvarchar),2) "DateTimeIn",
			TO_VARCHAR(T2."DateTimeRecord", 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast(T2."WorkTime" as nvarchar),4),2) || ':' || RIGHT(cast(T2."WorkTime" as nvarchar),2) "DateTimeOut", 
			Cast(null as nvarchar(250)) "CalcWorkCode", 
			Cast(null as nvarchar(20)) "TotalWorkCode"
		FROM #tblIn T1 LEFT JOIN #TblOut T2 On T1."empID" = T2."empID" And ADD_DAYS(T1."ddate",1) = T2."ddate";
	
	AbOutTmp = Select a.*, ROW_NUMBER() OVER (PARTITION BY a."empID",Cast(a."ddate" as Timestamp) ORDER BY IFNULL(a."InOutTime",0) Asc) As "RowNumber" 
	From #tblAttendDraft a Join #TblIn b On a."empID" = b."empID" And a."ddate" = b."ddate" Where a."WorkTime" != b."InOutTime";
	
	Delete From #tblAttendDraft a Where Exists (Select 1 From #TblIn b Where a."empID" = b."empID" And a."ddate" = b."ddate" And (a."DateTimeRecord" Is Null Or a."DateTimeRecord" >= b."DateTimeRecord"));
	Delete From #tblAttendDraft a Where Exists (Select 1 From #TblIn b Where a."empID" = b."empID" And a."ddate" = b."ddate") And ("WorkTime" Is null Or ("WorkTime" Not Between "StartTimeIn" And "EndTimeIn"));
	
	tblAttendance = select a.*, ROW_NUMBER() OVER (PARTITION BY a."empID",Cast(a."ddate" as Timestamp) ORDER BY IFNULL(a."InOutTime",0) Asc) As "RowNumber",
			Count(*) OVER (PARTITION BY a."empID", Cast(a."ddate" As Timestamp) ORDER BY a."empID" Asc) As "MaxRowNum"
		From #tblAttendDraft a;
	Delete From #TblIn;
	Delete From #TblOut;
	
	--Insert Into #TblIn
	--Select a.* From :tblAttendance a WHERE "WorkTime" Is null Or "ShftID" Is Null Or ("StartTimeIn" Is not null And "EndTimeIn" Is not null And "WorkTime" Between "StartTimeIn" And "EndTimeIn" And "RowNumber" = 1)
	--Union
	--Select a.* From :tblAttendance a WHERE "WorkTime" Is Not null And "WorkTime" Not Between "StartTimeIn" And "EndTimeIn" And "RowNumber" = 1;
	
	Insert Into #TblIn
	Select a.* From :tblAttendance a WHERE "WorkTime" Is null Or ("StartTimeIn" Is not null And "EndTimeIn" Is not null And "WorkTime" Between "StartTimeIn" And "EndTimeIn" And "RowNumber" = 1)
	Union
	Select a.* From :tblAttendance a WHERE "WorkTime" Is Not null And "RowNumber" = 1;
	
	Insert Into #TblOut
	Select a.* From :tblAttendance a JOIN #TblIn b On a."empID" = b."empID" And a."ddate" = b."ddate" Where a."WorkTime" Is Not null And a."RowNumber" = a."MaxRowNum" And a."MaxRowNum" != 1;
	
	Insert Into #tblInOut SELECT T1."empID", T1."ExtEmpNo", T1."empName", T1."dept", T1."Department",T1."ddate",T1."DateOnly",T1."DateTimeRecord", IFNULL(T1."RoundUnit",1) as "RoundUnit",T1."SchID"
			,T1."ShftID",T1."MinInterval",T1."MaxInterval",T1."BetweenInterval",T1."wkDay", T1."mthDay", T1."yrDay", T1."IsHoliday", T1."IsWeekend",
			T1."InOutTime" "InTime",
			T2."InOutTime" "OutTime",
			T1."WorkTime" "InWorkTime",
			T2."WorkTime" "OutWorkTime",
			CAST(null as int) "Late",
			CAST(null as int) "Early",
			CAST(null as int) "BeforeOT",
			CAST(null as int) "AfterOT",
			CAST(null as numeric(19,6)) "DHour",
			CAST(null as int) "DMinutes",
			CAST(null as numeric(19,6)) "DWorkHour",
			CAST(null as int) AS "DWorkMinutes",
			CAST(null as numeric(19,6)) "Rate",
			CAST(null as numeric(19,6)) "WorkRate",
			CAST(null as numeric(19,6)) "TotalRate",
			TO_VARCHAR(T1."DateTimeRecord",'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast(T1."WorkTime" as nvarchar),4),2) || ':' || RIGHT(cast(T1."WorkTime" as nvarchar),2) "DateTimeIn",
			TO_VARCHAR(T2."DateTimeRecord", 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast(T2."WorkTime" as nvarchar),4),2) || ':' || RIGHT(cast(T2."WorkTime" as nvarchar),2) "DateTimeOut", 
			Cast(null as nvarchar(250)) "CalcWorkCode", 
			Cast(null as nvarchar(20)) "TotalWorkCode"
		FROM #tblIn T1 LEFT JOIN #TblOut T2 On T1."empID" = T2."empID" And T1."ddate" = T2."ddate";
	
	Update a Set "InTime" = null, "OutTime" = null, "InWorkTime" = null, "OutWorkTime" = null, "DateTimeIn" = null, "DateTimeOut" = null
	From #tblInOut a Join (Select x.* From #tblInOut x LEFT JOIN APZ_OEWS y on y."ShiftID" = x."ShftID" Where IFNULL(y."IsNightShift",'N') = 'Y' And "OutTime" Is Not Null) b On a."empID" = b."empID" And ADD_DAYS(a."ddate",-1) = b."ddate" 
	Where a."InTime" = b."OutTime";
	Update a Set "OutTime" = null, "OutWorkTime" = null, "DateTimeOut" = null
	From #tblInOut a Where a."InTime" = a."OutTime";
	Delete From #tblInOut Where "ddate" > :endDate;
	
	--Xử lý data cho những ngày nghỉ
	shift = Select a."empID", a."ddate", b."ShiftID",ROW_NUMBER() OVER (PARTITION BY a."empID",Cast(a."ddate" as Timestamp) ORDER BY IFNULL(b."ShiftID",0) Asc) As "rn" 
		From #tblInOut a, APZ_OEWS b Where (a."IsWeekend"= 'Y' Or a."IsHoliday" = 'Y') And a."InTime" != 0 And a."InTime" between "StartTimeIn" - 400 And "EndTimeIn" + 200; --And a."OutTime" != 0 
	Update a Set "ShftID" = b."ShiftID"
	From #tblInOut a Join :shift b On a."empID" = b."empID" And a."ddate" = b."ddate" And b."rn"=1;
	Update a Set "OutTime" = b."InOutTime", "OutWorkTime" = b."InOutTime"
		, "DateTimeOut" = TO_VARCHAR(b."DateTimeRecord", 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast(b."WorkTime" as nvarchar),4),2) || ':' || RIGHT(cast(b."WorkTime" as nvarchar),2)
	From #tblInOut a 
		Left Join :AbOutTmp b On a."empID" = b."empID" And a."ddate" = ADD_DAYS(b."ddate",-1) And b."RowNumber"=1
	Where IFNULL(a."OutTime",0) = 0;
	
	/*Lấy ra thông tin ca trong từng ngày của từng nhân viên dựa vào ca đã chọn ở trên*/
	create local temporary table #InOutShift as(
		select a.*,b.*
			, CAST((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("EndBreak" as nvarchar),4),2) || ':' || RIGHT(cast("EndBreak" as nvarchar),2)) as timestamp) "EndBreakDate"
			, CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("StartBreak" as nvarchar),4),2) || ':' || RIGHT(cast("StartBreak" as nvarchar),2)) as timestamp) "StartBreakDate"
			, CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("InTime" as nvarchar),4),2) || ':' || RIGHT(cast("InTime" as nvarchar),2)) as timestamp) "InWorkTimeDate"
			, Case When Ifnull(b."IsNightShift",'N') = 'Y' 
				Then CAST ((TO_VARCHAR(ADD_DAYS(current_date,1), 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("OutTime" as nvarchar),4),2) || ':' || RIGHT(cast("OutTime" as nvarchar),2)) as timestamp)
				Else CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("OutTime" as nvarchar),4),2) || ':' || RIGHT(cast("OutTime" as nvarchar),2)) as timestamp) End As "OutWorkTimeDate"
			, CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("StartTime" as nvarchar),4),2) || ':' || RIGHT(cast("StartTime" as nvarchar),2)) as timestamp) "StartTimeDate"
			, Case When Ifnull(b."IsNightShift",'N') = 'Y' 
				Then CAST ((TO_VARCHAR(ADD_DAYS(current_date,1), 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("EndTime" as nvarchar),4),2) || ':' || RIGHT(cast("EndTime" as nvarchar),2)) as timestamp)
				Else CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("EndTime" as nvarchar),4),2) || ':' || RIGHT(cast("EndTime" as nvarchar),2)) as timestamp) End As  "EndTimeDate"
		from #tblInOut a LEFT JOIN APZ_OEWS b on b."ShiftID" = a."ShftID"
			where "DateTimeOut" is null 
				OR (IFNULL("MinInterval",0) = 0 or ("DateTimeOut" is not null and SECONDS_BETWEEN("DateTimeIn","DateTimeOut") >= "MinInterval" * 60)));
 	
 	-- Kiểm tra dât có trường UDF đánh dấu emp không cần chấm công
	If Exists(Select 1 FROM SYS.TABLE_COLUMNS Where  SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'OHEM' AND Column_Name = 'U_FullWorkCode') Then
		--Update data cho những nhân viên đã đủ công 
		sqlString = 'Update a
			Set "InTime"= "StartTime",
				"OutTime"= "EndTime",
				"InWorkTime"= "StartTime",
				"OutWorkTime"= "EndTime"
			From #InOutShift a, OHEM b 
	 		Where a."empID" = b."empID" AND(IFNULL(b."U_FullWorkCode",'''') = ''Y'' And a."IsHoliday" = ''N'' And a."IsWeekend" = ''N'')
	 			AND ((b."startDate" is not null AND a."ddate" > b."startDate") Or (b."termDate" is not null AND a."ddate" < b."termDate"));';
	 	EXECUTE immediate :sqlString;
	 	sqlString = 'Update a
			Set "InTime"= null,
				"OutTime"= null,
				"InWorkTime"= null,
				"OutWorkTime"= null
			From #InOutShift a, OHEM b 
	 		Where a."empID" = b."empID" AND(IFNULL(b."U_FullWorkCode",'''') = ''Y'' And (a."IsHoliday" = ''Y'' Or a."IsWeekend" = ''Y''))
	 			AND ((b."startDate" is not null AND a."ddate" > b."startDate") Or (b."termDate" is not null AND a."ddate" < b."termDate"));';
	 	EXECUTE immediate :sqlString;
	 	sqlString = 'Update a
			Set "DateTimeIn"=  CAST ((TO_VARCHAR(current_date, ''yyyy/MM/dd'') || '' '' || LEFT(RIGHT(''0000'' || cast("InTime" as nvarchar),4),2) || '':'' || RIGHT(cast("InTime" as nvarchar),2)) as timestamp),
				"DateTimeOut"= Case When Ifnull(a."IsNightShift",''N'') = ''Y'' 
				Then CAST ((TO_VARCHAR(ADD_DAYS(current_date,1), ''yyyy/MM/dd'') || '' '' || LEFT(RIGHT(''0000'' || cast("EndTime" as nvarchar),4),2) || '':'' || RIGHT(cast("EndTime" as nvarchar),2)) as timestamp)
				Else CAST ((TO_VARCHAR(current_date, ''yyyy/MM/dd'') || '' '' || LEFT(RIGHT(''0000'' || cast("EndTime" as nvarchar),4),2) || '':'' || RIGHT(cast("EndTime" as nvarchar),2)) as timestamp) End
			From #InOutShift a, OHEM b 
	 		Where a."empID" = b."empID" AND(IFNULL(b."U_FullWorkCode",'''') = ''Y'' And a."IsHoliday" = ''N'' And a."IsWeekend" = ''N'')
	 			AND ((b."startDate" is not null AND a."ddate" > b."startDate") Or (b."termDate" is not null AND a."ddate" < b."termDate"));';
	 	EXECUTE immediate :sqlString;
	End If;
 	
 	--[APZ_InOutBaseDoc_Load] 2
	/*PHẦN TÍNH TOÁN RA CÔNG*/
	--Chỉ tính thời gian làm việc trong khoảng thời gian đã báo
	update #InOutShift set "InWorkTime" = case when ("InTime" < "StartTime" and "OutTime" >="StartTime") then "StartTime"
												when ("InTime" < "StartTime" and "OutTime" < "StartTime") or ("InTime" > "EndTime" and "OutTime" > "EndTime") then null 
												else "InTime" end
							,"OutWorkTime" = case when ("InTime" < "StartTime" and "OutTime" < "StartTime") or ("InTime" > "EndTime" and "OutTime" > "EndTime") then null 
												when ("OutTime" > "EndTime" and "InTime" <= "EndTime") then "EndTime" 
												else "OutTime" end
						where "OutTime" is not null and "StartTime" is not null and "EndTime" is not null;
	Update #InOutShift 
	Set "InWorkTime" = case when ("InTime" < "StartTime" AND ((LENGTH("OutTime") = 4 And "OutTime" >= "StartTime") Or ("OutTime" Between "StartTimeOut"-400 And "EndTimeOut" + 400))) then "StartTime" 
						when ("InTime" < "StartTime" AND LENGTH("OutTime") = 4 AND "OutTime" < "StartTime") or (LENGTH("InTime") = 3 AND "InTime" > "EndTime" AND "OutTime" > "EndTime") then null
						when ("InTime" > "StartTime" AND LENGTH("OutTime") = 3 OR (LENGTH("OutTime") = 4 AND "OutTime" > "EndTime")) then "StartTime" 
						else "InTime" end
		,"OutWorkTime" = case when ("InTime" < "StartTime" AND LENGTH("OutTime") = 4 And "OutTime" < "StartTime" And "InTime" < "OutTime") then null
						when ("OutTime" > "EndTime" and ((LENGTH("InTime") = 3 AND "InTime" <= "EndTime") Or LENGTH("InTime") = 4)) then "EndTime" 
						when ("OutTime" < "EndTime" and ((LENGTH("InTime") = 3 AND "InTime" < "OutTime") Or LENGTH("InTime") = 4)) then "EndTime" 
						else "OutTime" end 
	Where "OutTime" is not null and "StartTime" is not null and "EndTime" is not null And Ifnull("IsNightShift",'N') = 'Y';
	
	--Nếu giờ vào là trong khoảng thời gian ăn trưa thì set giờ vào là giờ kết thúc ăn trưa
	update #InOutShift set "InWorkTime" = case when "InWorkTime" between "StartBreak" and "EndBreak" then "EndBreak" else "InWorkTime" end 
						where "InWorkTime" is not null and "StartBreak" is not null and "EndBreak" is not null;
	-- Nếu giờ ra là trong khoảng thời gian ăn trưa thì set giờ ra là giờ bắt đầu ăn trưa
	update #InOutShift set "OutWorkTime" = case when "OutWorkTime" between "StartBreak" and "EndBreak" then "StartBreak" else "OutWorkTime" end
						where "OutWorkTime" is not null and "StartBreak" is not null and "EndBreak" is not null;
	
	/*Update lại DateTimeOut: Case IsNightShift and not IsNightShift */
	Update a Set "InWorkTimeDate" = CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("InTime" as nvarchar),4),2) || ':' || RIGHT(cast("InTime" as nvarchar),2)) as timestamp),
		"OutWorkTimeDate" = CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("OutTime" as nvarchar),4),2) || ':' || RIGHT(cast("OutTime" as nvarchar),2)) as timestamp)
	From #InOutShift a, APZ_OEWS c
	Where a."ShftID" = c."ShiftID" And Ifnull(c."IsNightShift",'N') = 'N';
	Update a Set "InWorkTimeDate" = CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("InTime" as nvarchar),4),2) || ':' || RIGHT(cast("InTime" as nvarchar),2)) as timestamp),
		"OutWorkTimeDate" = CAST ((TO_VARCHAR(ADD_DAYS(current_date,1), 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("OutTime" as nvarchar),4),2) || ':' || RIGHT(cast("OutTime" as nvarchar),2)) as timestamp)
	From #InOutShift a, APZ_OEWS c
	Where a."ShftID" = c."ShiftID" And Ifnull(c."IsNightShift",'N') = 'Y';
	
	update #InOUtShift set 
		--Tính đi muộn
		"Late" = (case when ifnull("CalcLate",'N') = 'Y' then (case when SECONDS_BETWEEN("StartTimeDate","InWorkTimeDate")/60 - ifnull("LateGrace",0) >0 
															then  SECONDS_BETWEEN("StartTimeDate","InWorkTimeDate")/60 - ifnull("LateGrace",0) else 0 end ) else 0 end)
		--Tính về sớm
		,"Early" = (case when ifnull("CalcEarly",'N') = 'Y' then (case when SECONDS_BETWEEN("OutWorkTimeDate","EndTimeDate") / 60 - ifnull("EarlyGrace",0) > 0 
															then  SECONDS_BETWEEN("OutWorkTimeDate","EndTimeDate") / 60 - ifnull("EarlyGrace",0) else 0 end ) else 0 end)
		--Tính thời gian OT trước giờ làm việc
		,"BeforeOT" = (case when ifnull("CalcBeforeOT",'N') = 'Y' then (case when SECONDS_BETWEEN("InWorkTimeDate","StartTimeDate") / 60 - IFNULL("MinBeforeDuty",0) > 0 
																	then  SECONDS_BETWEEN("InWorkTimeDate","StartTimeDate") / 60 - IFNULL("MinBeforeDuty",0) else 0 end ) else 0 end)
		--Tính thời gian OT sau giờ làm việc
		,"AfterOT" = (case when ifnull("CalcAfterOT",'N') = 'Y' then (case when SECONDS_BETWEEN("EndTimeDate","OutWorkTimeDate") / 60 - IFNULL("MinAfterDuty",0) > 0
																	then SECONDS_BETWEEN("EndTimeDate","OutWorkTimeDate") / 60 - IFNULL("MinAfterDuty",0) else 0 end ) else 0 end);

	update #InOutShift set 
		--Tính Rate dựa vào giờ VÀO - RA
		"Rate" = case when ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") and "StartBreak" is not null and "EndBreak" is not null
					--Trừ đi khoảng thời gian ăn trưa
					then (((SECONDS_BETWEEN(Cast("DateTimeIn" as timestamp),Cast("DateTimeOut" as timestamp)) / 60 - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") / 60)/60.0) / :HourPerDay)
					else (((SECONDS_BETWEEN(Cast("DateTimeIn" as timestamp),Cast("DateTimeOut" as timestamp)) / 60)/60.0) / :HourPerDay) end
	
		--WorkRate: Rate dựa vào khoảng thời gian được tính là làm việc
		,"WorkRate" = case when ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") and "StartBreak" is not null and "EndBreak" is not null
			--Trừ đi khoảng thời gian ăn trưa
			then (((SECONDS_BETWEEN(Cast("InWorkTimeDate" as timestamp),Cast("OutWorkTimeDate" as timestamp)) / 60 - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") / 60)/60.0) / :HourPerDay)
			else (((SECONDS_BETWEEN(Cast("InWorkTimeDate" as timestamp),Cast("OutWorkTimeDate" as timestamp)) / 60)/60.0) / :HourPerDay) end
		
		--Số giờ thực tế giữa vào - ra theo ca
		,"DWorkHour" = case when ("StartBreak" is not null and "EndBreak" is not null) AND ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") then (
			--Trừ đi khoảng thời gian ăn trưa khi thời gian In Out có chứa thời gian nghỉ ăn trưa
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "EndTimeDate")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("StartTimeDate", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("StartTimeDate", "EndTimeDate") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /3600.0
			else SECONDS_BETWEEN("StartTimeDate", "EndTimeDate")/3600.0 end
		-- Số phút thực tế giữa vào - ra theo ca
		,"DWorkMinutes" = case when ("StartBreak" is not null and "EndBreak" is not null) AND ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") then (
			--Trừ đi khoảng thời gian ăn trưa khi thời gian In Out có chứa thời gian nghỉ ăn trưa
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "EndTimeDate")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("StartTimeDate", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("StartTimeDate", "EndTimeDate") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /60.0
			else SECONDS_BETWEEN("StartTimeDate", "EndTimeDate")/60.0 end
	
		--Số giờ làm việc thực tế
		,"DHour" = case when ("StartBreak" is not null and "EndBreak" is not null) AND ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") then (
			--Trừ đi khoảng thời gian ăn trưa khi thời gian In Out có chứa thời gian nghỉ ăn trưa
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "DateTimeOut")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("DateTimeIn", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /3600.0
			else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut")/3600.0 end
		-- Số phút làm việc thực tế
		,"DMinutes" = case when ("StartBreak" is not null and "EndBreak" is not null) AND ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") then (
			--Trừ đi khoảng thời gian ăn trưa khi thời gian In Out có chứa thời gian nghỉ ăn trưa
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "DateTimeOut")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("DateTimeIn", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /60.0
			else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut")/60.0 end
	where "InWorkTime" is not null and "OutWorkTime" is not null;
 
 	/*Những trường hợp giờ không nằm trong khoảng thời gian của ca thì sử dụng bảng combine để set workcode*/
	IF EXISTS(SELECT 1 FROM OBJECTS WHERE OBJECT_TYPE='TABLE' AND SCHEMA_NAME = CURRENT_SCHEMA AND OBJECT_NAME='APZ_EWS1')
	Then
		Update a Set "CalcWorkCode" = Case When "DMinutes" >= a."WorkingTotal" Then b."WorkCode"
								Else
									Case When IFNULL(a."IsWeekend",'N') = 'Y' Or IFNULL(a."IsHoliday",'N') = 'Y' Then null 
									Else
										(Select Case When IFNULL(c."WorkCode",'') != '' AND IFNULL(c."AbsentCode",'') != '' Then c."WorkCode" || ',' || c."AbsentCode"
												When IFNULL(c."WorkCode",'') = '' AND IFNULL(c."AbsentCode",'') = '' Then ''
												When IFNULL(c."WorkCode",'') = '' AND IFNULL(c."AbsentCode",'') != '' Then c."AbsentCode"
												Else c."WorkCode" End
										From APZ_EWS1 c
										Where c."ShiftID" = a."ShiftID"
											AND a."DMinutes" Between c."FromMinutes" And c."ToMinutes") 
									End 
								End
		From #InOutShift a 
		JOIN APZ_OEWS b On a."ShiftID" = b."ShiftID"
		Where Ifnull(a."InTime",0) != 0 AND Ifnull(a."OutTime",0) != 0;
		
		Insert Into :tblSplitWC("empID", "ddate", "CalcWorkCode","HourPerDay") Select x."empID", x."ddate", y."WorkCode", IFNULL(z."WorkingTotal",0)
		From #InOutShift x 
			Join APZ_EWS1 y On x."ShiftID" = y."ShiftID" AND x."DWorkMinutes" Between y."FromMinutes" And y."ToMinutes"
			LEFT JOIN APZ_OEWS z On x."ShiftID" = z."ShiftID"
		Where x."CalcWorkCode" LIKE '%,%' Or x."CalcWorkCode" In (Select a."Code" From APZ_OHWC a LEFT JOIN APZ_OHWT b On a."WorkType" = b."Id" Where b."Typ" = 'W' And a."Rate" = 0.5);
		
		Update x Set "Rate" = IFNULL(y."Rate",0)
		From :tblSplitWC x Left Join APZ_OHWC y on x."CalcWorkCode" = y."Code";
	End If;
	
	--UPDATE lại các thông tin trong bảng tổng hợp dựa vào thông tin ca của từng dòng
	--update #InOUtShift set "CalcWorkCode" = "WorkCode" where "InTime" is not null or "OutTime" is not null;
	update a set "CalcWorkCode" = a."AbsentCode"
	From #InOUtShift a 
	where IFNULL(a."InTime",0) = 0 and IFNULL(a."OutTime",0) = 0 And Ifnull(a."CalcAbsent",'N') = 'Y';
	Update a Set "CalcWorkCode" = Null 
	From #InOutShift a 
	Where Ifnull(a."IsHoliday",'N') = 'N' AND Ifnull(a."IsWeekend",'N') = 'N' AND ((Ifnull(a."InTime",0) != 0 And Ifnull(a."OutTime",0) =0) OR (Ifnull(a."InTime",0) = 0 And Ifnull(a."OutTime",0) != 0));
	--Cập nhật thông tin WorkCode,AbsentCode những ngày nằm trong ngày nghỉ lễ
	Update a Set "CalcWorkCode" = "HldCode"
	From #InOutShift a
	Where Ifnull(a."IsHoliday",'N') = 'Y' And Ifnull(a."CalcHld",'N') = 'Y'; --And Ifnull(a."InTime",0) != 0 AND Ifnull(a."OutTime",0) != 0
	--Update a Set "CalcWorkCode" = null
	--From #InOutShift a
	--Where Ifnull(a."IsHoliday",'N') = 'Y' And Ifnull(a."InTime",0) = 0 AND Ifnull(a."OutTime",0) = 0 And Ifnull(a."CalcHld",'N') = 'Y';
	
	---Delete thông tin WorkCode, AbsentCode những ngày nằm trong trường hợp là nhân viên mới hoặc nhân viên vừa nghỉ việc	
	UPDATE a SET "CalcWorkCode" = null
	From #InOutShift a
	WHERE EXISTS (SELECT 1 
	              FROM OHEM b
	              WHERE a."empID" = b."empID" AND 
	              	((b."startDate" is not null AND a."ddate" < b."startDate") Or (b."termDate" is not null AND a."ddate" > b."termDate")));
	--Cập nhật thông tin workcode không lấy mã nghỉ phép khi rơi vào ngày cuối tuần
	Update a Set "CalcWorkCode" = Null From #InOutShift a Where Ifnull(a."IsWeekend",'N') = 'Y' AND (Ifnull(a."InTime",0) = 0 OR Ifnull(a."OutTime",0) = 0) AND Ifnull(a."IsHoliday",'N') = 'N';
	
	/*Cập nhật lại thông tin Late, Early*/	
	Update x Set 
	"Late" = Case When (IFNULL("Late",0) < IFNULL("Early",0)) 
		Then IFNULL("Late",0) 
		Else (Case When ("Late" - IFNULL(y."Rate"*y."HourPerDay"*60,0) - SECONDS_BETWEEN("DateTimeIn","StartBreakDate")/60) > 0 Then ("Late" - IFNULL(y."Rate"*y."HourPerDay"*60,0) - SECONDS_BETWEEN("DateTimeIn","StartBreakDate")/60 ) Else 0 End) End, 
	"Early" = Case When (IFNULL("Late",0) < IFNULL("Early",0)) 
		Then (Case When ("Early" - IFNULL(y."Rate"*y."HourPerDay"*60,0) - SECONDS_BETWEEN("StartBreakDate","DateTimeOut")/60) > 0 Then ("Early" - IFNULL(y."Rate"*y."HourPerDay"*60,0) - SECONDS_BETWEEN("StartBreakDate","DateTimeOut")/60 ) Else 0 End) 
		Else IFNULL("Early",0) End
	From #InOUtShift x JOIN :tblSplitWC y On x."empID" = y."empID" And x."ddate" = y."ddate";
	Update a Set "Late" = 0, "Early" = 0
	From #InOUtShift a JOIN APZ_OHWC c on c."Branch" = :Branch and a."CalcWorkCode" = c."Code" LEFT JOIN APZ_OHWT d on c."WorkType" = d."Id" where d."Typ" = 'L';
	
	--Với trường hợp có giờ ra nhưng không có giờ vào thì sẽ lấy số phút mặc định được set ở WOrk Shift
	update #InOutShift set "DWorkHour" = ifnull("NoInWT",0)/60.0 
							,"DWorkMinutes" = ifnull("NoInWT",0)
							,"WorkRate" = ifnull("NoInWT",0) /60.0 / :HourPerDay
		where "InWorkTime" is null and "OutWorkTime" is not null;
	--Với trường hợp có giờ vào nhưng không có giờ ra thì sẽ lấy số phút mặc định được set ở WOrk Shift
	update #InOutShift set "DWorkHour" = ifnull("NoOutWT",0)/60.0 
							,"DWorkMinutes" = ifnull("NoOutWT",0)
							,"WorkRate" =ifnull("NoOutWT",0) /60.0 / :HourPerDay
		where "OutWorkTime" is null and "InWorkTime" is not null;

	--Làm tròn các thông số theo thông số làm tròn
	update #InOutShift set "DHour" = ROUND("DHour",ifnull("RoundUnit",0)),
							"DWorkHour" = ROUND("DWorkHour",ifnull("RoundUnit",0)),
							"Late" = ROUND("Late",ifnull("RoundUnit",0)),
							"Early" = ROUND("Early",ifnull("RoundUnit",0)),
							"AfterOT" = ROUND("AfterOT",ifnull("RoundUnit",0)),
							"BeforeOT" = ROUND("BeforeOT",ifnull("RoundUnit",0)),
							"Rate" = ROUND("Rate",ifnull("RoundUnit",0)),
							"WorkRate" = ROUND("WorkRate",ifnull("RoundUnit",0));
							
	update #InOutShift set "BeforeOT" = (case when "BeforeOT" > "MaxBeforeOT" then "MaxBeforeOT" else "BeforeOT" end)
						, "AfterOT" = (case when "AfterOT" > "MaxAfterOT" then "MaxAfterOT" else "AfterOT" end);

	-- Tổng RATE cho cả ngày làm việc hôm đó (Hiện tại đang chưa dùng mà sẽ dùng phần công được chấm trong phần thông tin ca)
	--update  a set "TotalRate" = (select SUM("Rate") from #InOUtShift x where x."EnrollNumber" = a."EnrollNumber" and x."DateOnly" = a."DateOnly") from #InOUtShift a;
	--update #tblInOut  set WorkCode = ifnull((Select TOP 1 Code from #tblWorkCode where Rate =ROUND(#tblInOut.Rate,4) or Rate =ROUND(#tblInOut.Rate,3) or Rate =ROUND(#tblInOut.Rate,2) or Rate =ROUND(#tblInOut.Rate,1) ),'')
	--update #tblInOut set TotalWorkCode = ifnull((Select TOP 1 Code from #tblWorkCode where Rate =ROUND(#tblInOut.TotalRate,4) or Rate =ROUND(#tblInOut.TotalRate,3) or Rate =ROUND(#tblInOut.TotalRate,2) or Rate =ROUND(#tblInOut.TotalRate,1) ),'')
	
	CREATE local temporary table #tblWorkCode AS (Select top 0 a.*  From APZ_OHWC a);
	insert into #tblWorkCode
		select a.* from APZ_OHWC a join APZ_OHWT b on a."WorkType" = b."Id" and b."Typ" = 'W' and a."Branch" = :Branch;
	
	--update #InOutShift  
	--set "CalcWorkCode" = ifnull((select "Code" 
	--						from (Select "Code", row_number() over(order by "Rate" DESC) lineid 
	--							from #tblWorkCode where "Rate" =ROUND("WorkRate",4) or "Rate" =ROUND("WorkRate",3) 
	--								or "Rate" =ROUND("WorkRate",2) or "Rate" =ROUND("WorkRate",1) or "Rate" < "WorkRate") xxx 
	--							where xxx.lineid = 1),'')
	--where "ShiftID" is not null and "IsFlexWorkCode" = 'Y' and ("InTime" is not null or "OutTime" is not null);
	
	create local temporary table #tblOverTimeTemp as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_IOD2 b ON 1=2);
	create local temporary table #tblOverTime as (select top 0 b.*, cast(null as nvarchar(254)) "empName", cast(null as int) "dept" from (select 1 A from DUMMY) a left join APZ_IOD2 b ON 1=2);
	
	create local temporary table #CheckOT as (Select a.* From #InOutShift a 
		Where "InTime" is not null And "OutTime" is not null And ((IFNULL(a."AfterOT",0) >= 0 Or IFNULL(a."BeforeOT",0) >= 0 
			Or (IFNULL(a."IsHoliday",'N') = 'Y' And IFNULL(a."DWorkHour",0) > 0) 
			Or (IFNULL(a."IsWeekend",'N') = 'Y' And IFNULL(a."DWorkHour",0) > 0)))
			);
	
	create local temporary table #tempNextDay as( Select Distinct "empID", "ddate", "IsHoliday", "IsWeekend"
	From #temp_EmpSchedule a);
	
	--Get Normal DayShift -- Ngày thường
	create local temporary table #NrmlDay as (Select a."empID", a."DateOnly"
	From #CheckOT a 
	Where Ifnull("IsWeekend",'N')='N' AND Ifnull("IsHoliday",'N')='N' AND (a."AfterOT" >= 0 OR a."BeforeOT" >= 0));
	
	Insert into #tblOverTimeTemp ("Period", "empID", "WorkDay", "OvertimeType", "OvertimeHour")
	Select :Period as "Period",a."empID",a."DateOnly"
		, (Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 1.5), IFNULL(a."BeforeOT",0) + IFNULL(a."AfterOT",0)
		From #CheckOT a JOIN #NrmlDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly" 
		Where a."EndTime" != 2200 And a."StartTime" != 600 AND Ifnull(a."IsNightShift",'N')='N' AND Ifnull(a."CalcBeforeOT",'N') = 'Y' AND Ifnull(a."CalcAfterOT",'N') = 'Y'
	Union All
	Select :Period as "Period",a."empID",a."DateOnly",(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 2.1),
		Case When (a."EndTime" = 2200 And Ifnull(a."AfterOT",0)>0) Then (Ifnull(a."AfterOT",0))
			When (a."StartTime" = 600 And Ifnull(a."BeforeOT",0)>0) Then (Ifnull(a."BeforeOT",0))
			Else 0 End 
		From #CheckOT a JOIN #NrmlDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly" 
		Where ((a."EndTime" = 2200 AND Ifnull(a."AfterOT",0)>0 And Ifnull(a."BeforeOT",0)>0) Or (a."StartTime" = 600 And Ifnull(a."BeforeOT",0)>0 AND Ifnull(a."AfterOT",0)>0))  
			And Ifnull(a."IsNightShift",'N')='N' AND Ifnull(a."CalcAfterOT",'N') = 'Y' And Ifnull(a."CalcBeforeOT",'N') = 'Y'
	Union All
	Select :Period as "Period",a."empID",a."DateOnly",Ifnull(a."WDOTWCode",''),
		Case When (a."EndTime" = 2200 And Ifnull(a."BeforeOT",0)>0) Then (Ifnull(a."BeforeOT",0))
			When (a."StartTime" = 600 And Ifnull(a."AfterOT",0)>0) Then (Ifnull(a."AfterOT",0))
			Else 0 End
	From #CheckOT a JOIN #NrmlDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly" 
	Where ((a."EndTime" = 2200 AND Ifnull(a."BeforeOT",0)>0) Or (a."StartTime" = 600 And Ifnull(a."AfterOT",0)>0)) And Ifnull(a."IsNightShift",'N')='N' AND Ifnull(a."CalcAfterOT",'N') = 'Y'
		And Ifnull(a."CalcBeforeOT",'N') = 'Y';
	
	--Get Normal NightShift --Ngày thường ca đêm
	NmlNightDay = Select a.*,c."IsHoliday" as "NIsHoliday", c."IsWeekend" as "NIsWeekend"
		From #CheckOT a JOIN #NrmlDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
						LEFT JOIN #tempNextDay c On a."empID" = c."empID" And ADD_DAYS(a."DateOnly",1) = c."ddate"
		Where Ifnull(a."IsNightShift",'N')='Y';
	INSERT InTo #tblOverTimeTemp ("Period", "empID", "WorkDay", "OvertimeType", "OvertimeHour")
	Select :Period as "Period",a."empID",a."DateOnly",
			(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 1.5),IFNULL(a."BeforeOT",0) + IFNULL(a."AfterOT",0)
		FROM #CheckOT a JOIN :NmlNightDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where Ifnull(b."NIsHoliday",'N') = 'N' AND Ifnull(b."NIsWeekend",'N') = 'N' AND Ifnull(a."CalcBeforeOT",'N') = 'Y' AND Ifnull(a."CalcAfterOT",'N') = 'Y' 
	UNION All
	Select :Period as "Period",a."empID",a."DateOnly",
			(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 1.5),(Ifnull(a."BeforeOT",0))
		FROM #CheckOT a JOIN :NmlNightDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where Ifnull(b."NIsHoliday",'N') = 'Y' AND Ifnull(b."NIsWeekend",'N') = 'N' AND Ifnull(a."BeforeOT",0) > 0 AND Ifnull(a."CalcBeforeOT",'N') = 'Y'
	UNION All
	Select :Period as "Period",a."empID",a."DateOnly",
			(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 3),(Ifnull(a."AfterOT",0))
		From #CheckOT a JOIN :NmlNightDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where Ifnull(b."NIsHoliday",'N') = 'Y' AND Ifnull(a."AfterOT",0) > 0 AND Ifnull(a."CalcAfterOT",'N') = 'Y'
	UNION All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 2.1),(Ifnull(a."AfterOT",0))
		From #CheckOT a JOIN :NmlNightDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where Ifnull(b."NIsWeekend",'N') = 'Y' And Ifnull(b."NIsHoliday",'N') = 'N' AND Ifnull(a."AfterOT",0) > 0 AND Ifnull(a."CalcWKOT",'N') = 'Y'
	UNION All
	Select :Period as "Period",a."empID",a."DateOnly",
		 (Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 1.5),Ifnull(a."BeforeOT",0)
		From #CheckOT a JOIN :NmlNightDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where Ifnull(b."NIsWeekend",'N') = 'Y' And Ifnull(b."NIsHoliday",'N') = 'N' AND Ifnull(a."BeforeOT",0) > 0 AND Ifnull(a."CalcWKOT",'N') = 'Y';
	
	----Get Weekend DayShift --Ngày cuối tuần
	WkndDay = Select a."empID", a."DateOnly"
	From #CheckOT a 
	Where IFNULL(a."DWorkHour",0) > 0 AND IFNULL("IsWeekend",'N')='Y' AND IFNULL(a."CalcWKOT",'N')='Y' AND IFNULL("IsHoliday",'N')='N';
	
	Insert into #tblOverTimeTemp ("Period", "empID", "WorkDay", "OvertimeType", "OvertimeHour")
	Select :Period as "Period",a."empID",a."DateOnly",
		a."WKOTWCode", (IFNULL(a."AfterOT",0) + IFNULL(a."BeforeOT",0)) + Case When IFNULL(a."DMinutes",0) >= IFNULL(a."DWorkMinutes",0) Then IFNULL(a."DWorkMinutes",0) Else IFNULL(a."DMinutes",0) End
		From #CheckOT a JOIN :WkndDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly" Where a."EndTime" != 2200 And IFNULL(a."IsNightShift",'N')='N';
	
	WkdBADay = Select a.* From #CheckOT a JOIN :WkndDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly" And a."EndTime" = 2200 And IFNULL(a."IsNightShift",'N')='N';
	INSERT InTo #tblOverTimeTemp  ("Period", "empID", "WorkDay", "OvertimeType", "OvertimeHour")
	Select  :Period as "Period",a."empID",a."DateOnly",
		a."WKOTWCode",Case When IFNULL(a."DMinutes",0) >= IFNULL(a."DWorkMinutes",0) Then IFNULL(a."DWorkMinutes",0) Else IFNULL(a."DMinutes",0) End --IFNULL(a."BeforeOT",0) + 
		From #CheckOT a JOIN :WkdBADay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly" Where IFNULL(a."BeforeOT",0) != 0
	Union All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 2.7),(a."AfterOT")
		From #CheckOT a JOIN :WkdBADay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly" Where IFNULL(a."AfterOT",0) <= 120
	Union All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 2.1),(IFNULL(a."AfterOT",0)-120)
		From #CheckOT a JOIN :WkdBADay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
			LEFT JOIN #tempNextDay c On a."empID" = c."empID" And ADD_DAYS(a."DateOnly",1) = c."ddate"
		Where IFNULL(a."AfterOT",0) > 120 And IFNULL(c."IsHoliday",'N') = 'N' And IFNULL(c."IsWeekend",'N') = 'N';

	--Weekend NightShift
	WkdBANight = Select a.*,c."IsHoliday" as "NIsHoliday", c."IsWeekend" as "NIsWeekend"
		From #CheckOT a JOIN :WkndDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
						LEFT JOIN #tempNextDay c On a."empID" = c."empID" And  ADD_DAYS(a."DateOnly",1) = c."ddate"
		Where IFNULL(a."IsNightShift",'N')='Y';
	Insert InTo #tblOverTimeTemp ("Period", "empID", "WorkDay", "OvertimeType", "OvertimeHour")
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 2.7), IFNULL(a."BeforeOT",0) + Case When IFNULL(a."DMinutes",0) >= IFNULL(a."DWorkMinutes",0) Then IFNULL(a."DWorkMinutes",0) Else IFNULL(a."DMinutes",0) End
		From #CheckOT a JOIN :WkdBANight b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
	--UNION All
	--Select :Period as "Period",a."empID",a."DateOnly",
	--	a."WDOTWCode",IFNULL(a."BeforeOT",0)
	--	From #CheckOT a JOIN :WkdBANight b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
	--	Where IFNULL(a."BeforeOT",0) > 0
	UNION All
	Select :Period as "Period",a."empID",a."DateOnly",
		a."WKOTWCode", (a."AfterOT")
		From #CheckOT a JOIN :WkdBANight b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where IFNULL(b."NIsWeekend",'N') = 'Y' And a."AfterOT" > 0
	UNION All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 1.5),(a."AfterOT")
		From #CheckOT a JOIN :WkdBANight b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where IFNULL(b."NIsHoliday",'N') = 'N' And a."AfterOT" > 0
	UNION All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 3),(a."AfterOT")
		From #CheckOT a JOIN :WkdBANight b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where IFNULL(b."NIsHoliday",'N') = 'Y' And a."AfterOT" > 0;	
	
	----Get Holiday DayShift
	HldDay = Select a."empID", a."DateOnly"
	From #CheckOT a 
	Where IFNULL(a."DWorkHour",0) != 0 AND IFNULL(a."CalcHLOT",'N')='Y' AND IFNULL("IsHoliday",'N')='Y';
	Insert InTo #tblOverTimeTemp ("Period", "empID", "WorkDay", "OvertimeType", "OvertimeHour")
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 3),Case When IFNULL(a."DMinutes",0) >= IFNULL(a."DWorkMinutes",0) Then IFNULL(a."DWorkMinutes",0) Else IFNULL(a."DMinutes",0) End --(IFNULL(a."AfterOT",0) + IFNULL(a."BeforeOT",0)) + 
		From #CheckOT a JOIN :HldDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly" Where a."EndTime" != 2200  And IFNULL(a."IsNightShift",'N')='N';
	
	HldBADay = Select a.* From #CheckOT a JOIN :HldDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly" And a."EndTime" = 2200 And IFNULL(a."IsNightShift",'N')='N';
	
	Insert InTo #tblOverTimeTemp ("Period", "empID", "WorkDay", "OvertimeType", "OvertimeHour")
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 3),Case When IFNULL(a."DWorkMinutes",0) >= IFNULL(a."DMinutes",0) Then IFNULL(a."DMinutes",0) Else IFNULL(a."DWorkMinutes",0) End --IFNULL(a."BeforeOT",0) + 
		From #CheckOT a JOIN :HldBADay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly" Where IFNULL(a."BeforeOT",0) != 0
	Union All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 3.9),(a."AfterOT")
		From #CheckOT a JOIN :HldBADay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly" Where IFNULL(a."AfterOT",0) > 0 And IFNULL(a."AfterOT",0) <=120
	Union All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 2.1),(IFNULL(a."AfterOT",0)-120)
		From #CheckOT a JOIN :HldBADay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
			LEFT JOIN #tempNextDay c On a."empID" = c."empID" And a."DateOnly" = ADD_DAYS(c."ddate",1)
		Where IFNULL(a."AfterOT",0) > 120 And IFNULL(c."IsHoliday",'N') = 'N' And IFNULL(c."IsWeekend",'N') = 'N' and IFNULL(a."AfterOT",0) > 120
	Union All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 2.7),(IFNULL(a."AfterOT",0)-120)
		From #CheckOT a JOIN :HldBADay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
			LEFT JOIN #tempNextDay c On a."empID" = c."empID" And a."DateOnly" = ADD_DAYS(c."ddate",1)
		Where IFNULL(a."AfterOT",0) > 120 And IFNULL(c."IsHoliday",'N') = 'N' And IFNULL(c."IsWeekend",'N') = 'Y';
	
	--Holiday NightShift
	HldBANight = Select a.*,c."IsHoliday" as "NIsHoliday", c."IsWeekend" as "NIsWeekend"
		From #CheckOT a JOIN :HldDay b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
						LEFT JOIN #tempNextDay c On a."empID" = c."empID" And a."DateOnly" = ADD_DAYS(c."ddate",1)
		Where IFNULL(a."IsNightShift",'N')='Y';
	Insert InTo #tblOverTimeTemp ("Period", "empID", "WorkDay", "OvertimeType", "OvertimeHour")
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 3.9), Case When IFNULL(a."DMinutes",0) >= IFNULL(a."DWorkMinutes",0) Then IFNULL(a."DWorkMinutes",0) Else IFNULL(a."DMinutes",0) End
		From #CheckOT a JOIN :HldBANight b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
	UNION All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 3),IFNULL(a."BeforeOT",0)
		From #CheckOT a JOIN :HldBANight b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where IFNULL(a."BeforeOT",0) > 0
	UNION All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 2), (a."AfterOT")
		From #CheckOT a JOIN :HldBANight b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where IFNULL(b."NIsWeekend",'N') = 'Y' And IFNULL(b."NIsHoliday",'N') = 'N' And a."AfterOT" > 0
	UNION All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 1.5),(a."AfterOT")
		From #CheckOT a JOIN :HldBANight b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where IFNULL(b."NIsWeekend",'N') = 'N' And IFNULL(b."NIsHoliday",'N') = 'N' And a."AfterOT" > 0
	UNION All
	Select :Period as "Period",a."empID",a."DateOnly",
		(Select "Code" From APZ_OHOT Where "Branch" = a."branch" And "Rate" = 3),(a."AfterOT")
		From #CheckOT a JOIN :HldBANight b On a."empID" = b."empID" And a."DateOnly" = b."DateOnly"
		Where IFNULL(b."NIsHoliday",'N') = 'Y' And a."AfterOT" > 0;
	
	Update a Set "OvertimeHour" = Case
			When "OvertimeHour" <= 57 Then 0
			When 58 <= "OvertimeHour" And "OvertimeHour" <= 87 Then 60
			When 88 <= "OvertimeHour" And "OvertimeHour" <= 117 Then 90
			When 118 <= "OvertimeHour" And "OvertimeHour" <= 147 Then 120
			When 148 <= "OvertimeHour" And "OvertimeHour" <= 177 Then 150
			When 178 <= "OvertimeHour" And "OvertimeHour" <= 207 Then 180
			When 208 <= "OvertimeHour" And "OvertimeHour" <= 237 Then 210
			When 238 <= "OvertimeHour" And "OvertimeHour" <= 290 Then 240
			Else "OvertimeHour" End
	From #tblOverTimeTemp a; 
	Update a Set "OvertimeHour" = ROUND("OvertimeHour"/60.0 , 1) From #tblOverTimeTemp a; 
	
	select cast(null as int) "AbsEntry", :Period "Period", "empID", "ExtEmpNo", "empName", "dept", "SchID", "ShiftID", cast(null as int) as "LineNum", "DateOnly" as "WorkDay", "CalcWorkCode" as "WorkCode"
			,"InTime", "OutTime", "InWorkTime", "OutWorkTime", "Late", "Early", "BeforeOT", "AfterOT", "IsWeekend", "IsHoliday", "DHour", "DMinutes", "DWorkHour", "DWorkMinutes"
			,"WorkRate" "WorkTotal", cast(null as nvarchar(254)) "FreeTxt", cast(null as int) as "LogInstanc"
		from #InOUtShift order by "empID","WorkDay";

	exstOT = select Distinct a."empID",a."WorkDay",a."OvertimeType" from #tblOverTimeTemp a Where IFNULL("OvertimeHour",0) > 0;
	Delete From #tblOverTimeTemp a 
	Where Exists (Select 1 From :exstOT b Where b."empID" = a."empID" And b."WorkDay" = a."WorkDay" And b."OvertimeType" = a."OvertimeType") And IFNULL("OvertimeHour",0) = 0;
	
	insert into #tblOverTime ("Period","empID","LineNum","WorkDay","OvertimeType","OvertimeHour", "empName","dept")
	select Distinct a."Period",a."empID",a."LineNum",a."WorkDay",a."OvertimeType", "OvertimeHour", b."lastName" || ' ' || b."firstName" "empName",b."dept"
			from #tblOverTimeTemp a left join OHEM b ON a."empID" = b."empID" Where IFNULL("OvertimeHour",0) > 0
	order by a."empID",a."WorkDay";
	
	select a.*, b."ExtEmpNo" from #tblOverTime a left join OHEM b ON a."empID" = b."empID";
	
	Drop Table #temp_Schedule;
	Drop Table #TEMP_EMPLOYEE;
	Drop Table #NUMBER;
	Drop Table #TEMPDATE;
	Drop Table #FULLDATE;
	Drop Table #TEMP_EMPSCHEDULE;
	Drop Table #TBLATTENDDRAFT;
	Drop Table #TBLIN;
	Drop Table #TBLOUT;
	Drop Table #TBLINOUT;
	Drop Table #INOUTSHIFT;
	Drop Table #TBLWORKCODE;
	Drop Table #TBLOVERTIMETEMP;
	Drop Table #TBLOVERTIME;
	Drop Table #CheckOT;
	Drop Table #tempNextDay;
	Drop Table #NRMLDAY;

END;
