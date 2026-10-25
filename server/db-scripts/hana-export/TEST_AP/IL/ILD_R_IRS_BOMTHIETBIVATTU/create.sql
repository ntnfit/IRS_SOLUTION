CREATE procedure ILD_R_IRS_BOMThietBiVatTu
(
	in TaiSanCode nvarchar(1000),
	in Floc nvarchar(1000) --Cụm chức năng (Floc - Level 5) 

)
as 
begin
	create local temporary table #tbl
	(
		FixAssetCode nvarchar(50),
		FixAssetName nvarchar(100),
		Floc nvarchar(50),
		ItemCode nvarchar(50),
		ItemName nvarchar(100),
		Quantity int,
		UOM nvarchar(20),
		WarrantyFromDate nvarchar(10),
		WarrantyToDate	nvarchar(10),
		WarrantyExpDays int, --Warranty Expiration: Số ngày sắp đến bảo hành 
		Creator nvarchar(10)
		
	);
	
	insert into #tbl (FixAssetCode, FixAssetName, Floc, ItemCode, ItemName, Quantity, UOM, 
					  WarrantyFromDate, WarrantyToDate,WarrantyExpDays, Creator)
	select a."U_ItemCodeF" as "FixAssetCode", 
		   a."U_ItemNameF" as "FixAssetName",
		   b."U_Floc",
	   	   b."U_ItemCode",
	   	   b."U_ItemName",
	   	   b."U_Quantity",
	       b."U_UOM",
	       TO_VARCHAR(b."U_WarrantyFromDate",'DD/MM/YYYY'),
	       TO_VARCHAR(b."U_WarrantyToDate",'DD/MM/YYYY'),
	       Days_Between (b."U_WarrantyFromDate",b."U_WarrantyToDate" ) as "WarrantyExpDays",
	   	   a."Creator"
	from  "@BOMAINTENANCE_H" a
	inner join "@BOMAINTENANCE_L" b on a."DocEntry"  = b."DocEntry"
	where 1 = 1 
			and
			(
				 ifnull(:TaiSanCode, '') = '' 
				 or cast (a."U_ItemCodeF" as nvarchar) in (select "DATA" from split(:TaiSanCode,','))
			)
			and
			(
				 ifnull(:Floc, '') = '' 
				 or cast (b."U_Floc" as nvarchar) in (select "DATA" from split(:Floc,','))
			)
	
	;
	
	select * from #tbl;
	drop table #tbl;
	
end;

