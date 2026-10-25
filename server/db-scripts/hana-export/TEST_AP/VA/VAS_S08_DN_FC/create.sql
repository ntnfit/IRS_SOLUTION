create PROCEDURE VAS_S08_DN_FC (
	in fdate		timestamp,
	in tdate		timestamp,
	in list_accts	table (Code nvarchar(30)),
	in currency		nvarchar(10),
	in exclCancel	char(1),
	in currTyp		nvarchar(1),
	in branch		int,
	out tab_BJ		table (
		ID			int,
		JEId		int,
		JELine		int,
		RefDate		timestamp,
		JEMemo		nvarchar(300),
		Account		nvarchar(30),
		ContraAct	nvarchar(30),
		FCDebit		numeric(21,6),
		Debit		numeric(21,6),
		FCCredit	numeric(21,6),
		Credit		numeric(21,6),
		FCBal		numeric(21,6),
		Balance		numeric(21,6),
		DebCred		nvarchar(1),
		JEURL		nvarchar(100),
		Period		tinyint,
		
		TransType	nvarchar(20),
		CreatedBy	int,
		Canceled	nvarchar(10),
		DocNum		int,
		DocNo		nvarchar(100),
		DocMemo		nvarchar(300),
		DocURL		nvarchar(100),
		
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		AcctName	nvarchar(100),
		AcctFName	nvarchar(100),
		BankAcct	nvarchar(30),
		BankName	nvarchar(100),
		FCCurrency	nvarchar(20),
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
	
	create local temporary table #BJ as (
		select * from :tab_BJ
	);
	
	if (:currTyp = 'S') then
		insert into #BJ (ID, Account, JEMemo, DocMemo, FCDebit, FCCredit, Debit, Credit, Period)
		select
			0,
			b.Code,
			N'Số dư đầu kỳ / Opening balance',
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."FCDebit",0)-IFNULL(a."FCCredit",0)),
			0,
			SUM(IFNULL(a."SYSDeb",0)-IFNULL(a."SYSCred",0)),
			0,
			0
		from JDT1 a, :list_accts b
		where
			a."Account" = b.Code
			and NULLIF(TRIM(a."FCCurrency"),mainCurr) = currency
			and a."RefDate" < :fdate
			and (a."BPLId"=branch or branch=0)
		group by b.Code;
	else
		insert into #BJ (ID, Account, JEMemo, DocMemo, FCDebit, FCCredit, Debit, Credit, Period)
		select
			0,
			b.Code,
			N'Số dư đầu kỳ / Opening balance',
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."FCDebit",0)-IFNULL(a."FCCredit",0)),
			0,
			SUM(IFNULL(a."Debit",0)-IFNULL(a."Credit",0)),
			0,
			0
		from JDT1 a, :list_accts b
		where
			a."Account" = b.Code
			and NULLIF(TRIM(a."FCCurrency"),mainCurr) = currency
			and a."RefDate" < :fdate
			and (a."BPLId"=branch or branch=0)
		group by b.Code;
	end if;
	
	insert into #BJ (ID, JEId, JELine, RefDate, JEMemo, Account, ContraAct, FCDebit, FCCredit, Debit, Credit, DebCred, TransType, CreatedBy, Period)
	select
		ROW_NUMBER() OVER (PARTITION BY a."Account" ORDER BY a."RefDate", a."DebCred" desc, a."TransId", a."Line_ID"),
		a."TransId",
		a."Line_ID",
		a."RefDate",
		a."LineMemo",
		a."Account",
		a."ContraAct",
		IFNULL(a."FCDebit",0),
		IFNULL(a."FCCredit",0),
		CASE when :currTyp = 'S' then IFNULL(a."SYSDeb",0) else IFNULL(a."Debit",0) end,
		CASE when :currTyp = 'S' then IFNULL(a."SYSCred",0) else IFNULL(a."Credit",0) end,
		a."DebCred",
		a."TransType",
		a."CreatedBy",
		1
	from
		JDT1 a
		inner join :list_accts b on a."Account" = b.Code
	where
		NULLIF(TRIM(a."FCCurrency"),mainCurr) = currency
		and (a."BPLId"=branch or branch=0)
		and a."RefDate" between :fdate and :tdate;
	
	update b set Canceled = a."CANCELED", DocNum = a."DocNum", DocNo = a."DocNum", DocMemo = a."Comments" from APZ_ODOC a, #BJ b where a."ObjType" = b.TransType and a."DocEntry" = b.CreatedBy;
	
	delete from #BJ where exclCancel = 'Y' and IFNULL(Canceled,'N') <> 'N';
	
	update #BJ set FCBal = (select SUM(a.FCDebit-a.FCCredit) from #BJ a where a.Account = #BJ.Account and a.ID <= #BJ.ID);
	
	update #BJ set Balance = (select SUM(a.Debit-a.Credit) from #BJ a where a.Account = #BJ.Account and a.ID <= #BJ.ID);
	
	insert into #BJ (Account, JEMemo, DocMemo, FCBal, Balance, Period)
	select
		a.Account,
		N'Số dư cuối kỳ / Closing balance',
		N'Số dư cuối kỳ / Closing balance',
		SUM(a.FCDebit-a.FCCredit),
		SUM(a.Debit-a.Credit),
		3
	from #BJ a
	group by a.Account;
	
	list_accs =
		select a.Account
		from #BJ a
		where a.Period = 3;
		
	insert into #BJ (ID, Account, JEMemo, DocMemo, Debit, Credit, Period)
	select
		0,
		a.Account,
		N'Số dư đầu kỳ / Opening balance',
		N'Số dư đầu kỳ / Opening balance',
		0,
		0,
		0
	from
		:list_accs a
		left join #BJ b on b.Period = 0 and a.Account = b.Account
	where b.Account is null;
	
	insert into #BJ (Account, JEMemo, DocMemo, FCDebit, FCCredit, Debit, Credit, Period)
	select
		a.Account,
		N'Cộng số phát sinh trong kỳ / During the period',
		N'Cộng số phát sinh trong kỳ / During the period',
		IFNULL(SUM(b.FCDebit),0),
		IFNULL(SUM(b.FCCredit),0),
		IFNULL(SUM(b.Debit),0),
		IFNULL(SUM(b.Credit),0),
		2
	from
		:list_accs a
		left join #BJ b on b.Period = 1 and a.Account = b.Account
	group by a.Account;
		
	update b set DocNo = a."ObjSapCode" || ' ' || CAST(IFNULL(b.DocNum,b.CreatedBy) as nvarchar(20)) from AZ_OBJTYPE a, #BJ b where a."ObjID" = b.TransType and TRIM(IFNULL(b.DocNo,'')) = '';

	update b set AcctName = a."AcctName", AcctFName = a."FrgnName" from OACT a, #BJ b where a."AcctCode" = b.Account;
	
	update #BJ set JEURL = 'http://$b1$/link?table=OJDT&key=' || CAST(JEId as nvarchar(20));
	
	update b set DocURL = 'http://$b1$/link?table=' || a."ObjName" || '&key=' || CAST(b.CreatedBy as nvarchar(20)) from AZ_OBJTYPE a, #BJ b where a."ObjID" = b.TransType;

	update b set CompName = a."PrintHeadr", CompFName = a."PrintHdrF" from OADM a, #BJ b;
		
	update b set CompAdd = a."Street", CompFAdd = a."StreetF" from ADM1 a, #BJ b;
	
	update b set FCCurrency = currency, RptCurrcy = CASE when :currTyp = 'L' then a."MainCurncy" when :currTyp = 'S' then a."SysCurrncy" end, DeciAmtFC = a."SumDec", DeciAmt = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from OADM a, #BJ b;
	
	update b set DeciAmtFC = IFNULL(NULLIF(a."Decimals",-1),b.DeciAmtFC) from OCRN a, #BJ b where a."CurrCode" = b.FCCurrency;
	
	update b set DeciAmt = IFNULL(NULLIF(a."Decimals",-1),b.DeciAmt) from OCRN a, #BJ b where a."CurrCode" = b.RptCurrcy;
	
	update #BJ set
		FormNo = N'Mẫu số S08-DN',
		IssueForm = N'(Ban hành theo TT số 200/2014/TT-BTC' || char(13) || char(10) || N'Ngày 22/12/2014 của BTC)',
		FormNoF = N'Form No S08-DN',
		IssueFormF = N'(Issued with Circular No: 200/2014/TT-BTC' || char(13) || char(10) || N'Dated 22/12/2014 by Ministry of Finance)';
	
	tab_BJ = select * from #BJ;
	drop table #BJ;
END

