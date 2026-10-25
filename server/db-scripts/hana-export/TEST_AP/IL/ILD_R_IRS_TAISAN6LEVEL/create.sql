CREATE procedure ILD_R_IRS_TaiSan6Level
(
	in TaiSanCode nvarchar(250)
)
as
begin
/*
	U_Level1: nha may 1
	U_Level2: quy trinh cafe
	U_Level3: he thong cap lieu cafe
	U_Level4: Tài sản (Fix Asset Master) - Cha
    -- Level 5 va 6 phải lấy từ BOM: vì ở đây thiết lâp tài sản - floc và item con đi theo
	U_Level5: Cụm chức năng (Floc)
	U_Level6: Item master data (Con)
	
*/

create local temporary table #taisan
(
	Level1 nvarchar(50),
	Level2 nvarchar(50),
	Level3 nvarchar(50),
	FixAssetCode nvarchar(50), -- Level 4
	FixAssetName nvarchar(250)-- Leval 4
	
);
create local temporary table #bom
(
		FixAssetCode nvarchar(50),
		FixAssetName nvarchar(100),
		Floc nvarchar(50), --Level5
		ItemCode nvarchar(50), --Level6
		ItemName nvarchar(100), --Level6
		Quantity int,
		UOM nvarchar(20)
);
-- Insert dữ liệu Bom
insert into #bom (FixAssetCode, FixAssetName, Floc, ItemCode, ItemName, Quantity, UOM)
select a."U_ItemCodeF" as "FixAssetCode", 
		   a."U_ItemNameF" as "FixAssetName",
		   b."U_Floc",
	   	   b."U_ItemCode",
	   	   b."U_ItemName",
	   	   b."U_Quantity",
	       b."U_UOM"
	from  "@BOMAINTENANCE_H" a
	inner join "@BOMAINTENANCE_L" b on a."DocEntry"  = b."DocEntry"
	where 1 = 1 
			and
			(
				 ifnull(:TaiSanCode, '') = '' 
				 or cast (a."U_ItemCodeF" as nvarchar) in (select "DATA" from split(:TaiSanCode,','))
			);

--
insert into #taisan
select "U_Level1","U_Level2","U_Level3","ItemCode" as "FixAssetCode", "ItemName" as "FixAssetName"
from OITM a 
where   a."ItemType"  =  'F' and ifnull(a."U_IsAdditionFee",0) = 0;
	
select a.Level1,a.Level2,a.Level3,a.FixAssetCode,a.FixAssetName,b.Floc, b.ItemCode, b.ItemName
from   #taisan a
inner join  #bom b on a.FixAssetCode  =  b.FixAssetCode
order by a.FixAssetCode;


drop table #taisan;
drop table #bom;

end;

