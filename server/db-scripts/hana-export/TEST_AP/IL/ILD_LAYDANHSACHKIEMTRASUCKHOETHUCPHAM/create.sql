Create PROCEDURE ILD_LayDanhSachKiemTraSucKhoeThucPham
as 
begin

create local temporary table #tbl (
		Code nvarchar(5),
		Name nvarchar(50)
		);
		
		insert into #tbl(Code,Name) values ('0',N'Không cần kiểm tra' );
		insert into #tbl(Code,Name) values ('1',N'Cần kiểm tra' );
		insert into #tbl(Code,Name) values ('2',N'Đang chờ kết quả');
		insert into #tbl(Code,Name) values ('3',N'Kết quả đạt' );
 
   select * from #tbl order by Code;
   
   drop table #tbl;
  
end;


