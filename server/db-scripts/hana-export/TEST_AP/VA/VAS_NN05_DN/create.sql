create PROCEDURE VAS_NN05_DN (
	in branch		int,
	in period		nvarchar(20),
	in typeCur		nvarchar(2),
	out tab_TB table (
		CompName		nvarchar(100)
		,CompAdd		nvarchar(300)
		,LineNum		smallint
		,LineName		nvarchar(50)
		,PrdID			int
		,PrdYear		nvarchar(10)
		,SoPhaiNopTK	numeric(21,6)
		,SoPhaiNopSS	numeric(21,6)
		,SoDaNop		numeric(21,6)
		,SoConNopTK		numeric(21,6)
		,SoConNopSS		numeric(21,6)
		,ChenhLech		numeric(21,6)
		,ID_ThanhToan	nvarchar(100)
		,SoChungTu		nvarchar(200)
		,NgayThanhToan	nvarchar(200)
		,TongSCNTK		numeric(21,6)
		,TongSCNSS		numeric(21,6)
		,TongSCNCL		numeric(21,6)
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
	declare prdmon smallint;
	declare prdyear smallint;
	declare prdmax int; 
	declare fixqt nvarchar(30);
	declare X integer := 0;
	
	select MONTH("F_RefDate"),YEAR("F_RefDate"),"AbsEntry" into prdmon, prdyear, prdmax from OFPR where "Code" = period;
	fixqt = 'PIT finalization';
	---------------------------------------------
	create local temporary table #TMP1 (
		CompName		nvarchar(100)
		,CompAdd		nvarchar(300)
		,LineNum		smallint
		,LineName		nvarchar(50)
		,PrdID			int
		,PrdYear		nvarchar(10)
		,SoPhaiNopTK	numeric(21,6)
		,SoPhaiNopSS	numeric(21,6)
		,SoDaNop		numeric(21,6)
		,SoConNopTK		numeric(21,6)
		,SoConNopSS		numeric(21,6)
		,ChenhLech		numeric(21,6)
		,ID_ThanhToan	nvarchar(100)
		,SoChungTu		nvarchar(200)
		,NgayThanhToan	nvarchar(200)
		,TongSCNTK		numeric(21,6)
		,TongSCNSS		numeric(21,6)
		,TongSCNCL		numeric(21,6)
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
	insert into #TBL_DATA (TransId,LineId,RefNo,RefDate,FinncPriod,Account,ContraAct,Debit,Credit)
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
	from ALL_JE a, "JDT1" b
	where a.TransId = b."TransId" and a.LineId = b."Line_ID" and a.Account like '3334%' and b."FinncPriod" <= prdmax and (b."BPLId" = branch or branch  in (-1,0));
	---------------------------------------
	create local temporary table #TBL_MONTH (number int);
	while X <= prdmon do
		insert into #TBL_MONTH (number) values (X);
		X := :X + 1;
	end while;
	---------------------------------------
	insert into #TMP1 (LineNum,LineName,PrdID,PrdYear)
	select c.number ,CASE c.number when 0 then N'Tồn đầu năm/ OB' else CAST(c.number as nvarchar(2)) end ,d."AbsEntry" ,prdyear
	from "OADM" a
	cross join "ADM1" b
	inner join #TBL_MONTH c on c.number between 0 and prdmon
	left join "OFPR" d on YEAR(d."F_RefDate") = prdyear and MONTH(d."F_RefDate") = c.number;
	----------------------------------------------
	--  Tạm thời rào lại khi chưa có UDO TAX3
--	update @tbl set SoPhaiNopTK = (select a.U_TKSoPhaiNop from [@APZ_OTAX2] a where a.Code = @prdyear and a.U_Month = [@tbl].LineNum) where [@tbl].LineNum <> 0
	update #TMP1 set SoPhaiNopSS = (select SUM(-IFNULL(a.Debit,0)+IFNULL(a.Credit,0)) from #TBL_DATA a where a.RefNo <> fixqt and not exists(select 1 from "OACT" where "Finanse" = 'Y' and "AcctCode" = a.ContraAct) and a.FinncPriod = PrdID) where LineNum <> 0;
	update #TMP1 set SoDaNop = (select SUM(IFNULL(a.Debit,0)-IFNULL(a.Credit,0)) from #TBL_DATA a where a.RefNo <> fixqt and exists(select 1 from "OACT" where "Finanse" = 'Y' and "AcctCode" = a.ContraAct) and a.FinncPriod = PrdID) where LineNum <> 0;
	update #TMP1 set SoConNopSS = (select SUM(-IFNULL(a.Debit,0)+IFNULL(a.Credit,0)) from #TBL_DATA a where YEAR(a.RefDate) < prdyear) where LineNum = 0;
--	update @tbl set SoConNopTK = (select a.U_TKSoPhaiNop from [@APZ_OTAX2] a where a.Code = @prdyear and a.U_Month = [@tbl].LineNum) where [@tbl].LineNum = 0
	update #TMP1 set ID_ThanhToan = (select string_agg(CAST(a.TransId as nvarchar),',') from #TBL_DATA a where a.RefNo <> fixqt and not exists(select 1 from "OJDT" where "StornoToTr" is not null and ("TransId" = a.TransId and "StornoToTr" = a.TransId)) and exists(select 1 from "OACT" where "Finanse" = 'Y' and "AcctCode" = a.ContraAct) and a.FinncPriod = PrdID group by a.TransId) where LineNum <> 0;
	update #TMP1 set SoChungTu = (select string_agg(ifnull(a."Ref2",''),',') from "OJDT" a where #TMP1.ID_ThanhToan like CONCAT(CONCAT('%',a."TransId"),'%') group by ifnull(a."Ref2",''));
	update #TMP1 set NgayThanhToan = (select string_agg(TO_DATE(a."RefDate",'dd/MM/yyyy'),',') from "OJDT" a where #TMP1.ID_ThanhToan like CONCAT(CONCAT('%',a."TransId"),'%') group by a."RefDate");
	---------------------------------------------
	insert into #TMP1 (LineNum,LineName,PrdYear)
	select
		13
		,N'Quyết toán/ Finalization'
		,prdyear
	from
		"OADM" a
		cross join "ADM1" b;
	--------------------------------------------
	-- Tạm thời rào lại khi chưa có UDO TAX3
--	update @tbl set SoPhaiNopTK = (select a.U_TKSoPhaiNop from [@APZ_OTAX2] a where a.Code = @prdyear and a.U_Month = [@tbl].LineNum) where [@tbl].LineNum = 13


	update #TMP1 set SoPhaiNopSS = (select SUM(-IFNULL(a.Debit,0)+IFNULL(a.Credit,0)) from #TBL_DATA a where a.RefNo = fixqt and YEAR(a.RefDate) = prdyear) where #TMP1.LineNum = 13;
	update #TMP1 set SoConNopTK = IFNULL((select SoConNopTK from #TMP1 where LineNum = 0),0) + IFNULL((select SUM(IFNULL(a.SoPhaiNopTK,0)-IFNULL(a.SoDaNop,0)) from #TMP1 a where a.LineNum <= #TMP1.LineNum),0);
	update #TMP1 set SoConNopSS = IFNULL((select SoConNopSS from #TMP1 where LineNum = 0),0) + IFNULL((select SUM(IFNULL(a.SoPhaiNopSS,0)-IFNULL(a.SoDaNop,0)) from #TMP1 a where a.LineNum <= #TMP1.LineNum),0);

	update #TMP1 set ChenhLech = IFNULL(SoConNopSS,0) - IFNULL(SoConNopTK,0);

	update #TMP1 set SoConNopTK = NULL, SoConNopSS = NULL, ChenhLech = NULL where not exists(select 1 from #TBL_DATA where YEAR(RefDate) = prdyear and RefNo = fixqt) and LineNum = 13;

	update #TMP1 set TongSCNTK	= IFNULL((select a.SoConNopTK from #TMP1 a where a.LineNum = 0),0)
								+ IFNULL((select SUM(a.SoPhaiNopTK) from #TMP1 a where a.LineNum between 1 and 13),0)
								- IFNULL((select SUM(a.SoDaNop) from #TMP1 a where a.LineNum between 1 and 13),0);
	update #TMP1 set TongSCNSS	= IFNULL((select a.SoConNopSS from #TMP1 a where a.LineNum = 0),0)
								+ IFNULL((select SUM(a.SoPhaiNopSS) from #TMP1 a where a.LineNum between 1 and 13),0)
								- IFNULL((select SUM(a.SoDaNop) from #TMP1 a where a.LineNum between 1 and 13),0);
	update #TMP1 set TongSCNCL	= IFNULL(TongSCNSS,0) - IFNULL(TongSCNTK,0);
-- Tạm thời rào lại khi chưa có UDO TAX3
--	update @tbl set GhiChu		= (select a.U_TKSCLGhiChu from [@APZ_OTAX2] a where a.Code = @prdyear and a.U_Month = [@tbl].LineNum)

	update #TMP1 set AcctCode = (select top 1 a."AcctCode" from "OACT" a where a."AcctCode" like '3334%')
					,AcctName = (select top 1 CONCAT(a."AcctName" , ifnull(CONCAT('/ ',a."FrgnName"),'')) from "OACT" a where a."AcctCode" like '3334%');
	
	-- Cong ty theo chi nhanh
	update #TMP1 set CompName 	= (select a."PrintHeadr" from "OADM" a,"ADM1" b) where branch in (-1,0);
	update #TMP1 set CompAdd 	= (select b."Street" from "OADM" a,"ADM1" b) where branch in (-1,0);
	update #TMP1 set CompName	= (select a."BPLName" from "OBPL" a where a."BPLId" = branch) where branch not in (-1,0);
	update #TMP1 set CompAdd 	= (select a."Street" from "OBPL" a where a."BPLId" = branch) where branch not in (-1,0);
	update #TMP1 set Curr = (select case when typeCur='LC' then a."MainCurncy" else a."SysCurrncy" end from "OADM" a);
	
	update #TMP1 set DateTitle = CONCAT(CONCAT(CONCAT(N'Ngày........tháng........năm........ ',char(13)),char(10)),N'Prepared on the date................')
					,RptName = CONCAT(CONCAT(CONCAT(CONCAT(CONCAT(N'SỔ CHI TIẾT THUẾ THU NHẬP DOANH NGHIỆP NĂM ',cast(prdyear as nvarchar(20))),char(13)),char(10)),'CORPORATE INCOME TAX '),cast(prdyear as nvarchar(20)));
	
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

