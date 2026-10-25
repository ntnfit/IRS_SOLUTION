CREATE procedure ILD_LayItemKiemDinh
as
begin
	select "ItemCode","ItemName" from OITM a where   a."ItemType"  <>  'F' 
	and a."U_IsNeedCheckHRE" = 'Y'
	and ifnull(a."U_IsAdditionFee",0) = 0 order by a."ItemName";

end;

