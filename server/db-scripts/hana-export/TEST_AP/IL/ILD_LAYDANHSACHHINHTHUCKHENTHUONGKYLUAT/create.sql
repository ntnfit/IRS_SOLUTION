CREATE PROCEDURE ILD_LayDanhSachHinhThucKhenThuongKyLuat
as 
begin

  select "Code","Name" from APZ_OHPT order by "Code", "Type";

end;


