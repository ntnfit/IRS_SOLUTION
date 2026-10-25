CREATE PROCEDURE VAS_CN01_DN_FC (
	in fdate		timestamp,
	in tdate		timestamp,
	in list_BPs		table (Code nvarchar(30)),
	in list_accts	table (Code nvarchar(30)),
	in curr			nvarchar(10),
	in currTyp		nvarchar(1),
	in branch		int,
	out tab_BPBal	table (
		ID				int
		,GrpCode		nvarchar(20)
		,GrpName		nvarchar(300)
		,BPCode			nvarchar(20)
		,BPName			nvarchar(300)
		,FCCurrency		nvarchar(100)
		
		,OBFC			numeric(28,10)
		,OBDebFC		numeric(28,10)
		,OBCredFC		numeric(28,10)
		,OB				numeric(28,10)
		,OBDeb			numeric(28,10)
		,OBCred			numeric(28,10)
		
		,DebFC			numeric(28,10)
		,CredFC			numeric(28,10)
		,Deb			numeric(28,10)
		,Cred			numeric(28,10)

		,EBFC			numeric(28,10)
		,EBDebFC		numeric(28,10)
		,EBCredFC		numeric(28,10)
		,EB				numeric(28,10)
		,EBDeb			numeric(28,10)
		,EBCred			numeric(28,10)
		
		,CompName		nvarchar(100)
		,CompAdd		nvarchar(300)
		
		,RptCurrcy	nvarchar(20),
		DeciAmt		int,
		DecSep		nvarchar(1),
		ThousSep	nvarchar(1)
	)
)
AS
BEGIN
	create local temporary table #BPBal (
		ID				int
		,GrpCode		nvarchar(20)
		,GrpName		nvarchar(300)
		,BPCode			nvarchar(20)
		,BPName			nvarchar(300)
		,FCCurrency		nvarchar(100)
		
		,OBFC			numeric(28,10)
		,OBDebFC		numeric(28,10)
		,OBCredFC		numeric(28,10)
		,OB				numeric(28,10)
		,OBDeb			numeric(28,10)
		,OBCred			numeric(28,10)
		
		,DebFC			numeric(28,10)
		,CredFC			numeric(28,10)
		,Deb			numeric(28,10)
		,Cred			numeric(28,10)

		,EBFC			numeric(28,10)
		,EBDebFC		numeric(28,10)
		,EBCredFC		numeric(28,10)
		,EB				numeric(28,10)
		,EBDeb			numeric(28,10)
		,EBCred			numeric(28,10)
		
		,CompName		nvarchar(100)
		,CompAdd		nvarchar(300)
		
		,RptCurrcy	nvarchar(20),
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
		Credit		numeric(21,6),
		FCDebit		numeric(21,6),
		FCCredit	numeric(21,6),
		FCCurrency	nvarchar(10)
	);
	
	insert into #JE (TransId, LineId, RefDate, ShortName, Account, Debit, Credit,FCDebit,FCCredit,FCCurrency)
	select
		a."TransId",
		a."Line_ID",
		a."RefDate",
		a."ShortName",
		a."Account",
		CASE when :currTyp = 'S' then IFNULL(a."SYSDeb",0) else IFNULL(a."Debit",0) end,
		CASE when :currTyp = 'S' then IFNULL(a."SYSCred",0) else IFNULL(a."Credit",0) end
		,a."FCDebit",a."FCCredit"
		,case when ifnull(a."FCCurrency",'')='' then 'VND' else a."FCCurrency" end FCCurrency
	from JDT1 a, :list_BPs b, :list_accts c
	where
		a."ShortName" = b.Code
		and a."Account" = c.Code
		and a."ShortName"<>a."Account"
		and (a."BPLId"=branch or branch=0)
		and (a."RefDate" <= :tdate)
		and (:curr='' or :curr='*' or case when ifnull(a."FCCurrency",'')='' then 'VND' else a."FCCurrency" end=:curr)
		;
	--=======================================================================================
	insert into #BPBal (BPCode,FCCurrency,EBFC,EB)
	select ShortName,FCCurrency,sum(FCDebit-FCCredit),sum(Debit-Credit)
	from #JE group by ShortName,FCCurrency;
	
	update #BPBal set DebFC = (select sum(FCDebit) from #JE x where x.ShortName=#BPBal.BPCode and x.FCCurrency=#BPBal.FCCurrency and x.RefDate between fdate and tdate);
	update #BPBal set CredFC = (select sum(FCCredit) from #JE x where x.ShortName=#BPBal.BPCode and x.FCCurrency=#BPBal.FCCurrency and x.RefDate between fdate and tdate);
	update #BPBal set Deb = (select sum(Debit) from #JE x where x.ShortName=#BPBal.BPCode and x.FCCurrency=#BPBal.FCCurrency  and x.RefDate between fdate and tdate);
	update #BPBal set Cred = (select sum(Credit) from #JE x where x.ShortName=#BPBal.BPCode and x.FCCurrency=#BPBal.FCCurrency  and x.RefDate between fdate and tdate);
	
	update #BPBal set OBFC = ifnull(EBFC,0) + ifnull(CredFC,0) - ifnull(DebFC,0);
	update #BPBal set OB = ifnull(EB,0) + ifnull(Cred,0) - ifnull(Deb,0);
	
	update #BPBal set OBDebFC = OBFC, OBCredFC = 0 where OBFC>0;
	update #BPBal set OBDebFC = 0, OBCredFC = - OBFC where OBFC<0;
	update #BPBal set EBDebFC = EBFC, EBCredFC = 0 where EBFC>0;
	update #BPBal set EBDebFC = 0, EBCredFC = - EBFC where EBFC<0;
	
	update #BPBal set OBDeb = OB, OBCred = 0 where OB>0;
	update #BPBal set OBDeb = 0, OBCred = - OB where OB<0;
	update #BPBal set EBDeb = EB, EBCred = 0 where EB>0;
	update #BPBal set EBDeb = 0, EBCred = - EB where EB<0;
	--========================================================================================	
	update a set BPName = b."CardName" from #BPBal a, OCRD b where a.BPCode = b."CardCode";
	
	update a set
		CompName	= b."CompnyName",
		CompAdd		= b."CompnyAddr",
		RptCurrcy	= case when currTyp='L' then b."MainCurncy" else b."SysCurrncy" end
	from #BPBal a, OADM b;
	
	
	tab_BPBal = select * from #BPBal;
	
	drop table #BPBal;
	drop table #JE;
END

