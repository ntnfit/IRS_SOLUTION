CREATE PROCEDURE VAS_S64_DN (
	in fdate		timestamp,
	in tdate		timestamp,
	in list_accts	table (Code nvarchar(30)),
	in currTyp		nvarchar(1),
	in branch		int,
	out tab_TBOOK	table (
		AcctCode	nvarchar(30),
		AcctName	nvarchar(100),
		AcctFName	nvarchar(100),
		ContraAct	nvarchar(30),
		CAccName	nvarchar(250),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		Period		tinyint,
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		RptCurrcy	nvarchar(30),
		FormNo		nvarchar(200),
		IssueForm	nvarchar(500),
		FormNoF		nvarchar(200),
		IssueFormF	nvarchar(500),
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	)
)
AS
BEGIN
	create local temporary table #TBOOK as (
		select * from :tab_TBOOK
	);
	
	if (:currTyp = 'S') then
		insert into #TBOOK (Period, AcctCode, AcctName, AcctFName, CAccName, Debit, Credit)
		select
			0,
			a."Account",
			c."AcctName",
			c."FrgnName",
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."SYSDeb",0)-IFNULL(a."SYSCred",0)),
			0
		from
			JDT1 a
			inner join :list_accts b on a."Account" = b.Code
			left join OACT c on a."Account" = c."AcctCode"
		where a."RefDate" < :fdate
			and (a."BPLId"=branch or branch=0)
		group by a."Account", c."AcctName", c."FrgnName";
	else
		insert into #TBOOK (Period, AcctCode, AcctName, AcctFName, CAccName, Debit, Credit)
		select
			0,
			a."Account",
			c."AcctName",
			c."FrgnName",
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."Debit",0)-IFNULL(a."Credit",0)),
			0
		from
			JDT1 a
			inner join :list_accts b on a."Account" = b.Code
			left join OACT c on a."Account" = c."AcctCode"
		where a."RefDate" < :fdate
			and (a."BPLId"=branch or branch=0)
		group by a."Account", c."AcctName", c."FrgnName";
	end if;

	insert into #TBOOK (Period, AcctCode, AcctName, AcctFName, ContraAct, CAccName, Debit, Credit)
	select
		1,
		a.Account,
		c."AcctName",
		c."FrgnName",
		a.ContraAct,
		IFNULL(d."AcctName",'') || IFNULL(' / ' || d."FrgnName",''),
		SUM(CASE when :currTyp = 'S' then IFNULL(a.DebitSC,0) else IFNULL(a.Debit,0) end),
		SUM(CASE when :currTyp = 'S' then IFNULL(a.CreditSC,0) else IFNULL(a.Credit,0) end)
	from
		ALL_JE a
		inner join :list_accts b on a.Account = b.Code
		left join OACT c on a.Account = c."AcctCode"
		left join OACT d on a.ContraAct = d."AcctCode"
	where a.RefDate between :fdate and :tdate
		and (a."BPLID"=branch or branch=0)
	group by
		a.Account, c."AcctName", c."FrgnName", a.ContraAct, d."AcctName", d."FrgnName";
	
	insert into #TBOOK (Period, AcctCode, AcctName, AcctFName, CAccName, Debit, Credit)
	select
		2,
		a.AcctCode,
		a.AcctName,
		a.AcctFName,
		N'Cộng số phát sinh trong kỳ / During the period',
		SUM(a.Debit),
		SUM(a.Credit)
	from #TBOOK a
	where a.Period = 1
	group by a.AcctCode, a.AcctName, a.AcctFName;
	
	insert into #TBOOK (Period, AcctCode, AcctName, AcctFName, CAccName, Debit, Credit)
	select
		3,
		a.AcctCode,
		a.AcctName,
		a.AcctFName,
		N'Số dư cuối kỳ / Closing balance',
		SUM(IFNULL(a.Debit,0)-IFNULL(a.Credit,0)),
		0
	from #TBOOK a
	where a.Period in (0,2)
	group by a.AcctCode, a.AcctName, a.AcctFName;
	
	update #TBOOK set
		Credit	= CASE when Debit < 0 then -Credit else 0 end,
		Debit	= CASE when Debit < 0 then 0 else Debit end
	where Period in (0,3);
	
	insert into #TBOOK (Period, AcctCode, AcctName, AcctFName, CAccName, Debit, Credit)
	select
		0,
		a.AcctCode,
		a.AcctName,
		a.AcctFName,
		N'Số dư đầu kỳ / Opening balance',
		0,
		0
	from
		#TBOOK a
		left join #TBOOK b on b.Period = 0 and a.AcctCode = b.AcctCode
	where a.Period = 3 and b.Period is null;
	
	update b set CompName = a."PrintHeadr", CompFName = a."PrintHdrF" from OADM a, #TBOOK b;
		
	update b set CompAdd = a."Street", CompFAdd = a."StreetF" from ADM1 a, #TBOOK b;
	
	update b set RptCurrcy = CASE when :currTyp = 'L' then a."MainCurncy" when :currTyp = 'S' then a."SysCurrncy" end, DeciAmt = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from OADM a, #TBOOK b;
	
	update b set DeciAmt = IFNULL(NULLIF(a."Decimals",-1),b.DeciAmt) from OCRN a, #TBOOK b where a."CurrCode" = b.RptCurrcy;
	
	update #TBOOK set
		FormNo = N'Mẫu số S64-DN',
		IssueForm = N'(Ban hành theo TT số 200/2014/TT-BTC' || char(13) || char(10) || N'Ngày 22/12/2014 của BTC)',
		FormNoF = N'Form No S64-DN',
		IssueFormF = N'(Issued with Circular No: 200/2014/TT-BTC' || char(13) || char(10) || N'Dated 22/12/2014 by Ministry of Finance)';

	tab_TBOOK = select * from #TBOOK;
	
	drop table #TBOOK;
END

