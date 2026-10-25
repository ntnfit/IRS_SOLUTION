CREATE procedure VAS_F_PO_05 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	
	select ifnull(x."BPLId",0) into bplID from "OPOR" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		FormNo				nvarchar(50)
		, RptName			nvarchar(200)
		, CompName			nvarchar(200)
		, CompAdd			nvarchar(300)
		, CompTel			nvarchar(100)
		, CompFax			nvarchar(100)
		, CompTaxId			nvarchar(50)
		, CardCode			nvarchar(50)
		, CardName			nvarchar(200)
		, CardAddr			nvarchar(500)
		, CardAddr1			nvarchar(500)
		, ContactPerson		nvarchar(100)
		, Comments			nvarchar(300)
		, DocCur			nvarchar(10)
		, RequestNo			nvarchar(100) default N'REQUESTNO.001.001'
		, PONo				nvarchar(200)
		, DocDate			nvarchar(100)
		, ItemCode			nvarchar(50)
		, ItemName			nvarchar(200)
		, ItemNote			nvarchar(100)
		, Manf 				nvarchar(100)
		, Origin 			nvarchar(100)
		, WhsCode			nvarchar(100)
		, WhsName			nvarchar(100)
		, Unit				nvarchar(100)
		, Qty				numeric(21,6)
		, UnitPrice			numeric(21,6)
		, LineTotal			numeric(21,6)
		, DocTotal			numeric(21,6)
		, DocTotalFC		numeric(21,6)
		, DocRate			numeric(21,6)
		, VatAmt			numeric(21,6)
		, Total				numeric(21,6)
		, TotalInWords		nvarchar(500)
		, DelTime			nvarchar(500)
		, Project			nvarchar(100)
		, PayMethod			nvarchar(500)
		, FreightCost 		numeric(21,6)
		, TransferCost 		numeric(21,6)
		, EnginCost 		numeric(21,6)
		, OtherCost 		numeric(21,6)
		, TotalBefVAT 		numeric(21,6)
		, TotalVAT 			numeric(21,6)
		, TotalAftVAT 		numeric(21,6)
		, DelAdd			nvarchar(500)
		, PayTerm 			nvarchar(100)
		, BankInfor 		nvarchar(300)
		, Note 				nvarchar(300)
		, DecQty			int
		, DecPrice			int
		, DecAmt			int
		, DateSep			nvarchar(1)
		, ThousSep			nvarchar(1)
		, DateTitle			nvarchar(100)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (PONo,RequestNo,CardCode,CardName,CardAddr,CardAddr1,Comments,DocDate,DocCur,ItemCode,ItemName,Unit,Qty,UnitPrice,LineTotal,DocRate,WhsCode,Note,VatAmt,DocTotalFC,Project,DelTime,ContactPerson,TotalVAT,TotalAftVAT,TotalBefVAT,DelAdd,PayTerm,BankInfor)
	select
		a."U_JVNumber"
		,''
		,a."CardCode"
		,a."CardName"
		,replace(a."Address",'VIETNAM','')
		,replace(a."Address2",'VIETNAM','')
		,a."NumAtCard"
		,to_char(a."DocDate",'DD/MM/YYYY')
		,a."DocCur"
		,b."ItemCode"
		,b."Dscription"
		,ifnull(b."unitMsr",b."unitMsr2")
		,b."Quantity"
		,b."Price"
		,case when a."DocCur"=c."MainCurncy" then b."LineTotal" else b."TotalFrgn" end
		,a."DocRate"
		,b."WhsCode"
		,''
		,case when a."DocCur"=c."MainCurncy" then a."VatSum" else a."VatSumFC" end
		,a."DocTotalFC",
		b."Project",to_char(b."ShipDate",'DD/MM/YYYY'),
		d."FirstName",
		a."VatSum",a."DocTotal",a."DocTotal"-a."VatSum",
		a."Address2",'','Bank Informations'
	from 
		"OPOR" a
		left join "POR1" b on a."DocEntry"=b."DocEntry"
		left join "OADM" c on 1=1
		left join "OCPR" d on d."CntctCode"=a."CntctCode"
	where 
		a."CANCELED"='N' and a."DocEntry"=DocID;
	
	
	update #tbl set DocTotal = (select sum(LineTotal) from #tbl);
	update #tbl set Total = ifnull(DocTotal,0)+ifnull(VatAmt,0);
	UPDATE #tbl SET PayMethod = (SELECT b."Descript" FROM "OPOR" a,"OPYM" b WHERE a."PeyMethod"=b."PayMethCod");

	select top 1 floor(Total),(-floor(Total) + round(Total,2)) * 100 into num1,num2 from #tbl;

	update #tbl set TotalInWords = CONCAT(CONCAT(Num2Text (to_integer(num1)),' '),(select ("CurrName") from "OCRN" where "CurrCode" = #tbl.DocCur));

	if num2 <> 0 then
		update #tbl set TotalInWords = CONCAT(CONCAT(CONCAT(CONCAT(TotalInWords,' comma '),lower(Num2Text (to_integer(num2)))),' '),(select "Chk100Name" from "OCRN" where "CurrCode" = #tbl.Doccur));
	end if;

	update #tbl set WhsName = (select x."WhsName" from "OWHS" x where x."WhsCode"=#tbl.WhsCode);
	
	update #tbl set DateTitle = 'Ngày.......tháng.......năm......./ Date.....................'
					,RptName = 'ĐƠN ĐẶT HÀNG/ PURCHASE ORDER';
	
	select 2,a."DecSep",a."ThousSep",a."PriceDec" into DecQty,DateSep,ThousSep,DecPrice from "OADM" a;	
	update #tbl set DecQty = DecQty,DateSep = DateSep,ThousSep =ThousSep,DecPrice=DecPrice;

	update #tbl set DecAmt = case when (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.DocCur)='-1' then (select a."SumDec" from "OADM" a) else (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.DocCur) end;
	
	update #tbl set 
		CompTel = (select a."Phone1" from "OADM" a),
		CompFax = (select a."Fax" from "OADM" a);

	if bplID=0 then
		update #tbl set 
			CompName	=	(select a."CompnyName" from "OADM" a,"ADM1" b),
			CompAdd		=	(select b."Street" from "OADM" a,"ADM1" b),
			CompTaxId	=	(select a."TaxIdNum" from "OADM" a,"ADM1" b);
	end if;
	if bplID<>0 then
		update #tbl set 
			CompName	=(select x."BPLName" from "OBPL" x where x."BPLId"=bplID),
			CompAdd		=(select x."Street" from "OBPL" x where x."BPLId"=bplID),
			CompTaxId	=(select x."TaxIdNum" from "OBPL" x where x."BPLId"=bplID);
	end if;

		
	select 
		FormNo "FormNo",
		RptName "RptName",
		CompName "CompName",
		CompAdd "CompAdd",
		CompTel "CompTel",
		CompFax "CompFax",
		CompTaxId "CompTaxId",
		CardCode "CardCode",
		CardName "CardName",
		CardAddr "CardAddr",
		CardAddr1 "CardAddr1",
		ContactPerson "ContactPerson",
		Comments "Comments",
		DocCur "DocCur",
		RequestNo "RequestNo",
		PONo "PONo",
		DocDate "DocDate",
		ItemCode "ItemCode",
		ItemName "ItemName",
		ItemNote "ItemNote",
		Manf "Manf",
		Origin "Origin",
		WhsCode "WhsCode",
		WhsName "WhsName",
		Unit "Unit",
		Qty "Qty",
		UnitPrice "UnitPrice",
		LineTotal "LineTotal",
		DocTotal "DocTotal",
		DocTotalFC "DocTotalFC",
		DocRate "DocRate",
		VatAmt "VatAmt",
		Total "Total",
		TotalInWords "TotalInWords",
		DelTime "DelTime",
		Project "Project",
		PayMethod "PayMethod",
		FreightCost "FreightCost",
		TransferCost "TransferCost",
		EnginCost "EnginCost",
		OtherCost "OtherCost",
		TotalBefVAT "TotalBefVAT",
		TotalVAT "TotalVAT",
		TotalAftVAT "TotalAftVAT",
		DelAdd "DelAdd",
		PayTerm "PayTerm",
		BankInfor "BankInfor",
		Note "Note",
		DecQty "DecQty",
		DecPrice "DecPrice",
		DecAmt "DecAmt",
		DateSep "DateSep",
		ThousSep "ThousSep",
		DateTitle "DateTitle"
	from #tbl;

	drop table #tbl;
end

