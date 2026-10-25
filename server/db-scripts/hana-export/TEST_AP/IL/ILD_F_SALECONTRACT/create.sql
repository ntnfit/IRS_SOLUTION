CREATE procedure ILD_F_SaleContract (
	in DocID	int
)
as
begin
	create local temporary table #tbl (
		CompName 		nvarchar(300), 
		CompAddr 		nvarchar(300),
		CompTel 		nvarchar(30), 
		CompTaxNum		nvarchar(30),
		
		RptNameVN 		nvarchar(300) default N'SALES CONTRACT', 
		
		DocNum			int,
		DocDate			date,	
		ItemCode		nvarchar(50),
		ItemName		nvarchar(150),
		Price			numeric(19,6),
		Qty				numeric(19,6),
		Amount			numeric(19,6),
		DocCur			nvarchar(10),
		
		BPCode			nvarchar(15),
		BPName			nvarchar(100),
		BPAddress		nvarchar(254),
		BPTaxId			nvarchar(50),

		PackingType		nvarchar(100),
		DispatchTime	nvarchar(100),
		ContractF		date,
		ContractT		date,
		
		TermOfDeli		nvarchar(100),
		OriginGoods		nvarchar(100),
		
		Dec_Amount		int default 2,		
		Dec_Qty			int default 1,
		Dec_Price		int default 5

	);

	insert into #tbl (DocNum, DocDate, BPCode, BPName, BPAddress, BPTaxId, ItemCode, ItemName, Price, Qty, TermOfDeli, OriginGoods, PackingType, DispatchTime,
		ContractF, ContractT, Amount, DocCur)
	select a."DocNum", a."DocDate", a."CardCode", a."CardName", c."Address", c."LicTradNum", b."ItemCode", b."Dscription", 
		b."Price", b."Quantity",
		a."U_Incoterm", e."Name", b."U_PackType", to_nvarchar(b."U_DisTime", 'Mon.yyyy'), a."U_ConValFrom", a."U_ConValTo",
		b."LineTotal", b."Currency" 
	from ordr a
	left join rdr1 b on a."DocEntry" = b."DocEntry"
	left join ocrd c on a."CardCode" = c."CardCode"
	left join ocry e on b."CountryOrg" = e."Code"
	where a."DocEntry" = :DocID ;
	
	
	update x set CompName = y."CompnyName", CompAddr = SUBSTR_BEFORE(y."CmpnyAddrF",char(13)), CompTel = y."Phone1", CompTaxNum = y."TaxIdNum" ,
					Dec_Amount = y."SumDec",
					Dec_Qty = y."QtyDec",
					Dec_Price = y."PriceDec"
	from #tbl x  left join  OADM y on 1=1 left join oadp z on 1=1 ;
	
	select * from #tbl;

	drop table #tbl;
	
end

