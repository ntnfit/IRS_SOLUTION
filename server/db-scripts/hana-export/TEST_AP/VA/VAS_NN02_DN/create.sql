create PROCEDURE VAS_NN02_DN (
	in branch		int,
	in yearno		int,
	in typeCur		nvarchar(2),
	out tab_TB table (
		CompName		nvarchar(100)
		,CompAdd		nvarchar(300)
		,LineNum		smallint
		,LineName		nvarchar(50)
		,PrdID			int
		,ThueDKDKT		numeric(21,6)
		,ThueDKCPN		numeric(21,6)
		,ThueDauVao		numeric(21,6)
		,ThueDauRa		numeric(21,6)
		,ThuePSTK		numeric(21,6)
		,ThuePNTK		numeric(21,6)
		,ThueDKTTK		numeric(21,6)
		,SoDaNopTK		numeric(21,6)
        ,SoConPNCK		numeric(21,6)
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
	declare fdate, tdate timestamp;
	declare X integer := 0;
	select MIN("F_RefDate"),MAX("T_RefDate") into fdate,tdate from "OFPR" where year("F_RefDate")=yearno;
	---------------------------------------------
	create local temporary table #TMP1 (
		CompName		nvarchar(100)
		,CompAdd		nvarchar(300)
		,LineNum		smallint
		,LineName		nvarchar(50)
		,PrdID			int
		,ThueDKDKT		numeric(21,6)
		,ThueDKCPN		numeric(21,6)
		,ThueDauVao		numeric(21,6)
		,ThueDauRa		numeric(21,6)
		,ThuePSTK		numeric(21,6)
		,ThuePNTK		numeric(21,6)
		,ThueDKTTK		numeric(21,6)
		,SoDaNopTK		numeric(21,6)
        ,SoConPNCK		numeric(21,6)
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
	);
	insert into #TBL_DATA (RefDate,Account,ContraAct,Debit,Credit,FinncPriod)
	SELECT 
		a.RefDate
		,a.Account
		,a.ContraAct
		,CASE WHEN typeCur='LC' THEN IFNULL(a.Debit,0) ELSE IFNULL(a.DebitSC,0) END	Debit
		,CASE WHEN typeCur='LC' THEN IFNULL(a.Credit,0) ELSE IFNULL(a.CreditSC,0) END Credit
		,b."FinncPriod"
	FROM 
		ALL_JE a
		INNER JOIN "JDT1" b ON a.TransId=b."TransId" AND a.LineId=b."Line_ID"
	WHERE 
		a.RefDate <=tdate
		AND (b."BPLId" = branch or branch in (-1,0))
		AND (a.Account LIKE '133%' OR a.Account LIKE '3331%');
	---------------------------------------
	create local temporary table #TBL_MONTH (number int);
	while X <= MONTH(tdate) do
		insert into #TBL_MONTH (number) values (X);
		X := :X + 1;
	end while;
	---------------------------------------
	insert into #TMP1 (LineNum,LineName,PrdID)
	select c.number ,CASE c.number when 0 then N'Tồn đầu năm/ OB' else CAST(c.number as nvarchar(2)) end ,d."AbsEntry"
	from "OADM" a
	cross join "ADM1" b
	inner join #TBL_MONTH c on c.number between 0 and MONTH(tdate)
	left join "OFPR" d on YEAR(d."F_RefDate") = yearno and MONTH(d."F_RefDate") = c.number;
	---------------------------------------------------------------------------------------------------------------------------------

	UPDATE #TMP1 SET ThueDKDKT = IFNULL((SELECT SUM(x.Debit-x.Credit) FROM #TBL_DATA x WHERE x.FinncPriod<PrdID AND x.Account LIKE '133%'),0)
					,ThueDKCPN = IFNULL((SELECT SUM(-x.Debit+x.Credit) FROM #TBL_DATA x WHERE x.FinncPriod<PrdID AND x.Account LIKE '3331%'),0)
					,ThueDauVao = IFNULL((SELECT SUM(x.Debit-x.Credit) FROM #TBL_DATA x WHERE x.FinncPriod=PrdID AND x.Account LIKE '133%' AND (x.ContraAct NOT LIKE '3331%')),0)
					,ThueDauRa = IFNULL((SELECT SUM(-x.Debit+x.Credit) FROM #TBL_DATA x WHERE x.FinncPriod=PrdID AND x.Account LIKE '3331%' AND x.ContraAct not like '133%' and not exists(select 1 from "OACT" where "Finanse" = 'Y' and "AcctCode" = x.ContraAct)),0);
	
	UPDATE #TMP1 SET ThuePSTK = ThueDauRa-ThueDauVao;
	UPDATE #TMP1 SET ThuePNTK = CASE WHEN ThuePSTK-ThueDKDKT>0 THEN ThuePSTK-ThueDKDKT ELSE 0 END
					,ThueDKTTK = CASE WHEN ThuePSTK-ThueDKDKT<0 THEN -(ThuePSTK-ThueDKDKT) ELSE 0 END;	
	UPDATE #TMP1 SET SoDaNopTK = IFNULL((SELECT SUM(x.Debit-x.Credit) FROM #TBL_DATA x WHERE x.Account LIKE '3331%' AND (exists(select 1 from "OACT" where "Finanse" = 'Y' and "AcctCode" = x.ContraAct)) AND x.FinncPriod=PrdID),0);
	UPDATE #TMP1 SET SoConPNCK = ThueDKCPN+ThuePNTK-SoDaNopTK;	
	----------------------------------------------------------------------------------------------------------------------------------
--	update #TMP1 set AcctCode = (select top 1 a."AcctCode" from "OACT" a where a."AcctCode" like '33312%')
--					,AcctName = (select top 1 CONCAT(a."AcctName" , ifnull(CONCAT('/ ',a."FrgnName"),'')) from "OACT" a where a."AcctCode" like '33312%');
	
	-- Cong ty theo chi nhanh
	update #TMP1 set CompName 	= (select a."PrintHeadr" from "OADM" a,"ADM1" b) where branch in (-1,0);
	update #TMP1 set CompAdd 	= (select b."Street" from "OADM" a,"ADM1" b) where branch in (-1,0);
	update #TMP1 set CompName	= (select a."BPLName" from "OBPL" a where a."BPLId" = branch) where branch not in (-1,0);
	update #TMP1 set CompAdd 	= (select a."Street" from "OBPL" a where a."BPLId" = branch) where branch not in (-1,0);
	update #TMP1 set Curr = (select case when typeCur='LC' then a."MainCurncy" else a."SysCurrncy" end from "OADM" a);
	
	update #TMP1 set DateTitle = CONCAT(CONCAT(CONCAT(N'Ngày........tháng........năm........ ',char(13)),char(10)),N'Prepared on the date................')
					,RptName = CONCAT(CONCAT(CONCAT(CONCAT(CONCAT(N'SỔ CHI TIẾT TÀI KHOẢN THUẾ GIÁ TRỊ GIA TĂNG ',cast(YEAR(tdate) as nvarchar(20))),char(13)),char(10)),'VAT ACCOUNT BALANCE '),cast(YEAR(tdate) as nvarchar(20)));
	
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
	tab_TB = select * from #TMP1;
	drop table #TMP1;
	drop table #TBL_DATA;
	drop table #TBL_MONTH;
END

