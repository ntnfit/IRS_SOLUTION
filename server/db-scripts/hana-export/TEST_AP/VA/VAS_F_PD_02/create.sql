CREATE procedure VAS_F_PD_02 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	
	select ifnull(x."BPLId",0) into bplID from "OPDN" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		FormNo			nvarchar(50)
		, RptName		nvarchar(200)
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
		, SeriNo		nvarchar(100)
		, DocDate		nvarchar(100)
		, POEntry		int
		, LineID		int
		, ItemCode		nvarchar(50)
		, ItemName		nvarchar(200)
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
		, DecAmt		int
        , DecAmtMain	int
		, DateSep		nvarchar(1)
		, ThousSep		nvarchar(1)
		, DateTitle		nvarchar(100)
		, Logo			nvarchar(100)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CardCode,CardName,Customer,U_InvNo,SeriNo,U_InvDate,DocDate,LineID,ItemCode,ItemName,Unit,POEntry,OrderQty, RecptQty, Comments, CardAddr, UnitPrice, LineTotal, DocTotal, DocTotalFC, DocRate, WhsCode, DocCurr, RefNo,FormNo)
	select
		a."CardCode"
		,c."CardName"
		,c."CntctPrsn"
		,CASE WHEN IFNULL(e."U_JVNumber",'')='' then a."U_InvNo" ELSE e."U_JVNumber" end
		,a."U_JVNumber"
		,to_char(a."U_InvDate",'DD/MM/YYYY')
		,to_char(a."DocDate",'DD/MM/YYYY')
		,e."LineNum"
		,b."ItemCode"
		,d."ItemName"
		,d."InvntryUom"
		,e.POEntry
		,e."Quantity"
		,b."Quantity"
		,a."Comments"
		,''
		,b."Price"
		,case ifnull(a."DocCur", b."Currency") when (SELECT "MainCurncy" FROM "OADM") then b."LineTotal" else b."TotalFrgn" end
		,case when a."DocCur" = (SELECT "MainCurncy" FROM "OADM") then a."DocTotal" - a."VatSum" - IFNULL(a."TotalExpns",0) else a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC",0) end
		,CASE WHEN (SELECT x."DirectRate" FROM "OADM" x)='Y' then (a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC",0))*b."Rate" ELSE (a."DocTotalFC" - a."VatSumFC" - IFNULL(a."TotalExpFC",0))/NULLIF(b."Rate",0) end
		,a."DocRate"
		,b."WhsCode"
		,a."DocCur"
		,''
		,'01 - VT'
	from
		"OPDN" a
		left join "PDN1" b on a."DocEntry" = b."DocEntry"
		left join "OCRD" c on c."CardCode" = a."CardCode"
		left join "OITM" d on d."ItemCode" = b."ItemCode"
		LEFT JOIN (
			SELECT a."DocEntry",a."U_JVNumber",b."ItemCode",b."LineNum",b."Quantity",b."ObjType" ,a."DocEntry" as POEntry FROM "OPOR" a,"POR1" b WHERE a."DocEntry"=b."DocEntry" AND a."CANCELED"='N'
			UNION 
            SELECT b."DocEntry",d."U_JVNumber",b."ItemCode",b."LineNum",SUM(c."Quantity"),b."ObjType" ,d."DocEntry" as POEntry
			FROM "OPCH" a
				LEFT JOIN "PCH1" b ON a."DocEntry"=b."DocEntry"
				LEFT JOIN "POR1" c ON b."BaseEntry"=c."DocEntry" AND b."BaseLine"=c."LineNum" AND b."BaseType"=c."ObjType" AND c."ItemCode"=b."ItemCode"
				LEFT JOIN "OPOR" d ON d."DocEntry"=c."DocEntry" AND d."CANCELED"='N'
			WHERE a."CANCELED"='N'
			GROUP BY b."DocEntry",b."ItemCode",b."LineNum",b."ObjType",d."U_JVNumber",d."DocEntry"
		) e ON e."LineNum"=b."BaseLine" AND e."DocEntry"=b."BaseEntry" AND e."ObjType"=b."BaseType" AND e."ItemCode"=b."ItemCode"
	where 
		a."CANCELED" = 'N' and b."DocEntry" = DocID;

	UPDATE #tbl SET QtyBal = ifnull((select SUM(b."Quantity")-IFNULL(SUM(c.Quantity),0) from 
												"OPDN" a
												LEFT join "PDN1" b ON a."DocEntry"=b."DocEntry"
												LEFT JOIN (SELECT a."DocEntry",a."ObjType",b."BaseEntry",b."BaseLine",b."BaseType",b."ItemCode",SUM(b."Quantity") Quantity FROM "ORPD" a,"RPD1" b WHERE a."DocEntry"=b."DocEntry" AND a."CANCELED"='N' GROUP BY a."DocEntry",a."ObjType",b."BaseEntry",b."BaseLine",b."BaseType",b."ItemCode"
												) c ON c."ItemCode"=b."ItemCode" AND c."BaseEntry"=b."DocEntry" AND c."BaseLine"=b."LineNum" AND c."BaseType"=b."ObjType"
											WHERE a."DocEntry"=b."DocEntry" and b."BaseType"='22' and b."BaseEntry"=POEntry AND b."BaseLine"=#tbl.LineID and b."ItemCode"=#tbl.ItemCode and a."DocEntry"<=DocID and a."CANCELED"='N'),0)
							+ ifnull((select sum(b."Quantity")-IFNULL(SUM(e.Quantity),0) from "OPDN" a
												left join "PDN1" b on a."DocEntry"=b."DocEntry" and b."BaseType"='18'
												left join "PCH1" c on c."DocEntry"=b."BaseEntry" and c."LineNum"=b."BaseLine" and c."BaseType"='22'
												left join "OPCH" d on c."DocEntry"=d."DocEntry" and d."CANCELED"='N'
												LEFT JOIN (SELECT a."DocEntry",a."ObjType",b."BaseEntry",b."BaseLine",b."BaseType",b."ItemCode",SUM(b."Quantity") Quantity FROM "ORPD" a,"RPD1" b WHERE a."DocEntry"=b."DocEntry" AND a."CANCELED"='N' GROUP BY a."DocEntry",a."ObjType",b."BaseEntry",b."BaseLine",b."BaseType",b."ItemCode"
												) e ON e."ItemCode"=b."ItemCode" AND e."BaseEntry"=b."DocEntry" AND e."BaseLine"=b."LineNum" AND e."BaseType"=b."ObjType"
											 where a."CANCELED"='N' and b."ItemCode"=#tbl.ItemCode and c."BaseEntry"=POEntry AND b."BaseLine"=#tbl.LineID AND a."DocEntry"<=DocID),0);


	update #tbl set WhsName = (select a."WhsName" from "OWHS" a where a."WhsCode"=#tbl.WhsCode);
	
	UPDATE #tbl SET Customer = (SELECT a."CntctPrsn" FROM "OCRD" a, "OCPR" b WHERE a."CardCode"=#tbl.CardCode AND a."CntctPrsn"=b."Name" AND a."CardCode"=b."CardCode");

	update #tbl set RptName = N'PHIẾU NHẬP KHO/ GOODS RECEIPT PO';
	
	update #tbl set DateTitle = N'Ngày.......tháng.......năm......./ Date.....................';
	update #tbl set AmountInWord = Num2Text(to_integer(DocTotal));

	select a."QtyDec",a."DecSep",a."ThousSep",a."PriceDec" into DecQty,DateSep,ThousSep,DecPrice from "OADM" a;
	
	update #tbl set DecQty = DecQty, DateSep = DateSep,ThousSep = ThousSep, DecPrice=DecPrice;

	update #tbl set DecAmt = case when (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.DocCurr)='-1' then (select a."SumDec" from "OADM" a) else (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.DocCurr) end;
	
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
		FormNo "FormNo",RptName "RptName",CompName "CompName",CompAdd "CompAdd",MainCurr "MainCurr",CardCode "CardCode",CardName "CardName",CardAddr "CardAddr",Comments "Comments",Customer "Customer",U_InvNo "U_InvNo",U_InvDate "U_InvDate", 
		SeriNo "SeriNo",DocDate "DocDate",POEntry "POEntry",LineID "LineID",ItemCode "ItemCode",ItemName "ItemName"
		,WhsCode "WhsCode"
		,WhsName "WhsName"
		,Unit "Unit"
		,OrderQty "OrderQty"
		,UnitPrice "UnitPrice"
		,RecptQty "RecptQty"
		,QtyBal "QtyBal"
		,LineTotal "LineTotal"
		,DocTotal "DocTotal"
		,DocTotalFC "DocTotalFC"
		,DocRate "DocRate"
		,DocCurr "DocCurr"
		,RefNo "RefNo"
		,AmountInWord "AmountInWord"
		,DocCurrName "DocCurrName"
		,DecQty "DecQty"
		,DecPrice "DecPrice"
		,DecAmt "DecAmt"
		,DecAmtMain "DecAmtMain"
		,DateSep "DateSep"
		,ThousSep "ThousSep"
		,DateTitle "DateTitle"
		,Logo "Logo"

 	from #tbl;
	
	drop table #tbl;
end

