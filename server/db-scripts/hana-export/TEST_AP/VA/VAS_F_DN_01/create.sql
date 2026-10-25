CREATE procedure VAS_F_DN_01 (
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
		, CardCode		nvarchar(50)
		, CardName		nvarchar(200)
		, CardAddr		nvarchar(200)
		, Comments		nvarchar(300)
		, CardRefNo		nvarchar(100)
		, Model			nvarchar(100)
		, U_JVNumber	nvarchar(100)
		, DocDate		nvarchar(100)
		, ItemCode		nvarchar(50)
		, ItemName		nvarchar(200)
		, WhsCode		nvarchar(100)
		, WhsName		nvarchar(300)
		, Unit			nvarchar(100)
		, Batch			nvarchar(300)
		, OrderQty		numeric(21,6)
		, RecptQty		numeric(21,6)
        , QtyBatch		numeric(21,6)
		, UnitPrice		numeric(21,6)
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
		, DateSep		nvarchar(1)
		, ThousSep		nvarchar(1)
		, DateTitle		nvarchar(100)
	);
	CREATE local temporary TABLE #Batch(
		DocEntry 	int,
		ItemCode 	nvarchar(50),
		DocType		nvarchar(20),
		DistNumber	nvarchar(50),
		Quantity	numeric(21,6),
		DocLine		int
	);
	insert into #Batch (DocEntry,ItemCode,DocType,DistNumber,Quantity,DocLine)
	SELECT a."DocEntry", a."ItemCode", a."DocType", c."DistNumber", -b."Quantity" AS "Quantity", a."DocLine" 
	FROM "OITL" a 
	LEFT OUTER JOIN "ITL1" b ON a."LogEntry" = b."LogEntry" 
	LEFT OUTER JOIN "OBTN" c ON (c."SysNumber" = b."SysNumber" AND b."ItemCode" = c."ItemCode") 
	WHERE ((a."DocType" = '15' AND a."DocEntry" = DocID) OR (a."DocType" = '16' AND a."BaseType" = '15' AND a."BaseEntry" = DocID)) AND IFNULL(b."Quantity", 0) <> 0;

	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (DocDate,CardCode,CardName,CardAddr,Comments,CardRefNo,U_JVNumber,ItemCode,ItemName,WhsCode,Unit,Batch,OrderQty,RecptQty,QtyBatch,UnitPrice,LineTotal,DocTotal,DocTotalFC,DocRate,DocCurr,FormNo)
	select 
		CONCAT('Ngày/ Date: ',to_char(a."DocDate",'DD/MM/YYYY'))
		,a."CardCode"
		,a."CardName"
		,replace(a."Address",'VIETNAM','')
		,a."Comments"
		,a."U_InvNo"
		,a."U_JVNumber"
		,b."ItemCode"
		,b."Dscription"
		,b."WhsCode"
		,b."unitMsr"
		,d.DistNumber
		,c."Quantity"
		,b."Quantity"
		,d.Quantity
		,b."Price"
		,case when a."DocCur"=e."MainCurncy" then b."LineTotal" else b."TotalFrgn" end
		,case when a."DocCur"=e."MainCurncy" then a."DocTotal"-a."VatSum"-IFNULL(a."TotalExpns",0) else a."DocTotalFC"-a."VatSumFC"-IFNULL(a."TotalExpFC",0) end
		,a."DocTotalFC"
		,a."DocRate"
		,a."DocCur"
		,'02 - VT'
	from
		"ODLN" a
		left join "DLN1" b on a."DocEntry"=b."DocEntry"
		left join "RDR1" c on c."DocEntry"=b."BaseEntry" and c."LineNum"=b."BaseLine" and c."ItemCode"=b."ItemCode"
		left join #Batch d on a."DocEntry"=d.DocEntry and b."ItemCode"=d.ItemCode and d.DocLine = b."LineNum"
		left join "OADM" e on 1=1
	where a."DocEntry"=DocID and a."CANCELED"='N';
	--
	update #tbl set WhsName =  (select a."WhsName" from "OWHS" a where a."WhsCode"=#tbl.WhsCode);
	

	update #tbl set RptName = N'PHIẾU XUẤT KHO/ DELIVERY NOTE';
	
	update #tbl set DateTitle = N'Ngày.......tháng.......năm......./ Date.....................';
	update #tbl set AmountInWord = Num2Text(to_integer(DocTotal));
	update #tbl set AmountInWord = CONCAT(AmountInWord,' ');
	update #tbl set AmountInWord = CONCAT(AmountInWord,IFNULL((select "CurrName" from "OCRN" where "CurrCode" = DocCurr),''));
	update #tbl set AmountInWord = CONCAT(AmountInWord,CASE when DocTotal = ROUND(DocTotal,0) then N' chẵn' else  CONCAT(CONCAT(CONCAT(' và ', LOWER(Num2Text(cast((DocTotal-ROUND(DocTotal-0.5,0))*100 as int)))),' '),IFNULL((select "Chk100Name" from "OCRN" where "CurrCode" = DocCurr),0)) end);

	update #tbl set 
		DecQty = 2,
		DateSep = (select a."DecSep" from "OADM" a),
		ThousSep = (select a."ThousSep" from "OADM" a),
		DecPrice=0;
		
	update #tbl set DecAmt = case when (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.DocCurr)='-1' then (select a."SumDec" from "OADM" a) else (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.DocCurr) end;
	
	if bplID=0 then
		update #tbl set 
			CompName	= (select a."CompnyName" from "OADM" a,"ADM1" b),
			CompAdd		= (select b."Street" from "OADM" a,"ADM1" b);
	end if;
	if bplID<>0 then
		update #tbl set 
			CompName	= (select x."BPLName" from "OBPL" x where "BPLId"=bplID),
			CompAdd		= (select x."Street" from "OBPL" x where "BPLId"=bplID);
	end if;
	update #tbl set Batch = IFNULL(Batch,'');
		

	select 
		FormNo "FormNo",
		RptName "RptName",
		CompName "CompName",
		CompAdd "CompAdd",
		CardCode "CardCode",
		CardName "CardName",
		CardAddr "CardAddr",
		Comments "Comments",
		CardRefNo "CardRefNo",
		Model "Model",
		U_JVNumber "U_JVNumber",
		DocDate "DocDate",
		ItemCode "ItemCode",
		ItemName "ItemName",
		WhsCode "WhsCode",
		WhsName "WhsName",
		Unit "Unit",
		Batch "Batch",
		OrderQty "OrderQty",
		RecptQty "RecptQty",
		QtyBatch "QtyBatch",
		UnitPrice "UnitPrice",
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
		DateSep "DateSep",
		ThousSep "ThousSep",
		DateTitle "DateTitle"
	from #tbl;

	drop table #tbl;
	drop table #Batch;
end

