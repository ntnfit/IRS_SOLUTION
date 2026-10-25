CREATE procedure ILD_F_Delivery (
	in ObjType	int,
	in DocID	int
)
as
begin
	create local temporary table #tbl (
		BPLId			int,
		UserSign		int,
		CompName 		nvarchar(300), 
		CompAddr 		nvarchar(300),
		CompTel 		nvarchar(30), 
		CompFax 		nvarchar(30), 
		CompTaxNum		nvarchar(30),
		
		RptNameVN 		nvarchar(300) default N'Delivery Note', 
		
		OrderDate		timestamp,
		OrderDocNum		int,
		DeliveryDocNum	int,
		CustomerID		nvarchar(15),
		DespatchDate	timestamp,
		DeliveryNethod	nvarchar(100),
		Cont_TruckNo	nvarchar(100),
		ShipName		nvarchar(150),
		ShipAddress		nvarchar(300),
		InvoiceName		nvarchar(150),
		InvoiceAddress	nvarchar(300),
		
		ItemCode		nvarchar(50),
		ItemName		nvarchar(150),
		OrderQty		numeric(19,6),
		DeliveryQty		numeric(19,6),
		OutStanding		numeric(19,6),
		
		Dec_OrderQty	int default 2,
		Dec_DeliQty		int default 2,
		Dec_OutStand	int default 2,
		Logo			blob,
		CompanySlog		nvarchar(150) default 'Where Passion Meets Coffee',
		CompanyNameFooter1	nvarchar(150) default 'ILD COFFEE VIETNAM COMPANY LIMITED',
		CompanyNameFooter2	nvarchar(250)
		--default 'Lot No. 19-4 Road No. 11,  Protrade International IZ, An Tay Commune, Ben Cat City, Binh Duong Province, Vietnam'
	);

if :ObjType = '15' then
	insert into #tbl (OrderDate, OrderDocNum, DeliveryDocNum, CustomerID, DespatchDate, ItemCode, ItemName, OrderQty, DeliveryQty, 
						Cont_TruckNo,
						ShipName,
						ShipAddress,
						DeliveryNethod
						)
	select d."DocDate", d."DocNum",
	 					a."DocNum",
	  a."CardCode", a."DocDate", b."ItemCode", e."ItemName", c."Quantity", b."Quantity",
	 					a."U_ContNo",
	 					case when  SUBSTR_AFTER (upper(a."PickRmrk"),'-') = '' then  upper(a."PickRmrk") 
	 								else LTRIM(SUBSTR_AFTER (upper(a."PickRmrk"),'-')) end as "ShipName",
	 					a."Address2"as  "ShipAddress",
	 					f."TrnspName" as "DeliveryMethod"
	from odln a
	left join dln1 b on a."DocEntry" = b."DocEntry"
	left join rdr1 c on c."ObjType" = b."BaseType" and c."DocEntry" = b."BaseEntry" and c."LineNum" = b."BaseLine"
	left join ordr d on c."DocEntry" = d."DocEntry"
	left join oitm e on b."ItemCode" = e."ItemCode"
	left join OSHP f on f."TrnspCode" = a."TrnspCode"
	where a."DocEntry"=:DocID;
end if;

if :ObjType = '112' then	
	insert into #tbl (OrderDate, OrderDocNum, DeliveryDocNum, CustomerID, DespatchDate, ItemCode, ItemName, OrderQty, DeliveryQty, 
						Cont_TruckNo,
						ShipName,
						ShipAddress,
						DeliveryNethod
						)
	select d."DocDate", d."DocNum",
	 					a."DocNum",
	  a."CardCode", a."DocDate", b."ItemCode", e."ItemName", c."Quantity", b."Quantity",
	 					a."U_ContNo",
	 					case when  SUBSTR_AFTER (upper(a."PickRmrk"),'-') = '' then  upper(a."PickRmrk") 
	 								else LTRIM(SUBSTR_AFTER (upper(a."PickRmrk"),'-')) end as "ShipName",
	 					a."Address2"as  "ShipAddress",
	 					f."TrnspName" as "DeliveryMethod"
	from ODRF a
	left join DRF1 b on a."DocEntry" = b."DocEntry"
	left join rdr1 c on c."ObjType" = b."BaseType" and c."DocEntry" = b."BaseEntry" and c."LineNum" = b."BaseLine"
	left join ordr d on c."DocEntry" = d."DocEntry"
	left join oitm e on b."ItemCode" = e."ItemCode"
	left join OSHP f on f."TrnspCode" = a."TrnspCode"
	where a."DocEntry"=:DocID;			
end if;
						
	update x set OutStanding = OrderQty - DeliveryQty from #tbl x ;
	-- update ShipAddress 
	update a set InvoiceName = upper(d."Address"),
				 InvoiceAddress = d."Street"
	from #tbl a 
	left join  OCRD c on c."CardCode" = a.CustomerID
	left join CRD1 d on c."CardCode"  =  d."CardCode" and d."AdresType" ='B';
	--
	update x set CompName = y."CompnyName", CompAddr = SUBSTR_BEFORE(y."CompnyAddr",char(13)) , Logo = z."LogoImage",
			CompanyNameFooter2 = SUBSTR_BEFORE(  "CmpnyAddrF",char(13))
	from #tbl x  left join  OADM y on 1=1 left join oadp z on 1=1 ;
	
	select * from #tbl;

	drop table #tbl;
	
end
