CREATE procedure ILD_R_IRS_LoadSalaryPosted
(
	Thang nvarchar(500),
	Nam nvarchar(500)

)
as
BEGIN
declare trangThai nvarchar(20);
create local temporary table #tbl
(	
	PhongBan nvarchar(20),
	
	BHXH_NLD DECIMAL(21,6),
	BHYT_NLD DECIMAL(21,6),
	BHTN_NLD DECIMAL(21,6),
	KPCD_NLD DECIMAL(21,6),
	TNCN_NLD DECIMAL(21,6),
	THUC_LINH DECIMAL(21,6),
	KhenThuongKhac DECIMAL(21,6),
	
	BHXH_CTY DECIMAL(21,6),
	BHYT_CTY DECIMAL(21,6),
	BHTNNLD_CTY DECIMAL(21,6),
	BHTN_CTY DECIMAL(21,6),
	KPCD_CTY DECIMAL(21,6),
	
	CHIPHITHUONG DECIMAL(21,6),
	CHIPHITHUONGTAM DECIMAL(21,6), --Chi phí thưởng tạm tính
	DIEUCHINHTANG DECIMAL(21,6),
	DIEUCHINHGIAM DECIMAL(21,6),
	
	
	
	NgayTao nvarchar(20),
	UserTao nvarchar(15),
	
	KyTinhLuong nvarchar(20),
	TrangThai  nvarchar(50)
	
	
	 
);

IF (ifnull(Thang,'') <>  '' and  ifnull(Nam,'') <>  '') then
 JE=  select 

		"U_PhongBan" as "PhongBan",
		sum("U_BHXH_NLD") as "BHXH_NLD",
		sum("U_BHYT_NLD")  as  "BHYT_NLD",
		sum("U_BHTN_NLD") as "BHTN_NLD",
		sum("U_KPCD_NLD") as  "KPCD_NLD",
		sum("U_TNCN_NLD") as  "TNCN_NLD",
		sum("U_THUC_LINH") as "THUC_LINH",
		sum("U_KT_KHAC_NLD") as "KhenThuongKhac",
		-- Công ty
		sum("U_BHXH_CTY") as "BHXH_CTY",
		sum("U_BHYT_CTY") as "BHYT_CTY",
		sum("U_BHTNNLD_CTY") as "BHTNNLD_CTY" ,
		sum("U_BHTN_CTY") as  "BHTN_CTY",
		sum("U_KPCD_CTY") as "KPCD_CTY",
		-- Khen thuong
		sum("U_CHIPHITHUONG") as "CHIPHITHUONG",
		sum("U_CHIPHITHUONGTAM") as "CHIPHITHUONGTAM",
		-- Dieu chinh
		sum("U_DIEUCHINHTANG") as "DIEUCHINHTANG",
		sum("U_DIEUCHINHGIAM") as "DIEUCHINHGIAM",
		
		TO_VARCHAR("U_NgayTao",'DD/MM/YYYY') as  "NgayTao",
		"U_UserCode" as "UserTao",
		Max("U_KyTinhLuong") as "KyTinhLuong",
		
		Max("U_TrangThaiTuKeToan") as "TrangThai"
		
		from  "@SALARYIRSTOSAP"
		where "U_Thang"  = Thang and "U_Nam"  =  Nam
		group by "U_PhongBan","U_NgayTao","U_UserCode";
		
		
		
 insert into #tbl (PhongBan,BHXH_NLD,BHYT_NLD,BHTN_NLD,KPCD_NLD,TNCN_NLD,THUC_LINH,KhenThuongKhac,
 					BHXH_CTY, BHYT_CTY, BHTNNLD_CTY, BHTN_CTY,KPCD_CTY,
 					CHIPHITHUONG,CHIPHITHUONGTAM, DIEUCHINHTANG, DIEUCHINHGIAM,
 					NgayTao, UserTao,KyTinhLuong, TrangThai )
 select "PhongBan","BHXH_NLD","BHYT_NLD","BHTN_NLD","KPCD_NLD","TNCN_NLD","THUC_LINH","KhenThuongKhac",
 					"BHXH_CTY", "BHYT_CTY", "BHTNNLD_CTY", "BHTN_CTY","KPCD_CTY",
 					"CHIPHITHUONG","CHIPHITHUONGTAM", "DIEUCHINHTANG", "DIEUCHINHGIAM",
 					"NgayTao", "UserTao",  "KyTinhLuong","TrangThai"
 from :JE;
  
 
 
 
END IF;
 


select * from #tbl;
drop table #tbl;




END;

