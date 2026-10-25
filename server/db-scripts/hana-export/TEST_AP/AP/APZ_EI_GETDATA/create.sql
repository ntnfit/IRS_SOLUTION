CREATE PROCEDURE APZ_EI_GetData
(
 	in ObjType 			nvarchar(11),
 	in DocEntry			nvarchar(11)
 )
 AS 
 BEGIN
 	declare MainCurr nvarchar(30);
 	declare SONumber, ContractNo nvarchar(5000);
	select "MainCurncy" into MainCurr from OADM;
 	create local temporary table #tbl(
		"CompName"					nvarchar(100)
		,"CompAddr"					nvarchar(300)
		,"ComLidTradNum"			nvarchar(50)
		,"ComPhone"					nvarchar(50)
		,"ComFax"					nvarchar(50)
		,"ComEmail"					nvarchar(100)
		,"ComWebSite"				nvarchar(100)
		,"ComBankNo"				nvarchar(300)
		,"ComTaxCode"				nvarchar(300)
		
		,"DocCode"					nvarchar(100)
		,"ObjType"					int
		,"ObjName"					nvarchar(100)
		,"DocEntry"					int
		,"DocNum"					int
		,"DocDate"					date
		,"Comments"					nvarchar(254)		
		,"CardCode"					nvarchar(50)
		,"CardName"					nvarchar(254)
		,"Address"					nclob
		,"LidTradNum"				nvarchar(50)
		,"BuyerName"				nvarchar(100)
		,"BuyerPhone"				nvarchar(100)
		,"Cellular"					nvarchar(50)
		,"E_Mail"					nvarchar(100)
		,"PaymentTerm"				nvarchar(100)
		,"SONo"						nvarchar(30)
		,"ContractNo"				nvarchar(5000)
		
		,"VatSum"					decimal(21,6)
		,"DocTotal"					decimal(21,6)
		,"DocCur"					nvarchar(10)
		,"DocRate"					decimal(21,6)
		,"U_InvNo"					nvarchar(20)
		,"U_InvDate"				nvarchar(100)		
		,"U_InvSerial"				nvarchar(20)
		,"U_invSym"					nvarchar(20)
		,"U_eInvTransID"			nvarchar(50)
		,"U_ARNo"					int
		,"U_ARReason"				nvarchar(5000)
		,"U_InvoiceStatus"			nvarchar(10)
		,"IsIncrease"				nvarchar(10)
		,"PartnerInvoiceStringID"	nvarchar(100)
		,"partnerGUID"				nvarchar(100)
		,"CmdType"					int
		,"UserDefine"				nclob
	
		,"LineNum"					int
		,"ItemTypeID"				int --0: Hàng hoá dịch vụ (mặc định); 1: Thuế khác; 2: Phí khác; 3: Phí phục vụ; 4: Ghi chú; 10: Tiền đất giảm trừ khi tính thuế
		,"ItemCode"					nvarchar(50)
		,"ItemName"					nvarchar(100)
		,"UoMName"					nvarchar(100)
		,"Quantity"					decimal(21,6) default 0
		,"UnitPrice"				decimal(21,6) default 0
		,"Amount"					decimal(21,6) default 0
		,"TaxRateID"				int default 0
		,"TaxRate"					int default 0
		,"TaxAmount"				decimal(21,6) default 0
		,"IsDiscount" 				int default 0
		,"OriginalInvoiceIdentify"	nvarchar(100)
	);
	-- AR Invoice
	insert into #tbl(	"DocCode",													"ObjType",			"ObjName",			
						"DocEntry",			"DocNum",		"DocDate",				"DocRate",			"Comments",			"U_ARNo",			"U_ARReason",		"U_InvoiceStatus"
						,"CardCode",		"CardName",		"Address",														"LidTradNum",		"BuyerName",		"BuyerPhone",		"Cellular", 		"E_Mail", 		"PaymentTerm"
						,"VatSum",			"DocCur",		"DocTotal"
						,"LineNum",			"ItemTypeID",	"ItemCode",				"ItemName",			"UoMName",			"Quantity",			"UnitPrice",		"Amount"
						,"TaxRateID",																						"TaxRate",			"TaxAmount",		"IsDiscount")

	select 				(a."ObjType" || '.' || a."DocEntry" || '.' || a."DocNum"),	a."ObjType",		case when a."isIns" = 'Y' then 'A/R Reserve Invoice' else 'A/R Invoice' end
						,a."DocEntry",		a."DocNum",		a."DocDate",			a."U_DocRate",		a."Comments",		a."U_ARNo",			a."U_ARReason",		a."U_InvoiceStatus"
						,a."CardCode",		a."CardName",	replace(replace(c."Address",'\','/'), char(13),','),			c."LicTradNum",		a."CardCode",		'',					c."Cellular",		c."E_Mail",		d."PymntGroup"
						,a."VatSum",		a."DocCur",		case when a."DocCur" = MainCurr then a."DocTotal" else a."DocTotalFC" end
						,b."LineNum",		0,				b."ItemCode",			b."Dscription",		b."unitMsr",		b."Quantity",		b."Price",			case when a."DocCur" = MainCurr then b."LineTotal" else b."TotalFrgn" end
						,(case  b."VatPrcnt" when 0 then 1 when 5 then 2 when 8 then 9 when 10 then 3 else 4 end),			b."VatPrcnt",		b."VatSum", 		case sign(b."Quantity") when -1 then 1 else 0 end
	from	OINV a
			inner join INV1 b 	on a."DocEntry" = b."DocEntry"
			inner join OCRD c	on c."CardCode" = a."CardCode"
			left join OCTG d	on d."GroupNum" = a."GroupNum"
			left join OUOM e 	on e."UomCode"	= b."UomCode"
	where
		a."CANCELED" = 'N'
		and IFNULL(NULLIF(a."U_InvNo",''),'')=''
		and cast(a."DocEntry" as nvarchar(11)) = $DocEntry
		and cast(b."ObjType" as nvarchar(11)) = $ObjType;
	
	-- AP Credit Memo
	insert into #tbl(	"DocCode",													"ObjType",			"ObjName",			
						"DocEntry",			"DocNum",		"DocDate",				"DocRate",			"Comments",			"U_ARNo",			"U_ARReason",		"U_InvoiceStatus"
						,"CardCode",		"CardName",		"Address",														"LidTradNum",		"BuyerName",		"BuyerPhone",	"Cellular", 		"E_Mail", 		"PaymentTerm"
						,"VatSum",			"DocCur",		"DocTotal"
						,"LineNum"			,"ItemTypeID",	"ItemCode",				"ItemName",			"UoMName",			"Quantity",			"UnitPrice",		"Amount"
						,"TaxRateID",																						"TaxRate",			"TaxAmount",		"IsDiscount")

	select 				(a."ObjType" || '.' || a."DocEntry" || '.' || a."DocNum"),	a."ObjType",		'A/P Credit Memo'
						,a."DocEntry",		a."DocNum",		a."DocDate",			a."U_DocRate",		a."Comments",		a."U_ARNo",			a."U_ARReason",		a."U_InvoiceStatus"
						,a."CardCode",		a."CardName",	replace(replace(c."Address",'\','/'), char(13),','),			c."LicTradNum",		a."CardCode",		'',				c."Cellular",		c."E_Mail",		d."PymntGroup"
						,a."VatSum",		a."DocCur",		case when a."DocCur" = MainCurr then a."DocTotal" else a."DocTotalFC" end
						,b."LineNum",		0,				b."ItemCode",			b."Dscription",		b."unitMsr",		b."Quantity",		b."Price",			case when a."DocCur" = MainCurr then b."LineTotal" else b."TotalFrgn" end
						,(case b."VatPrcnt" when 0 then 1 when 5 then 2 when 8 then 9 when 10 then 3 else 4 end),			b."VatPrcnt",		b."VatSum", 		case sign(b."Quantity") when -1 then 1 else 0 end
	from 
		ORPC a
		inner join RPC1 b 	on a."DocEntry" = b."DocEntry"
		inner join OCRD c	on c."CardCode" = a."CardCode"
		left join OCTG d	on d."GroupNum" = a."GroupNum"
		left join OUOM e 	on e."UomCode"	= b."UomCode"
	where
		a."CANCELED" = 'N'
		and IFNULL(NULLIF(a."U_InvNo",''),'')=''
		and cast(a."DocEntry" as nvarchar(11)) = $DocEntry
		and cast(b."ObjType" as nvarchar(11)) = $ObjType;

	-- AR Credit Memo
	insert into #tbl(	"DocCode",													"ObjType",			"ObjName",			
						"DocEntry",			"DocNum",		"DocDate",				"DocRate",			"Comments",			"U_ARNo",			"U_ARReason",		"U_InvoiceStatus"
						,"CardCode",		"CardName",		"Address",														"LidTradNum",		"BuyerName",		"BuyerPhone",		"Cellular", 		"E_Mail", 		"PaymentTerm"
						,"VatSum",			"DocCur",		"DocTotal"
						,"LineNum"			,"ItemTypeID",	"ItemCode",				"ItemName",				"UoMName",		"Quantity",			"UnitPrice",		"Amount"
						,"TaxRateID",																						"TaxRate",			"TaxAmount",		"IsDiscount")

	select 				(a."ObjType" || '.' || a."DocEntry" || '.' || a."DocNum"),	a."ObjType",		case when a."isIns" = 'Y' then 'A/R Reserve Invoice' else 'A/R Credit Memo' end
						,a."DocEntry",		a."DocNum",		a."DocDate",			a."U_DocRate",		a."Comments",		a."U_ARNo",			a."U_ARReason",		a."U_InvoiceStatus"
						,a."CardCode",		a."CardName",	replace(replace(c."Address",'\','/'), char(13),','),			c."LicTradNum",		a."CardCode",		'',					c."Cellular",		c."E_Mail",		d."PymntGroup"
						,a."VatSum",		a."DocCur",		case when a."DocCur" = MainCurr then a."DocTotal" else a."DocTotalFC" end
						,b."LineNum",		0,				b."ItemCode",			b."Dscription",			b."unitMsr",	b."Quantity",		b."Price",			case when a."DocCur" = MainCurr then b."LineTotal" else b."TotalFrgn" end
						,(case  b."VatPrcnt" when 0 then 1 when 5 then 2 when 8 then 9 when 10 then 3 else 4 end),			b."VatPrcnt",		b."VatSum", 		case sign(b."Quantity") when -1 then 1 else 0 end
	from	ORIN a
			inner join RIN1 b 	on a."DocEntry" = b."DocEntry"
			inner join OCRD c	on c."CardCode" = a."CardCode"
			left join OCTG d	on d."GroupNum" = a."GroupNum"
			left join OUOM e 	on e."UomCode"	= b."UomCode"
	where
		a."CANCELED" = 'N'
		and IFNULL(NULLIF(a."U_InvNo",''),'')=''
		and cast(a."DocEntry" as nvarchar(11)) = $DocEntry
		and cast(b."ObjType" as nvarchar(11)) = $ObjType;


	--Thêm ghi chú
	create local temporary table #tblSO("ARType" int, "AREntry" int, "SOEntry" int, "SONum" int, "ContractNo" nvarchar(50));
	insert into #tblSO("ARType", "AREntry", "SOEntry", "SONum", "ContractNo")
	select 	distinct a."ObjType", a."DocEntry", c."DocEntry", d."DocNum", d."U_ContractNo"
	from INV1 a inner join DLN1 b on b."ObjType" = a."BaseType" and b."DocEntry" = a."BaseEntry"
				inner join RDR1 c on c."ObjType" = b."BaseType" and c."DocEntry" = b."BaseEntry"
				inner join ORDR d on d."DocEntry" = c."DocEntry"
	where a."ObjType" || a."DocEntry" in (select "ObjType" || "DocEntry" from #tbl);
	
	--26/12/2024 thay doi tu so SONo sang U_ContractNo
	/*
	update x set x."SONo" = y."SONum" from #tbl x inner join #tblSO y on y."ARType" = x."ObjType" and y."AREntry" = x."DocEntry";
	select string_agg("SONum", ', ') into SONumber from #tblSO;
	if ifnull(SONumber, '') <> '' then
		insert into #tbl("LineNum", "ItemTypeID", "ItemName", "SONo", "DocCode", "ObjType", "ObjName", "DocEntry", "DocNum", "DocDate", "DocRate", "Comments", "U_ARNo", "U_ARReason", "U_InvoiceStatus", "CardCode", "CardName", "Address", "LidTradNum", "BuyerName", "BuyerPhone", "Cellular", "E_Mail", "PaymentTerm", "VatSum", "DocCur", "DocTotal", "TaxRateID")
		select top 1 ifnull((select max("LineNum") from #tbl),0) + 1,
									4, 'Contract No: ' || SONumber, "SONo", "DocCode", "ObjType", "ObjName", "DocEntry", "DocNum", "DocDate", "DocRate", "Comments", "U_ARNo", "U_ARReason", "U_InvoiceStatus", "CardCode", "CardName", "Address",	"LidTradNum", "BuyerName", "BuyerPhone", "Cellular", "E_Mail", "PaymentTerm", "VatSum", "DocCur", "DocTotal", "TaxRateID"
		from #tbl
		where "ItemTypeID" = 0;
	end if;
	*/
		
	update x set x."SONo" = y."SONum", x."ContractNo" = y."ContractNo" from #tbl x inner join #tblSO y on y."ARType" = x."ObjType" and y."AREntry" = x."DocEntry";
	select string_agg("ContractNo", ', ') into ContractNo from #tblSO;
	if ifnull(ContractNo, '') <> '' then
		insert into #tbl("LineNum", "ItemTypeID", "ItemName", "SONo", "DocCode", "ObjType", "ObjName", "DocEntry", "DocNum", "DocDate", "DocRate", "Comments", "U_ARNo", "U_ARReason", "U_InvoiceStatus", "CardCode", "CardName", "Address", "LidTradNum", "BuyerName", "BuyerPhone", "Cellular", "E_Mail", "PaymentTerm", "VatSum", "DocCur", "DocTotal", "TaxRateID")
		select top 1 ifnull((select max("LineNum") from #tbl),0) + 1,
									4, 'Contract No: ' || ContractNo, "SONo", "DocCode", "ObjType", "ObjName", "DocEntry", "DocNum", "DocDate", "DocRate", "Comments", "U_ARNo", "U_ARReason", "U_InvoiceStatus", "CardCode", "CardName", "Address",	"LidTradNum", "BuyerName", "BuyerPhone", "Cellular", "E_Mail", "PaymentTerm", "VatSum", "DocCur", "DocTotal", "TaxRateID"
		from #tbl
		where "ItemTypeID" = 0;
	end if;
	
	--Kết thúc thêm ghi chú
	
	--Bổ sung thông tin đặc thù của doanh nghiệp
	update	x
	set		x."UserDefine" = '{\"QuyDoiVND\": \"' || TO_VARCHAR(round(x."DocTotal" * x."DocRate", 0), '9,999') || '\"}'
	from	#tbl x
	where	x."DocCur" <> 'VND';
	
	--Kết thúc bổ sung thông tin đặc thù
	
	
	EINVOICE = select top 1 "U_PartnerGUID", "U_InvoiceForm", "U_InvoiceSerial" from "@EINVOICE" order by "Code" desc;
	
	update	x
	set		x."U_InvDate" 	= replace(TO_NVARCHAR(CURRENT_TIMESTAMP, 'yyyy-mm-dd hh24:mi:ss'),' ','T') || '.0000000+07:00',
			x."U_invSym"  	= y."U_InvoiceForm",
			x."U_InvSerial"	= y."U_InvoiceSerial",
			x."U_InvNo"		= '0',
			x."PartnerInvoiceStringID" = x."DocCode",
			x."partnerGUID"	= y."U_PartnerGUID",
			x."IsIncrease" = case x."U_InvoiceStatus" when '1' then 'true' when '2' then 'false' else '' end
	from	#tbl x left join $EINVOICE y on 1=1;

	--Cập nhật Thông tin Hoá đơn gốc dùng trong trường hợp thay thế, điều chỉnh
	update	x
	set		x."OriginalInvoiceIdentify" = '[' || ifnull(y."U_invSym",'') || ']_[' || ifnull(y."U_InvSerial",'') || ']_[' || ifnull(y."U_InvNo",'') || ']'
	from	#tbl x inner join OINV y on x."ObjType" = y."ObjType" and x."U_ARNo" = y."DocEntry";
	
	update	x
	set		x."OriginalInvoiceIdentify" = '[' || ifnull(y."U_invSym",'') || ']_[' || ifnull(y."U_InvSerial",'') || ']_[' || ifnull(y."U_InvNo",'') || ']'
	from	#tbl x inner join ORPC y on x."ObjType" = y."ObjType" and x."U_ARNo" = y."DocEntry";

	update	x
	set		x."OriginalInvoiceIdentify" = '[' || ifnull(y."U_invSym",'') || ']_[' || ifnull(y."U_InvSerial",'') || ']_[' || ifnull(y."U_InvNo",'') || ']'
	from	#tbl x inner join ORIN y on x."ObjType" = y."ObjType" and x."U_ARNo" = y."DocEntry";

	--Trường hợp A/R Credit Memo chọn tham chiếu từ A/R Invoice
	update	x
	set		x."OriginalInvoiceIdentify" = '[' || ifnull(y."U_invSym",'') || ']_[' || ifnull(y."U_InvSerial",'') || ']_[' || ifnull(y."U_InvNo",'') || ']'
	from	#tbl x inner join OINV y on x."ObjType" = 14 and x."U_ARNo" = y."DocEntry";
		
	--Cập nhật mã số thuế là trống nếu không đúng cấu trúc quy định của BTC
	update	x
	set		x."LidTradNum" = ''
	from	#tbl x
	where 	LOCATE_REGEXPR(START '\D' in replace(x."LidTradNum",'-','')) > 0
		or 	LOCATE_REGEXPR(START '\D' in left(x."LidTradNum",10)) > 0
		or 	LOCATE_REGEXPR(START '\D' in right(x."LidTradNum",3)) > 0
		or 	length(x."LidTradNum") not in (10, 14);
		
	select * from #tbl order by "DocDate", "DocEntry";
	drop table #tbl;
	drop table #tblSO;
END
