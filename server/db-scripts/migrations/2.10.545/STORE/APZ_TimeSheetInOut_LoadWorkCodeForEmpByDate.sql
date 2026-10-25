
CALL APZ_EXISTSPROC('APZ_TimeSheetInOut_LoadWorkCodeForEmpByDate');

GO

CREATE PROCEDURE APZ_TimeSheetInOut_LoadWorkCodeForEmpByDate
(	
	in inTime int default 0,
	in outTime int default 0,
	in period nvarchar(20) default '',
	in empId int default 0,
	in workDay timeStamp
)
AS 
BEGIN
	declare Branch int;
	DECLARE HourPerDay int;
	DECLARE sqlString nvarchar(4000000) = '';
	
	if(ifnull(inTime,0) = 0 OR ifnull(inTime,0) = 0) then 
		return;
	end if;
	
	select top 1 "Branch" into Branch from APZ_OHWP where "PeriodCode" = :period;
	select ifnull((select "HourPerDay" from APZ_OHWP where "PeriodCode" = :period and "Branch" = :Branch limit 1),8) into HourPerDay from DUMMY;

	/*Lấy ra bảng tổng hợp thông tin nhân viên*/
	create local temporary table #temp_Employee("empID" int, "ExtEmpNo" nvarchar(50), "empName" nvarchar(254), "dept" int, "Department" nvarchar(254), "EnrollNumber" int, "MachineNo" int);
	insert into #temp_Employee
		SELECT Top 1 a."empID",a."ExtEmpNo",a."firstName" || ',' || a."lastName",a."dept",b."Name",c."EnrollNo",c."MachineNo"
			FROM OHEM a LEFT JOIN APZ_OTEM c on a."empID" = c."empNo"
						LEFT JOIN OUDP b ON a."dept"=b."Code"
					WHERE a."branch"=:Branch AND a."Active"='Y' and a."empID" = :empId;
	
	/*Lấy ra thông tin lịch trình làm việc cho từng nhân viên theo khoảng thời gian trong kì chấm công*/
	create local temporary table #temp_Schedule("empID" int, "ExtEmpNo" nvarchar(50), "empName" nvarchar(254), "dept" int, "Department" nvarchar(254), "EnrollNumber" int, "MachineNo" int
												, "SchID" int, "ValidFrom" timestamp, "ValidTo" timestamp);
												
	insert into #temp_Schedule
		SELECT a.* ,d."SchID",d."ValidFrom",d."ValidTo"
			from #temp_Employee a JOIN APZ_OHWP h ON h."PeriodCode" =:period and h."Branch" = :Branch
								 JOIN APZ_OWSR d on d."empID" = a."empID" 
								 	and ((d."ValidFrom" <= h."FromDate" and ifnull(d."ValidTo",h."ToDate") >= h."ToDate") 
														or (d."ValidFrom" between h."FromDate" and h."ToDate") 
														or ((d."ValidTo" is not null and d."ValidTo" between h."FromDate" and h."ToDate") 
														or (d."ValidTo" is null and d."ValidFrom" between h."FromDate" and h."ToDate")));
	
	Create local temporary table #tempDate ("ddate" timestamp null);
	Insert Into #tempDate SELECT :workDay FROM Dummy;
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
		
	create local temporary table #tblInOut as(
		SELECT a.*,
			Cast(null as int) As "InTime",
			Cast(null as int) As "OutTime",
			Cast(null as int) As "InWorkTime",
			Cast(null as int) As "OutWorkTime",
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
			Cast(null as nvarchar(50)) "DateTimeIn",
			Cast(null as nvarchar(50)) "DateTimeOut", 
			Cast(null as nvarchar(250)) "CalcWorkCode", 
			Cast(null as nvarchar(20)) "TotalWorkCode"
		FROM #temp_EmpSchedule a);
		
	Update #tblInOut Set "InTime" = Cast(:inTime as int),
			"OutTime" = Cast(:outTime as int),
			"InWorkTime" = Cast(:inTime as int),
			"OutWorkTime" = Cast(:outTime as int),
			"DateTimeIn" = TO_VARCHAR(:workDay,'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast(:inTime as nvarchar),4),2) || ':' || RIGHT(cast(:inTime as nvarchar),2),
			"DateTimeOut" = TO_VARCHAR(:workDay, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast(:outTime as nvarchar),4),2) || ':' || RIGHT(cast(:outTime as nvarchar),2);
	
	/*Chọn ca làm việc dựa vào dữ liệu workSchedule*/
	---Trường hợp thời gian InOut nằm trong khoảng thời gian ca làm việc của WorkSchedule
	create local temporary table #temp_Shift as(
		SELECT a.*, e."RoundUnit", 
        Cast(( CASE 
            WHEN e."CycleMode" = 'W' THEN (SELECT Cast(f."ShiftID" AS Int)
	            FROM APZ_WSC1 f LEFT OUTER JOIN APZ_OEWS b ON Cast(f."ShiftID" AS Int) = b."ShiftID" 
	            WHERE f."DayID" = a."wkDay" AND e."SchID" = f."SchID" AND e."CalcType" = '1' 
	            	AND (IFNULL(a."InTime", 0) <> 0 AND (a."InTime" BETWEEN b."StartTimeIn" AND b."EndTimeIn"))
	            	AND (IFNULL(a."OutTime", 0) <> 0 AND (a."OutTime" BETWEEN b."StartTimeOut" AND b."EndTimeOut"))) 
            WHEN e."CycleMode" = 'M' THEN (SELECT Cast(f2."ShiftID" AS Int) 
            	FROM APZ_WSC2 f2 LEFT OUTER JOIN APZ_OEWS b ON Cast(f2."ShiftID" AS Int) = b."ShiftID"
           		WHERE f2."DayID" = a."mthDay" AND e."SchID" = f2."SchID" AND e."CalcType" = '1' 
           			AND (IFNULL(a."InTime", 0) <> 0 AND (a."InTime" BETWEEN b."StartTimeIn" AND b."EndTimeIn"))
           			AND (IFNULL(a."OutTime", 0) <> 0 AND (a."OutTime" BETWEEN b."StartTimeOut" AND b."EndTimeOut"))) 
            WHEN e."CycleMode" = 'Y' THEN (SELECT Cast(f3."ShiftID" AS Int)
            	FROM APZ_WSC3 f3 LEFT OUTER JOIN APZ_OEWS b ON Cast(f3."ShiftID" AS Int) = b."ShiftID"
           		WHERE f3."DayID" = a."yrDay" AND e."SchID" = f3."SchID" AND e."CalcType" = '1' 
           			AND (IFNULL(a."InTime", 0) <> 0 AND (a."InTime" BETWEEN b."StartTimeIn" AND b."EndTimeIn"))
           			AND (IFNULL(a."OutTime", 0) <> 0 AND (a."OutTime" BETWEEN b."StartTimeOut" AND b."EndTimeOut"))) 
            ELSE NULL 
        END) As Int) AS "ShftID" 
    	FROM #tblInOut a LEFT OUTER JOIN APZ_OWSC e ON e."SchID" = a."SchID");
   
   Update a Set "ShftID" = Case When b."CycleMode" = 'W' Then (Select Cast(f."ShiftID" AS Int) From (Select *,ROW_NUMBER() OVER (PARTITION BY "ShiftID") As "Rn" From APZ_WSC1) f Where f."SchID" = a."SchID" And f."Rn" = 1)
							When b."CycleMode" = 'M' Then (Select Cast(f."ShiftID" AS Int) From (Select *,ROW_NUMBER() OVER (PARTITION BY "ShiftID") As "Rn" From APZ_WSC2) f Where f."SchID" = a."SchID" And f."Rn" = 1)
							Else (Select Cast(f."ShiftID" AS Int) From (Select *,ROW_NUMBER() OVER (PARTITION BY "ShiftID") As "Rn" From APZ_WSC3) f Where f."SchID" = a."SchID" And f."Rn" = 1) End
	From #temp_Shift a, APZ_OWSC b
	Where a."SchID" = b."SchID" And Ifnull(Cast(a."ShftID" AS Int),0) = 0;
  
	/*Lấy ra thông tin ca trong từng ngày của từng nhân viên dựa vào ca đã chọn ở trên*/
	create local temporary table #InOutShift as(
		select a.*,b.*
			, CAST((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("EndBreak" as nvarchar),4),2) || ':' || RIGHT(cast("EndBreak" as nvarchar),2)) as timestamp) "EndBreakDate"
			, CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("StartBreak" as nvarchar),4),2) || ':' || RIGHT(cast("StartBreak" as nvarchar),2)) as timestamp) "StartBreakDate"
			, CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("InWorkTime" as nvarchar),4),2) || ':' || RIGHT(cast("InWorkTime" as nvarchar),2)) as timestamp) "InWorkTimeDate"
			, Case When Ifnull(b."IsNightShift",'N') = 'Y' 
				Then CAST ((TO_VARCHAR(ADD_DAYS(current_date,1), 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("OutWorkTime" as nvarchar),4),2) || ':' || RIGHT(cast("OutWorkTime" as nvarchar),2)) as timestamp)
				Else CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("OutWorkTime" as nvarchar),4),2) || ':' || RIGHT(cast("OutWorkTime" as nvarchar),2)) as timestamp) End As "OutWorkTimeDate"
			, CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("StartTime" as nvarchar),4),2) || ':' || RIGHT(cast("StartTime" as nvarchar),2)) as timestamp) "StartTimeDate"
			, Case When Ifnull(b."IsNightShift",'N') = 'Y' 
				Then CAST ((TO_VARCHAR(ADD_DAYS(current_date,1), 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("EndTime" as nvarchar),4),2) || ':' || RIGHT(cast("EndTime" as nvarchar),2)) as timestamp)
				Else CAST ((TO_VARCHAR(current_date, 'yyyy/MM/dd') || ' ' || LEFT(RIGHT('0000' || cast("EndTime" as nvarchar),4),2) || ':' || RIGHT(cast("EndTime" as nvarchar),2)) as timestamp) End As  "EndTimeDate"
			
		from #temp_Shift a LEFT JOIN APZ_OEWS b on b."ShiftID" = a."ShftID"
			where "DateTimeOut" is null 
				or (("MinInterval" is null or ("DateTimeOut" is not null and SECONDS_BETWEEN("DateTimeIn","DateTimeOut") >= "MinInterval" * 60)) 
					and (ifnull("MaxInterval",0) = 0 or ("DateTimeOut" is not null and SECONDS_BETWEEN("DateTimeIn","DateTimeOut") <= "MaxInterval" * 60))));

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
	update #InOUtShift set "CalcWorkCode" = "WorkCode";
	update #InOutShift set "CalcWorkCode" = '' where IFNULL("IsHoliday",'N') = 'Y';
	
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
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "DateTimeOut")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("DateTimeIn", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /3600.0
			else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut")/3600.0 end
		-- Số phút thực tế giữa vào - ra
		,"DMinutes" = case when ("StartBreak" is not null and "EndBreak" is not null) AND ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") then (
			--Trừ đi khoảng thời gian ăn trưa khi thời gian In Out có chứa thời gian nghỉ ăn trưa
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "DateTimeOut")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("DateTimeIn", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /60.0
			else SECONDS_BETWEEN("DateTimeIn", "DateTimeOut")/60.0 end
	
		--Số giờ làm việc thực tế
		,"DWorkHour" = case when ("StartBreak" is not null and "EndBreak" is not null) AND ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") then (
			--Trừ đi khoảng thời gian ăn trưa khi thời gian In Out có chứa thời gian nghỉ ăn trưa
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "OutWorkTimeDate")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("InWorkTimeDate", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("InWorkTimeDate", "OutWorkTimeDate") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /3600.0
			else SECONDS_BETWEEN("InWorkTimeDate", "OutWorkTimeDate")/3600.0 end
		-- Số phút làm việc thực tế
		,"DWorkMinutes" = case when ("StartBreak" is not null and "EndBreak" is not null) AND ("OutWorkTime" > "EndBreak" and "InWorkTime" < "StartBreak") then (
			--Trừ đi khoảng thời gian ăn trưa khi thời gian In Out có chứa thời gian nghỉ ăn trưa
			case when "OutWorkTime" > "EndBreak" and "InWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("EndBreakDate", "OutWorkTimeDate")
				when "InWorkTime" < "StartBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then SECONDS_BETWEEN("InWorkTimeDate", "StartBreakDate")
				when "InWorkTime" between "StartBreak" and "EndBreak" and "OutWorkTime" between "StartBreak" and "EndBreak" then 0
				else SECONDS_BETWEEN("InWorkTimeDate", "OutWorkTimeDate") - SECONDS_BETWEEN("StartBreakDate","EndBreakDate") end
				) /60.0
			else SECONDS_BETWEEN("InWorkTimeDate", "OutWorkTimeDate")/60.0 end
	where "InWorkTime" is not null and "OutWorkTime" is not null;
	
	/*Những trường hợp giờ không nằm trong khoảng thời gian của ca thì sử dụng bảng combine để set workcode*/
	IF EXISTS(SELECT 1 FROM OBJECTS WHERE OBJECT_TYPE='TABLE' AND SCHEMA_NAME = CURRENT_SCHEMA AND OBJECT_NAME='APZ_EWS1')
	Then
		Update a Set "CalcWorkCode" = Ifnull(b."WorkCode",'') || ',' || Ifnull(b."AbsentCode", '')
		From #InOutShift a, APZ_EWS1 b
		Where a."ShiftID" = b."ShiftID"
			And a."DMinutes" < a."WorkingTotal"
			And a."DMinutes" Between b."FromMinutes" And b."ToMinutes";
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
	update  a set "TotalRate" = (select SUM("Rate") from #InOUtShift) from #InOUtShift a;
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
			select :Period, "empID",:workDay,"WDOTWCode",ROUND((("AfterOT"+"BeforeOT")/60.0),ifnull("RoundUnit",0)) 
				from #InOUtShift where ("CalcAfterOT" = 'Y' or "CalcBeforeOT" = 'Y') and "WDOTWCode" is not null and ("AfterOT" >0 or "BeforeOT">0) and "IsWeekend" != 'Y' and "IsHoliday" != 'Y'
		UNION ALL
			select :Period,"empID",:workDay,"HLOTWCode","DHour"
				from #InOUtShift where "CalcHLOT" = 'Y' and "HLOTWCode" is not null and "DHour" >0 and "IsHoliday" = 'Y'
		UNION ALL
			 select :Period,"empID", :workDay, "WKOTWCode","DHour"
				from #InOUtShift where "CalcWKOT" = 'Y' and "WKOTWCode" is not null and "DHour" >0 and "IsWeekend" = 'Y' and "IsHoliday" != 'Y';

	select cast(null as int) "AbsEntry", :Period "Period", "empID", "empName", "dept", "SchID", "ShiftID", cast(null as int) as "LineNum", :workDay as "WorkDay", "CalcWorkCode" as "WorkCode"
			,"InTime", "OutTime", "InWorkTime", "OutWorkTime", "Late", "Early", "BeforeOT", "AfterOT", "IsWeekend", "IsHoliday", "DHour", "DMinutes", "DWorkHour", "DWorkMinutes"
			,"WorkRate" "WorkTotal", cast(null as nvarchar(254)) "FreeTxt", cast(null as int) as "LogInstanc"
		from #InOUtShift order by "empID","WorkDay";

	insert into #tblOverTime ("Period","empID","LineNum","WorkDay","OvertimeType","OvertimeHour", "empName","dept")
	select a."Period",a."empID",a."LineNum",a."WorkDay",a."OvertimeType",SUM(a."OvertimeHour") "OvertimeHour", b."firstName" || ',' || b."lastName" "empName",b."dept"
			from #tblOverTimeTemp a left join OHEM b ON a."empID" = b."empID" 
				group by a."Period",a."empID",a."LineNum",a."WorkDay",a."OvertimeType",b."firstName", b."lastName",b."dept" order by a."empID",a."WorkDay";
				
	select * from #tblOverTime;

END;
 