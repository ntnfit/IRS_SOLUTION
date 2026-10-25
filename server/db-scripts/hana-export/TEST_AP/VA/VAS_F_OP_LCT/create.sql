CREATE procedure VAS_F_OP_LCT (
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

	create local temporary table #PayOrder (
		DocEntry		int,
		DocNo			nvarchar(50),
		PayMethod		nvarchar(100),
		BankName		nvarchar(100),
		BankAccNo		nvarchar(50),
		DocDate			timestamp,
		TotInFigures	numeric(21,9),
		TotInWords		nvarchar(300),
		DocCurr			nvarchar(30),
		PaidAcct		nvarchar(30),
		CompName		nvarchar(100),
		CompTaxID		nvarchar(30),
		CompIDIssDate	timestamp,
		CompIDIssPlace	nvarchar(100),
		CompTel			nvarchar(30),
		CompAdd			nvarchar(300),
		TempBankCode	nvarchar(30),
		TempBankName	nvarchar(100),
		TempBankAdd		nvarchar(300),
		TempBankSwift	nvarchar(30),
		CardBankCode	nvarchar(30),
		CardBankName	nvarchar(100),
		CardBankAdd		nvarchar(300),
		CardBankSwift	nvarchar(30),
		CardCode		nvarchar(30),
		CardName		nvarchar(100),
		CardAdd			nvarchar(300),
		CardBankAccNo	nvarchar(30),
		Details			nvarchar(300),
		DeciAmt			smallint
	);

	if (objTyp = '46') then
		insert into #PayOrder (DocEntry, DocNo, DocDate, TotInFigures, DocCurr, PaidAcct, CardBankAccNo, CardBankName, CardBankSwift, CardCode, Details)
		select
			a."DocEntry",
			a."U_JVNumber",
			a."DocDate",
			CASE when IFNULL(a."DocCurr",:mainCurr) = :mainCurr then a."DocTotal" else a."DocTotalFC" end,
			IFNULL(a."DocCurr",:mainCurr),
			IFNULL(a."CashAcct",a."TrsfrAcct"),
			a."U_BankAcct",
			a."U_BankName",
			a."U_Swift",
			a."CardCode",
			a."Comments"
		from OVPM a
		where a."DocEntry" = :docKey;
	else
		insert into #PayOrder (DocEntry, DocNo, DocDate, TotInFigures, DocCurr, PaidAcct, CardBankAccNo, CardBankName, CardBankSwift, CardCode, Details)
		select
			a."DocEntry",
			a."U_JVNumber",
			a."DocDate",
			CASE when IFNULL(a."DocCurr",:mainCurr) = :mainCurr then a."DocTotal" else a."DocTotalFC" end,
			IFNULL(a."DocCurr",:mainCurr),
			IFNULL(a."CashAcct",a."TrsfrAcct"),
			a."U_BankAcct",
			a."U_BankName",
			a."U_Swift",
			a."CardCode",
			a."Comments"
		from OPDF a
		where a."DocEntry" = :docKey;
	end if;
	
	--update a set CardBankAdd = b."" from #PayOrder a, [@BP_BANK_1] b on a.CardCode = b."Code" and a.CardBankAccNo = b."U_AccountNo";
	
	update a set BankAccNo = b."U_BankAcct", BankName = b."U_BankName" from #PayOrder a, OACT b where a.PaidAcct = b."AcctCode";
	
	update a set CardName = b."CardName" from #PayOrder a, OCRD b where a.CardCode = b."CardCode";
	
	select TotInFigures, DocCurr into num, currCode from #PayOrder;
	select a."ChkName", a."Chk100Name", IFNULL(NULLIF(a."Decimals",-1),deci) into chkName, chk100Name, deci from OCRN a where a."CurrCode" = currCode;
	
	part1 = floor(num);
	part2 = CAST(num-floor(num) as float);
	
	part1Txt = Num2Text(part1) || IFNULL(' ' || chkName,'');
	
	if (part2 = '0') then
		part2Txt = '';
	else
		part2Txt = N' và ' || lower(Num2Text(right(part2,length(part2)-2))) || IFNULL(' ' || chk100Name,'');
	end if;
	
	totTxt = part1Txt || part2Txt || '.';
	
	update #PayOrder set TotInWords = totTxt, DeciAmt = deci;
	
	update b set CompName = a."PrintHeadr", DeciAmt = deci from OADM a, #PayOrder b;
	
	select * from #PayOrder;
	
	drop table #PayOrder;
end

