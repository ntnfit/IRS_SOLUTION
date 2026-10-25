create procedure VAS_S10_DN (
	in list_Items	table (Code nvarchar(50)),
	in list_Accs	table (Code nvarchar(30)),
	in list_Whss	table (Code nvarchar(30)),
	in fdate		timestamp,
	in tdate		timestamp,
	in branch		int,
	out tab_IVL		table (
		Period		smallint,
		ID			int,
		TransNum	int,
		ItemCode	nvarchar(50),
		ItemName	nvarchar(100),
		ItemFName	nvarchar(100),
		InvntAct	nvarchar(30),
		AcctName	nvarchar(100),
		AcctFName	nvarchar(100),
		ContraAct	nvarchar(30),
		WhsCode		nvarchar(30),
		WhsName		nvarchar(100),
		DocType		nvarchar(30),
		DocEntry	int,
		DocLine		int,
		DocNum		int,
		DocNo		nvarchar(50),
		DocDate		timestamp,
		JEMemo		nvarchar(300),
		DocMemo		nvarchar(300),
		IOPrice		numeric(21,6),
		InQty		numeric(21,6),
		InVal		numeric(21,6),
		OutQty		numeric(21,6),
		OutVal		numeric(21,6),
		BalQty		numeric(21,6),
		BalVal		numeric(21,6),
		Vendor		nvarchar(100),
		JEUrl		nvarchar(100),
		DocUrl		nvarchar(100),
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		RptCurr		nvarchar(20),
		FormNo		nvarchar(200),
		IssueForm	nvarchar(500),
		FormNoF		nvarchar(200),
		IssueFormF	nvarchar(500),
		DeciQty		int,
		DeciPrice	int,
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	)
)
as
begin
	create local temporary table #IVL as (
		select * from :tab_IVL
	);
	
	insert into #IVL (Period, ID, ItemCode, InvntAct, WhsCode, JEMemo, DocMemo, InQty, InVal)
	select
		0,
		0,
		a."ItemCode",
		IFNULL(b."InvntAcc",a."InvntAct"),
		a."Warehouse",
		N'Số dư đầu kỳ / Opening balance',
		N'Số dư đầu kỳ / Opening balance',
		SUM(IFNULL(a."InQty",0)-IFNULL(a."OutQty",0)),
		SUM(a."TransValue")
	from
		OINM a
		left join AZ_IVL0 b on a."TransNum" = b."TransNum"
		inner join :list_Items c on a."ItemCode" = c.Code
		inner join :list_Accs d on IFNULL(b."InvntAcc",a."InvntAct") = d.Code
		inner join :list_Whss e on a."Warehouse" = e.Code
		inner join OWHS f on (f."WhsCode"=a."Warehouse" and (f."BPLid"=branch or branch=0))
	where a."DocDate" < :fdate
	group by a."ItemCode", IFNULL(b."InvntAcc",a."InvntAct"), a."Warehouse";
	
	insert into #IVL (Period, ID, TransNum, ItemCode, InvntAct, ContraAct, Vendor, WhsCode, DocType, DocEntry, DocLine, DocDate, JEMemo, InQty, InVal, OutQty, OutVal, JEUrl)
	select
		1,
		ROW_NUMBER() OVER (PARTITION BY a."ItemCode", IFNULL(b."InvntAcc",a."InvntAct"), a."Warehouse" ORDER BY a."DocDate", a."TransNum"),
		a."TransNum",
		a."ItemCode",
		IFNULL(b."InvntAcc",a."InvntAct"),
		CASE
			when a."TransType" in ('13','14','18','19','59','60','10000071') then a."CardCode"
			when a."TransType" in ('15','16') then a."CostAct"
			when a."TransType" in ('162') and a."TransValue" > 0 then a."IncAct"
			when a."TransType" in ('162') and a."TransValue" < 0 then a."DecAcc"
		END,
		f."CardCode",
		a."Warehouse",
		a."TransType",
		a."CreatedBy",
		a."DocLineNum",
		a."DocDate",
		a."JrnlMemo",
		IFNULL(CASE when a."TransType" in ('14','16','18','20','69','162') then IFNULL(a."InQty",0)-IFNULL(a."OutQty",0) else CASE when a."TransType" not in ('13','15','19','21') then a."InQty" else 0 end end,0),
		IFNULL(CASE when a."TransType" in ('14','16','18','20','69','162') then a."TransValue" else CASE when a."TransType" not in ('13','15','19','21') then CASE when a."TransValue" > 0 then a."TransValue" else 0 end end end,0),
		IFNULL(CASE when a."TransType" in ('13','15','19','21') then -IFNULL(a."InQty",0)+IFNULL(a."OutQty",0) else CASE when a."TransType" not in ('14','16','18','20','69','162') then a."OutQty" else 0 end end,0),
		IFNULL(CASE when a."TransType" in ('13','15','19','21') then -a."TransValue" else CASE when a."TransType" not in ('14','16','18','20','69','162') then CASE when a."TransValue" < 0 then -a."TransValue" else 0 end end end,0),
		a."JrnlMemo"
	from
		OINM a
		left join AZ_IVL0 b on a."TransNum" = b."TransNum"
		inner join :list_Items c on a."ItemCode" = c.Code
		inner join :list_Accs d on IFNULL(b."InvntAcc",a."InvntAct") = d.Code
		inner join :list_Whss e on a."Warehouse" = e.Code
		inner join OWHS t on (t."WhsCode"=a."Warehouse" and (t."BPLid"=branch or branch=0))
		left join OCRD f on a."CardCode" = f."CardCode"
	where
		a."DocDate" between :fdate and :tdate
		and (IFNULL(a."InQty",0) <> 0 or IFNULL(a."OutQty",0) <> 0 or IFNULL(a."TransValue",0) <> 0);
	
	update tbl set ContraAct = a."Account" from #IVL tbl, JDT1 a where a."TransType" = tbl.DocType and a."CreatedBy" = tbl.DocEntry and a."ShortName" = tbl.Vendor and tbl.ContraAct = tbl.Vendor;
	
	update tbl set ContraAct = a.ContraAct from #IVL tbl, ALL_JE a where a.TransType = tbl.DocType and a.DocID = tbl.DocEntry and (IFNULL(a.DocLineId,-1) = -1 or a.DocLineId = tbl.DocLine) and a.Account = tbl.InvntAct and tbl.ContraAct is null;
	
	insert into #IVL (Period, ID, ItemCode, InvntAct, WhsCode, JEMemo, DocMemo, InQty, InVal, OutQty, OutVal)
	select
		2,
		MAX(a.ID)+1,
		a.ItemCode,
		a.InvntAct,
		a.WhsCode,
		N'Số dư cuối kỳ / Ending balance',
		N'Số dư cuối kỳ / Ending balance',
		SUM(CASE when a.Period = 1 then a.InQty else 0 end),
		SUM(CASE when a.Period = 1 then a.InVal else 0 end),
		SUM(CASE when a.Period = 1 then a.OutQty else 0 end),
		SUM(CASE when a.Period = 1 then a.OutVal else 0 end)
	from #IVL a
	group by a.ItemCode, a.InvntAct, a.WhsCode;
	
	update #IVL set BalQty = IFNULL((select SUM(IFNULL(a.InQty,0)-IFNULL(a.OutQty,0)) from #IVL a where a.ItemCode = #IVL.ItemCode and a.InvntAct = #IVL.InvntAct and a.WhsCode = #IVL.WhsCode and a.Period < 2 and a.ID <= #IVL.ID),0);
	update #IVL set BalVal = IFNULL((select SUM(IFNULL(a.InVal,0)-IFNULL(a.OutVal,0)) from #IVL a where a.ItemCode = #IVL.ItemCode and a.InvntAct = #IVL.InvntAct and a.WhsCode = #IVL.WhsCode and a.Period < 2 and a.ID <= #IVL.ID),0);
	
	update #IVL set IOPrice = IFNULL((InVal-OutVal)/NULLIF(InQty-OutQty,0),0) where Period = 1;
	
	update tbl set DocNum = a."DocNum", DocNo = a."DocNum", DocMemo = a."Comments" from #IVL tbl, APZ_ODOC a where a."ObjType" = tbl.DocType and a."DocEntry" = tbl.DocEntry;
	
	update tbl set DocNo = a."ObjSapCode" || ' ' || CAST(IFNULL(tbl.DocNum,tbl.DocEntry) as nvarchar(20)) from #IVL tbl, AZ_OBJTYPE a where a."ObjID" = tbl.DocType and TRIM(IFNULL(tbl.DocNo,'')) = '';
	
	update tbl set JEUrl = 'http://$b1$/link?table=OJDT&key=' || CAST(a."TransId" as nvarchar(20)) from #IVL tbl, JDT1 a where a."TransType" = tbl.DocType and a."CreatedBy" = tbl.DocEntry;
	update tbl set DocURL = 'http://$b1$/link?table=' || a."ObjName" || '&key=' || CAST(tbl.DocEntry as nvarchar(20)) from #IVL tbl, AZ_OBJTYPE a where a."ObjID" = tbl.DocType;
	
	list_Itms =
		select a.ItemCode, a.InvntAct, a.WhsCode
		from #IVL a
		where a.Period = 2;
	
	insert into #IVL (Period, ID, ItemCode, InvntAct, WhsCode, JEMemo, DocMemo, BalQty, BalVal)
	select
		0,
		0,
		a.ItemCode,
		a.InvntAct,
		a.WhsCode,
		N'Số dư đầu kỳ / Opening balance',
		N'Số dư đầu kỳ / Opening balance',
		0,
		0
	from
		:list_Itms a
		left join #IVL b on b.Period = 0 and a.ItemCode = b.ItemCode and a.InvntAct = b.InvntAct and a.WhsCode = b.WhsCode
	where b.ItemCode is null;
	
	update tbl set ItemName = a."ItemName", ItemFName = a."FrgnName" from #IVL tbl, OITM a where a."ItemCode" = tbl.ItemCode;
	update tbl set InvntAct = a."AcctCode", AcctName = a."AcctName", AcctFName = a."FrgnName" from #IVL tbl, OACT a where a."AcctCode" = tbl.InvntAct;
	update tbl set WhsName = a."WhsName" from #IVL tbl, OWHS a where a."WhsCode" = tbl.WhsCode;
	
	update tbl set CompName = a."PrintHeadr", CompFName = a."PrintHdrF" from #IVL tbl, OADM a;
	update tbl set CompAdd = a."Street", CompFAdd = a."StreetF" from #IVL tbl, ADM1 a;
	
	update tbl set RptCurr = a."MainCurncy", DeciQty = a."QtyDec", DeciPrice = a."PriceDec", DeciAmt = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from #IVL tbl, OADM a;
	
	update tbl set DeciPrice = IFNULL(NULLIF(a."Decimals",-1),tbl.DeciPrice), DeciAmt = IFNULL(NULLIF(a."Decimals",-1),tbl.DeciAmt) from #IVL tbl, OCRN a where a."CurrCode" = tbl.RptCurr;
	
	update #IVL set
		FormNo = N'Mẫu số S10-DN',
		IssueForm = N'(Ban hành theo TT số 200/2014/TT-BTC' || char(13) || char(10) || N'Ngày 22/12/2014 của BTC)',
		FormNoF = N'Form No S10-DN',
		IssueFormF = N'(Issued with Circular No: 200/2014/TT-BTC' || char(13) || char(10) || N'Dated 22/12/2014 by Ministry of Finance)';
	
	tab_IVL = select * from #IVL;
	
	drop table #IVL;
end

