
CALL APZ_EXISTSPROC('APZ_Document_ViewDownPaymentDrawed');

GO

CREATE PROCEDURE APZ_Document_ViewDownPaymentDrawed(IN objType int, IN docEntry int)
AS
BEGIN
	declare mainCur nvarchar(3);
	select "MainCurncy" into mainCur from OADM;
	create local temporary table #tbl("FatherId" nvarchar(30), "Id" nvarchar(30), "DocNum" int, "ObjType" int, "Remarks" nvarchar(254), "VatGroup" nvarchar(8), "VatPercent" decimal(19, 6), "Currency" nvarchar(3),
	"NetAmount" decimal(19, 6), "TaxAmount" decimal(19, 6), "GrossAmount" decimal(19, 6), "OpenNetAmount" decimal(19, 6), "OpenTaxAmount" decimal(19, 6), "OpenGrossAmount" decimal(19, 6), "DocDate" date);
	
	if :objType = 18 or :objType= 25  then
		insert into #tbl("Id", "DocNum", "ObjType", "Currency", "NetAmount", "TaxAmount", "GrossAmount", "DocDate")
		select cast("BaseAbs" as nvarchar), "BaseDocNum", b."ObjType", b."DocCur", case when b."DocCur" = :mainCur then "DrawnSum" else "DrawnSumFc" end,
		case when b."DocCur" = :mainCur then "Vat" else "VatFc" end, case when b."DocCur" = :mainCur then "Gross" else "GrossFc" end, b."DocDate"
		from PCH9 a inner join OPCH b on a."DocEntry" = b."DocEntry" where b."DocEntry" = :docEntry;
		
		insert into #tbl("FatherId", "Id", "Currency", "VatGroup", "NetAmount", "TaxAmount", "GrossAmount")
		select cast("BaseAbs" as nvarchar), cast("BaseAbs" as nvarchar) || '-' || cast("LineNum" as nvarchar), b."DocCur", "VatGroup", case when b."DocCur" = :mainCur then "BaseNet" else "BaseNetFc" end,
		case when b."DocCur" = :mainCur then "BaseVat" else "BaseVatFc" end, case when b."DocCur" = :mainCur then "BaseGross" else "BaseGrossF" end
		from PCH11 a inner join OPCH b on a."DocEntry" = b."DocEntry" where b."DocEntry" = :docEntry and a."LineType" = 'D';
		
	elseif :objType = 13 or :objType = 1300 then
		insert into #tbl("Id", "DocNum", "ObjType", "Currency", "NetAmount", "TaxAmount", "GrossAmount", "DocDate")
		select cast("BaseAbs" as nvarchar), "BaseDocNum", b."ObjType", b."DocCur", case when b."DocCur" = :mainCur then "DrawnSum" else "DrawnSumFc" end,
		case when b."DocCur" = :mainCur then "Vat" else "VatFc" end, case when b."DocCur" = :mainCur then "Gross" else "GrossFc" end, b."DocDate"
		from INV9 a inner join OINV b on a."DocEntry" = b."DocEntry" where b."DocEntry" = :docEntry;
		
		insert into #tbl("FatherId", "Id", "Currency", "VatGroup", "NetAmount", "TaxAmount", "GrossAmount")
		select cast("BaseAbs" as nvarchar), cast("BaseAbs" as nvarchar) || '-' || cast("LineNum" as nvarchar), b."DocCur", "VatGroup", case when b."DocCur" = :mainCur then "BaseNet" else "BaseNetFc" end,
		case when b."DocCur" = :mainCur then "BaseVat" else "BaseVatFc" end, case when b."DocCur" = :mainCur then "BaseGross" else "BaseGrossF" end
		from INV11 a inner join OINV b on a."DocEntry" = b."DocEntry" where b."DocEntry" = :docEntry and a."LineType" = 'D';
	elseif :objType = 112 then
		insert into #tbl("Id", "DocNum", "ObjType", "Currency", "NetAmount", "TaxAmount", "GrossAmount", "DocDate")
		select cast("BaseAbs" as nvarchar), "BaseDocNum", b."ObjType", b."DocCur", case when b."DocCur" = :mainCur then "DrawnSum" else "DrawnSumFc" end,
		case when b."DocCur" = :mainCur then "Vat" else "VatFc" end, case when b."DocCur" = :mainCur then "Gross" else "GrossFc" end, b."DocDate"
		from DRF9 a inner join ODRF b on a."DocEntry" = b."DocEntry" where b."DocEntry" = :docEntry;
		
		insert into #tbl("FatherId", "Id", "Currency", "VatGroup", "NetAmount", "TaxAmount", "GrossAmount")
		select cast("BaseAbs" as nvarchar), cast("BaseAbs" as nvarchar) || '-' || cast("LineNum" as nvarchar), b."DocCur", "VatGroup", case when b."DocCur" = :mainCur then "BaseNet" else "BaseNetFc" end,
		case when b."DocCur" = :mainCur then "BaseVat" else "BaseVatFc" end, case when b."DocCur" = :mainCur then "BaseGross" else "BaseGrossF" end
		from DRF11 a inner join ODRF b on a."DocEntry" = b."DocEntry" where b."DocEntry" = :docEntry and a."LineType" = 'D';
	end if;
	
	select * from #tbl;
	drop table #tbl;
END