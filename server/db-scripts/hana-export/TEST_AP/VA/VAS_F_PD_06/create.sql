CREATE procedure VAS_F_PD_06 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	
	select ifnull(x."BPLId",0) into bplID from "OPDN" x where x."DocEntry"=DocId;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		FormNo			nvarchar(50)
		, RptName		nvarchar(200)
		, PeriodTitle	nvarchar(100)
		, CompName		nvarchar(200)
		, CompAdd		nvarchar(300)
		, CompTel		nvarchar(50)
		, CompFax		nvarchar(50)
		, CompTaxId		nvarchar(50)
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
	
	insert into #tbl (PeriodTitle,CompName,CompAdd,CompTel,CompFax,CompTaxId,CardCode,CardName,SeriNo,DocDate,ItemCode,ItemName,Unit,OrderQty, RecptQty, Comments, CardAddr, UnitPrice, LineTotal, DocTotal, DocTotalFC, DocRate, WhsName, DocCurr, RefNo)
	select
		CONCAT('Ngày ',to_char(b."DocDate",'DD/MM/YYYY'))
		, UPPER(a."CompnyName")
		, n."Street"
		, IFNULL(a."Phone1",a."Phone2")
		, a."Fax"
		, IFNULL(a."TaxIdNum",IFNULL(a."TaxIdNum2",a."TaxIdNum3"))
		, d."CardCode"
		, d."CardName"
		, b."U_JVNumber"
		, CONCAT('Ngày/Date ',to_char(b."DocDate",'DD/MM/YYYY'))
		, c."ItemCode"
		, c."Dscription"
		, g."InvntryUom"
		, m."Quantity"
		, c."Quantity"
		, b."Comments"
		, d."Address"
		, c."Price"
		, case ifnull(b."DocCur", c."Currency") when 'VND' then c."LineTotal" else c."TotalFrgn" end
		, case when b."DocCur" = 'VND' then b."DocTotal" - b."VatSum" else b."DocTotalFC" - b."VatSumFC" end
		, b."DocTotal" - b."VatSum"
		, b."DocRate"
		, l."WhsName"
		, b."DocCur"
		, b."Ref2"
	from
		"OPDN" b
		left join "OADM" a on a."CompnyName" is not null
		left join "PDN1" c on b."DocEntry" = c."DocEntry"
		left join "OCRD" d on b."CardCode" = d."CardCode"
		left join "OCPR" e on b."CntctCode" = e."CntctCode"
		left join "OOCR" f on f."DimCode" = 3 and c."OcrCode" = f."OcrCode"
		left join "OITM" g on c."ItemCode" = g."ItemCode"
		left join "OWHS" l on l."WhsCode" = c."WhsCode"
		left join "POR1" m on c."BaseType" = m."ObjType" and c."BaseEntry" = m."DocEntry" and c."BaseLine" = m."LineNum"
		left join "OBPL" n on n."BPLId"=b."BPLId"
	where b."CANCELED" = 'N' and b."DocEntry" = DocID;

	
	update #tbl set RptName = N'PHIẾU NHẬP KHO/GOODS RECEIPT PO';

	update #tbl set AmountInWord = Num2Text(to_integer(DocTotal));
	update #tbl set AmountInWord = CONCAT(AmountInWord,' ');
	update #tbl set AmountInWord = CONCAT(AmountInWord,IFNULL((select "CurrName" from "OCRN" where "CurrCode" = DocCurr),''));

	
	select 
		FormNo "FormNo",
		RptName "RptName",
		PeriodTitle "PeriodTitle",
		CompName "CompName",
		CompAdd "CompAdd",
		CompTel "CompTel",
		CompFax "CompFax",
		CompTaxId "CompTaxId",
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

