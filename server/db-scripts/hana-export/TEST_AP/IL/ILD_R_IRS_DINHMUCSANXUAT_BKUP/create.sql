CREATE procedure ILD_R_IRS_DinhMucSanXuat_Bkup
(
	FGCODE nvarchar(250),
	GROUPID nvarchar(50)
)
as
Begin

	Create local temporary table #tbl
	(
		Level smallint,
		ItemCode nvarchar(50),
		ItemName nvarchar(150),
		ItemGroup nvarchar(50),
		UOM nvarchar(20),
		Warehouse nvarchar(50),
		Quantity decimal(19,6),
		Father nvarchar(50)
		
	);
	
	insert into #tbl (Level,ItemCode,ItemName,UOM, Warehouse,Quantity)
	select 1 as "Level",
		 "Code" as "FgCode",
		 "Name" as "FgName",
		 '' as "UOM",
		 "ToWH" as "Warehouse",
		 "Qauntity" as "Quantity"
	 
	from OITT;
---
	
	insert into #tbl (Level,ItemCode,ItemName,UOM, Warehouse,Quantity,Father)
	select 2 as "Level",
		 "Code" as "ItemCode",
		 "ItemName" as "ItemName",
		 "Uom" as "UOM",
		 "Warehouse" as "Warehouse",
		 "Quantity" as "Quantity",
		 "Father" as "FatherCode"
		 
	from  ITT1;
	
	update a
	set ItemGroup  = c."ItmsGrpNam",
		UOM =  b."InvntryUom"
	from #tbl a
	inner join OITM b on a.ItemCode = b."ItemCode" 
	inner join OITB c on  b."ItmsGrpCod"= c."ItmsGrpCod";
	
	
	update a
	set Father  =  ItemCode
	from #tbl a
	where Ifnull(Father,'') = '';
	 
	 
	select Level,ItemCode,ItemName,UOM, Warehouse,Quantity,Father, ItemGroup
	from #tbl
	where  
				(
					ifnull(:FGCODE, '') = '' 
					or cast (ItemCode as nvarchar) in (select "DATA" from split(:FGCODE ,','))
				) 
	order by Father;
	drop table #tbl;
end;
