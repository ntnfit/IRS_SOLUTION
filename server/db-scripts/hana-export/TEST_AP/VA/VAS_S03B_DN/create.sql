CREATE PROCEDURE VAS_S03b_DN (
	in fdate		timestamp,
	in tdate		timestamp,
	in list_accts	table (Code nvarchar(30)),
	in currTyp		nvarchar(1),
	in branch		int,
	out tab_GL		table (
		JEId		int,
		JELine		int,
		RefDate		timestamp,
		JEMemo		nvarchar(300),
		Account		nvarchar(30),
		ContraAct	nvarchar(30),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		JEURL		nvarchar(100),
		Period		tinyint,
		
		TransType	nvarchar(20),
		CreatedBy	int,
		DocDate		timestamp,
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
		RptCurrcy	nvarchar(20),
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
	create local temporary table #GL (
		JEId		int,
		JELine		int,
		RefDate		timestamp,
		JEMemo		nvarchar(300),
		Account		nvarchar(30),
		ContraAct	nvarchar(30),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		JEURL		nvarchar(100),
		Period		tinyint,
		
		TransType	nvarchar(20),
		CreatedBy	int,
		DocDate		timestamp,
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
		RptCurrcy	nvarchar(20),
		FormNo		nvarchar(200),
		IssueForm	nvarchar(500),
		FormNoF		nvarchar(200),
		IssueFormF	nvarchar(500),
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	);
	
	list_childAccts =
		select x."AcctCode", y."AcctCode" as "GLAccount", y."AcctName", y."FrgnName"
		from
			(select a."AcctCode", a."GroupMask", MAX(b."GrpLine") "GrpLine"
			from
				OACT a
				left join OACT b on
					a."GroupMask" = b."GroupMask"
					and b."Levels" = 2
					and a."GrpLine" >= b."GrpLine"
			where a."Postable" = 'Y'
			group by a."AcctCode", a."GroupMask") x
			inner join OACT y on x."GroupMask" = y."GroupMask" and x."GrpLine" = y."GrpLine"
			inner join :list_accts z on y."AcctCode" = z.Code;
		
	if (:currTyp = 'S') then
		insert into #GL (Account, AcctName, AcctFName, JEMemo, DocMemo, Debit, Credit, Period)
		select
			b."GLAccount",
			b."AcctName",
			b."FrgnName",
			N'Số dư đầu kỳ / Opening balance',
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."SYSDeb",0)-IFNULL(a."SYSCred",0)),
			0,
			0
		from JDT1 a, :list_childAccts b
		where
			a."Account" = b."AcctCode"
			and a."RefDate" < :fdate
			and (:branch in (0,-1) or a."BPLId"=:branch)
		group by b."GLAccount", b."AcctName", b."FrgnName";
	else
		insert into #GL (Account, AcctName, AcctFName, JEMemo, DocMemo, Debit, Credit, Period)
		select
			b."GLAccount",
			b."AcctName",
			b."FrgnName",
			N'Số dư đầu kỳ / Opening balance',
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."Debit",0)-IFNULL(a."Credit",0)),
			0,
			0
		from JDT1 a, :list_childAccts b
		where
			a."Account" = b."AcctCode"
			and a."RefDate" < :fdate
			and (:branch in (0,-1) or a."BPLId"=:branch)
		group by b."GLAccount", b."AcctName", b."FrgnName";
	end if;
		
	insert into #GL (JEId, JELine, RefDate, Account, AcctName, AcctFName, ContraAct, Debit, Credit, TransType, CreatedBy, Period)
	select
		a.TransId,
		a.LineId,
		a.RefDate,
		b."GLAccount",
		b."AcctName",
		b."FrgnName",
		a.ContraAct,
		CASE when :currTyp = 'S' then IFNULL(a.DebitSC,0) else IFNULL(a.Debit,0) end,
		CASE when :currTyp = 'S' then IFNULL(a.CreditSC,0) else IFNULL(a.Credit,0) end,
		a.TransType,
		a.DocID,
		1
	from
		ALL_JE a
		inner join :list_childAccts b on a.Account = b."AcctCode"
	where a."REFDATE" between :fdate and :tdate and (:branch in (0,-1) or a."BPLID"=:branch);
		
	insert into #GL (Account, AcctName, AcctFName, JEMemo, DocMemo, Debit, Period)
	select
		a.Account,
		a.AcctName,
		a.AcctFName,
		N'Số dư cuối kỳ / Closing balance',
		N'Số dư cuối kỳ / Closing balance',
		SUM(a.Debit-a.Credit),
		3
	from #GL a
	group by a.Account, a.AcctName, a.AcctFName;
		
	list_GLAccts =
		select a.Account, a.AcctName, a.AcctFName
		from #GL a
		where a.Period = 3;
		
	update #GL set
		Credit	= CASE when Debit < 0 then -Debit else 0 end,
		Debit	= CASE when Debit > 0 then Debit else 0 end
	where Period in (0,3);
		
	insert into #GL (Account, AcctName, AcctFName, JEMemo, DocMemo, Debit, Credit, Period)
	select
		a.Account,
		a.AcctName,
		a.AcctFName,
		N'Số dư đầu kỳ / Opening balance',
		N'Số dư đầu kỳ / Opening balance',
		0,
		0,
		0
	from
		:list_GLAccts a
		left join #GL b on b.Period = 0 and a.Account = b.Account
	where b.Account is null;
		
	insert into #GL (Account, AcctName, AcctFName, JEMemo, DocMemo, Debit, Credit, Period)
	select
		a.Account,
		a.AcctName,
		a.AcctFName,
		N'Cộng số phát sinh trong kỳ / During the period',
		N'Cộng số phát sinh trong kỳ / During the period',
		IFNULL(SUM(b.Debit),0),
		IFNULL(SUM(b.Credit),0),
		2
	from
		:list_GLAccts a
		left join #GL b on b.Period = 1 and a.Account = b.Account
	group by a.Account, a.AcctName, a.AcctFName;
		
	update b set JEMemo = a."LineMemo", DocDate = a."TaxDate"
	from JDT1 a, #GL b
	where
		a."TransId" = b.JEId
		and a."Line_ID" = b.JELine;
		
	update b set DocNum = a."DocNum", DocNo = a."U_JVNumber", DocMemo = a."Comments" from AZ_ODOC a, #GL b where a."ObjType" = b.TransType and a."DocEntry" = b.CreatedBy;
		
	update b set DocNo = a."ObjSapCode" || ' ' || CAST(IFNULL(b.DocNum,b.CreatedBy) as nvarchar(20)) from AZ_OBJTYPE a, #GL b where a."ObjID" = b.TransType and TRIM(IFNULL(b.DocNo,'')) = '';
	
	update #GL set JEURL = 'http://$b1$/link?table=OJDT&key=' || CAST(JEId as nvarchar(20));
	
	update b set DocURL = 'http://$b1$/link?table=' || a."ObjName" || '&key=' || CAST(b.CreatedBy as nvarchar(20)) from AZ_OBJTYPE a, #GL b where a."ObjID" = b.TransType;

	update b set CompName = a."PrintHeadr", CompFName = a."PrintHdrF" from OADM a, #GL b;
		
	update b set CompAdd = a."Street", CompFAdd = a."StreetF" from ADM1 a, #GL b;
	
	update b set RptCurrcy = CASE when :currTyp = 'L' then a."MainCurncy" when :currTyp = 'S' then a."SysCurrncy" end, DeciAmt = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from OADM a, #GL b;
	
	update b set DeciAmt = IFNULL(NULLIF(a."Decimals",-1),b.DeciAmt) from OCRN a, #GL b where a."CurrCode" = b.RptCurrcy;
	
	update #GL set
		FormNo = N'Mẫu số S03b-DN',
		IssueForm = N'(Ban hành theo TT số 200/2014/TT-BTC' || char(13) || char(10) || N'Ngày 22/12/2014 của BTC)',
		FormNoF = N'Form No S03b-DN',
		IssueFormF = N'(Issued with Circular No: 200/2014/TT-BTC' || char(13) || char(10) || N'Dated 22/12/2014 by Ministry of Finance)';
	
	tab_GL = select * from #GL;

	drop table #GL;
END

