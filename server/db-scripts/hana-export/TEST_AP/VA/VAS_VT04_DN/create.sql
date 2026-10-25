CREATE PROCEDURE VAS_VT04_DN (
	in username		nvarchar(30),
	in branch		int,
	in tdate		timestamp,
	out out_Tb 		table (
		TransId			int,
		LineId			int,
		DocId			nvarchar(30),
		TransType		int,
		CreatedBy		int,
		RefDate			timestamp,
		ActCode			nvarchar(100),
		ActName			nvarchar(500),
		CardCode		nvarchar(50),
		LineMemo		nvarchar(500),
		Balance			numeric(21,6),
		URL				nvarchar(100),
		ItemCode		nvarchar(30),
		Quantity		numeric(21,6),
		Price			numeric(21,6),
		LineTotal		numeric(21,6),
		Total			numeric(21,6),
		Curr			nvarchar(30),
		QtyDec			int,
		PriceDec		int,
		SumDec			int,
		DecSep			nvarchar(1),
		ThousSep		nvarchar(1),
		Logo			nvarchar(50),
		DateTitle		nvarchar(200),
		CompName		nvarchar(300),
		CompAdd			nvarchar(500)
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
	
	insert into #tbl (TransId,LineId,TransType,CreatedBy,RefDate,CardCode,LineMemo,Balance,URL,ItemCode,Quantity,Price,LineTotal,ActCode)
	select
		tmp1."TransId"
		,tmp1."Line_ID"
		,tmp1."TransType"
		,tmp1."CreatedBy"
		,tmp1."RefDate"
		,tmp2."CardCode"
		,tmp1."LineMemo"
		,tmp1.Balance
		,tmp1.URL
		,tmp2."ItemCode"
		,tmp2.Quantity
		,tmp2.Price
		,tmp2.LineTotal
		,tmp1."Account"
	from
		(select
			a."TransId"
			,a."Line_ID"
			,a."TransType"
			,a."CreatedBy"
			,a."RefDate"
			,a."LineMemo"
			,(a."Debit"-a."Credit")-IFNULL(SUM(case b."IsCredit" when 'D' then b."ReconSum" else -b."ReconSum" end),0) as Balance
			,'http://$b1$/link?table=OJDT&key=' || CAST(a."TransId" as nvarchar) as URL
			,a."Account"
		from
			JDT1 a
			left join OJDT a0 on a."TransId" = a0."TransId"
			left join ITR1 b on a."TransId" = b."TransId" and a."Line_ID" = b."TransRowId"
								and exists(select 1 from OITR x where x."ReconNum" = b."ReconNum" and x."ReconDate" <= :tdate)
		where
			a."Account" like '151%'
			and a."RefDate" <= :tdate and (exists(select 1 from :list_branch x where x.BPLId=cast(IFNULL(a."BPLId",0) as nvarchar)))--(a."BPLId" = :branch or :branch in ('-1','0'))
		group by
			a."TransId"
			,a."Line_ID"
			,a."TransType"
			,a."CreatedBy"
			,a."RefDate"
			,a."LineMemo"
			,(a."Debit"-a."Credit")
			,a."Account"
		having
			(a."Debit"-a."Credit")<>IFNULL(SUM(case b."IsCredit" when 'D' then b."ReconSum" else -b."ReconSum" end),0)) tmp1
		left join
			(select
				t1."ObjType"
				,t1."DocEntry"
				,t2."LineNum"
				,t1."CardCode"
				,t2."ItemCode"
				,t2."Quantity"-IFNULL(SUM(t3."Quantity"),0) as Quantity
				,t2."Price" * (CASE IFNULL(NULLIF(LTRIM(t2."Currency"),''),'VND') when 'VND' then 1 else t2."Rate" end) as Price
				,t2."LineTotal"-IFNULL(SUM(t4."TransValue"),0) as LineTotal
			from
				OPCH t1
				left join PCH1 t2 on t1."DocEntry" = t2."DocEntry"
				left join PDN1 t3 on t3."BaseType" = t2."ObjType" and t3."BaseEntry" = t2."DocEntry" and t3."BaseLine" = t2."LineNum"
									and exists(select 1 from OPDN x where x."DocEntry" = t3."DocEntry" and x."DocDate" <= :tdate and "CANCELED" = 'N')
				left join OINM t4 on t4."TransType" = t3."ObjType" and t4."CreatedBy" = t3."DocEntry" and t4."DocLineNum" = t3."LineNum"
			where
				t1."CANCELED" = 'N'
				and t1."DocDate" <= :tdate
				and t1."isIns" = 'Y' and (exists(select 1 from :list_branch x where x.BPLId=cast(IFNULL(t1."BPLId",0) as nvarchar)))--(t1."BPLId" = :branch or :branch in ('-1','0'))
			group by
				t1."ObjType"
				,t1."DocEntry"
				,t2."LineNum"
				,t1."CardCode"
				,t2."ItemCode"
				,t2."Quantity"
				,t2."Price"
				,t2."Currency"
				,t2."Rate"
				,t2."LineTotal"
			having (t2."Quantity"<>IFNULL(SUM(t3."Quantity"),0)) or (t2."LineTotal"<>IFNULL(SUM(t3."LineTotal"),0))) tmp2 on tmp1."TransType" = tmp2."ObjType" and tmp1."CreatedBy" = tmp2."DocEntry";
	
	delete from #tbl where exists(select 1 from OPCH x where x."ObjType"=#tbl.TransType and x."DocEntry"=#tbl.CreatedBy and IFNULL(x."CANCELED",'N')<>'N');
	delete from #tbl where exists(select 1 from OPDN x where x."ObjType"=#tbl.TransType and x."DocEntry"=#tbl.CreatedBy and IFNULL(x."CANCELED",'N')<>'N');
	
	update #tbl set CardCode = case TransType 
									when '18' then (select x."CardCode" from OPCH x where x."DocEntry"=#tbl.CreatedBy)
									when '19' then (select x."CardCode" from ORPC x where x."DocEntry"=#tbl.CreatedBy)
									when '24' then (select x."CardCode" from ORCT x where x."DocEntry"=#tbl.CreatedBy and x."DocType"<>'A')
									when '46' then (select x."CardCode" from OVPM x where x."DocEntry"=#tbl.CreatedBy and x."DocType"<>'A')
									when '69' then (select x."CardCode" from OIPF x where x."DocEntry"=#tbl.CreatedBy)
									when '204' then (select x."CardCode" from ODPO x where x."DocEntry"=#tbl.CreatedBy) else CardCode end;
									
	update a set DocId = cast(TransId as nvarchar) || '/' || cast(LineId as nvarchar) from #tbl a;								

	update #tbl set Total = (select SUM(a.Balance) from (select ROW_NUMBER() OVER (PARTITION BY DocId ORDER BY ItemCode) as RN, Balance from #tbl) a where a.RN = 1);
									
	update b set ActName = a."AcctName" || + IFNULL('/ ' || a."FrgnName",'') from OACT a,#tbl b where a."AcctCode"=b.ActCode;
	
	
	update c set CompName = a."PrintHeadr", CompAdd = b."Street" from OADM a,ADM1 b,#tbl c where :branch in ('-1','0');
	update b set CompName = a."BPLName", CompAdd = a."Street" from OBPL a,#tbl b where a."BPLId" = :branch;
	
	update b set Curr = a."MainCurncy", QtyDec = a."QtyDec", PriceDec = a."PriceDec", SumDec = a."SumDec", DecSep = a."DecSep", ThousSep = a."ThousSep", DateTitle = N'NgÃ y........thÃ¡ng........nÄƒm........ ' || char(13) || char(10) || N'Prepared on the date................' from OADM a,#tbl b;
	update b set Logo = a."BitmapPath" || IFNULL(a."LogoFile",'') from OADP a,#tbl b;
	
	update b set SumDec = case when a."Decimals"=-1 then b.SumDec else a."Decimals" end 
				 ,PriceDec = case when a."Decimals"=-1 then b.PriceDec else a."Decimals" end from OCRN a,#tbl b where a."CurrCode"=b.Curr;

	out_Tb = select * from #tbl;



	drop table #tbl;
END

