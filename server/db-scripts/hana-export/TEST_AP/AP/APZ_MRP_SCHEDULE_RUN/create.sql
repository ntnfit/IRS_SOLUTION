

CREATE PROCEDURE apz_mrp_schedule_run
(
	IN MrpCode int,
	scheduleType char(1),
	fdate DATE,
	tdate DATE,
	isDetailSchedule char(1)
)
AS
BEGIN
	create local temporary table #TypeScheduling ("TypePlanning" int, "Planning" nvarchar(200));
	insert into #TypeScheduling("TypePlanning", "Planning") values (1, N'1. MRP Plan QTY');
	insert into #TypeScheduling("TypePlanning", "Planning") values (2, N'2. Requirement QTY');
	insert into #TypeScheduling("TypePlanning", "Planning") values (3, N'3. Scheduling');
	insert into #TypeScheduling("TypePlanning", "Planning") values (4, N'4. MRP Remain');
	
	create local temporary table #TempItemCodeInSchedule ("ItemCode" nvarchar(50));
	insert into #TempItemCodeInSchedule ("ItemCode") select "ItemCode" from APZ_PRS1 a INNER JOIN (Select "PSCode" from APZ_OPRS Where "MrpId" = :MrpCode) b ON a."PSId" = b."PSCode" GROUP BY "ItemCode";
	
	if :scheduleType = 'O' then
		create local temporary table #TempPoShift ("LinID" int, "WorkDate" date, "Shift" nvarchar(50), "Planning" nvarchar(200));
		insert into #TempPoShift select a."LinID", a."WorkDate",a."Shift",b."Planning" from #TypeScheduling b,
																							(select "LinID", "WorkDate", "Shift" from  apz_WSH1 t
																							inner join apz_OWSH d on t.ID = d."Id"
																							inner join APZ_OMRP c on ifnull(c."BplID",-1) = ifnull(d."BplID",-1) and c."MrpCode" = :MrpCode
																							where t."WorkDate" between :fdate and :tdate
																							UNION ALL
																							SELECT 0 "LinID", add_days(:fdate, -1) "WorkDate", null "Shift" from dummy) a;
		
		SELECT DISTINCT "WorkDate", "Shift", "Planning" FROM #TempPoShift;
		
		select  a."ItemCode",
			    a."ItemName",
				b."Planning",
				c."WorkDate",
				sum(case when b."TypePlanning"=1 and c."LinID"=0 then a."Qty" else 0 end) as "Qty",
				c."Shift",
				0 "DocEntry",
				a."BomVersion",
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
			(Select "ItemCode", "ItemName","Qty", "BomVersion", "CardCode", "DueDate", "LTime"
					, case when :isDetailSchedule = 'Y' then "LineNum" else null end "BaseLine"
					, case when :isDetailSchedule = 'Y' then "DemandType" else null end "DemandType"
					, case when :isDetailSchedule = 'Y' then "DemandEntry" else null end "DemandEntry", case when :isDetailSchedule = 'Y' then "DemandLine" else null end "DemandLine"
					, case when :isDetailSchedule = 'Y' then "FatherCode" else null end "FatherCode" from apz_MRP1 where "OMrpId"=:MrpCode and "DocumentType"='M' and "Type" between 5000001 and 5100000 - 1 and ifnull("Qty",0)<>0 And "ItemCode" Not In (select * from #TempItemCodeInSchedule)) a
			inner join APZ_OMRP d on d."MrpCode" = :MrpCode
			inner join (SELECT DISTINCT "WorkDate", "Shift", "LinID" from  #TempPoShift) c on a."DueDate" = c."WorkDate"
			INNER JOIN OITM t ON a."ItemCode" = t."ItemCode"
				Group by a."ItemCode", a."ItemName", b."Planning", c."WorkDate", c."Shift", a."BomVersion", a."CardCode", a."LTime", t."ItemType", t."ItmsGrpCod", a."DemandType", a."DemandEntry", a."DemandLine", "FatherCode", a."BaseLine";
				
			drop table #TempPoShift;
	else
	
		create local temporary table #TempShift ("WorkDate" date, "Shift" nvarchar(50));
		insert into #TempShift select "WorkDate", "Shift" from APZ_WSH1 a inner join apz_OWSH b on a.ID = b."Id" 
						where ifnull(b."BplID",-1) = ifnull((select "BplID" from APZ_OMRP where "MrpCode" = :MrpCode),-1) and a."WorkDate" between :fdate and :tdate;
						
		create local temporary table #TempShiftDay ("WorkDate" date, "Shift" nvarchar(4000));
		insert into #TempShiftDay select "WorkDate", (SELECT string_agg("Shift", ',') FROM #TempShift WHERE "WorkDate" = a."WorkDate") as "Shift" from #TempShift a group by "WorkDate"
																							union all SELECT add_days(:fdate, -1) "WorkDate", null "Shift" from dummy;
																								
		select distinct a."WorkDate","Shift",b."Planning" from #TypeScheduling b, #TempShiftDay a;
		
		select  a."ItemCode",
			    a."ItemName",
				b."Planning",
				c."WorkDate",
				sum(case when b."TypePlanning"=1 then a."Qty" else 0 end) as "Qty",
				c."Shift",
				0 "DocEntry",
				a."BomVersion",
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
			(Select "ItemCode", "ItemName","Qty", "BomVersion", "CardCode", "DueDate", "LTime"
					, case when :isDetailSchedule = 'Y' then "LineNum" else null end "BaseLine"
					, case when :isDetailSchedule = 'Y' then "DemandType" else null end "DemandType"
					, case when :isDetailSchedule = 'Y' then "DemandEntry" else null end "DemandEntry", case when :isDetailSchedule = 'Y' then "DemandLine" else null end "DemandLine"
					, case when :isDetailSchedule = 'Y' then "FatherCode" else null end "FatherCode" from apz_MRP1 
					where "OMrpId"=:MrpCode and (:scheduleType = 'P' and "DocumentType"='B' and "Type" between 5100000 and 5999999 OR :scheduleType = 'T' and "Type" between 5000001 and 5100000 - 1 and exists(select 1 from ITT1 where "Code" = "ItemCode")) and IFNULL("Qty",0)<>0 And "ItemCode" Not In (select * from #TempItemCodeInSchedule)) a
			inner join #TempShiftDay c on c."WorkDate" = a."DueDate" 
			inner join OITM t ON a."ItemCode" = t."ItemCode"
				Group by a."ItemCode", a."ItemName", b."Planning", c."WorkDate", c."Shift", a."BomVersion", a."CardCode", a."LTime", t."ItemType", t."ItmsGrpCod", a."DemandType", a."DemandEntry", a."DemandLine", "FatherCode", a."BaseLine";
			
		drop table #TempShift;
		drop table #TempShiftDay;
		
	end if;
	
	drop table #TypeScheduling;
	drop table #TempItemCodeInSchedule;
END





