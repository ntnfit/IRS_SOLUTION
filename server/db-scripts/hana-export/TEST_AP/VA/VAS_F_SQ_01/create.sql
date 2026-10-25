CREATE procedure VAS_F_SQ_01 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	
	select ifnull(x."BPLId",0) into bplID from "OQUT" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		CompName 		nvarchar(300), 
		CompAddr 		nvarchar(300),
		CompTel 		nvarchar(30), 
		CompFax 		nvarchar(30), 
		CompWebsite 	nvarchar(100), 
		CompEmail 		nvarchar(100),
		RptName 		nvarchar(300) default N'BÁO GIÁ/SALES QUOTATION', 
		BPCode 			nvarchar(20), 
		BPName 			nvarchar(100), 
		BPAddress 		nvarchar(300), 
		BPTel 			nvarchar(50), 
		BPFax 			nvarchar(50), 
		BPContactPerson	nvarchar(100),
		SQNo 			nvarchar(50), 
		SQDate 			timestamp, 
		SQPersonName 	nvarchar(100), 
		SQPersonPhone 	nvarchar(50), 
		SQPersonEmail 	nvarchar(100),
		PrjCode 		nvarchar(20), 
		PrjName 		nvarchar(100),
		GrpCode 		nvarchar(20), 
		GrpName 		nvarchar(100),
		ItmCode 		nvarchar(50), 
		ItmName 		nvarchar(300), 
		Manufacturer 	nvarchar(100),
		UoM 			nvarchar(50), 
		Qty 			numeric(21,6), 
		Price 			numeric(21,6), 
		LineTotal 		numeric(21,6), 
		TotalVat 		numeric(21,6), 
		DocTotal 		numeric(21,6), 
		IncludeNote 	nvarchar(300) default '1. Chi phí lắp đặt, chuyển giao - đào tạo/ Installation, operation Cost', 
		WarrantyNote 	nvarchar(100), 
		DeliveryNote 	nvarchar(100), 
		PayTerm 		nvarchar(100), 
		ValidNote 		nvarchar(100), 
		Note 			nvarchar(300)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (BPCode,BPName,BPAddress,BPTel,BPFax,BPContactPerson,SQNo,SQDate,SQPersonName,SQPersonPhone,SQPersonEmail,PrjCode,PrjName,ItmCode,ItmName,Qty,UoM,Price,LineTotal,Manufacturer,GrpCode,GrpName,WarrantyNote,DeliveryNote,PayTerm,ValidNote,TotalVat,DocTotal)
	select a."CardCode",a."CardName",a."Address",d."Phone1",d."Fax",CONCAT(CONCAT(ifnull(c."FirstName",'') , ifnull(CONCAT(' ',c."MiddleName"),'')) , ifnull(CONCAT(' ',c."LastName"),'')), a."U_JVNumber",a."DocDate",e."SlpName",e."Telephone",e."Email",a."Project",f."PrjName",b."ItemCode",b."Dscription",b."Quantity",b."unitMsr",b."Price",b."LineTotal",'',h."ItmsGrpCod",h."ItmsGrpNam",'','','','',a."VatSum",a."DocTotal"
	from "OQUT" a
	left join "QUT1" b on b."DocEntry"=a."DocEntry"
	left join "OCPR" c on c."CntctCode"=a."CntctCode"
	left join "OCRD" d on d."CardCode"=a."CardCode"
	left join "OSLP" e on e."SlpCode"=a."SlpCode"
	left join "OPRJ" f on f."PrjCode"=a."Project"
	left join "OITM" g on g."ItemCode"=b."ItemCode"
	left join "OITB" h on h."ItmsGrpCod"=g."ItmsGrpCod"
	where a."DocEntry"=DocID;
	
	
	
	select x."CompnyName",x."CompnyAddr",x."Phone1",x."Fax",y."IntrntAdrs",x."E_Mail" into tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail from OADM x, ADM1 y ;
	
	update #tbl set CompName = tCompName, CompAddr = tCompAddr, CompTel = tCompTel, CompFax = tCompFax, CompWebsite = tCompWebsite, CompEmail = tCompEmail;

	select 
		CompName  "CompName",
		CompAddr  "CompAddr",
		CompTel  "CompTel",
		CompFax  "CompFax",
		CompWebsite  "CompWebsite",
		CompEmail  "CompEmail",
		RptName  "RptName",
		BPCode  "BPCode",
		BPName  "BPName",
		BPAddress  "BPAddress",
		BPTel  "BPTel",
		BPFax  "BPFax",
		BPContactPerson "BPContactPerson",
		SQNo  "SQNo",
		SQDate  "SQDate",
		SQPersonName  "SQPersonName",
		SQPersonPhone  "SQPersonPhone",
		SQPersonEmail  "SQPersonEmail",
		PrjCode  "PrjCode",
		PrjName  "PrjName",
		GrpCode  "GrpCode",
		GrpName  "GrpName",
		ItmCode  "ItmCode",
		ItmName  "ItmName",
		Manufacturer  "Manufacturer",
		UoM  "UoM",
		Qty  "Qty",
		Price  "Price",
		LineTotal  "LineTotal",
		TotalVat  "TotalVat",
		DocTotal  "DocTotal",
		IncludeNote  "IncludeNote",
		WarrantyNote  "WarrantyNote",
		DeliveryNote  "DeliveryNote",
		PayTerm  "PayTerm",
		ValidNote  "ValidNote",
		Note  "Note"
	from #tbl;

	drop table #tbl;
end

