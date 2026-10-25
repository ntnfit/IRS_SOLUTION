CREATE procedure VAS_F_PackingList_Bkup(
	in ObjType	int,
	in DocID	int
)
as
begin
	create local temporary table #tbl (
		CompName			nvarchar(100)
		,CompAdd			nvarchar(254)
		,DocEntry			int
		,BaseEntry			int
		,BaseLine			int
		,BaseType			int
		,InvoiceNo			nvarchar(50)
		,DocDate			timestamp
		,BPCode				nvarchar(50)
		,BPName				nvarchar(250)
		,BPAddr				nvarchar(250)
		,ShipTo				nvarchar(250)
		,ShipToAddress		nvarchar(250)
		,Vessel				nvarchar(50)
		,ETD				timestamp
		,FromPort			nvarchar(250)
		,ToPort				nvarchar(250)
		,SONo				nvarchar(50)
		,ItemCode			nvarchar(100)
		,ItemName			nvarchar(100)
		,Qty				numeric(21,6)
		,Uom				nvarchar(30)
		,Carton				numeric(21,6)
		,TotalNetWeight		numeric(21,6)
		,TotalGrossWeight	numeric(21,6)
		,TotalCBM			numeric(21,6)
		,Origin				nvarchar(50)
		,Incoterm			nvarchar(50)
		
	);
	if :ObjType = '15' then
	insert into #tbl(DocEntry, BaseEntry, BaseLine, BaseType, InvoiceNo, DocDate, BPCode, BPName, Vessel, ETD, FromPort, ToPort, ItemCode, ItemName, 
						Qty, Uom, Incoterm, 
						ShipTo,
						ShipToAddress)
	select a."DocEntry", b."BaseEntry", b."BaseLine", b."BaseType", a."DocNum", a."DocDate", a."CardCode", upper(a."CardName"), upper(a."U_Vessel"), a."U_ETD", a."U_FromPort", a."U_ToPort", b."ItemCode", 
						b."Dscription", b."Quantity", b."unitMsr", a."U_Incoterm", 
						case when  SUBSTR_AFTER (upper(a."PickRmrk"),'-') = '' then  upper(a."PickRmrk") 
						else LTRIM(SUBSTR_AFTER (upper(a."PickRmrk"),'-')) end,
						a."Address2"
	from ODLN a 
	left join DLN1 b on a."DocEntry" = b."DocEntry"
	where a."DocEntry" = :DocID;
	
	else if :ObjType = '112' then
	
	insert into #tbl(DocEntry, BaseEntry, BaseLine, BaseType, InvoiceNo, DocDate, BPCode, BPName, Vessel, ETD, FromPort, ToPort, ItemCode, ItemName, 
						Qty, Uom, Carton, Incoterm, 
						ShipTo,
						ShipToAddress)
	select a."DocEntry", b."BaseEntry", b."BaseLine", b."BaseType", a."DocNum", a."DocDate", a."CardCode", upper(a."CardName"), upper(a."U_Vessel"), a."U_ETD", a."U_FromPort", a."U_ToPort", b."ItemCode", 
						b."Dscription", b."Quantity", b."unitMsr", b."PackQty", a."U_Incoterm", 
						case when  SUBSTR_AFTER (upper(a."PickRmrk"),'-') = '' then  upper(a."PickRmrk") else LTRIM(SUBSTR_AFTER (upper(a."PickRmrk"),'-')) end,
						a."Address2"
	from ODRF a 
	left join DRF1 b on a."DocEntry" = b."DocEntry"
	where a."DocEntry" = :DocID;
	
	end if;
	end if;
	
update a set BPAddr =   ifnull(d."Street",'')
--|| d."Address" ||   ', ' || d."Street" || ', ' || d."City"  || ', ' || d."County" 
	from #tbl a 
	left join  OCRD c on c."CardCode" = a.BPCode
	left join CRD1 d on c."CardCode"  =  d."CardCode";
	
	update a set SONo = c."DocNum" from #tbl a left join RDR1 b on a.BaseEntry = b."DocEntry" and a.BaseLine = b."LineNum" and a.BaseType = 17 left join ORDR c on b."DocEntry" = c."DocEntry";
	--update a set ShipTo = c."Address2" from #tbl a left join RDR1 b on a.BaseEntry = b."DocEntry" and a.BaseLine = b."LineNum" and a.BaseType = 17 left join ORDR c on b."DocEntry" = c."DocEntry";
	update a set Carton = (case when b."SWeight1" = 0 then 0 when b."SWeight1" != 0 then (nullif(a.Qty,0)/b."SWeight1") end) from #tbl a left join OITM b on a.ItemCode = b."ItemCode";
	update #tbl set CompName = (select "CompnyName" from "OADM");
	update #tbl set	CompAdd = (select SUBSTR_BEFORE("CmpnyAddrF", char(13)) from "OADM");
	update a set TotalNetWeight = b."SWeight1" * a.Carton from #tbl a left join OITM b on a.ItemCode = b."ItemCode";
	update a set TotalGrossWeight = b."SalFactor1" * a.Carton from #tbl a left join OITM b on a.ItemCode = b."ItemCode";
	update a set TotalCBM = ("SLength1" * "SHeight1" * "SWidth1"/1000000000)* a.Carton from #tbl a left join OITM b on a.ItemCode = b."ItemCode";
	select * from #tbl;
	drop table #tbl;
end
