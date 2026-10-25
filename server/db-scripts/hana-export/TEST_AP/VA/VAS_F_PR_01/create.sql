CREATE procedure VAS_F_PR_01 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(300);
	
	select ifnull(x."BPLId",0) into bplID from "OPRQ" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		FormNo				nvarchar(50)
		, RptName			nvarchar(200)
		, CompName			nvarchar(200)
		, CompAdd			nvarchar(300)
		, CompTaxId			nvarchar(50)
		, CardCode			nvarchar(50)
		, CardName			nvarchar(200)
		, CardAddr			nvarchar(200)
		, Comments			nvarchar(300)
		, DocDate			nvarchar(100)
		, ItemCode			nvarchar(50)
		, ItemName			nvarchar(200)
		, WhsName			nvarchar(100)
		, Unit				nvarchar(100)
		, Qty				numeric(21,6)
		, UnitPrice			numeric(21,6)
		, LineTotal			numeric(21,6)
		, DocTotal			numeric(21,6)
		, DocTotalFC		numeric(21,6)
		, DocRate			numeric(21,6)
		, DocCurr			nvarchar(10)
		, DecQty			int
		, DecPrice			int
		, DecAmt			int
		, DateSep			nvarchar(1)
		, ThousSep			nvarchar(1)
		, DateTitle			nvarchar(100)
	);	

	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CardCode,CardName,CardAddr,Comments,DocDate,DocCurr,ItemCode,ItemName,Unit,Qty,UnitPrice,LineTotal,DocRate)
	SELECT 
		a."CardCode", a."ReqName", e."Name", a."Comments", 
		'Ngày/ Date: ' || to_char(a."DocDate",'DD/MM/YYYY'), a."DocCur", b."ItemCode", b."Dscription", b."unitMsr", b."Quantity", b."Price", 
		CASE WHEN a."DocCur" = c."MainCurncy" THEN b."LineTotal" ELSE b."TotalFrgn" END, a."DocRate" 
	FROM "OPRQ" a 
	LEFT OUTER JOIN "PRQ1" b ON a."DocEntry" = b."DocEntry" 
	LEFT OUTER JOIN "OADM" c ON 1 = 1 
	LEFT OUTER JOIN "OUSR" d ON d."USER_CODE" = a."Requester" 
	LEFT OUTER JOIN "OUDP" e ON e."Code" = d."Department" 
	WHERE a."CANCELED" = 'N' AND a."DocEntry" = DocID;


	update #tbl set RptName = N'ĐỀ NGHỊ MUA HÀNG/ PURCHASE REQUEST';
	
	update #tbl set DateTitle = N'Ngày.......tháng.......năm......./ Date.....................';
--	update #tbl set AmountInWord = Num2Text(to_integer(DocTotal));

	select a."QtyDec",a."DecSep",a."ThousSep",a."PriceDec" into DecQty,DateSep,ThousSep,DecPrice from "OADM" a;
	
	update #tbl set DecQty = DecQty, DateSep = DateSep,ThousSep = ThousSep, DecPrice=DecPrice;
	update #tbl set DecQty = case when floor(Qty)<>Qty then 2 else 0 end;
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
		CompTaxId  "CompTaxId",
		CardCode  "CardCode",
		CardName  "CardName",
		CardAddr  "CardAddr",
		Comments  "Comments",
		DocDate  "DocDate",
		ItemCode  "ItemCode",
		ItemName  "ItemName",
		WhsName  "WhsName",
		Unit  "Unit",
		Qty  "Qty",
		UnitPrice  "UnitPrice",
		LineTotal  "LineTotal",
		DocTotal  "DocTotal",
		DocTotalFC  "DocTotalFC",
		DocRate  "DocRate",
		DocCurr  "DocCurr",
		DecQty  "DecQty",
		DecPrice  "DecPrice",
		DecAmt  "DecAmt",
		DateSep  "DateSep",
		ThousSep  "ThousSep",
		DateTitle  "DateTitle"
	from #tbl;

	drop table #tbl;

end

