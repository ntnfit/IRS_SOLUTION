
create procedure apz_inventory_posting_list(in ItmFrom nvarchar(50) default '',
 in ItmTo nvarchar(50) default '',
  in BpFrom nvarchar(50) default '', 
  in BpTo nvarchar(50) default ''
, in DateFrom timestamp default null, in DateTo timestamp  default null, in WhsCode nvarchar(300) default '')
AS
BEGIN
declare MaxDate timestamp;
declare MinDate timestamp;
create local temporary table #tblItmTemplate
    ("ItemCode" NVARCHAR(50) null);
     INSERT INTO #tblItmTemplate
     SELECT "ItemCode" FROM oitm WHERE "ItemCode" BETWEEN IFNULL(NULLIF(:ItmFrom,''),
     IFNULL((SELECT TOP 1 "ItemCode" FROM oitm),'')) AND IFNULL(NULLIF(:ItmTo,''),
     IFNULL((SELECT TOP 1 "ItemCode" FROM oitm ORDER BY "ItemCode" desc),''));

 create local temporary table #tblBpTemplate ( "CardCode" NVARCHAR(50) null);

    INSERT INTO #tblBpTemplate
    SELECT "CardCode" FROM ocrd WHERE "CardCode" BETWEEN IFNULL(NULLIF(:BpFrom,''),
    IFNULL((SELECT TOP 1 "CardCode" FROM ocrd),'')) AND 
    IFNULL(NULLIF(:BpTo,''),IFNULL((SELECT TOP 1 "CardCode" FROM OCRD ORDER BY "CardCode" desc),''));
 
 SELECT IFNULL(:WhsCode,'') into WhsCode from DUMMY;
 create local temporary table #tblWHTemplate ( "WhsCode" NVARCHAR(50) null);
 
INSERT INTO #tblWHTemplate
 select a."WhsCode" from OWHS a inner join Split(:WhsCode,',') b on a."WhsCode" = trim(b.Data) or ifnull(:WhsCode, '') = '';
  
  	select ifnull((select MAX("DocDate") from OIVL),current_date) into MaxDate from DUMMY;
  	select ifnull((select MIN("DocDate") from OIVL),current_date) into MinDate from DUMMY;
  	
  	select ifnull(:DateFrom,:MinDate) into DateFrom from DUMMY;
  	select ifnull(:DateTo,:MaxDate) into DateTo from DUMMY;
  	create local temporary table #temline ("ROW_NUM" int
  	, "ItemCode" nvarchar(254)
	,"DocDate" timestamp 
	,"TransType" nvarchar(100)
	,"CreatedBy" nvarchar(100)
	,"BASE_REF" int
	,"DocLineNum" int
	,"LocCode" nvarchar(500) 
	,"BPCardCode" nvarchar(500)
	,"CardName" nvarchar(500)
	,"Balance" nvarchar(155)
	,"Price" decimal(19,6)
	,"Comment" nvarchar(500)
	,"Ref1" nvarchar(155)
	,"Ref2" nvarchar(155)
	,"TransSeq" nvarchar(155)
		 );

INSERT INTO #temline
    SELECT ROW_NUMBER() OVER (PARTITION BY T0."ItemCode" ORDER BY T0."DocDate") AS "ROW_NUM",T0."ItemCode",t0."DocDate",t0."TransType","CreatedBy",t0."BASE_REF",t0."DocLineNum",t0."LocCode",t1."BPCardCode",t1."CardName",
    CASE WHEN  t0."InQty">0 then t0."InQty" 
    ELSE -t0."OutQty" END "Quantity",
		SUM(T0."InQty"-T0."OutQty") OVER (PARTITION BY T0."ItemCode" ORDER BY T0."DocDate" rows between unbounded preceding and current row ) as "Balance",
	T0."Price",T1."Comment",T1."Ref1",T1."Ref2"
	--,T0."TransSeq"
    FROM OIVL T0
    INNER JOIN OILM T1 ON T0."MessageID"=T1."MessageID"
	INNER JOIN #tblWHTemplate T2 ON T2."WhsCode" = T0."LocCode"
	INNER JOIN #tblItmTemplate T3 on T3."ItemCode" = T0."ItemCode"
    WHERE (T1."BPCardCode" IN (SELECT "CardCode" FROM #tblBpTemplate) OR (:BpFrom='' AND :BpTo=''))
    AND T0."DocDate" between :DateFrom AND :DateTo
    ORDER BY T0."DocDate";

    SELECT * FROM #temline
   ORDER BY "ItemCode","DocDate" ;
     --drop table #temline;
END




