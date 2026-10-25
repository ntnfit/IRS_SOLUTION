
CREATE PROCEDURE APZ_Capacity_Run
(
	in scheduleId int default 0, -------------------------------- Schedule code chạy Capacity -----------------------------------------------------------------------
	in tranType char(1) default 'R' ----------------------------- TranType R là run chạy mới. Không phải R thì là lấy dứ liệu từ database ---------------------------
)
AS
BEGIN

	DECLARE ex1Qry nvarchar(5000);
	DECLARE ex2Qry nvarchar(5000);
	DECLARE ex3Qry nvarchar(5000);
	DECLARE ex4Qry nvarchar(5000);
	DECLARE ex5Qry nvarchar(5000);
		
	DECLARE id int;
	DECLARE sqlString nvarchar(5000);
	----------------------------------------- lấy dữ liệu mã capacity tương ứng với schedule --------------------------------------------------------------
	DECLARE existCode bigint;
	
	DECLARE beginDate TimeStamp; 
	DECLARE endDate TimeStamp;
	
	Select CURRENT_DATE Into beginDate From Dummy;
	SELECT IFNULL((SELECT top 1 "GrcId" FROM APZ_CGRC where "ScheCode" = :scheduleId),0) Into existCode FROM Dummy;
	
	------------------------------------------ Lấy thời gian bắt đầu và kết thúc của schedule ----------------------------------------------------------------
	SELECT b."FixDate" Into endDate FROM APZ_OPRS a inner join APZ_OMRP b ON a."MrpId" = b."MrpCode" where a."PSCode" = :scheduleId;

	IF (:tranType <> 'R' And :existCode > 0)
	THEN
		------------------------------------------------- LẤY DANH SÁCH LỆNH LINK --------------------------------------------------------------------------
		Create Local Temporary Table #tempLink AS (SELECT "DocEntry", "U_OriginLine" FROM OWOR WHERE 1 = 0);
		INSERT INTO #tempLink SELECT MIN("DocEntry") AS "DocEntry", "U_OriginLine" FROM OWOR WHERE "U_OriginType" = '800005' and "U_OriginCode" = :existCode group by "U_OriginLine";

		-------------------------------------------------- END ---------------------------------------------------------------------------------------------

		------------------------------------------------- lấy dữ liệu capacity trong database ---------------------------------------------------------------

		Create Local Temporary Table #tempData AS (SELECT a.*, CAST(Null AS TimeStamp) as "StartDate", CAST(Null AS TimeStamp) as "EndDate",
			Cast(Null As Int) as "LinkEntry", Cast(Null as Nvarchar) AS "LineType"
			FROM APZ_CGRC a WHERE 1 = 0);
		INSERT Into #tempData
		SELECT a.*, ADD_SECONDS(ADD_SECONDS(a."WorkDay",(a."StartTime"/100)*3600),MOD(a."StartTime",100)*60) As "StartDate" 
				, ADD_SECONDS(ADD_SECONDS(a."EndWorkDay",(a."EndTime"/100)*3600),MOD(a."EndTime",100)*60) As "EndDate"
				, CASE when a."IsBase" = 'Y' then NULL else b."DocEntry" End AS "LinkEntry", 'C' As "LineType"
				FROM APZ_CGRC a
					LEFT JOIN #tempLink b on a."Line" = b."U_OriginLine" where a."GrcId" = :existCode;

		------------------------------------------- UPDATE trạng thái các lênh kế hoạc theo trạng thái các lệnh thực tế trên hện thống ----------------------------------------------

		UPDATE a SET "Complete" = CASE WHEN b."Status" = 'L' THEN 'D' WHEN b."Status" = 'C' THEN 'L' ELSE 'G' END 
		FROM #tempData a, OWOR b WHERE a."LinkEntry" = b."DocEntry" And a."LinkEntry" IS NOT NULL;

		------------------------------------------- UPDATE Thông tin thời gian của các appointment liên đã gen lệnh sản xuất -----------------------------------------------------------------------------

		UPDATE a Set a."StartDate" = ADD_SECONDS(ADD_SECONDS(b."StartDate",(B."U_StartTime"/100)*3600),MOD(B."U_StartTime",100)*60), 
					a."WorkDay" = b."StartDate", a."StartTime" = b."U_StartTime",
					a."EndDate" = ADD_SECONDS(ADD_SECONDS(b."EndDate",(b."U_EndTime"/100)*3600),MOD(b."U_EndTime",100)*60), 
					a."EndWorkDay" = b."EndDate", a."EndTime" = b."U_EndTime",
					a."ResCode" = b."ItemCode", a."ResName" = b."ResName"
		FROM #tempData a inner join (SELECT x."StartDate", x."EndDate", x."U_StartTime", x."U_EndTime", x."ItemCode", z."ResName", x."U_OriginLine", x."DocEntry" 
								FROM WOR1 x INNER JOIN OWOR y ON x."DocEntry" = y."DocEntry" INNER JOIN ORSC z ON x."ItemCode" = z."ResCode" 
								WHERE y."U_OriginType" = '800005' and y."U_OriginCode" = :existCode and x."ItemType" = 290 and x."U_OriginLine" is not NULL) b 
								ON a."LinkEntry" = b."DocEntry" and a."Line" = b."U_OriginLine";

		--------------------------------------------------------------- END --------------------------------------------------------------------------------------------

		-------------------------------- lấy dữ liệu line để đánh số thứ tự --------------------------------------------------------------------------
		Select IFNULL((SELECT MAX("Line") FROM #tempData),0) Into id From Dummy;

		------------------------------------------ Thêm dữ liệu các lệnh sản xuất đã có trên hệ thống không phải gen từ capacity này -----------------------------------------------------------
		INSERT INTO #tempData 
		( "GrcId","ScheCode","ScheLine","WorkDay","EndWorkDay","Qty","Shift","Line","ResCode","OpeCode","StartTime","EndTime","IsBase","IsOverLoad","IdColor","TimeRun", "SumTimeOrder", "TotalTime"
		, "TotalTimeShift","FatherLine","ResName","OpeName","ItemCode","ItemName","DemandEntry","DemandLine","DemandType","StartDate","EndDate","FatherCode","BomVersion", "CardCode","LTime","Notes"
		, "ProId","OpeOrder","FatherOpeOrder","LinkEntry","Complete","LineType")
		SELECT :existCode, :scheduleId, 1, a."StartDate", a."EndDate", b."PlannedQty", a."U_Shift",:id + ROW_NUMBER() over(order by a."ItemCode"), a."ItemCode", a."U_OpeCode", a."U_StartTime", a."U_EndTime", 
			'N', 'N', 'ApzProd', a."PlannedQty", 1, 1, 1, Cast(-1 as int), c."ResName", d."OpeName", b."ItemCode", e."ItemName", NULL, NULL, NULL, 
			ADD_SECONDS(ADD_SECONDS(a."StartDate",(a."U_StartTime"/100)*3600),MOD(a."U_StartTime",100)*60), 
			ADD_SECONDS(ADD_SECONDS(a."EndDate",(a."U_EndTime"/100)*3600),MOD(a."U_EndTime",100)*60), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b."DocEntry", 'G', 'P' 
		FROM WOR1 a inner join OWOR b ON a."DocEntry" = b."DocEntry" 
					inner JOIN ORSC c On a."ItemCode" = c."ResCode"
					inner join APZ_OOMD d ON a."U_OpeCode" = d."OpeCode"
					inner join OITM e ON b."ItemCode" = e."ItemCode" 
		WHERE a."ItemType" = 290 And (b."Status" <> 'C' or b."Status" <> 'L') 
			And (a."EndDate" Between :beginDate and :endDate or a."EndDate" Between :beginDate and :endDate) 
			And (IFNULL(b."U_OriginType",'') <> '800005' or not Exists(SELECT 1 FROM #tempLink where "DocEntry" = a."DocEntry"));
		
		SELECT * FROM #tempData;
		Return;
	END IF;
	------------------------------------------- Lấy danh sách các lệnh sản suất tương ứng với các mặt hàng đã lên schedule ------------------------------------------------------------------
	
	Create Local Temporary Table #tempPro AS (
		SELECT a."Line", a."WorkDay", a."Qty", a."Shift", a."ItemCode", a."ItemName", a."DemandEntry", a."DemandLine", a."DemandType", a."FatherCode", a."BomVersion", a."CardCode", 
			a."LTime", a."Notes",CAST(NULL AS nvarchar(50)) AS "FnGood", Cast(NULL As Int) AS "ProId" 
		FROM APZ_PRS1 a Where 1 = 0);
	Insert Into #tempPro
		SELECT a."Line", a."WorkDay", a."Qty", a."Shift", a."ItemCode", a."ItemName", a."DemandEntry", a."DemandLine", a."DemandType", a."FatherCode", a."BomVersion", a."CardCode", 
			a."LTime", a."Notes", CAST(NULL AS nvarchar(50)) AS "FnGood", ROW_NUMBER() OVER ( ORDER BY a."Line") AS "ProId" 
		FROM APZ_PRS1 a INNER JOIN OITM b ON a."ItemCode" = b."ItemCode"
		WHERE "PSId" = :scheduleId and b."InvntItem" = 'Y' and "TypePlanning" = 2 and IFNULL("Qty", 0) > 0;
	
	------------------------------------------- UPDATE dữ liệu Finish good theo demand type từ kết quả MRP tính ra ------------------------------------------------------------------------
	UPDATE a SET a."FnGood" = b."ItemCode" FROM #tempPro a INNER JOIN APZ_FCT1 b ON a."DemandEntry" = b."Code" AND a."DemandLine" = b."LineID" WHERE a."DemandType" = '8000005';

	UPDATE a SET a."FnGood" = b."ItemCode" FROM #tempPro a INNER JOIN RDR1 b ON a."DemandEntry" = b."DocEntry" AND a."DemandLine" = b."LineNum" WHERE a."DemandType" = '17';

	UPDATE a SET a."FnGood" = b."ItemCode" FROM #tempPro a INNER JOIN APZ_PRS1 b ON a."DemandEntry" = b."PSId" AND a."DemandLine" = b."Line" WHERE a."DemandType" = '800004';
	------------------------------------------------------------ END UPDATE các demand type là các nguồn đầu vào -------------------------------------------------------------------------

	------------------------------------------- Khi không có nguồn đầu vào demand type là ZZ hoặc NULL và không có Father code, sẽ được phân bổ độc lập không phụ thuộc lẫn nhau  ---------------------------------------------------------------------
	UPDATE #tempPro set "FatherCode" = NULL, "FnGood" = "ItemCode" where "DemandType" = 'ZZ';

	UPDATE a set a."FnGood" = a."ItemCode" FROM #tempPro a where a."DemandType" is NULL;

	------------------------------------------- Lấy cấu trúc routing hình cây của các mã sản xuất theo routing được khai báo trong master data -----------------------------------------------------------------------------------------------------------------
	Create Local Temporary Table #tempRTId AS (
		SELECT MIN("CreateDate") AS "CreateDate", "RoutCode", "ProdCode", "WorkDay", CAST(NULL AS nvarchar(50)) AS "FatherCode", "ProdCode" AS "FnGood" 
		FROM (SELECT "WorkDay" FROM #tempPro GROUP BY "WorkDay") AS a 
			INNER JOIN (SELECT "RoutCode", "ProdCode", "FromDate", "ToDate", "Status", "CreateDate" FROM APZ_ORTA WHERE "Status" = 'A' AND "IsBase" = 'Y') AS b 
				ON a."WorkDay" BETWEEN IFNULL(b."FromDate", a."WorkDay") AND IFNULL(b."ToDate", a."WorkDay")
		Where 1 = 0
		GROUP BY "RoutCode", "ProdCode", "WorkDay");
	Insert Into #tempRTId
	SELECT MIN("CreateDate") AS "CreateDate", "RoutCode", "ProdCode", "WorkDay", CAST(NULL AS nvarchar(50)) AS "FatherCode", "ProdCode" AS "FnGood" 
		FROM (SELECT "WorkDay" FROM #tempPro GROUP BY "WorkDay") AS a 
			INNER JOIN (SELECT "RoutCode", "ProdCode", "FromDate", "ToDate", "Status", "CreateDate" FROM APZ_ORTA WHERE "Status" = 'A' AND "IsBase" = 'Y') AS b 
				ON a."WorkDay" BETWEEN IFNULL(b."FromDate", a."WorkDay") AND IFNULL(b."ToDate", a."WorkDay") 
	GROUP BY "RoutCode", "ProdCode", "WorkDay";
	
	Create Local Temporary Table #tempRout as (
		SELECT a.*, b."WorkDay", b."FnGood", Cast(Null As nvarchar(50)) As "FatherCode",b."ProdCode" AS "ItemCode",CAST(-1 AS Int) AS "FatherOpeOrder", Cast(Null As Decimal(19,6)) As "Effic"
		FROM APZ_RTA1 a INNER JOIN #tempRTId b ON a."RoutCode" = b."RoutCode" Where 1 = 0
	);
	
	Create Local Temporary Table #tempRoutDraft as (
		SELECT a.*, b."WorkDay", b."FnGood", CASE WHEN a."WorkOrder" = 'N' THEN b."FatherCode" ELSE b."ProdCode" END AS "FatherCode", b."ProdCode" AS "ItemCode", CAST(-1 AS integer) AS "FatherOpeOrder" 
		FROM APZ_RTA1 a INNER JOIN #tempRTId b ON a."RoutCode" = b."RoutCode"
	);
	
	Insert Into #tempRout
	SELECT a.* , c."Effic"
	FROM (SELECT * From #tempRoutDraft
			UNION ALL
			SELECT a.*, b."WorkDay", b."FnGood", CASE WHEN a."WorkOrder" = 'N' THEN b."FatherCode" ELSE b."ProdCode" END AS "FatherCode", b."ProdCode", b."VisOrder" AS "FatherOpeOrder" 
			FROM APZ_RTA1 a INNER JOIN #tempRoutDraft b ON a."RoutCode" = b."SubRoutCode" WHERE IFNULL(b."SubRoutCode", '') <> '') a 
		LEFT JOIN APZ_RTA2 c ON a."RoutCode" = c."RoutCode" and a."LineNum" = c."FatherLine" and a."ResCode" = c."ResCode" where IFNULL("OpCode",'') <> '';
	
	------------------------------------------------------------------ END lấy cấu trúc routing --------------------------------------------------------------------------------------

	--------------------------------------------------- Lấy danh sách các ca sản xuất được sắp xếp theo thứ tự thời gian ------------------------------------------------------------------
	Create Local Temporary Table #tempShp As ( SELECT *, row_number() over(order by case when IFNULL("Time2", 'N') = 'N' Or "Time2" = '' Then "End1" Else "End2" End) "Line" FROM APZ_OSHP);

	--------------------------------------------------- lấy danh sách lệnh sản xuất theo các công đoạn và máy sản xuất theo Routing ----------------------------------------------------------------------------
	Create Local Temporary Table #TempTime As (
		SELECT c."ItemCode", c."ItemName", c."Qty", c."Line", a."WorkDay", c."Shift", Cast(Null As Decimal(19,6)) AS "BaseQty", Cast(Null As Int) As "BaseTime",
			a."ResCode", a."ResName", "OpCode" AS "OpeCode", "OpName" AS "OpeName", "DemandEntry", "DemandLine", "DemandType", c."FatherCode", c."BomVersion", c."CardCode", c."LTime", c."Notes", 
			c."ProId", a."VisOrder" AS "OpeOrder", d."Line" AS "ShiftOrder", CAST(NULL AS Int) "FatherOpeOrder", CAST(Null As Nvarchar(50)) "RoutCode", CAST(NULL AS Int) AS "RoutLine", a."WorkDay" AS "OrderDate" 
		FROM #tempRout a 
			INNER JOIN #tempPro c ON a."ItemCode" = c."ItemCode" AND IFNULL(a."FatherCode", '') = IFNULL(c."FatherCode", '') AND a."FnGood" = c."FnGood" AND a."WorkDay" = c."WorkDay" 
			INNER JOIN #tempShp d ON c."Shift" = d."Code"
		Where 1 = 0);
	
	Insert Into #TempTime
	SELECT c."ItemCode", c."ItemName", c."Qty", c."Line", a."WorkDay", c."Shift", IFNULL(a."InvQty", 1) * a."Effic" / 100.0 AS "BaseQty", 
			CASE WHEN a."UomSetTime" = 'M' THEN IFNULL(a."SetTime", 0) ELSE 60 * IFNULL(a."SetTime", 0) END 
				+ CASE WHEN a."UomRunTime" = 'M' THEN IFNULL(a."RunTime", 0) ELSE 60 * IFNULL(a."RunTime", 0) END 
				+ CASE WHEN a."UomTransTime" = 'M' THEN IFNULL(a."TransTime", 0) ELSE 60 * IFNULL(a."TransTime", 0) END AS "BaseTime",
			a."ResCode", a."ResName", "OpCode" AS "OpeCode", "OpName" AS "OpeName", "DemandEntry", "DemandLine", "DemandType", c."FatherCode", c."BomVersion", c."CardCode", c."LTime", c."Notes", 
			c."ProId", a."VisOrder" AS "OpeOrder", d."Line" AS "ShiftOrder", "FatherOpeOrder", "RoutCode", "LineNum" AS "RoutLine", a."WorkDay" AS "OrderDate" 
		FROM #tempRout a 
			INNER JOIN #tempPro c ON a."ItemCode" = c."ItemCode" AND IFNULL(a."FatherCode", '') = IFNULL(c."FatherCode", '') AND a."FnGood" = c."FnGood" AND a."WorkDay" = c."WorkDay" 
			INNER JOIN #tempShp d ON c."Shift" = d."Code" ORDER BY "Line";

	--------------------------------------------------- Lấy lịch sản xuất của các máy theo ngày được định nghĩa trong resource work schedule -----------------------------------------------------------------------------------------------
	Create Local Temporary Table #tempWorkShIFSchedule As (
		SELECT a."ResCode", a."ResName", b."Code", b."WorkDate", b."Shift", CAST(Null AS Int) AS "ShiftOrder", c."Start1" "StartTime", Cast(Null As Int) As "EndTime"
		FROM ORSC a Left Join (SELECT * FROM APZ_RWS1 where IFNULL("Shift", '') <> '') b ON a."U_WorkSchedule" = b."Code"
			Left Join #tempShp c ON b."Shift" = c."Code"
		Where 1 = 0);
	Insert Into #tempWorkShIFSchedule
	SELECT a."ResCode", a."ResName", b."Code", b."WorkDate", b."Shift", IFNULL(c."Line",0) "ShiftOrder", c."Start1" "StartTime", Cast(Null As Int) As "EndTime"
	FROM ORSC a Left Join (SELECT * FROM APZ_RWS1 where "WorkDate" between :beginDate and :endDate and IFNULL("Shift", '') <> '') b ON a."U_WorkSchedule" = b."Code"
			Left Join #tempShp c ON b."Shift" = c."Code";

	--------------------------------------------------- lấy tổng thời gian hoạt động trong ca làm việc, và tổng thời gian làm việc từ ngày bắt đầu schedule đến ngày hiện tại ---------------------------------------------------------------------------
	Create Local Temporary Table #tempWorkScheduleTotal As (
		SELECT *, CAST(Null AS Decimal(19,6)) "TotalShift",CAST(Null AS Decimal(19,6)) "TotalToDate", CAST(Null AS Int) "DateShiftOrder", 'Y' "IsBase", 'N' "IsOverLoad",CAST(NULL AS TimeStamp) AS "StartDate",CAST(NULL AS TimeStamp) AS "EndDate"
		FROM #tempWorkShIFSchedule Where 1 = 0);
	Insert Into #tempWorkScheduleTotal
	SELECT *, case when "Shift" is NULL then Cast(0 As int) else ("EndTime" / 100) * 60 + MOD("EndTime",100) - ("StartTime" / 100) * 60 - MOD("StartTime",100) end "TotalShift",
		sum(case when "Shift" is NULL then Cast(0 As int) else ("EndTime" / 100) * 60 + MOD("EndTime",100) - ("StartTime" / 100) * 60 - MOD("StartTime",100) end) over (PARTITION BY "ResCode" order by "WorkDate", "ShiftOrder") AS "TotalToDate",
		row_number() over (PARTITION BY "ResCode" order by "WorkDate", "ShiftOrder") "DateShiftOrder", 'Y' AS "IsBase", 'N' As "IsOverLoad",
		ADD_SECONDS(ADD_SECONDS("WorkDate",("StartTime"/100)*3600),MOD("StartTime",100)*60) AS "StartDate",
		ADD_SECONDS(ADD_SECONDS("WorkDate",("EndTime"/100)*3600),MOD("EndTime",100)*60) AS "EndDate"
	FROM #tempWorkShIFSchedule;

	--------------------------------------------------- BEGIN Lấy danh sách lệnh với ngày và giờ kết thúc. ----------------------------------------------------------------------------------------------------
	-----------------------------------------------------Với những lệnh không vào ca sẽ được UPDATE lùi về ngày có ca làm việc --------------------------------------------------------------------------
	UPDATE a SET "Shift" = b."Shift", "WorkDay" = b."WorkDate" 
	FROM #TempTime a, #tempWorkScheduleTotal b
	WHERE a."ResCode" = b."ResCode" 
		AND b."DateShiftOrder" = (SELECT MAX("DateShiftOrder") FROM #tempWorkScheduleTotal a1 
						Where a."ResCode" = a1."ResCode" And (a1."WorkDate" < a."WorkDay" OR a1."WorkDate" = a."WorkDay" AND a1."ShiftOrder" <= a."ShiftOrder" AND a."Shift" IS NOT NULL));

	--------------------------------------------------------- bắt đầu lấy thời gian kết thúc -----------------------------------------------------------------------------------------------------------
	Create Local Temporary Table #tempStartTime As (
		SELECT a.*, b."WorkDate" AS "EndWorkDate", b."StartTime" AS "StartTime", b."EndTime" AS "EndTime", 
			CAST(NULL AS timestamp) AS "StartDate", CAST(Null As timestamp) AS "EndDate", 
			b."TotalShift", b."TotalToDate", b."DateShiftOrder", Cast(Null AS Int) AS "TotalTimeRun", 'N' AS "IsBase", 'N' AS "IsOverLoad"
		From #TempTime a Left join #tempWorkScheduleTotal b on a."ResCode" = b."ResCode" and a."WorkDay" = b."WorkDate" and a."Shift" = b."Shift"
		Where 1 = 0);
	Insert Into #tempStartTime
	SELECT a.*, b."WorkDate" AS "EndWorkDate", b."StartTime" AS "StartTime", b."EndTime" AS "EndTime", 
		CAST(NULL AS timestamp) AS "StartDate", ADD_SECONDS(ADD_SECONDS(b."WorkDate", (b."EndTime" / 100) * 3600), (MOD(b."EndTime", 100)) * 60) AS "EndDate", 
		b."TotalShift", b."TotalToDate", b."DateShiftOrder", a."Qty" * a."BaseTime" / a."BaseQty" AS "TotalTimeRun", 'N' AS "IsBase", 'N' AS "IsOverLoad" 
	FROM #TempTime a Left join #tempWorkScheduleTotal b on a."ResCode" = b."ResCode" and a."WorkDay" = b."WorkDate" and a."Shift" = b."Shift";
	-------------------------------------------------------------------------- End lấy ngày, giờ kết thúc ------------------------------------------------------------------------------------------------


	--------------------------------------------------- UPDATE ngày bắt đầu của lệnh sản xuất dự vào totaltime của lệnh và totaltodate của các máy khi không bị overload ----------------------------------------------------------
	UPDATE a SET a."WorkDay" = b."WorkDate", a."StartDate" = ADD_SECONDS(ADD_SECONDS(b."WorkDate", (b."EndTime" / 100) * 3600), (MOD(b."EndTime", 100) - (a."TotalTimeRun" + b."TotalToDate" - a."TotalToDate")) * 60) 
	FROM #tempStartTime a, #tempWorkScheduleTotal b
	WHERE a."ResCode" = b."ResCode" And a."DateShiftOrder" >= b."DateShiftOrder" 
		And a."TotalTimeRun" >= a."TotalToDate" - b."TotalToDate" And a."TotalTimeRun" <= a."TotalToDate" - b."TotalToDate" + b."TotalShift";

	--------------------------------------------------- UPDATE ngày và thời gian bắt đầu của lệnh sản xuất khi bị overload (vượt quá thời gian chạy MRP) -------------------------------------------------------------------------------------------------------
	UPDATE a SET a."WorkDay" = b."WorkDate", a."StartDate" = ADD_SECONDS(ADD_SECONDS(b."WorkDate", (b."StartTime" / 100) * 3600), (MOD(b."StartTime", 100)) * 60) 
	FROM #tempStartTime a, #tempWorkScheduleTotal b 
	WHERE a."ResCode" = b."ResCode" and a."DateShiftOrder" >= b."DateShiftOrder" and b."DateShiftOrder" = 1 and a."TotalTimeRun" > a."TotalToDate";

	UPDATE #tempStartTime SET "StartTime" = HOUR("StartDate") * 100 + MINUTE("StartDate");

	-------------------------------------------------- lấy dữ liệu ca sản xuất của các máy -------------------------------------------------------------------------------------------
	INSERT into #tempStartTime ("ItemCode","Line","WorkDay","EndWorkDate","Shift","ResCode","ResName","ShiftOrder","StartTime","EndTime","StartDate","EndDate","TotalShift","TotalToDate","DateShiftOrder","IsBase","IsOverLoad","BaseQty","BaseTime")
	SELECT CAST("TotalShift" AS nvarchar) || N' minutes', 100000,"WorkDate","WorkDate","Shift","ResCode","ResName", "ShiftOrder","StartTime","EndTime", "StartDate", "EndDate", "TotalShift", "TotalToDate", "DateShiftOrder", "IsBase","IsOverLoad", 1, 1 
	FROM #tempWorkScheduleTotal;

	-------------------------------------------------- Lấy dự liệu capacity của schedule hiện tại -----------------------------------------------------------------------------------
	Create Local Temporary Table #tempDataRun As (
		SELECT CAST(NULL AS INT) as "GrcId", CAST(NULL AS INT) as "ScheCode", "Line" AS "ScheLine", "WorkDay", "EndWorkDate" AS "EndWorkDay", "Qty", "Shift", CAST(Null As Int) AS "Line", "ResCode", "OpeCode"
			, "StartTime", "EndTime", "IsBase", "IsOverLoad", CAST(Null AS Nvarchar) AS "IdColor", "TotalTimeRun" AS "TimeRun", "TotalToDate" AS "SumTimeOrder", "TotalToDate" AS "TotalTime", "TotalShift" AS "TotalTimeShift"
			, CAST(-1 AS integer) AS "FatherLine", "ResName", "OpeName", "ItemCode", "ItemName", "DemandEntry", "DemandLine", "DemandType", "StartDate", "EndDate", "FatherCode", "BomVersion", "CardCode", "LTime"
			, "Notes", "ProId", "OpeOrder", "FatherOpeOrder", CAST(NULL AS integer) AS "LinkEntry", 'N' AS "Complete", 'C' AS "LineType", "RoutCode", "RoutLine", CAST(Null As nvarchar(254)) As "ExInfo1"
			, CAST(Null As nvarchar(254)) As "ExInfo2", CAST(Null As nvarchar(254)) As "ExInfo3", CAST(Null As nvarchar(254)) As "ExInfo4", CAST(Null As nvarchar(254)) As "ExInfo5", "OrderDate"
		FROM #tempStartTime WHERE 1 = 0);
	Insert into #tempDataRun
	SELECT :existCode As "GrcId", :scheduleId As "ScheCode","Line" AS "ScheLine", "WorkDay", "EndWorkDate" AS "EndWorkDay", "Qty", "Shift", ROW_NUMBER() OVER ( ORDER BY "ResCode") AS "Line", "ResCode", "OpeCode"
		, "StartTime", "EndTime", "IsBase", "IsOverLoad", CASE WHEN "IsBase" = 'N' THEN "OpeCode" ELSE (CASE WHEN "IsOverLoad" = 'N' THEN 'ApzNormal' ELSE 'ApzOverLoad' END) END AS "IdColor", "TotalTimeRun" AS "TimeRun"
		, "TotalToDate" AS "SumTimeOrder", "TotalToDate" AS "TotalTime", "TotalShift" AS "TotalTimeShift", CAST(-1 AS integer) AS "FatherLine", "ResName", "OpeName", "ItemCode", "ItemName", "DemandEntry", "DemandLine"
		, "DemandType", "StartDate", "EndDate", "FatherCode", "BomVersion", "CardCode", "LTime", "Notes", "ProId", "OpeOrder", "FatherOpeOrder", CAST(NULL AS integer) AS "LinkEntry", 'N' AS "Complete", 'C' AS "LineType", "RoutCode", "RoutLine"
		, CAST(NULL AS nvarchar(254)) AS "ExInfo1", CAST(NULL AS nvarchar(254)) AS "ExInfo2", CAST(NULL AS nvarchar(254)) AS "ExInfo3", CAST(NULL AS nvarchar(254)) AS "ExInfo4", CAST(NULL AS nvarchar(254)) AS "ExInfo5", "OrderDate"	
	FROM #tempStartTime 
	Order By "ResCode", "WorkDay";

	------------------------------------------------- UPDATE lại status của các production theo trạng thái các lênh thực tế -----------------------------------------------------
	UPDATE a SET "Complete" = CASE WHEN b."Status" IN ('L','C') THEN 'D' ELSE 'G' END FROM #tempDataRun a INNER JOIN OWOR b ON a."LinkEntry" = b."DocEntry" WHERE a."LinkEntry" IS NOT NULL;

	------------------------------------------------- UPDATE dữ liệu với các trương extension -----------------------------------------------------------------------------------

	------------------------------------------------- END UPDATE ----------------------------------------------------------------------------------------------------------------
	SELECT IFNULL((select "Ex1Qry" from APZ_CPCS),'') Into ex1Qry From Dummy;
	SELECT IFNULL((select "Ex2Qry" from APZ_CPCS),'') Into Ex2Qry From Dummy;
	SELECT IFNULL((select "Ex3Qry" from APZ_CPCS),'') Into Ex3Qry From Dummy;
	SELECT IFNULL((select "Ex4Qry" from APZ_CPCS),'') Into Ex4Qry From Dummy;
	SELECT IFNULL((select "Ex5Qry" from APZ_CPCS),'') Into Ex5Qry From Dummy;
	
	IF (:ex1Qry <> '' or :ex2Qry <> '' or :ex3Qry <> '' or :ex4Qry <> '' or :ex5Qry <> '')
	Then
		Select N'UPDATE a set ' Into sqlString From Dummy;
		IF :ex1Qry <> ''
		Then
			ex1Qry = replace(replace(replace(:ex1Qry, ':DemandType','a.DemandType'), ':demandentry','a.DemandEntry'),':demandline','a.DemandLine');
			sqlString = ' a.ExInfo1 = (' || :ex1Qry || ')';
		Else
			sqlString = :sqlString || ' a.ExInfo1 = NULL';
		End If;

		IF :ex2Qry <> ''
		Then
			ex2Qry = replace(replace(replace(:ex2Qry, ':demandType','a.DemandType'), ':demandentry','a.DemandEntry'),':demandline','a.DemandLine');
			sqlString = ' ,a.ExInfo2 = (' || :ex2Qry || ')';
		Else
			sqlString = :sqlString || ' ,a.ExInfo2 = NULL';
		End If;

		IF :ex3Qry <> ''
		Then
			ex3Qry = replace(replace(replace(:ex3Qry, ':demandType','a.DemandType'), ':demandentry','a.DemandEntry'),':demandline','a.DemandLine');
			sqlString = ' ,a.ExInfo3 = (' || :ex3Qry || ')';
		Else
			sqlString = ' ,a.ExInfo3 = NULL';
		End If;

		IF :ex4Qry <> ''
		Then
			ex4Qry = replace(replace(replace(:ex4Qry, ':demandType','a.DemandType'), ':demandentry','a.DemandEntry'),':demandline','a.DemandLine');
			sqlString = :sqlString || ' ,a.ExInfo4 = (' || :ex4Qry || ')';
		Else
			sqlString = :sqlString || ' ,a.ExInfo4 = NULL';
		End If;
		
		IF :ex5Qry <> ''
		Then
			ex5Qry = replace(replace(replace(:ex5Qry, ':demandType','a.DemandType'), ':demandentry','a.DemandEntry'),':demandline','a.DemandLine');
			sqlString = :sqlString || ' ,a.ExInfo5 = (' || :ex5Qry || ')';
		Else
			sqlString = :sqlString || ' ,a.ExInfo5 = NULL';
		End If;

		sqlString = :sqlString || ' FROM #tempDataRun a ;';
		---print :sql
		exec :sqlString;
	END If;

	----------------------------------------- lấy line id để đánh số thứ tự ---------------------------------------------------------------------
	Select IFNULL((SELECT MAX("Line") FROM #tempDataRun),0) Into id From Dummy;

	------------------------------------------ Thêm dữ liệu các lệnh sản xuất đã có trên hệ thống không phải gen từ capacity này -----------------------------------------------------------
	INSERT into #tempDataRun
	SELECT NULL, :scheduleId, 1,a."StartDate", a."EndDate", b."PlannedQty", a."U_Shift",:id + ROW_NUMBER() over(order by a."ItemCode"), a."ItemCode", a."U_OpeCode", a."U_StartTime", a."U_EndTime", 
		'N', 'N', 'ApzProd', a."PlannedQty", 1, 1, 1,Cast(-1 AS int) As "FatherLine",c."ResName", d."OpeName", b."ItemCode", e."ItemName", NULL, NULL, NULL, 
		ADD_SECONDS(ADD_SECONDS(a."StartDate",(a."U_StartTime"/100)*3600),MOD(a."U_StartTime",100)*60), 
		ADD_SECONDS(ADD_SECONDS(a."EndDate",(a."U_EndTime"/100)*3600),MOD(a."U_EndTime",100)*60), 
		NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, b."DocEntry", 'G' "Complete" , 'P', NULL, NULL, NULL, NULL, NULL, NULL, NULL, a."EndDate"
	FROM WOR1 a Inner join OWOR b ON a."DocEntry" = b."DocEntry" 
			Inner join ORSC c On a."ItemCode" = c."ResCode"
			Inner join APZ_OOMD d ON a."U_OpeCode" = d."OpeCode"
			Inner join OITM e ON b."ItemCode" = e."ItemCode" 
	WHERE a."ItemType" = 290 And (b."Status" <> 'C' Or b."Status" <> 'L') 
				And (a."EndDate" Between :beginDate And :endDate Or a."EndDate" Between :beginDate And :endDate) And (IFNULL(b."U_OriginType",'') <> '800005' 
					Or IFNULL(b."U_OriginCode",-2) <> :existCode);

	SELECT * FROM #tempDataRun;
	
END


