CREATE PROCEDURE VAS_TS06_DN (
	in branch		int,
	in periodcode	nvarchar(20)
)
as
begin
	declare dprarea nvarchar(50);
	declare PeriodCate nvarchar(20);
	declare fdate timestamp;
	declare period int;
	declare assetclass nvarchar(1000);
	assetclass='';
	
	select a."Code" into dprarea from "ODPA" a where a."MainArea" = 'Y';
	select a."F_RefDate","AbsEntry" into fdate,period from "OFPR" a where a."Code"=periodcode;
	
	create local temporary table #tbl (
		RptName			nvarchar(300)
		,CompName		nvarchar(300)
		,CompAddr		nvarchar(300)
		,GrpCode		nvarchar(100)
		,GrpName		nvarchar(500)
		,FACode			nvarchar(100)
		,FAName			nvarchar(500)
		,DprType		NVARCHAR(100)
		,BgDate			timestamp
		,EdDate			timestamp
		,Nguyengia		numeric(28,10)
		,TgKH			numeric(28,10)
		,TgdaKH			numeric(28,10)
		,Tgconlai		numeric(28,10)
		,KHtrongthang	numeric(28,10)
		,KHKyTruoc		numeric(28,10)
		,KHluyke		numeric(28,10) -- Khau hao luy ke tu dau nam tro ve truoc
		,KHluyke1		numeric(28,10) -- Kh?u hao luy k? t? d?u nam
		,KHluyke2		numeric(28,10) -- Kh?u hao luy k? t? khi b?t d?u s? d?ng
		,GTconlai		numeric(28,10)
		,TKkhauhao		nvarchar(200)
		,TKphanbo		nvarchar(100)
		,Cate			nvarchar(100)
		,Area			nvarchar(100)
		,Mth			int		
		,Yr				int
		,Deprt			nvarchar(400)
		,Curr			nvarchar(30)

		,DimCode1		nvarchar(100)
		,DimCode2		nvarchar(100)
		,DimCode3		nvarchar(100)
		,DimCode4		nvarchar(100)
		,DimCode5		nvarchar(100)
		,DimName1		nvarchar(4000)
		,DimName2		nvarchar(4000)
		,DimName3		nvarchar(4000)
		,DimName4		nvarchar(4000)
		,DimName5		nvarchar(4000)
		,Project		nvarchar(100)

		,DeciQty		int
		,DeciPrice		int
		,DeciAmt		int
		,DateSep		nvarchar(1)
		,ThousSep		nvarchar(1)
		,Logo			nvarchar(100)
		,DateTitle		nvarchar(200)
	);
	--------------------------------------------------------------------------------------------
			
	insert into #tbl (GrpCode,GrpName,FACode,FAName,DprType,BgDate,EdDate,TgKH,Tgconlai,TgdaKH,TKkhauhao,TKphanbo,Area)
		select 
			z."AssetClass"
			,c."Name"
			,a."ItemCode"
			,a."ItemName"
			,g."DprMeth"
			,a."CapDate"
			,b."DprEnd"
			,b."UsefulLife"
			,b."RemainLife"
			,IFNULL(b."UsefulLife",0)-IFNULL(b."RemainLife",0)
			,f."OrdDprAcc"
			,f."OrdDprAct"
			,b."DprArea"
		from 
			"OITM" a
			left join "ITM7" b on (b."ItemCode"=a."ItemCode" and ((b."DprArea"='*') or (b."DprArea"=dprarea)) and b."PeriodCat"=(select "Category" from "OFPR" where "AbsEntry"=period))
			left join (select distinct "ItemCode",y."AssetClass" from "ODRN" x
					left join "DRN2" y  on x."DocEntry"=y."DocEntry"
					where x."PostPeriod"=period AND x."Canceled"='N') z on z."ItemCode"=b."ItemCode"
			left join "OACS" c on c."Code"=z."AssetClass"
			left join "ACS1" e on e."Code"=c."Code" and e."DprAreaID"=b."DprArea"
			left join "OADT" f on f."Code"=e."AcctDtn"
			LEFT JOIN "ODTP" g ON g."Code"=b."DprType"
		where 
			a."ItemType"='F' 
			and a."frozenFor"='N' and a."AsstStatus"='A'
			and (z."AssetClass" IN (select "DATA" from Split(assetclass,',')) or assetclass='')
			and (c."BPLId" = branch or branch in ('-1','0'))

		UNION ALL
        select 
			c."Code"
			,c."Name"
			,a."ItemCode"
			,a."ItemName"
			,g."DprMeth"
			,a."CapDate"
			,b."DprEnd"
			,b."UsefulLife"
			,b."RemainLife"
			,IFNULL(b."UsefulLife",0)-IFNULL(b."RemainLife",0)
			,f."OrdDprAcc"
			,f."OrdDprAct"
			,b."DprArea"
		from 
			"OITM" a
			left join "ITM7" b on (b."ItemCode"=a."ItemCode" and ((b."DprArea"='*') or (b."DprArea"=dprarea)) and b."PeriodCat"=(select "Category" from "OFPR" where "AbsEntry"=period))
			left join "OACS" c on c."Code"=a."AssetClass"
			left join "ACS1" e on e."Code"=c."Code" and e."DprAreaID"=b."DprArea"
			left join "OADT" f on f."Code"=e."AcctDtn"
			LEFT JOIN "ODTP" g ON g."Code"=b."DprType"
		where 
			a."ItemType"='F' 
			and a."frozenFor"='N' and a."AsstStatus"='A'
			AND (a."AssetClass" IN (select "DATA" from Split(assetclass,',')) or assetclass='')
			and b."ItemCode" not in (select distinct "ItemCode" from "ODRN" x
									left join "DRN2" y on x."DocEntry"=y."DocEntry"
									where x."PostPeriod"=period AND x."Canceled"='N')
			and (c."BPLId" = branch or branch in ('-1','0'));
	--
	create local temporary table #tblKH (
		ItemCode	nvarchar(50),
		DprArea		nvarchar(50),
		RefDate		timestamp,
		MinDate		timestamp
	);
	insert into #tblKH (ItemCode,DprArea,RefDate,MinDate)
	SELECT b."ItemCode",a."DprArea",MAX(a."RefDate") as "RefDate",min(a."RefDate") as "MinDate" from "ODRN" a, "DRN2" b where a."DocEntry"=b."DocEntry" and a."Canceled"='N' and a."PostPeriod"<=period GROUP BY b."ItemCode",a."DprArea" having sum("OrdDprAmt")<>0
	UNION ALL
	select b."ItemCode",a."DprArea",MAX(a."PostDate") as "RefDate",min(a."PostDate") as "MinDate" from "OMDP" a, "MDP1" b where a."DocEntry"=b."DocEntry" and a."FinncPriod"<=period group by b."ItemCode",a."DprArea" HAVING sum("LineTotal")<>0;
	
	update #tbl set EdDate = (select max(x.RefDate) from #tblKH x where x.ItemCode=#tbl.FACode and (x.DprArea='*' or x.DprArea=#tbl.Area));
	
	UPDATE #tbl SET TgdaKH = CASE WHEN (DprType<>'NO' and EdDate>=BgDate) THEN DAYS_BETWEEN(BgDate,EdDate)+1 ELSE 0 end;
	update #tbl set Tgconlai=case when TgKH=0 then 0 else TgKH-TgdaKH END;
    UPDATE #tbl SET Tgconlai = CASE WHEN IFNULL(Tgconlai,0)<0 THEN 0 ELSE Tgconlai end;
	--
	select "Category" into PeriodCate from "OFPR" where "AbsEntry"=period;
	update #tbl set Cate = PeriodCate;
	-- NGUYÊN GIÁ ================================================================================================================================================
	create local temporary table #tblNG (
		ItemCode	nvarchar(50),
		DprArea		nvarchar(50),
		Amnt		numeric(28,10)
	);
	insert into #tblNG (ItemCode,DprArea,Amnt)
	select "ItemCode","DprArea",SUM("APC") as Amnt from "ITM8" where "PeriodCat"=PeriodCate group by "ItemCode","DprArea"
	union all
	select b."ItemCode",a."DprArea",SUM(b."LineTotal") as Amnt from "OACQ" a, "ACQ1" b where a."DocEntry"=b."DocEntry" and a."DocStatus"='P' and a."PeriodCat"=PeriodCate and a."FinncPriod"<=period group by b."ItemCode",a."DprArea"
	union all
	select b."ItemCode",a."DprArea",SUM(-b."LineTotal") as Amnt from "OACD" a, "ACD1" b where a."DocEntry"=b."DocEntry" and a."DocStatus"='P' and a."PeriodCat"=PeriodCate and a."FinncPriod"<=period group by b."ItemCode",a."DprArea";

	update #tbl set Nguyengia = (select sum(x.Amnt) from #tblNG x where x.ItemCode=#tbl.FACode and (x.DprArea='*' or x.DprArea=#tbl.Area));
	-- KHẤU HAO ==================================================================================================================================================
	-- Khấu hao các năm trước--------------------------------------------------------------------------
	update #tbl set KHluyke = (select SUM(ifnull("OrDpAcc",0)+IFNULL("SpDpAcc1",0)) from "ITM8" where "ItemCode"=FACode and "PeriodCat"=#tbl.Cate and (("DprArea"='*') or ("DprArea"=#tbl.Area)));
	-- Khấu hao năm nay--------------------------------------------------------------------------------
	create local temporary table #tblKH2 (
		ItemCode	nvarchar(50),
		DprArea		nvarchar(50),
		PostPeriod	int,
		PID			int,
		Amnt		numeric(28,10)
	);
	insert into #tblKH2 (ItemCode,DprArea,PostPeriod,Amnt)
	select b."ItemCode",a."DprArea",a."PostPeriod",SUM(b."OrdDprAmt") as Amnt from "ODRN" a, "DRN2" b where a."DocEntry"=b."DocEntry" and a."Canceled"='N' and a."PeriodCat"=PeriodCate and a."PostPeriod"<=period group by b."ItemCode",a."DprArea",a."PostPeriod"
	union all
	select b."ItemCode",a."DprArea",a."FinncPriod",SUM(b."LineTotal") as Amnt from "OMDP" a, "MDP1" b where a."DocEntry"=b."DocEntry" AND a."DocStatus"='P' and a."PeriodCat"=PeriodCate and a."FinncPriod"<=period group by b."ItemCode",a."DprArea",a."FinncPriod";
	-- Khấu hao kỳ trước
	update #tbl set KHKyTruoc = (select sum(x.Amnt) from #tblKH2 x where x.PostPeriod=period-1 and x.ItemCode=#tbl.FACode and (x.DprArea='*' or x.DprArea=#tbl.Area));
	-- Khấu hao kỳ này
	update #tbl set KHtrongthang = (select sum(x.Amnt) from #tblKH2 x where x.PostPeriod=period and x.ItemCode=#tbl.FACode and (x.DprArea='*' or x.DprArea=#tbl.Area));
	-- Khấu hao lũy kế từ đầu năm
	update #tbl set KHluyke1 = (select sum(x.Amnt) from #tblKH2 x where x.ItemCode=#tbl.FACode and (x.DprArea='*' or x.DprArea=#tbl.Area));
	-- Tổng giá trị đã khấu hao
	update #tbl set KHluyke2 = IFNULL(KHluyke,0) + IFNULL(KHluyke1,0);
	-- Giá trị còn lại
	update #tbl set GTconlai = IFNULL(Nguyengia,0) - IFNULL(KHluyke2,0);
	--=============================================================================================================================================================
	if (select count(FACode) from #tbl)=0 then
			insert into #tbl (FAName) values (N'No Item');
	end if;

	update x set FAName = LTRIM(FAName), Mth = MONTH("F_RefDate"), Yr=YEAR("T_RefDate") from #tbl x, "OFPR" y where "AbsEntry"=period;
	
	update #tbl set
	   RptName  = N'BẢNG TÍNH KHẤU HAO TÀI SẢN CỐ ĐỊNH',
	   DateTitle= N'Ngày........tháng........năm........ ';
	update #tbl set
	   CompName = IFNULL((select a."BPLName" from "OBPL" a where a."BPLId" = branch),(select "CompnyName" from "OADM")),
	   CompAddr = IFNULL((select a."Street" from "OBPL" a where a."BPLId" = branch),(select top 1 "Street" from "ADM1"));
	
	update #tbl set Curr = (select a."MainCurncy" from "OADM" a);
		-- So chu so sau dau phay 
	update b set DeciQty = a."QtyDec",DeciPrice = a."PriceDec",DateSep = a."DecSep",ThousSep = a."ThousSep" from #tbl b, "OADM" a;
--	update #tbl set Logo = LogoImage from OADP
	update #tbl set DeciAmt = case when (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.Curr)='-1' then (select a."SumDec" from "OADM" a) else (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.Curr) end;
	update x set Deprt = y."OcrCode4" from #tbl x, ITM6 y where x.FACode=y."ItemCode" and y."ValidFrom"<=:fdate and ifnull(y."ValidTo",'2099-12-31')>=:fdate;	
	
	select * from #tbl;
	
	
	drop table #tbl;
	drop table #tblKH;
	drop table #tblKH2;
	drop table #tblNG;
end

