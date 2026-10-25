CREATE PROCEDURE TmSp_FetchActiveItem(
IN fromItemCodeOrg nvarchar(100),
IN toItemCodeOrg nvarchar(100),
IN fromDateOrg nvarchar(50),
IN includeInvoice nvarchar(1),
IN includeDelivery nvarchar(1),
IN includeOrder nvarchar(1),
IN includeQuota	nvarchar(1),
IN includeDPI nvarchar(1)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
hasData integer;
cnt		integer;
fromItemCode nvarchar(100);
toItemCode nvarchar(100);
fromDate nvarchar(50);
BEGIN

fromItemCode := LTRIM(RTRIM(:fromItemCodeOrg));
toItemCode := LTRIM(RTRIM(:toItemCodeOrg));
fromDate := LTRIM(RTRIM(:fromDateOrg));

if :fromItemCode = ''
then
select  top 1 "ItemCode" into fromItemCode from OITM order by "ItemCode" ASC;
end if;

if :toItemCode = ''
then
select  top 1 "ItemCode" into toItemCode from OITM order by "ItemCode" DESC;
end if;

SELECT COUNT(*) INTO cnt FROM "PUBLIC"."M_TEMPORARY_TABLES" where "TABLE_NAME" = upper('#TmpTblActiveItem');
if :cnt>0
then 
drop table #TmpTblActiveItem;
end if;
CREATE LOCAL TEMPORARY TABLE #TmpTblActiveItem ("ItemCode" nvarchar(50));
insert into  #TmpTblActiveItem (select "ItemCode" from OITM where 1<>1);

SELECT COUNT(*) INTO cnt FROM "PUBLIC"."M_TEMPORARY_TABLES" where "TABLE_NAME" = upper('#TmpTblABOM');
if :cnt>0
then
drop table #TmpTblABOM;
end if;
CREATE LOCAL TEMPORARY TABLE #TmpTblABOM ("ItemCode" nvarchar(50));
insert into  #TmpTblABOM (select "ItemCode" from OITM where 1<>1);


---for delivery and invoice we combine DOC1 and DOC14
if (:includeDelivery ='Y')
then
insert into  #TmpTblActiveItem
SELECT distinct T1."ItemCode" 
FROM "ODLN" T0 , "DLN1" T1 
WHERE T1."DocEntry" = T0."DocEntry" AND T0."DocDate" >= :fromDate
and T1."ItemCode" between  :fromItemCode and :toItemCode   
union
SELECT distinct T1."ItemCode" 
FROM "ODLN" T0 , "DLN14" T1 
WHERE T1."DocEntry" = T0."DocEntry" AND T0."DocDate" >= :fromDate  
and T1."ItemCode" between  :fromItemCode and :toItemCode;  
end if;

if :includeInvoice ='Y'
then
insert into  #TmpTblActiveItem
SELECT distinct T1."ItemCode" 
FROM "OINV" T0 , "INV1" T1 
WHERE T1."DocEntry" = T0."DocEntry" AND T0."DocDate" >= :fromDate 
and T1."ItemCode" between  :fromItemCode and :toItemCode 
union
SELECT distinct T1."ItemCode" 
FROM "OINV" T0 , "INV14" T1 
WHERE T1."DocEntry" = T0."DocEntry" and T0."isIns" = 'N' AND T0."DocDate" >= :fromDate 
and T1."ItemCode" between  :fromItemCode and :toItemCode ; 
end if;

--for orders and reserve invoice, we need expand ABOM later
if :includeInvoice ='Y'
then
insert into #TmpTblABOM
SELECT distinct T1."ItemCode" 
FROM "OINV" T0 Join "INV1" T1
on T1."DocEntry" = T0."DocEntry" 
Join OITT T2 on T1."ItemCode" = T2."Code"
WHERE T2."TreeType" = 'A' 
and T0."isIns" = 'Y'
and T0."DocDate" >= :fromDate; 

end if;

if :includeOrder ='Y'
then
insert into  #TmpTblActiveItem
SELECT distinct T1."ItemCode" 
FROM "ORDR" T0 , "RDR1" T1 
WHERE T1."DocEntry" = T0."DocEntry" AND T0."DocDate" >= :fromDate 
and T1."ItemCode" between  :fromItemCode and :toItemCode ;

insert into #TmpTblABOM
SELECT distinct T1."ItemCode" 
FROM "ORDR" T0 Join "RDR1" T1
on T1."DocEntry" = T0."DocEntry" 
Join OITT T2 on T1."ItemCode" = T2."Code"
WHERE T2."TreeType" = 'A' 
and T0."DocDate" >= :fromDate; 

 
end if;

if :includeQuota ='Y'
then
insert into  #TmpTblActiveItem
SELECT distinct T1."ItemCode" 
FROM "OQUT" T0 , "QUT1" T1 
WHERE T1."DocEntry" = T0."DocEntry" AND T0."DocDate" >= :fromDate 
and T1."ItemCode" between  :fromItemCode and :toItemCode ;

insert into #TmpTblABOM
SELECT distinct T1."ItemCode" 
FROM "OQUT" T0 Join "QUT1" T1
on T1."DocEntry" = T0."DocEntry" 
Join OITT T2 on T1."ItemCode" = T2."Code"
WHERE T2."TreeType" = 'A' 
and T0."DocDate" >= :fromDate; 

  
end if;

if :includeDPI ='Y'
then 
insert into  #TmpTblActiveItem
SELECT distinct T1."ItemCode" 
FROM "ODPI" T0 , "DPI1" T1 
WHERE T1."DocEntry" = T0."DocEntry" AND T0."DocDate" >= :fromDate 
and T1."ItemCode" between  :fromItemCode and :toItemCode ;

insert into #TmpTblABOM
SELECT distinct T1."ItemCode" 
FROM "ODPI" T0 Join "DPI1" T1
on T1."DocEntry" = T0."DocEntry" 
Join OITT T2 on T1."ItemCode" = T2."Code"
WHERE T2."TreeType" = 'A' 
and T0."DocDate" >= :fromDate ;

   
end if;

--here we need expand ABOM item
select count(distinct T2."Code") into hasData from 
#TmpTblABOM T0 
join OITT T1
on T0."ItemCode" = T1."Code"
and T1."TreeType" in('A','S','T')
join ITT1 T2
on T1."Code" = T2."Father"
left join #TmpTblABOM T3
on T3."ItemCode" = T2."Code"
where T3."ItemCode" is null;

while :hasData > 0
do 
insert into #TmpTblABOM
select distinct T2."Code" from 
#TmpTblABOM T0 
join OITT T1
on T0."ItemCode" = T1."Code"
and T1."TreeType" in('A','S','T')
join ITT1 T2
on T1."Code" = T2."Father"
left join #TmpTblABOM T3
on T3."ItemCode" = T2."Code"
where T3."ItemCode" is null;

select count(distinct T2."Code") into hasData from 
#TmpTblABOM T0 
join OITT T1
on T0."ItemCode" = T1."Code"
and T1."TreeType" in('A','S','T')
join ITT1 T2
on T1."Code" = T2."Father"
left join #TmpTblABOM T3
on T3."ItemCode" = T2."Code"
where T3."ItemCode" is null;
end while;

delete
from #TmpTblABOM T0
where T0."ItemCode" in (select "Code" from OITT where "TreeType" in ('A','S'));

--finally we return 
select *
from
(
select distinct "ItemCode" from #TmpTblActiveItem
union
select distinct "ItemCode" from #TmpTblABOM
) T0
order by 1;

SELECT COUNT(*) INTO cnt FROM "PUBLIC"."M_TEMPORARY_TABLES" where "TABLE_NAME" = upper('#TmpTblActiveItem');
if :cnt>0
then 
drop table #TmpTblActiveItem;
end if;

SELECT COUNT(*) INTO cnt FROM "PUBLIC"."M_TEMPORARY_TABLES" where "TABLE_NAME" = upper('#TmpTblABOM');
if :cnt>0
then
drop table #TmpTblABOM;
end if;

END;


