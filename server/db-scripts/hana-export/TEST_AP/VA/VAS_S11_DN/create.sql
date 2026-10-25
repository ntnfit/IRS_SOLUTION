CREATE PROCEDURE VAS_S11_DN (
	in list_accts	table (Code nvarchar(30)),
	in fdate		timestamp,
	in tdate		timestamp,
	in branch		int,
	out tab_Items	table (
		AcctCode	nvarchar(30),
		AcctName	nvarchar(100),
		AcctFName	nvarchar(100),
		ItemCode	nvarchar(50),
		ItemName	nvarchar(100),
		ItemFName	nvarchar(100),
		ItmGrpCod	int,
		ItmGrpNam	nvarchar(100),
		OBValue		numeric(21,6),
		InValue		numeric(21,6),
		OutValue	numeric(21,6),
		EBValue		numeric(21,6),
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		RptCurr		nvarchar(30),
		FormNo		nvarchar(200),
		IssueForm	nvarchar(500),
		FormNoF		nvarchar(200),
		IssueFormF	nvarchar(500),
		DeciAmt		smallint,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	)
)
as
begin
	create local temporary table #tbl as (
		select * from :tab_Items
	);
	
	insert into #tbl (AcctCode, ItemCode, OBValue, InValue, OutValue, EBValue)
	select
		a."InvntAct",
		a."ItemCode",
		SUM(CASE when a."DocDate" < fdate then a."TransValue" else 0 end),
		SUM(CASE when a."DocDate" >= fdate and (a."TransType" in ('14','16','18','20','67') or a."TransValue" > 0) and a."TransType" not in ('13','15','19','21') then a."TransValue" else 0 end),
		SUM(CASE when a."DocDate" >= fdate and (a."TransType" in ('13','15','19','21') or a."TransValue" < 0) and a."TransType" not in ('14','16','18','20','67') then -a."TransValue" else 0 end),
		SUM(a."TransValue")
	from
		OINM a
		inner join :list_accts b on a."InvntAct" = b.Code
		inner join OWHS c on (c."WhsCode"=a."Warehouse" and (c."BPLid"=branch or branch=0))
	where a."DocDate" <= tdate
	group by a."InvntAct", a."ItemCode";
	
	delete from #tbl where IFNULL(OBValue,0)=0 and IFNULL(InValue,0)=0 and IFNULL(OutValue,0)=0 and IFNULL(EBValue,0)=0;
	
	update tbl set AcctName = a."AcctName", AcctFName = a."FrgnName" from #tbl tbl, OACT a where tbl.AcctCode = a."AcctCode";
	
	update tbl set ItemName = a."ItemName", ItemFName = a."FrgnName", ItmGrpCod = b."ItmsGrpCod", ItmGrpNam = b."ItmsGrpNam" from #tbl tbl, OITM a, OITB b where tbl.ItemCode = a."ItemCode" and a."ItmsGrpCod" = b."ItmsGrpCod";
	
	update tbl set CompName = a."PrintHeadr", CompFName = a."PrintHdrF" from #tbl tbl, OADM a;
		
	update tbl set CompAdd = a."Street", CompFAdd = a."StreetF" from #tbl tbl, ADM1 a;
	
	update tbl set RptCurr = a."MainCurncy", DeciAmt = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep" from #tbl tbl, OADM a;
	
	update tbl set DeciAmt = IFNULL(NULLIF(a."Decimals",-1),tbl.DeciAmt) from #tbl tbl, OCRN a where a."CurrCode" = tbl.RptCurr;
	
	update #tbl set
		FormNo = N'Mẫu số S11-DN',
		IssueForm = N'(Ban hành theo TT số 200/2014/TT-BTC' || char(13) || char(10) || N'Ngày 22/12/2014 của BTC)',
		FormNoF = N'Form No S11-DN',
		IssueFormF = N'(Issued with Circular No: 200/2014/TT-BTC' || char(13) || char(10) || N'Dated 22/12/2014 by Ministry of Finance)';
	
	tab_Items = select * from #tbl;
	
	drop table #tbl;
end

