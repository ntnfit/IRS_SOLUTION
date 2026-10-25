CREATE procedure VAS_F_PD_01 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	
	select ifnull(x."BPLId",0) into bplID from "OPDN" x where x."DocEntry"=DocId;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		FormNo			nvarchar(50)
		, RptName		nvarchar(200)
		, CompName		nvarchar(200)
		, CompAdd		nvarchar(300)
		, Customer		NVARCHAR(300)
		, CardCode		nvarchar(50)
		, CardName		nvarchar(200)
		, CardAddr		nvarchar(200)
		, Comments		nvarchar(300)
		, U_InvNo		nvarchar(100)
		, U_JVNumber	nvarchar(100)
		, DocDate		nvarchar(100)
		, InvDate		NVARCHAR(100)
		, ItemCode		nvarchar(50)
		, ItemName		nvarchar(200)
		, WhsCode		nvarchar(100)
		, WhsName		nvarchar(300)
		, Unit			nvarchar(100)
		, Batch			nvarchar(300)
		, OrderQty		numeric(21,6)
		, RecptQty		numeric(21,6)
        , QtyBatch		numeric(21,6)
		, UnitPrice		numeric(21,6)
		, LineTotal		numeric(21,6)
		, DocTotal		numeric(21,6)
		, DocTotalFC	numeric(21,6)
		, DocRate		numeric(21,6)
		, DocCurr		nvarchar(10)
		, RefNo			nvarchar(50)
		, AmountInWord	nvarchar(400)
		, DocCurrName	nvarchar(50)
		, DecQty		int
		, DecPrice		int
		, DecAmt		int
		, DateSep		nvarchar(1)
		, ThousSep		nvarchar(1)
		, DateTitle		nvarchar(100)
		, ViTri			nvarchar(100)
		, TkNo			nvarchar(50)
		, TkCo			nvarchar(50)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	create local temporary table #Batch (
		DocEntry	int,
		ItemCode	nvarchar(50),
		DocType		nvarchar(20),
		DistNumber	nvarchar(50),
		Quantity	numeric(21,6),
		DocLine		int,
		AbsEntry	int
	);

	insert into #Batch (DocEntry,ItemCode,DocType,DistNumber,Quantity,DocLine,AbsEntry)
	select a."DocEntry",a."ItemCode",a."DocType",c."DistNumber",b."Quantity",a."DocLine",c."AbsEntry"
	from
		"OITL" a
		left join "ITL1" b on a."LogEntry"=b."LogEntry"
		left join "OBTN" c on c."SysNumber"=b."SysNumber" and b."ItemCode"=c."ItemCode"
	where
		((a."DocType" = '20'
						AND a."DocEntry" = DocID)
						or (a."DocType" = '21'
							and a."BaseType" = '20'
							and a."BaseEntry" = DocID));


	
	insert into #tbl (DocDate,CardCode,CardName,CardAddr,Comments,U_InvNo,InvDate,U_JVNumber,ItemCode,ItemName,WhsCode,Unit,Batch,OrderQty,RecptQty,QtyBatch,UnitPrice,LineTotal,DocTotal,DocTotalFC,DocRate,DocCurr,FormNo,ViTri,TkNo,TkCo)
	select 
			CONCAT('Ngày/ Date: ',to_char(a."DocDate",'DD/MM/YYYY'))
			,a."CardCode"
			,a."CardName"
			,replace(a."Address",'VIETNAM','')
			,a."Comments"
			,a."U_InvNo"
			,CONCAT('Ngày/ Date: ',to_char(a."U_InvDate",'DD/MM/YYYY'))
			,a."U_JVNumber"
			,b."ItemCode"
			,b."Dscription"
			,b."WhsCode"
			,b."unitMsr"
			,d.DistNumber
			,c."Quantity"
			,case when g."Quantity" is null then b."Quantity" else g."Quantity" end 
			,d.Quantity
			,b."Price"
			,case when a."DocCur"=e."MainCurncy" then b."LineTotal" else b."TotalFrgn" end
			,case when a."DocCur"=e."MainCurncy" then a."DocTotal"-a."VatSum"-IFNULL(a."TotalExpns",0) else a."DocTotalFC"-a."VatSumFC"-IFNULL(a."TotalExpFC",0) end
			,a."DocTotalFC"
			,a."DocRate",a."DocCur"
			,'01 - VT'
			,h."BinCode"
			,b."AcctCode"
			,i."Account"
	from
		"OPDN" a
		left join "PDN1" b on a."DocEntry"=b."DocEntry"
		left join "POR1" c on c."DocEntry"=b."BaseEntry" and c."LineNum"=b."BaseLine" and c."ItemCode"=b."ItemCode"
		left join #Batch d on a."DocEntry"=d.DocEntry and b."ItemCode"=d.ItemCode and b."LineNum"=d.DocLine
		left join "OADM" e on 1=1
		left join "OITL" f on f."DocType" = b."ObjType" and f."DocEntry" = b."DocEntry" and f."DocLine" = b."LineNum"
		left join "OBTL" g on g."ITLEntry" = f."LogEntry" and g."SnBMDAbs"=d.AbsEntry
		left join "OBIN" h on h."AbsEntry" = g."BinAbs"
		left join "OBTN" j on g."SnBMDAbs"	= j."AbsEntry" and f."ItemCode" = j."ItemCode"
		left join "JDT1" i on i."TransType" = a."ObjType" and i."CreatedBy" = a."DocEntry" and i."DocLine" = b."LineNum" and i."Credit" <> 0
	where a."DocEntry"=DocID and a."CANCELED"='N';


	update #tbl set WhsName = (select a."WhsName" from "OWHS" a where a."WhsCode"=#tbl.WhsCode);
	
	UPDATE #tbl SET Customer = (SELECT a."CntctPrsn" FROM "OCRD" a, "OCPR" b WHERE a."CardCode"=#tbl.CardCode AND a."CntctPrsn"=b."Name" AND a."CardCode"=b."CardCode");

	update #tbl set RptName = N'PHIẾU NHẬP KHO/ GOODS RECEIPT PO';
	
	update #tbl set DateTitle = N'Ngày.......tháng.......năm......./ Date.....................';
	update #tbl set AmountInWord = Num2Text(to_integer(DocTotal));

	select a."QtyDec",a."DecSep",a."ThousSep",a."PriceDec" into DecQty,DateSep,ThousSep,DecPrice from "OADM" a;
	
	update #tbl set DecQty = DecQty, DateSep = DateSep,ThousSep = ThousSep, DecPrice=DecPrice;

	update #tbl set DecAmt = case when (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.DocCurr)='-1' then (select a."SumDec" from "OADM" a) else (select a."Decimals" from "OCRN" a where a."CurrCode" = #tbl.DocCurr) end;
	
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
	
	update #tbl set Batch = '' where Batch is null;


	select * from #tbl;
	drop table #Batch;
	drop table #tbl;
end

