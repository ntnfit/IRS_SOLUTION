CREATE procedure VAS_F_Invoice_Bkup(
	in ObjType	int,
	in DocID	int
	
)
as
begin
	create local temporary table #tbl (
		CompName		nvarchar(100)
		,CompAdd		nvarchar(254)
		,DocEntry		int
		,BaseEntry		int
		,BaseLine		int
		,BaseType		int
		,InvoiceNo		nvarchar(50)
		,DocDate		timestamp
		,BPCode			nvarchar(50)
		,BPName			nvarchar(250)
		,BPAddr			nvarchar(250)
		,ShipTo			nvarchar(250)
		,ShipToAddress	nvarchar(250)
		,Vessel			nvarchar(50)
		,ETD			timestamp
		,FromPort		nvarchar(250)
		,ToPort			nvarchar(250)
		,SONo			nvarchar(50)
		,ItemCode		nvarchar(100)
		,ItemName		nvarchar(100)
		,Qty			numeric(21,6)
		,Uom			nvarchar(30)
		,UnitPrice		numeric(21,6)
		,Amount			numeric(21,6)
		,Origin			nvarchar(50)
		,Incoterm		nvarchar(50)
		,GroupNum		int
		,PaymentTerm	nvarchar(50)
		,Curr			nvarchar(10)
		,Decimal_Qty		int default 2
		,Decimal_UnitPrice  int default 5
		,Decimal_Amount  int default 2
		,BankInfo		nvarchar(250)
	);
	if :ObjType = '15' then
	insert into #tbl(DocEntry, BaseEntry, BaseLine, BaseType, InvoiceNo, DocDate, BPCode, BPName, Vessel, ETD, FromPort, ToPort, ItemCode, ItemName, 
						Qty, Uom, UnitPrice, Amount, GroupNum, Incoterm, 
						ShipTo, 
						ShipToAddress,
						Curr,Origin)
	select a."DocEntry", b."BaseEntry", b."BaseLine", b."BaseType", a."DocNum", a."DocDate", a."CardCode", upper(a."CardName"), upper(a."U_Vessel"), a."U_ETD", a."U_FromPort", a."U_ToPort", b."ItemCode", 
						b."Dscription", b."Quantity", b."unitMsr", b."Price", b."LineTotal", a."GroupNum", a."U_Incoterm",
						 upper(a."PickRmrk"),
						 a."Address2",
						 b."Currency",b."CountryOrg"
	from ODLN a 
	left join DLN1 b on a."DocEntry" = b."DocEntry"
	where a."DocEntry" = :DocID;
	
	else if :ObjType = '112' then
	
	insert into #tbl(DocEntry, BaseEntry, BaseLine, BaseType, InvoiceNo, DocDate, BPCode, BPName, Vessel, ETD, FromPort, ToPort, ItemCode, ItemName, 
						Qty, Uom, UnitPrice, Amount, GroupNum, Incoterm, 
						ShipTo,
						ShipToAddress,
						 Curr,Origin)
	select a."DocEntry", b."BaseEntry", b."BaseLine", b."BaseType", a."DocNum", a."DocDate", a."CardCode", upper(a."CardName"), upper(a."U_Vessel"), a."U_ETD", a."U_FromPort", a."U_ToPort", b."ItemCode", 
						b."Dscription", b."Quantity", b."unitMsr", b."Price", b."LineTotal", a."GroupNum", a."U_Incoterm",
						 upper(a."PickRmrk"),
						 a."Address2",
						 b."Currency",b."CountryOrg"
	from ODRF a 
	left join DRF1 b on a."DocEntry" = b."DocEntry"
	where a."DocEntry" = :DocID;
	
	end if;
	end if;
	
	update a set BPAddr =    ifnull(d."Street",'')
	--|| ifnull(d."Address2",'') ||   ', ' || ifnull(d."Street",'') || ', ' || 	ifnull( d."City",'')  || ', ' || ifnull(d."County",'') 
	from #tbl a 
	left join  OCRD c on c."CardCode" = a.BPCode
	left join CRD1 d on c."CardCode"  =  d."CardCode";
	
	update a set SONo = c."DocNum" from #tbl a left join RDR1 b on a.BaseEntry = b."DocEntry" and a.BaseLine = b."LineNum" and a.BaseType = 17 left join ORDR c on b."DocEntry" = c."DocEntry";
	--update a set ShipTo = c."Address2" from #tbl a left join RDR1 b on a.BaseEntry = b."DocEntry" and a.BaseLine = b."LineNum" and a.BaseType = 17 left join ORDR c on b."DocEntry" = c."DocEntry";
	update #tbl set CompName = (select "CompnyName" from "OADM");
	update #tbl set	CompAdd = (select SUBSTR_BEFORE("CompnyAddr", char(13)) from "OADM");
	update a set PaymentTerm = b."PymntGroup" from #tbl a left join OCTG b on a.GroupNum = b."GroupNum";
	select * from #tbl;
	drop table #tbl;
end

