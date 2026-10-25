CREATE procedure VAS_S12_DN (
	in list_items	table (Code nvarchar(50)),
	in list_whses	table (Code nvarchar(30)),
	in fdate		timestamp,
	in tdate		timestamp,
	in branch		int,
	out tab_Items	table (
		ID			int,
		Period		tinyint,
		ItemCode	nvarchar(50),
		ItemName	nvarchar(100),
		ItemFName	nvarchar(100),
		UoM			nvarchar(30),
		WhsCode		nvarchar(30),
		WhsName		nvarchar(100),
		DocType		nvarchar(30),
		DocEntry	int,
		DocDate		timestamp,
		DocNum		int,
		DocNo		nvarchar(50),
		DocUrl		nvarchar(100),
		DocMemo		nvarchar(300),
		JrnlMemo	nvarchar(300),
		InQty		numeric(21,6),
		OutQty		numeric(21,6),
		BalQty		numeric(21,6),
		Note		nvarchar(300),
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		FormNo		nvarchar(200),
		IssueForm	nvarchar(500),
		FormNoF		nvarchar(200),
		IssueFormF	nvarchar(500),
		DeciQty		smallint,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	)
)
as
begin
	create local temporary table #tbl as (
		select * from :tab_Items
	);
	
	insert into #tbl (ID, Period, ItemCode, WhsCode, DocMemo, JrnlMemo, InQty)
	select
		0,
		0,
		a."ItemCode",
		a."Warehouse",
		N'Số dư đầu kỳ / Opening balance',
		N'Số dư đầu kỳ / Opening balance',
		SUM(IFNULL(a."InQty",0)-IFNULL(a."OutQty",0))
	from
		OINM a
		inner join :list_items b on a."ItemCode" = b.Code
		inner join :list_whses c on a."Warehouse" = c.Code
		inner join OWHS d on (d."WhsCode"=a."Warehouse" and (d."BPLid"=branch or branch=0))
	where a."DocDate" < fdate
	group by a."ItemCode", a."Warehouse"
	having SUM(IFNULL(a."InQty",0)-IFNULL(a."OutQty",0)) <> 0;
		
	insert into #tbl (ID, Period, ItemCode, WhsCode, DocType, DocEntry, DocDate, JrnlMemo, InQty, OutQty)
	select
		ROW_NUMBER() OVER (PARTITION BY a."ItemCode", a."Warehouse" ORDER BY a."DocDate", a."TransNum"),
		1,
		a."ItemCode",
		a."Warehouse",
		a."TransType",
		a."CreatedBy",
		a."DocDate",
		a."JrnlMemo",
		CASE
			when a."TransType" in ('14','16','18','20') then IFNULL(a."InQty",0)-IFNULL(a."OutQty",0)
			when a."TransType" in ('13','15','19','21') then 0
			else a."InQty"
		end,
		CASE
			when a."TransType" in ('13','15','19','21') then IFNULL(a."OutQty",0)-IFNULL(a."InQty",0)
			when a."TransType" in ('14','16','18','20') then 0
			else a."OutQty"
		end
	from
		OINM a
		inner join :list_items b on a."ItemCode" = b.Code
		inner join :list_whses c on a."Warehouse" = c.Code
		inner join OWHS d on (d."WhsCode"=a."Warehouse" and (d."BPLid"=branch or branch=0))
	where
		a."DocDate" between fdate and tdate
		and (IFNULL(a."InQty",0) <> 0 or IFNULL(a."OutQty",0) <> 0);
		
	update #tbl set BalQty = (select SUM(IFNULL(a.InQty,0)-IFNULL(a.OutQty,0)) from #tbl a where a.ItemCode = #tbl.ItemCode and a.WhsCode = #tbl.WhsCode and a.ID <= #tbl.ID);
		
	update tbl set DocNum = a."DocNum", DocNo = a."U_JVNumber", DocMemo = a."Comments" from #tbl tbl, APZ_ODOC a where tbl.DocType = a."ObjType" and tbl.DocEntry = a."DocEntry";
	
	update tbl set DocNo = a."ObjSapCode" || ' ' || CAST(IFNULL(tbl.DocNum,tbl.DocEntry) as nvarchar(20)) from #tbl tbl, AZ_OBJTYPE a where tbl.DocType = a."ObjID" and TRIM(IFNULL(tbl.DocNo,'')) = '';
	
	update tbl set DocUrl = 'http://$b1$/link?table=' || a."ObjName" || '&key=' || CAST(tbl.DocEntry as nvarchar(20)) from #tbl tbl, AZ_OBJTYPE a where tbl.DocType = a."ObjID";
	
	insert into #tbl (Period, ItemCode, WhsCode, DocMemo, JrnlMemo, BalQty)
	select
		3,
		a.ItemCode,
		a.WhsCode,
		N'Số dư cuối kỳ / Closing balance',
		N'Số dư cuối kỳ / Closing balance',
		SUM(IFNULL(a.InQty,0)-IFNULL(a.OutQty,0))
	from #tbl a
	group by a.ItemCode, a.WhsCode;
		
	update #tbl set InQty = 0, OutQty = 0 where Period = 0;
		
	insert into #tbl (Period, ItemCode, WhsCode, DocMemo, JrnlMemo, InQty, OutQty)
	select
		2,
		a.ItemCode,
		a.WhsCode,
		N'Cộng phát sinh trong kỳ / During the period',
		N'Cộng phát sinh trong kỳ / During the period',
		SUM(a.InQty),
		SUM(a.OutQty)
	from #tbl a
	where a.Period < 2
	group by a.ItemCode, a.WhsCode;
		
	insert into #tbl (ID, Period, ItemCode, WhsCode, DocMemo, JrnlMemo)
	select
		0,
		0,
		a.ItemCode,
		a.WhsCode,
		N'Số dư đầu kỳ / Opening balance',
		N'Số dư đầu kỳ / Opening balance'
	from
		#tbl a
		left join #tbl b on a.ItemCode = b.ItemCode and a.WhsCode = b.WhsCode and b.Period = 0
	where
		a.Period = 3
		and b.ItemCode is null;
		
	update tbl set ItemName = a."ItemName", ItemFName = a."FrgnName", UoM = a."InvntryUom" from #tbl tbl, OITM a where tbl.ItemCode = a."ItemCode";
		
	update tbl set WhsName = a."WhsName" from #tbl tbl, OWHS a where tbl.WhsCode = a."WhsCode";
		
	update tbl set CompName = a."PrintHeadr", CompFName = a."PrintHdrF" from #tbl tbl, OADM a;
		
	update tbl set CompAdd = a."Street", CompFAdd = a."StreetF" from #tbl tbl, ADM1 a;
	
	update tbl set DeciQty = a."QtyDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from #tbl tbl, OADM a;
	
	update #tbl set
		FormNo = N'Mẫu số S12-DN',
		IssueForm = N'(Ban hành theo TT số 200/2014/TT-BTC' || char(13) || char(10) || N'Ngày 22/12/2014 của BTC)',
		FormNoF = N'Form No S12-DN',
		IssueFormF = N'(Issued with Circular No: 200/2014/TT-BTC' || char(13) || char(10) || N'Dated 22/12/2014 by Ministry of Finance)';
		
	tab_Items = select * from #tbl;
		
	drop table #tbl;
end

