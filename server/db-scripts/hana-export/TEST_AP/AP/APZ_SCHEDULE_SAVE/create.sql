

CREATE PROCEDURE APZ_Schedule_Save (IN type char(1))
AS
BEGIN
	Declare docId bigint = 0;
	declare schetype char(1);
	declare mrpCode int;
	declare line int;
	
	create local temporary table #tempDocLocal as (select top 0 * from APZ_OPRS);
	exec 'drop table #tempDocLocal;';
	exec 'create local temporary table #tempDocLocal as (select * from "#tempdoc");';
	
	create local temporary table #tempLineLocal as (select top 0 *, '' "Planning", '' "WorkDate" from APZ_PRS1);
	exec 'drop table #tempLineLocal;';
	exec 'create local temporary table #tempLineLocal as (select * from "#templine");';
	
	IF not exists(Select top 1 "ItemCode" from #tempLineLocal where "Planning" = N'2. Requirement QTY' and ifnull("Qty",0) > 0) then
		SELECT 1300020, 'You must enter requiment quantity before save schedule' from dummy;
		return;
	end if;
	
	if :type = 'U' then

		Select top 1 "Type" into schetype from #tempDocLocal;
		select top 1 "PSCode" into docId from #tempDocLocal;
		
		IF EXISTS(SELECT 1 from APZ_OMRP WHERE comparesplit(cast(:docId as nvarchar), "PRSId", ',') = 1 and "Status" <> 'D') then
			select 130003, 'Could not update Schedule. Schedule was generated mrp' from dummy;
			return;
		END IF;
		
		IF IFNULL(:schetype,'') = '' then
			Select 130002, 'Could not save schedule. Schedule type could not null' from dummy;
			return;
		END IF;

		IF :schetype = 'O' then
			delete from APZ_PRS1 where "PSId" = :docId and not exists (select 1 from OWOR where "U_OriginType" = '800004' and "U_OriginCode" = "PSId" and "U_OriginLine" = "Line");
		ELSE IF :schetype = 'T' then
				delete from APZ_PRS1 where "PSId" = :docId and not exists (select 1 from 
												(select "U_OriginCode", "U_OriginLine" from WTR1 where ifnull("U_OriginType",'') = '800004'
												UNION ALL
												select "U_OriginCode", "U_OriginLine" from WTQ1 where ifnull("U_OriginType",'') = '800004'
												UNION ALL
												Select "U_OriginCode", "U_OriginLine" from DRF1 where ifnull("U_OriginType",'') = '800004') a where a."U_OriginCode" = "PSId" and a."U_OriginLine" = "Line");
			ELSE

				delete from APZ_PRS1 where "PSId" = :docId and not exists (select 1 from
												(select "U_OriginCode", "U_OriginLine" from POR1 where ifnull("U_OriginType",'') = '800004'
												UNION ALL
												select "U_OriginCode", "U_OriginLine" from PRQ1 where ifnull("U_OriginType",'') = '800004'
												UNION ALL
												select "U_OriginCode", "U_OriginLine" from WTR1 where ifnull("U_OriginType",'') = '800004'
												UNION ALL
												select "U_OriginCode", "U_OriginLine" from WTQ1 where ifnull("U_OriginType",'') = '800004'
												UNION ALL
												Select "U_OriginCode", "U_OriginLine" from DRF1 where ifnull("U_OriginType",'') = '800004') a where a."U_OriginCode" = "PSId" and a."U_OriginLine" = "Line");
			END IF;
		END IF;
		
		delete from APZ_OPRS where "PSCode" = :docId;
		delete from #tempLineLocal where ("ItemCode", "WorkDate","Shift") in (select "ItemCode", "WorkDate","Shift" from APZ_PRS1 where "PSId" = :docId);

	else

		select ifnull((select MAX("PSCode") from APZ_OPRS), 0) + 1 into docId from dummy;
		update #tempDocLocal set "PSCode" = :docId;
	end if;

	create local temporary table #tempItemCodeExists as (select "ItemCode" from #tempLineLocal where "Planning" = N'2. Requirement QTY' and ifnull("Qty",0) > 0 GROUP BY "ItemCode");

	Select "MrpId" into mrpCode from #tempDocLocal;

	IF EXISTS (SELECT 1 from #tempItemCodeExists a INNER JOIN (Select "ItemCode" FROM apz_PRS1 t1 INNER JOIN APZ_OPRS t2 ON t1."PSId" = t2."PSCode" and t2."MrpId" = :mrpCode )b ON a."ItemCode"= b."ItemCode")
	then
		SELECT 130001, 'Could not create two schedule of mrp for item' from dummy;
		return;
	END IF;

	create local temporary table #TypeScheduling ("TypePlanning" int,"Planning"	nvarchar(200));
	insert into #TypeScheduling("TypePlanning", "Planning") values (1, N'1. MRP Plan QTY');
	insert into #TypeScheduling("TypePlanning", "Planning") values (2, N'2. Requirement QTY');
	insert into #TypeScheduling("TypePlanning", "Planning") values (3, N'3. Scheduling');
	insert into #TypeScheduling("TypePlanning", "Planning") values (4, N'4. MRP Remain');

	insert into apz_OPRS ("PSCode", "PSName", "MrpId", "Status","Type", "CreateDate", "DetailSchedule")
	select "PSCode", "PSName", "MrpId" , "Status", "Type", "CreateDate", "DetailSchedule" from #tempDocLocal;
	select ifnull((select max("Line") from APZ_PRS1 where "PSId" = :docId),0) into line from dummy;

	INSERT INTO apz_PRS1 ("PSId", "ItemCode", "ItemName", "TypePlanning", "WorkDay", "Qty", "Shift", "Line", "BomVersion", "CardCode", "LTime", "DemandType", "DemandEntry", "DemandLine", "FatherCode", "BaseType", "BaseEntry", "BaseLine")
	SELECT :docId AS "PSId", a."ItemCode", a."ItemName", b."TypePlanning", a."WorkDate", a."Qty", a."Shift", :line + ROW_NUMBER() OVER ( ORDER BY a."ItemCode" DESC), a."BomVersion", a."CardCode", "LTime", a."DemandType", a."DemandEntry", a."DemandLine", "FatherCode", "BaseType", "BaseEntry", "BaseLine"
		from #tempLineLocal a
			 INNER JOIN #tempItemCodeExists t ON a."ItemCode" = t."ItemCode"
			 inner join #TypeScheduling b on a."Planning" = b."Planning"
			where  Ifnull(a."Qty", 0) > 0;
	
	Update ONNM SET "AutoKey" = IFNULL((select max("PSCode") from APZ_OPRS),0) + 1 where "ObjectCode" = '800004';
	select :docId, '' from dummy;
	
	drop table #tempDocLocal;
	drop table #tempLineLocal;
	drop table #tempItemCodeExists;
	drop table #TypeScheduling;
END




