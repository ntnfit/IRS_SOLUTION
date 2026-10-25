
create PROCEDURE APZ_CycleCountDetermination_GetList()
AS 
BEGIN
create local temporary table #tablLocal
(
"WhsCode" nvarchar(50),
"WhsName" nvarchar(254),
"BinActivat" char(1),
"CycleBy" int,
"CycleType"  char(1)
);
insert into #tablLocal (select a."WhsCode", a."WhsName",a."BinActivat" as "BinActivat",b."CycleBy",b."CycleType" from OWHS  a left join OCCS b on a."WhsCode"=b."WhsCode");
Update #tablLocal set "CycleBy"=0,"CycleType"='G' where IFNULL("CycleBy",0)=0;
select * from #tablLocal;
Drop table #tablLocal;
END;




