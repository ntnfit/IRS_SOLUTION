
CREATE PROCEDURE APZ_BinLocations_GetBinLocProp(IN absEntry int)
AS
BEGIN

	declare wghtInMG numeric(19, 6);
	declare unitDisp nvarchar(2);
	declare itemWght numeric(19, 3);	
	
	declare itemQty int; -- item quantity	
	declare noOfItems int; -- number of items	
	declare noOfBase int; -- number of batch or serial
	
	select (select SUM("OnHandQty") from OIBQ where "BinAbs" = :absEntry) into itemQty from dummy;
	
	select (select COUNT(1) from OIBQ where "BinAbs" = :absEntry) into noOfItems from dummy;
	
	select (select COUNT(1) from OSBQ where "BinAbs" = :absEntry) + (select COUNT(1) from OBBQ where "BinAbs" = :absEntry) into noOfBaSe from dummy;
	
	create local temporary table #tbWght ("WightInMG" numeric(19, 6), "UnitDisply" nvarchar(2));
	
	insert into #tbWght select top 1 c."WightInMG", c."UnitDisply" 
	from OIBQ a inner join OITM b on a."ItemCode" = b."ItemCode"
		inner join OWGT c on b."IWght1Unit"=c."UnitCode" where a."BinAbs" = :absEntry order by c."WightInMG" desc;
	
	select (select "WightInMG" from #tbWght) into wghtInMG from dummy;
	select (select "UnitDisply" from #tbWght) into unitDisp from dummy;
	
	select (select sum(x."OnHandQty"*y."IWeight1"*z."WightInMG"/:wghtInMG) from OIBQ x inner join OITM y on x."ItemCode" = y."ItemCode" 
			inner join OWGT z on y."IWght1Unit"=z."UnitCode" where x."BinAbs" = :absEntry) into itemWght from dummy;

	select a.* 
		,case :itemWght 
			when null then null 
			else (CAST(:itemWght as nvarchar) || :unitDisp) 
			end as "ItmWght"
		,:itemQty as "ItemQty"
		,nullif(:noOfItems, 0) as "NoOfItems"
		,nullif(:noOfBase, 0) as "NoOfBaSe"

	from OBIN a where a."AbsEntry" = :absEntry;
	
END;




