CREATE procedure VAS_F_PD_05 (
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
		FormNo 			nvarchar(50)
		, RptName 		nvarchar(200)
		, CompName		nvarchar(200)
		, CompAdd		nvarchar(300)
		, CompTel		nvarchar(50)
		, CompFax		nvarchar(50)
		, CompTaxId		nvarchar(50)
		, CardCode		nvarchar(50)
		, CardName		nvarchar(200)
		, CardAddr 		nvarchar(200)
		, Comments 		nvarchar(300)
		, CardAtn		nvarchar(200)
		, CardRefNo		nvarchar(100)
		, Model			nvarchar(100)
		, SeriNo		nvarchar(100)
		, DocDate		nvarchar(100)
		, ItemCode		nvarchar(50)
		, ItemName		nvarchar(200)
		, WhsName 		nvarchar(100)
		, Unit			nvarchar(100)
		, OrderQty		numeric(21,6)
		, UnitPrice 	numeric(21,6)
		, RecptQty		numeric(21,6)
		, LineTotal 	numeric(21,6)
		, DocTotal 		numeric(21,6)
		, DocTotalFC 	numeric(21,6)
		, DocRate 		numeric(21,6)
		, DocCurr 		nvarchar(10)
		, RefNo 		nvarchar(50)
		, AmountInWord 	nvarchar(400)
		, DocCurrName 	nvarchar(50)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	
	insert into #tbl (CompName,CompAdd,CompTel,CompFax,CompTaxId,CardCode,CardName,CardAtn,CardRefNo,Model,SeriNo,DocDate,ItemCode,ItemName,Unit,OrderQty, RecptQty, Comments, CardAddr, UnitPrice, LineTotal, DocTotal, DocTotalFC, DocRate, WhsName, DocCurr, RefNo)
	select
		UPPER(a."CompnyName")
		, n."Street"
		, IFNULL(a."Phone1",a."Phone2")
		, a."Fax"
		, IFNULL(a."TaxIdNum",IFNULL(a."TaxIdNum2",a."TaxIdNum3"))
		, d."CardCode"
		, d."CardName"
		, e."Name"
		, b."NumAtCard"
		, f."OcrName"
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
		CompName "CompName",
		CompAdd "CompAdd",
		CompTel "CompTel",
		CompFax "CompFax",
		CompTaxId "CompTaxId",
		CardCode "CardCode",
		CardName "CardName",
		CardAddr "CardAddr",
		Comments "Comments",
		CardAtn "CardAtn",
		CardRefNo "CardRefNo",
		Model "Model",
		SeriNo "SeriNo",
		DocDate "DocDate",
		ItemCode "ItemCode",
		ItemName "ItemName",
		WhsName "WhsName",
		Unit "Unit",
		OrderQty "OrderQty",
		UnitPrice "UnitPrice",
		RecptQty "RecptQty",
		LineTotal "LineTotal",
		DocTotal "DocTotal",
		DocTotalFC "DocTotalFC",
		DocRate "DocRate",
		DocCurr "DocCurr",
		RefNo "RefNo",
		AmountInWord "AmountInWord",
		DocCurrName "DocCurrName"
	from #tbl;

	drop table #tbl;
end

