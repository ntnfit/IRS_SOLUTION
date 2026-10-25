create PROCEDURE VAS_NN01_DN (
	in 	branch		int,
	in 	fdate		timestamp,
	in 	tdate		timestamp,
	in 	filter		char(1),
	in 	typeCur		nvarchar(2),
	out tab_TB table (
		LineID		int,
		Levels		int,
		GrpLine		int,
		Postable	char(1),
		FatherNum 	nvarchar(100),
		AcctCode	nvarchar(100),
		AcctName	nvarchar(500),
		OBSCN 		numeric(21,6),
		SPN 		numeric(21,6),
		SDN 		numeric(21,6),
		EBSCN 		numeric(21,6),
		TotOBSCN 	numeric(21,6),
		TotSPN 		numeric(21,6),
		TotSDN 		numeric(21,6),
		TotEBSCN 	numeric(21,6),
		Curr 		nvarchar(30),
		DeciQty 	int,
		DeciPrice 	int,
		DeciAmt 	int,
		DateSep 	nvarchar(1),
		ThousSep 	nvarchar(1),
		Logo 		nvarchar(300),
		DateTitle 	nvarchar(200),
		CompName 	nvarchar(300),
		CompAdd 	nvarchar(500),
		RptName 	nvarchar(500)
	)
)
AS
BEGIN
	---------------------------------------------
--	declare fdate, tdate timestamp;
--	declare X integer := 0;
--	select MIN("F_RefDate"),MAX("T_RefDate") into fdate,tdate from "OFPR" where year("F_RefDate")=yearno;
	DECLARE lvs int;
	SELECT MAX("Levels") into lvs FROM "OACT" WHERE "AcctCode" LIKE '333%';
	---------------------------------------------
	create local temporary table #TMP1 (
		LineID		int,
		Levels		int,
		GrpLine		int,
		Postable	char(1),
		FatherNum 	nvarchar(100),
		AcctCode	nvarchar(100),
		AcctName	nvarchar(500),
		OBSCN 		numeric(21,6),
		SPN 		numeric(21,6),
		SDN 		numeric(21,6),
		EBSCN 		numeric(21,6),
		TotOBSCN 	numeric(21,6),
		TotSPN 		numeric(21,6),
		TotSDN 		numeric(21,6),
		TotEBSCN 	numeric(21,6),
		Curr 		nvarchar(30),
		DeciQty 	int,
		DeciPrice 	int,
		DeciAmt 	int,
		DateSep 	nvarchar(1),
		ThousSep 	nvarchar(1),
		Logo 		nvarchar(300),
		DateTitle 	nvarchar(200),
		CompName 	nvarchar(300),
		CompAdd 	nvarchar(500),
		RptName 	nvarchar(500)
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
	---------------------------------------
	insert into #TMP1 (LineID,GrpLine,Postable,Levels, FatherNum, AcctCode, AcctName)
	select ROW_NUMBER() OVER (PARTITION BY a."FatherNum" ORDER BY a."GrpLine"),a."GrpLine",a."Postable",a."Levels",a."FatherNum",a."AcctCode",a."AcctName"
	FROM "OACT" a
	WHERE a."AcctCode" LIKE '333%' AND "FatherNum" IN (SELECT x."AcctCode" FROM "OACT" x WHERE x."AcctCode" LIKE '333%')
	ORDER BY a."GrpLine";
	--------------------------------------------

	insert into #TBL_DATA (TransId,LineId,RefNo,RefDate,FinncPriod,Account,ContraAct,Debit,Credit)
	select a.TransId, a.LineId,IFNULL(b."Ref2",'') Ref2,a.RefDate,b."FinncPriod",a.Account,a.ContraAct,
		case when typeCur='LC' then a.Debit else a.DebitSC end Debit,
		case when typeCur='LC' then a.Credit else a.CreditSC end Credit
	from ALL_JE a
	left join "JDT1" b on a.TransID=b."TransId" and a.LineID=b."Line_ID"
	where a.RefDate<=tdate and (b."BPLId" = branch or branch in (-1,0))
	and a.Account like '333%';
	-------------------------------------------
	update #TMP1 set OBSCN	= IFNULL((select SUM(-IFNULL(a.Debit,0)+IFNULL(a.Credit,0)) from #TBL_DATA a where a.RefDate < fdate and a.Account = #TMP1.AcctCode),0);
	update #TMP1 set SDN	= IFNULL((select SUM(IFNULL(a.Debit,0)) from #TBL_DATA a where a.RefDate between fdate and tdate and a.Account = #TMP1.AcctCode and (exists(select 1 from "OACT" where "Finanse" = 'Y' and "AcctCode" = a.ContraAct) or exists(select 1 from "OACT" where "LocManTran" = 'Y' and "AcctCode" = a.ContraAct))),0);
	update #TMP1 set EBSCN	= IFNULL((select SUM(-IFNULL(a.Debit,0)+IFNULL(a.Credit,0)) from #TBL_DATA a where a.Account=#TMP1.AcctCode),0);
	UPDATE #TMP1 SET SPN 	= IFNULL((select SUM(-IFNULL(a.Debit,0)+IFNULL(a.Credit,0)) from #TBL_DATA a where a.RefDate BETWEEN fdate AND tdate and a.Account=#TMP1.AcctCode),0)+IFNULL(SDN,0);

	UPDATE #TMP1 SET OBSCN = (SELECT b.Amt FROM #TMP1 a
									LEFT JOIN (SELECT x.FatherNum,SUM(x.OBSCN) Amt FROM #TMP1 x GROUP BY x.FatherNum) b ON a.AcctCode=b.FatherNum WHERE a.AcctCode=#TMP1.AcctCode)
					,SPN = (SELECT b.Amt FROM #TMP1 a
									LEFT JOIN (SELECT x.FatherNum,SUM(x.SPN) Amt FROM #TMP1 x GROUP BY x.FatherNum) b ON a.AcctCode=b.FatherNum WHERE a.AcctCode=#TMP1.AcctCode) 
					,SDN = (SELECT b.Amt FROM #TMP1 a
									LEFT JOIN (SELECT x.FatherNum,SUM(x.SDN) Amt FROM #TMP1 x GROUP BY x.FatherNum) b ON a.AcctCode=b.FatherNum WHERE a.AcctCode=#TMP1.AcctCode)
					,EBSCN = (SELECT b.Amt FROM #TMP1 a
									LEFT JOIN (SELECT x.FatherNum,SUM(x.EBSCN) Amt FROM #TMP1 x GROUP BY x.FatherNum) b ON a.AcctCode=b.FatherNum WHERE a.AcctCode=#TMP1.AcctCode)
	WHERE Postable='N';

	update #TMP1 set TotOBSCN	= IFNULL((select SUM(OBSCN) from #TMP1 where Postable='Y'),0);
	update #TMP1 set TotSPN		= IFNULL((select SUM(SPN) from #TMP1 where Postable='Y'),0);
	update #TMP1 set TotSDN		= IFNULL((select SUM(SDN) from #TMP1 where Postable='Y'),0);
	update #TMP1 set TotEBSCN	= IFNULL((select SUM(EBSCN) from #TMP1 where Postable='Y'),0);

	UPDATE #TMP1 SET AcctName = CASE WHEN Levels<>lvs THEN AcctName ELSE CONCAT('  - ',AcctName) END;


/*
*/
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
					,RptName = CONCAT(CONCAT(CONCAT(CONCAT(CONCAT(N'TÌNH HÌNH THỰC HIỆN NGHĨA VỤ VỚI NHÀ NƯỚC ',cast(YEAR(tdate) as nvarchar(20))),char(13)),char(10)),'TAXES AND PAYABLE TO STATE BUDGET '),cast(YEAR(tdate) as nvarchar(20)));
	
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
--	drop table #TBL_MONTH;
END

