CREATE PROCEDURE VAS_S06_DN (
	in fdate	timestamp,
	in tdate	timestamp,
	in accLvl	int,
	in isCP		nvarchar(1),
	in currTyp	nvarchar(1),
	in filter	nvarchar(1),
	in lang		nvarchar(2),
	in branch	int,
	
	
	out tab_TB table (
		GrpMask		int,
		GrpLine		int,
		AcctCode	nvarchar(20),
		AcctName	nvarchar(400),
		FrgnName	nvarchar(300),
		AccLvl		int,
		Postable	nvarchar(1),
		FatherCode	nvarchar(20),
		OBAmt		numeric(21,6),
		BDebit		numeric(21,6),
		BCredit		numeric(21,6),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		EBAmt		numeric(21,6),
		EDebit		numeric(21,6),
		ECredit		numeric(21,6),
		LocManTran	nvarchar(1),
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CurrCode	nvarchar(20),
		CurrName	nvarchar(50),
		FormNo		nvarchar(200),
		IssueForm	nvarchar(500),
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1),
		FStyle		nvarchar(2),
		FRed		smallint,
		FGreen		smallint,
		FBlue		smallint
	)
)
AS
BEGIN
	declare idxLvl int;

	create local temporary table #TB (
		GrpMask		int,
		GrpLine		int,
		AcctCode	nvarchar(20),
		AcctName	nvarchar(400),
		FrgnName	nvarchar(300),
		AccLvl		int,
		Postable	nvarchar(1),
		FatherCode	nvarchar(20),
		OBAmt		numeric(21,6),
		BDebit		numeric(21,6),
		BCredit		numeric(21,6),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		EBAmt		numeric(21,6),
		EDebit		numeric(21,6),
		ECredit		numeric(21,6),
		LocManTran	nvarchar(1),
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CurrCode	nvarchar(20),
		CurrName	nvarchar(50),
		FormNo		nvarchar(200),
		IssueForm	nvarchar(500),
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1),
		FStyle		nvarchar(2),
		FRed		smallint,
		FGreen		smallint,
		FBlue		smallint
	);

	create local temporary column table #JE (
		RefDate		timestamp,
		Account		nvarchar(20),
		ShortName	nvarchar(20),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		TransType	nvarchar(20),
		CreatedBy	int
	);
	
	insert into #JE (RefDate,Account,ShortName,Debit,Credit,TransType,CreatedBy)
	select
		a."RefDate",
		a."Account",
		a."ShortName",
		CASE when :currTyp = 'S' then IFNULL(a."SYSDeb",0) else IFNULL(a."Debit",0) end,
		CASE when :currTyp = 'S' then IFNULL(a."SYSCred",0) else IFNULL(a."Credit",0) end,
		a."TransType",
		a."CreatedBy"
	from JDT1 a
	where
		a."RefDate" <= :tdate
		--and (cast a."BPLId"  = :branch or :branch=0)
		and (:isCP = 'Y' or (a."RefDate" < :fdate or a."TransType" <> '-3'));
		
	insert into #TB (GrpMask, GrpLine, AcctCode, AcctName, FrgnName, AccLvl, Postable, FatherCode, LocManTran, FRed, FGreen, FBlue)
	select
		a."GroupMask",
		a."GrpLine",
		a."AcctCode",
		a."AcctName",
		a."FrgnName",
		a."Levels",
		a."Postable",
		a."FatherNum",
		a."LocManTran",
		0, 0, 0
	from OACT a;
	
	update #TB set
		OBAmt	= (select SUM(a.Debit-a.Credit) from #JE a where a.Account = #TB.AcctCode and a.RefDate < :fdate),
		Debit	= (select SUM(a.Debit) from #JE a where a.Account = #TB.AcctCode and a.RefDate between :fdate and :tdate),
		Credit	= (select SUM(a.Credit) from #JE a where a.Account = #TB.AcctCode and a.RefDate between :fdate and :tdate),
		EBAmt	= (select SUM(a.Debit-a.Credit) from #JE a where a.Account = #TB.AcctCode and a.RefDate <= :tdate);
		
	update #TB set
		BDebit = CASE when OBAmt > 0 then IFNULL(OBAmt,0) else 0 end,
		BCredit = CASE when OBAmt < 0 then -IFNULL(OBAmt,0) else 0 end,
		EDebit = CASE when EBAmt > 0 then IFNULL(EBAmt,0) else 0 end,
		ECredit = CASE when EBAmt < 0 then -IFNULL(EBAmt,0) else 0 end
	where LocManTran = 'N';
	
	update #TB set
		BDebit	= (select SUM(t.Amt) from (select a.ShortName, SUM(a.Debit-a.Credit) Amt from #JE a where a.Account = #TB.AcctCode and a.RefDate < :fdate group by a.ShortName) t where t.Amt > 0),
		BCredit	= (select -SUM(t.Amt) from (select a.ShortName, SUM(a.Debit-a.Credit) Amt from #JE a where a.Account = #TB.AcctCode and a.RefDate < :fdate group by a.ShortName) t where t.Amt < 0),
		EDebit	= (select SUM(t.Amt) from (select a.ShortName, SUM(a.Debit-a.Credit) Amt from #JE a where a.Account = #TB.AcctCode and a.RefDate <= :tdate group by a.ShortName) t where t.Amt > 0),
		ECredit	= (select -SUM(t.Amt) from (select a.ShortName, SUM(a.Debit-a.Credit) Amt from #JE a where a.Account = #TB.AcctCode and a.RefDate <= :tdate group by a.ShortName) t where t.Amt < 0)
	where LocManTran = 'Y';
	
	select MAX(a.AccLvl)-1 into idxLvl from #TB a;
	
	while (:idxLvl > 0)
	do
		update #TB set
			BDebit	= (select SUM(a.BDebit) from #TB a where a.FatherCode = #TB.AcctCode),
			BCredit	= (select SUM(a.BCredit) from #TB a where a.FatherCode = #TB.AcctCode),
			Debit	= (select SUM(a.Debit) from #TB a where a.FatherCode = #TB.AcctCode),
			Credit	= (select SUM(a.Credit) from #TB a where a.FatherCode = #TB.AcctCode),
			EDebit	= (select SUM(a.EDebit) from #TB a where a.FatherCode = #TB.AcctCode),
			ECredit	= (select SUM(a.ECredit) from #TB a where a.FatherCode = #TB.AcctCode)
		where #TB.AccLvl = :idxLvl and #TB.Postable = 'N';
		
		select :idxLvl - 1 into idxLvl from dummy;
	end while;
	
	update b set CurrCode = CASE when :currTyp = 'S' then a."SysCurrncy" else a."MainCurncy" end, DeciAmt = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from OADM a, #TB b;
	
	update b set CurrName = a."CurrName", DeciAmt = IFNULL(NULLIF(a."Decimals",-1),b.DeciAmt) from OCRN a, #TB b where a."CurrCode" = b.CurrCode;
	
	if (:lang = 'VN')
	then
		update #TB set FormNo = N'Mẫu số S06-DN', IssueForm = N'(Ban hành theo TT số 200/2014/TT-BTC' || char(13) || char(10) || N'Ngày 22/12/2014 của BTC)';
		
		update b set CompName = a."PrintHeadr" from OADM a, #TB b;
		
		update b set CompAdd = a."Street" from ADM1 a, #TB b;
	else
		update #TB set FormNo = N'Form No S06-DN', IssueForm = N'(Issued with Circular No: 200/2014/TT-BTC' || char(13) || char(10) || N'Dated 22/12/2014 by Ministry of Finance)', AcctName = FrgnName;
	
		update b set CompName = "PrintHdrF" from OADM a, #TB b;
		
		update b set CompAdd = a."StreetF" from ADM1 a, #TB b;
	end if;
	
	update #TB set AcctName = LPAD(AcctName,LENGTH(AcctName)+(AccLvl-1)*3,' ') where :accLvl <> 0;
	
	update #TB set FStyle = 'B' where AccLvl <= 2;
	
	update #TB set FRed = 100, FGreen = 100, FBlue = 100 where Postable = 'N';
	
	update #TB set AcctName = UPPER(AcctName) where AccLvl = 1;
	
	tab_TB =
		select *
		from #TB
		where
			((:accLvl = 0 and Postable = 'Y')
				or AccLvl <= :accLvl)
			and (:filter = 'N' or IFNULL(BDebit,0) <> 0 or IFNULL(BCredit,0) <> 0 or IFNULL(Debit,0) <> 0 or IFNULL(Credit,0) <> 0 or IFNULL(EDebit,0) <> 0 or IFNULL(ECredit,0) <> 0)
			and AccLvl>1
		--	and GrpMask < 8
			;
			
	drop table #TB;
	drop table #JE;
END

