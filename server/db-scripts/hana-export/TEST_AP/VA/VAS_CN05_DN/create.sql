CREATE PROCEDURE VAS_CN05_DN (
	in	username		nvarchar(100),
	in 	branch			int,
	in 	tdate			timestamp,
	in 	BPCode			table (Code nvarchar(30)),
	in 	AcctCode		table (Code nvarchar(30)),
	in 	p1				int,
	in 	p2				int,
	in 	p3				int,
	in 	p4				int,
	out out_Tb 			table (
		CompName 		nvarchar(100), 
		CompAddr 		nvarchar(300), 
		RptName 		nvarchar(100), 
		Curr			nvarchar(10),
		DocType 		nvarchar(50), 
		DocID 			int, 
		JEID 			int, 
		JE_LineID 		int, 
		DocNo 			nvarchar(30), 
		Note 			nvarchar(300),
		BPCode 			nvarchar(50), 
		BPName 			nvarchar(100), 
		ActCode 		nvarchar(50), 
		ActName 		nvarchar(100), 
		RefDate 		timestamp, 
		DueDate 		timestamp, 
		OverDay 		int,
		OrgDebit 		numeric(21,6), 
		OrgCredit 		numeric(21,6), 
		OpenDebit 		numeric(21,6), 
		OpenCredit 		numeric(21,6), 
		P1 				numeric(21,6), 
		P2 				numeric(21,6), 
		P3 				numeric(21,6), 
		P4 				numeric(21,6), 
		P5 				numeric(21,6), 
		Url 			nvarchar(100),
		SumDec			int,
		DecSep			varchar(1),
		ThousSep		varchar(1),
		Logo			nvarchar(100),
		DateTitle		nvarchar(100)
	)
)
AS
BEGIN
	
	list_JE = 
		select a."TransType",a."CreatedBy",a."TransId",a."Line_ID",a."ShortName",a."Account",a."RefDate",a."DueDate",a."LineMemo",IFNULL(a."Debit",0) Debit,IFNULL(a."Credit",0) Credit, a."BalDueDeb",a."BalDueCred"
		from 
			JDT1 a
			inner join :BPCode b on a."ShortName" = b.Code
			inner join :AcctCode c on a."Account" = c.Code
		where
			a."RefDate" <= :tdate
			and (IFNULL(a."BPLId",0)=:branch or :branch=0);
	
	list_Reconcile =
		select a."ReconDate",b."TransId",b."TransRowId",b."ReconSum"
		from 
			OITR a
			inner join ITR1 b on a."ReconNum" = b."ReconNum"
			inner join :BPCode d on d.Code = b."ShortName"
			inner join :AcctCode e on e.Code = b."Account"
		where
			a."ReconDate" <= :tdate
			and IFNULL(a."BPLId",0)=:branch;
	
	create local temporary table #tbl as (
		select * from :out_Tb
	);
	-- Du lieu phat sinh
	insert into #tbl (DocType,DocID,DocNo,JEID,JE_LineID,BPCode,BPName,ActCode,ActName,RefDate,DueDate,Note,OrgDebit,OrgCredit, OpenDebit, OpenCredit)
	select "TransType","CreatedBy",'JE ' || cast(a."TransId" as nvarchar),"TransId","Line_ID","ShortName",b."CardName","Account",c."AcctName","RefDate","DueDate","LineMemo",Debit,Credit,case when "BalDueDeb"-"BalDueCred">0 then "BalDueDeb"-"BalDueCred" else 0 end,case when "BalDueDeb"-"BalDueCred">0 then 0 else -"BalDueDeb"+"BalDueCred" end
	from 
		:list_JE a
		inner join OCRD b on a."ShortName" = b."CardCode"
		inner join OACT c on c."AcctCode" = a."Account";
	
	-- Cap nhat thanh toan
--	update #tbl set OpenDebit = IFNULL(OrgDebit,0) - IFNULL((select sum(x."ReconSum") from :list_Reconcile x where x."TransId" = #tbl.JEID and x."TransRowId" = #tbl.JE_LineID),0) where OrgDebit <> 0;
--	update #tbl set OpenCredit = IFNULL(OrgCredit,0) - IFNULL((select sum(x."ReconSum") from :list_Reconcile x where x."TransId" = #tbl.JEID and x."TransRowId" = #tbl.JE_LineID),0) where OrgCredit <> 0;
	delete from #tbl where IFNULL(OpenDebit,0) = 0 and IFNULL(OpenCredit,0) = 0;
	
	update a set OverDay = DAYS_BETWEEN(DueDate,:tdate) from #tbl a;
	update #tbl set P1 = IFNULL(OpenDebit,0) - IFNULL(OpenCredit,0) where OverDay <= :p1 and OverDay >= 0;
	update #tbl set P2 = IFNULL(OpenDebit,0) - IFNULL(OpenCredit,0) where OverDay <= :p2 and OverDay > :p1;
	update #tbl set P3 = IFNULL(OpenDebit,0) - IFNULL(OpenCredit,0) where OverDay <= :p3 and OverDay > :p2;
	update #tbl set P4 = IFNULL(OpenDebit,0) - IFNULL(OpenCredit,0) where OverDay <= :p4 and OverDay > :p3;
	update #tbl set P5 = IFNULL(OpenDebit,0) - IFNULL(OpenCredit,0) where OverDay > :p4;
	
	update a set Url = 'http://$b1$/link?table=OJDT&key=' || CAST(JEID as varchar(10)) from #tbl a;
	
	update c set CompName = a."PrintHeadr", CompAddr = b."Street" from OADM a,ADM1 b,#tbl c where :branch in ('-1','0');
	update b set CompName = a."BPLName", CompAddr = a."Street" from OBPL a,#tbl b where a."BPLId" = :branch;
	
	update b set Curr = a."MainCurncy", SumDec = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep", RptName = N'BÁO CÁO PHÂN TÍCH TUỔI NỢ', DateTitle = N'Ngày........tháng........năm........ ' || char(13) || char(10) || N'Prepared on the date................' from OADM a,#tbl b;
	update b set Logo = a."BitmapPath" || IFNULL(a."LogoFile",'') from OADP a,#tbl b;
	
	update b set SumDec = case when a."Decimals"=-1 then b.SumDec else a."Decimals" end from OCRN a,#tbl b where a."CurrCode" = b.Curr;
	
	out_Tb = select * from #tbl;
	
	drop table #tbl;
END

