

CREATE PROCEDURE APZ_GenPOFromSchedule (IN scheduleID bigint default 0, type varchar(1) default '', inputType nvarchar(1) default 'S')
AS
BEGIN
	----------- Lấy số không tuyệt đối dựa vào setting sap để so sách số lượng đã gen----------------------------------------------------------------
	declare zeroSap numeric(19,6);
	declare origintype nvarchar(20);
	declare mrpBranch smallint;
	
	zeroSap = ifnull((select power(10.0000000, -1 * "QtyDec") from OADM), 0.0000001);

	-------------------------------- Lấy origintype để so sánh trên lệnh sản xuất dựa và input type ------------------------------------------------
	 
	origintype = case when :inputType = 'S' then '800004' else '800005' end;

	------------- Lấy danh sách lệnh sản xuất đã lập kế hoạch trên schedule hoặc capacity----------------------------------------------------------------------------
	-----------------------------------------------------------------------------------------------------------------------------------------------------------------
	------------- Lấy danh sách từ schedule khi input type là schedule-----------------------------------------------------------------------------------------------
	create local temporary table #tempInput (
		 "BomVersion" nvarchar(55), 
		 "ItemCode" nvarchar(50),  
		 "LTime" int, 
		 "WorkDay" date, 
		 "Qty" decimal(19, 6),  
		 "Shift" nvarchar(20),
		 "Notes" nvarchar(254), 
		 "Line" int, 
		 "PSId" bigint, 
		 "DemandType" nvarchar(20), 
		 "DemandEntry" int, 
		 "DemandLine" int , 
		 "DueDate" date, 
		 "U_FnCode" nvarchar(50)
	);
	insert into #tempInput
		select  "BomVersion", "ItemCode", "LTime", "WorkDay", "Qty", "Shift", "Notes", "Line", "PSId", "DemandType", "DemandEntry", "DemandLine", "WorkDay" AS "DueDate", '' AS "U_FnCode" 
		from apz_PRS1 where :inputType = 'S' and  "PSId" = :scheduleID and  "TypePlanning" in ('2') AND ifnull("Qty",0) <> 0
	union all
	------------------------------ Lấy danh sách từ capacity khi input type là capacity -----------------------------------------------------------------------------
	select "BomVersion", "ItemCode", "LTime", b."StartDate", "Qty", "Shift", "Notes", a."Line", a."GrcId", "DemandType", "DemandEntry", "DemandLine", b."DueDate", ''
		 from apz_CGRC a
				----------------------------------------- Bảng chứa danh sách các lệnh so sách với lệnh sản xuất. Vì một lệnh sản xuất có thể dùng nhiều resource khác nhau nên hiện tại đang group lại và lấy id nhỏ nhất -------------------------------
				INNER JOIN (SELECT MIN("Line") AS "Line", MIN("WorkDay") AS "StartDate", MAX("EndWorkDay") AS "DueDate" FROM APZ_CGRC WHERE "ScheCode" = :scheduleID AND "Complete" IN ('C','G') GROUP BY "ProId") b ON a."Line" = b."Line"
			 where :inputType = 'C' and "ScheCode" = :scheduleID and a."Complete" in ('C','G') AND ifnull("Qty",0) <> 0;
	--------------------------------- END lấy danh sách lệnh --------------------------------------------------------------------------------------------------------

	------------------------------ Lấy danh sách lệnh chưa gen lệnh sản xuất nếu gen type là P ---------------------------------------------------------------------
	if :type = 'P' then -- Planned

		UPDATE a SET "U_FnCode" = b."ItemCode" FROM #tempInput a INNER JOIN RDR1 b ON a."DemandType" = '17' AND a."DemandEntry" = b."DocEntry" AND a."DemandLine" = b."LineNum";
		UPDATE a SET "U_FnCode" = b."ItemCode" FROM #tempInput a INNER JOIN APZ_FCT1 b ON a."DemandType" = '8000005' AND a."DemandEntry" = b."Code" AND a."DemandLine" = b."LineID";

		mrpBranch = IFNULL((SELECT a."BplID" FROM APZ_OMRP a INNER JOIN apz_OPRS b ON a."MrpCode" = b."MrpId" WHERE b."PSCode" = :scheduleID), -1);

		SELECT a."BomVersion", a."ItemCode", b."ItemName", CURRENT_DATE AS "PostDate", ADD_DAYS(a."WorkDay", IFNULL(a."LTime", 0)) AS "StartDate", IFNULL(a."Qty", 0) - IFNULL(d."GenQty", 0) AS "Quantity", a."Shift" AS "U_Shift", 
						case
							--default whs with branch
							when ifnull(f."BplID",0) != 0 and ifnull((select "DflWhs" from OBPL where "BPLId" = f."BplID"),'') != '' then (select "DflWhs" from OBPL where "BPLId" = f."BplID")
							--bom version
							when ifnull(a."BomVersion",'') <> '' and ifnull((SELECT max("ToWH") from APZ_OITT where "Version" = a."BomVersion"),'') <> '' then (SELECT max("ToWH") from APZ_OITT where "Version" = a."BomVersion")
							--whs bom
							when ifnull(c."ToWH",'') != '' then c."ToWH"
							--default whs
							else b."DfltWH"
						end
						as "Whse"
						, a."Notes" AS "Remark", :origintype AS "U_OriginType", a."Line" AS "U_OriginLine", a."PSId" AS "U_OriginCode", f."PrjCode" AS "Project", ifnull(g."Series", -1) AS "Series", ifnull(g."BPLId", -1) AS "BplID"
						, a."DemandType", a."DemandEntry", a."DemandLine", a."DueDate", a."U_FnCode", CASE WHEN a."DemandType" = '17' THEN "DemandLine" ELSE NULL END AS "U_OriginOrderLine", a."Qty" AS "PlanQty" 

		FROM #tempInput a 
			inner join apz_OPRS e on e."PSCode" = :scheduleID
			inner join APZ_OMRP f on f."MrpCode" = e."MrpId"
			INNER JOIN OITM b ON a."ItemCode" = b."ItemCode" 
            inner JOIN OITT c ON a."ItemCode" = c."Code"
			left join (SELECT SUM("PlannedQty") AS "GenQty", "U_OriginCode", "U_OriginLine" FROM OWOR WHERE "U_OriginType" = :origintype GROUP BY "U_OriginCode", "U_OriginLine") d on a."PSId" = d."U_OriginCode" and a."Line" = d."U_OriginLine"
			left join (SELECT TOP 1 "Series", "BPLId" FROM NNM1 WHERE ifnull("BPLId", -1) = :mrpBranch AND "ObjectCode" = '202') g on 1 = 1
        WHERE ifnull(a."Qty",0) - ifnull(d."GenQty",0) > :zeroSap  AND b."InvntItem"='Y'
		order by a."ItemCode", a."WorkDay";

	---------------------------------------------- END lấy danh sách tạo lệnh sản xuất -------------------------------------------------------------------------------------------------------------------------------
	---------------------------------- Lấy danh sách lênh đã gen nhưng chưa release ----------------------------------------------------------------------------------------------------------------------------------
	else if :type = 'R' then -- Release
		SELECT ifnull(c."BPLId", -1) AS "BplID", d."DocEntry" AS "PONo", d."DocNum" AS "PONum", a."ItemCode", b."ItemName", d."StartDate", d."PlannedQty" AS "Quantity", d."Project", d."Warehouse" AS "Whse", d."Comments" AS "Remark",
					 d."U_Shift", d."U_OriginCode", CURRENT_DATE AS "CloseDate", d."U_OriginLine", d."U_OriginType", d."PlannedQty" - ifnull(d."CmpltQty", 0) AS "ReportQty",
					ifnull(d."Series",-1) as "Series"
					,a."DemandType", a."DemandEntry", a."DemandLine", d."DueDate", d."U_FnCode", d."U_OriginOrderLine", a."Qty" AS "PlanQty"
		FROM #tempInput a
			inner join APZ_OPRS c on a."PSId" = c."PSCode"
			INNER JOIN OITM b ON a."ItemCode" = b."ItemCode"
			inner join (SELECT "U_OriginCode", "U_OriginLine", "U_OriginType", "U_Shift", "U_FnCode", "DocEntry", "U_OriginOrderLine", "DocNum", "StartDate", "DueDate", "PlannedQty", "Project", "Warehouse", "Comments", "CmpltQty", "Series" FROM OWOR WHERE "U_OriginType" = :origintype AND "Status" = 'P') d 
			ON a."PSId" = d."U_OriginCode" and a."Line" = d."U_OriginLine";


	---------------------------------------- END lấy danh sách lệnh sản xuất chưa release ------------------------------------------------------------------------------------------------------------------------------
	------------------------------------ Lấy danh sách lệnh sản xuất đã release nhưng chưa report ----------------------------------------------------------------------------------------------------------------------
	else if :type = 'E' then -- Report

		SELECT 'C' AS "TranType", d."DocEntry" AS "PONo", d."DocNum" AS "PONum", a."ItemCode", b."ItemName", d."StartDate", d."PlannedQty" AS "Quantity", d."Project", d."Warehouse" AS "Whse", d."Comments" AS "Remark", 
				d."U_Shift", d."U_OriginType", d."U_OriginCode", d."U_OriginLine", CURRENT_DATE AS "CloseDate", d."PlannedQty" - ifnull(d."CmpltQty", 0) AS "ReportQty", 
					ifnull(g."Series",-1) as "Series", ifnull(e."BplID",-1) as "BplID",
					ifnull((select sum(ifnull(a1."AvgPrice",0) * ifnull(a2."PlannedQty",0))
							from OITW a1 inner join WOR1 a2 on a1."ItemCode" = a2."ItemCode" and a1."WhsCode" = a2."wareHouse"
								where a2."DocEntry" = d."DocEntry"),0)/d."PlannedQty" as "Price"
								,a."DemandType", a."DemandEntry", a."DemandLine", d."DueDate" /*, d.U_FnCode, d.U_OriginOrderLine */, a."Qty" "PlanQty"
		FROM #tempInput a
			inner join APZ_OPRS c on a."PSId" = c."PSCode"
			inner join APZ_OMRP e on c."MrpId" = e."MrpCode"
			INNER JOIN OITM b ON a."ItemCode" = b."ItemCode"
			inner join (SELECT "U_OriginCode", "U_OriginLine", "U_OriginType", "U_Shift", "U_FnCode", "U_OriginOrderLine", "DocEntry", "DocNum", "StartDate", "DueDate", "PlannedQty", "Project", "Warehouse", "Comments", "CmpltQty", "Series" FROM OWOR WHERE "U_OriginType" = :origintype AND "Status" = 'R' AND "PlannedQty" > ifnull("CmpltQty", 0)) d 
			ON a."PSId" = d."U_OriginCode" and a."Line" = d."U_OriginLine"
			left join NNM1 g on ifnull(g."BPLId",-1) = ifnull(e."BplID",-1) and g."ObjectCode" = '59';

	-------------------------------------------- END lấy danh sách lệnh đã release nhưng chưa report -------------------------------------------------------------------------------------------------------------
	-------------------------------------------------- Lấy danh sách các lệnh sản xuất đã release -----------------------------------------------------------------------------------------------------------------
	else if :type = 'L' then-- Closed

		SELECT d."PlannedQty" - ifnull(d."CmpltQty", 0) AS "ReportQty", d."DocEntry" AS "PONo", d."DocNum" AS "PONum", a."ItemCode", b."ItemName", d."StartDate", d."PlannedQty" AS "Quantity", d."Project", d."Warehouse" AS "Whse", d."Comments" AS "Remark", 
				d."U_Shift", CURRENT_DATE AS "CloseDate", d."U_OriginType", d."U_OriginCode", d."U_OriginLine"
				,a."DemandType", a."DemandEntry", a."DemandLine", d."DueDate", d."U_FnCode", d."U_OriginOrderLine", a."Qty" AS "PlanQty"
		FROM #tempInput a
			INNER JOIN OITM b ON a."ItemCode" = b."ItemCode"
			inner join (SELECT "U_OriginCode", "U_OriginLine", "U_OriginType", "U_Shift", "U_FnCode", "U_OriginOrderLine", "DocEntry", "DocNum", "StartDate", "DueDate", "PlannedQty", "Project", "Warehouse", "Comments", "CmpltQty", "Series" FROM OWOR WHERE "U_OriginType" = :origintype AND "Status" = 'R' AND ifnull("CmpltQty", 0) <> 0) d 
			ON a."PSId" = d."U_OriginCode" and a."Line" = d."U_OriginLine";
	---------------------------------------------------------- END lấy danh sách các lệnh đã release ----------------------------------------------------------------------------------------------------------
	------------------------------------------------- Lấy danh sách các lệnh đã release hoặc đang ở dạng plan -------------------------------------------------------------------------------------------------
	else if :type = 'C' then -- Cancel

		SELECT d."PlannedQty" - ifnull(d."CmpltQty", 0) AS "ReportQty", d."DocEntry" AS "PONo", d."DocNum" AS "PONum", a."ItemCode", b."ItemName", d."StartDate", d."PlannedQty" AS "Quantity", d."Project", d."Warehouse" AS "Whse", d."Comments" AS "Remark", 
				d."U_Shift", CURRENT_DATE AS "CloseDate", d."U_OriginType", d."U_OriginCode", d."U_OriginLine"
				,a."DemandType", a."DemandEntry", a."DemandLine", d."DueDate", d."U_FnCode", d."U_OriginOrderLine", a."Qty" AS "PlanQty"
		FROM #tempInput a
			INNER JOIN OITM b ON a."ItemCode" = b."ItemCode"
			inner join (SELECT "U_OriginCode", "U_OriginLine", "U_OriginType", "U_Shift", "U_OriginOrderLine", "DocEntry", "U_FnCode", "DocNum", "StartDate", "DueDate", "PlannedQty", "Project", "Warehouse", "Comments", "CmpltQty", "Series" FROM OWOR WHERE "U_OriginType" = :origintype AND "Status" IN ('R','P') AND ifnull("CmpltQty", 0) = 0) d 
			ON a."PSId" = d."U_OriginCode" and a."Line" = d."U_OriginLine";
	end if;
	end if;
	end if;
	end if;
	end if;
	-------------------------------------------------------------- END Lấy danh sách các lệnh đã release và đang ở dạng plan ---------------------------------------------------------------------------------
		
END



