CREATE procedure VAS_F_RT_01 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	
	select ifnull(x."BPLId",0) into bplID from "ORDN" x where x."DocEntry"=DocID;
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
		, Reason		nvarchar(100)
		, Comments		nvarchar(300)
		, Customer		nvarchar(200)
		, U_InvNo		nvarchar(100)
		, U_InvDate		NVARCHAR(100)
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
	insert into #tbl (CardCode,CardName,Customer,U_InvNo,SeriNo,U_InvDate,DocDate,PeriodTitle,LineID,ItemCode,ItemName,ItemSpec,Unit,POEntry,OrderQty, RecptQty, Comments, CardAddr, UnitPrice, LineTotal, DocTotal, DocTotalFC, DocRate, WhsCode, DocCurr, RefNo,FormNo)
	SELECT 
		a."CardCode", c."CardName", c."CntctPrsn", 
		a."U_JVNumber", 
		a."U_JVNumber", 
		to_char(a."U_InvDate",'DD/MM/YYYY'), 
		to_char(a."DocDate", 'DD/MM/YYYY'), 
		'Ng�y ' || to_char(a."DocDate", 'DD') || ' n?m ' || to_char(a."DocDate", 'MM') || ' n?m ' || to_char(a."DocDate", 'YYYY'), 
		b."LineNum", b."ItemCode", d."ItemName",d."FrgnName", d."InvntryUom", 0, b."Quantity", b."Quantity", a."Comments", '', b."Price", 
		CASE IFNULL(a."DocCur", b."Currency") WHEN (SELECT "MainCurncy" FROM "OADM") THEN b."LineTotal" ELSE b."TotalFrgn" END, 
		CASE WHEN a."DocCur" = (SELECT "MainCurncy" FROM "OADM") THEN a."DocTotal" - a."VatSum" - IFNULL(a."TotalExpns", 0) ELSE a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC", 0) END, 
		CASE WHEN (SELECT x."DirectRate" FROM "OADM" x) = 'Y' THEN (a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC", 0)) * b."Rate" ELSE (a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC", 0)) / NULLIF(b."Rate", 0) END, 
		a."DocRate", b."WhsCode", a."DocCur", '', '01 - VT' 
	FROM "ORDN" a 
	LEFT OUTER JOIN "RDN1" b ON a."DocEntry" = b."DocEntry" 
	LEFT OUTER JOIN "OCRD" c ON c."CardCode" = a."CardCode" 
	LEFT OUTER JOIN "OITM" d ON d."ItemCode" = b."ItemCode" 
	WHERE a."CANCELED" = 'N' AND b."DocEntry" = DocID;


	update #tbl set WhsName = (select a."WhsName" from "OWHS" a where a."WhsCode"=#tbl.WhsCode);
	
	UPDATE #tbl SET Customer = (SELECT a."CntctPrsn" FROM "OCRD" a, "OCPR" b WHERE a."CardCode"=#tbl.CardCode AND a."CntctPrsn"=b."Name" AND a."CardCode"=b."CardCode");

	update #tbl set RptName = N'PHI?U NH?N H�NG TR? L?I/ RETURN';
	
	update #tbl set DateTitle = N'Ng�y.......th�ng.......n?m......./ Date.....................';
	update #tbl set AmountInWord = Num2Text(to_integer(DocTotal));

	select a."QtyDec",a."DecSep",a."ThousSep",a."PriceDec" into DecQty,DateSep,ThousSep,DecPrice from "OADM" a;
	
	update #tbl set DecQty = DecQty, DateSep = DateSep,ThousSep = ThousSep, DecPrice=DecPrice;
	update #tbl set DecQty = case when floor(RecptQty)<>RecptQty then 2 else 0 end;
	update #tbl set DecPrice = case when floor(UnitPrice)<>UnitPrice then 2 else 0 end;
	update #tbl set DecAmt = case when floor(LineTotal)<>LineTotal then 2 else 0 end;
	
	
	if bplID=0 then
		update #tbl set 
			CompName 	= (select a."CompnyName" from "OADM" a,"ADM1" b),
			CompAdd 	= (select b."Street" from "OADM" a,"ADM1" b);
	end if;
	if bplID<>0 then
		update #tbl set 
			CompName	= (select x."BPLName" from "OBPL" x where x."BPLId"=bplID),
			CompAdd		= (select x."Street" from "OBPL" x where x."BPLId"=bplID);
	end if;	

	select 
		FormNo  "FormNo",
		RptName  "RptName",
		PeriodTitle  "PeriodTitle",
		CompName  "CompName",
		CompAdd  "CompAdd",
		MainCurr  "MainCurr",
		CardCode  "CardCode",
		CardName  "CardName",
		CardAddr  "CardAddr",
		Reason  "Reason",
		Comments  "Comments",
		Customer  "Customer",
		U_InvNo  "U_InvNo",
		U_InvDate  "U_InvDate",
		U_SoTK  "U_SoTK",
		SoHopDong  "SoHopDong",
		SeriNo  "SeriNo",
		DocDate  "DocDate",
		POEntry  "POEntry",
		LineID  "LineID",
		ItemCode  "ItemCode",
		ItemName  "ItemName",
		ItemSpec  "ItemSpec",
		WhsCode  "WhsCode",
		WhsName  "WhsName",
		Unit  "Unit",
		OrderQty  "OrderQty",
		UnitPrice  "UnitPrice",
		RecptQty  "RecptQty",
		QtyBal  "QtyBal",
		LineTotal  "LineTotal",
		DocTotal  "DocTotal",
		DocTotalFC  "DocTotalFC",
		DocRate  "DocRate",
		DocCurr  "DocCurr",
		RefNo  "RefNo",
		AmountInWord  "AmountInWord",
		DocCurrName  "DocCurrName",
		DecQty  "DecQty",
		DecPrice  "DecPrice",
		DecAmt  "DecAmt",
		DecAmtMain  "DecAmtMain",
		DateSep  "DateSep",
		ThousSep  "ThousSep",
		DateTitle  "DateTitle",
		Logo  "Logo",
		Note  "Note"
	from #tbl;

	drop table #tbl;
end

