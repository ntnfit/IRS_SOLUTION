CREATE PROCEDURE VAS_S31_DN (
	in fdate		timestamp,
	in tdate		timestamp,
	in list_BPs		table (Code nvarchar(30)),
	in list_accts	table (Code nvarchar(30)),
	in exclCancel	char(1) default 'N',
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
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		Balance		numeric(21,6),
		BalDeb		numeric(21,6),
		BalCred		numeric(21,6),
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
		CardName	nvarchar(100),
		CardFName	nvarchar(100),
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
	create local temporary table #BPDtls as (
		select * from :tab_BPDtls
	);
	
	if (:currTyp = 'S') then
		insert into #BPDtls (ID, ShortName, Account, JEMemo, DocMemo, Debit, Credit, Period)
		select
			0,
			a."ShortName",
			a."Account",
			N'Số dư đầu kỳ / Opening balance',
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."SYSDeb",0)-IFNULL(a."SYSCred",0)),
			0,
			0
		from
			JDT1 a
			inner join :list_BPs b on a."ShortName" = b.Code
			inner join :list_accts c on a."Account" = c.Code
		where
			a."RefDate" < :fdate
			and (a."BPLId"=branch or branch=0)
		group by a."ShortName", a."Account";
	else
		insert into #BPDtls (ID, ShortName, Account, JEMemo, DocMemo, Debit, Credit, Period)
		select
			0,
			a."ShortName",
			a."Account",
			N'Số dư đầu kỳ / Opening balance',
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."Debit",0)-IFNULL(a."Credit",0)),
			0,
			0
		from
			JDT1 a
			inner join :list_BPs b on a."ShortName" = b.Code
			inner join :list_accts c on a."Account" = c.Code
		where
			a."RefDate" < :fdate
			and (a."BPLId"=branch or branch=0)
		group by a."ShortName", a."Account";
	end if;
	
	insert into #BPDtls (ID, JEId, JELine, RefDate, ShortName, Account, ContraAct, Debit, Credit, TransType, CreatedBy, Period)
	select
		ROW_NUMBER() OVER (PARTITION BY a.BPCode, a.Account ORDER BY a.RefDate, a.TransId, a.LineId),
		a.TransId,
		a.LineId,
		a.RefDate,
		a.BPCode,
		a.Account,
		a.ContraAct,
		CASE when :currTyp = 'S' then IFNULL(a.DebitSC,0) else IFNULL(a.Debit,0) end,
		CASE when :currTyp = 'S' then IFNULL(a.CreditSC,0) else IFNULL(a.Credit,0) end,
		a.TransType,
		a.DocID,
		1
	from
		ALL_JE a
		inner join :list_BPs b on a.BPCode = b.Code
		inner join :list_accts c on a.Account = c.Code
	where
		a.RefDate between :fdate and :tdate
		and (a."BPLID"=branch or branch=0)
		;
	
	update a set JEMemo = b."LineMemo" from #BPDtls a, JDT1 b where a.JEID = b."TransId" and a.JELine = b."Line_ID"; 
	
	update b set Canceled = a."CANCELED", DocNum = a."DocNum", DocNo = a."DocNum", DocMemo = a."Comments" from APZ_ODOC a, #BPDtls b where a."ObjType" = b.TransType and a."DocEntry" = b.CreatedBy;
	
	delete from #BPDtls where exclCancel = 'Y' and IFNULL(Canceled,'N') <> 'N';
	
	update #BPDtls set Balance = (select SUM(a.Debit-a.Credit) from #BPDtls a where a.ShortName = #BPDtls.ShortName and a.Account = #BPDtls.Account and a.ID <= #BPDtls.ID);
	
	insert into #BPDtls (ShortName, Account, JEMemo, DocMemo, Balance, Period)
	select
		a.ShortName,
		a.Account,
		N'Số dư cuối kỳ / Closing balance',
		N'Số dư cuối kỳ / Closing balance',
		SUM(a.Debit-a.Credit),
		3
	from #BPDtls a
	group by a.ShortName, a.Account;
	
	list_BPBal =
		select a.ShortName, a.Account
		from #BPDtls a
		where a.Period = 3;
	
	insert into #BPDtls (ID, ShortName, Account, JEMemo, DocMemo, Debit, Credit, Period)
	select
		0,
		a.ShortName,
		a.Account,
		N'Số dư đầu kỳ / Opening balance',
		N'Số dư đầu kỳ / Opening balance',
		0,
		0,
		0
	from
		:list_BPBal a
		left join #BPDtls b on b.Period = 0 and a.ShortName = b.ShortName and a.Account = b.Account
	where b.ShortName is null;
	
	insert into #BPDtls (ShortName, Account, JEMemo, DocMemo, Debit, Credit, Period)
	select
		a.ShortName,
		a.Account,
		N'Cộng số phát sinh trong kỳ / During the period',
		N'Cộng số phát sinh trong kỳ / During the period',
		IFNULL(SUM(b.Debit),0),
		IFNULL(SUM(b.Credit),0),
		2
	from
		:list_BPBal a
		left join #BPDtls b on b.Period = 1 and a.ShortName = b.ShortName and a.Account = b.Account
	group by a.ShortName, a.Account;
	
	update #BPDtls set BalDeb = CASE when Balance >= 0 then Balance end;
	
	update #BPDtls set BalCred = CASE when Balance < 0 then -Balance end;
	
	update b set DocNo = a."ObjSapCode" || ' ' || CAST(IFNULL(b.DocNum,b.CreatedBy) as nvarchar(20)) from AZ_OBJTYPE a, #BPDtls b where a."ObjID" = b.TransType and TRIM(IFNULL(b.DocNo,'')) = '';
	
	update b set CardName = a."CardName", CardFName = a."CardFName" from OCRD a, #BPDtls b where a."CardCode" = b.ShortName;
	
	update b set AcctName = a."AcctName", AcctFName = a."FrgnName" from OACT a, #BPDtls b where a."AcctCode" = b.Account;
	
	update #BPDtls set JEURL = 'http://$b1$/link?table=OJDT&key=' || CAST(JEId as nvarchar(20));
	
	update b set DocURL = 'http://$b1$/link?table=' || a."ObjName" || '&key=' || CAST(b.CreatedBy as nvarchar(20)) from AZ_OBJTYPE a, #BPDtls b where a."ObjID" = b.TransType;

	update b set CompName = a."PrintHeadr", CompFName = a."PrintHdrF" from OADM a, #BPDtls b;
		
	update b set CompAdd = a."Street", CompFAdd = a."StreetF" from ADM1 a, #BPDtls b;
	
	update b set RptCurrcy = CASE when :currTyp = 'L' then a."MainCurncy" when :currTyp = 'S' then a."SysCurrncy" end, DeciAmt = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from OADM a, #BPDtls b;
	
	update b set DeciAmt = IFNULL(NULLIF(a."Decimals",-1),b.DeciAmt) from OCRN a, #BPDtls b where a."CurrCode" = b.RptCurrcy;
	
	update #BPDtls set
		FormNo = N'Mẫu số S31-DN',
		IssueForm = N'(Ban hành theo TT số 200/2014/TT-BTC' || char(13) || char(10) || N'Ngày 22/12/2014 của BTC)',
		FormNoF = N'Form No S31-DN',
		IssueFormF = N'(Issued with Circular No: 200/2014/TT-BTC' || char(13) || char(10) || N'Dated 22/12/2014 by Ministry of Finance)';
	
	tab_BPDtls = select * from #BPDtls;
	
	drop table #BPDtls;
END

