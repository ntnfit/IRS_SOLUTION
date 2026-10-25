

CREATE PROCEDURE APZ_Schedule_GetLine (IN PrsId int)
AS
BEGIN
	declare fdate date;
	declare tdate Date;
	declare isDetailSchedule char(1);
	Declare mrpCode int;
	
	select top 1 b."BeginDate" into fdate from APZ_OPRS a inner join APZ_OMRP b on a."MrpId" = b."MrpCode" where "PSCode" = :PrsId;
	select top 1 b."FCstDate" into tdate from APZ_OPRS a inner join APZ_OMRP b on a."MrpId" = b."MrpCode" where "PSCode" = :PrsId;
	select IFNULL((select "DetailSchedule" from APZ_OPRS where "PSCode" = :PrsId), 'N') into isDetailSchedule from dummy;
	Select top 1 "MrpId" into mrpCode from APZ_OPRS where "PSCode" = :PrsId;
	
	create local temporary table #TypeScheduling ("TypePlanning" int, "Planning" nvarchar(200));
	insert into #TypeScheduling("TypePlanning", "Planning") values (1, N'1. MRP Plan QTY');
	insert into #TypeScheduling("TypePlanning", "Planning") values (2, N'2. Requirement QTY');
	insert into #TypeScheduling("TypePlanning", "Planning") values (3, N'3. Scheduling');
	insert into #TypeScheduling("TypePlanning", "Planning") values (4, N'4. MRP Remain');

	create local temporary table #TempItemCodeInSchedule ("ItemCode" nvarchar(50));
	insert INTO #TempItemCodeInSchedule select "ItemCode" from APZ_PRS1 a INNER JOIN (Select "PSCode" from APZ_OPRS Where "MrpId" = :mrpCode) b ON a."PSId" = b."PSCode" GROUP BY "ItemCode";

	if (select top 1 "Type" from apz_OPRS where "PSCode" = :PrsId) = 'O' then
			
		CREATE local temporary TABLE #TempPoShift  ("LinID" int, "WorkDate" date, "Shift" nvarchar(50), "Planning" nvarchar(200));
		insert into #TempPoShift SELECT a."LinID", a."WorkDate", a."Shift", b."Planning" AS "Planning" FROM #TypeScheduling b, 
												(SELECT "LinID", "WorkDate", "Shift" FROM apz_WSH1 t
													inner join APZ_OPRS c on c."PSCode" = :PrsId
													inner join apz_OWSH e on t.ID = e."Id"
													inner join APZ_OMRP d on c."MrpId" = d."MrpCode" and ifnull(e."BplID",-1) = ifnull(d."BplID",-1)
															where t."WorkDate" between :fdate and :tdate
												UNION ALL
												SELECT 0, add_days(:fdate, -1) "WorkDate", null "Shift" from dummy) a;

		SELECT "WorkDate","Shift","Planning" FROM #TempPoShift;
												

		select  a."ItemCode",
				a."ItemName",
				b."Planning" as "Planning", 
				a."WorkDay" as "WorkDate",
				a."Qty", a."Shift", 
				ifnull(f."DocEntry",0) as "DocEntry",
				ifnull(a."BomVersion",'') "BomVersion", 
				a."CardCode",
				a."LTime",
				t."ItemType",
				t."ItmsGrpCod" "ItemGroup",
				a."DemandType",
				a."DemandEntry",
				a."DemandLine",
				a."FatherCode",
				a."BaseType", 
				a."BaseEntry", 
				a."BaseLine"
		from (SELECT "ItemCode", "ItemName", "WorkDay", "Qty", "Shift", "BomVersion", "CardCode", "TypePlanning", "PSId", "Line", "LTime", "DemandType", "DemandEntry", "DemandLine", "FatherCode", "BaseType", "BaseEntry", "BaseLine" FROM apz_PRS1 WHERE "PSId" = :PrsId) a 
		INNER JOIN OITM t ON a."ItemCode" = t."ItemCode"
		inner join #TypeScheduling b on a."TypePlanning" = b."TypePlanning" 
			left join (SELECT "U_OriginCode", "U_OriginLine", "DocEntry" FROM OWOR WHERE IFNULL("U_OriginType", '') = '800004') f on f."U_OriginCode" = a."PSId" and f."U_OriginLine" = a."Line"
			
		UNION ALL

		select	a."ItemCode",
				a."ItemName",
				b."Planning",
				c."WorkDate",
				sum(case when b."TypePlanning"=1 and c."LinID"=0 then a."Qty" else 0 end) as "Qty",
				c."Shift",
				0 "DocEntry",
				ifnull(a."BomVersion",'') "BomVersion",
				a."CardCode",
				a."LTime",
				t."ItemType",
				t."ItmsGrpCod" "ItemGroup",
				a."DemandType",
				a."DemandEntry",
				a."DemandLine",
				a."FatherCode",
				'700003' "BaseType",
				:MrpCode "BaseEntry",
				a."BaseLine"
			from  #TypeScheduling b, 
			(SELECT "ItemCode", "ItemName", "Qty", "BomVersion", "CardCode", "DueDate", "LTime"
				, case when :isDetailSchedule = 'Y' then "LineNum" else null end "BaseLine"
				, case when :isDetailSchedule = 'Y' then "DemandType" else null end "DemandType"
				, case when :isDetailSchedule = 'Y' then "DemandEntry" else null end "DemandEntry", case when :isDetailSchedule = 'Y' then "DemandLine" else null end "DemandLine"
				, case when :isDetailSchedule = 'Y' then "FatherCode" else null end "FatherCode" from apz_MRP1 where "OMrpId"=:MrpCode and "DocumentType"='M' and "Type" between 5000001 and 5100000 - 1 and ifnull("Qty",0)<>0 And "ItemCode" Not In (select * from #TempItemCodeInSchedule)) a
			inner join APZ_OMRP d on d."MrpCode" = :MrpCode
			inner join (SELECT DISTINCT "WorkDate", "Shift", "LinID" from  #TempPoShift) c on a."DueDate" = c."WorkDate"
			INNER JOIN OITM t ON a."ItemCode" = t."ItemCode"
				group by a."ItemCode",a."ItemName", b."Planning", c."WorkDate", c."Shift", a."BomVersion", a."CardCode", a."LTime", t."ItemType", t."ItmsGrpCod", a."DemandType", a."DemandEntry", a."DemandLine", a."FatherCode", a."BaseLine";
				
				
			drop table #TempItemCodeInSchedule;
			drop table #TempPoShift;

		else

			create local temporary table #tempShift ("WorkDate" date, "Shift" nvarchar(50));
			insert into #tempShift 
			select "WorkDate", "Shift" from APZ_WSH1 a inner join apz_OWSH b on a.ID = b."Id"
						where ifnull(b."BplID",-1) = ifnull((select c."BplID" from APZ_OMRP c inner join APZ_OPRS d on c."MrpCode" = d."MrpId"
								 where d."PSCode" = :PrsId),0) and a."WorkDate" between :fdate and :tdate;
								 
			create local temporary table #tempShiftDistinct as (select distinct "Shift", "WorkDate" from #tempShift);
			create local temporary table #tempShiftDay("WorkDate" date, "Shift" nvarchar(4000));
			insert into #tempShiftDay
				select "WorkDate", (select string_agg("Shift", ',') from #tempShiftDistinct where "WorkDate" = a."WorkDate") AS "Shift"
				from #tempShift a Group by "WorkDate"
				UNION ALL
				SELECT add_days(:fdate, -1) "WorkDate", null "Shift" from dummy;

			drop table #tempShiftDistinct;
			
			select distinct a."WorkDate","Shift",b."Planning"  from #TypeScheduling b, #tempShiftDay a;
			
			create local temporary table #output (
				"ItemCode" nvarchar(50),
				"ItemName" nvarchar(254),
				"Planning" nvarchar(200),
				"WorkDate" date,
				"Qty" decimal(21, 6), 
				"Shift" nvarchar(50),
				"DocEntry" int,
				"BomVersion" nvarchar(55),
				"CardCode" nvarchar(15),
				"LTime" int,
				"DemandType" nvarchar(20),
				"DemandEntry" int,
				"DemandLine" int,
				"FatherCode" nvarchar(50),
				"BaseType" nvarchar(20),
				"BaseEntry" int,
				"BaseLine" int
			);
			
			insert INTO #output
			SELECT a."ItemCode", a."ItemName", b."Planning" AS "Planning", a."WorkDay" AS "WorkDate", a."Qty", a."Shift", case when ifnull(f."DocEntry",0) <> 0 then f."DocEntry"
																								when  ifnull(h."DocEntry",0) <> 0 then h."DocEntry"
																								else ifnull(h1."DocEntry",0)
																								 end "DocEntry",
											ifnull(a."BomVersion",'') "BomVersion", a."CardCode", a."LTime", a."DemandType", a."DemandEntry", a."DemandLine", "FatherCode", a."BaseType", a."BaseEntry", a."BaseLine"
				from (SELECT "ItemCode", "ItemName", "WorkDay", "Qty", "Shift", "BomVersion", "CardCode", "TypePlanning", "PSId", "Line", "LTime", "DemandType", "DemandEntry", "DemandLine", "FatherCode", "BaseType", "BaseEntry", "BaseLine" FROM apz_PRS1 WHERE "PSId" = :PrsId) a 
					 inner join #TypeScheduling b on a."TypePlanning" = b."TypePlanning" 
					 --Join purchase order
					 left join (SELECT "U_OriginCode", "U_OriginLine", MAX("DocEntry") AS "DocEntry" FROM POR1 WHERE ifnull("U_OriginType", '') = '800004' GROUP BY "U_OriginCode", "U_OriginLine") f on f."U_OriginCode" = a."PSId" and f."U_OriginLine" = a."Line"
					 --Join purchase request
					 left join (SELECT "U_OriginCode", "U_OriginLine", MAX("DocEntry") AS "DocEntry" FROM PRQ1 t1 WHERE ifnull("U_OriginType", '') = '800004' AND ("LineStatus" = 'O' OR ("LineStatus" = 'C' AND "DocEntry" IN 
					 			(SELECT "BaseEntry" FROM POR1 WHERE "ItemCode" = t1."ItemCode"))) GROUP BY "U_OriginCode", "U_OriginLine") h on a."PSId" = h."U_OriginCode" and h."U_OriginLine" = a."Line"
					 --JOIN Purachase order  and purcahse request have been created
					 LEFT JOIN (SELECT "U_OriginCode", "U_OriginLine", MAX(t1."DocEntry") AS "DocEntry" FROM DRF1 t1 INNER JOIN ODRF t2 ON t1."DocEntry" = t2."DocEntry" 
					 			WHERE ifnull("U_OriginType", '') = '800004' AND t2."ObjType" IN ('22','1470000113') AND t2."WddStatus" = 'W' GROUP BY "U_OriginCode", "U_OriginLine") h1
							on a."PSId" = h1."U_OriginCode" and h1."U_OriginLine" = a."Line";

			INSERT INTO #output
				select	a."ItemCode",
						a."ItemName",
						b."Planning",
						c."WorkDate",
						sum(case when b."TypePlanning"=1 then a."Qty" else 0 end) as "Qty", 
						c."Shift",
						0 "DocEntry",
						ifnull(a."BomVersion",'') "BomVersion",
						a."CardCode",
						a."LTime",
						a."DemandType",
						a."DemandEntry",
						a."DemandLine",
						a."FatherCode",
						'700003' "BaseType",
						:MrpCode "BaseEntry",
						a."BaseLine"
				from  #TypeScheduling b, 
				(SELECT "ItemCode", "ItemName", "Qty", "BomVersion", "CardCode", "DueDate", "LTime"
					, case when :isDetailSchedule = 'Y' then "LineNum" else null end "BaseLine"
					, case when :isDetailSchedule = 'Y' then "DemandType" else null end "DemandType"
					, case when :isDetailSchedule = 'Y' then "DemandEntry" else null end "DemandEntry", case when :isDetailSchedule = 'Y' then "DemandLine" else null end "DemandLine"
					, case when :isDetailSchedule = 'Y' then "FatherCode" else null end "FatherCode" from apz_MRP1 where "OMrpId"=:MrpCode and "DocumentType"='B' and "Type" between 5100000 and 5999999 and ifnull("Qty",0)<>0 And "ItemCode" Not In (select * from #TempItemCodeInSchedule)) a
				inner join #tempShiftDay c on c."WorkDate" = a."DueDate" 
					group by a."ItemCode",a."ItemName",b."Planning",c."WorkDate",c."Shift",a."BomVersion",a."CardCode",a."LTime",a."DemandType",a."DemandEntry",a."DemandLine", a."FatherCode", a."BaseLine";

			SELECT a.*, t."ItemType", t."ItmsGrpCod" AS "ItemGroup" FROM #output a INNER JOIN OITM t ON a."ItemCode" = t."ItemCode";
			
			
			drop table #tempShift;
			drop table #tempShiftDay;
			drop table #output;
			
	END IF;
	
	drop table #TypeScheduling;
END



