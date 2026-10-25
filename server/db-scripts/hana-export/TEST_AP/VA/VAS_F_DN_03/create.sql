CREATE procedure VAS_F_DN_03 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	
	select ifnull(x."BPLId",0) into bplID from "ODLN" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		FormNo			nvarchar(50)
		, RptName		nvarchar(200)
		, CompName		nvarchar(200)
		, CompAdd		nvarchar(300)
		, MainCurr		nvarchar(100)
		, CardCode		nvarchar(50)
		, CardName		nvarchar(200)
		, CardAddr		nvarchar(200)
		, Comments		nvarchar(300)
		, Customer		nvarchar(200)
		, U_InvNo		nvarchar(100)
		, U_InvDate		NVARCHAR(100)
		, U_JVNumber	nvarchar(100)
		, DocDate		nvarchar(100)
		, POEntry		int
		, LineID		int
		, ItemCode		nvarchar(50)
		, ItemName		nvarchar(200)
		, WhsCode		nvarchar(100)
		, WhsName		nvarchar(500)
		, Unit			nvarchar(100)
		, OrderQty		numeric(21,6)
		, UnitPrice		numeric(21,6)
		, DelQty		numeric(21,6)
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
	);

	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CardCode,CardName,Customer,U_InvNo,U_JVNumber,U_InvDate,DocDate,LineID,ItemCode,ItemName,Unit,POEntry,OrderQty, DelQty, Comments, CardAddr, UnitPrice, LineTotal, DocTotal, DocTotalFC, DocRate, WhsCode, DocCurr, RefNo,FormNo)
	SELECT 
		a."CardCode", 
		c."CardName", 
		c."CntctPrsn", 
		CASE WHEN IFNULL(e."U_JVNumber", '') = '' THEN a."U_InvNo" ELSE e."U_JVNumber" END, 
		a."U_JVNumber", 
		to_char(a."U_InvDate",'DD/MM/YYYY'), 
		to_char(a."DocDate",'DD/MM/YYYY'), 
		e."LineNum", 
		b."ItemCode", 
		d."ItemName", 
		d."InvntryUom", 
		e."POEntry", 
		e."Quantity", 
		b."Quantity", 
		a."Comments", 
		'', 
		b."Price", 
		CASE IFNULL(a."DocCur", b."Currency") WHEN (SELECT "MainCurncy" FROM "OADM") THEN b."LineTotal" ELSE b."TotalFrgn" END, 
		CASE WHEN a."DocCur" = (SELECT "MainCurncy" FROM "OADM") THEN a."DocTotal" - a."VatSum" - IFNULL(a."TotalExpns", 0) ELSE a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC", 0) END, 
		CASE WHEN (SELECT x."DirectRate" FROM "OADM" x) = 'Y' THEN (a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC", 0)) * b."Rate" ELSE (a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC", 0)) / NULLIF(b."Rate", 0) END, 
		a."DocRate", 
		b."WhsCode", 
		a."DocCur", 
		'', 
		'01 - VT' 
	FROM "ODLN" a 
	LEFT OUTER JOIN "DLN1" b ON a."DocEntry" = b."DocEntry" 
	LEFT OUTER JOIN "OCRD" c ON c."CardCode" = a."CardCode" 
	LEFT OUTER JOIN "OITM" d ON d."ItemCode" = b."ItemCode" 
	LEFT OUTER JOIN 
		(SELECT a."DocEntry", a."U_JVNumber", b."ItemCode", b."LineNum", b."Quantity", b."ObjType", a."DocEntry" AS "POEntry" FROM "ORDR" a, "RDR1" b WHERE a."DocEntry" = b."DocEntry" AND a."CANCELED" = 'N' 
		 UNION ALL 
		 SELECT b."DocEntry", d."U_JVNumber", b."ItemCode", b."LineNum", SUM(c."Quantity"), b."ObjType", d."DocEntry" 
		 FROM "OINV" a
		 LEFT OUTER JOIN "INV1" b ON (a."DocEntry" = b."DocEntry") 
		 LEFT OUTER JOIN "RDR1" c ON (b."BaseEntry" = c."DocEntry" AND b."BaseLine" = c."LineNum" AND b."BaseType" = c."ObjType" AND c."ItemCode" = b."ItemCode") 
		 LEFT OUTER JOIN "ORDR" d ON (d."DocEntry" = c."DocEntry" AND d."CANCELED" = 'N') 
		 WHERE a."CANCELED" = 'N' 
		 GROUP BY b."DocEntry", b."ItemCode", b."LineNum", b."ObjType", d."U_JVNumber", d."DocEntry") 
		 AS e ON (e."LineNum" = b."BaseLine" AND e."DocEntry" = b."BaseEntry" AND e."ObjType" = b."BaseType" AND e."ItemCode" = b."ItemCode") 
	WHERE a."CANCELED" = 'N' AND b."DocEntry" = DocID;

	
	UPDATE #tbl SET QtyBal = IFNULL((SELECT SUM(b."Quantity") - IFNULL(SUM(c."Quantity"), 0) 
										FROM "ODLN" a 
										LEFT OUTER JOIN "DLN1" b ON a."DocEntry" = b."DocEntry" 
										LEFT OUTER JOIN (	SELECT a."DocEntry", a."ObjType", b."BaseEntry", b."BaseLine", b."BaseType", b."ItemCode", SUM(b."Quantity") AS "Quantity" 
															FROM "ORDN" a, "RDN1" b 
															WHERE a."DocEntry" = b."DocEntry" AND a."CANCELED" = 'N' 
															GROUP BY a."DocEntry", a."ObjType", b."BaseEntry", b."BaseLine", b."BaseType", b."ItemCode"
														) AS c ON (c."ItemCode" = b."ItemCode" AND c."BaseEntry" = b."DocEntry" AND c."BaseLine" = b."LineNum" AND c."BaseType" = b."ObjType") 
										WHERE a."DocEntry" = b."DocEntry" AND b."BaseType" = '17' AND b."BaseEntry" = POEntry AND b."BaseLine" = #tbl.LineID AND b."ItemCode" = #tbl.ItemCode AND a."DocEntry" <= DocID AND a."CANCELED" = 'N'), 0) 
							  + IFNULL((SELECT SUM(b."Quantity") - IFNULL(SUM(e."Quantity"), 0) 
							  			FROM "ODLN" a 
							  			LEFT OUTER JOIN "DLN1" b ON (a."DocEntry" = b."DocEntry" AND b."BaseType" = '13') 
							  			LEFT OUTER JOIN "INV1" c ON (c."DocEntry" = b."BaseEntry" AND c."LineNum" = b."BaseLine" AND c."BaseType" = '17') 
							  			LEFT OUTER JOIN "OINV" d ON (c."DocEntry" = d."DocEntry" AND d."CANCELED" = 'N') 
							  			LEFT OUTER JOIN (	SELECT a."DocEntry", a."ObjType", b."BaseEntry", b."BaseLine", b."BaseType", b."ItemCode", SUM(b."Quantity") AS "Quantity" 
							  								FROM "ORDN" a, "RDN1" b 
							  								WHERE a."DocEntry" = b."DocEntry" AND a."CANCELED" = 'N' 
							  								GROUP BY a."DocEntry", a."ObjType", b."BaseEntry", b."BaseLine", b."BaseType", b."ItemCode"
							  							 ) AS e ON (e."ItemCode" = b."ItemCode" AND e."BaseEntry" = b."DocEntry" AND e."BaseLine" = b."LineNum" AND e."BaseType" = b."ObjType") 
							  			WHERE a."CANCELED" = 'N' AND b."ItemCode" = #tbl.ItemCode AND /*d."IsIns" = 'Y' AND*/ c."BaseEntry" = POEntry AND b."BaseLine" = #tbl.LineID AND a."DocEntry" <= DocID), 0);	

	update #tbl set WhsName = (select a."WhsName" from "OWHS" a where a."WhsCode"=#tbl.WhsCode);
	
	UPDATE #tbl SET Customer = (SELECT a."CntctPrsn" FROM "OCRD" a, "OCPR" b WHERE a."CardCode"=#tbl.CardCode AND a."CntctPrsn"=b."Name" AND a."CardCode"=b."CardCode");

	update #tbl set RptName = N'PHIẾU XUẤT KHO/DELIVERY NOTE';
	
	update #tbl set DateTitle = N'Ngày.......tháng.......năm......./ Date.....................';
	update #tbl set AmountInWord = Num2Text(to_integer(DocTotal));

	select a."QtyDec",a."DecSep",a."ThousSep",a."PriceDec" into DecQty,DateSep,ThousSep,DecPrice from "OADM" a;
	
	update #tbl set DecQty = DecQty, DateSep = DateSep,ThousSep = ThousSep, DecPrice=DecPrice;
	update #tbl set DecQty = case when floor(DelQty)<>DelQty then 2 else 0 end;
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
		U_JVNumber  "U_JVNumber",
		DocDate  "DocDate",
		POEntry  "POEntry",
		LineID  "LineID",
		ItemCode  "ItemCode",
		ItemName  "ItemName",
		WhsCode  "WhsCode",
		WhsName  "WhsName",
		Unit  "Unit",
		OrderQty  "OrderQty",
		UnitPrice  "UnitPrice",
		DelQty  "DelQty",
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
		Logo  "Logo"

	from #tbl;

	drop table #tbl;

end

