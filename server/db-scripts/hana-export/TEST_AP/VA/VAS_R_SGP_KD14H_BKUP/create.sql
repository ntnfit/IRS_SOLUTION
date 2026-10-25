CREATE PROCEDURE VAS_R_SGP_KD14H_BKUP (
	in fdate		timestamp,
	in tdate		timestamp,
	in lnghiepvu	nvarchar(5000)

)
AS
BEGIN
	create local temporary table #tbl (
		ID 					int 
		--
		,DocType			nvarchar(20)
		,DocID				int
		,DocNo				nvarchar(20)
		,DocDate			date
		,ItemCode			nvarchar(50)
		,ItemName			nvarchar(150)
		,UoM				nvarchar(100)
		,Qty				numeric(21,6)
		,VATPercent			numeric(21,6)
		,Price				numeric(21,6)
		,Amount				numeric(21,6)
		,AmountAftVat		numeric(21,6)
		,ItmsGrpNam			nvarchar(100)
		,SSCC				nvarchar(100)
		,Batch				nvarchar(100)
		,MnfDate			date
		,ExpDate			date
		,SSCC_Status		nvarchar(20)
		
		,SOEntry			nvarchar(20)
		,SONo				nvarchar(20)
		
		,DOEntry			nvarchar(20)
		,DONo				nvarchar(20)
		
		,Whse				nvarchar(20)
		,Bin				nvarchar(100)
		
		,SoHoaDon			nvarchar(100)
		,BPCode				nvarchar(15)
		,BPName				nvarchar(100)
		,Source				nvarchar(5)
		,LamTron		numeric(21,6)	
		,ChenhLech		numeric(21,6)	
		,SumTotal 		numeric(21,6)	
		,AmountNew		numeric(21,6)
		,ToTotalCL		numeric(21,6)
	);
	
	create local temporary table #tbl1 as (select * from #tbl) ;
	--===================================================================================================================================================
---
	insert into #tbl1 (DocType, DocID, DocDate,  ItemCode, ItemName, 
						UoM, Qty, 
						SSCC,Batch,MnfDate,ExpDate,SSCC_Status,
						Price, VATPercent,
						SoHoaDon,
						-- Delivery
						DOEntry,
						DONo,
						-- sale order 
						SOEntry,
						SOnO,
						BPCode, BPName, 
				 		Source, ItmsGrpNam,
				 		Whse,
				 		Bin
				 		)
	select a."ObjType", a."DocEntry", a."DocDate", b."ItemCode", b."Dscription",
	
		 	b."unitMsr", tl."Quantity",
		 	t2."DistNumber",t2."MnfSerial" as "batch",t2."MnfDate", t2."ExpDate", t2."LotNumber" as "SSCC_Status",
		 			  
			case when b."TaxOnly" = 'N' then b."LineTotal"/b."Quantity" else 0 end as "Price",
			b."VatPrcnt",
			
			 a."U_InvNo",
			 -- Delivery
			 d2."DocEntry",
			 d2."DocNum",
			 -- Sales order
			 s2."DocEntry",
			 s2."DocNum",

			 a."CardCode", a."CardName", 
			  'AR' as "Source", g."ItmsGrpNam",
			  
			  b."WhsCode",
			  tl."BinCode" as "Bin"
	from oinv a 
	left join inv1 b on b."DocEntry" = a."DocEntry"
	left join oitm c on c."ItemCode" = b."ItemCode"
	left join OMRC d on d."FirmCode" = c."FirmCode"
	left join oitb g on g."ItmsGrpCod" = c."ItmsGrpCod"
	-- ODLN: Delivery
	left join DLN1 d1 on d1."ObjType" = b."BaseType" and d1."DocEntry" = b."BaseEntry" and d1."LineNum" = b."BaseLine"
	left join ODLN d2 on d2."DocEntry" = d1."DocEntry"
	-- RDR1: Sales Order
	left join RDR1 s1 on s1."ObjType" = d1."BaseType" and s1."DocEntry" = d1."BaseEntry" and s1."LineNum" = d1."BaseLine"
	left join ORDR s2 on s2."DocEntry" = s1."DocEntry"
	--	
	left join (select   c."BinCode",a."SnBMDAbs",a."Quantity", b."BaseEntry", b."BaseLine", b."BaseType",  b."LocCode", b."ItemCode"
				from OBTL a
				inner join OITL b on a."ITLEntry"  = b."LogEntry"and "BaseType"  =  17
				
				inner join OBIN c on c."AbsEntry"  = a."BinAbs"
				
				) tl on   s1."DocEntry" = tl."BaseEntry" 
															and s1."ObjType" = tl."BaseType" AND s1."LineNum" = tl."BaseLine" 
															and s1."ItemCode" = tl."ItemCode" AND s1."WhsCode" = tl."LocCode"
						
		
	left join OBTN t2 ON tl."ItemCode" = t2."ItemCode" AND tL."SnBMDAbs" = t2."AbsEntry"
	where a."DocDate" between :fdate and :tdate and a."CANCELED"='N' 
	
	
 
 
	union all
	
	-- A/R Credit Memo:          Ke thua --> Delivery --> Return --> A/R Credit Memo
	select a."ObjType", a."DocEntry", a."DocDate",  b."ItemCode", b."Dscription",
			 b."unitMsr", 
			
			 tl."Quantity",
			 t2."DistNumber",t2."MnfSerial" as "batch",t2."MnfDate", t2."ExpDate", t2."LotNumber" as "SSCC_Status",
			case when b."TaxOnly" = 'N' then -b."LineTotal"/b."Quantity" else 0 end as "Price", b."VatPrcnt",
			  a."U_InvNo", 
			 -- Delivery
			 d2."DocEntry",
			 d2."DocNum",
			 -- Sales order
			 s2."DocEntry",
			 s2."DocNum",
			  
			   a."CardCode", a."CardName", 
			 'ARCRE' as "Source", g."ItmsGrpNam",
			 b."WhsCode",
			 tl."BinCode" as "Bin"
	from ORIN a 
	left join RIN1 b on b."DocEntry" = a."DocEntry"
	left join oitm c on c."ItemCode" = b."ItemCode"
	left join OMRC d on d."FirmCode" = c."FirmCode"
	left join oitb g on g."ItmsGrpCod" = c."ItmsGrpCod"	
	
	-- Return 
	left join RDN1 rt1 on rt1."ObjType" = b."BaseType" and rt1."DocEntry" = b."BaseEntry" and rt1."LineNum" = b."BaseLine"
	left join ORDN rt2 on rt2."DocEntry" = rt1."DocEntry"
	-- ODLN: Delivery
	left join DLN1 d1 on d1."ObjType" = rt1."BaseType" and d1."DocEntry" = rt1."BaseEntry" and d1."LineNum" = rt1."BaseLine"
	left join ODLN d2 on d2."DocEntry" = d1."DocEntry"
	-- RDR1: Sales Order
	left join RDR1 s1 on s1."ObjType" = d1."BaseType" and s1."DocEntry" = d1."BaseEntry" and s1."LineNum" = d1."BaseLine"
	left join ORDR s2 on s2."DocEntry" = s1."DocEntry"
	
	left join (select   c."BinCode",a."SnBMDAbs",a."Quantity", b."BaseEntry", b."BaseLine", b."BaseType",  b."LocCode", b."ItemCode"
				from OBTL a
				inner join OITL b on a."ITLEntry"  = b."LogEntry"and "BaseType"  =  17
				
				inner join OBIN c on c."AbsEntry"  = a."BinAbs"
				
				) tl on   s1."DocEntry" = tl."BaseEntry" 
															and s1."ObjType" = tl."BaseType" AND s1."LineNum" = tl."BaseLine" 
															and s1."ItemCode" = tl."ItemCode" AND s1."WhsCode" = tl."LocCode"
		
		
		
	left join OBTN t2 ON tl."ItemCode" = t2."ItemCode" AND tl."SnBMDAbs" = t2."AbsEntry"
	
	where a."DocDate" between :fdate and :tdate and a."CANCELED"='N' ;

----

	insert into #tbl (ID ,DocType,DocID, DocNo,DocDate, ItemCode, ItemName,
						UoM,Qty, Price, Amount, AmountAftVat, ItmsGrpNam,
						SSCC,Batch,MnfDate,ExpDate, SSCC_Status,SOEntry,SONo,DOEntry,DONo,
						Whse,Bin,SoHoaDon,BPCode,BPName,Source)
				
	select ROW_NUMBER() OVER(PARTITION BY DocType, DocID),DocType,DocID, DocNo,DocDate, ItemCode, ItemName,
			UoM,Qty, Price, Amount, AmountAftVat, ItmsGrpNam,
			SSCC,Batch,MnfDate,ExpDate, SSCC_Status,SOEntry,SONo,DOEntry,DONo,
			Whse,Bin,SoHoaDon,BPCode,BPName,Source
	from #tbl1 ;
	
	update #tbl set Amount = Qty*Price;
	update #tbl set AmountAftVAT = Qty*Price*(1 + VATPercent/100);
	
	update #tbl set AmountAftVAT = 0 where AmountAftVAT is null ;
	update #tbl set Amount = 0 where Amount is null ;
	
	
	if ifnull(:lnghiepvu, '') <> '' then
		delete from #tbl where Source not in (select Data from split(:lnghiepvu, ',')) ;
	end if ;
	-- 

	
	
	
										

update x set SumTotal = (select sum(-"TransValue") from oinm y 
								where y."CreatedBy" = x.DocId and y."TransType" = x.DocType)
	from #tbl x
	where x.ID = 1 and x.DocType = 60 ; 
	
	update x set SumTotal = -SumTotal from #tbl x where x.DocType in (14) and x.ID = 1 ;
	
	update x set  LamTron = (select sum(Amount) from #tbl y where y.DocType = x.DocType  and y.DocId = x.DocId ) 
	from #tbl x 
	where x.ID = 1;

	update x set ChenhLech = SumTotal  - LamTron from #tbl x 
	where x.ID = 1 and x.SumTotal <> x.LamTron ;
		
	update  x set Amount = Amount + ChenhLech from #tbl x 
	where x.ID = 1 and x.ChenhLech <> 0;
----





	select  DONO,DOCDATE,
			ITEMCODE,ITEMNAME,UOM,QTY,
			BATCH,SSCC,PRICE,AMOUNT,
			MNFDATE,EXPDATE,SSCC_STATUS,BIN,SOHOADON,
			BPCODE,BPNAME,DOCID,SONO,
			WHSE,SOURCE,DOCTYPE,SOENTRY,DOENTRY
 from #tbl 
		where ((ifnull(:lnghiepvu, '') = '') or (  Source in  (select Data from split(:lnghiepvu, ','))  )) 
		--and SOHOADON = '00000006'  
		 order by DocDate, Source ;
	
	drop table #tbl;
	drop table #tbl1 ;
END;

