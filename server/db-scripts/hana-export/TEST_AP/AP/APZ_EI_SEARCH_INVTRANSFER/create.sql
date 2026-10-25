CREATE PROCEDURE APZ_EI_Search_InvTransfer(
	in FDate	date,
	in TDate	date,
	in ObjType		nvarchar(20),
	in FromWhsCode	nclob,
	in ToWhsCode	nclob
)
AS
BEGIN
	declare MainCurr nvarchar(30);
	declare pKeyOption nvarchar(15)='';
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
	
	--SELECT '.' || TO_VARCHAR(CURRENT_TIMESTAMP, 'YYMMDDHH24MISS') into pKeyOption default '' FROM DUMMY;
	/*
	declare ListToWhs table (WhsCode nvarchar(50));
	if (ifnull($ToWhsCode, '') = '') then
		insert into :ListToWhs (WhsCode)
		select "WhsCode" from OWHS where "WhsCode" like 'O%';
	else
		insert into :ListToWhs (WhsCode)
		select Data from split($ToWhsCode,',');
	end if;
	*/
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
		,"TransporterName"			nvarchar(1000) --Người vận chuyển
		,"Transportation"			nvarchar(1000) --Phương tiện vận chuyển
		
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
		,"eInvMesseage"				nvarchar(5000)
		,"PartnerInvoiceStringID"	nvarchar(100)
		,"partnerGUID"				nvarchar(100)
	);
	
	-- Inventory Transfer
	if :ObjType = '67' then
		insert into #tbl(	"DocCode",															"ObjType",		"ObjName"
							,"DocEntry",	"DocNum",		"DocDate",			"Comments"
							,"CardCode",	"CardName"
							,"Address"
							,"LidTradNum",	"BuyerName",	"BuyerPhone",		"Cellular", 	"E_Mail", 		"PaymentTerm"
							,"FromWhsCode",	"FromWhsName",	"ToWhsCode",		"ToWhsName"
							,"ContractNo",	"ShiftCommandNo","TransporterName",	"Transportation"
							,"VatSum",		"DocCur",		"DocTotal"		
							,"U_InvNo",		"U_InvDate",	"U_eInvTransID",	"U_eInvMTC")
	
		select 				(a."ObjType" || '.' || a."DocEntry" || '.' || a."DocNum"),			a."ObjType",	'Inventory Transfer'
							,a."DocEntry",	a."DocNum",		a."DocDate",		a."Comments"
							,a."CardCode",	a."CardName"
							,replace(replace(replace(a."Address",'\','/'), char(13),','),',,,VIETNAM',', Việt Nam')
							,c."LicTradNum",p."Name",		'',					c."Cellular",	c."E_Mail",		d."PymntGroup"
							,a."Filler",	w1."WhsName",	a."ToWhsCode",		w2."WhsName"
							,a."U_ContractNo",a."DocNum",	'' "TransporterName",ifnull(a."U_ContNo",'') as "U_ContNo"
							,a."VatSum",	a."DocCur",		case when a."DocCur" = MainCurr then a."DocTotal" else a."DocTotalFC" end
							,a."U_InvNo",	a."U_InvDate",	a."U_eInvTransID",	a."U_eInvMTC"
		from	OWTR a
				left join OCRD c	on c."CardCode" = a."CardCode"
				left join OCTG d	on d."GroupNum" = a."GroupNum"
				left join OWHS w1	on w1."WhsCode" = a."Filler"
				left join OWHS w2	on w2."WhsCode" = a."ToWhsCode"
				left join OCPR p	on p."CntctCode" = a."CntctCode" and p."CardCode" = a."CardCode"
		where
			a."CANCELED" = 'N' and IFNULL(a."U_ARNo",0) = 0
			and (IFNULL(a."U_InvNo",'') = '' and IFNULL(a."U_eInvTransID",'') = '')
			and a."DocDate" between $FDate and $TDate
			and ((ifnull($FromWhsCode, '') = '') or (a."Filler" in (select Data from split($FromWhsCode,','))))
			--and a."ToWhsCode" in (select WhsCode from :ListToWhs)
			and ((ifnull($ToWhsCode, '') = '') or (a."ToWhsCode" in (select Data from split($ToWhsCode,','))))
			and a."U_TransferType" = '2';
			
			-- Visible Context Action
			--update APZ_OCTA set "Invisible" = 'Y' where "ItemId" ='APZ_EI_Search_Transf' and "UniqueAlias" ='Auto_HSM';
			--update APZ_OCTA set "Invisible" = 'N' where "ItemId" ='APZ_EI_Search_Transf' and "UniqueAlias" ='Draft';
	--Delivery
	elseif :ObjType = '15' then	
		-- BO dieu kien nay, vi co the ho quen gui web ma da xuat Invoice roi.
		--Comment 02/06/2026: viewARInvoice = select distinct b."BaseEntry" from OINV a inner join INV1 b on b."DocEntry" = a."DocEntry" where a."CANCELED" = 'N' and b."BaseType" = '15';
		insert into #tbl(	"DocCode",													"ObjType",		"ObjName"
							,"DocEntry",	"DocNum",		"DocDate",			"Comments"
							,"CardCode",	"CardName"
							,"Address"
							,"LidTradNum",	"BuyerName",	"BuyerPhone",		"Cellular", 	"E_Mail", 	"PaymentTerm"
							,"FromWhsCode",	"FromWhsName",	"ToWhsCode",		"ToWhsName"
							,"ContractNo",	"ShiftCommandNo","TransporterName",	"Transportation"
							,"VatSum",		"DocCur",		"DocTotal"		
							,"U_InvNo",		"U_InvDate",	"U_eInvTransID",	"U_eInvMTC")
	
		select 	distinct	(a."ObjType" || '.' || a."DocEntry" || '.' || a."DocNum"),	a."ObjType",	'Delivery'
							,a."DocEntry",	a."DocNum",		a."DocDate",		a."Comments"
							,a."CardCode",	a."CardName"
							,replace(replace(replace(a."Address",'\','/'), char(13),','),',,,VIETNAM',', Việt Nam')
							,c."LicTradNum",p."Name",		'',					c."Cellular",	c."E_Mail",	d."PymntGroup"
							,b."WhsCode",	w1."WhsName",	a."U_To_Whse",		w2."WhsName"
							,a."U_ContractNo",a."DocNum",	'' "TransporterName",ifnull(a."U_ContNo",'') as "U_ContNo"
							,a."VatSum",	a."DocCur",		case when a."DocCur" = MainCurr then a."DocTotal" else a."DocTotalFC" end
							,a."U_InvNo",	a."U_InvDate",	a."U_eInvTransID",	a."U_eInvMTC"
		from	ODLN a
				left join (select "DocEntry", "WhsCode" from (
							select "DocEntry", "WhsCode", ROW_NUMBER() OVER (PARTITION BY "DocEntry" ORDER BY "LineNum") as rn from DLN1) t where rn = 1
							) b 	on b."DocEntry" = a."DocEntry"
				left join OCRD c	on c."CardCode" = a."CardCode"
				left join OCTG d	on d."GroupNum" = a."GroupNum"
				left join OWHS w1	on w1."WhsCode" = b."WhsCode"
				left join OWHS w2	on w2."WhsCode" = a."U_To_Whse"
				left join OCPR p	on p."CntctCode" = a."CntctCode" and p."CardCode" = a."CardCode"
		where
			a."CANCELED" = 'N' and IFNULL(a."U_ARNo",0) = 0
			and (IFNULL(a."U_InvNo",'') = '' and IFNULL(a."U_eInvTransID",'') = '')
			and a."DocDate" between $FDate and $TDate
			and ((ifnull($FromWhsCode, '') = '') or (b."WhsCode" in (select Data from split($FromWhsCode,','))))
			and ((ifnull($ToWhsCode, '') = '') or (a."U_To_Whse" in (select Data from split($ToWhsCode,','))))
			and a."U_Term" = 'FOB';
			--and a."DocEntry" not in (select "BaseEntry" from :viewARInvoice)
			
			-- Visible Context Action
			--update APZ_OCTA set "Invisible" = 'N' where "ItemId"= 'APZ_EI_Search_Transf' and "UniqueAlias" ='Auto_HSM';
			--update APZ_OCTA set "Invisible" = 'Y' where "ItemId"= 'APZ_EI_Search_Transf' and "UniqueAlias" ='Draft';
	end if;
	
	--Cập nhật địa chỉ kho xuất, địa chỉ kho nhập, tỷ giá
	update	x
	set		x."FromWhsAddress" = (select top 1 "Street" from ADM1),
			x."ToWhsAddress" = x."Address",
			x."DocRate" = y."Rate"
	from	#tbl x
			left join ORTT y on y."RateDate" = x."DocDate" and y."Currency" = 'VND';
	
	EINVOICE = select top 1 "U_PartnerGUID", "U_TransferForm", "U_TransferSerial" from "@EINVOICE" order by "Code" desc;
	
	update	x
	set		x."U_invSym"  	= y."U_TransferForm",
			x."U_InvSerial"	= y."U_TransferSerial",
			x."U_InvNo"		= '0',
			x."PartnerInvoiceStringID" = x."DocCode",
			x."partnerGUID"	= y."U_PartnerGUID"
	from	#tbl x left join $EINVOICE y on 1=1;
	
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