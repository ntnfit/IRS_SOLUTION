CREATE procedure VAS_F_PO_03 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	
	select ifnull(x."BPLId",0) into bplID from "OPOR" x where x."DocEntry"=DocId;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		Title 			nvarchar(100)
		,U_JVNumber 	nvarchar(300)
		,DDate 			timestamp
		,CompyName 		nvarchar(200)
		,CompnyAddr 	nvarchar(400)
		,CompnyPhone 	nvarchar(50)
		,CompnyFax 		nvarchar(50)
		,CompnyAttn 	nvarchar(200)
		
		,Shipper 		nvarchar(500)
		,CardCode 		nvarchar(100)
		,CardName 		nvarchar(500)
		,PaymentTerm 	nvarchar(500)
		,AcctName 		nvarchar(200)
		,AcctNo 		nvarchar(100)
		,BankName 		nvarchar(300)
		,SwiftCode 		nvarchar(100)
		,Shipment 		nvarchar(500)
		,ItemCode 		nvarchar(100)
		,ItemName 		nvarchar(254)
		,Unit 			nvarchar(50)
		,DocCur 		nvarchar(10)
		,Qty 			numeric(21,6)
		,Price 			numeric(21,6)
		,LineTotal 		numeric(21,6)
		,TotalVat 		numeric(21,6)
		,Total 			numeric(21,6)
		,TotalInWords 	nvarchar(500)
		,Comments 		nvarchar(500)
		,DateTitle 		nvarchar(200)
		,Seller 		nvarchar(100)
		,Buyer 			nvarchar(100)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	
	insert into #tbl (U_JVNumber, CardCode, CardName, AcctName, DDate, ItemCode, ItemName, Unit, DocCur, Qty, Price, LineTotal,Total,TotalVat)
	select a."U_JVNumber", a."CardCode", a."CardName", a."CardName", a."DocDate", b."ItemCode", b."Dscription", d."InvntryUom", a."DocCur", b."Quantity", b."Price"
			,case when a."DocCur" = 'VND' then b."LineTotal" else b."TotalFrgn" end, a."DocTotal",a."VatSum"
	from "OADM" T0
	left join "OPOR" a on (a."DocEntry" = DocID and a."CANCELED" = 'N')
	left join "POR1" b on a."DocEntry" = b."DocEntry"
	left join "OCRD" c on c."CardCode" = a."CardCode"
	left join "OITM" d on d."ItemCode" = b."ItemCode"
	left join "OOCR" e on e."OcrCode" = b."OcrCode";
	

	update #tbl set PaymentTerm = (select "PymntGroup" from "OCTG" where "GroupNum" = (select "GroupNum" from "OPOR" where "DocEntry" = DocID and "GroupNum" <> -1))
					, AcctNo = (select "DflAccount" from "OCRD" where "CardCode" = #tbl.CardCode)
					, BankName = (select "BankName" from "ODSC" where "BankCode" = (select "BankCode" from "OCRD" where "CardCode" = #tbl.CardCode))
					, SwiftCode = (select "DflSwift" from "OCRD" where "CardCode" = #tbl.CardCode);

--	update #tbl set Total = (select sum(LineTotal) from #tbl)  + VatSum;

	
	select top 1 floor(Total) into num1 from #tbl;
	select top 1 (-floor(Total) + round(Total,2)) * 100 into num2 from #tbl;
	update #tbl set TotalInWords = CONCAT(CONCAT(Num2Text(to_integer(num1)),' '),(select lower("CurrName") from "OCRN" where "CurrCode" = #tbl.DocCur));
	
	if num2 <> 0 then
		update #tbl set TotalInWords = CONCAT(TotalInWords,' phẩy ');
		update #tbl set TotalInWords = CONCAT(TotalInWords,lower(Num2Text (to_integer(num2))));
		update #tbl set TotalInWords = CONCAT(TotalInWords,' ');
		update #tbl set TotalInWords = CONCAT(TotalInWords,(select "Chk100Name" from "OCRN" where "CurrCode" = #tbl.Doccur));
	end if;
	
	update #tbl set 
		CompyName 	= (select "CompnyName" from "OADM"),
		CompnyAddr 	= (select "CompnyAddr" from "OADM"),
		CompnyPhone	= (select "Phone1" from "OADM"),
		CompnyFax 	= (select "Fax" from "OADM"),
		CompnyAttn 	= 'Người đại diện',
		Title 		= 'SALES ORDER AND CONTRACT',
		Comments 	= CONCAT(CONCAT('Grand Total (',DocCur), ')'),
		DateTitle 	= CONCAT(CONCAT(CONCAT(CONCAT(CONCAT('Ngày ',to_char(DDate,'DD')),' tháng '),to_char(DDate,'MM')),' năm '),to_char(DDate,'YYYY'));
		
	select 
		Title "Title",
		U_JVNumber "U_JVNumber",
		DDate "Date",
		CompyName "CompyName",
		CompnyAddr "CompnyAddr",
		CompnyPhone "CompnyPhone",
		CompnyFax "CompnyFax",
		CompnyAttn "CompnyAttn",
		Shipper "Shipper",
		CardCode "CardCode",
		CardName "CardName",
		PaymentTerm "PaymentTerm",
		AcctName "AcctName",
		AcctNo "AcctNo",
		BankName "BankName",
		SwiftCode "SwiftCode",
		Shipment "Shipment",
		ItemCode "ItemCode",
		ItemName "ItemName",
		Unit "Unit",
		DocCur "DocCur",
		Qty "Qty",
		Price "Price",
		LineTotal "LineTotal",
		TotalVat "TotalVat",
		Total "Total",
		TotalInWords "TotalInWords",
		Comments "Comment",
		DateTitle "DateTitle",
		Seller "Seller",
		Buyer "Buyer"
	from #tbl;

	drop table #tbl;
end

