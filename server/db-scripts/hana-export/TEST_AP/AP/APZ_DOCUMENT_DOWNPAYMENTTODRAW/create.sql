
CREATE PROCEDURE APZ_Document_DownPaymentToDraw(IN objType int, IN cardCode nvarchar(15), IN docCur nvarchar(3))
AS
BEGIN
	declare mainCur nvarchar(3);
	declare decRound int;
	select "MainCurncy" into mainCur from OADM;
	
	create local temporary table #tbl("FatherId" nvarchar(30), "Id" nvarchar(30), "DocNum" int, "ObjType" int, "Remarks" nvarchar(254), "VatGroup" nvarchar(8), "VatPercent" decimal(19, 6), "Currency" nvarchar(3),
	"NetAmount" decimal(19, 6), "TaxAmount" decimal(19, 6), "GrossAmount" decimal(19, 6), "OpenNetAmount" decimal(19, 6), "OpenTaxAmount" decimal(19, 6), "OpenGrossAmount" decimal(19, 6), "DocDate" date);
	
	if :objType = 18 or :objType = 25 then
		insert into #tbl("FatherId", "Id", "VatGroup", "VatPercent", "NetAmount")
		select cast(a."DocEntry" as nvarchar), cast(a."DocEntry" as nvarchar) || '-' || cast(min(b."LineNum") as nvarchar), b."VatGroup", b."VatPrcnt"/100,
		case when :docCur = :mainCur then sum(b."OpenSum")*a."DpmPrcnt"/100 else sum(b."OpenSumFC")*a."DpmPrcnt"/100 end
		from ODPO a inner join DPO1 b on a."DocEntry" = b."DocEntry" where a."CardCode" = :cardCode and a."DocCur" = :docCur and a."ReceiptNum" is not null and a."DpmStatus" = 'O'
		group by a."DocEntry", b."VatGroup", b."VatPrcnt", a."DpmPrcnt";
		
		update a set "NetAmount" = a."NetAmount" - ifnull(b."AppliedAmt",0) from #tbl a, (select "DocEntry", "VatGroup", case when :docCur = :mainCur then "DpApplNet" else "DpApplNetF" end "AppliedAmt" from DPO15) b
		where a."FatherId" = cast(b."DocEntry" as nvarchar) and a."VatGroup" = b."VatGroup";
		
		update #tbl set "TaxAmount" = "NetAmount"*"VatPercent", "GrossAmount"="NetAmount"*(1+"VatPercent");
		update #tbl set  "OpenNetAmount" = "NetAmount", "OpenTaxAmount"="TaxAmount","OpenGrossAmount"="GrossAmount";
		insert into #tbl("Id", "NetAmount", "TaxAmount", "GrossAmount", "OpenNetAmount", "OpenTaxAmount", "OpenGrossAmount")
		select "FatherId", sum("NetAmount"), sum("TaxAmount"), sum("GrossAmount"), sum("OpenNetAmount"), sum("OpenTaxAmount"), sum("OpenGrossAmount") from #tbl group by "FatherId";
		update a set "DocNum" = b."DocNum", "ObjType" = b."ObjType", "Remarks" = b."Comments", "DocDate" = b."DocDate" from #tbl a, ODPO b where a."Id" = cast(b."DocEntry" as nvarchar);
		update #tbl set "Currency" = :docCur;
	elseif :objType = 13 or :objType = 1300 then
		insert into #tbl("FatherId", "Id", "VatGroup", "VatPercent", "NetAmount")
		select cast(a."DocEntry" as nvarchar), cast(a."DocEntry" as nvarchar) || '-' || cast(min(b."LineNum") as nvarchar), b."VatGroup", b."VatPrcnt"/100,
		case when :docCur = :mainCur then sum(b."OpenSum")*a."DpmPrcnt"/100 else sum(b."OpenSumFC")*a."DpmPrcnt"/100 end
		from ODPI a inner join DPI1 b on a."DocEntry" = b."DocEntry" where a."CardCode" = :cardCode and a."DocCur" = :docCur and a."ReceiptNum" is not null and a."DpmStatus" = 'O'
		group by a."DocEntry", b."VatGroup", b."VatPrcnt", a."DpmPrcnt";
		
		update a set "NetAmount" = a."NetAmount" - ifnull(b."AppliedAmt",0) from #tbl a, (select "DocEntry", "VatGroup", case when :docCur = :mainCur then "DpApplNet" else "DpApplNetF" end "AppliedAmt" from DPI15) b
		where a."FatherId" = cast(b."DocEntry" as nvarchar) and a."VatGroup" = b."VatGroup";
		
		update #tbl set "TaxAmount" = "NetAmount"*"VatPercent", "GrossAmount"="NetAmount"*(1+"VatPercent");
		update #tbl set  "OpenNetAmount" = "NetAmount", "OpenTaxAmount"="TaxAmount","OpenGrossAmount"="GrossAmount";
		insert into #tbl("Id", "NetAmount", "TaxAmount", "GrossAmount", "OpenNetAmount", "OpenTaxAmount", "OpenGrossAmount")
		select "FatherId", sum("NetAmount"), sum("TaxAmount"), sum("GrossAmount"), sum("OpenNetAmount"), sum("OpenTaxAmount"), sum("OpenGrossAmount") from #tbl group by "FatherId";
		update a set "DocNum" = b."DocNum", "ObjType" = b."ObjType", "Remarks" = b."Comments", "DocDate" = b."DocDate" from #tbl a, ODPI b where a."Id" = cast(b."DocEntry" as nvarchar);
		update #tbl set "Currency" = :docCur;
	end if;
	
	select a."Decimals" into decRound from OCRN a where a."CurrCode"=:docCur;
	if :decRound = -1 then
		select "SumDec" into decRound from OADM;
	end if;
	
	update #tbl set "NetAmount" = round("NetAmount", :decRound), "GrossAmount" = round("GrossAmount", :decRound);
	select * from #tbl;
	drop table #tbl;
END

