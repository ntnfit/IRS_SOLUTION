CREATE procedure VAS_F_OP_UNC_NCB (
	in docKey int,
	in objTyp nvarchar(30)
)
as
begin
	declare num numeric(28,10);
	declare part1, deci bigint;
	declare part2 nvarchar(10);
	declare part1Txt, part2Txt, totTxt nvarchar(500);
	declare currCode, mainCurr nvarchar(30);
	declare chkName, chk100Name nvarchar(50);
	
	select a."MainCurncy", a."SumDec" into mainCurr, deci from OADM a;

	create local temporary table #PayOrder(
		DocEntry		int,
		DocNo			nvarchar(50),
		DocDate			timestamp,
		CompName		nvarchar(100),
		CompTaxId		nvarchar(30),
		CompIssDate		timestamp,
		CompIssPlace	nvarchar(50),
		CompBankAcc		nvarchar(50),
		CompBankName	nvarchar(100),
		CardCode		nvarchar(30),
		CardName		nvarchar(100),
		CardTaxId		nvarchar(30),
		CardIssDate		timestamp,
		CardIssPlace	nvarchar(50),
		CardBankAcc		nvarchar(50),
		CardBankName	nvarchar(100),
		PaidAcct		nvarchar(30),
		TotInFigures	numeric(28,10),
		TotInWords		nvarchar(500),
		DocCurr			nvarchar(30),
		Details			nvarchar(300),
		Charge			char(1),
		DeciAmt			smallint
	);
	
	if (objTyp = '46') then
		insert into #PayOrder (DocEntry, DocNo, DocDate, CardCode, CardBankAcc, CardBankName, PaidAcct, TotInFigures, DocCurr, Details)
		select
			a."DocEntry",
			a."U_JVNumber",
			a."DocDate",
			a."CardCode",
			a."U_BankAcct",
			a."U_BankName",
			IFNULL(a."CashAcct",a."TrsfrAcct"),
			CASE when IFNULL(a."DocCurr",:mainCurr) = :mainCurr then a."DocTotal" else a."DocTotalFC" end,
			IFNULL(a."DocCurr",:mainCurr),
			a."Comments"
		from OVPM a
		where a."DocEntry" = :docKey;
	else
		insert into #PayOrder (DocEntry, DocNo, DocDate, CardCode, CardBankAcc, CardBankName, PaidAcct, TotInFigures, DocCurr, Details)
		select
			a."DocEntry",
			a."U_JVNumber",
			a."DocDate",
			a."CardCode",
			a."U_BankAcct",
			a."U_BankName",
			IFNULL(a."CashAcct",a."TrsfrAcct"),
			CASE when IFNULL(a."DocCurr",:mainCurr) = :mainCurr then a."DocTotal" else a."DocTotalFC" end,
			IFNULL(a."DocCurr",:mainCurr),
			a."Comments"
		from OPDF a
		where a."DocEntry" = :docKey and a."ObjType" = '46';
	end if;
	
	update a set CompBankAcc = b."U_BankAcct", CompBankName = b."U_BankName" from #PayOrder a, OACT b where a.PaidAcct = b."AcctCode";
	
	update a set CardName = b."CardName", CardTaxId = b."LicTradNum" from #PayOrder a, OCRD b where a.CardCode = b."CardCode";
	
	select TotInFigures, DocCurr into num, currCode from #PayOrder;
	select a."ChkName", a."Chk100Name", IFNULL(NULLIF(a."Decimals",-1),deci) into chkName, chk100Name, deci from OCRN a where a."CurrCode" = currCode;
	
	part1 = floor(num);
	part2 = CAST(num-floor(num) as float);
	
	part1Txt = Num2Text(part1) || IFNULL(' ' || chkName,'');
	
	if (part2 = '0') then
		part2Txt = '.';
	else
		part2Txt = N' và ' || lower(Num2Text(right(part2,length(part2)-2))) || IFNULL(' ' || chk100Name,'');
	end if;
	
	totTxt = part1Txt || part2Txt || '.';
	
	update #PayOrder set TotInWords = totTxt, DeciAmt = deci;
	
	update b set CompName = a."PrintHeadr", DeciAmt = deci from OADM a, #PayOrder b;
	
	select * from #PayOrder;
	
	drop table #PayOrder;
end

