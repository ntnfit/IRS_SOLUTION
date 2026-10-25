CREATE procedure ILD_R_PXKKVCNB (
	in fromdate date ,
	in todate date ,
	in docnum nvarchar(15)
)
as
begin
	declare PKKKVCNB table 
	(
		"DocType"				nvarchar(50),
		"DocNo" 				nvarchar(15),
		"DocID" 				int,
		"PostingDate"			timestamp,	
		"DocumentDate" 			timestamp,
		"PBCode"				nvarchar(15),
		"PBName"				nvarchar(100),
		"Contactperson" 		nvarchar(50),
		"ContainerNo" 			nvarchar(20),
		"ShipTo"				nvarchar(254),
		"SalesOrder"			nvarchar(15),
		"ProductionOrder"		nvarchar(15),
		"FromWH"				nvarchar(8),
		"ToWH"					nvarchar(8),
		"FromGL"				nvarchar(15),
		"ToGL"					nvarchar(15),
		"ItemCode"				nvarchar(50),
		"ItemName"				nvarchar(100),
		"SSCC"					varchar(36),
		"SSCCStatus"			nvarchar(20),
		"Quantity"				decimal(21,6),
		"Batch"					nvarchar(36),
		"ManufacturingDate" 	timestamp,
		"ExpirationDate"		timestamp,
		"AdmissionDate"			timestamp,
		"Serial"				nvarchar(40),
		"TaxInvoice"			nvarchar(20),
		"TaxInvoiceDate"		timestamp,
		"UnitPrice"				decimal(21,6),
		"SaleAmount"			decimal(21,6),		
		"UnitCost"				decimal(21,6),
		"Cost"					decimal(21,6),
		"Father1"				nvarchar(50),
		"AdmissionDateFather1"	timestamp,
		"TransferType"			nvarchar(50),
		"Term"					nvarchar(50)		
	);
	declare ListEntry	nvarchar(5000) ;
	
	select STRING_AGG(c."DocEntry", ',') as "DocEntry" into ListEntry default ''
	from (
		select distinct STRING_AGG(dl."DocEntry", ',') as "DocEntry"
		from (
			select a."DocEntry",b."DocNum" ,a."ItemCode",a."WhsCode",a."FromWhsCod",a."Quantity"
			from WTR1 a 
			left join OWTR b on b."DocEntry" = a."DocEntry"
			where b."CANCELED" = 'Y'
			and b."DocDate" between :fromdate and :todate
			union all
			select a."DocEntry" ,b."DocNum",a."ItemCode",a."WhsCode",a."FromWhsCod",a."Quantity"
			from WTR1 a 
			left join OWTR b on b."DocEntry" = a."DocEntry"
			where b."CANCELED" = 'N'
			and b."DocDate" between :fromdate and :todate
			and a."BaseType" = 0 and a."Quantity" < 0
			) dl
		group by dl."ItemCode",dl."WhsCode",dl."FromWhsCod",abs(dl."Quantity")
		having(sum(dl."Quantity") = 0)
	) c ;

--Insert du lieu cho phieu Inventory Transfer. Itemcode trong bang OITW theo fromwarehouse va bin
	INSERT INTO :PKKKVCNB
	select  '67' as "DocType",
		TRDN."DocNum" as "DocNo", JE."TransId" as "DocID", TRDN."DocDate" as "PostingDate"
		,TRDN."TaxDate" "DocumentDate",TRDN."CardCode" as "PBCode", TRDN."CardName" as "PBName"
		,bp."Name" as "Contactperson", TRDN."U_ContNo" as "ContainerNo", TRDN."Address" as "Ship-to"
		,'' as "Sales Order", '' as "ProductionOrder", TRDN."Filler" as "Fromwarehouse"
		,TRDN."ToWhsCode" as "Towarehouse",JE."Account" as "FromGL", JE."ContraAct" as "T GL"
		,OBTN."ItemCode",OITL."ItemName",OBTN."DistNumber" AS "SSCC"
		,CASE OBTN."Status"
			WHEN 0 THEN 'Released'
			WHEN 1 THEN 'Not Accessible'
			WHEN 2 THEN 'Locked'
			ELSE 'Unknown'
		 END "SSCCStatus"
		--,OBTN."U_SSCC_Status"
		,OITL."Quantity",OBTN."MnfSerial" AS "Batch"
		,OBTN."MnfDate" as "ManufacturingDate",OBTN."ExpDate" as "ExpirationDate"
		,OBTN."InDate" as "AdmissionDate"
		,TRDN."U_invSym" || TRDN."U_InvSerial" as "Serial",TRDN."U_InvNo" as "TaxInvoice"
		,TRDN."U_InvDate" as "Tax InvoiceDate"
		,null as "UnitPrice"
		,null as "SaleAmount"
		,OITW."AvgPrice" as "UnitCost"
		,ROUND(OITL."Quantity"*OITW."AvgPrice",2)  as "Cost"
		,OBTN."U_Father1" as "Father1" , OBTN."U_InDate_Father1" as "AdmissionDate_Father1"
		,'Outside Processing' as  "TransferType",TRDN."U_Term"

	from OBTN	
		LEFT JOIN(SELECT min(T1."LogEntry") AS "LogEntry"
				,T2."SysNumber",T1."ItemCode",T1."ItemName",T1."DocLine",T1."DocEntry",T1."DocNum",max(abs(T2."Quantity")) AS "Quantity"
				FROM OITL T1 
				INNER JOIN ITL1 T2 ON T1."LogEntry" = T2."LogEntry" 
				AND T1."ItemCode" = T2."ItemCode" 
				where T1."DocType" = 67
				GROUP BY T2."SysNumber",T1."ItemCode",T1."ItemName",T1."DocLine",T1."DocEntry",T1."DocNum"
			) OITL ON OITL."ItemCode" = OBTN."ItemCode" AND OITL."SysNumber" = OBTN."SysNumber"
					
		left join OWTR TRDN on TRDN."DocEntry" = OITL."DocEntry"
		left join WTR1 RDN1 on RDN1."DocEntry" = TRDN."DocEntry" and RDN1."ItemCode" = OBTN."ItemCode"  and RDN1."LineNum" = OITL."DocLine"
		left join OCPR bp on bp."CntctCode" = TRDN."CntctCode"
		left join OITW on OITW."ItemCode" = OBTN."ItemCode" and OITW."WhsCode" = RDN1."FromWhsCod"
		left join 
			(select 
				"TransId","BaseRef", "Account" , "ContraAct","DocLine"
			 from JDT1 
			 WHERE "Debit" > 0 AND "TransType" = 67 
			) JE on JE."BaseRef" = TRDN."DocNum" and JE."DocLine" = RDN1."LineNum"
		left join (select "part" as "DocEntry" from SPLITSTRING(:ListEntry,',')) c on c."DocEntry" = TRDN."DocEntry"
	where 
		TRDN."DocDate" between :fromdate and :todate  
	 	and TRDN."U_TransferType" = 2 and c."DocEntry" is null
	 	AND (IFNULL(:docnum, '') = '' OR cast(TRDN."DocNum" as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:docnum, ',')))
		;

-- INsert du lieu cho phieu Delivery.	
	INSERT INTO :PKKKVCNB
	select  '15' as "DocType",
		TRDN."DocNum" as "DocNo", OITL."DocEntry" as "DocID", TRDN."DocDate" as "PostingDate"
		,TRDN."TaxDate" "DocumentDate",TRDN."CardCode" as "PBCode", TRDN."CardName" as "PBName"
		,bp."Name" as "Contact person", TRDN."U_ContNo" as "Container No", TRDN."Address" as "Ship-to"
		,cast(ORDR."DocNum" as nvarchar) as "Sales Order",left(OBTN."DistNumber",9) as "ProductionOrder"
		,RDN1."WhsCode" as "Fromwarehouse"
		,'' as "Towarehouse"		
		,JE."Account" as "FromGL", JE."ContraAct" as "ToGL"
		,OBTN."ItemCode",OITL."ItemName",OBTN."DistNumber" AS "SSCC"
		,CASE OBTN."Status"
			WHEN 0 THEN 'Released'
			WHEN 1 THEN 'Not Accessible'
			WHEN 2 THEN 'Locked'
			ELSE 'Unknown'
		 END "SSCCStatus"
		--,OBTN."U_SSCC_Status"
		,OITL."Quantity",OBTN."MnfSerial" AS "Batch"
		,OBTN."MnfDate" as "ManufacturingDate",OBTN."ExpDate" as "ExpirationDate",OBTN."InDate" as "AdmissionDate"
		,TRDN."U_invSym" || TRDN."U_InvSerial" as "Serial",TRDN."U_InvNo" as "TaxInvoice"
		,TRDN."U_InvDate" as "TaxInvoice Date"
		,RDN1."Price" as "UnitPrice"
		,ROUND(OITL."Quantity"*RDN1."Price",2) as "SaleAmount"
		,null as "UnitCost", null as "Cost"
		,OBTN."U_Father1" as "Father1" , OBTN."U_InDate_Father1" as "AdmissionDate_Father1"
		,'Delivery' as  "TransferType", TRDN."U_Term"

	from OBTN	
		LEFT JOIN(SELECT min(T1."LogEntry") AS "LogEntry"
				,T2."SysNumber",T1."ItemCode",T1."ItemName",T1."DocLine",T1."DocEntry",T1."DocNum",max(abs(T2."Quantity")) AS "Quantity"
				FROM OITL T1 
				INNER JOIN ITL1 T2 ON T1."LogEntry" = T2."LogEntry" 
				AND T1."ItemCode" = T2."ItemCode" 
				where T1."DocType" = 15
				--AND T1."DocNum"=411250203
				GROUP BY T2."SysNumber",T1."ItemCode",T1."ItemName",T1."DocLine",T1."DocEntry",T1."DocNum"
			) OITL ON OITL."ItemCode" = OBTN."ItemCode" AND OITL."SysNumber" = OBTN."SysNumber"
				 
		left join ODLN TRDN on TRDN."DocEntry" = OITL."DocEntry"
		left join DLN1 RDN1 on RDN1."DocEntry" = TRDN."DocEntry" and RDN1."ItemCode" = OBTN."ItemCode"  and RDN1."LineNum" = OITL."DocLine"
		left join OCPR bp on bp."CntctCode" = TRDN."CntctCode"
		left join ORDR on ORDR."DocEntry" in (SELECT "DocEntry" FROM RDR1 WHERE "TrgetEntry" = OITL."DocEntry")
		left join OITW on OITW."ItemCode" = OBTN."ItemCode" and OITW."WhsCode" = RDN1."WhsCode"
		left join 
			(select 
				"TransId","BaseRef", "Account" , "ContraAct","DocLine"
			 from JDT1 
			 WHERE "Debit" > 0  and "TransType" = 15
			) JE on JE."BaseRef" = TRDN."DocNum" 
			and JE."DocLine" = RDN1."LineNum"
		
		where 
		TRDN."DocDate" between :fromdate and :todate  and
		 TRDN."U_Term" = 'FOB' and TRDN.CANCELED = 'N'
		 AND (IFNULL(:docnum, '') = '' OR cast(TRDN."DocNum" as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:docnum, ','))) ;
	
		-- truy xuat sscc father1
		TDistNumber = select "SSCC" AS "DistNumber" FROM :PKKKVCNB group by "SSCC";
		call ILD_TRUYXUATSSCCFATTHER1(
			:TDistNumber
			,'N'
			,result_table			
			);

--- Cap nhat gia tri COST va UnitCost
	DO_JE = select  "ItemCode","BASE_REF" as "DO_No","CreatedBy",-sum("TransValue") as "TransValue"  from OINM
			where    substring("CostAct",0,3) in ('632','157') and "TransType" = 15
					and "DocDate" between fromdate and todate
				and "CreatedBy"  in (select distinct "DocID" From :PKKKVCNB where "DocType" = 15)
				group by   "ItemCode","BASE_REF","CreatedBy";
				
-- COST = Tong gia tri DO / (Tong so luong DO * So luong tung SSCC)				
	update a
	set a."Cost"  =  b."TransValue" /c."TotalQty" * a."Quantity"
	from :PKKKVCNB a
	inner join :DO_JE b on a."DocID" =  b."CreatedBy" and a."ItemCode" = b."ItemCode"
	inner join (select "DocID","ItemCode", sum("Quantity") as "TotalQty" 
				from :PKKKVCNB where "DocType" = 15 group by "DocID", "ItemCode"
				) c on c."DocID" = a."DocID" and c."ItemCode" = a."ItemCode"
	where a."DocType" = 15 and (c."TotalQty" * a."Quantity") > 0;

	update :PKKKVCNB set "UnitCost" = "Cost"/"Quantity" where "DocType" = 15 and "Quantity" > 0; 	
	
	select a."DocType",
		a."DocNo"
		,a."DocID"
		,a."PostingDate"
		,a."DocumentDate"
		,a."PBCode"
		,a."PBName"
		,a."Contactperson"
		,a."ContainerNo"
		,a."ShipTo"
		,a."SalesOrder"
		,a."ProductionOrder"
		,a."FromWH"
		,a."ToWH"
		,a."FromGL"
		,a."ToGL"
		,a."ItemCode"
		,a."ItemName"
		,a."SSCC"
		,a."SSCCStatus"
		,a."Quantity"
		,a."Batch"
		,a."ManufacturingDate"
		,a."ExpirationDate"
		,a."AdmissionDate"
		,a."Serial"
		,a."TaxInvoice" as "DO_TaxInv"
		,a."TaxInvoiceDate" as "DO_TaxInvDate"
		,inv."U_InvNo" as "AR_TaxInv"
		,inv."U_InvDate" as "AR_TaxInvDate"
		,a."UnitPrice"
		,a."SaleAmount"			
		,a."UnitCost"
		,a."Cost"
		,b.SSCC2 AS "Father1"
		,b.INDATE2 AS "AdmissionDateFather1"
		,a."TransferType",a."Term"
	from :PKKKVCNB a
	left join $result_table b on a.SSCC = B.SSCC1 
	left join (
			select
				max(a."DocNum") "DocNum", max(a."U_InvNo") "U_InvNo" ,max(a."U_InvDate") "U_InvDate", max(b."BaseRef") "BaseRef"
				,b."DocEntry",max(b."BaseEntry") "BaseEntry"
			from OINV a
			inner join INV1 b ON b."DocEntry" = a."DocEntry"
			where a.CANCELED = 'N' and b."BaseType" = 15 group by b."DocEntry"
			) inv on inv."BaseEntry" = a."DocID" ;
			
END
