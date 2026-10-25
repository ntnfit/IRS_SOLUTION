CREATE PROCEDURE APZ_EI_Search_AdjRep(
	in FDate	date,
	in TDate	date,
	in BPCode	nclob
)
AS
BEGIN
	declare MainCurr nvarchar(30);
	DECLARE v_ContextStatus  NVARCHAR(20);
	DECLARE v_ContextMessage NVARCHAR(200);
	
	/* =========================================================
       KIỂM TRA THÔNG SỐ HÓA ĐƠN ĐIỆN TỬ
	   ========================================================= */
	CALL "SP_CHECK_EINVOICE_CONTEXT_ACTION"
	(
	    v_ContextStatus,
	    v_ContextMessage
	);
	
	IF :v_ContextStatus = 'ERROR' THEN
		SELECT :v_ContextMessage AS "eInvMesseage"
		FROM DUMMY;
	    RETURN;
	END IF;
	
	select "MainCurncy" into MainCurr from OADM;
	
	if(FDate = '' or FDate is null) then
		FDate = to_date('1900-01-01', 'YYYY-MM-DD');
	end if;
	
	if(TDate = '' or TDate is null) then
		TDate = to_date('9999-01-01', 'YYYY-MM-DD');
	end if;
	create local temporary table #tbl(
		"DocCode"					nvarchar(100)
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
		,"ContractNo"				nvarchar(50)
		
		,"VatSum"					decimal(21,6)
		,"DocTotal"					decimal(21,6)
		,"DocCur"					nvarchar(10)
		,"DocRate"					decimal(21,6)
		,"U_InvNo"					nvarchar(20)
		,"U_InvDate"				date		
		,"U_InvSerial"				nvarchar(20)
		,"U_invSym"					nvarchar(20)
		,"U_eInvTransID"			nvarchar(100)
		,"U_eInvMTC"				nvarchar(100)
		,"U_ARNo"					int
		,"U_ARReason"				nvarchar(5000)
		,"U_InvoiceStatus"			nvarchar(10)
		,"IsIncrease"				nvarchar(10)
		,"ARType"					nvarchar(100)
		,"eInvMesseage"				nvarchar(5000)
		,"PartnerInvoiceStringID"	nvarchar(100)
		,"partnerGUID"				nvarchar(100)
		,"OriginalInvoiceIdentify"	nvarchar(100)
		,"InvoiceTypeID"			nvarchar(10) --4: Hóa đơn bán hàng (dành cho Tổ chức, Cá nhân trong khu PTQ); 5: Phiếu xuất kho & vận chuyển nội bộ
	);
	-- AR Invoice
	insert into #tbl(	"DocCode",															"ObjType",			"ObjName",
						"DocEntry",		"DocNum",		"DocDate",		"DocRate",			"Comments"
						,"CardCode",	"CardName",		"Address",												"LidTradNum",		"BuyerName",	"BuyerPhone",	"Cellular", 		"E_Mail", 	"PaymentTerm"
						,"VatSum",		"DocCur",		"DocTotal"
						,"U_invSym",	"U_InvSerial",	"U_InvNo",		"U_InvDate",		"U_eInvTransID",	"U_eInvMTC",		"U_ARNo",		"U_ARReason",	"U_InvoiceStatus",	"InvoiceTypeID")

	select 				(a."ObjType" || '.' || a."DocEntry" || '.' || a."DocNum"),			a."ObjType",		case when a."isIns" = 'Y' then 'A/R Reserve Invoice' else 'A/R Invoice' end
						,a."DocEntry",	a."DocNum",		a."DocDate",	a."U_DocRate",		a."Comments"
						,a."CardCode",	a."CardName",	replace(replace(c."Address",'\','/'), char(13),','),	c."LicTradNum",		a."CardCode",	'',				c."Cellular",		c."E_Mail",	d."PymntGroup"
						,a."VatSum",	a."DocCur",		case when a."DocCur" = MainCurr then a."DocTotal" else a."DocTotalFC" end
						,a."U_invSym",	a."U_InvSerial",a."U_InvNo",	a."U_InvDate",		a."U_eInvTransID",	a."U_eInvMTC",		a."U_ARNo",		a."U_ARReason",	a."U_InvoiceStatus",'4'
	from	OINV a
			--inner join INV1 b 	on a."DocEntry" = b."DocEntry"
			inner join OCRD c	on c."CardCode" = a."CardCode"
			left join OCTG d	on d."GroupNum" = a."GroupNum"
	where
		a."CANCELED" = 'N' and IFNULL(a."U_ARNo",0) <> 0
		and (IFNULL(a."U_InvNo",'') = '' and IFNULL(a."U_eInvTransID",'') = '')
		and a."DocDate" between $FDate and $TDate
		and ((ifnull($BPCode, '') = '') or (a."CardCode" in (select Data from split($BPCode,','))));
	
	-- A/P Credit Memo
	insert into #tbl(	"DocCode",															"ObjType",			"ObjName",			"DocEntry",		"DocNum",			"DocDate",			"DocRate",		"Comments"
						,"CardCode",	"CardName",		"Address",												"LidTradNum",		"BuyerName",	"BuyerPhone",		"Cellular", 		"E_Mail",		"PaymentTerm"
						,"VatSum",		"DocCur",		"DocTotal"
						,"U_invSym",	"U_InvSerial",	"U_InvNo",		"U_InvDate",		"U_eInvTransID",	"U_eInvMTC",		"U_ARNo",		"U_ARReason",		"U_InvoiceStatus",	"InvoiceTypeID")
	
	select 				(a."ObjType" || '.' || a."DocEntry" || '.' || a."DocNum"),			a."ObjType",		'A/P Credit Memo',	a."DocEntry",	a."DocNum",			a."DocDate",		a."U_DocRate",	a."Comments"
						,a."CardCode",	a."CardName",	replace(replace(c."Address",'\','/'), char(13),','),	c."LicTradNum",		a."CardCode",	'',					c."Cellular",		c."E_Mail",		d."PymntGroup"
						,a."VatSum",	a."DocCur",		case when a."DocCur" = MainCurr then a."DocTotal" else a."DocTotalFC" end
						,a."U_invSym",	a."U_InvSerial",a."U_InvNo",	a."U_InvDate",		a."U_eInvTransID",	a."U_eInvMTC",		a."U_ARNo",		a."U_ARReason",		a."U_InvoiceStatus",'4'
	from 
		ORPC a
		--inner join RPC1 b 	on a."DocEntry" = b."DocEntry"
		inner join OCRD c	on c."CardCode" = a."CardCode"
		left join OCTG d	on d."GroupNum" = a."GroupNum"
	where
		a."CANCELED" = 'N' and IFNULL(a."U_ARNo",0) <> 0
		and (IFNULL(a."U_InvNo",'') = '' and IFNULL(a."U_eInvTransID",'') = '')
		and a."DocDate" between $FDate and $TDate
		and ((ifnull($BPCode, '') = '') or (a."CardCode" in (select Data from split($BPCode,','))));		

	-- AR Credit Memo
	insert into #tbl(	"DocCode",															"ObjType",			"ObjName",
						"DocEntry",		"DocNum",		"DocDate",		"DocRate",			"Comments"
						,"CardCode",	"CardName",		"Address",												"LidTradNum",		"BuyerName",	"BuyerPhone",	"Cellular", 		"E_Mail", 	"PaymentTerm"
						,"VatSum",		"DocCur",		"DocTotal"
						,"U_invSym",	"U_InvSerial",	"U_InvNo",		"U_InvDate",		"U_eInvTransID",	"U_eInvMTC",		"U_ARNo",		"U_ARReason",	"U_InvoiceStatus",	"InvoiceTypeID")

	select 				(a."ObjType" || '.' || a."DocEntry" || '.' || a."DocNum"),			a."ObjType",		case when a."isIns" = 'Y' then 'A/R Reserve Invoice' else 'A/R Credit Memo' end
						,a."DocEntry",	a."DocNum",		a."DocDate",	a."U_DocRate",		a."Comments"
						,a."CardCode",	a."CardName",	replace(replace(c."Address",'\','/'), char(13),','),	c."LicTradNum",		a."CardCode",	'',				c."Cellular",		c."E_Mail",	d."PymntGroup"
						,a."VatSum",	a."DocCur",		case when a."DocCur" = MainCurr then a."DocTotal" else a."DocTotalFC" end
						,a."U_invSym",	a."U_InvSerial",a."U_InvNo",	a."U_InvDate",		a."U_eInvTransID",	a."U_eInvMTC",		a."U_ARNo",		a."U_ARReason",	a."U_InvoiceStatus",'4'
	from	ORIN a
			--inner join INV1 b 	on a."DocEntry" = b."DocEntry"
			inner join OCRD c	on c."CardCode" = a."CardCode"
			left join OCTG d	on d."GroupNum" = a."GroupNum"
	where
		a."CANCELED" = 'N' and IFNULL(a."U_ARNo",0) <> 0
		and (IFNULL(a."U_InvNo",'') = '' and IFNULL(a."U_eInvTransID",'') = '')
		and a."DocDate" between $FDate and $TDate
		and ((ifnull($BPCode, '') = '') or (a."CardCode" in (select Data from split($BPCode,','))));

	-- Inventory Transfer
	insert into #tbl(	"DocCode",															"ObjType",			"ObjName",
						"DocEntry",		"DocNum",		"DocDate",		"DocRate",			"Comments"
						,"CardCode",	"CardName",		"Address",												"LidTradNum",		"BuyerName",	"BuyerPhone",	"Cellular", 		"E_Mail", 	"PaymentTerm"
						,"VatSum",		"DocCur",		"DocTotal"
						,"U_invSym",	"U_InvSerial",	"U_InvNo",		"U_InvDate",		"U_eInvTransID",	"U_eInvMTC",		"U_ARNo",		"U_ARReason",	"U_InvoiceStatus",	"InvoiceTypeID")

	select 				(a."ObjType" || '.' || a."DocEntry" || '.' || a."DocNum"),			a."ObjType",		'Inventory Transfer'
						,a."DocEntry",	a."DocNum",		a."DocDate",	y."Rate",			a."Comments"
						,a."CardCode",	a."CardName",	replace(replace(c."Address",'\','/'), char(13),','),	c."LicTradNum",		a."CardCode",	'',				c."Cellular",		c."E_Mail",	d."PymntGroup"
						,a."VatSum",	'VND',			case when 'VND' = MainCurr then a."DocTotal" else a."DocTotalFC" end
						,a."U_invSym",	a."U_InvSerial",a."U_InvNo",	a."U_InvDate",		a."U_eInvTransID",	a."U_eInvMTC",		a."U_ARNo",		a."U_ARReason",	a."U_InvoiceStatus",'5'
	from	OWTR a
			--inner join INV1 b 	on a."DocEntry" = b."DocEntry"
			inner join OCRD c	on c."CardCode" = a."CardCode"
			left join OCTG d	on d."GroupNum" = a."GroupNum"
			left join ORTT y 	on y."RateDate" = a."DocDate" and y."Currency" = 'VND'
	where
		a."CANCELED" = 'N' and IFNULL(a."U_ARNo",0) <> 0
		and (IFNULL(a."U_InvNo",'') = '' and IFNULL(a."U_eInvTransID",'') = '')
		and a."DocDate" between $FDate and $TDate
		and ((ifnull($BPCode, '') = '') or (a."CardCode" in (select Data from split($BPCode,','))));
	
	--Thêm ghi chú
	create local temporary table #tblSO("ARType" int, "AREntry" int, "SOEntry" int, "SONum" int, "ContractNo" nvarchar(50));
	insert into #tblSO("ARType", "AREntry", "SOEntry", "SONum", "ContractNo")
	select 	distinct a."ObjType", a."DocEntry", c."DocEntry", d."DocNum", d."U_ContractNo"
	from INV1 a inner join DLN1 b on b."ObjType" = a."BaseType" and b."DocEntry" = a."BaseEntry"
				inner join RDR1 c on c."ObjType" = b."BaseType" and c."DocEntry" = b."BaseEntry"
				inner join ORDR d on d."DocEntry" = c."DocEntry"
	where a."ObjType" || a."DocEntry" in (select "ObjType" || "DocEntry" from #tbl);
	
	update x set x."SONo" = y."SONum", x."ContractNo" = y."ContractNo" from #tbl x inner join #tblSO y on y."ARType" = x."ObjType" and y."AREntry" = x."DocEntry";
	--Kết thúc thêm ghi chú
	
	EINVOICE = select top 1 "U_PartnerGUID" from "@EINVOICE" order by "Code" desc;
	
	update	x
	set		x."IsIncrease" = case x."U_InvoiceStatus" when 1 then 'true' when 2 then 'false' else '' end,
			x."PartnerInvoiceStringID" = x."DocCode",
			x."partnerGUID"	= y."U_PartnerGUID"
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
		
	update	x
	set		x."OriginalInvoiceIdentify" = '[' || ifnull(y."U_invSym",'') || ']_[' || ifnull(y."U_InvSerial",'') || ']_[' || ifnull(y."U_InvNo",'') || ']'
	from	#tbl x inner join OWTR y on x."ObjType" = y."ObjType" and x."U_ARNo" = y."DocEntry";
	
	update	x
	set		x."ARType" = y."Descr"
	from	#tbl x inner join UFD1 y on y."TableID" = 'OINV' and y."FieldID" = 39 and y."FldValue" = x."U_InvoiceStatus"
	where	x."ObjType" = 13;
	
	update	x
	set		x."ARType" = y."Descr"
	from	#tbl x inner join UFD1 y on y."TableID" = 'ORPC' and y."FieldID" = 39 and y."FldValue" = x."U_InvoiceStatus"
	where	x."ObjType" = 19;
	
	update	x
	set		x."ARType" = y."Descr"
	from	#tbl x inner join UFD1 y on y."TableID" = 'ORIN' and y."FieldID" = 39 and y."FldValue" = x."U_InvoiceStatus"
	where	x."ObjType" = 14;	

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