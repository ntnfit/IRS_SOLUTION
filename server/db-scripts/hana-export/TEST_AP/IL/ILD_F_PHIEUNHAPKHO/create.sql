CREATE procedure ILD_F_PhieuNhapKho (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(150);
	
	select ifnull(x."BPLId",0) into bplID from "OIGN" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		DocID 				int
		, LineID 			int
		, FormNo			nvarchar(50)
		, RptName			nvarchar(200)
		, CompName			nvarchar(200)
		, CompAdd			nvarchar(300)
		, CompTel			nvarchar(100)
		, CompFax			nvarchar(100)
		, CompTaxId			nvarchar(50)
		, CompWebsite		nvarchar(100)
		, CompEmail			nvarchar(100)
		, CardCode			nvarchar(50)
		, CardName			nvarchar(200)
		, CardAddr			nvarchar(500)
		, CardAddr1			nvarchar(500)
		, ContactPerson		nvarchar(100)
		, Comments			nvarchar(300)
		, DocCur			nvarchar(10)
		, PONo				nvarchar(200)
		, Requester 		nvarchar(100)
		, Department 		nvarchar(100)
		, SalEmp			nvarchar(100)
		, DocDate			nvarchar(100)
		, ItemCode			nvarchar(50)
		, ItemName			nvarchar(200)
		, ItemNote			nvarchar(100)
		, Manf 				nvarchar(100)
		, Origin 			nvarchar(100)
		, FromWhsCode		nvarchar(20)
		, WhsCode			nvarchar(20)
		, WhsName			nvarchar(100)
		, Unit				nvarchar(100)
		, Qty				numeric(21,6)
		, InStock 			numeric(21,6)
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
		, Logo				nvarchar(100)
		, LogoUrl 			nvarchar(100)
		, DateTitle			nvarchar(100)
	);	

	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (DocID,LineID,SalEmp,PONo,CardCode,CardName,CardAddr,CardAddr1,Comments,DocDate,DocCur,ItemCode,ItemName,Unit,Qty,UnitPrice,LineTotal,DocRate,FromWhsCode,WhsCode,Note,VatAmt,DocTotalFC,Project,DelTime,ContactPerson,TotalVAT,TotalAftVAT,TotalBefVAT,PayTerm,BankInfor,DelAdd,Department)
	SELECT 
		a."DocEntry", b."LineNum", a."SlpCode",
		 --a."U_JVNumber",
		a."DocNum",
		  a."CardCode", a."CardName", replace(a."Address", 'VIETNAM', ''), replace(a."Address2", 'VIETNAM', ''), a."Comments", 
		to_char(a."DocDate",'DD/MM/YYYY'), 
		a."DocCur", b."ItemCode", b."Dscription", IFNULL(b."unitMsr", b."unitMsr2"), b."Quantity", b."Price", 
		CASE WHEN a."DocCur" = c."MainCurncy" THEN b."LineTotal" ELSE b."TotalFrgn" END, 
		a."DocRate", b."FromWhsCod", b."WhsCode", a."Comments", 
		CASE WHEN a."DocCur" = c."MainCurncy" THEN a."VatSum" ELSE a."VatSumFC" END, 
		a."DocTotalFC", b."Project", to_char(b."ShipDate", 'DD/MM/YYYY'), d."Name", a."VatSum", a."DocTotal", a."DocTotal" - a."VatSum", '', '', '', '' 
	FROM "OIGN" a 
	LEFT OUTER JOIN "IGN1" b ON a."DocEntry" = b."DocEntry" 
	LEFT OUTER JOIN "OADM" c ON 1 = 1 
	LEFT OUTER JOIN "OCPR" d ON d."CntctCode" = a."CntctCode" 
	WHERE a."CANCELED" = 'N' AND a."DocEntry" = DocID;



	update #tbl set WhsName = (select x."WhsName" from "OWHS" x where x."WhsCode"=#tbl.WhsCode);
	
	update #tbl set DateTitle = N'Ngày.......tháng.......năm......./ Date.....................',RptName = N'PHIẾU NHẬP KHO/GOODS RECEIPT';


	select x."CompnyName",x."CompnyAddr",x."Phone1",x."Fax",y."IntrntAdrs",x."E_Mail" into tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail from OADM x, ADM1 y ;
	
	update #tbl set CompName = tCompName, CompAdd = tCompAddr, CompTel = tCompTel, CompFax = tCompFax, CompWebsite = tCompWebsite, CompEmail = tCompEmail;
	

	if bplID=0 then
		update #tbl set 
			CompName 	= (select a."CompnyName" from "OADM" a,"ADM1" b),
			CompAdd 	= (select b."Street" from "OADM" a,"ADM1" b);
	end if;
	if bplID<>0 then
		update #tbl set 
			CompName	= (select x."BPLName" from "OBPL" x where x."BPLId"=bplID),
			CompAdd		= (select x."Street" from "OBPL" x where x."BPLId"=bplID);
	end if;	
	
	update #tbl
	set  CompAdd  =  SUBSTR_BEFORE(CompAdd,char(13));

	select 
		DocID  "DocID",
		LineID  "LineID",
		FormNo  "FormNo",
		RptName  "RptName",
		CompName  "CompName",
		CompAdd  "CompAdd",
		CompTel  "CompTel",
		CompFax  "CompFax",
		CompTaxId  "CompTaxId",
		CompWebsite  "CompWebsite",
		CompEmail  "CompEmail",
		CardCode  "CardCode",
		CardName  "CardName",
		CardAddr  "CardAddr",
		CardAddr1  "CardAddr1",
		ContactPerson  "ContactPerson",
		Comments  "Comments",
		DocCur  "DocCur",
		PONo  "PONo",
		Requester  "Requester",
		Department  "Department",
		SalEmp  "SalEmp",
		DocDate  "DocDate",
		ItemCode  "ItemCode",
		ItemName  "ItemName",
		ItemNote  "ItemNote",
		Manf  "Manf",
		Origin  "Origin",
		FromWhsCode  "FromWhsCode",
		WhsCode  "WhsCode",
		WhsName  "WhsName",
		Unit  "Unit",
		Qty  "Qty",
		InStock  "InStock",
		UnitPrice  "UnitPrice",
		LineTotal  "LineTotal",
		DocTotal  "DocTotal",
		DocTotalFC  "DocTotalFC",
		DocRate  "DocRate",
		VatAmt  "VatAmt",
		Total  "Total",
		TotalInWords  "TotalInWords",
		DelTime  "DelTime",
		Project  "Project",
		PayMethod  "PayMethod",
		TotalBefVAT  "TotalBefVAT",
		TotalVAT  "TotalVAT",
		TotalAftVAT  "TotalAftVAT",
		DelAdd  "DelAdd",
		PayTerm  "PayTerm",
		BankInfor  "BankInfor",
		Note  "Note",
		DecQty  "DecQty",
		DecPrice  "DecPrice",
		DecAmt  "DecAmt",
		DateSep  "DateSep",
		ThousSep  "ThousSep",
		Logo  "Logo",
		LogoUrl  "LogoUrl",
		DateTitle  "DateTitle"
	from #tbl;

	drop table #tbl;

end;

