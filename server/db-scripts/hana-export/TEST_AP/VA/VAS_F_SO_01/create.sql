CREATE procedure VAS_F_SO_01 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	
	select ifnull(x."BPLId",0) into bplID from "ORDR" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		CompName 		nvarchar(300), 
		CompAddr 		nvarchar(300),
		CompTel 		nvarchar(30), 
		CompFax 		nvarchar(30), 
		RptName 		nvarchar(300), 
		DocNo 			nvarchar(100), 
		DDate 			timestamp, 
		InvoiceNo 		nvarchar(100), 
		SONo 			nvarchar(100), 
		BPCode 			nvarchar(20),
		BillName 		nvarchar(300), 
		BillAddr 		nvarchar(300), 
		BillCity 		nvarchar(300), 
		BillCountry 	nvarchar(100), 
		BillPhone 		nvarchar(30), 
		BillFax 		nvarchar(30),
		ShipName 		nvarchar(300), 
		ShipAddr 		nvarchar(300), 
		ShipCity 		nvarchar(300), 
		ShipCountry 	nvarchar(100), 
		ShipPhone 		nvarchar(30), 
		ShipFax 		nvarchar(30),
		ItmCode 		nvarchar(20), 
		ItmName 		nvarchar(300), 
		PartNo 			nvarchar(300), 
		Qty 			numeric(21,6), 
		Cur 			nvarchar(10), 
		Rate 			numeric(21,6), 
		Price 			numeric(21,6), 
		LineTotal 		numeric(21,6), 
		Amount 			numeric(21,6), 
		DisCount 		numeric(21,6), 
		VatPer 			numeric(21,6), 
		VatAmt 			numeric(21,6), 
		DocTotal 		numeric(21,6), 
		DelTerm 		nvarchar(300), 
		PayTerm 		nvarchar(300), 
		Signer 			nvarchar(100), 
		Note 			nvarchar(300)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CompName,CompAddr,CompTel,CompFax,RptName,DocNo,DDate,SONo,BPCode,BillName,ShipName,BillPhone,BillFax,ShipPhone,ShipFax,
				ItmCode,ItmName,PartNo,Qty,Cur,Rate,Price,LineTotal,DisCount,VatPer,VatAmt,DocTotal)
	SELECT c."CompnyName", c."CompnyAddr", c."Phone1", c."Fax", n'SALE ORDER', '..............', a."DocDate", a."U_JVNumber", a."CardCode", a."CardName", a."CardName", e."Phone1", e."Fax", e."Phone1", e."Fax", b."ItemCode", b."Dscription", d."FrgnName", b."Quantity", a."DocCur", a."DocRate", b."Price" AS "Price", CASE WHEN b."TotalFrgn" = 0 THEN b."LineTotal" ELSE "TotalFrgn" END AS "LineTotal", CASE WHEN a."DiscSumFC" = 0 THEN a."DiscSum" ELSE "DiscSumFC" END, b."VatPrcnt", CASE WHEN a."VatSumFC" = 0 THEN a."VatSum" ELSE "VatSumFC" END, CASE WHEN a."DocTotalFC" = 0 THEN a."DocTotal" ELSE a."DocTotalFC" END 
	FROM "ORDR" a, "RDR1" b, "OADM" c, "OITM" d, "OCRD" e 
	WHERE a."DocEntry" = b."DocEntry" AND a."DocEntry" = DocID AND d."ItemCode" = b."ItemCode" AND e."CardCode" = a."CardCode";
	
	update #tbl set Amount = (select sum(x.LineTotal) from #tbl x);
	
	update #tbl set 
		BillAddr 	= (select "StreetB"	from "RDR12" where "DocEntry"=DocID), 
		BillCity 	= (select "CityB"	from "RDR12" where "DocEntry"=DocID), 
		BillCountry = (select "Name" from "OCRY" where "Code"=(select "CountryB" from "RDR12" where "DocEntry"=DocID)), 
		ShipAddr 	= (select "StreetS"	from "RDR12" where "DocEntry"=DocID), 
		ShipCity 	= (select "CityS"	from "RDR12" where "DocEntry"=DocID), 
		ShipCountry = (select "Name" from "OCRY" where "Code"=(select "CountryS" from "RDR12" where "DocEntry"=DocID));

	update #tbl set DelTerm = (select "TrnspName" from "OSHP" where "TrnspCode"=(select "TrnspCode" from "ORDR" where "DocEntry"=DocID)),
					PayTerm = (select "PymntGroup" from "OCTG" where "GroupNum" = (select "GroupNum" from "ORDR" where "DocEntry"=DocID)),
					Signer  = (select "SlpName" from "OSLP" where "SlpCode"=(select "SlpCode" from "ORDR" where "DocEntry"=DocID));
	update #tbl set 
		CompTel	= CONCAT('Phone : ',ifnull(CompTel,'')),
		CompFax	= CONCAT('Fax : ',ifnull(CompFax,''));



	select 
		CompName "CompName",
		CompAddr "CompAddr",
		CompTel "CompTel",
		CompFax "CompFax",
		RptName "RptName",
		DocNo "DocNo",
		DDate "DDate",
		InvoiceNo "InvoiceNo",
		SONo "SONo",
		BPCode "BPCode",
		BillName "BillName",
		BillAddr "BillAddr",
		BillCity "BillCity",
		BillCountry "BillCountry",
		BillPhone "BillPhone",
		BillFax "BillFax",
		ShipName "ShipName",
		ShipAddr "ShipAddr",
		ShipCity "ShipCity",
		ShipCountry "ShipCountry",
		ShipPhone "ShipPhone",
		ShipFax "ShipFax",
		ItmCode "ItmCode",
		ItmName "ItmName",
		PartNo "PartNo",
		Qty "Qty",
		Cur "Cur",
		Rate "Rate",
		Price "Price",
		LineTotal "LineTotal",
		Amount "Amount",
		DisCount "DisCount",
		VatPer "VatPer",
		VatAmt "VatAmt",
		DocTotal "DocTotal",
		DelTerm "DelTerm",
		PayTerm "PayTerm",
		Signer "Signer",
		Note "Note"
	from #tbl;


	drop table #tbl;
end

