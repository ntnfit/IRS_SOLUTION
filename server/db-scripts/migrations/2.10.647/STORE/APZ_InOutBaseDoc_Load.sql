
CALL APZ_EXISTSPROC('APZ_InOutBaseDoc_Load');

GO

CREATE PROCEDURE APZ_InOutBaseDoc_Load(in AbsEntry int default 0)
AS 
BEGIN
	declare Period nvarchar(50);
	declare Branch int;
	DECLARE StartDate timestamp;
	DECLARE EndDate timestamp;
	DECLARE HourPerDay int;
	DECLARE sqlString nvarchar(4000000) = '';
	
	if(ifnull(AbsEntry,0) = 0) then 
		return;
	end if;
	
	select top 1  "Period","Branch" into Period, Branch from APZ_OTBD where "AbsEntry" = :AbsEntry;

	select IFNULL((SELECT a."FromDate" FROM APZ_OTBD a WHERE a."AbsEntry"=:AbsEntry),'1900-01-01') into StartDate from DUMMY;
	select IFNULL((SELECT a."ToDate" FROM APZ_OTBD a WHERE a."AbsEntry"=:AbsEntry),'1900-01-01') into EndDate from DUMMY;
	select ifnull((select "HourPerDay" from APZ_OHWP where "PeriodCode" = :Period and "Branch" = :Branch limit 1),8) into HourPerDay from DUMMY;

	/*Lấy ra bảng tổng hợp thông tin nhân viên*/
	--create local temporary table #temp_Employee("empID" int, "ExtEmpNo" nvarchar(50), "empName" nvarchar(254), "dept" int, "Department" nvarchar(254), "EnrollNumber" int, "MachineNo" int);
	--insert into #temp_Employee
	--	SELECT a."empID",a."ExtEmpNo",a."firstName" || ',' || a."lastName",a."dept",b."Name",c."EnrollNo",c."MachineNo"
	--		FROM OHEM a LEFT JOIN APZ_OTEM c on a."empID" = c."empNo"
	--					LEFT JOIN OUDP b ON a."dept"=b."Code"
	--				WHERE a."branch"=:Branch AND a."Active"='Y' and c."EnrollNo" is not null and c."empNo" is not null;
	
	create local temporary table #temp_Employee("empID" int, "ExtEmpNo" nvarchar(50), "empName" nvarchar(254), "dept" int, "Department" nvarchar(254), "EnrollNumber" int, "MachineNo" int);
	If Exists(Select 1 FROM SYS.TABLE_COLUMNS Where  SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'OHEM' AND Column_Name = 'U_FullWorkCode') Then
		sqlString = 'insert into #temp_Employee
			SELECT a."empID",a."ExtEmpNo",a."firstName" || '','' || a."lastName",a."dept",b."Name",c."EnrollNo",c."MachineNo"
				FROM OHEM a LEFT JOIN APZ_OTEM c on a."empID" = c."empNo"
							LEFT JOIN OUDP b ON a."dept"=b."Code"
						WHERE a."branch"=''' || :Branch || ''' AND a."Active"=''Y'' And IFNULL(a."U_FullWorkCode",'''') = ''Y''
			UNION 
			SELECT a."empID",a."ExtEmpNo",a."firstName" || '','' || a."lastName",a."dept",b."Name",c."EnrollNo",c."MachineNo"
				FROM OHEM a LEFT JOIN APZ_OTEM c on a."empID" = c."empNo"
							LEFT JOIN OUDP b ON a."dept"=b."Code"
						WHERE a."branch"=''' || :Branch || ''' AND a."Active"=''Y'' and c."EnrollNo" is not null and c."empNo" is not null;';
	 	EXECUTE immediate :sqlString;
	Else
		insert into #temp_Employee
		SELECT a."empID",a."ExtEmpNo",a."firstName" || ',' || a."lastName",a."dept",b."Name",c."EnrollNo",c."MachineNo"
			FROM OHEM a LEFT JOIN APZ_OTEM c on a."empID" = c."empNo"
						LEFT JOIN OUDP b ON a."dept"=b."Code"
					WHERE a."branch"= Ifnull(:Branch,0) AND a."Active"='Y' and c."EnrollNo" is not null and c."empNo" is not null;
	End if;
	
	/*Lấy ra thông tin lịch trình làm việc cho từng nhân viên theo khoảng thời gian trong kì chấm công*/
	create local temporary table #temp_Schedule("empID" int, "ExtEmpNo" nvarchar(50), "empName" nvarchar(254), "dept" int, "Department" nvarchar(254), "EnrollNumber" int, "MachineNo" int
												, "SchID" int, "ValidFrom" timestamp, "ValidTo" timestamp);
												
	insert into #temp_Schedule
		SELECT a.* ,d."SchID",d."ValidFrom",d."ValidTo"
			from #temp_Employee a LEFT JOIN APZ_OHWP h ON h."PeriodCode" =:period and h."Branch" = :Branch
								LEFT JOIN APZ_OWSR d on d."empID" = a."empID" 
													and ((d."ValidFrom" <= h."FromDate" and ifnull(d."ValidTo",h."ToDate") >= h."ToDate") 
														or (d."ValidFrom" between h."FromDate" and h."ToDate") 
														or ((d."ValidTo" is not null and d."ValidTo" between h."FromDate" and h."ToDate") 
														or (d."ValidTo" is null and d."ValidFrom" between h."FromDate" and h."ToDate")));
	
	/*Lấy ra bảng chứa thông tin các ngày trong khoảng kì chấm công*/
	Create local temporary table #Number (GENERATED_PERIOD_START int);
	
	Insert Into #Number
		SELECT GENERATED_PERIOD_START FROM SERIES_GENERATE_DECIMAL(1, 0, DAYS_BETWEEN(:StartDate,:EndDate) + 1);
		
	Create local temporary table #tempDate ("ddate" timestamp null);
	Insert Into #tempDate
		SELECT DISTINCT CAST(ADD_DAYS(:StartDate, a.GENERATED_PERIOD_START) AS timestamp) FROM #Number A;

	create local temporary table #FullDate as(SELECT A.*, mod(WEEKDAY("ddate") + 1, 7) + 1 AS "wkDay", DAYOFMONTH("ddate") AS "mthDay", DAYOFYEAR("ddate") AS "yrDay" FROM #tempDate A);

	/*Lấy ra các thông tin liên quan đến lịch trình làm việc như thông tin 
	- CÁCH CHỌN GIỜ VÀO/RA
	- THÔNG TIN NGÀY NGHỈ LỄ
	- CHU KÌ CỦA LỊCH TRÌNH (Theo Tuần, Tháng, Năm)
	*/
	create local temporary table #temp_EmpSchedule as(
		select  a.*,b.*,e."CycleMode"
		--,case when e.CycleMode = 'W' then f.ShiftID when e.CycleMode = 'M' then f2.ShiftID when e.CycleMode = 'Y' then f3.ShiftID else null end as ShftID
		,k."InOutMethod",k."MinInterval",k."MaxInterval",k."BetweenInterval",l."WndFrm",l."WndTo"
		, case when exists(select * from APZ_HLD1 ld1 where (ld1."isCurYear" = 'Y' and "ddate" between ld1."StrDate" and ld1."EndDate") 
							or (ld1."isCurYear" = 'N' and DAYOFYEAR("ddate") between DAYOFYEAR(ld1."StrDate") and DAYOFYEAR(ld1."EndDate"))) then 'Y' else 'N' end "IsHoliday"
		, case when cast(l."WndFrm" as int) <= cast(l."WndTo" as int) then (case when a."wkDay" between cast(l."WndFrm" as int) and cast(l."WndTo" as int) then 'Y' else 'N' end) 
				else (case when a."wkDay" >= cast(l."WndFrm" as int) or a."wkDay" <= cast(l."WndTo" as int) then 'Y' else 'N' end) end "IsWeekend"
			from #FullDate a LEFT JOIN #temp_Schedule b on a."ddate" between b."ValidFrom" and b."ValidTo"
							LEFT JOIN APZ_OWSC e on e."SchID" = b."SchID"
							LEFT JOIN APZ_OIOM k on k."MethodId" = e."InOutID"
							LEFT JOIN APZ_OHLD l on l."HldCode" = e."HldCode" order by "EnrollNumber","ddate");
							
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
			and ifnull((CAST(b."DateTimeRecord" as Date)),T0."ddate") between :StartDate and :EndDate 
		Order By Cast(b."DateTimeRecord" as date),b."EnrollNo";

	--Khi đã có dữ liệu chấm công ở 1 máy bất kì thì xóa dữ liệu ở các máy có dữ liệu = null
	delete from #tblAttendDraft x where "DateTimeRecord" is null and exists(select 1 from #tblAttendDraft y where y."empID" = x."empID" and y."ddate" = x."ddate" and y."DateTimeRecord" is not null);
	--Khi không có dữ liệu chấm công ở bất kỳ mày chấm công nào thì xóa dữ liệu null chỉ để lại 1 máy bất kì
	delete from #tblAttendDraft x where "DateTimeRecord" is null and "MachineNo" <> (select min("MachineNo") from #tblAttendDraft y where y."empID" = x."empID" and y."ddate" = x."ddate") 
											and ifnull((select count(1) from #tblAttendDraft y where y."empID" = x."empID" and y."ddate" = x."ddate"),0) > 1;
	
	/*Lấy ra bước nhảy tủy theo InOutMethod. Bước này mục đích để lọc VÀO - RA theo InOutMethod.*/
	create local temporary table #tblAttendance as (
		select *, ROW_NUMBER() OVER (PARTITION BY "empID",Cast("ddate" as Timestamp) ORDER BY IFNULL("InOutTime",0) Asc) As "RowNumber",
			Count(*) OVER (PARTITION BY "empID",Cast("ddate" As Timestamp) ORDER BY "empID" Asc) As "MaxRowNum",
			ROW_NUMBER() OVER (PARTITION BY "empID",Cast("ddate" As Timestamp) ORDER BY IFNULL("InOutTime",0) Asc) As "PunchType" 
		From #tblAttendDraft);
	--where DateTimeRecord is not null
	UPDATE a 
	SET "PunchType" = CASE a."InOutMethod" 
					-- Ra vào xen kẽ
					WHEN '1' THEN "RowNumber" 
					-- Theo khoảng phân giờ (Tạm thời chưa dùng)
					WHEN '2' THEN "RowNumber" 
					--Theo số máy chấm công
					WHEN '3' THEN Case When a."RowNumber" = (select min(x."RowNumber") from #tblAttendance x where x."empID" = a."empID" and x."DateOnly" = a."DateOnly" and MOD(Ifnull(x."MachineNo",0),2) = 1) then 1
						when a."RowNumber" = (select max(x."RowNumber") from #tblAttendance x where x."empID" = a."empID" and x."DateOnly" = a."DateOnly" and MOD(Ifnull(x."MachineNo",0),2) = 0) then 2 else 0 end
					-- Giờ chấm vào đầu tiên là giờ vào, giờ ra cuối cùng tính là giờ ra
					WHEN '4' THEN Case When a."RowNumber" = 1 Then 1 When a."RowNumber" = a."MaxRowNum" Then 2 Else 0 End
					-- Theo trạng thái check in/check out trên máy chấm công
					WHEN '5' THEN Case When a."RowNumber" = (Select min(x."RowNumber") From #tblAttendance x Where x."empID" = a."empID" And x."DateOnly" = a."DateOnly" And Ifnull(x."OriginType", 0) = 0) Then 1
						when a."RowNumber" = (select max(x."RowNumber") from #tblAttendance x where x."empID" = a."empID" and x."DateOnly" = a."DateOnly" and Ifnull(x."OriginType", 0) = 1) then 2 else 0 end 
					Else 1 End 
	FROM #tblAttendance a;

	/*Lọc ra bảng chứa các giờ VÀO*/
	Create local temporary table #tblIn as 
	(
		SELECT * FROM #tblAttendance WHERE MOD("PunchType", 2) = 1 AND "PunchType" <> 0 AND "empID" IS NOT NULL
	);
	/*Lọc ra bảng chứa các giờ RA*/
	Create local temporary table #tblOut as 
	(
		SELECT * FROM #tblAttendance WHERE MOD("PunchType", 2) = 0 AND "PunchType" <> 0 AND "empID" IS NOT NULL
	);
	/* Từ bảng dữ liệu VÀO và bảng dữ liệu ngày trong khoảng thời gian chấm công lọc ra thông tin ca làm việc. Sau đó cập nhật thêm thông tin thời gian ra từ bảng RA
		Trường hợp những ngày không có thông tin chấm công thì cập nhật default theo giá trị top 1 ở bảng Lịch trình làm việc
	*/
	--Insert data những ngày không nằm trong bảng dữ liệu VÀO
	Create local temporary table #tmpIn as 
	(
		Select a.* From #tblAttendance a LEFT JOIN #tblIn b On a."empID" = b."empID" AND a."ddate" = b."ddate" Where b."ddate" Is Null And b."DateOnly" Is Null And a."RowNumber" = 1
	);
	
	Update #tmpIn Set "DateTimeRecord" = Ifnull("DateTimeRecord",'1900-01-01'),
				"ClockDate" = Ifnull("ClockDate",'1900-01-01'),
				"OriginType" = Ifnull("OriginType",0),
				"Source" = Ifnull("Source",0),
				"InOutString" = Ifnull("InOutString",''),
				"InOutTime" = Ifnull("InOutTime",0),
				"DateOnly" =Ifnull("DateOnly",'1900-01-01'),
				"Increase" = Ifnull("Increase",0),
				"WorkTime" = Ifnull("WorkTime",0),
				"MachineNo" = Ifnull("MachineNo",0);
	--Insert Into #tblIn Select * From #tmpIn;
	
	create local temporary table #tblInOut as(
		SELECT T1.*,
			T1."InOutTime" "InTime",
			T1."InOutTime" "OutTime",
			T1."WorkTime" "InWorkTime",
			T1."WorkTime" "OutWorkTime",
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
			TO_VARCHAR(T1."DateTimeRecord", 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast(T1."WorkTime" as nvarchar),4),2) || ':' || RIGHT(cast(T1."WorkTime" as nvarchar),2) "DateTimeOut", 
			Cast(null as nvarchar(250)) "CalcWorkCode", 
			Cast(null as nvarchar(20)) "TotalWorkCode"
		FROM #tblIn T1);

	/*Chọn ca làm việc dựa vào dữ liệu workSchedule*/
	---Trường hợp thời gian InOut nằm trong khoảng thời gian ca làm việc của WorkSchedule
	create local temporary table #temp_Shift as(
		SELECT a.*, e."RoundUnit", 
        Cast(( CASE 
            WHEN e."CycleMode" = 'W' THEN (SELECT Cast(f."ShiftID" AS Int)
	            FROM APZ_WSC1 f LEFT OUTER JOIN APZ_OEWS b ON Cast(f."ShiftID" AS Int) = b."ShiftID" 
	            WHERE f."DayID" = a."wkDay" AND e."SchID" = f."SchID" AND e."CalcType" = '1' 
	            	AND (IFNULL(a."InTime", 0) <> 0 AND (a."InTime" BETWEEN b."StartTimeIn" AND b."EndTimeIn"))) 
            WHEN e."CycleMode" = 'M' THEN (SELECT Cast(f2."ShiftID" AS Int) 
            	FROM APZ_WSC2 f2 LEFT OUTER JOIN APZ_OEWS b ON Cast(f2."ShiftID" AS Int) = b."ShiftID"
           		WHERE f2."DayID" = a."mthDay" AND e."SchID" = f2."SchID" AND e."CalcType" = '1' 
           			AND (IFNULL(a."InTime", 0) <> 0 AND (a."InTime" BETWEEN b."StartTimeIn" AND b."EndTimeIn"))) 
            WHEN e."CycleMode" = 'Y' THEN (SELECT Cast(f3."ShiftID" AS Int)
            	FROM APZ_WSC3 f3 LEFT OUTER JOIN APZ_OEWS b ON Cast(f3."ShiftID" AS Int) = b."ShiftID"
           		WHERE f3."DayID" = a."yrDay" AND e."SchID" = f3."SchID" AND e."CalcType" = '1' 
           			AND (IFNULL(a."InTime", 0) <> 0 AND (a."InTime" BETWEEN b."StartTimeIn" AND b."EndTimeIn"))) 
            ELSE NULL 
        END) As Int) AS "ShftID" 
    	FROM #tblInOut a LEFT OUTER JOIN APZ_OWSC e ON e."SchID" = a."SchID");
	
	/* Cập nhật data top 1 cho những trường hợp không lấy thông tin ca theo giờ check in 
	Case have Intime Is Not Null and Intime Is Null */
	Update a Set "ShftID" = Case When e."CycleMode" = 'W' Then (Select Cast(f."ShiftID" AS Int)
								From (Select *,ROW_NUMBER() OVER (PARTITION BY "ShiftID") As "Rn" From APZ_WSC1) f LEFT JOIN APZ_OEWS b on f."ShiftID" = b."ShiftID"
								Where f."DayID" = a."wkDay" and a."SchID" = f."SchID" And e."CalcType" = '1' And (Ifnull(a."InTime",0) != 0  And a."InTime" < "StartTimeIn") And f."Rn" = 1)
							When e."CycleMode" = 'M' Then (Select Cast(f."ShiftID" AS Int)
								From (Select *,ROW_NUMBER() OVER (PARTITION BY "ShiftID") As "Rn" From APZ_WSC2) f LEFT JOIN APZ_OEWS b on f."ShiftID" = b."ShiftID"
								Where f."DayID" = a."mthDay" and a."SchID" = f."SchID" And e."CalcType" = '1' And (Ifnull(a."InTime",0) != 0  And a."InTime" < "StartTimeIn") And f."Rn" = 1)
							Else (Select Cast(f."ShiftID" AS Int)
								From (Select *,ROW_NUMBER() OVER (PARTITION BY "ShiftID") As "Rn" From APZ_WSC3) f LEFT JOIN APZ_OEWS b on f."ShiftID" = b."ShiftID"
								Where f."DayID" = a."yrDay" and a."SchID" = f."SchID" And e."CalcType" = '1' And (Ifnull(a."InTime",0) != 0  And a."InTime" < "StartTimeIn") And f."Rn" = 1) End
	From #temp_Shift a, APZ_OWSC e
	Where a."SchID" = e."SchID" And Ifnull(Cast(a."ShftID" AS Int),0) = 0 And IFNULL(a."InTime",0) != 0;
	
	Update a Set "ShftID" = Case When b."CycleMode" = 'W' Then (Select Cast(f."ShiftID" AS Int) From (Select *,ROW_NUMBER() OVER (PARTITION BY "ShiftID") As "Rn" From APZ_WSC1) f Where f."SchID" = a."SchID" And f."Rn" = 1)
							When b."CycleMode" = 'M' Then (Select Cast(f."ShiftID" AS Int) From (Select *,ROW_NUMBER() OVER (PARTITION BY "ShiftID") As "Rn" From APZ_WSC2) f Where f."SchID" = a."SchID" And f."Rn" = 1)
							Else (Select Cast(f."ShiftID" AS Int) From (Select *,ROW_NUMBER() OVER (PARTITION BY "ShiftID") As "Rn" From APZ_WSC3) f Where f."SchID" = a."SchID" And f."Rn" = 1) End
	From #temp_Shift a, APZ_OWSC b
	Where a."SchID" = b."SchID" And Ifnull(Cast(a."ShftID" AS Int),0) = 0;
    
    /*Update lại data Out: Case IsNightShift and not IsNightShift */
	Update a Set "OutTime" = b."InOutTime", "OutWorkTime" = b."WorkTime",
		"DateTimeOut" = TO_VARCHAR(b."DateTimeRecord", 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast(b."WorkTime" as nvarchar),4),2) || ':' || RIGHT(cast(b."WorkTime" as nvarchar),2)
	From #temp_Shift a, #tblOut b, APZ_OEWS c
	Where a."empID" = b."empID" And a."ddate" = b."ddate" And a."ShftID" = c."ShiftID" And IFNULL(c."IsNightShift",'N') = 'N';
	Update a Set "OutTime" = b."InOutTime", "OutWorkTime" = b."WorkTime",
		"DateTimeOut" = TO_VARCHAR(b."DateTimeRecord", 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast(b."WorkTime" as nvarchar),4),2) || ':' || RIGHT(cast(b."WorkTime" as nvarchar),2)
	From #temp_Shift a, #tblOut b, APZ_OEWS c
	Where a."empID" = b."empID" And a."ddate" = ADD_DAYS(b."ddate",1)  And a."ShftID" = c."ShiftID" And IFNULL(c."IsNightShift",'N') = 'Y';
	--Update a Set "InTime" = Cast(null as Int), "OutTime" = Cast(null as Int),"InWorkTime" = Cast(null as Int), "OutWorkTime" = Cast(null as Int) 
	--From #temp_Shift a Where IFNULL(a."IsWeekend",'N') = 'Y' And (a."InTime" Is Null Or a."OutTime" Is Null);
	
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
		from #temp_Shift a LEFT JOIN APZ_OEWS b on b."ShiftID" = a."ShftID"
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
												when ("InTime" < "StartTime" and "OutTime" < "StartTime") or ("InTime" > "EndTime" and "OutTime" > "EndTime") then null else "InTime" end
							,"OutWorkTime" = case when ("InTime" < "StartTime" and "OutTime" < "StartTime") or ("InTime" > "EndTime" and "OutTime" > "EndTime") then null 
													when ("OutTime" > "EndTime" and "InTime" <= "EndTime") then "EndTime" else "OutTime" end
						where "OutTime" is not null and "StartTime" is not null and "EndTime" is not null;
	Update #InOutShift 
	Set "InWorkTime" = case when ("InTime" < "StartTime" AND ((LENGTH("OutTime") = 3 AND "OutTime" < "StartTime") OR (LENGTH("OutTime") = 4 AND "OutTime" >= "StartTime"))) then "StartTime" 
						when ("InTime" < "StartTime" AND LENGTH("OutTime") = 4 AND "OutTime" < "StartTime") or (LENGTH("InTime") = 3 AND "InTime" > "EndTime" AND "OutTime" > "EndTime") then null 
						else "InTime" end
		,"OutWorkTime" = case when ("InTime" < "StartTime" AND LENGTH("OutTime") = 4 AND "OutTime" < "StartTime") or (LENGTH("InTime") = 3 AND "InTime" > "EndTime" AND "OutTime" > "EndTime") then null
						when ("OutTime" > "EndTime" and ((LENGTH("InTime") = 3 AND "InTime" <= "EndTime") Or LENGTH("InTime") = 4)) then "EndTime" else "OutTime" end 
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
		,"BeforeOT" = (case when ifnull("CalcBeforeOT",'N') = 'Y' then (case when SECONDS_BETWEEN("InWorkTimeDate","StartTimeDate") / 60 - "MinBeforeDuty" > 0 
																	then  SECONDS_BETWEEN("InWorkTimeDate","StartTimeDate") / 60 - "MinBeforeDuty" else 0 end ) else 0 end)
		--Tính thời gian OT sau giờ làm việc
		,"AfterOT" = (case when ifnull("CalcAfterOT",'N') = 'Y' then (case when SECONDS_BETWEEN("EndTimeDate","OutWorkTimeDate") / 60 - "MinAfterDuty" > 0
																	then SECONDS_BETWEEN("EndTimeDate","OutWorkTimeDate") / 60 - "MinAfterDuty" else 0 end ) else 0 end);
	
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
		
		--Số giờ thực tế giữa vào - ra
		,"DHour" = case when ("StartBreak" is not null and "EndBreak" is not null) AND ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") then (
			--Trừ đi khoảng thời gian ăn trưa khi thời gian In Out có chứa thời gian nghỉ ăn trưa
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "EndTimeDate")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("StartTimeDate", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("StartTimeDate", "EndTimeDate") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /3600.0
			else SECONDS_BETWEEN("StartTimeDate", "EndTimeDate")/3600.0 end
		-- Số phút thực tế giữa vào - ra
		,"DMinutes" = case when ("StartBreak" is not null and "EndBreak" is not null) AND ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") then (
			--Trừ đi khoảng thời gian ăn trưa khi thời gian In Out có chứa thời gian nghỉ ăn trưa
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "EndTimeDate")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("StartTimeDate", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("StartTimeDate", "EndTimeDate") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /60.0
			else SECONDS_BETWEEN("StartTimeDate", "EndTimeDate")/60.0 end
	
		--Số giờ làm việc thực tế
		,"DWorkHour" = case when ("StartBreak" is not null and "EndBreak" is not null) AND ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") then (
			--Trừ đi khoảng thời gian ăn trưa khi thời gian In Out có chứa thời gian nghỉ ăn trưa
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "DateTimeOut")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("DateTimeIn", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /3600.0
			else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut")/3600.0 end
		-- Số phút làm việc thực tế
		,"DWorkMinutes" = case when ("StartBreak" is not null and "EndBreak" is not null) AND ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") then (
			--Trừ đi khoảng thời gian ăn trưa khi thời gian In Out có chứa thời gian nghỉ ăn trưa
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "DateTimeOut")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("DateTimeIn", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /60.0
			else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut")/60.0 end
	where "InWorkTime" is not null and "OutWorkTime" is not null;
	
	--UPDATE lại các thông tin trong bảng tổng hợp dựa vào thông tin ca của từng dòng
	update #InOUtShift set "CalcWorkCode" = "WorkCode" where "InTime" is not null or "OutTime" is not null;
	update #InOUtShift set "CalcWorkCode" = "AbsentCode" where "InTime" is null and "OutTime" is null and  Ifnull("CalcAbsent",'N') = 'Y';
	--Cập nhật thông tin WorkCode,AbsentCode những ngày nằm trong ngày nghỉ lễ
	Update a Set "CalcWorkCode" = "HldCode"
	From #InOutShift a
	Where Ifnull(a."IsHoliday",'N') = 'Y' And Ifnull(a."InTime",0) = 0 AND Ifnull(a."OutTime",0) = 0 And Ifnull(a."CalcHld",'N') = 'Y';
	Update a Set "CalcWorkCode" = null
	From #InOutShift a
	Where Ifnull(a."IsHoliday",'N') = 'Y' And Ifnull(a."InTime",0) != 0 AND Ifnull(a."OutTime",0) != 0 And Ifnull(a."CalcHld",'N') = 'Y';
	
	---Delete thông tin WorkCode, AbsentCode những ngày nằm trong trường hợp là nhân viên mới hoặc nhân viên vừa nghỉ việc	
	UPDATE a SET "CalcWorkCode" = null
	From #InOutShift a
	WHERE EXISTS (SELECT 1 
	              FROM OHEM b
	              WHERE a."empID" = b."empID" AND 
	              	((b."startDate" is not null AND a."ddate" < b."startDate") Or (b."termDate" is not null AND a."ddate" > b."termDate")));
	--Cập nhật thông tin workcode không lấy mã nghỉ phép khi rơi vào ngày cuối tuần
	Update a Set "CalcWorkCode" = Null From #InOutShift a Where Ifnull(a."IsWeekend",'N') = 'Y' AND (Ifnull(a."InTime",0) = 0 OR Ifnull(a."OutTime",0) = 0) AND Ifnull(a."IsHoliday",'N') = 'N';
	
	/*Những trường hợp giờ không nằm trong khoảng thời gian của ca thì sử dụng bảng combine để set workcode*/
	IF EXISTS(SELECT 1 FROM OBJECTS WHERE OBJECT_TYPE='TABLE' AND SCHEMA_NAME = CURRENT_SCHEMA AND OBJECT_NAME='APZ_EWS1')
	Then
		Update a Set "CalcWorkCode" = Case When a."DWorkMinutes" >= b."WorkingTotal" Then b."WorkCode"
								When a."DWorkMinutes" < b."WorkingTotal" 
									And (a."InTime" Between b."StartTimeIn" And b."EndTimeIn") 
									And (a."OutTime" Between b."StartTimeOut" And b."EndTimeOut")
									Then b."WorkCode"
								Else (Select Case When IFNULL(c."WorkCode",'') != '' AND IFNULL(c."AbsentCode",'') != '' Then c."WorkCode" || ',' || c."AbsentCode"
												When IFNULL(c."WorkCode",'') = '' AND IFNULL(c."AbsentCode",'') = '' Then ''
												When IFNULL(c."WorkCode",'') = '' AND IFNULL(c."AbsentCode",'') != '' Then c."AbsentCode"
												Else c."WorkCode" End
										From APZ_EWS1 c
										Where c."ShiftID" = b."ShiftID"
											AND a."DWorkMinutes" Between c."FromMinutes" And c."ToMinutes") End
		From #InOutShift a JOIN APZ_OEWS b On a."ShiftID" = b."ShiftID"
		Where Ifnull(a."InTime",0) != 0 AND Ifnull(a."OutTime",0) != 0;
	End If;
	
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
	update  a set "TotalRate" = (select SUM("Rate") from #InOUtShift x where x."EnrollNumber" = a."EnrollNumber" and x."DateOnly" = a."DateOnly") from #InOUtShift a;
	--update #tblInOut  set WorkCode = ifnull((Select TOP 1 Code from #tblWorkCode where Rate =ROUND(#tblInOut.Rate,4) or Rate =ROUND(#tblInOut.Rate,3) or Rate =ROUND(#tblInOut.Rate,2) or Rate =ROUND(#tblInOut.Rate,1) ),'')
	--update #tblInOut set TotalWorkCode = ifnull((Select TOP 1 Code from #tblWorkCode where Rate =ROUND(#tblInOut.TotalRate,4) or Rate =ROUND(#tblInOut.TotalRate,3) or Rate =ROUND(#tblInOut.TotalRate,2) or Rate =ROUND(#tblInOut.TotalRate,1) ),'')
	
	CREATE local temporary table #tblWorkCode AS (Select top 0 a.*  From APZ_OHWC a);
	
	insert into #tblWorkCode
		select a.* from APZ_OHWC a join APZ_OHWT b on a."WorkType" = b."Id" and b."Typ" = 'W' and a."Branch" = :Branch;
	
	update #InOUtShift  set "CalcWorkCode" = ifnull((select "Code" from (Select "Code", row_number() over(order by "Rate" DESC) lineid from #tblWorkCode where "Rate" =ROUND("WorkRate",4) or "Rate" =ROUND("WorkRate",3) or "Rate" =ROUND("WorkRate",2)
																								or "Rate" =ROUND("WorkRate",1) or "Rate" < "WorkRate") xxx where xxx.lineid = 1),'')
						where "ShiftID" is not null and "IsFlexWorkCode" = 'Y' and ("InTime" is not null or "OutTime" is not null);

	create local temporary table #tblOverTimeTemp as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_IOD2 b ON 1=2);
	create local temporary table #tblOverTime as (select top 0 b.*, cast(null as nvarchar(254)) "empName", cast(null as int) "dept" from (select 1 A from DUMMY) a left join APZ_IOD2 b ON 1=2);

	insert into #tblOverTimeTemp("Period", "empID", "WorkDay", "OvertimeType", "OvertimeHour")
			select :Period, "empID","DateOnly","WDOTWCode",ROUND((("AfterOT"+"BeforeOT")/60.0),ifnull("RoundUnit",0)) 
				from #InOUtShift where ("CalcAfterOT" = 'Y' or "CalcBeforeOT" = 'Y') and "WDOTWCode" is not null and ("AfterOT" >0 or "BeforeOT">0) and "IsWeekend" != 'Y' and "IsHoliday" != 'Y'
		UNION ALL
			select :Period,"empID","DateOnly","HLOTWCode","DHour"
				from #InOUtShift where "CalcHLOT" = 'Y' and "HLOTWCode" is not null and "DHour" >0 and "IsHoliday" = 'Y'
		UNION ALL
			 select :Period,"empID", "DateOnly", "WKOTWCode","DHour"
				from #InOUtShift where "CalcWKOT" = 'Y' and "WKOTWCode" is not null and "DHour" >0 and "IsWeekend" = 'Y' and "IsHoliday" != 'Y';

	select cast(null as int) "AbsEntry", :Period "Period", "empID", "empName", "dept", "SchID", "ShiftID", cast(null as int) as "LineNum", "DateOnly" as "WorkDay", "CalcWorkCode" as "WorkCode"
			,"InTime", "OutTime", "InWorkTime", "OutWorkTime", "Late", "Early", "BeforeOT", "AfterOT", "IsWeekend", "IsHoliday", "DHour", "DMinutes", "DWorkHour", "DWorkMinutes"
			,"WorkRate" "WorkTotal", cast(null as nvarchar(254)) "FreeTxt", cast(null as int) as "LogInstanc"
		from #InOUtShift order by "empID","WorkDay";

	insert into #tblOverTime ("Period","empID","LineNum","WorkDay","OvertimeType","OvertimeHour", "empName","dept")
	select a."Period",a."empID",a."LineNum",a."WorkDay",a."OvertimeType",SUM(a."OvertimeHour") "OvertimeHour", b."firstName" || ',' || b."lastName" "empName",b."dept"
			from #tblOverTimeTemp a left join OHEM b ON a."empID" = b."empID" 
				group by a."Period",a."empID",a."LineNum",a."WorkDay",a."OvertimeType",b."firstName", b."lastName",b."dept" order by a."empID",a."WorkDay";
				
	select * from #tblOverTime;

END;
 