CREATE PROCEDURE VAS_VT06_DN (
	in username		nvarchar(30),
	in branch		int,
	in fdate		timestamp,
	in tdate		timestamp,
	out out_Tb 		table (
		RefNo				nvarchar(30),
		RefDate				timestamp,
		RefType				nvarchar(30),
		InvNo				nvarchar(30),
		InvDate				timestamp,
		ItemCode			nvarchar(30),
		ItemDescr			nvarchar(300),
		ItemUom				nvarchar(30),
		Quantity			numeric(21,6),
		PriceFC				numeric(21,6),
		LineTotalFC			numeric(21,6),
		InvRate				numeric(21,6),
		PriceLC				numeric(21,6),
		LineTotalLC			numeric(21,6),
		ImportTax			numeric(21,6),
		OtherTax			numeric(21,6),
		Orgn				nvarchar(50),
		DtoDServ			numeric(21,6),
		LineTotalInclTax	numeric(21,6),
		LineTotalBefVAT		numeric(21,6),
		Note				nvarchar(30),
		AcctCode			nvarchar(30),
		DocCur				nvarchar(30),
		Curr				nvarchar(30),
		CardCode			nvarchar(30),
		CardName			nvarchar(200),
		DocEntry			int,
		PrdId				int,
		PrdDate				timestamp,
		OriDocTyp			int,
		OriAbsEnt			int,
		OriLinNum			int,
		CompnyName			nvarchar(300),
		CompnyAddr			nvarchar(300),
		QtyDec				int,
		PriceDec			int,
		SumDecLC			int,
		SumDecFC			int,
		RateDec				int,
		DecSep				nvarchar(1),
		ThousSep			nvarchar(1),
		Logo				nvarchar(100),
		DateTitle			nvarchar(200)
	)
)
AS
BEGIN
	
	declare usercode nvarchar(20) ;
	select x."USER_CODE" into usercode from OADM x0 left join OUSR x on x."U_NAME" = :username;
	
	list_branch = 
			select IFNULL(a."BPLId",0) BPLId
			from 
				OADM a0
				left join OBPL a on 1=1
				left join USR6 b on b."UserCode" = :usercode and a."BPLId" = b."BPLId"
			where
				:branch in (0,a."BPLId");
	
	--select * from :list_branch;
	
	create local temporary table #tbl as (
		select * from :out_Tb
	);
	
	insert into #tbl (RefNo,RefDate,RefType,InvNo,InvDate,ItemCode,ItemDescr,ItemUom,Quantity,PriceFC,LineTotalFC,InvRate,PriceLC,LineTotalLC,ImportTax,OtherTax,Orgn,DtoDServ,DocCur,CardCode,DocEntry,PrdId,PrdDate,OriDocTyp,OriAbsEnt,OriLinNum)
	select
		a0."Ref1"
		,a0."DocDate"
		,a0."DocNum"
		,IFNULL(b."U_InvNo",(select z."U_InvNo" from PDN1 x, PCH1 y, OPCH z where x."BaseType" = y."ObjType" and x."BaseEntry" = y."DocEntry" and x."BaseLine" = y."LineNum" and y."DocEntry" = z."DocEntry" and x."ObjType" = a."OriBDocTyp" and x."DocEntry" = a."OriBAbsEnt" and x."LineNum" = a."OriBLinNum"))
		,IFNULL(b."U_InvDate",(select z."U_InvDate" from PDN1 x, PCH1 y, OPCH z where x."BaseType" = y."ObjType" and x."BaseEntry" = y."DocEntry" and x."BaseLine" = y."LineNum" and y."DocEntry" = z."DocEntry" and x."ObjType" = a."OriBDocTyp" and x."DocEntry" = a."OriBAbsEnt" and x."LineNum" = a."OriBLinNum"))
		,a."ItemCode"
		,c."ItemCode" || ' ' || c."ItemName"
		,c."InvntryUom"
		,a."Quantity"
		,a."PriceFOB"
		,a."FobValueFC"
		,a."Rate"
		,a."PriceFOB"*a."Rate"
		,a."FobValue"
		,IFNULL(a."TtlCustLC",0)
		,0
		,''
		,IFNULL(a."TtlCostLC",0)-IFNULL(a."TtlCustLC",0)
		,a."Currency"
		,a."CardCode"
		,a."DocEntry"
		,a00."AbsEntry"
		,a0."DocDate"
		,a."OriBDocTyp"
		,a."OriBAbsEnt"
		,a."OriBLinNum"
	from
		IPF1 a
		inner join OIPF a0 on a."DocEntry" = a0."DocEntry"
		left join OFPR a00 on a0."DocDate" between a00."F_RefDate" and a00."T_RefDate"
		left join OPCH b on
			(b."CANCELED" = 'N')
			and ((b."ObjType" = a."OriBDocTyp" and b."DocEntry" = a."OriBAbsEnt")
				or exists(select 1 from PDN1 x, PCH1 y where x."ObjType" = y."BaseType" and x."DocEntry" = y."BaseEntry" and x."LineNum" = y."BaseLine" and x."ObjType" = a."OriBDocTyp" and x."DocEntry" = a."OriBAbsEnt" and x."LineNum" = a."OriBLinNum" and y."DocEntry" = b."DocEntry"))
		left join OITM c on a."ItemCode" = c."ItemCode"
	where
		a."OriBDocTyp" in ('18','20')
		and not exists(select 1 from IPF1 x where x."BaseType" = '69' and x."BaseEntry" = a."DocEntry" and x."OrigLine" = a."LineNum")
		and a0."DocDate" between :fdate and :tdate
		and a."WhsCode" in (select x."WhsCode" from OWHS x where exists(select 1 from :list_branch t where t.BPLId=IFNULL(x."BPLid",0))/*(x.BplID = @branch or @branch in ('-1','0'))*/);
	
	update a set LineTotalInclTax = LineTotalLC+IFNULL(ImportTax,0)+IFNULL(DtoDServ,0) 
				 ,LineTotalBefVAT = LineTotalLC+IFNULL(ImportTax,0)+IFNULL(OtherTax,0) from #tbl a;
	update b set CardName = a."CardName" from OCRD a,#tbl b where a."CardCode"=b.CardCode;
	
	update #tbl set AcctCode = (select distinct b."InvntAct" from PDN1 a inner join OINM b on a."ObjType" = b."TransType" and a."DocEntry" = b."CreatedBy" and a."LineNum" = b."DocLineNum" where a."ObjType" = #tbl.OriDocTyp and a."DocEntry" = #tbl.OriAbsEnt and a."LineNum" = #tbl.OriLinNum) where OriDocTyp = '20';
	update #tbl set AcctCode = (select distinct c."InvntAct" from PCH1 a inner join PDN1 b on a."BaseType" = b."ObjType" and a."BaseEntry" = b."DocEntry" and a."BaseLine" = b."LineNum" inner join OINM c on c."TransType" = b."ObjType" and c."CreatedBy" = b."DocEntry" and c."DocLineNum" = b."LineNum" where a."ObjType" = #tbl.OriDocTyp and a."DocEntry" = #tbl.OriAbsEnt and a."LineNum" = #tbl.OriLinNum) where OriDocTyp = '18';
	update #tbl set AcctCode = (select distinct b."InvntAct" from PCH1 a inner join OINM b on a."ObjType" = b."TransType" and a."DocEntry" = b."CreatedBy" and a."LineNum" = b."DocLineNum" where a."ObjType" = #tbl.OriDocTyp and a."DocEntry" = #tbl.OriAbsEnt and a."LineNum" = #tbl.OriLinNum) where OriDocTyp = '18' and IFNULL(AcctCode,'') = '';
	
	
	update c set CompnyName = a."PrintHeadr", CompnyAddr = b."Street" from OADM a,ADM1 b,#tbl c where :branch in ('-1','0');
	update b set CompnyName = a."BPLName", CompnyAddr = a."Street" from OBPL a,#tbl b where a."BPLId" = :branch;
	
	update b set Curr = a."MainCurncy", QtyDec = a."QtyDec", PriceDec = a."PriceDec", RateDec = a."RateDec", SumDecLC = a."SumDec", SumDecFC = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep", DateTitle = N'Ngày........tháng........năm........ ' || char(13) || char(10) || N'Prepared on the date................' from OADM a,#tbl b;
	update b set Logo = a."BitmapPath" || IFNULL(a."LogoFile",'') from OADP a,#tbl b;
	
	update b set SumDecLC = case when a."Decimals"=-1 then b.SumDecLC else a."Decimals" end from OCRN a,#tbl b where a."CurrCode"=b.Curr;
	update b set SumDecFC = case when a."Decimals"=-1 then b.SumDecFC else a."Decimals" end from OCRN a,#tbl b where a."CurrCode"=b.DocCur;

	out_Tb = select * from #tbl;

	drop table #tbl;
END

