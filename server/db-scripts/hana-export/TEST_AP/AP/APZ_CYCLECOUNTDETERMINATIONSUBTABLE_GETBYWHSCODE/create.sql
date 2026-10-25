
create PROCEDURE APZ_CycleCountDeterminationSubtable_GetByWhsCode(in whsCode nvarchar(20))
AS 
BEGIN
create local temporary table #tblLocal 
(
"Name" nvarchar(254),
"WhsCode" nvarchar(8),
"Entry" int,
"CycleCode" smallint,
"Alert" char(1),
"DestUser" smallint,
"NextDate" timestamp,
"Time" smallint,
"ExcldZrQty" char(1),
"Alerted" char(1) ,
"ChangExist" char(1),
"TempEntry" int
);
insert  into #tblLocal (
select T0."ItmsGrpNam" as "Name",T1.*,T0."ItmsGrpCod" as "TempEntry"  from ccs1 t1 right join oitb T0 on t0."ItmsGrpCod"=t1."Entry" and "WhsCode"=:whsCode
union 
select T0."SLCode" as "Name",T1.*,T0."AbsEntry" as "TempEntry" from ccs1 t1 right join OBSL T0 on t0."AbsEntry"=t1."Entry" and "WhsCode"=:whsCode 
); 
update #tblLocal set "WhsCode"=:whsCode,"Entry"="TempEntry";

select * from #tblLocal;
Drop table #tblLocal;

END;




