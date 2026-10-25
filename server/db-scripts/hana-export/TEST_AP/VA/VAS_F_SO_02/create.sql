CREATE procedure VAS_F_SO_02 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	
	select ifnull(x."BPLId",0) into bplID from "ORDR" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		CompName	nvarchar(100)
		,CompAdd	nvarchar(254)
		,CompTel	nvarchar(20)
		,CompFax	nvarchar(20)
		,CompTaxId	nvarchar(20)
		,CardCode	nvarchar(50)
		,CardName	nvarchar(100)
		,CardAdd	nvarchar(254)
		,SeriNo		nvarchar(50)
		,DocDate	timestamp
		,DocCur		nvarchar(30)
		,ItemCode	nvarchar(50)
		,ItemName	nvarchar(100)
		,Unit		nvarchar(30)
		,Quantity	numeric(21,6)
		,Price		numeric(21,6)
		,LineTotal	numeric(21,6)
		,Remarks	nvarchar(254)
		,IsDeci		nvarchar(1)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CompName,CompAdd,CompTel,CompFax,CompTaxId,CardCode,CardName,CardAdd,SeriNo,DocDate,DocCur,ItemCode,ItemName,Unit,Quantity,Price,LineTotal,Remarks)
	SELECT UPPER(a."CompnyName"), REPLACE(a."CompnyAddr", 'VIETNAM', ''), IFNULL(a."Phone1", a."Phone2"), a."Fax", IFNULL(a."TaxIdNum", IFNULL(a."TaxIdNum2", a."TaxIdNum3")), d."CardCode", d."CardName", d."Address", b."U_JVNumber", b."DocDate", b."DocCur", c."ItemCode", c."Dscription", '', c."Quantity", c."Price", CASE IFNULL(b."DocCur", a."MainCurncy") WHEN a."MainCurncy" THEN c."LineTotal" ELSE c."TotalFrgn" END, '' 
	FROM OADM a 
	LEFT OUTER JOIN ORDR b ON b."DocEntry" = DocID 
	LEFT OUTER JOIN RDR1 c ON b."DocEntry" = c."DocEntry" 
	LEFT OUTER JOIN OCRD d ON b."CardCode" = d."CardCode";


	update #tbl set IsDeci = (select case MAX(Quantity-ROUND(Quantity,0)) when 0 then 'N' else 'Y' end from #tbl);

	select 
		CompName 	"CompName",
		CompAdd 	"CompAdd",
		CompTel 	"CompTel",
		CompFax 	"CompFax",
		CompTaxId 	"CompTaxId",
		CardCode 	"CardCode",
		CardName 	"CardName",
		CardAdd 	"CardAdd",
		SeriNo 		"SeriNo",
		DocDate 	"DocDate",
		DocCur 		"DocCur",
		ItemCode 	"ItemCode",
		ItemName 	"ItemName",
		Unit 		"Unit",
		Quantity 	"Quantity",
		Price 		"Price",
		LineTotal 	"LineTotal",
		Remarks 	"Remarks",
		IsDeci 		"IsDeci"
	from #tbl;

	drop table #tbl;
end

