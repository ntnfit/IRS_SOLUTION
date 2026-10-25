CREATE procedure VAS_F_DN_05 (
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
		FormNo 		nvarchar(50)
		, RptName 	nvarchar(200)
		, CompName	nvarchar(200)
		, CompAdd	nvarchar(300)
		, CompTel	nvarchar(50)
		, CompFax	nvarchar(50)
		, CompTaxId	nvarchar(50)
		, CardCode	nvarchar(50)
		, CardName	nvarchar(200)
		, CardAddr 	nvarchar(200)
		, Comments 	nvarchar(300)
		, CardAtn	nvarchar(200)
		, CardRefNo	nvarchar(100)
		, Model		nvarchar(100)
		, SeriNo	nvarchar(100)
		, DocDate	nvarchar(100)
		, ItemCode	nvarchar(50)
		, ItemName	nvarchar(200)
		, WhsName 	nvarchar(100)
		, Unit		nvarchar(100)
		, DelQty	numeric(21,6)
		, UnitPrice numeric(21,6)
		, RecQty	numeric(21,6)
		, LineTotal numeric(21,6)
		, DocTotal 	numeric(21,6)
		, DocTotalFC numeric(21,6)
		, DocRate 	numeric(21,6)
		, DocCurr 	nvarchar(10)
		, RefNo 	nvarchar(50)
		, AmountInWord nvarchar(400)
		, DocCurrName nvarchar(50)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CompName,CompAdd,CompTel,CompFax,CompTaxId,CardCode,CardName,CardAtn,CardRefNo,Model,SeriNo,DocDate,ItemCode,ItemName,Unit,DelQty, RecQty, Comments, CardAddr, UnitPrice, LineTotal, DocTotal, DocTotalFC, DocRate, WhsName, DocCurr, RefNo)
	SELECT 
		UPPER(a."CompnyName"), 
		REPLACE(a."CompnyAddr", 'VIETNAM', ''), 
		IFNULL(a."Phone1", a."Phone2"), 
		a."Fax", 
		IFNULL(a."TaxIdNum", 
		IFNULL(a."TaxIdNum2", a."TaxIdNum3")), 
		d."CardCode", d."CardName", e."Name", b."NumAtCard", f."OcrName", b."U_JVNumber",
		'Ngày/Date '|| to_char(b."DocDueDate",'DD') || ' tháng/month ' || to_char(b."DocDueDate",'MM') || ' năm/year ' || to_char(b."DocDueDate",'YYYY'), 
		c."ItemCode", c."Dscription", g."InvntryUom", c."Quantity", m."Quantity", b."Comments", d."Address", c."Price", 
		CASE IFNULL(b."DocCur", c."Currency") WHEN 'VND' THEN c."LineTotal" ELSE c."TotalFrgn" END, 
		CASE WHEN b."DocCur" = 'VND' THEN b."DocTotal" - b."VatSum" ELSE b."DocTotalFC" - b."VatSumFC" END, 
		b."DocTotalFC" - b."VatSumFC", b."DocRate", l."WhsName", b."DocCur", b."Ref2" 
	FROM "OADM" a 
	LEFT OUTER JOIN "ODLN" b ON b."DocEntry" = DocID 
	LEFT OUTER JOIN "DLN1" c ON b."DocEntry" = c."DocEntry" 
	LEFT OUTER JOIN "OCRD" d ON b."CardCode" = d."CardCode" 
	LEFT OUTER JOIN "OCPR" e ON b."CntctCode" = e."CntctCode" 
	LEFT OUTER JOIN "OOCR" f ON (f."DimCode" = 1 and c."OcrCode" = f."OcrCode") 
	LEFT OUTER JOIN "OITM" g ON c."ItemCode" = g."ItemCode" 
	LEFT OUTER JOIN "OWHS" l ON l."WhsCode" = c."WhsCode" 
	LEFT OUTER JOIN "RDR1" m ON (c."BaseType" = m."ObjType" and c."BaseEntry" = m."DocEntry" and c."BaseLine" = m."LineNum") 
	WHERE b."CANCELED" = 'N';

	update #tbl set RptName = N'PHIẾU XUẤT KHO/ DELIVERY NOTE';

	update #tbl set AmountInWord = Num2Text(to_integer(DocTotal))
									|| ' ' || IFNULL((select "CurrName" from "OCRN" where "CurrCode" = DocCurr),'')
									|| CASE when DocTotal = ROUND(DocTotal,0) then N' chẵn' else N' và ' || LOWER(Num2Text(to_integer((DocTotal-ROUND(DocTotal-0.5,0))*100))) || ' ' || IFNULL((select "Chk100Name" from "OCRN" where "CurrCode" = DocCurr),0) end;
	
	select 
		FormNo  "FormNo",
		RptName  "RptName",
		CompName  "CompName",
		CompAdd  "CompAdd",
		CompTel  "CompTel",
		CompFax  "CompFax",
		CompTaxId  "CompTaxId",
		CardCode  "CardCode",
		CardName  "CardName",
		CardAddr  "CardAddr",
		Comments  "Comments",
		CardAtn  "CardAtn",
		CardRefNo  "CardRefNo",
		Model  "Model",
		SeriNo  "SeriNo",
		DocDate  "DocDate",
		ItemCode  "ItemCode",
		ItemName  "ItemName",
		WhsName  "WhsName",
		Unit  "Unit",
		DelQty  "DelQty",
		UnitPrice  "UnitPrice",
		RecQty  "RecQty",
		LineTotal  "LineTotal",
		DocTotal  "DocTotal",
		DocTotalFC  "DocTotalFC",
		DocRate  "DocRate",
		DocCurr  "DocCurr",
		RefNo  "RefNo",
		AmountInWord  "AmountInWord",
		DocCurrName  "DocCurrName"
	from #tbl;

	drop table #tbl;
end

