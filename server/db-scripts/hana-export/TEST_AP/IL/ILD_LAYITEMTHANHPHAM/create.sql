Create procedure ILD_LayItemThanhPham
as
begin
		select "ItemCode","ItemName" from OITM a 
		where   
			  a."ItmsGrpCod"  in (120,121)
		order by a."ItemCode";
end;

