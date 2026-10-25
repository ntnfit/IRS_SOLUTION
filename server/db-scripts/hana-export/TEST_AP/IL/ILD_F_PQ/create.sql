CREATE procedure ILD_F_PQ (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(300);
	
	select ifnull(x."BPLId",0) into bplID from "OPQT" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		CompnyName			nvarchar(300)
		,CompnyAddr			nvarchar(500)
		,RptName			nvarchar(200)
		,CntName			nvarchar(300)
		,CntEmail			nvarchar(200)
		,CompWeb			nvarchar(400)
		,CompTel			nvarchar(500)
		,BankCode			nvarchar(100)
		,BankName			nvarchar(300)
		,BankAct			nvarchar(200)
		,BankSwift			nvarchar(400)
		,BankIban			nvarchar(300)
		,DocEntry			int
		,DocDate			timestamp
		,DueDate			timestamp
		-- Khach hang
		,CardCode			nvarchar(100)
		,CardName			nvarchar(300)
		,CardAddr			nvarchar(500)
		,Contact			nvarchar(300)
		,ContactTel			nvarchar(100)
		,DocCur				nvarchar(10)
		,ItmCode			nvarchar(100)
		,ItmName			nvarchar(500)
		,Unit				nvarchar(300)
		,VisOrder			int
		,LineNum			int
		,LineType			nvarchar(1)
		,LineSep			int
		,Qty				numeric(21,6)
		,Price				numeric(21,6)
		,LineTotal			numeric(21,6)
		,Vat				numeric(21,6)
		,Discount			numeric(21,6)
		,Total				numeric(21,6)
		,VatAmt				numeric(21,6)
		,TotalVat			numeric(21,6)
		,Note				nvarchar(900)
		,Delivery			nvarchar(500)
		,Payment			nvarchar(500)
		,Comments			nvarchar(500) --
		,Note1				nvarchar(900)

		,AuthSig			nvarchar(200)
		,AuthBy				nvarchar(200)
		,Title				nvarchar(200)
		,DDate				timestamp     --
		,ROW_NUM			int
		,RequiredDate		date
		,PQTReqQty			numeric(21,6)
		,GTotal				numeric(21,6) -- Tổng tiền line sau thuế

		,S_AttnName  		nvarchar(100)
		,S_Phone			nvarchar(50)
		,S_Fax				nvarchar(50)
		
		,SellerName			nvarchar(50)
		,S_MobilePhone		nvarchar(50)
		,S_BankInfo			nvarchar(100)
		,S_Code				nvarchar(50)			
		,S_VatNumber		nvarchar(50)
		
		,PayTerm			nvarchar(100)
		,DelTerm			nvarchar(100)			
		,Remark				nvarchar(100)
		
		
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl(DocEntry,DocDate,DueDate,DocCur,CardCode,CardName,CardAddr,LineNum,LineType,LineSep,ItmCode,ItmName,Unit,Qty,Price,LineTotal,Vat,VatAmt,TotalVat,Note,VisOrder,Note1,ROW_NUM,Discount,
				RequiredDate,PQTReqQty, GTotal,
				--- Supplier
				S_AttnName,
				S_Phone,
				S_Fax,
				SellerName,
				S_MobilePhone,
				S_BankInfo,
				S_Code,
				S_VatNumber,
				PayTerm,
				DelTerm,
				Remark
				)
	select a."DocEntry", a."DocDate",a."DocDueDate",a."DocCur",a."CardCode",a."CardName",replace(a."Address",'VIETNAM',''), b."LineNum",b."LineType",b.LineSeq
			,b."ItemCode",b."ItemName",b."InvntryUom",b."Quantity",b."PriceBefDi",case when a."DocCur"='VND' then b."LineTotal" else b."TotalFrgn" end
			,b."Rate",case when a."DocCur"='VND' then a."VatSum" else a."VatSumFC" end,case when a."DocCur"='VND' then a."DocTotal" else a."DocTotalFC" end
			,a."Header",b."VisOrder",a."Footer"
			,ROW_NUMBER() OVER (ORDER BY b."VisOrder", b."LineNum"),b."DiscPrcnt",
			b."PQTReqDate", b."PQTReqQty", b."GTotal",
			--- Supplier
			'' as 	"S_AttnName",
			'' as	"S_Phone",
			'' as	"S_Fax",
			'' as	"SellerName",
			'' as	"S_MobilePhone",
			'' as	"S_BankInfo",
			'' as	"S_Code",
			'' as	"S_VatNumber",
			'' as  	"PayTerm",
			'' as 	"DelTerm",
			'' as 	"Remark"
			
	from OPQT a
		left join (
			select  a."PQTReqDate",a."PQTReqQty",a."GTotal",a."VisOrder",a."DocEntry",a."LineNum",a."LineType",-2 LineSeq,a."ItemCode",a."Dscription",b."ItemName",b."InvntryUom",a."Currency",a."Quantity",a."PriceBefDi",a."LineTotal",a."DiscPrcnt",c."Rate",a."TotalFrgn" from "PQT1" a,"OITM" b,"OVTG" c where a."ItemCode" = b."ItemCode" and a."VatGroup"=c."Code"
			union all
			select '',a."AftLineNum",0,0,a."DocEntry",10000,a."LineType",a."LineSeq",'',a."LineText",'','','',0,0,0,0,0,0 from "PQT10" a
		) b on a."DocEntry" = b."DocEntry"
		left join "OCRD" c on c."CardCode" = a."CardCode"
	where a."DocEntry" = DocID and a."CANCELED"='N';
	
	update #tbl set Total = TotalVat-VatAmt;

	update #tbl set LineTotal = (select SUM(a.LineTotal) from #tbl a where a.LineType = 'R' and a.ROW_NUM < #tbl.ROW_NUM and a.ROW_NUM > IFNULL((select MAX(aa.ROW_NUM) from #tbl aa where aa.LineType = 'S' and aa.ROW_NUM < #tbl.ROW_NUM),0))
	where #tbl.LineType = 'S';

	update #tbl set Payment = (select b."PymntGroup" from "OPQT" a, "OCTG" b where a."DocEntry" = DocID and a."GroupNum"=b."GroupNum");
	
	select x."CompnyName",x."CompnyAddr",x."Phone1",x."Fax",y."IntrntAdrs",x."E_Mail" into tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail from OADM x, ADM1 y ;
	
	
	update #tbl set CompnyName = tCompName, CompnyAddr = tCompAddr, CompTel = tCompTel, CompWeb = tCompWebsite, RptName= 'PURCHASE QUOTATION';
	
	update #tbl set 
		CntName 	= (select b."SlpName" from "OPQT" a, "OSLP" b where a."DocEntry" = DocID and a."SlpCode"=b."SlpCode"),
		CntEmail 	= (select b."Email" from "OPQT" a, "OSLP" b where a."DocEntry" = DocID and a."SlpCode"=b."SlpCode") ; 

	update #tbl set 
		Contact 	= (select b."Name" from "OPQT" a, "OCPR" b where a."CntctCode"=b."CntctCode" and a."DocEntry" = DocID),
		ContactTel 	= (select b."Tel1" from "OPQT" a, "OCPR" b where a."CntctCode"=b."CntctCode" and a."DocEntry" = DocID);

	update #tbl set 
		BankName	= (select b."BankName" from "ODSC" b,"DSC1" a where a."BankCode"=#tbl.BankCode and a."BankCode"=b."BankCode"),
		BankAct		= (select a."Account" from "ODSC" b,"DSC1" a where a."BankCode"=#tbl.BankCode and a."BankCode"=b."BankCode"),
		BankSwift	= (select a."SwiftNum" from "ODSC" b,"DSC1" a where a."BankCode"=#tbl.BankCode and a."BankCode"=b."BankCode"),
		BankIban	= (select a."IBAN" from "ODSC" b,"DSC1" a where a."BankCode"=#tbl.BankCode and a."BankCode"=b."BankCode");
	
	select 
		CompnyName 	"CompnyName",
		CompnyAddr 	"CompnyAddr",
		RptName	 	"RptName",
		CntName 	"CntName",
		CntEmail 	"CntEmail",
		CompWeb 	"CompWeb",
		CompTel 	"CompTel",
		BankCode 	"BankCode",
		BankName 	"BankName",
		BankAct 	"BankAct",
		BankSwift 	"BankSwift",
		BankIban 	"BankIban",
		DocEntry 	"DocEntry",
		DocDate 	"DocDate",
		DueDate 	"DueDate",
		CardCode	"CardCode",
		CardName 	"CardName",
		CardAddr 	"CardAddr",
		Contact 	"Contact",
		ContactTel 	"ContactTel",
		DocCur 		"DocCur",
		ItmCode 	"ItmCode",
		ItmName 	"ItmName",
		Unit 		"Unit",
		VisOrder 	"VisOrder",
		LineNum 	"LineNum",
		LineType 	"LineType",
		LineSep 	"LineSep",
		Qty 		"Qty",
		Price 		"Price",
		LineTotal 	"LineTotal",
		Vat 		"Vat",
		Discount 	"Discount",
		Total 		"Total",
		VatAmt 		"VatAmt",
		TotalVat 	"TotalVat",
		Note 		"Note",
		Delivery 	"Delivery",
		Payment 	"Payment",
		Comments 	"Comment",
		Note1 		"Note1",
		AuthSig 	"AuthSig",
		AuthBy 		"AuthBy",
		Title 		"Title",
		DDate 		"Date",
		ROW_NUM 	"ROW_NUM",
		RequiredDate "RequiredDate",
		PQTReqQty	"PQTReqQty",
		GTotal		"GTotal",
		
		S_AttnName   	"S_AttnName",
		S_Phone	"S_Phone",
		S_Fax  	"S_Fax",
		SellerName	"SellerName",
		S_MobilePhone	"S_MobilePhone",
		S_BankInfo	"S_BankInfo",
		S_Code	"S_Code",
		S_VatNumber "S_VatNumber",
		
		PayTerm		"PayTerm",
		DelTerm 	"DelTerm",
		Remark		"Remark"
	from #tbl;

	drop table #tbl;
end


