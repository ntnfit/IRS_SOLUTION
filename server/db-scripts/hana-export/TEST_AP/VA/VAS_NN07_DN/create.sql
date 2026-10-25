create PROCEDURE VAS_NN07_DN (
	in branch		int,
	in prdyear		int,
	in typeCur		nvarchar(2),
	out tab_TB table (
		CompName		nvarchar(100)
		,CompAdd		nvarchar(300)
		,LineId			int
		,LineName		nvarchar(100)
		,SoPhaiNop		numeric(21,6)
		,SoDaNop		numeric(21,6)
		,SoConLai		numeric(21,6)
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
	create local temporary table #TMP1 (
		CompName		nvarchar(100)
		,CompAdd		nvarchar(300)
		,LineId			int
		,LineName		nvarchar(100)
		,SoPhaiNop		numeric(21,6)
		,SoDaNop		numeric(21,6)
		,SoConLai		numeric(21,6)
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
	create local temporary table #TBL_DATA (
		TransId			int 
		,LineId			int
		,RefDate		datetime
		,RefNo			nvarchar(30)
		,Account		nvarchar(30)
		,ContraAct		nvarchar(30)
		,Debit			numeric(21,6)
		,Credit			numeric(21,6)
	);
	insert into #TBL_DATA (TransId,LineId,RefDate,RefNo,Account,ContraAct,Debit,Credit)	
	select 
		a.TransId
		,a.LineId
		,a.RefDate
		,b."Ref2"
		,a.Account
		,a.ContraAct
		,case when typeCur='LC' then a.Debit else a.DebitSC end
		,case when typeCur='LC' then a.Credit else a.CreditSC end	
	from ALL_JE a, "JDT1" b	
	where a.TransId = b."TransId" and a.LineId = b."Line_ID" and a.Account like '33381%' and YEAR(a.RefDate) <= prdyear and (b."BPLId" = branch or branch in (-1,0))	;
	---------------------------------------
	insert into #TMP1 (LineId,LineName,SoPhaiNop,SoDaNop)	
	values (1, N'Số tồn đầu năm/ OB',
		(select SUM(-a.Debit+a.Credit) from #TBL_DATA a where YEAR(a.RefDate) < prdyear and not exists(select 1 from "OACT" where "AcctCode" = a.ContraAct and "Finanse" = 'Y')),
		(select SUM(a.Debit-a.Credit) from #TBL_DATA a where YEAR(a.RefDate) < prdyear and exists(select 1 from "OACT" where "AcctCode" = a.ContraAct and "Finanse" = 'Y')));
	insert into #TMP1 (LineId,LineName,SoPhaiNop,SoDaNop)	
	values (2, CONCAT(N'Năm/ Year ',CAST(prdyear as nvarchar(10))), 
		(select SUM(-a.Debit+a.Credit) from #TBL_DATA a where YEAR(a.RefDate) = prdyear and not exists(select 1 from "OACT" where "AcctCode" = a.ContraAct and "Finanse" = 'Y')),
		(select SUM(a.Debit-a.Credit) from #TBL_DATA a where YEAR(a.RefDate) = prdyear and exists(select 1 from "OACT" where "AcctCode" = a.ContraAct and "Finanse" = 'Y')));
		
		
	----------
	update #TMP1 set SoConLai = (select SUM(IFNULL(a.SoPhaiNop,0)-IFNULL(a.SoDaNop,0)) from #TMP1 a where a.LineId <= #TMP1.LineId)	;
	update #TMP1 set ID_ThanhToan = (select top 1 cast(a.TransId as nvarchar(10)) from #TBL_DATA a where YEAR(a.RefDate) = prdyear);--(select distinct CAST(a.TransId as nvarchar) + ',' from #TBL_DATA a where not exists(select 1 from "OJDT" where "StornoToTr" is not null and ("TransId" = a.TransId and "StornoToTr" = a.TransId)) and exists(select 1 from "OACT" where "Finanse" = 'Y' and "AcctCode" = a.ContraAct) and YEAR(a.RefDate) = prdyear for xml path('')) where LineId <> 1;
	update #TMP1 set SoChungTu = (select top 1 a.RefNo from #TBL_DATA a where YEAR(a.RefDate) = prdyear);--(select distinct ISNULL(a."Ref2" + ',','') from "OJDT" a where exists(select 1 from dbo.Split(ID_ThanhToan,',') where Data = a.TransId) for xml path(''));
	update #TMP1 set NgayThanhToan = (select top 1 TO_VARCHAR (a.RefDate,'dd/MM/yyyy') from #TBL_DATA a where YEAR(a.RefDate) = prdyear);--(select distinct ISNULL(CONVERT(nvarchar(10),a."RefDate",103) + ',','') from "OJDT" a where exists(select 1 from dbo.Split(ID_ThanhToan,',') where Data = a.TransId) for xml path(''))	where LineID=2;
	
	update #TMP1 set AcctCode = (select top 1 a."AcctCode" from "OACT" a where a."AcctCode" like '33381%')
					,AcctName = (select top 1 CONCAT(a."AcctName" , ifnull(CONCAT('/ ',a."FrgnName"),'')) from "OACT" a where a."AcctCode" like '33381%');
	
	update #TMP1 set AcctName = N'Thuế nhà thầu - Witholding Tax'
					,AcctCode = '33382';
	-- Cong ty theo chi nhanh
	update #TMP1 set CompName 	= (select a."PrintHeadr" from "OADM" a,"ADM1" b) where branch in (-1,0);
	update #TMP1 set CompAdd 	= (select b."Street" from "OADM" a,"ADM1" b) where branch in (-1,0);
	update #TMP1 set CompName	= (select a."BPLName" from "OBPL" a where a."BPLId" = branch) where branch not in (-1,0);
	update #TMP1 set CompAdd 	= (select a."Street" from "OBPL" a where a."BPLId" = branch) where branch not in (-1,0);
	update #TMP1 set Curr = (select case when typeCur='LC' then a."MainCurncy" else a."SysCurrncy" end from "OADM" a);
	
	update #TMP1 set DateTitle = CONCAT(CONCAT(CONCAT(N'Ngày........tháng........năm........ ',char(13)),char(10)),N'Prepared on the date................')
					,RptName = CONCAT(CONCAT(CONCAT(CONCAT(CONCAT(N'SỔ CHI TIẾT THUẾ BẢO VỆ MÔI TRƯỜNG NĂM ',cast(prdyear as nvarchar(20))),char(13)),char(10)),'ENVIROMENT PROTECTION TAX '),cast(prdyear as nvarchar(20)));
	
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

END

