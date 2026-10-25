create procedure ILD_LayItemBaoTri_KiemDinh
as
begin
	select "ItemCode","ItemName" from OITM a where   a."ItemType"  <>  'F' 
	and (a."U_IsNeedCheckM" = 'Y' or  a."U_IsNeedCheckHRE" = 'Y' )
	and ifnull(a."U_IsAdditionFee",0) = 0 order by a."ItemName";

end;

