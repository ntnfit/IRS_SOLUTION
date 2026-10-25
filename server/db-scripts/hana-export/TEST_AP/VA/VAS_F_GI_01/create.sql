CREATE procedure VAS_F_GI_01 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	
	select ifnull(x."BPLId",0) into bplID from "OIGE" x where x."DocEntry"=DocID;
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
		, WhsCode 		nvarchar(20)
		, WhsName 		nvarchar(100)
		, Unit			nvarchar(100)
		, DelQty		numeric(21,6)
		, UnitPrice 	numeric(21,6)
		, RecQty		numeric(21,6)
		, LineTotal 	numeric(21,6)
		, DocTotal 		numeric(21,6)
		, DocTotalFC 	numeric(21,6)
		, DocRate 		numeric(21,6)
		, DocCurr 		nvarchar(10)
		, RefNo 		nvarchar(50)
		, AmountInWord 	nvarchar(400)
		, DocCurrName 	nvarchar(50)
		, ViTri 		nvarchar(50)
		, TkNo 			nvarchar(50)
		, TkCo 			nvarchar(50)
	);	

	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CompName,CompAdd,CompTel,CompFax,CompTaxId,SeriNo,DocDate,ItemCode,ItemName,Unit,DelQty, RecQty, Comments, CardAddr, UnitPrice, LineTotal, DocTotal, DocTotalFC, DocRate, WhsCode, WhsName, DocCurr, RefNo,ViTri,TkNo,TkCo)
	SELECT 
		UPPER(a."CompnyName"), 
		REPLACE(a."CompnyAddr", 'VIETNAM', ''), 
		IFNULL(a."Phone1", a."Phone2"), 
		a."Fax", 
		IFNULL(a."TaxIdNum", 
		IFNULL(a."TaxIdNum2", a."TaxIdNum3")), 
		b."U_JVNumber", 
		'Ngày/Date ' || to_char(b."DocDate", 'DD') || ' tháng/month ' || to_char(b."DocDate", 'MM') || ' năm/year ' || to_char(b."DocDate", 'YYYY'), 
		c."ItemCode", c."Dscription", c."unitMsr", c."Quantity", 0, b."Comments", '', c."Price", 
		CASE IFNULL(b."DocCur", c."Currency") WHEN 'VND' THEN c."LineTotal" ELSE c."TotalFrgn" END, 
		CASE WHEN b."DocCur" = 'VND' THEN b."DocTotal" - b."VatSum" ELSE b."DocTotalFC" - b."VatSumFC" END, 
		b."DocTotalFC" - b."VatSumFC", b."DocRate", c."WhsCode", l."WhsName", b."DocCur", '', '', '', c."AcctCode" 
	FROM "OADM" a, "OIGE" b, "IGE1" c, "OWHS" l 
	WHERE b."CANCELED" = 'N' AND b."DocEntry" = DocID AND b."DocEntry" = c."DocEntry" AND c."WhsCode" = l."WhsCode";



	update #tbl set TkNo = (select case when x."GLMethod"='C' then z."BalInvntAc" when x."GLMethod"='W' then t."BalInvntAc" else y."BalInvntAc" end from "OITM" x, "OITW" y, "OITB" z, "OWHS" t where x."ItemCode"=#tbl.ItemCode and x."ItemCode"=y."ItemCode" and y."WhsCode"=t."WhsCode" and x."ItmsGrpCod"=z."ItmsGrpCod" and t."WhsCode"=#tbl.WhsCode);



	update #tbl set RptName = N'PHIẾU XUẤT KHO NỘI BỘ/INTERNAL OUT SLIP';

	update #tbl set AmountInWord = Num2Text(to_integer(DocTotal))
									|| ' ' || IFNULL((select "CurrName" from "OCRN" where "CurrCode" = DocCurr),'')
									|| CASE when DocTotal = ROUND(DocTotal,0) then N' chẵn' else N' và ' || LOWER(Num2Text(to_integer((DocTotal-ROUND(DocTotal-0.5,0))*100))) || ' ' || IFNULL((select "Chk100Name" from "OCRN" where "CurrCode" = DocCurr),0) end;

	select a."QtyDec",a."DecSep",a."ThousSep",a."PriceDec" into DecQty,DateSep,ThousSep,DecPrice from "OADM" a;
	

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
		WhsCode  "WhsCode",
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
		DocCurrName  "DocCurrName",
		ViTri  "ViTri",
		TkNo  "TkNo",
		TkCo  "TkCo"
	from #tbl;
	drop table #tbl;

end

