CREATE PROCEDURE VAS_CN01c_DN_FC (
	in fdate		timestamp,
	in tdate		timestamp,
	in list_BPs		table (Code nvarchar(30)),
	in list_accts	table (Code nvarchar(30)),
	in currTyp		nvarchar(1),
	out tab_BPBal	table (
		BPCode		nvarchar(30),
		BPName		nvarchar(100),
		BPFName		nvarchar(100),
		AcctCode	nvarchar(30),
		AcctName	nvarchar(100),
		AcctFName	nvarchar(100),
		FCOBDebSum	numeric(21,6),
		FCOBCredSum	numeric(21,6),
		FCOBDeb		numeric(21,6),
		FCOBCred	numeric(21,6),
		FCDebit		numeric(21,6),
		FCCredit	numeric(21,6),
		FCEBDebSum	numeric(21,6),
		FCEBCredSum	numeric(21,6),
		FCEBDeb		numeric(21,6),
		FCEBCred	numeric(21,6),
		FCCurr		nvarchar(30),
		OBDebSum	numeric(21,6),
		OBCredSum	numeric(21,6),
		OBDeb		numeric(21,6),
		OBCred		numeric(21,6),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		EBDebSum	numeric(21,6),
		EBCredSum	numeric(21,6),
		EBDeb		numeric(21,6),
		EBCred		numeric(21,6),
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		RptCurrcy	nvarchar(20),
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

	create local temporary table #BPBal as (
		select * from :tab_BPBal
	);

	create local temporary table #JE (
		TransId		int,
		LineId		int,
		RefDate		timestamp,
		ShortName	nvarchar(30),
		Account		nvarchar(30),
		FCDebit		numeric(21,6),
		FCCredit	numeric(21,6),
		FCCurr		nvarchar(30),
		Debit		numeric(21,6),
		Credit		numeric(21,6)
	);
	
	insert into #JE (TransId, LineId, RefDate, ShortName, Account, FCDebit, FCCredit, FCCurr, Debit, Credit)
	select
		a."TransId",
		a."Line_ID",
		a."RefDate",
		a."ShortName",
		a."Account",
		a."FCDebit",
		a."FCCredit",
		UPPER(IFNULL(a."FCCurrency",mainCurr)),
		CASE when :currTyp = 'S' then IFNULL(a."SYSDeb",0) else IFNULL(a."Debit",0) end,
		CASE when :currTyp = 'S' then IFNULL(a."SYSCred",0) else IFNULL(a."Credit",0) end
	from JDT1 a, :list_BPs b, :list_accts c
	where
		a."ShortName" = b.Code
		and a."Account" = c.Code
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
		0,--CASE when :currTyp = 'S' then IFNULL(a."SYSDeb",0) else IFNULL(a."Debit",0) end,
		0--CASE when :currTyp = 'S' then IFNULL(a."SYSCred",0) else IFNULL(a."Credit",0) end
	from JDT1 a, :list_BPs b, :list_accts c
	where
		a."ShortName" = b.Code
		and a."Account" = c.Code
		and a."RefDate" <= :tdate
		and IFNULL(a."U_ConvertCurr",'') <> ''
		and IFNULL(a."U_AmountConvert",0) <> 0;
	*/
	insert into #BPBal (BPCode, AcctCode, FCCurr, FCOBDebSum, FCOBCredSum, FCDebit, FCCredit, OBDebSum, OBCredSum, Debit, Credit)
	select
		a.ShortName,
		a.Account,
		a.FCCurr,
		IFNULL(SUM(CASE when a.RefDate < fdate then a.FCDebit else 0 end),0),
		IFNULL(SUM(CASE when a.RefDate < fdate then a.FCCredit else 0 end),0),
		IFNULL(SUM(CASE when a.RefDate >= fdate then a.FCDebit else 0 end),0),
		IFNULL(SUM(CASE when a.RefDate >= fdate then a.FCCredit else 0 end),0),
		IFNULL(SUM(CASE when a.RefDate < fdate then a.Debit else 0 end),0),
		IFNULL(SUM(CASE when a.RefDate < fdate then a.Credit else 0 end),0),
		IFNULL(SUM(CASE when a.RefDate >= fdate then a.Debit else 0 end),0),
		IFNULL(SUM(CASE when a.RefDate >= fdate then a.Credit else 0 end),0)
	from #JE a
	group by a.ShortName, a.Account, a.FCCurr;
	
	update #BPBal set FCOBDeb =
						CASE
							when abs(FCOBDebSum) > abs(FCOBCredSum) then FCOBDebSum-FCOBCredSum
							when abs(FCOBDebSum) = abs(FCOBCredSum) then CASE when FCOBDebSum-FCOBCredSum > 0 then FCOBDebSum-FCOBCredSum else 0 end
							else 0
						end;
	update #BPBal set FCOBCred =
						CASE
							when abs(FCOBDebSum) < abs(FCOBCredSum) then FCOBCredSum-FCOBDebSum
							when abs(FCOBDebSum) = abs(FCOBCredSum) then CASE when FCOBCredSum-FCOBDebSum > 0 then FCOBCredSum-FCOBDebSum else 0 end
							else 0
						end;
	
	update #BPBal set OBDeb =
						CASE
							when abs(OBDebSum) > abs(OBCredSum) then OBDebSum-OBCredSum
							when abs(OBDebSum) = abs(OBCredSum) then CASE when OBDebSum-OBCredSum > 0 then OBDebSum-OBCredSum else 0 end
							else 0
						end;
	update #BPBal set OBCred =
						CASE
							when abs(OBDebSum) < abs(OBCredSum) then OBCredSum-OBDebSum
							when abs(OBDebSum) = abs(OBCredSum) then CASE when OBCredSum-OBDebSum > 0 then OBCredSum-OBDebSum else 0 end
							else 0
						end;
	
	delete from #BPBal where IFNULL(FCOBDeb,0) = 0 and IFNULL(FCOBCred,0) = 0 and IFNULL(FCDebit,0) = 0 and IFNULL(FCCredit,0) = 0 and IFNULL(OBDeb,0) = 0 and IFNULL(OBCred,0) = 0 and IFNULL(Debit,0) = 0 and IFNULL(Credit,0) = 0;
	
	update #BPBal set FCEBDebSum = FCOBDeb + FCDebit, FCEBCredSum = FCOBCred + FCCredit, EBDebSum = OBDeb + Debit, EBCredSum = OBCred + Credit;
	
	update #BPBal set FCEBDeb =
						CASE
							when abs(FCEBDebSum) > abs(FCEBCredSum) then FCEBDebSum-FCEBCredSum
							when abs(FCEBDebSum) = abs(FCEBCredSum) then CASE when FCEBDebSum-FCEBCredSum > 0 then FCEBDebSum-FCEBCredSum else 0 end
							else 0
						end;
	update #BPBal set FCEBCred =
						CASE
							when abs(FCEBDebSum) < abs(FCEBCredSum) then FCEBCredSum-FCEBDebSum
							when abs(FCEBDebSum) = abs(FCEBCredSum) then CASE when FCEBCredSum-FCEBDebSum > 0 then FCEBCredSum-FCEBDebSum else 0 end
							else 0
						end;
	
	update #BPBal set EBDeb =
						CASE
							when abs(EBDebSum) > abs(EBCredSum) then EBDebSum-EBCredSum
							when abs(EBDebSum) = abs(EBCredSum) then CASE when EBDebSum-EBCredSum > 0 then EBDebSum-EBCredSum else 0 end
							else 0
						end;
	update #BPBal set EBCred =
						CASE
							when abs(EBDebSum) < abs(EBCredSum) then EBCredSum-EBDebSum
							when abs(EBDebSum) = abs(EBCredSum) then CASE when EBCredSum-EBDebSum > 0 then EBCredSum-EBDebSum else 0 end
							else 0
						end;
	
	update a set BPName = b."CardName", BPFName = b."CardFName" from #BPBal a, OCRD b where a.BPCode = b."CardCode";
	
	update a set AcctName = b."AcctName", AcctFName = b."FrgnName" from #BPBal a, OACT b where a.AcctCode = b."AcctCode";
	
	update a set
		CompName	= b."PrintHeadr",
		CompFName	= b."PrintHdrF",
		RptCurrcy	= CASE when :currTyp = 'L' then b."MainCurncy" when :currTyp = 'S' then b."SysCurrncy" end,
		DeciAmtFC	= b."SumDec",
		DeciAmt		= b."SumDec",
		DecSep		= b."DecSep",
		ThousSep	= b."ThousSep"
	from #BPBal a, OADM b;
	
	update a set CompAdd = b."Street", CompFAdd = b."StreetF" from #BPBal a, ADM1 b;
	
	update a set DeciAmtFC = IFNULL(NULLIF(b."Decimals",-1),a.DeciAmtFC) from #BPBal a, OCRN b where a.FCCurr = b."CurrCode";
	
	update a set DeciAmt = IFNULL(NULLIF(b."Decimals",-1),a.DeciAmt) from #BPBal a, OCRN b where a.RptCurrcy = b."CurrCode";
	
	tab_BPBal = select * from #BPBal;
	
	drop table #BPBal;
	drop table #JE;
END

