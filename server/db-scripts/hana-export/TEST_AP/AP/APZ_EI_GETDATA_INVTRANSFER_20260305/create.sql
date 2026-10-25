CREATE PROCEDURE APZ_EI_GetData_InvTransfer_20260305
(
 	in ObjType 			nvarchar(100),
 	in DocEntry			nvarchar(100)
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
		,"BuyerName"				nvarchar(100) --Người nhận hàng
		,"BuyerPhone"				nvarchar(100)
		,"Cellular"					nvarchar(50)
		,"E_Mail"					nvarchar(100)
		,"PaymentTerm"				nvarchar(100)
		,"ContractNo"				nvarchar(5000)
		,"FromWhsCode"				nvarchar(50)
		,"FromWhsName"				nvarchar(1000)
		,"FromWhsAddress"			nvarchar(1000)
		,"ToWhsCode"				nvarchar(50)
		,"ToWhsName"				nvarchar(1000)
		,"ToWhsAddress"				nvarchar(1000)
		,"ShiftCommandNo"			nvarchar(1000) --Số lệnh điều động
		,"ShiftUnitName"			nvarchar(1000) --Của
		,"TransporterName"			nvarchar(1000) --Người vận chuyển
		,"Transportation"			nvarchar(1000) --Phương tiện vận chuyển
				
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
		,"UIDefine"					nclob
	
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
	-- Inventory Transfer
	insert into #tbl(	"DocCode",													"ObjType",			"ObjName",			
						"DocEntry",			"DocNum",		"DocDate",				"Comments",			"U_ARNo",			"U_ARReason",		"U_InvoiceStatus"
						,"CardCode",		"CardName",		"Address",														"LidTradNum",		"BuyerName",		"BuyerPhone",			"Cellular", 		"E_Mail", 		"PaymentTerm"
						,"FromWhsCode",		"FromWhsName",	"ToWhsCode",			"ToWhsName"
						,"ContractNo",		"ShiftCommandNo","TransporterName",		"Transportation"
						,"VatSum",			"DocCur",		"DocTotal"
						,"LineNum",			"ItemTypeID",	"ItemCode",				"ItemName",			"UoMName",			"Quantity",			"UnitPrice",		"Amount"
						,"TaxRateID",																						"TaxRate",			"TaxAmount",		"IsDiscount")

	select 				(a."ObjType" || '.' || a."DocEntry" || '.' || a."DocNum"),	a."ObjType",		'Inventory Transfer'
						,a."DocEntry",		a."DocNum",		a."DocDate",			a."Comments",		a."U_ARNo",			a."U_ARReason",		a."U_InvoiceStatus"
						,a."CardCode",		a."CardName",	
						replace(replace(replace(a."Address",'\','/'), char(13),','),',,,VIETNAM',', Việt Nam'),
						c."LicTradNum",		p."Name",			'',						c."Cellular",		c."E_Mail",		d."PymntGroup"
						,a."Filler",		w1."WhsName",	a."ToWhsCode",			w2."WhsName"
						,a."U_ContractNo",	a."DocNum",		'' "TransporterName",	a."U_ContNo"
						,a."VatSum",		a."DocCur",		case when a."DocCur" = MainCurr then a."DocTotal" else a."DocTotalFC" end
						,b."LineNum",		0,				b."ItemCode",			b."Dscription",		b."unitMsr",		b."Quantity",		0 as "Price",		0 as "Amount"
						,(case  b."VatPrcnt" when 0 then 1 when 5 then 2 when 8 then 9 when 10 then 3 else 4 end),			0 as "VatPrcnt",	0 as "VatSum", 		case sign(b."Quantity") when -1 then 1 else 0 end
	from	OWTR a
			inner join WTR1 b 	on a."DocEntry" = b."DocEntry"
			inner join OCRD c	on c."CardCode" = a."CardCode"
			left join OCTG d	on d."GroupNum" = a."GroupNum"
			left join OUOM e 	on e."UomCode"	= b."UomCode"
			left join OWHS w1	on w1."WhsCode" = a."Filler"
			left join OWHS w2	on w2."WhsCode" = a."ToWhsCode"
			left join OCPR p	on p."CntctCode" = a."CntctCode" and p."CardCode" = a."CardCode"
	where
		a."CANCELED" = 'N'
		and IFNULL(NULLIF(a."U_InvNo",''),'')=''
		and cast(a."DocEntry" as nvarchar(11)) = $DocEntry
		and cast(b."ObjType" as nvarchar(11)) = $ObjType;

	--Cập nhật địa chỉ kho xuất, địa chỉ kho nhập, thông tin cty
	update	x
	set		x."FromWhsAddress" = (select top 1 "Street" from ADM1),
			x."ToWhsAddress" = x."Address",
			x."CompName" = (select top 1 case when ifnull("PrintHeadr",'') <> '' then "PrintHeadr" else "CompnyName" end from OADM),
			x."DocRate" = y."Rate"
	from	#tbl x
			left join ORTT y on y."RateDate" = x."DocDate" and y."Currency" = 'VND';
	
	--Bổ sung thông tin đặc thù của doanh nghiệp
	update	x
	set		x."UIDefine" = '{\"ShiftCommandNo\":\"' || x."ShiftCommandNo" || '\",\"ShiftUnitName\":\"' || ifnull(x."CompName",'') || '\",\"TransporterName\":\"' || ifnull(x."TransporterName",'') || '\",\"OutWareHouse\":\"' || ifnull(x."FromWhsAddress",'') || '\",\"InWareHouse\":\"' || ifnull(x."ToWhsAddress",'') || '\",\"Transportation\":\"' || ifnull(x."Transportation",'') || '\"}'
	from	#tbl x;
	--Kết thúc bổ sung thông tin đặc thù
	
	EINVOICE = select top 1 "U_PartnerGUID", "U_TransferForm", "U_TransferSerial" from "@EINVOICE" order by "Code" desc;
	
	update	x
	set		x."U_InvDate" 	= replace(TO_NVARCHAR(CURRENT_TIMESTAMP, 'yyyy-mm-dd hh24:mi:ss'),' ','T') || '.0000000+07:00',
			x."U_invSym"  	= y."U_TransferForm",
			x."U_InvSerial"	= y."U_TransferSerial",
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

	update	x
	set		x."OriginalInvoiceIdentify" = '[' || ifnull(y."U_invSym",'') || ']_[' || ifnull(y."U_InvSerial",'') || ']_[' || ifnull(y."U_InvNo",'') || ']'
	from	#tbl x inner join OWTR y on x."ObjType" = y."ObjType" and x."U_ARNo" = y."DocEntry";
	
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

END
