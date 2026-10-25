CREATE PROCEDURE VAS_S38_DN (
	in fdate		timestamp,
	in tdate		timestamp,
	in list_accts	table (Code nvarchar(30)),
	in currTyp		nvarchar(1),
	in branch		int,
	out tab_AC	table (
		ID			int,
		JEId		int,
		JELine		int,
		RefDate		timestamp,
		JEMemo		nvarchar(300),
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
	create local temporary table #AC (
		ID			int,
		JEId		int,
		JELine		int,
		RefDate		timestamp,
		JEMemo		nvarchar(300),
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
		FormNo		nvarchar(200) default 'Mẫu số S38-DN',
		IssueForm	nvarchar(500) default '(Ban hành theo TT số 200/2014/TT-BTC Ngày 22/12/2014 của BTC)',
		FormNoF		nvarchar(200) default 'Form No S31-DN',
		IssueFormF	nvarchar(500) default '(Issued with Circular No: 200/2014/TT-BTC Dated 22/12/2014 by Ministry of Finance)',
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	);
	
	tblACCOUNT = select b."AcctCode",b."AcctName",b."FrgnName" from :list_accts a, OACT b where a.Code=b."AcctCode";
	
	tblDOC = select "ObjID",max("ObjSapCode") as "ObjSapCode",max("ObjName") as "ObjName" from AZ_OBJTYPE group by "ObjID";
	
	create local temporary table #tblCOMP (CompName nvarchar(100), CompAddr nvarchar(300), RptCurr nvarchar(10), SumDec int, DeciAmt int, DecSep nvarchar(10), ThousSep nvarchar(10));
	insert into #tblCOMP select top 1 a."CompnyName",b."Street",CASE when :currTyp = 'L' then a."MainCurncy" else a."SysCurrncy" end,a."SumDec",0,a."DecSep",a."ThousSep" from OADM a, ADM1 b;
	update b set DeciAmt = IFNULL(NULLIF(a."Decimals",-1),b.DeciAmt) from OCRN a, #tblCOMP b where a."CurrCode" = b.RptCurr;
	
	tblCURR = select "DocCurrCod","CurrCode" from OCRN;
	
	
	
	if (:currTyp = 'S') then
		insert into #AC (ID, Account,AcctName,AcctFName, JEMemo, DocMemo, Debit, Credit, Period,
			CompName,CompAdd,RptCurrcy,DeciAmt,DecSep,ThousSep)
		select
			0,
			a."Account",c."AcctName",c."FrgnName",
			N'Số dư đầu kỳ / Opening balance',
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."SYSDeb",0)-IFNULL(a."SYSCred",0)),
			0,
			0,
			d.CompName,d.CompAddr,d.RptCurr,d.DeciAmt,d.DecSep,d.ThousSep
		from
			JDT1 a
			inner join :tblACCOUNT c on a."Account" = c."AcctCode"
			left join #tblCOMP d on 1=1
		where
			a."RefDate" < :fdate
			and (a."BPLId"=branch or branch=0)
		group by a."ShortName", a."Account",c."AcctName",c."FrgnName",d.CompName,d.CompAddr,d.RptCurr,d.DeciAmt,d.DecSep,d.ThousSep;
	else
		insert into #AC (ID, Account,AcctName,AcctFName, JEMemo, DocMemo, Debit, Credit, Period,
			CompName,CompAdd,RptCurrcy,DeciAmt,DecSep,ThousSep)
		select
			0,
			a."Account",c."AcctName",c."FrgnName",
			N'Số dư đầu kỳ / Opening balance',
			N'Số dư đầu kỳ / Opening balance',
			SUM(IFNULL(a."Debit",0)-IFNULL(a."Credit",0)),
			0,
			0,
			d.CompName,d.CompAddr,d.RptCurr,d.DeciAmt,d.DecSep,d.ThousSep
		from
			JDT1 a
			inner join :tblACCOUNT c on a."Account" = c."AcctCode"
			left join #tblCOMP d on 1=1
		where
			a."RefDate" < :fdate
			and (a."BPLId"=branch or branch=0)
		group by a."ShortName", a."Account",c."AcctName",c."FrgnName",d.CompName,d.CompAddr,d.RptCurr,d.DeciAmt,d.DecSep,d.ThousSep;
	end if;
	---------------------------------------------------------------------------------------------------------------
	insert into #AC (ID, JEId, JELine, RefDate, Account,AcctName,AcctFName, ContraAct, Debit, Credit, TransType, CreatedBy, Period, JEURL,DocURL,DocNo,DocNum,DocMemo,
		CompName,CompAdd,RptCurrcy,DeciAmt,DecSep,ThousSep)
	select
		ROW_NUMBER() OVER (PARTITION BY a.Account ORDER BY a.RefDate, a.TransId, a.LineId),
		a.TransId,
		a.LineId,
		a.RefDate,
		a.Account,c."AcctName",c."FrgnName",
		a.ContraAct,
		CASE when :currTyp = 'S' then IFNULL(a.DebitSC,0) else IFNULL(a.Debit,0) end,
		CASE when :currTyp = 'S' then IFNULL(a.CreditSC,0) else IFNULL(a.Credit,0) end,
		a.TransType,
		a.DocID,
		1,
		'http://$b1$/link?table=OJDT&key=' || CAST(a.TransId as nvarchar(20)),
		'http://$b1$/link?table=' || e."ObjName" || '&key=' || CAST(a.DocID as nvarchar(20)),
		e."ObjSapCode" || ' ' || CAST(a.DocID as nvarchar(20)),
		doc."DocNum",doc."Comments",
		d.CompName,d.CompAddr,d.RptCurr,d.DeciAmt,d.DecSep,d.ThousSep
	from
		ALL_JE a
		inner join :tblACCOUNT c on a.Account = c."AcctCode"
		left join #tblCOMP d on 1=1
		left join :tblDOC e on e."ObjID"=a.TransType
		left join AZ_ODOC doc on doc."ObjType"=a.TransType and doc."DocEntry"=a.DocID
	where
		a.RefDate between :fdate and :tdate
		and (a."BPLID"=branch or branch=0);
	
	update a set JEMemo = b."LineMemo" from #AC a, JDT1 b where a.JEID = b."TransId" and a.JELine = b."Line_ID"; 
	
	update #AC set Balance = (select SUM(a.Debit-a.Credit) from #AC a where a.Account = #AC.Account and a.ID <= #AC.ID);
	
	insert into #AC (Account,AcctName,AcctFName, JEMemo, DocMemo, Balance, Period,
		CompName,CompAdd,RptCurrcy,DeciAmt,DecSep,ThousSep)
	select
		a.Account,AcctName,AcctFName,
		N'Số dư cuối kỳ / Closing balance',
		N'Số dư cuối kỳ / Closing balance',
		SUM(a.Debit-a.Credit),
		3,
		CompName,CompAdd,RptCurrcy,DeciAmt,DecSep,ThousSep
	from #AC a
	group by a.Account,AcctName,AcctFName,CompName,CompAdd,RptCurrcy,DeciAmt,DecSep,ThousSep;
	
	list_AC =
		select a.Account
		from #AC a
		where a.Period = 3;
	
	insert into #AC (ID, Account,AcctName,AcctFName, JEMemo, DocMemo, Debit, Credit, Period,
		CompName,CompAdd,RptCurrcy,DeciAmt,DecSep,ThousSep)
	select
		0,
		a.Account,AcctName,AcctFName,
		N'Số dư đầu kỳ / Opening balance',
		N'Số dư đầu kỳ / Opening balance',
		0,
		0,
		0,
		CompName,CompAdd,RptCurrcy,DeciAmt,DecSep,ThousSep
	from
		:list_AC a
		left join #AC b on b.Period = 0 and a.Account = b.Account
	where b.Account is null;
	
	insert into #AC (Account,AcctName,AcctFName, JEMemo, DocMemo, Debit, Credit, Period,
		CompName,CompAdd,RptCurrcy,DeciAmt,DecSep,ThousSep)
	select
		a.Account,AcctName,AcctFName,
		N'Cộng số phát sinh trong kỳ / During the period',
		N'Cộng số phát sinh trong kỳ / During the period',
		IFNULL(SUM(b.Debit),0),
		IFNULL(SUM(b.Credit),0),
		2,
		CompName,CompAdd,RptCurrcy,DeciAmt,DecSep,ThousSep
	from
		:list_AC a
		left join #AC b on b.Period = 1 and a.Account = b.Account
	group by a.Account,AcctName,AcctFName,CompName,CompAdd,RptCurrcy,DeciAmt,DecSep,ThousSep;
	
	update #AC set BalDeb = CASE when Balance >= 0 then Balance end;
	
	update #AC set BalCred = CASE when Balance < 0 then -Balance end;
	

	
	tab_AC = select * from #AC;
	
	drop table #tblCOMP;
	drop table #AC;
END

