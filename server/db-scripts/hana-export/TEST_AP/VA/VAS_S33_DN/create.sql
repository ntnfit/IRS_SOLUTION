create procedure VAS_S33_DN (
	in list_accts	table (Code nvarchar(30)),
	in list_currs	table (Code nvarchar(30)),
	in fdate		timestamp,
	in tdate		timestamp,
	in currTyp		char(1),
	in branch		int,
	out tab_ACs		table (
		ID			int,
		Period		tinyint,
		AcctCode	nvarchar(30),
		AcctName	nvarchar(100),
		AcctFName	nvarchar(100),
		JEID		int,
		JELine		int,
		DocType		nvarchar(30),
		DocEntry	int,
		DocDate		timestamp,
		DocNum		int,
		DocNo		nvarchar(30),
		ContraAct	nvarchar(30),
		JEMemo		nvarchar(300),
		DocMemo		nvarchar(300),
		FCDeb		numeric(21,6),
		FCCred		numeric(21,6),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		FCBal		numeric(21,6),
		FCBalDeb	numeric(21,6),
		FCBalCred	numeric(21,6),
		Balance		numeric(21,6),
		BalDeb		numeric(21,6),
		BalCred		numeric(21,6),
		FCCurr		nvarchar(30),
		ExRate		numeric(21,6),
		JEUrl		nvarchar(100),
		DocUrl		nvarchar(100),
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		FormNo		nvarchar(200),
		IssueForm	nvarchar(500),
		FormNoF		nvarchar(200),
		IssueFormF	nvarchar(500),
		DeciAmtFC	smallint,
		DeciAmt		smallint,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	)
)
as
begin
	declare mainCurr nvarchar(30);
	
	select a."MainCurncy" into mainCurr from OADM a;

	create local temporary table #tbl as (
		select * from :tab_ACs
	);
	
	if (currTyp = 'L') then
		insert into #tbl (ID, Period, AcctCode, JEMemo, DocMemo, FCDeb, FCCred, Debit, Credit, FCCurr)
		select
			0,
			0,
			a."Account",
			N'Số dư đầu kỳ / Opening balance',
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."FCDebit",0)-IFNULL(a."FCCredit",0)),
			0,
			SUM(IFNULL(a."Debit",0)-IFNULL(a."Credit",0)),
			0,
			NULLIF(LTRIM(a."FCCurrency"),mainCurr)
		from
			JDT1 a
			inner join :list_accts b on a."Account" = b.Code
			inner join :list_currs c on NULLIF(LTRIM(a."FCCurrency"),mainCurr) = c.Code
		where a."RefDate" < fdate
		and (a."BPLId"=branch or branch=0)
		group by a."Account", NULLIF(LTRIM(a."FCCurrency"),mainCurr);
	else
		insert into #tbl (ID, Period, AcctCode, JEMemo, DocMemo, FCDeb, FCCred, Debit, Credit, FCCurr)
		select
			0,
			0,
			a."Account",
			N'Số dư đầu kỳ / Opening balance',
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."FCDebit",0)-IFNULL(a."FCCredit",0)),
			0,
			SUM(IFNULL(a."SYSDeb",0)-IFNULL(a."SYSCred",0)),
			0,
			NULLIF(LTRIM(a."FCCurrency"),mainCurr)
		from
			JDT1 a
			inner join :list_accts b on a."Account" = b.Code
			inner join :list_currs c on NULLIF(LTRIM(a."FCCurrency"),mainCurr) = c.Code
		where a."RefDate" < fdate
		and (a."BPLId"=branch or branch=0)
		group by a."Account", NULLIF(LTRIM(a."FCCurrency"),mainCurr);
	end if;
	
	insert into #tbl (ID, Period, AcctCode, JEID, JELine, DocType, DocEntry, DocDate, ContraAct, JEMemo, FCDeb, FCCred, Debit, Credit, FCCurr, JEUrl)
	select
		ROW_NUMBER() OVER (PARTITION BY a."Account", NULLIF(LTRIM(a."FCCurrency"),mainCurr) ORDER BY a."RefDate", a."TransId"),
		1,
		a."Account",
		a."TransId",
		a."Line_ID",
		a."TransType",
		a."CreatedBy",
		a."RefDate",
		a."ContraAct",
		a."LineMemo",
		a."FCDebit",
		a."FCCredit",
		CASE when currTyp = 'L' then a."Debit" else a."SYSDeb" end,
		CASE when currTyp = 'L' then a."Credit" else a."SYSCred" end,
		NULLIF(LTRIM(a."FCCurrency"),mainCurr),
		'http://$b1$/link?table=OJDT&key=' || CAST(a."TransId" as nvarchar(20))
	from
		JDT1 a
		inner join :list_accts b on a."Account" = b.Code
		inner join :list_currs c on NULLIF(LTRIM(a."FCCurrency"),mainCurr) = c.Code
	where a."RefDate" between fdate and tdate
	and (a."BPLId"=branch or branch=0);
	
	update #tbl set FCBal = (select SUM(IFNULL(a.FCDeb,0)-IFNULL(a.FCCred,0)) from #tbl a where a.AcctCode = #tbl.AcctCode and a.FCCurr = #tbl.FCCurr and a.ID <= #tbl.ID);
	update #tbl set Balance = (select SUM(IFNULL(a.Debit,0)-IFNULL(a.Credit,0)) from #tbl a where a.AcctCode = #tbl.AcctCode and a.FCCurr = #tbl.FCCurr and a.ID <= #tbl.ID);
	
	update tbl set DocNum = a."DocNum", DocNo = a."U_JVNumber", DocMemo = a."Comments" from #tbl tbl, APZ_ODOC a where tbl.DocType = a."ObjType" and tbl.DocEntry = a."DocEntry";
	
	update tbl set DocNo = a."ObjSapCode" || ' ' || CAST(IFNULL(tbl.DocNum,tbl.DocEntry) as nvarchar(20)) from #tbl tbl, AZ_OBJTYPE a where tbl.DocType = a."ObjID" and TRIM(IFNULL(tbl.DocNo,'')) = '';
	
	update tbl set DocUrl = 'http://$b1$/link?table=' || a."ObjName" || '&key=' || CAST(tbl.DocEntry as nvarchar(20)) from #tbl tbl, AZ_OBJTYPE a where tbl.DocType = a."ObjID";
	
	insert into #tbl (Period, AcctCode, JEMemo, DocMemo, FCBal, Balance, FCCurr)
	select
		3,
		a.AcctCode,
		N'Số dư cuối kỳ / Closing balance',
		N'Số dư cuối kỳ / Closing balance',
		SUM(IFNULL(FCDeb,0)-IFNULL(FCCred,0)),
		SUM(IFNULL(Debit,0)-IFNULL(Credit,0)),
		a.FCCurr
	from #tbl a
	group by a.AcctCode, a.FCCurr;
	
	update #tbl set Debit = 0, Credit = 0 where Period = 0;
	
	insert into #tbl (Period, AcctCode, JEMemo, DocMemo, FCDeb, FCCred, Debit, Credit, FCCurr)
	select
		2,
		a.AcctCode,
		N'Cộng phát sinh trong kỳ / During the period',
		N'Cộng phát sinh trong kỳ / During the period',
		SUM(a.FCDeb),
		SUM(a.FCCred),
		SUM(a.Debit),
		SUM(a.Credit),
		a.FCCurr
	from #tbl a
	where a.Period < 2
	group by a.AcctCode, a.FCCurr;
	
	insert into #tbl (ID, Period, AcctCode, JEMemo, DocMemo, FCCurr)
	select
		0,
		0,
		a.AcctCode,
		N'Số dư đầu kỳ / Opening balance',
		N'Số dư đầu kỳ / Opening balance',
		a.FCCurr
	from
		#tbl a
		left join #tbl b on a.AcctCode = b.AcctCode and a.FCCurr = b.FCCurr and b.Period = 0
	where
		a.Period = 3
		and b.AcctCode is null;
		
	update #tbl set
		FCBalDeb	= CASE when FCBal > 0 then FCBal else 0 end,
		FCBalCred	= CASE when FCBal < 0 then -FCBal else 0 end,
		BalDeb		= CASE when Balance > 0 then Balance else 0 end,
		BalCred		= CASE when Balance < 0 then -Balance else 0 end;
		
	update #tbl set ExRate = Balance/FCBal where IFNULL(FCBal,0) <> 0;
	
	update tbl set AcctName = a."AcctCode", AcctFName = a."FrgnName" from #tbl tbl, OACT a where tbl.AcctCode = a."AcctCode";
	
	update tbl set CompName = a."PrintHeadr", CompFName = a."PrintHdrF" from #tbl tbl, OADM a;
		
	update tbl set CompAdd = a."Street", CompFAdd = a."StreetF" from #tbl tbl, ADM1 a;
	
	update tbl set DeciAmtFC = a."SumDec", DeciAmt = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from #tbl tbl, OADM a;
	
	update tbl set DeciAmtFC = IFNULL(NULLIF(a."Decimals",-1),tbl.DeciAmtFC) from #tbl tbl, OCRN a where tbl.FCCurr = a."CurrCode";
	update tbl set DeciAmt = IFNULL(NULLIF(a."Decimals",-1),tbl.DeciAmt) from #tbl tbl, OCRN a where a."CurrCode" = mainCurr;
	
	update #tbl set
		FormNo = N'Mẫu số S33-DN',
		IssueForm = N'(Ban hành theo TT số 200/2014/TT-BTC' || char(13) || char(10) || N'Ngày 22/12/2014 của BTC)',
		FormNoF = N'Form No S33-DN',
		IssueFormF = N'(Issued with Circular No: 200/2014/TT-BTC' || char(13) || char(10) || N'Dated 22/12/2014 by Ministry of Finance)';
	
	tab_ACs = select * from #tbl;
	
	drop table #tbl;
end

