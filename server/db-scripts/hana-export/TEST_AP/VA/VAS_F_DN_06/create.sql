CREATE procedure VAS_F_DN_06 (
	in DocID	int,
	in SlipId	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	
	select ifnull(x."BPLId",0) into bplID from "ODLN" x where x."DocEntry"=DocID;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		CompName	nvarchar(100)
		,CompAdd	nvarchar(254)
		,CompTel	nvarchar(20)
		,CompFax	nvarchar(20)
		,CompTaxId	nvarchar(20)
		,SlipId		int
		,SlipName	nvarchar(100)
		,CardCode	nvarchar(50)
		,CardName	nvarchar(100)
		,CardAtn	nvarchar(100)
		,CardRefNo	nvarchar(30)
		,Model		nvarchar(50)
		,SeriNo		nvarchar(50)
		,DocDate	timestamp
		,ItemCode	nvarchar(50)
		,ItemName	nvarchar(100)
		,Unit		nvarchar(30)
		,OrderNo	nvarchar(30)
		,DelQty		numeric(21,6)
		,RecQty		numeric(21,6)
		,ProdDate	timestamp
		,Remarks	nvarchar(254)
		,isDeci		nvarchar(1)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CompName,CompAdd,CompTel,CompFax,CompTaxId,SlipId,SlipName,CardCode,CardName,CardAtn,CardRefNo,Model,SeriNo,DocDate,ItemCode,ItemName,Unit,OrderNo,DelQty,ProdDate)
	SELECT 
		UPPER(a."CompnyName"), 
		REPLACE(a."CompnyAddr", 'VIETNAM', ''), 
		IFNULL(a."Phone1", a."Phone2"), 
		a."Fax", 
		IFNULL(a."TaxIdNum", IFNULL(a."TaxIdNum2", a."TaxIdNum3")), SlipId, 
		CASE SlipId 
			WHEN 1 THEN 'Slip 1: Bộ phận giao hàng lưu / Issued section keep' 
			WHEN 2 THEN 'Slip 2: Khách hàng lưu / Customer keep' 
			WHEN 3 THEN 'Slip 3: Bảo vệ lưu / Security keep' 
		END, d."CardCode", d."CardName", e."Name", b."NumAtCard", '', b."U_JVNumber", b."DocDueDate", c."ItemCode", c."Dscription", '' AS "U_ItmFrgName", c."FreeTxt", c."Quantity", '' AS "U_ProdDate" 
	FROM "OADM" a 
	LEFT OUTER JOIN "ODLN" b ON b."DocEntry" = DocID 
	LEFT OUTER JOIN "DLN1" c ON b."DocEntry" = c."DocEntry" 
	LEFT OUTER JOIN "OCRD" d ON b."CardCode" = d."CardCode" 
	LEFT OUTER JOIN "OCPR" e ON b."CntctCode" = e."CntctCode";

	
	select 
		CompName  "CompName",
		CompAdd  "CompAdd",
		CompTel  "CompTel",
		CompFax  "CompFax",
		CompTaxId  "CompTaxId",
		SlipId  "SlipId",
		SlipName  "SlipName",
		CardCode  "CardCode",
		CardName  "CardName",
		CardAtn  "CardAtn",
		CardRefNo  "CardRefNo",
		Model  "Model",
		SeriNo  "SeriNo",
		DocDate  "DocDate",
		ItemCode  "ItemCode",
		ItemName  "ItemName",
		Unit  "Unit",
		OrderNo  "OrderNo",
		DelQty  "DelQty",
		RecQty  "RecQty",
		ProdDate  "ProdDate",
		Remarks  "Remarks",
		isDeci  "isDeci"
	from #tbl;

	drop table #tbl;
end

