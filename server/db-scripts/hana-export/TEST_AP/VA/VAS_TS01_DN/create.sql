CREATE procedure VAS_TS01_DN (
	in list_AsCl	table (Code nvarchar(30)),
	in period		nvarchar(30),
	in branch		int,
	out tab_FA		table (
		AssetCode			nvarchar(50),
		AssetName			nvarchar(200),
		AcctCode			nvarchar(30),
		AcctName			nvarchar(100),
		AssetTypeCode		nvarchar(30),
		AssetTypeName		nvarchar(200),
		ClassCode			nvarchar(50),
		ClassName			nvarchar(100),
		BuyDate				timestamp,
		Dept				nvarchar(300),
		StatDate			timestamp,
		APC					numeric(21,6),
		UsefulLife			int,
		DprMVal				numeric(21,6),
		OBDprVal			numeric(21,6),
		M0					numeric(21,6),
		M1 numeric(21,6), M2 numeric(21,6), M3 numeric(21,6), M4 numeric(21,6),
		M5 numeric(21,6), M6 numeric(21,6), M7 numeric(21,6), M8 numeric(21,6),
		M9 numeric(21,6), M10 numeric(21,6), M11 numeric(21,6), M12 numeric(21,6),
		DprDif				numeric(21,6),
		YTDDprVal			numeric(21,6),
		EBDprVal			numeric(21,6),
		NBV					numeric(21,6),
		CompName			nvarchar(100),
		CompAdd				nvarchar(300),
		DeciAmt				int,
		DecSep				nvarchar(1),
		ThousSep			nvarchar(1),
		LastYear			nvarchar(10),
		ThisYear			nvarchar(10)
	)
)
as
begin
	declare perAbs, prvPerAbs int;
	declare perCat, monStr nvarchar(30);
	declare mainArea nvarchar(100);
	declare monID, monMax tinyint;
	declare maxFDate, rptDate timestamp;
	
	select a."AbsEntry", a."Category" into perAbs, perCat from OFPR a where a."Code" = :period;
	
	select "Code" into mainArea from ODPA where "MainArea" = 'Y';
	
	create local temporary table #tbl as (
		select * from :tab_FA
	);

	insert into #tbl (AssetCode, AssetName, AcctCode, AcctName, ClassCode, ClassName, BuyDate, StatDate, UsefulLife, LastYear, ThisYear)
	select 
		a."ItemCode",
		a."ItemName",
		b2."AcctCode",
		b2."AcctName",
		b."Code",
		b."Name",
		a."CapDate",
		d."DprStart",
		d."UsefulLife",
		CAST(YEAR(c."F_RefDate")-1 as nvarchar),
		CAST(YEAR(c."F_RefDate") as nvarchar)
	from
		OITM a
		inner join OACS b on b."Code" = a."AssetClass"
		left join ACS1 b0 on b0."Code" = b."Code"
		inner join OADT b1 on b1."Code" = b0."AcctDtn"
		left join OACT b2 on b2."AcctCode" = b1."BalanceAct"
		left join OFPR c on c."Code" = :period
		left join ITM7 d on d."ItemCode" = a."ItemCode" and d."PeriodCat" = c."Category" and d."DprArea" in ('*',mainArea)
	where
		a."ItemType" = 'F'
		and a."AssetClass" in (select Code from :list_AsCl)
		and (b."BPLId"=branch or branch=0)
		and a."CapDate" <= c."T_RefDate";


	update #tbl set APC	= IFNULL((select SUM(a."APC") from ITM8 a where a."ItemCode" = #tbl.AssetCode and a."DprArea" in ('*',mainArea) and a."PeriodCat" = :perCat),0)
						+ IFNULL((select SUM(b."LineTotal") from OACQ a, ACQ1 b where a."DocEntry" = b."DocEntry" and a."DocStatus" = 'P' and b."ItemCode" = #tbl.AssetCode and a."DprArea" in ('*',mainArea) and a."PeriodCat" = :perCat and a."FinncPriod" <= :perAbs),0)
						- IFNULL((select SUM(b."LineTotal") from OACD a, ACD1 b where a."DocEntry" = b."DocEntry" and a."DocStatus" = 'P' and b."ItemCode" = #tbl.AssetCode and a."DprArea" in ('*',mainArea) and a."PeriodCat" = :perCat and a."FinncPriod" <= :perAbs),0);
	
	update #tbl set DprMVal	= APC/UsefulLife where UsefulLife <> 0;
	
	update #tbl set OBDprVal = IFNULL((select SUM(IFNULL(a."OrDpAcc",0)+IFNULL(a."SpDpAcc1",0)) from ITM8 a where a."ItemCode" = #tbl.AssetCode and a."DprArea" in ('*',mainArea) and a."PeriodCat" = :perCat),0);
	
	select 1, MONTH(a."F_RefDate") into monID, monMax from OFPR a where a."Code" = :period;
	
	while (monID <= monMax) do
		monStr = monID;
	
		execute immediate ('update #tbl set M' || :monStr || ' = IFNULL((select SUM(b."OrdDprAmt") from ODRN a, DRN2 b, OFPR c where a."PostPeriod" = c."AbsEntry" and a."DocEntry" = b."DocEntry" and a."Canceled" = ''N'' and a."Status" = ''S'' and b."ItemCode" = #tbl.AssetCode and a."DprArea" in (''*'',''' || :mainArea || ''') and a."PeriodCat" = ''' || :perCat || ''' and MONTH(c."F_RefDate") = ' || :monStr || '),0) + IFNULL((select SUM(b."LineTotal") from OMDP a, MDP1 b, OFPR c where a."FinncPriod" = c."AbsEntry" and a."DocEntry" = b."DocEntry" and a."DocStatus" = ''P'' and b."ItemCode" = #tbl.AssetCode and a."DprArea" in (''*'',''' || :mainArea || ''') and a."PeriodCat" = ''' || :perCat || ''' and MONTH(c."F_RefDate") = ' || :monStr || '),0)');
		
		monID = monID + 1;
	end while;
	
	select a."F_RefDate" into rptDate from OFPR a where a."Code" = :period;
	
	select MAX(a."F_RefDate") into maxFDate from OFPR a where a."F_RefDate" < rptDate;
		
	update #tbl set Dept = (select MAX(b."OcrName") from ITM6 a, OOCR b where a."OcrCode4" = b."OcrCode" and b."DimCode" = 4 and a."ItemCode" = #tbl.AssetCode and rptDate between IFNULL(a."ValidFrom",'1900-01-01') and IFNULL(a."ValidTo",rptDate));
		
	select a."AbsEntry" into prvPerAbs from OFPR a where a."F_RefDate" = :maxFDate;
		
	update #tbl set M0	= IFNULL((select SUM(b."OrdDprAmt") from ODRN a, DRN2 b, OFPR c where a."PostPeriod" = c."AbsEntry" and a."DocEntry" = b."DocEntry" and a."Canceled" = 'N' and a."Status" = 'S' and b."ItemCode" = #tbl.AssetCode and a."DprArea" in ('*',mainArea) and a."PostPeriod" = :prvPerAbs),0)
						+ IFNULL((select SUM(b."LineTotal") from OMDP a, MDP1 b, OFPR c where a."FinncPriod" = c."AbsEntry" and a."DocEntry" = b."DocEntry" and a."DocStatus" = 'P' and b."ItemCode" = #tbl.AssetCode and a."DprArea" in ('*',mainArea) and a."FinncPriod" = :prvPerAbs),0);
	
	update #tbl set DprDif =
		CASE
			when monMax = 1 then M1-M0
			when monMax = 2 then M2-M1
			when monMax = 3 then M3-M2
			when monMax = 4 then M4-M3
			when monMax = 5 then M5-M4
			when monMax = 6 then M6-M5
			when monMax = 7 then M7-M6
			when monMax = 8 then M8-M7
			when monMax = 9 then M9-M8
			when monMax = 10 then M10-M9
			when monMax = 11 then M11-M10
			when monMax = 12 then M12-M11
		end;
	
	update #tbl set YTDDprVal = IFNULL(M1,0)+IFNULL(M2,0)+IFNULL(M3,0)+IFNULL(M4,0)+IFNULL(M5,0)+IFNULL(M6,0)+IFNULL(M7,0)+IFNULL(M8,0)+IFNULL(M9,0)+IFNULL(M10,0)+IFNULL(M11,0)+IFNULL(M12,0);
	update #tbl set EBDprVal = IFNULL(OBDprVal,0) + IFNULL(YTDDprVal,0);
	
	update #tbl set NBV = IFNULL(APC,0) - IFNULL(EBDprVal,0);
	update #tbl set DprDif = IFNULL(DprDif,0);
	
	update #tbl set AssetTypeCode = LEFT(AcctCode,3);
	update #tbl set AssetTypeName =
		CASE
			when AssetTypeCode = '211' then N'TSCĐ Hữu Hình'
			when AssetTypeCode = '212' then N'TSCĐ Thuê Tài Chính'
			when AssetTypeCode = '213' then N'TSCĐ Vô Hình'
			when AssetTypeCode = '217' then N'Bất động sản đầu tư'
			when AssetTypeCode = '242' then N'Chi phí trả trước'
		end;
	
	update a set CompName = b."PrintHeadr", DecSep = b."DecSep", ThousSep = b."ThousSep" from #tbl a, OADM b;
	
	update a set CompAdd = b."Street" from #tbl a, ADM1 b;
	
	update a set DeciAmt = IFNULL(NULLIF(c."Decimals",-1),b."SumDec") from #tbl a, OADM b, OCRN c where b."MainCurncy" = c."CurrCode";

	tab_FA = select * from #tbl;
	
	drop table #tbl;
end

