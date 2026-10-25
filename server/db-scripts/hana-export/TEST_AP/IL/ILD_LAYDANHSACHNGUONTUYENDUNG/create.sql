Create PROCEDURE ILD_LayDanhSachNguonTuyenDung
as 
begin

create local temporary table #tbl (
		Code nvarchar(5),
		Name nvarchar(100)
		);
		
	Insert into #tbl (Code,Name) values	('0',	N'Nhân viên giới thiệu');
	Insert into #tbl (Code,Name) values	('1',	N'Vietnamworks');
	Insert into #tbl (Code,Name) values	('2',	N'Careerbuilder');
	Insert into #tbl (Code,Name) values	('3',	N'Jobstreet');
	Insert into #tbl (Code,Name) values	('4',	N'Vn.indeed.com');
	Insert into #tbl (Code,Name) values	('5',	N'Timviec365');
	Insert into #tbl (Code,Name) values	('6',	N'Timviecnhanh');
	Insert into #tbl (Code,Name) values	('7',	N'TopCV');
	Insert into #tbl (Code,Name) values	('8',	N'123Job');
	Insert into #tbl (Code,Name) values	('9',	N'Vieclam24h');
	Insert into #tbl (Code,Name) values	('10',	N'Ybox.vn');
	Insert into #tbl (Code,Name) values	('11',	N'Jobsgo');
	Insert into #tbl (Code,Name) values	('12',	N'Mywork');
	Insert into #tbl (Code,Name) values	('13',	N'Topdev');
	Insert into #tbl (Code,Name) values	('14',	N'ITViec');
	Insert into #tbl (Code,Name) values	('15',	N'Khác');


 
   select * from #tbl order by Code;
   
   drop table #tbl;
  
end;


