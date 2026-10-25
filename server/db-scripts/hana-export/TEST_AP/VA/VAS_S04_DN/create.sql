CREATE procedure VAS_S04_DN (
	in fdate timestamp,
	in tdate timestamp,
	in currTyp nvarchar(3),
	in branch int
)
as
begin
	declare D1Name, D2Name, D3Name, D4Name, D5Name nvarchar(50);
	
	select a."DimDesc" into D1Name from ODIM a where a."DimCode" = 1;
	select a."DimDesc" into D2Name from ODIM a where a."DimCode" = 2;
	select a."DimDesc" into D3Name from ODIM a where a."DimCode" = 3;
	select a."DimDesc" into D4Name from ODIM a where a."DimCode" = 4;
	select a."DimDesc" into D5Name from ODIM a where a."DimCode" = 5;
	
	tblD1 = select "OcrCode","OcrName" from OOCR where "DimCode"=1;
	tblD2 = select "OcrCode","OcrName" from OOCR where "DimCode"=2;
	tblD3 = select "OcrCode","OcrName" from OOCR where "DimCode"=3;
	tblD4 = select "OcrCode","OcrName" from OOCR where "DimCode"=4;
	tblD5 = select "OcrCode","OcrName" from OOCR where "DimCode"=5;
	tblPrj = select "PrjCode","PrjName" from OPRJ;
	
	tblDOC = select "ObjID",max("ObjName") as "ObjName",max("ObjSapCode") as "ObjSapCode" from AZ_OBJTYPE group by "ObjID";
	
	create local temporary table #tblCOMP (CompName nvarchar(100), CompAddr nvarchar(300), RptCurr nvarchar(10), SumDec int, DeciAmt int, DecSep nvarchar(10), ThousSep nvarchar(10));
	insert into #tblCOMP select top 1 a."CompnyName",b."Street",CASE when :currTyp = 'L' then a."MainCurncy" else a."SysCurrncy" end,a."SumDec",0,a."DecSep",a."ThousSep" from OADM a, ADM1 b;
	update b set DeciAmt = IFNULL(NULLIF(a."Decimals",-1),b.DeciAmt) from OCRN a, #tblCOMP b where a."CurrCode" = b.RptCurr;
	
--	tblCURR = select "DocCurrCod","CurrCode" from OCRN;
	
	create local temporary table #Docs (
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
		OcrCode1	nvarchar(30),
		OcrCode2	nvarchar(30),
		OcrCode3	nvarchar(30),
		OcrCode4	nvarchar(30),
		OcrCode5	nvarchar(30),
		PrjCode		nvarchar(30),
		OcrName1	nvarchar(150),
		OcrName2	nvarchar(150),
		OcrName3	nvarchar(150),
		OcrName4	nvarchar(150),
		OcrName5	nvarchar(150),
		PrjName		nvarchar(150),
		DimName1	nvarchar(50),
		DimName2	nvarchar(50),
		DimName3	nvarchar(50),
		DimName4	nvarchar(50),
		DimName5	nvarchar(50),
		JEURL		nvarchar(100),
		
		TransType	nvarchar(20),
		CreatedBy	int,
		DocCode		nvarchar(30),
		DocNum		int,
		DocNo		nvarchar(100),
		BPCode		nvarchar(30),
		BPName		nvarchar(100),
		DocMemo		nvarchar(300),
		DocURL		nvarchar(100),
		
		CompName	nvarchar(100),
		CompAdd		nvarchar(300),
		CompFName	nvarchar(100),
		CompFAdd	nvarchar(300),
		RptCurrcy	nvarchar(20),
		FormNo		nvarchar(200) default 'Mẫu số S04-DN',
		IssueForm	nvarchar(500) default '(Ban hành theo TT số 200/2014/TT-BTC  Ngày 22/12/2014 của BTC)',
		FormNoF		nvarchar(200),
		IssueFormF	nvarchar(500),
		DeciAmtFC	int,
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	);
	
	insert into #Docs (JEId, JELine, RefDate, JEMemo, Account, Debit, Credit, FCDebit, FCCredit, FCCurrcy, OcrCode1, OcrCode2, OcrCode3, OcrCode4, OcrCode5, PrjCode, TransType, CreatedBy,JEURL,
		DimName1,DimName2,DimName3,DimName4,DimName5,
		OcrName1,OcrName2,OcrName3,OcrName4,OcrName5,PrjName,
		DocNum,DocNo,BPCode,BPName,DocMemo,
		DocURL,
		CompName,CompAdd,RptCurrcy,DeciAmtFC,DeciAmt,DecSep,ThousSep)
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
		a."ProfitCode",
		a."OcrCode2",
		a."OcrCode3",
		a."OcrCode4",
		a."OcrCode5",
		a."Project",
		a."TransType",
		a."CreatedBy",
		'http://$b1$/link?table=OJDT&key=' || CAST(a."TransId" as nvarchar(20)),
		:D1Name,:D2Name,:D3Name,:D4Name,:D5Name,
		d1."OcrName",d2."OcrName",d3."OcrName",d4."OcrName",d5."OcrName",p."PrjName",
		doc."DocNum",doc."U_JVNumber",doc."CardCode",doc."CardName",doc."Comments",
		'http://$b1$/link?table=' || f."ObjName" || '&key=' || CAST(a."CreatedBy" as nvarchar(20)),
		g.CompName,g.CompAddr,g.RptCurr,g.SumDec,g.DeciAmt,g.DecSep,g.ThousSep
	from JDT1 a
	left join :tblD1 d1 on d1."OcrCode"=a."ProfitCode"
	left join :tblD2 d2 on d2."OcrCode"=a."OcrCode2"
	left join :tblD3 d3 on d3."OcrCode"=a."OcrCode3"
	left join :tblD4 d4 on d4."OcrCode"=a."OcrCode4"
	left join :tblD5 d5 on d5."OcrCode"=a."OcrCode5"
	left join :tblPrj p on p."PrjCode"=a."Project"
	left join AZ_ODOC doc on doc."ObjType"=a."TransType" and doc."DocEntry"=a."CreatedBy"
	left join :tblDOC f on f."ObjID"=a."TransType"
	left join #tblCOMP g on 1=1
	where a."RefDate" between :fdate and :tdate
	and (a."BPLId"=branch or branch=0)
	and (a."Debit"<>0 or a."Credit"<>0)
	;
	
	select * from #Docs order by RefDate, JEID, JELine;
	
	drop table #Docs;
end

