create PROCEDURE VAS_VT01_DN (
	in list_items table (Code nvarchar(50)),
	in list_whses table (Code nvarchar(30)),
	in fdate timestamp,
	in tdate timestamp,
	in grpAct nvarchar(1),
	in branch int,
	out tab_IV table (
		ItemCode	nvarchar(50),
		ItemName	nvarchar(100),
		InvntryUom	nvarchar(50),
		ItmsGrpCod	int,
		ItmsGrpNam	nvarchar(100),
		Warehouse	nvarchar(30),
		OBQty		numeric(21,6),
		OBPrice		numeric(21,6),
		OBVal		numeric(21,6),
		InQty		numeric(21,6),
		InPrice		numeric(21,6),
		InVal		numeric(21,6),
		OutQty		numeric(21,6),
		OutPrice	numeric(21,6),
		OutVal		numeric(21,6),
		EBQty		numeric(21,6),
		EBPrice		numeric(21,6),
		EBVal		numeric(21,6),
		InvntAct	nvarchar(30),
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		RptCurr		nvarchar(30),
		DeciQty		smallint,
		DeciPrice	smallint,
		DeciAmt		smallint,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	)
)
AS
BEGIN
	create local temporary table #IVOP (
		ItemCode	nvarchar(50),
		InvntAct	nvarchar(30),
		WhsCode		nvarchar(30),
		Qty			numeric(21,6),
		Val			numeric(21,6)
	);
	
	insert into #IVOP (ItemCode,InvntAct,WhsCode,Qty,Val)
	select
		b."ItemCode",
		IFNULL(MAX(b."InvntAct"),''),
		b."Warehouse",
		SUM(IFNULL(b."InQty",0) - IFNULL(b."OutQty",0)),
		SUM(b."TransValue")
	from
		:list_items a
		inner join OINM b on a.Code = b."ItemCode"
		inner join :list_whses c on b."Warehouse" = c.Code
		inner join OWHS d on (d."WhsCode"=b."Warehouse" and (d."BPLid"=branch or branch=0))
	where b."DocDate" < :fdate
	group by b."ItemCode", (CASE when :grpAct = 'Y' then b."InvntAct" else '' end), b."Warehouse";
	
	update #IVOP set InvntAct = '' where :grpAct = 'N';
	
	create local temporary table #IVIP (
		ItemCode	nvarchar(50),
		InvntAct	nvarchar(30),
		WhsCode		nvarchar(30),
		InQty		numeric(21,6),
		InVal		numeric(21,6),
		OutQty		numeric(21,6),
		OutVal		numeric(21,6)
	);
	
	insert into #IVIP (ItemCode,InvntAct,WhsCode,InQty,InVal,OutQty,OutVal)
	select
		b."ItemCode",
		IFNULL(MAX(b."InvntAct"),''),
		b."Warehouse",
		SUM(CASE when b."TransType" in ('14','16','18','20','69','162') then IFNULL(b."InQty",0)-IFNULL(b."OutQty",0) else CASE when b."TransType" not in ('13','15','19','21') then b."InQty" end end),
		SUM(CASE when b."TransType" in ('14','16','18','20','69','162') then b."TransValue" else CASE when b."TransType" not in ('13','15','19','21') then CASE when b."TransValue" > 0 then b."TransValue" end end end),
		SUM(CASE when b."TransType" in ('13','15','19','21') then -IFNULL(b."InQty",0)+IFNULL(b."OutQty",0) else CASE when b."TransType" not in ('14','16','18','20','69','162') then b."OutQty" end end),
		SUM(CASE when b."TransType" in ('13','15','19','21') then -b."TransValue" else CASE when b."TransType" not in ('14','16','18','20','69','162') then CASE when b."TransValue" < 0 then -b."TransValue" end end end)
	from
		:list_items a
		inner join OINM b on a.Code = b."ItemCode"
		inner join :list_whses c on b."Warehouse" = c.Code
		inner join OWHS d on (d."WhsCode"=b."Warehouse" and (d."BPLid"=branch or branch=0))
	where b."DocDate" between :fdate and :tdate
	group by b."ItemCode", (CASE when :grpAct = 'Y' then b."InvntAct" else '' end), b."Warehouse";
	
	update #IVIP set InvntAct = '' where :grpAct = 'N';
	
	create local temporary table #IVEP (
		ItemCode	nvarchar(50),
		ItemName	nvarchar(100),
		InvntryUom	nvarchar(50),
		ItmsGrpCod	int,
		ItmsGrpNam	nvarchar(100),
		Warehouse	nvarchar(30),
		OBQty		numeric(21,6),
		OBPrice		numeric(21,6),
		OBVal		numeric(21,6),
		InQty		numeric(21,6),
		InPrice		numeric(21,6),
		InVal		numeric(21,6),
		OutQty		numeric(21,6),
		OutPrice	numeric(21,6),
		OutVal		numeric(21,6),
		EBQty		numeric(21,6),
		EBPrice		numeric(21,6),
		EBVal		numeric(21,6),
		InvntAct	nvarchar(30),
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		RptCurr		nvarchar(30),
		DeciQty		smallint,
		DeciPrice	smallint,
		DeciAmt		smallint,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	);
	
	insert into #IVEP (ItemCode,Warehouse,OBQty,OBVal,InQty,InVal,OutQty,OutVal,InvntAct)
	select
		IFNULL(a.ItemCode,b.ItemCode),
		IFNULL(a.WhsCode,b.WhsCode),
		IFNULL(a.Qty,0),
		IFNULL(a.Val,0),
		IFNULL(b.InQty,0),
		IFNULL(b.InVal,0),
		IFNULL(b.OutQty,0),
		IFNULL(b.OutVal,0),
		IFNULL(a.InvntAct,b.InvntAct)
	from
		#IVOP a
		full outer join #IVIP b on a.ItemCode = b.ItemCode and a.InvntAct = b.InvntAct and a.WhsCode = b.WhsCode;
	
	delete from #IVEP where IFNULL(OBQty,0) = 0 and IFNULL(InQty,0) = 0 and IFNULL(OutQty,0) = 0 and IFNULL(OBVal,0) = 0 and IFNULL(InVal,0) = 0 and IFNULL(OutVal,0) = 0;
	
	update #IVEP set
		EBQty = IFNULL(OBQty,0) + IFNULL(InQty,0) - IFNULL(OutQty,0),
		EBVal = IFNULL(OBVal,0) + IFNULL(InVal,0) - IFNULL(OutVal,0);
	
	update #IVEP set
		OBPrice = IFNULL(OBVal/NULLIF(OBQty,0),0),
		InPrice = IFNULL(InVal/NULLIF(InQty,0),0),
		OutPrice = IFNULL(OutVal/NULLIF(OutQty,0),0),
		EBPrice = IFNULL(EBVal/NULLIF(EBQty,0),0);
	
	update tbl set ItemName = a."ItemName", InvntryUom = a."InvntryUom", ItmsGrpCod = b."ItmsGrpCod", ItmsGrpNam = b."ItmsGrpNam" from #IVEP tbl, OITM a, OITB b where a."ItmsGrpCod" = b."ItmsGrpCod" and a."ItemCode" = tbl.ItemCode;
		
	update tbl set CompName = a."PrintHeadr", CompFName = a."PrintHdrF" from #IVEP tbl, OADM a;
		
	update tbl set CompAdd = a."Street", CompFAdd = a."StreetF" from #IVEP tbl, ADM1 a;
	
	update tbl set RptCurr = a."MainCurncy", DeciQty = a."QtyDec", DeciPrice = a."PriceDec", DeciAmt = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from #IVEP tbl, OADM a;
	
	update tbl set DeciPrice = IFNULL(NULLIF(a."Decimals",-1),tbl.DeciPrice), DeciAmt = IFNULL(NULLIF(a."Decimals",-1),tbl.DeciAmt) from #IVEP tbl, OCRN a where a."CurrCode" = tbl.RptCurr;
	
	tab_IV = select * from #IVEP;
	
	drop table #IVOP;
	drop table #IVIP;
	drop table #IVEP;
END

