create PROCEDURE VAS_NN08_DN (
	in branch		int,
	in period		nvarchar(30),
	in typeCur		nvarchar(2),
	out tab_TB table (
		CompName		nvarchar(100)
		,CompAdd		nvarchar(300)
		,LineNum		int
		,LineName		nvarchar(100)
		,AbsEntry		int
		,CardCode		nvarchar(30)
		,CardName		nvarchar(100)
		,SPN			numeric(21,6)
		,SDN			numeric(21,6)
		,SCN			numeric(21,6)
		,TongSCN		numeric(21,6)
		,ID_ThanhToan	nvarchar(100)
		,SoChungTu		nvarchar(200)
		,NgayThanhToan	nvarchar(200)
		,AcctCode		nvarchar(100)
		,AcctName		nvarchar(500)
		,Curr			nvarchar(30)
		,DeciQty		int
		,DeciPrice		int
		,DeciAmt		int
		,DateSep		nvarchar(1)
		,ThousSep		nvarchar(1)
		,Logo			nvarchar(300)
		,DateTitle		nvarchar(200)
		,RptName		nvarchar(500)
	)
)
AS
BEGIN
	---------------------------------------------
	declare prdyear int;
	declare prdpst nvarchar(30);
	declare prdmon smallint;
	declare prdmin int;
	declare prdmax int;
	declare X integer := 1;
	---------------------------------------------
	create local temporary table #TMP1 (
		CompName		nvarchar(100)
		,CompAdd		nvarchar(300)
		,LineNum		int
		,LineName		nvarchar(100)
		,AbsEntry		int
		,CardCode		nvarchar(30)
		,CardName		nvarchar(100)
		,SPN			numeric(21,6)
		,SDN			numeric(21,6)
		,SCN			numeric(21,6)
		,TongSCN		numeric(21,6)
		,ID_ThanhToan	nvarchar(100)
		,SoChungTu		nvarchar(200)
		,NgayThanhToan	nvarchar(200)
		,AcctCode		nvarchar(100)
		,AcctName		nvarchar(500)
		,Curr			nvarchar(30)
		,DeciQty		int
		,DeciPrice		int
		,DeciAmt		int
		,DateSep		nvarchar(1)
		,ThousSep		nvarchar(1)
		,Logo			nvarchar(300)
		,DateTitle		nvarchar(200)
		,RptName		nvarchar(500)
	);
	------------------------------------------------
	select  YEAR("F_RefDate"), MONTH("F_RefDate"), "AbsEntry" into prdyear,prdmon,prdmax from "OFPR" where "Code" = period;
	select MIN("AbsEntry") into prdmin from "OFPR" where YEAR("F_RefDate") = prdyear;
	prdpst = CAST(prdyear-1 as nvarchar(30));
	
	
	create local temporary table #TBL_MONTH (number int);
	
	while X <= prdmon do
		insert into #TBL_MONTH (number) values (X);
		X := :X + 1;
	end while;
	
	------------------------------------------------
	create local temporary table #TBL_DATA (
		TransId		int
		,LineId		int
		,RefNo		nvarchar(30)
		,RefDate	timestamp
		,FinncPriod	int
		,Account	nvarchar(30)
		,ContraAct	nvarchar(30)
		,Debit		numeric(21,6)
		,Credit		numeric(21,6)
		,Project	nvarchar(30)
	);
	insert into #TBL_DATA (TransId,LineId,RefNo,RefDate,FinncPriod,Account,ContraAct,Debit,Credit,Project)
	select 
		a.TransId
		,a.LineId
		,IFNULL(b."Ref2",'')
		,a.RefDate
		,b."FinncPriod"
		,a.Account
		,a.ContraAct
		,case when typeCur='LC' then a.Debit else a.DebitSC end
		,case when typeCur='LC' then a.Credit else a.CreditSC end
		,b."Project"
	from 
		ALL_JE a, "JDT1" b
	where a.TransId = b."TransId" and a.LineId = b."Line_ID" and (a.Account like '33383%') and b."FinncPriod" <= prdmax and (b."BPLId" = branch or branch in (-1,0));
	---------------------------------------
	insert into #TMP1 (LineNum,LineName,AbsEntry,CardCode,SPN,SDN)
	select
		c.number
		,CASE c.number when 0 then CONCAT(CONCAT(N'Số năm ',prdpst),N' chuyển sang') else CAST(c.number as nvarchar(2)) end
		,d."AbsEntry"
		,IFNULL(e.Project,'')
		,SUM(CASE f."Finanse" when 'N' then -IFNULL(e.Debit,0)+IFNULL(e.Credit,0) else 0 end)
		,SUM(CASE f."Finanse" when 'Y' then IFNULL(e.Debit,0)-IFNULL(e.Credit,0) else 0 end)
	from
		"OADM" a
		cross join "ADM1" b
		inner join #TBL_MONTH c on c.number between 0 and prdmon
		left join "OFPR" d on YEAR(d."F_RefDate") = prdyear and MONTH(d."F_RefDate") = c.number
		left join #TBL_DATA e on (CASE when e.FinncPriod < prdmin then 0 else MONTH(e.RefDate) end) = c.number
		left join "OACT" f on e.ContraAct = f."AcctCode"
	group by
		c.number
		,CASE c.number when 0 then CONCAT(CONCAT(N'Số năm ',prdpst),N' chuyển sang') else CAST(c.number as nvarchar(2)) end
		,d."AbsEntry"
		,IFNULL(e.Project,'')
	having not (c.number = 0 and SUM(CASE f."Finanse" when 'N' then -IFNULL(e.Debit,0)+IFNULL(e.Credit,0) else 0 end) = SUM(CASE f."Finanse" when 'Y' then IFNULL(e.Debit,0)-IFNULL(e.Credit,0) else 0 end));

	update #TMP1 set ID_ThanhToan = (select top 1 cast(a.TransId as nvarchar(10)) from #TBL_DATA a where YEAR(a.RefDate) = prdyear);--(select distinct CAST(a.TransId as nvarchar) + ',' from #TBL_DATA a where not exists(select 1 from "OJDT" where "StornoToTr" is not null and ("TransId" = a.TransId and "StornoToTr" = a.TransId)) and exists(select 1 from "OACT" where "Finanse" = 'Y' and "AcctCode" = a.ContraAct) and YEAR(a.RefDate) = prdyear for xml path('')) where LineId <> 1;
	update #TMP1 set SoChungTu = (select top 1 a.RefNo from #TBL_DATA a where YEAR(a.RefDate) = prdyear);--(select distinct ISNULL(a."Ref2" + ',','') from "OJDT" a where exists(select 1 from dbo.Split(ID_ThanhToan,',') where Data = a.TransId) for xml path(''));
	update #TMP1 set NgayThanhToan = (select top 1 TO_VARCHAR (a.RefDate,'dd/MM/yyyy') from #TBL_DATA a where YEAR(a.RefDate) = prdyear);--(select distinct ISNULL(CONVERT(nvarchar(10),a."RefDate",103) + ',','') from "OJDT" a where exists(select 1 from dbo.Split(ID_ThanhToan,',') where Data = a.TransId) for xml path(''))	where LineID=2;

	update #TMP1 set SCN = (select SUM(IFNULL(a.SPN,0) - IFNULL(a.SDN,0)) from #TMP1 a where a.LineNum <= #TMP1.LineNum);

	update #TMP1 set TongSCN = IFNULL((select SUM(a.SCN) from #TMP1 a where a.LineNum = 0),0)
							+ IFNULL((select SUM(a.SPN) from #TMP1 a where a.LineNum between 1 and 12),0)
							- IFNULL((select SUM(a.SDN) from #TMP1 a where a.LineNum between 1 and 12),0);

	update #TMP1 set SPN = 0, SDN = 0 where LineNum = 0;

	update #TMP1 set CardName = (select a."CardName" from "OCRD" a where a."CardCode" = #TMP1.CardCode);

	
	update #TMP1 set AcctName = N'Thuế nhà thầu - Witholding Tax'
					,AcctCode = '33382';
	-- Cong ty theo chi nhanh
	update #TMP1 set CompName 	= (select a."PrintHeadr" from "OADM" a,"ADM1" b) where branch in (-1,0);
	update #TMP1 set CompAdd 	= (select b."Street" from "OADM" a,"ADM1" b) where branch in (-1,0);
	update #TMP1 set CompName	= (select a."BPLName" from "OBPL" a where a."BPLId" = branch) where branch not in (-1,0);
	update #TMP1 set CompAdd 	= (select a."Street" from "OBPL" a where a."BPLId" = branch) where branch not in (-1,0);
	update #TMP1 set Curr = (select case when typeCur='LC' then a."MainCurncy" else a."SysCurrncy" end from "OADM" a);
	
	update #TMP1 set DateTitle = CONCAT(CONCAT(CONCAT(N'Ngày........tháng........năm........ ',char(13)),char(10)),N'Prepared on the date................')
					,RptName = CONCAT(CONCAT(CONCAT(N'SỔ CHI TIẾT THUẾ NHÀ THẦU',char(13)),char(10)),'WITHHOLDING TAX');
	
	update #TMP1 set DeciQty 	= (select a."QtyDec" from "OADM" a);
	update #TMP1 set DeciPrice 	= (select a."PriceDec" from "OADM" a);
	update #TMP1 set DateSep 	= (select a."DecSep" from "OADM" a);
	update #TMP1 set ThousSep 	= (select a."ThousSep" from "OADM" a);
	
	update #TMP1 set DeciAmt = case when (select a."Decimals" from "OCRN" a where a."CurrCode" = #TMP1.Curr)='-1' then (select a."SumDec" from "OADM" a) else (select a."Decimals" from "OCRN" a where a."CurrCode" = #TMP1.Curr) end;
	update #TMP1 set Logo = (select CONCAT("BitmapPath","LogoFile") LogoImage from "OADP");
/*
	-- So chu so sau dau phay 
	
	update @tbl set Logo = LogoImage from OADP
	
	
	select * from @tbl		

*/
	tab_TB = select * from #TMP1 order by LineNum;
	drop table #TMP1;
	drop table #TBL_DATA;
	drop table #TBL_MONTH;
END

