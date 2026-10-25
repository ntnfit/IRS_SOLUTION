CREATE PROCEDURE VAS_CN01_DN (
	in fdate		timestamp,
	in tdate		timestamp,
	in list_BPs		table (Code nvarchar(30)),
	in list_accts	table (Code nvarchar(30)),
	in currTyp		nvarchar(1),
	in branch		int,
	out tab_BPBal	table (
		BPCode		nvarchar(30),
		BPName		nvarchar(100),
		BPFName		nvarchar(100),
		AcctCode	nvarchar(30),
		AcctName	nvarchar(100),
		AcctFName	nvarchar(100),
		OBDeb		numeric(21,6),
		OBCred		numeric(21,6),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		RptCurrcy	nvarchar(20),
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	)
)
AS
BEGIN
	create local temporary table #BPBal (
		BPCode		nvarchar(30),
		BPName		nvarchar(100),
		BPFName		nvarchar(100),
		AcctCode	nvarchar(30),
		AcctName	nvarchar(100),
		AcctFName	nvarchar(100),
		OBDeb		numeric(21,6),
		OBCred		numeric(21,6),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		RptCurrcy	nvarchar(20),
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
		Debit		numeric(21,6),
		Credit		numeric(21,6)
	);
	
	insert into #JE (TransId, LineId, RefDate, ShortName, Account, Debit, Credit)
	select
		a."TransId",
		a."Line_ID",
		a."RefDate",
		a."ShortName",
		a."Account",
		CASE when :currTyp = 'S' then IFNULL(a."SYSDeb",0) else IFNULL(a."Debit",0) end,
		CASE when :currTyp = 'S' then IFNULL(a."SYSCred",0) else IFNULL(a."Credit",0) end
	from JDT1 a, :list_BPs b, :list_accts c
	where
		a."ShortName" = b.Code
		and a."Account" = c.Code
		and (a."BPLId"=branch or branch=0)
		and a."RefDate" <= :tdate;
		
	insert into #BPBal (BPCode, AcctCode)
	select a.Code, b.Code
	from :list_BPs a, :list_accts b;
		
	update #BPBal set OBDeb = (select SUM(a.Debit-a.Credit) from #JE a where a.RefDate < :fdate and a.ShortName = #BPBal.BPCode and a.Account = #BPBal.AcctCode);
	update #BPBal set OBCred = CASE when OBDeb < 0 then -OBDeb else 0 end;
	update #BPBal set OBDeb = CASE when OBDeb > 0 then OBDeb else 0 end;
	
	update #BPBal set Debit = (select SUM(a.Debit) from #JE a where a.RefDate between :fdate and :tdate and a.ShortName = #BPBal.BPCode and a.Account = #BPBal.AcctCode);
	update #BPBal set Credit = (select SUM(a.Credit) from #JE a where a.RefDate between :fdate and :tdate and a.ShortName = #BPBal.BPCode and a.Account = #BPBal.AcctCode);
	
	delete from #BPBal where IFNULL(OBDeb,0) = 0 and IFNULL(OBCred,0) = 0 and IFNULL(Debit,0) = 0 and IFNULL(Credit,0) = 0;
	
	update a set BPName = b."CardName", BPFName = b."CardFName" from #BPBal a, OCRD b where a.BPCode = b."CardCode";
	
	update a set AcctName = b."AcctName", AcctFName = b."FrgnName" from #BPBal a, OACT b where a.AcctCode = b."AcctCode";
	
	update #BPBal set OBDeb = ifnull(OBDeb,0), OBCred = ifnull(OBCred,0), Debit = ifnull(Debit,0), Credit = ifnull(Credit,0);
	
	update a set
		CompName	= b."PrintHeadr",
		CompFName	= b."PrintHdrF",
		RptCurrcy	= CASE when :currTyp = 'L' then b."MainCurncy" when :currTyp = 'S' then b."SysCurrncy" end,
		DeciAmt		= b."SumDec",
		DecSep		= b."DecSep",
		ThousSep	= b."ThousSep"
	from #BPBal a, OADM b;
	
	update a set CompAdd = b."Street", CompFAdd = b."StreetF" from #BPBal a, ADM1 b;
	
	update a set DeciAmt = IFNULL(NULLIF(b."Decimals",-1),a.DeciAmt) from #BPBal a, OCRN b where a.RptCurrcy = b."CurrCode";
	
	tab_BPBal = select * from #BPBal;
	
	drop table #BPBal;
	drop table #JE;
END

