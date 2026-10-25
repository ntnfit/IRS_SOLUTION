
CREATE PROCEDURE USP_APZ_MATERIAL_SHORTAGE(IN fdate date, IN tdate date, IN excludePrd nvarchar(1000) DEFAULT '')
AS 
BEGIN

	DECLARE columUnPivot nvarchar(254) = 'StockPlan,Demand,Ordered,Shortage,AllDemand,AllAvailable,AllShortage';
	declare tbl table (WOID int, "PrdNo" nvarchar(50), "PrdName" nvarchar(100), "PrdUoM" nvarchar(100), "PrdPlnQty" decimal(19,6), "PrdOpnQty" decimal(19,6), "SubCode" nvarchar(50), "SubName" nvarchar(100), "SubUoM" nvarchar(20), "JobNo" nvarchar(30), 
						"StockPlan" decimal(19,6), "Demand" decimal(19,6), "Ordered" decimal(19,6), "Shortage" decimal(19,6), "AllDemand" decimal(19,6), "AllAvailable" decimal(19,6), "AllShortage" decimal(19,6));
	
	if :excludePrd is null
	THEN
		excludePrd = '';
	END IF;
	
	insert into :tbl (WOID,"PrdNo","PrdUoM","PrdPlnQty","PrdOpnQty","SubCode","SubName","SubUoM","Demand")
	select a."DocEntry",a."ItemCode", a."Uom", a."PlannedQty",case when a."PlannedQty">a."CmpltQty"+a."RjctQty" then a."PlannedQty"-a."CmpltQty"-a."RjctQty" else 0 end, b."ItemCode", c."ItemName", c."InvntryUom", case when b."PlannedQty"+b."AdditQty">b."IssuedQty" then b."PlannedQty"+b."AdditQty"-b."IssuedQty" else 0 end
	from OWOR a, WOR1 b, OITM c
	where a."DocEntry"=b."DocEntry" and b."ItemCode"=c."ItemCode" and c."InvntItem"='Y' and a."DueDate" between :fdate and :tdate
	and cast(a."DocEntry" as nvarchar) not in (select Data from Split(:excludePrd,','));
	-------------------------
	-- Danh mục dữ liệu
	create local temporary table #SO ("ItemCode" nvarchar(50), "WhsCode" nvarchar(8), "ShipDate" date, "OpnQty" decimal(21,6));
	insert into #SO("ItemCode", "WhsCode", "ShipDate", "OpnQty") select b."ItemCode",b."WhsCode",b."ShipDate",b."OpenInvQty"
										from ORDR a, RDR1 b where a."DocEntry"=b."DocEntry" and a.CANCELED='N' and a."DocStatus"='O' and b."LineStatus"='O' and b."OpenInvQty">0 and b."ShipDate"<= :tdate;
	
	
	create local temporary table #PO ("ItemCode" nvarchar(50), "WhsCode" nvarchar(8), "ShipDate" date, "OpnQty" decimal(21,6));
	insert into #PO("ItemCode", "WhsCode", "ShipDate", "OpnQty") select b."ItemCode",b."WhsCode",b."ShipDate",b."OpenInvQty"
										from OPOR a, POR1 b where a."DocEntry"=b."DocEntry" and a.CANCELED='N' and a."DocStatus"='O' and b."LineStatus"='O' and b."OpenInvQty">0 and b."ShipDate"<= :tdate;
									
	create local temporary table #PROD_H ("ItemCode" nvarchar(50), "Warehouse" nvarchar(8), "DueDate" date, "OpnQty" decimal(21,6));
	insert into #PROD_H("ItemCode", "Warehouse", "DueDate", "OpnQty") select a."ItemCode",a."Warehouse",a."DueDate",case when a."Type"='D' then -(a."PlannedQty"-a."CmpltQty"-a."RjctQty") else (a."PlannedQty"-a."CmpltQty"-a."RjctQty") end 
										from OWOR a where a."DueDate"<=:tdate and a."Status" in ('P','R') and a."PlannedQty">a."CmpltQty"+a."RjctQty";	
	
	
	create local temporary table #PROD_L ("ItemCode" nvarchar(50), "Warehouse" nvarchar(8), "DueDate" date, "OpnQty" decimal(21,6));
	insert into #PROD_L ("ItemCode", "Warehouse", "DueDate", "OpnQty") select b."ItemCode",b."wareHouse",a."DueDate",case when a."Type"='D' then -(b."PlannedQty"+b."AdditQty"-b."IssuedQty") else (b."PlannedQty"+b."AdditQty"-b."IssuedQty") end as "OpnQty" 
										from OWOR a, WOR1 b where a."DocEntry"=b."DocEntry" and a."DueDate"<=:tdate and a."Status" in ('P','R') and b."PlannedQty"+b."AdditQty">b."IssuedQty";
	
	-------------------------
	update :tbl set "StockPlan"	= ifnull((select sum(x."OpnQty") from #PO x where x."ItemCode"="SubCode" and x."ShipDate"<:fdate),0)
								- ifnull((select sum(x."OpnQty") from #SO x where x."ItemCode"="SubCode" and x."ShipDate"<:fdate),0)
								+ ifnull((select sum(y."OpnQty") from #PROD_H y where y."ItemCode"="SubCode" and y."DueDate"<:fdate),0)
								- ifnull((select sum(z."OpnQty") from #PROD_L z where z."ItemCode"="SubCode" and z."DueDate"<:fdate),0);
	update :tbl set "Ordered"		= ifnull((select sum(x."OpnQty") from #PO x where x."ItemCode"="SubCode" and x."ShipDate" between :fdate and :tdate),0)
								+ ifnull((select sum(y."OpnQty") from #PROD_H y where y."ItemCode"="SubCode" and y."DueDate" between :fdate and :tdate and y."OpnQty">0),0)
								+ ifnull((select sum(-z."OpnQty") from #PROD_L z where z."ItemCode"="SubCode" and z."DueDate" between :fdate and :tdate and z."OpnQty"<0),0);
	update :tbl set "Shortage" = case when "Demand"-"StockPlan"-"Ordered">0 then "Demand"-"StockPlan"-"Ordered" else 0 end;
	-----------------------------

	update t1 set "AllDemand" = t2."AllDemand" from :tbl t1 inner join (select "SubCode", sum("Demand") as "AllDemand" from :tbl group by "SubCode") t2 on t1."SubCode"=t2."SubCode";
	
	update :tbl set "AllAvailable" = case when ifnull("StockPlan",0) + ifnull("Ordered",0) - ifnull("AllDemand",0)>0 then ifnull("StockPlan",0) + ifnull("Ordered",0) - ifnull("AllDemand",0) else 0 end;
	
	update :tbl set "AllShortage" = case when ifnull("StockPlan",0) + ifnull("Ordered",0) - ifnull("AllDemand",0)<0 then -(ifnull("StockPlan",0) + ifnull("Ordered",0) - ifnull("AllDemand",0)) else 0 end;
	-----------------------------------------------------
	update t1 set "PrdName" = t2."ItemName" from OITM t2 inner join :tbl t1 on t2."ItemCode"=t1."PrdNo";
	

				  
	select WOID,"PrdNo","PrdName","PrdUoM","PrdOpnQty","PrdPlnQty","SubCode","SubName","SubUoM","JobNo", 
	
	--unpivot
			MAP(element_number,
				1, 'StockPlan',
				2, 'Demand', 
				3, 'Ordered', 
				4, 'Shortage', 
				5, 'AllDemand', 
				6, 'AllAvailable', 
				7, 'AllShortage'
				) as "Note",
			MAP(element_number,
				1, "StockPlan",
				2, "Demand", 
				3, "Ordered", 
				4, "Shortage", 
				5, "AllDemand", 
				6, "AllAvailable", 
				7, "AllShortage"
				) as "Qty",
			0 "LineId", 0 "Code"
			
		FROM :tbl, SERIES_GENERATE_INTEGER(1, 1, 7)
			order by element_number;
	
END;




