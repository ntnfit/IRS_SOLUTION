CREATE PROCEDURE VAS_S03a_DN (
	in fdate	timestamp,
	in tdate	timestamp,
	in currTyp	nvarchar(3),
	in branch	int,
	out tab_JL	table (
		JEId		int,
		JELine		int,
		RefDate		timestamp,
		JEMemo		nvarchar(300),
		Account		nvarchar(30),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		FCDebit		numeric(21,6),
		FCCredit	numeric(21,6),
		FCCurrcy	nvarchar(20),
		FCSymbol	nvarchar(20),
		FCDeci		int,
		JEURL		nvarchar(100),
		
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
	create local temporary table #JL (
		JEId		int,
		JELine		int,
		RefDate		timestamp,
		JEMemo		nvarchar(300),
		Account		nvarchar(30),
		Debit		numeric(21,6),
		Credit		numeric(21,6),
		FCDebit		numeric(21,6),
		FCCredit	numeric(21,6),
		FCCurrcy	nvarchar(20),
		FCSymbol	nvarchar(20),
		FCDeci		int,
		JEURL		nvarchar(100),
		
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
		RptCurrcy	nvarchar(20),
		FormNo		nvarchar(200),
		IssueForm	nvarchar(500),
		FormNoF		nvarchar(200),
		IssueFormF	nvarchar(500),
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	);
	
	insert into #JL (JEId, JELine, RefDate, JEMemo, Account, Debit, Credit, FCDebit, FCCredit, FCCurrcy, TransType, CreatedBy)
	select
		a."TransId",
		a."Line_ID",
		a."RefDate",
		a."LineMemo",
		a."Account",
		CASE when :currTyp = 'L' then a."Debit" when :currTyp = 'S' then a."SYSDeb" end,
		CASE when :currTyp = 'L' then a."Credit" when :currTyp = 'S' then a."SYSCred" end,
		a."FCDebit",
		a."FCCredit",
		a."FCCurrency",
		a."TransType",
		a."CreatedBy"
	from JDT1 a
	where a."RefDate" between :fdate and :tdate;
	--and (a."BPLId"=branch or branch=0);
	
	delete from #JL where IFNULL(Debit,0) = 0 and IFNULL(Credit,0) = 0 and IFNULL(FCDebit,0) = 0 and IFNULL(FCCredit,0) = 0;
	
	update b set DocNum = a."DocNum", DocNo = a."DocNum", DocMemo = a."Comments" from AZ_ODOC a, #JL b where a."ObjType" = b.TransType and a."DocEntry" = b.CreatedBy;
	
	update b set DocNo = a."ObjSapCode" || ' ' || CAST(IFNULL(b.DocNum,b.CreatedBy) as nvarchar(20)) from AZ_OBJTYPE a, #JL b where a."ObjID" = b.TransType and TRIM(IFNULL(b.DocNo,'')) = '';
	
	update #JL set JEURL = 'http://$b1$/link?table=OJDT&key=' || CAST(JEId as nvarchar(20));
	
	update b set DocURL = 'http://$b1$/link?table=' || a."ObjName" || '&key=' || CAST(b.CreatedBy as nvarchar(20)) from AZ_OBJTYPE a, #JL b where a."ObjID" = b.TransType;

	update b set CompName = a."PrintHeadr", CompFName = a."PrintHdrF" from OADM a, #JL b;
		
	update b set CompAdd = a."Street", CompFAdd = a."StreetF" from ADM1 a, #JL b;
	
	update b set FCDeci = a."SumDec", RptCurrcy = CASE when :currTyp = 'L' then a."MainCurncy" when :currTyp = 'S' then a."SysCurrncy" end, DeciAmt = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from OADM a, #JL b;
	
	update b set FCSymbol = a."DocCurrCod", FCDeci = IFNULL(NULLIF(a."Decimals",-1),b.FCDeci) from OCRN a, #JL b where a."CurrCode" = b.FCCurrcy;
	
	update b set DeciAmt = IFNULL(NULLIF(a."Decimals",-1),b.DeciAmt) from OCRN a, #JL b where a."CurrCode" = b.RptCurrcy;
	
	update #JL set
		FormNo		= N'Mẫu số S03a-DN',
		IssueForm	= N'(Ban hành theo TT số 200/2014/TT-BTC' || char(13) || char(10) || N'Ngày 22/12/2014 của BTC)',
		FormNoF		= N'Form No S03a-DN',
		IssueFormF	= N'(Issued with Circular No: 200/2014/TT-BTC' || char(13) || char(10) || N'Dated 22/12/2014 by Ministry of Finance)';
	
	tab_JL = select * from #JL;
	--update x set DocNo = b."DocNum" from #JL x left join APZ_ODOC b on b."DocEntry" = x.CreatedBy and b."ObjType" = x.TransType;

	drop table #JL;
END

