CREATE procedure ILD_R_IRS_DinhMucSanXuat
(
	FGCODE nvarchar(250),
	GROUPID nvarchar(50)
)
as
Begin

	declare  tbl table
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
	
	insert into :tbl (Level,ItemCode,ItemName,UOM, Warehouse,Quantity, Father)
	select 1 as "Level",
		 "Code" as "FgCode",
		 "Name" as "FgName",
		 '' as "UOM",
		 "ToWH" as "Warehouse",
		 "Qauntity" as "Quantity",
		 "Code" as "Father"
	 
	from OITT
	where  
		(
					ifnull(:FGCODE, '') = '' 
					or cast ("Code" as nvarchar) in (select "DATA" from split(:FGCODE ,','))
		) ;
---
	
	insert into :tbl (Level,ItemCode,ItemName,UOM, Warehouse,Quantity,Father,ItemGroup)
	select 2 as "Level",
		 a."Code" as "ItemCode",
		 a."ItemName" as "ItemName",
		 c."InvntryUom" as "UOM",
		 --a."Uom" as "UOM",
		 a."Warehouse" as "Warehouse",
		 a."Quantity" as "Quantity",
		 a."Father" as "FatherCode",
		 d."ItmsGrpNam"
		 
		 
	from  ITT1 a
	inner join (select distinct ItemCode from :tbl   where Level = 1) b on b.ItemCode = a."Father"
	inner join OITM c  on a."Code" = c."ItemCode" 
	inner join OITB  d on  c."ItmsGrpCod"= d."ItmsGrpCod"
	where  
	(
					ifnull(:GROUPID, '') = '' 
					or cast (c."ItmsGrpCod" as nvarchar) in (select "DATA" from split(:GROUPID ,','))
	) ;

	--
	delete from :tbl where Level =1;
	 
	 
	select Level,ItemCode,ItemName,UOM, Warehouse,Quantity,Father, ItemGroup
	from :tbl
	order by Father;
end;
