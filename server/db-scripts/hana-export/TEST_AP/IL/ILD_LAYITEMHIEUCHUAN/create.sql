create procedure ILD_LayItemHieuChuan
as
begin
	select "ItemCode","ItemName" from OITM a 
	where   
	 a."U_IsNeedCheckC" = 'Y' 
	and ifnull(a."U_IsAdditionFee",0) = 0 
	order by a."ItemName";

end;

