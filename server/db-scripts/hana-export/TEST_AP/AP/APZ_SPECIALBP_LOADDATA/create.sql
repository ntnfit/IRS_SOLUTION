
Create procedure APZ_SpecialBp_LoadData(in cardCode nvarchar(50))
As 
Begin

	select *, 'Y' "IsSpecial" from OCRD where "CardCode" = :cardCode;

	create local temporary table #tbl as (select top 0 *, cast(null as nvarchar(254)) "ItemName", cast(null as numeric(19,6)) "AddPrice1", cast(null as numeric(19,6)) "AddPrice2"
												, cast(null as nvarchar(254)) "UomName", cast(null as numeric(19,6)) "PriceBefDis", cast(null as nvarchar(1)) "IsUpdate" from OSPP);
	
	insert into #tbl
		SELECT
			 a.*,
			 b."ItemName" AS "ItemName",
			 c."AddPrice1" AS "AddPrice1",
			 c."AddPrice2" AS "AddPrice2",
			 b."InvntryUom" AS "UomName",
			 case when a."SrcPrice" = 0 then c."Price" when a."SrcPrice" = 1 then c."AddPrice1" else c."AddPrice2" end, 'N'
		FROM OSPP a 
			LEFT OUTER JOIN oitm b ON a."ItemCode" = b."ItemCode" 
			LEFT OUTER JOIN itm1 c ON a."ItemCode" = c."ItemCode" AND a."ListNum" = c."PriceList" 
			LEFT OUTER JOIN OUOM d ON d."UomEntry" = b."INUoMEntry" 
		WHERE a."CardCode" = :cardCode order by "ItemCode";
		
	update a set "Currency" = b."Currency", "Price" = a."PriceBefDis" * (1 - a."Discount"/100) from #tbl a inner join ITM1 b on a."ItemCode" = b."ItemCode" and a."ListNum" = b."PriceList";
	
	select * from #tbl;
	
	create local temporary table #spp1 as (select top 0 *, cast(null as numeric(19,6)) "SourcePrice" from SPP1);
	insert into #spp1
		select *, null from SPP1 where "CardCode" = :cardCode order by "LINENUM";
		
	update a set "Price" = b."Price" * (1 - a."Discount" / 100), "Currency" = b."Currency", "SourcePrice" = b."Price" from #spp1 a inner join ITM1 b ON a."ItemCode" = b."ItemCode" and a."ListNum" = b."PriceList";
	
	create local temporary table #spp2 as (select top 0 * from SPP2);
	insert into #spp2
		select * from SPP2 where "CardCode" = :cardCode Order by "SPP2LNum";
		
	update a set "Price" = b."SourcePrice" * (1 - a."Discount" / 100), "Currency" = b."Currency" from #spp2 a
																						INNER JOIN #spp1 b ON a."SPP1LNum" = b."LINENUM" and a."ItemCode" = b."ItemCode";
	
	select * from #spp1;
	select * from #spp2;

End;




