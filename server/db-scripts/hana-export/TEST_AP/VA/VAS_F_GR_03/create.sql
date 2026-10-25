CREATE procedure VAS_F_GR_03 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	
	select ifnull(x."BPLId",0) into bplID from "ORPD" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		FormNo			nvarchar(50)
		, RptName		nvarchar(200)
		, PeriodTitle	nvarchar(100)
		, CompName		nvarchar(200)
		, CompAdd		nvarchar(300)
		, MainCurr		nvarchar(100)
		, CardCode		nvarchar(50)
		, CardName		nvarchar(200)
		, CardAddr		nvarchar(200)
		, Comments		nvarchar(300)
		, Customer		nvarchar(200)
		, U_InvNo		nvarchar(100)
		, U_InvDate		nvarchar(100)
		, U_SoTK		nvarchar(50)
		, SoHopDong		nvarchar(100)
		, SeriNo		nvarchar(100)
		, DocDate		nvarchar(100)
		, POEntry		int
		, LineID		int
		, ItemCode		nvarchar(50)
		, ItemName		nvarchar(200)
		, ItemSpec		nvarchar(200)
		, WhsCode		nvarchar(100)
		, WhsName		nvarchar(500)
		, Unit			nvarchar(100)
		, OrderQty		numeric(21,6)
		, UnitPrice		numeric(21,6)
		, RecptQty		numeric(21,6)
        , QtyBal		numeric(21,6)
		, LineTotal		numeric(21,6)
		, DocTotal		numeric(21,6)
		, DocTotalFC	numeric(21,6)
		, DocRate		numeric(21,6)
		, DocCurr		nvarchar(10)
		, RefNo			nvarchar(50)
		, AmountInWord	nvarchar(400)
		, DocCurrName	nvarchar(50)
		, DecQty		int
		, DecPrice		int
		, DecAmt		int
        , DecAmtMain	int
		, DateSep		nvarchar(1)
		, ThousSep		nvarchar(1)
		, DateTitle		nvarchar(100)
		, Logo			nvarchar(100)
		, Note 			nvarchar(100)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CardCode,CardName,Customer,U_InvNo,SeriNo,U_InvDate,DocDate,PeriodTitle,LineID,ItemCode,ItemName,Unit,POEntry,OrderQty, RecptQty, Comments, CardAddr, UnitPrice, LineTotal, DocTotal, DocTotalFC, DocRate, WhsCode, DocCurr, RefNo,FormNo)
	select
		a."CardCode"
		,c."CardName"
		,c."CntctPrsn"
		,a."U_InvNo"
		,a."U_InvSerial"
		,to_char(a."U_InvDate",'DD/MM/YYYY')
		,CONCAT(CONCAT(CONCAT(CONCAT(CONCAT('Ngày ',to_char(b."DocDate",'DD')),N' tháng ') , to_char(b."DocDate",'MM')) , N' năm ' ), to_char(b."DocDate",'YYYY'))
		,CONCAT(CONCAT(CONCAT(CONCAT(CONCAT('Ngày ',to_char(b."DocDate",'DD')),N' tháng ') , to_char(b."DocDate",'MM')) , N' năm ' ), to_char(b."DocDate",'YYYY'))
		,0
		,b."ItemCode"
		,d."ItemName"
		,d."InvntryUom"
		,0
		,0
		,b."Quantity"
		,a."Comments"
		,''
		,b."Price"
		,case iFnull(a."DocCur", b."Currency") when (SELECT "MainCurncy" FROM "OADM") then b."LineTotal" else b."TotalFrgn" end
		,case when a."DocCur" = (SELECT "MainCurncy" FROM "OADM") then a."DocTotal" - a."VatSum" - IFNULL(a."TotalExpns",0) else a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC",0) end
		,CASE WHEN (SELECT x."DirectRate" FROM "OADM" x)='Y' then (a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC",0))*b."Rate" ELSE (a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC",0))/NULLIF(b."Rate",0) end
		,a."DocRate"
		,b."WhsCode"
		,a."DocCur"
		,''
		,'02 - VT'
	from
		"ORPD" a
		left join "RPD1" b on a."DocEntry" = b."DocEntry"
		left join "OCRD" c on c."CardCode" = a."CardCode"
		left join "OITM" d on d."ItemCode" = b."ItemCode"
	where 
		a."CANCELED" = 'N' and b."DocEntry" = DocID;
		
	UPDATE #tbl SET CardAddr = (SELECT IFNULL(x."StreetNo",'') FROM "OWHS" x WHERE x."WhsCode"=#tbl.WhsCode);

	UPDATE #tbl SET WhsName = (SELECT IFNULL(x."WhsName",'') FROM "OWHS" x WHERE x."WhsCode"=#tbl.WhsCode);

	update #tbl set ItemSpec = (select x."FrgnName" from "OITM" x where x."ItemCode"=#tbl.ItemCode);
	
	
	update #tbl set AmountInWord = Num2Text(to_integer(DocTotal));
	update #tbl set AmountInWord = CONCAT(AmountInWord,' ');
	update #tbl set AmountInWord = CONCAT(AmountInWord,IFNULL((select "CurrName" from "OCRN" where "CurrCode" = DocCurr),''));
	update #tbl set AmountInWord = CONCAT(AmountInWord,CASE when DocTotal = ROUND(DocTotal,0) then N' chẵn' else  CONCAT(CONCAT(CONCAT(' và ', LOWER(Num2Text(cast((DocTotal-ROUND(DocTotal-0.5,0))*100 as int)))),' '),IFNULL((select "Chk100Name" from "OCRN" where "CurrCode" = DocCurr),0)) end);
	
	update #tbl set DateTitle = 'Ngày.......tháng.......năm......./ Date.....................' ,RptName = 'PHIẾU TRẢ HÀNG /GOODS RETURN';
	
	select 2,a."DecSep",a."ThousSep",a."PriceDec" into DecQty,DateSep,ThousSep,DecPrice from "OADM" a;	
	update #tbl set DecQty = DecQty,DateSep = DateSep,ThousSep =ThousSep,DecPrice=DecPrice;

	update #tbl set DecAmt = case when (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.DocCurr)='-1' then (select a."SumDec" from "OADM" a) else (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.DocCurr) end;
	
	if bplID=0 then
		update #tbl set 
			CompName	=	(select a."CompnyName" from "OADM" a,"ADM1" b),
			CompAdd		=	(select b."Street" from "OADM" a,"ADM1" b);
	end if;
	if bplID<>0 then
		update #tbl set 
			CompName	=(select x."BPLName" from "OBPL" x where x."BPLId"=bplID),
			CompAdd		=(select x."Street" from "OBPL" x where x."BPLId"=bplID);
	end if;

	select 
		FormNo "FormNo",
		RptName "RptName",
		PeriodTitle "PeriodTitle",
		CompName "CompName",
		CompAdd "CompAdd",
		MainCurr "MainCurr",
		CardCode "CardCode",
		CardName "CardName",
		CardAddr "CardAddr",
		Comments "Comments",
		Customer "Customer",
		U_InvNo "U_InvNo",
		U_InvDate "U_InvDate",
		U_SoTK "U_SoTK",
		SoHopDong "SoHopDong",
		SeriNo "SeriNo",
		DocDate "DocDate",
		POEntry "POEntry",
		LineID "LineID",
		ItemCode "ItemCode",
		ItemName "ItemName",
		ItemSpec "ItemSpec",
		WhsCode "WhsCode",
		WhsName "WhsName",
		Unit "Unit",
		OrderQty "OrderQty",
		UnitPrice "UnitPrice",
		RecptQty "RecptQty",
		QtyBal "QtyBal",
		LineTotal "LineTotal",
		DocTotal "DocTotal",
		DocTotalFC "DocTotalFC",
		DocRate "DocRate",
		DocCurr "DocCurr",
		RefNo "RefNo",
		AmountInWord "AmountInWord",
		DocCurrName "DocCurrName",
		DecQty "DecQty",
		DecPrice "DecPrice",
		DecAmt "DecAmt",
		DecAmtMain "DecAmtMain",
		DateSep "DateSep",
		ThousSep "ThousSep",
		DateTitle "DateTitle",
		Logo "Logo",
		Note "Note"
	from #tbl;
	drop table #tbl;
end

