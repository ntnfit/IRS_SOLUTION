
-- apz_ProductionOrder_GetADoc 1
CREATE PROCEDURE apz_ProductionOrder_GetADoc(
in DocEntry int default 0
)
AS
begin
	
	select T0.*,
	(select ifnull("BPLid",-1) from OWHS where "WhsCode"=T0."Warehouse") "BplId",
	cast(null as decimal(21,6)) "ActualCompCost",
	cast(null as decimal(21,6)) "ActualProCost",
	cast(null as decimal(21,6)) "TotalVariable",
	cast(null as decimal(21,6)) "ActualAddCost"
	from OWOR T0 WHERE T0."DocEntry"=:DocEntry;
	
	/*
	create local temporary table #tbl as (SELECT top 0 *,cast('' as nvarchar(254)) "ItemName",cast(1 as int) "BplId" FROM owor);
	
	insert into #tbl
		SELECT T0.*,T1."ItemName",IfNULL(T2."BPLId",-1) "BplId" FROM owor T0
																INNER JOIN OITM T1 ON T0."ItemCode"=T1."ItemCode"
																LEFT JOIN NNM1 T2 ON T0."Series"=T2."Series" AND T2."ObjectCode"='202' WHERE T0."DocEntry"=:DocEntry;
	 
	SELECT a.*, b."ActualCompCost", c."ActualProCost", ifnull(c."ActualProCost",0) - ifnull(b."ActualCompCost",0) - ifnull(d."ActualAddCost",0) "TotalVariable", d."ActualAddCost" "ActualAddCost" FROM #tbl a 
						LEFT JOIN (select sum(ifnull("LineTotal",0)) "ActualCompCost" from IGE1 x INNER JOIN OITM y ON x."ItemCode" = y."ItemCode" where "BaseType" = 202 and "BaseEntry" = :DocEntry and y."InvntItem" = 'Y') b ON 1 = 1
						LEFT JOIN (select sum(ifnull("LineTotal",0)) "ActualAddCost" from IGE1 x INNER JOIN OITM y ON x."ItemCode" = y."ItemCode" where "BaseType" = 202 and "BaseEntry" = :DocEntry and y."InvntItem" = 'N') d ON 1 = 1
						LEFT JOIN (select SUM(ifnull("LineTotal",0)) "ActualProCost" from IGN1 where "BaseType" = 202 and "BaseEntry" = :DocEntry) c ON 1 = 1;
						
	
	drop table  #tbl;
	*/
end

