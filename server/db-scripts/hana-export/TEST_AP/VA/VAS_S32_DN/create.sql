CREATE PROCEDURE VAS_S32_DN (
	in fdate		timestamp,
	in tdate		timestamp,
	in list_BPs		table (Code nvarchar(30)),
	in list_accts	table (Code nvarchar(30)),
	in list_currs	table (Code nvarchar(30)),
	in currTyp		nvarchar(1),
	in branch		int,
	out tab_BPDtls	table (
		ID			int,
		JEId		int,
		JELine		int,
		RefDate		timestamp,
		DisDueDate	timestamp,
		JEMemo		nvarchar(300),
		ShortName	nvarchar(30),
		Account		nvarchar(30),
		ContraAct	nvarchar(30),
		FCCurr		nvarchar(30),
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
		ExRate		numeric(21,6),
		JEURL		nvarchar(100),
		Period		tinyint,
		
		TransType	nvarchar(20),
		CreatedBy	int,
		DocNum		int,
		DocNo		nvarchar(100),
		DocMemo		nvarchar(300),
		DocURL		nvarchar(100),
		
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		CardName	nvarchar(100),
		CardFName	nvarchar(100),
		AcctName	nvarchar(100),
		AcctFName	nvarchar(100),
		RptCurrcy	nvarchar(20),
		FormNo		nvarchar(200),
		IssueForm	nvarchar(500),
		FormNoF		nvarchar(200),
		IssueFormF	nvarchar(500),
		DeciAmtFC	int,
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	)
)
AS
BEGIN
	declare mainCurr nvarchar(30);
	
	select a."MainCurncy" into mainCurr from OADM a;

	create local temporary table #BPDtls (
		ID			int,
		JEId		int,
		JELine		int,
		RefDate		timestamp,
		DisDueDate	timestamp,
		JEMemo		nvarchar(300),
		ShortName	nvarchar(30),
		Account		nvarchar(30),
		ContraAct	nvarchar(30),
		FCCurr		nvarchar(30),
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
		ExRate		numeric(21,6),
		JEURL		nvarchar(100),
		Period		tinyint,
		
		TransType	nvarchar(20),
		CreatedBy	int,
		DocNum		int,
		DocNo		nvarchar(100),
		DocMemo		nvarchar(300),
		DocURL		nvarchar(100),
		
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		CardName	nvarchar(100),
		CardFName	nvarchar(100),
		AcctName	nvarchar(100),
		AcctFName	nvarchar(100),
		RptCurrcy	nvarchar(20),
		FormNo		nvarchar(200),
		IssueForm	nvarchar(500),
		FormNoF		nvarchar(200),
		IssueFormF	nvarchar(500),
		DeciAmtFC	int,
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	);
	
	create local temporary table #JE (
		TransId		int,
		LineId		int,
		RefDate		timestamp,
		ShortName	nvarchar(30),
		Account		nvarchar(30),
		ContraAct	nvarchar(30),
		Memo		nvarchar(300),
		FCDebit		numeric(21,6),
		FCCredit	numeric(21,6),
		FCCurr		nvarchar(30),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		TransType	nvarchar(30),
		CreatedBy	int
	);
	
	insert into #JE (TransId, LineId, RefDate, ShortName, Account, ContraAct, Memo, FCDebit, FCCredit, FCCurr, Debit, Credit, TransType, CreatedBy)
	select
		a."TransId",
		a."Line_ID",
		a."RefDate",
		a."ShortName",
		a."Account",
		a."ContraAct",
		a."LineMemo",
		a."FCDebit",
		a."FCCredit",
		IFNULL(a."FCCurrency",mainCurr),
		CASE when :currTyp = 'S' then IFNULL(a."SYSDeb",0) else IFNULL(a."Debit",0) end,
		CASE when :currTyp = 'S' then IFNULL(a."SYSCred",0) else IFNULL(a."Credit",0) end,
		a."TransType",
		a."CreatedBy"
	from JDT1 a, :list_BPs b, :list_accts c
	where
		a."ShortName" = b.Code
		and a."Account" = c.Code
		and IFNULL(a."FCCurrency",mainCurr) in (select Code from :list_currs)
		and (a."BPLId"=branch or branch=0)
		and a."RefDate" <= :tdate;
	/*
	union all
	select
		a."TransId",
		a."Line_ID",
		a."RefDate",
		a."ShortName",
		a."Account",
		sign(a."FCDebit")*a."U_AmountConvert",
		sign(a."FCCredit")*a."U_AmountConvert",
		IFNULL(a."U_ConvertCurr",mainCurr),
		CASE when :currTyp = 'S' then IFNULL(a."SYSDeb",0) else IFNULL(a."Debit",0) end,
		CASE when :currTyp = 'S' then IFNULL(a."SYSCred",0) else IFNULL(a."Credit",0) end
	from JDT1 a, :list_BPs b, :list_accts c
	where
		a."ShortName" = b.Code
		and a."Account" = c.Code
		and IFNULL(a."U_ConvertCurr",mainCurr) in (select Code from :list_currs)
		and a."RefDate" <= :tdate
		and IFNULL(a."U_ConvertCurr",'') <> ''
		and IFNULL(a."U_AmountConvert",0) <> 0;
	*/
	insert into #BPDtls (ID, ShortName, Account, FCCurr, JEMemo, DocMemo, FCDeb, FCCred, Debit, Credit, Period)
	select
		0,
		a.ShortName,
		a.Account,
		a.FCCurr,
		N'Số dư đầu kỳ / Opening balance',
		N'Số dư đầu kỳ / Opening balance',
		SUM(IFNULL(a.FCDebit,0)-IFNULL(a.FCCredit,0)),
		0,
		SUM(IFNULL(a.Debit,0)-IFNULL(a.Credit,0)),
		0,
		0
	from #JE a
	where a.RefDate < :fdate
	group by a.ShortName, a.Account, a.FCCurr;
	
	insert into #BPDtls (ID, JEId, JELine, RefDate, ShortName, Account, ContraAct, JEMemo, FCCurr, FCDeb, FCCred, Debit, Credit, TransType, CreatedBy, Period)
	select
		ROW_NUMBER() OVER (PARTITION BY a.ShortName, a.Account, a.FCCurr ORDER BY a.RefDate, a.TransId, a.LineId),
		a.TransId,
		a.LineId,
		a.RefDate,
		a.ShortName,
		a.Account,
		a.ContraAct,
		a.Memo,
		a.FCCurr,
		a.FCDebit,
		a.FCCredit,
		IFNULL(a.Debit,0),
		IFNULL(a.Credit,0),
		a.TransType,
		a.CreatedBy,
		1
	from #JE a
	where a.RefDate between :fdate and :tdate;
	
	update #BPDtls set FCBal	= (select SUM(a.FCDeb-a.FCCred) from #BPDtls a where a.ShortName = #BPDtls.ShortName and a.Account = #BPDtls.Account and a.FCCurr = #BPDtls.FCCurr and a.ID <= #BPDtls.ID);
	update #BPDtls set Balance	= (select SUM(a.Debit-a.Credit) from #BPDtls a where a.ShortName = #BPDtls.ShortName and a.Account = #BPDtls.Account and a.FCCurr = #BPDtls.FCCurr and a.ID <= #BPDtls.ID);
	
	insert into #BPDtls (ShortName, Account, FCCurr, JEMemo, DocMemo, FCBal, Balance, Period)
	select
		a.ShortName,
		a.Account,
		a.FCCurr,
		N'Số dư cuối kỳ / Closing balance',
		N'Số dư cuối kỳ / Closing balance',
		SUM(IFNULL(a.FCDeb,0)-IFNULL(a.FCCred,0)),
		SUM(IFNULL(a.Debit,0)-IFNULL(a.Credit,0)),
		3
	from #BPDtls a
	group by a.ShortName, a.Account, a.FCCurr;
	
	list_BPBal =
		select a.ShortName, a.Account, a.FCCurr
		from #BPDtls a
		where a.Period = 3;
	
	insert into #BPDtls (ID, ShortName, Account, FCCurr, JEMemo, DocMemo, FCDeb, FCCred, Debit, Credit, Period)
	select
		0,
		a.ShortName,
		a.Account,
		a.FCCurr,
		N'Số dư đầu kỳ / Opening balance',
		N'Số dư đầu kỳ / Opening balance',
		0,
		0,
		0,
		0,
		0
	from
		:list_BPBal a
		left join #BPDtls b on b.Period = 0 and a.ShortName = b.ShortName and a.Account = b.Account and a.FCCurr = b.FCCurr
	where b.ShortName is null;
	
	insert into #BPDtls (ShortName, Account, FCCurr, JEMemo, DocMemo, FCDeb, FCCred, Debit, Credit, Period)
	select
		a.ShortName,
		a.Account,
		a.FCCurr,
		N'Cộng số phát sinh trong kỳ / During the period',
		N'Cộng số phát sinh trong kỳ / During the period',
		IFNULL(SUM(b.FCDeb),0),
		IFNULL(SUM(b.FCCred),0),
		IFNULL(SUM(b.Debit),0),
		IFNULL(SUM(b.Credit),0),
		2
	from
		:list_BPBal a
		left join #BPDtls b on b.Period = 1 and a.ShortName = b.ShortName and a.Account = b.Account and a.FCCurr = b.FCCurr
	group by a.ShortName, a.Account, a.FCCurr;
	
	update #BPDtls set FCBalDeb = CASE when FCBal >= 0 then FCBal end;
	update #BPDtls set FCBalCred = CASE when FCBal < 0 then -FCBal end;
	
	update #BPDtls set BalDeb = CASE when Balance >= 0 then Balance end;
	update #BPDtls set BalCred = CASE when Balance < 0 then -Balance end;
	
	update b set DocNum = a."DocNum", DocNo = a."DocNum", DocMemo = a."Comments" from APZ_ODOC a, #BPDtls b where a."ObjType" = b.TransType and a."DocEntry" = b.CreatedBy;
	
	update b set DocNo = a."ObjSapCode" || ' ' || CAST(IFNULL(b.DocNum,b.CreatedBy) as nvarchar(20)) from AZ_OBJTYPE a, #BPDtls b where a."ObjID" = b.TransType and TRIM(IFNULL(b.DocNo,'')) = '';
	
	update b set CardName = a."CardName", CardFName = a."CardFName" from OCRD a, #BPDtls b where a."CardCode" = b.ShortName;
	
	update b set AcctName = a."AcctName", AcctFName = a."FrgnName" from OACT a, #BPDtls b where a."AcctCode" = b.Account;
	
	update #BPDtls set JEURL = 'http://$b1$/link?table=OJDT&key=' || CAST(JEId as nvarchar(20));
	
	update b set DocURL = 'http://$b1$/link?table=' || a."ObjName" || '&key=' || CAST(b.CreatedBy as nvarchar(20)) from AZ_OBJTYPE a, #BPDtls b where a."ObjID" = b.TransType;

	update b set CompName = a."PrintHeadr", CompFName = a."PrintHdrF" from OADM a, #BPDtls b;
		
	update b set CompAdd = a."Street", CompFAdd = a."StreetF" from ADM1 a, #BPDtls b;
	
	update b set RptCurrcy = CASE when :currTyp = 'L' then a."MainCurncy" when :currTyp = 'S' then a."SysCurrncy" end, DeciAmtFC = a."SumDec", DeciAmt = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from OADM a, #BPDtls b;
	
	update b set DeciAmtFC = IFNULL(NULLIF(a."Decimals",-1),b.DeciAmtFC) from OCRN a, #BPDtls b where a."CurrCode" = b.FCCurr;
	update b set DeciAmt = IFNULL(NULLIF(a."Decimals",-1),b.DeciAmt) from OCRN a, #BPDtls b where a."CurrCode" = b.RptCurrcy;
	
	update #BPDtls set
		FormNo = N'Mẫu số S32-DN',
		IssueForm = N'(Ban hành theo TT số 200/2014/TT-BTC' || char(13) || char(10) || N'Ngày 22/12/2014 của BTC)',
		FormNoF = N'Form No S32-DN',
		IssueFormF = N'(Issued with Circular No: 200/2014/TT-BTC' || char(13) || char(10) || N'Dated 22/12/2014 by Ministry of Finance)';
	
	tab_BPDtls = select * from #BPDtls;
	
	drop table #BPDtls;
	drop table #JE;
END

