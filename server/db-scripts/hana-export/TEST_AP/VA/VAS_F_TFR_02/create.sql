CREATE procedure VAS_F_TFR_02 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(300);
	
	select ifnull(x."BPLId",0) into bplID from "OWTQ" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		FormNo			nvarchar(50)
		, RptName		nvarchar(200)
		, PeriodTitle	nvarchar(100)
		, CompName		nvarchar(200)
		, CompAdd		nvarchar(300)
		, MainCurr		NVARCHAR(100)
		, CardCode		nvarchar(50)
		, CardName		nvarchar(200)
		, CardAddr		nvarchar(200)
		, Comments		nvarchar(300)
		, Customer		nvarchar(200)
		, U_InvNo		nvarchar(100)
		, U_InvDate		NVARCHAR(100)
		, U_SoTK		nvarchar(50)
		, SoHopDong		nvarchar(100)
		, SeriNo		nvarchar(100)
		, DocDate		nvarchar(100)
		, POEntry		INT
		, LineID		INT
		, ItemCode		nvarchar(50)
		, ItemName		nvarchar(200)
		, ItemSpec		nvarchar(200)
		, FromWhsCode 	nvarchar(120)
		, ToWhsCode 	nvarchar(120)
		, WhsCode		NVARCHAR(100)
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
		, DecAmt		INT
        , DecAmtMain	INT
		, DateSep		nvarchar(1)
		, ThousSep		nvarchar(1)
		, DateTitle		nvarchar(100)
		, Logo			nvarchar(100)
		, Note 			nvarchar(100)
	);	

	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CardCode,CardName,Customer,U_InvNo,SeriNo,U_InvDate,DocDate,PeriodTitle,LineID,ItemCode,ItemName,Unit,POEntry,OrderQty, RecptQty, Comments, CardAddr, UnitPrice, LineTotal, DocTotal, DocTotalFC, DocRate, WhsCode, DocCurr, RefNo,FormNo,FromWhsCode,ToWhsCode)
	SELECT 
		a."CardCode", c."CardName", c."CntctPrsn", a."U_InvNo", a."U_InvSerial", 
		to_char(a."U_InvDate",'DD/MM/YYYY'), 
		to_char(a."DocDate", 'DD/MM/YYYY'), 
		'Ngày ' || to_char(a."DocDate", 'DD') || ' tháng ' || to_char(a."DocDate", 'MM') || ' năm ' || to_char(a."DocDate", 'YYYY'), 0, 
		b."ItemCode", d."ItemName", d."InvntryUom", 0, 0, b."Quantity", a."Comments", '', b."Price", 
		CASE IFNULL(a."DocCur", b."Currency") WHEN (SELECT "MainCurncy" FROM "OADM") THEN b."LineTotal" ELSE b."TotalFrgn" END, 
		CASE WHEN a."DocCur" = (SELECT "MainCurncy" FROM "OADM") THEN a."DocTotal" - a."VatSum" - IFNULL(a."TotalExpns", 0) ELSE a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC", 0) END, 
		CASE WHEN (SELECT x."DirectRate" FROM OADM x) = 'Y' THEN (a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC", 0)) * b."Rate" ELSE (a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC", 0)) / NULLIF(b."Rate", 0) END, 
		a."DocRate", b."WhsCode", a."DocCur", '', '01 - VT', b."FromWhsCod", b."WhsCode" 
	FROM "OWTQ" a 
	LEFT OUTER JOIN "WTQ1" b ON a."DocEntry" = b."DocEntry" 
	LEFT OUTER JOIN "OCRD" c ON c."CardCode" = a."CardCode" 
	LEFT OUTER JOIN "OITM" d ON d."ItemCode" = b."ItemCode" 
	WHERE a."CANCELED" = 'N' AND b."DocEntry" = DocID;




	update #tbl set WhsName = (select x."WhsName" from "OWHS" x where x."WhsCode"=#tbl.WhsCode);
	
	update #tbl set DateTitle = N'Ngày.......tháng.......năm......./ Date.....................',RptName = N'ĐỀ NGHỊ CHUYỂN KHO/INVENTORY TRANSFER REQUEST';


	select x."CompnyName",x."CompnyAddr",x."Phone1",x."Fax",y."IntrntAdrs",x."E_Mail" into tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail from OADM x, ADM1 y ;
	
	update #tbl set CompName = tCompName, CompAdd = tCompAddr;--, CompTel = tCompTel, CompFax = tCompFax, CompWebsite = tCompWebsite, CompEmail = tCompEmail;
	

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
		FromWhsCode  "FromWhsCode",
		ToWhsCode  "ToWhsCode",
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

