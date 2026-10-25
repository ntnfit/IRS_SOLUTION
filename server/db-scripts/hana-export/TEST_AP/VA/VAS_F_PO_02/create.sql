CREATE procedure VAS_F_PO_02 (
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
		CompnyName 		nvarchar(200)
		,Title 			nvarchar(100)
		,U_JVNumber 	nvarchar(300)
		,DDate 			timestamp
		,CompyName 		nvarchar(200)
		,CompnyAddr 	nvarchar(400)
		,CompnyPhone 	nvarchar(50)
		,CompnyFax 		nvarchar(50)
		,DateTitle 		nvarchar(100)
		
		,CardCode 		nvarchar(200)
		,CardName 		nvarchar(500)
		,ItemCode 		nvarchar(100)
		,ItemName 		nvarchar(500)
		,Unit 			nvarchar(100)
		,Qty 			float
		,DocCur 		nvarchar(50)
		,Price 			numeric(21,6)
		,LineTotal 		numeric(21,6)
		,VatSum 		numeric(21,6)
		,VatPrsn 		numeric(21,6)
		,Total 			numeric(21,6)
		,TotalInWords 	nvarchar(700)
		,ChatLuong 		nvarchar(500)
		,ThoiGianGH 	nvarchar(500)
		,DiaDiemGH 		nvarchar(500)
		,ThanhToan 		nvarchar(500)
		,ThongTinTT 	nvarchar(500)
		,TaiKhoan 		nvarchar(200)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	
	insert into #tbl (U_JVNumber, CardCode, CardName, DDate, ItemCode, ItemName, Unit, DocCur, Qty, Price, LineTotal, VatSum, VatPrsn)
	select a."U_JVNumber", a."CardCode", a."CardName", a."DocDate"
			, b."ItemCode", b."Dscription", 'chiếc', "DocCur", b."Quantity", b."Price"
			, case when a."DocCur" = 'VND' then b."LineTotal" else b."TotalFrgn" end, case when a."DocCur" = 'VND' then a."VatSum" else "VatSumFC" end
			, IFNULL(case when IFNULL(a."DocTotal"-a."VatSum",0) != 0 then a."VatSum"/(a."DocTotal"-a."VatSum")*100 end,0)
	from "OADM" T0
	left join "OPOR" a on (a."DocEntry" = DocID and a."CANCELED" = 'N')
	left join "POR1" b on a."DocEntry" = b."DocEntry"
	left join "OCRD" c on c."CardCode" = a."CardCode"
	left join "OITM" d on d."ItemCode" = b."ItemCode"
	left join "OOCR" e on e."OcrCode" = b."OcrCode";
	
	update #tbl set Total = (select sum(LineTotal) from #tbl)  + VatSum;

	
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
		Title 		= 'PHIẾU ĐẶT HÀNG',
		DateTitle 	= CONCAT(CONCAT(CONCAT(CONCAT(CONCAT('....., ngày ',to_char(DDate,'DD')),' tháng '),to_char(DDate,'MM')),' năm '),to_char(DDate,'YYYY')),
		ChatLuong 	= CONCAT('Hàng mới 100%, Xuất xứ ',ifnull((select b."FirmName" from "OITM" a, "OMRC" b where a."FirmCode" = b."FirmCode" and a."ItemCode" = #tbl.ItemCode and a."FirmCode" <> -1),'')),
		ThoiGianGH 	= '',
		DiaDiemGH 	= '',
		ThanhToan 	= '';
		
	select 
		CompnyName "CompnyName",
		Title "Title",
		U_JVNumber "U_JVNumber",
		DDate "Date",
		CompyName "CompyName",
		CompnyAddr "CompnyAddr",
		CompnyPhone "CompnyPhone",
		CompnyFax "CompnyFax",
		DateTitle "DateTitle",
		CardCode "CardCode",
		CardName "CardName",
		ItemCode "ItemCode",
		ItemName "ItemName",
		Unit "Unit",
		Qty "Qty",
		DocCur "DocCur",
		Price "Price",
		LineTotal "LineTotal",
		VatSum "VatSum",
		VatPrsn "VatPrsn",
		Total "Total",
		TotalInWords "TotalInWords",
		ChatLuong "ChatLuong",
		ThoiGianGH "ThoiGianGH",
		DiaDiemGH "DiaDiemGH",
		ThanhToan "ThanhToan",
		ThongTinTT "ThongTinTT",
		TaiKhoan "TaiKhoan"
	from #tbl;

	drop table #tbl;
end

