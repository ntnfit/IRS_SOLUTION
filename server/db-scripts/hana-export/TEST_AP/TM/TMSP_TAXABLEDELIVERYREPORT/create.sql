-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_TaxableDeliveryReport (

v_CardCode nvarchar(100),
v_postingDateFrom datetime,
v_postingDateTo datetime,
v_deliveryDateFrom datetime,
v_deliveryDateTo datetime,
v_documentDateFrom datetime,
v_documentDateTo datetime,
v_documentNumberFrom nvarchar(20),
v_documentNumberTo nvarchar(20),
v_dateDiffValue nvarchar(10)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	v_sqlString nvarchar (4000);
	v_vardate nvarchar(1000);
	v_cnt integer;
BEGIN
	
	select count (*) into v_cnt from "PUBLIC"."M_TEMPORARY_TABLES" where TABLE_NAME = '#TAXABLEDELIVERYREPORT';
   	if :v_cnt > 0 then
    	drop table #TAXABLEDELIVERYREPORT;
    end if;

	create local temporary column table #TaxableDeliveryReport(
	"DocNum" INTEGER not null,
	"DocEntry" INTEGER not null,
	"CardCode" NVARCHAR (15) null,
	"CardName" NVARCHAR (100) null,
	"DocDate" TIMESTAMP null,
	"VatSum" DECIMAL (34) null);
	v_sqlString := 'insert into #TaxableDeliveryReport select "DocNum", "DocEntry", "CardCode", "CardName", "DocDate", "VatSum"  from ODLN 	where "DocEntry" in  	( 		( 			 			select distinct T0."DocEntry" from DLN4 T0 			inner join OSTT T1 ON  T0."staType" = T1."AbsId"  AND  T1."NfTaxId" = -3 			inner join ODLN T9 ON T9."DocEntry" = T0."DocEntry" 			left outer join DLN1 T2 ON T2."DocEntry" = T0."DocEntry" and T2."LineNum" = T0."LineNum" and (T0."RelateType" = 1 or T0."RelateType" = 2) 			left outer join DLN3 T4 ON T4."DocEntry" = T0."DocEntry" and T4."LineNum" = T0."LineNum" and T4."ExpnsCode" = T0."ExpnsCode" and T0."RelateType" = 3 			where  			( 			(T0."RelateType" = 1 and t2."LineStatus" = ''O'') 			or 			(T0."RelateType" = 2 and t2."LineStatus"  = ''O'') 			or 			(T0."RelateType" = 3 and T4."Status" = ''O'') 			) 		)  		intersect  		( 			 			( 				select distinct T0."DocEntry" from DLN4 T0 				inner join OSTT T1  ON  T1."AbsId" = T0."staType"  AND  T1."NfTaxId" = -3 				where "DocEntry" not in 				( 				select distinct t0."DocEntry" from dln4 t0 				inner join OSTT T1 ON  T0."staType" = T1."AbsId"  AND  T1."NfTaxId" = -3 				inner join DPI1 T2 ON T2."BaseEntry" = t0."DocEntry" and T2."BaseType" = 15 and T2."BaseLine" = t0."LineNum" and t0."RelateType" = 1 				inner join DLN1 T3 ON T3."DocEntry" = T0."DocEntry" and T3."LineNum" = T0."LineNum" and T0."RelateType" = 1 				inner join ODLN T4 ON T4."DocEntry" = T0."DocEntry" 				where (T3."Quantity" = T2."Quantity" and T4."DocType" = ''I'') or (T3."Price" = T2."Price" and T4."DocType" = ''S'') 				) 			)  			union  			 			( 				select distinct T0."DocEntry" from DLN4 T0 				inner join OSTT T1  ON  T1."AbsId" = T0."staType"  AND  T1."NfTaxId" = -3 and t0."RelateType" = 1 				inner join ODLN T2  ON  T2."DocEntry" = T0."DocEntry" 				inner join DPI1 T3 ON T3."BaseEntry" = t0."DocEntry" and T3."BaseType" = 15 and T3."BaseLine" = t0."LineNum" and t0."RelateType" = 1 				inner join ODPI T4 ON T4."DocEntry" = T3."DocEntry" 				where T4."DocStatus"  <> ''C'' 			) 		)  	) 	AND "ServPostID" IS NULL 	AND "DocDate" < ADD_DAYS (current_date, -' || :v_dateDiffValue || ')';                                                                                                  
	
	
	
	-- "CardCode"
	if (:v_CardCode <> '') THEN
		v_sqlString := :v_sqlString || ' AND "CardCode" = ''' || :v_CardCode || '''';
	end if;

	SELECT TO_VARCHAR(:v_postingDateFrom, 'YYYYMMDD') INTO v_vardate from dummy;
	
	-- posting date
	if (:v_postingDateFrom <> '') then 
		v_sqlString := :v_sqlString || ' AND "DocDate" >= ''' || :v_vardate  || '''';
	end if;
	
	SELECT TO_VARCHAR(:v_postingDateTo, 'YYYYMMDD')  INTO v_vardate from dummy;
	if (:v_postingDateTo <> '') then
		v_sqlString := :v_sqlString || ' AND "DocDate" <= ''' || :v_vardate || '''';
	end if;
	
	-- delivery date

	SELECT TO_VARCHAR(:v_deliveryDateFrom, 'YYYYMMDD') INTO v_vardate from dummy;
	if (:v_deliveryDateFrom <> '') then
		v_sqlString := :v_sqlString || ' AND "DocDueDate" >= ''' || :v_vardate || '''';
	end if;

	SELECT TO_VARCHAR(:v_deliveryDateTo, 'YYYYMMDD') INTO v_vardate from dummy;
	if (:v_deliveryDateTo <> '') then
		v_sqlString := :v_sqlString || ' AND "DocDueDate" <= ''' || :v_vardate || '''';
	end if;
	
	-- document date
	SELECT TO_VARCHAR(:v_documentDateFrom, 'YYYYMMDD') INTO v_vardate from dummy;
	if (:v_documentDateFrom <> '') then
		v_sqlString := :v_sqlString || ' AND "TaxDate" >= ''' || :v_vardate || '''';
	end if;

	SELECT  TO_VARCHAR(:v_documentDateTo, 'YYYYMMDD') INTO v_vardate from dummy;
	if (:v_documentDateTo <> '') then
		v_sqlString := :v_sqlString || ' AND "TaxDate" <= ''' || :v_vardate || '''';
	end if;

	-- document number
	if (:v_documentNumberFrom <> '') then
		v_sqlString := :v_sqlString || ' AND "DocNum" >= ''' || :v_documentNumberFrom || '''';
	end if;

	if (:v_documentNumberTo <> '') then
		v_sqlString := :v_sqlString || ' AND "DocNum" <= ''' || :v_documentNumberTo || '''';
	end if;

	select * from #TaxableDeliveryReport;
	--exec (:v_sqlString);
END;


