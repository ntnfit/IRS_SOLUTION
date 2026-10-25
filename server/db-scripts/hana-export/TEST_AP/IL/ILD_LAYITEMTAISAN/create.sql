create procedure ILD_LayItemTaiSan
as
begin
	select "ItemCode","ItemName" from OITM a where   a."ItemType"  =  'F'
	and ifnull(a."U_IsAdditionFee",0) = 0 order by a."ItemName";
end

