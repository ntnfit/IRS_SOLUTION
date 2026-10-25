CREATE procedure ILD_R_IRS_LoadSalaryIRS
(
	Thang nvarchar(500),
	Nam nvarchar(500),
	UserCode NVARCHAR(500)
	

)
as
begin
/*
	Tham số khu run báo cáo: Chọn tháng + năm --> Kiểm tra trong 2 table Journal Voucher và JE Entry:
		1. Nếu mà KHÔNG tồn tại U_PeriodProd =  (Năm và Tháng của tham số), thì bắt đầu query, Ngược Lại báo số JE
*/
declare NgayThangTinhLuong date;
create local temporary table #tbl
(	NhomButToan smallint,
	BHXH_NLD DECIMAL(21,6),
	BHYT_NLD DECIMAL(21,6),
	BHTN_NLD DECIMAL(21,6),
	KPCD_NLD DECIMAL(21,6),
	
	BHXH_CTY DECIMAL(21,6),
	BHYT_CTY DECIMAL(21,6),
	BHTNNLD_CTY DECIMAL(21,6),
	BHTN_CTY DECIMAL(21,6),
	KPCD_CTY DECIMAL(21,6) ,
	AN_CA DECIMAL(21,6),
	TNCN_NLD DECIMAL(21,6),
	KT_KHAC_NLD DECIMAL(21,6),
	THUC_LINH DECIMAL(21,6),
	
	CHIPHITHUONG DECIMAL(21,6),
	CHIPHITHUONGTAM DECIMAL(21,6),-- Chi Phí thưởng tạm
	CHIPHIOT DECIMAL(21,6),
	CHIPHIPHUCAP DECIMAL(21,6),
	
	DIEUCHINHTANG DECIMAL(21,6),
	DIEUCHINHGIAM DECIMAL(21,6),
	
	
	TaiKhoan nvarchar(15),
	TenTaiKhoanCP nvarchar(100),
	Debit decimal(21,6),
	Credit decimal(21,6),
	PhongBan nvarchar(20),
	CostCenter nvarchar(20),
	GhiChu nvarchar(100),
	Thang varchar(2),
	Nam varchar(4),
	STT2 smallint,
	UserCode nvarchar(20),
	UserID smallint
	 
);
create local temporary table #ThuongTamTinh
(	
	PhongBan nvarchar(20),
	CostCenter nvarchar(20),
	Luong13 	decimal(21,6),
	ThuongThanhTich decimal(21,6),
	TongThuongTam decimal(21,6) -- Tong thuong tam tinh
);
------============================== Lương tháng 13 tạm tính ==========================---------------
/* -- Chia theo tháng
insert into #ThuongTamTinh (PhongBan,CostCenter,Luong13)
select "PhongBan", "CostCenter",sum("Luong13") as "Luong13"
from (	
		select "empID","CostCenter", "PhongBan",
			  case when "SoThangLamViec">0 and ifnull("BasicSal",0)>0  then 
			  		cast("BasicSal"/"SoThangLamViec"   as decimal(21,1)) end as "Luong13"
		from (
		
			select year("startDate") as "NamVaoLam", nv."empID",  nv."CostCenter", pb."Name" as  "PhongBan",
				   luong."BasicSal", 
				   case when ifnull("startDate",'') <> '' and  year("startDate") =  :Nam then
				     cast( DAYS_BETWEEN("startDate",  NgayThangTinhLuong)/28  as int)
				   	end as  "SoThangLamViec"
			from APZ_OSAL luong
			inner join OHEM nv on luong."empID"  = nv."empID"
			inner join OUDP Pb on nv."dept"  = pb."Code" ) as Nguon1 
		) as Nguon2
	   group by  "CostCenter","PhongBan";
*/

IF(ifnull(Thang,'') <> '' and  ifnull(Nam,'') <>'') then
------============================== Lương tháng 13 tạm tính ==========================---------------

insert into #ThuongTamTinh (PhongBan,CostCenter,Luong13, ThuongThanhTich, TongThuongTam)
 select  "PhongBan", "CostCenter",   sum ("Luong13_TungThang"),  sum ("ThuongThanhTich"), sum ("Luong13_TungThang") + sum ("ThuongThanhTich")
from (

select "empID","PhongBan", "CostCenter",
--("BasicSal"/365 )* "SoNgaySeLamDen31_12" as "Luong13_CaNam",
(("BasicSal"/365 )* "SoNgaySeLamDen31_12") / "SoThangSeLamDen31_12" as "Luong13_TungThang",
"BasicSal" * 0.1 as "ThuongThanhTich"
from (	
		select "empID","CostCenter", "PhongBan","BasicSal",
		-- Effective days (Số ngày) : (31/12/NamTinhLuong -   EffectiveDate) +1
		 cast( DAYS_BETWEEN("EffectiveDate",  Cast(cast(nam as char(4)) ||'-12'    ||'-31' as date))+1  as int) as "SoNgaySeLamDen31_12",
		 cast (cast( DAYS_BETWEEN("EffectiveDate",  Cast(cast(nam as char(4)) ||'-12'    ||'-31' as date))  as int)/ 30  as decimal(21,6)) as "SoThangSeLamDen31_12" 
		
		from (
		
			select year("startDate") as "NamVaoLam", nv."empID",  nv."CostCenter", pb."Name" as  "PhongBan",
				   luong."BasicSal", "startDate",
				 
				 -- EffectiveDate: nếu Ngày vào làm <> khác năm đang tính lương, thì ngày này  = 01/01/NamTinhLuong
				 Case when ifnull("startDate",'') <> '' and  year("startDate") <>  :Nam then
				         Cast(cast(nam as char(4)) ||'-01'    ||'-01' as date) 
				      else -- năm vào làm = năm tính lương  --> Ngày vào làm
				         Cast("startDate" as date)
				   end as "EffectiveDate" 
				   	
				   	
			from APZ_OSAL luong
			inner join OHEM nv on luong."empID"  = nv."empID"
			inner join OUDP Pb on nv."dept"  = pb."Code"
	 		where "Period" in (select "PeriodCode"
												   from APZ_OHWP 
												   where "PYear"  = Nam  and  Right("PeriodCode",2)= Thang)  
												   
												  
			 ) as Nguon1 
		) as Nguon2  
		) as Nguon3
	  group by  "CostCenter","PhongBan";
------============================== End Lương tháng 13 tạm tính ==========================---------------



      If not exists (select "U_KyTinhLuong" from "@SALARYIRSTOSAP" where  "U_Thang" = Thang and "U_Nam"  =  Nam) then
       	select Cast(cast(nam as char(4)) ||'-'|| cast(thang as char(2)) ||'-17' as date) 
		into NgayThangTinhLuong
		from dummy;
      
		JE =
		select *
		from (
		
				select   "TongBHNV","TongBHCTY"  ,
				"ThueTNCN","LuongThucLinh",
				"CHIPHITHUONG" as  "CHIPHITHUONG",
				"CHIPHIOT" as  "CHIPHIOT",
				"CHIPHIPHUCAP" as  "CHIPHIPHUCAP",
				"DIEUCHINHTANG" as  "DIEUCHINHTANG",
				"DIEUCHINHGIAM" as  "DIEUCHINHGIAM",
				
				case when "LoaiBaoHiem" like  '%BHXH%' then "BH_NV" else 0 end as "BHXH_NV",
				case when "LoaiBaoHiem" like  '%BHXH%' then "BH_CTY" else 0 end as "BHXH_CTY",
						
				case when "LoaiBaoHiem" like  '%BHYT%' then "BH_NV" else 0 end as "BHYT_NV",
				case when "LoaiBaoHiem" like  '%BHYT%' then "BH_CTY" else 0 end as "BHYT_CTY",
						
						
				case when "LoaiBaoHiem" like  '%BHTN%' then "BH_NV" else 0 end as "BHTN_NV",
				case when "LoaiBaoHiem" like  '%BHTN%' then "BH_CTY" else 0 end as "BHTN_CTY",
						
						
				case when "LoaiBaoHiem" like  '%UnionFee%' then "BH_NV" else 0 end as "CongDoan_NV",
				case when "LoaiBaoHiem" like  '%UnionFee%' then "BH_CTY" else 0 end as "CongDoan_CTY",
						
						
						"PhongBan",
						"CostCenter",
						"U_TenNghiepVu",
						"U_TaiKhoanCP","U_TaiKhoanKPCD","U_TaiKhoanBHXH",
						"U_TaiKhoanBHYT","U_TaiKhoanBHTN","U_TaiKhoan334","U_TaiKhoanTTNCN",
						"U_TaiKhoanCPThuong",
						"U_TaiKhoanCPOT",
						"U_TaiKhoanCPPhuCap",
						"U_TaiKhoanCPThuongTam",
						
						
						Thang as "Thang",
						Nam as "Nam"
				 
				from
				(
						select Max("TongBHNV") as  "TongBHNV",
							   Max("TongBHCTY") as "TongBHCTY",
							   Max("ThueTNCN") as  "ThueTNCN",
							   Max("LuongThucLinh") as "LuongThucLinh",
							   Max("CHIPHITHUONG") as "CHIPHITHUONG",
							   Max("CHIPHIOT") as "CHIPHIOT",
							   Max("CHIPHIPHUCAP") as "CHIPHIPHUCAP",
							   "LoaiBaoHiem",
							
							   Max("EmpAmount") as "BH_NV",
							   Max("CompAmount") as "BH_CTY",
							   
							   Max("DIEUCHINHTANG") as "DIEUCHINHTANG",
							   Max("DIEUCHINHGIAM") as "DIEUCHINHGIAM",
							      "PhongBan",
							      "CostCenter"
						from (
						
								select "TotalDeduction" as "TongBHNV", 
										"U_Totalinsurance_ER"  as "TongBHCTY",
										"PersonalIncomeTax" as "ThueTNCN",
										"RealSalary" as "LuongThucLinh",
										luong."BonusTotal" as "CHIPHITHUONG",
										luong."TotalOT" as "CHIPHIOT",
										luong."U_TongPhuCap" as "CHIPHIPHUCAP",
										luong."U_salary_increase_adjust" as "DIEUCHINHTANG",
										luong."U_salary_reduc_adjust" as "DIEUCHINHGIAM",
								
								luong."empID", baohiem."Name" as "LoaiBaoHiem",baohiem."EmpAmount", baohiem."CompAmount"
									   ,pb."Name" as "PhongBan",
									   nv."CostCenter" as "CostCenter"
								from APZ_OSAL luong
								inner join OHEM nv on luong."empID"  = nv."empID"
								inner join OUDP Pb on nv."dept"  = pb."Code"
								left join (select "empID",  loaibh."Name", sum("EmpAmount") as "EmpAmount" ,  sum("CompAmount")   as "CompAmount"
											from APZ_SAL2 bh 
											inner join APZ_OHDT loaibh on bh."DeducCode"   = loaibh."Code"
											where "Period"  in (select "PeriodCode"
												   from APZ_OHWP 
												   where  "PYear"  = Nam  and  Right("PeriodCode",2)= Thang  )
											group by "empID",  loaibh."Name") baohiem on baohiem."empID" = luong."empID"
								
								where "Period" in (select "PeriodCode"
												   from APZ_OHWP 
												   where "PYear"  = Nam  and  Right("PeriodCode",2)= Thang  
												   
												   )
												   
								order by luong."empID"  
							) as Nguon1
						Group by "PhongBan","CostCenter", "LoaiBaoHiem"
						
						order by "PhongBan","CostCenter") as Nguon2 
						
						
						left join "@DINHKHOANLUONG" taikhoan on Nguon2."PhongBan"  =  taikhoan."U_PhongBan" and  Nguon2."CostCenter"  =  taikhoan."U_CostCenter"
						
						
						order by "PhongBan") as Nguon3;
						
						
--1-- N: 641/627 -- C:334: Chi Phí Lương
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,PhongBan,CostCenter, GhiChu)
		select distinct 1,"U_TenNghiepVu","U_TaiKhoanCP","LuongThucLinh",0,"PhongBan","CostCenter",'Chi phí lương'
		from :JE;
			
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,THUC_LINH,PhongBan,CostCenter,GhiChu)
		select distinct 1,"U_TenNghiepVu","U_TaiKhoan334",0,"LuongThucLinh","LuongThucLinh","CostCenter","PhongBan",'Chi phí lương'
		from :JE;
 
--2-- N: 642102/627102 -- C:3351: Chi Phí Thưởng TẠM TÍNH	

		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,PhongBan,CostCenter, GhiChu)
		select distinct 2,"U_TenNghiepVu","U_TaiKhoanCPThuongTam",b.TongThuongTam,0,"PhongBan","CostCenter",'Chi phí thưởng tạm'
		from :JE a
		left join #ThuongTamTinh b on a."PhongBan"  = b.PhongBan and a."CostCenter" = b.CostCenter;
			
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,CHIPHITHUONGTAM,PhongBan,CostCenter,GhiChu)
		select distinct 2,"U_TenNghiepVu",'3351',0,b.TongThuongTam as "Credit",b.TongThuongTam ,"PhongBan","CostCenter",'Chi phí thưởng tạm'
		from :JE a
		left join #ThuongTamTinh b on a."PhongBan"  = b.PhongBan and a."CostCenter" = b.CostCenter;
--3-- N: 642102/627102 -- C:334: Chi Phí Thưởng 	
		 
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,PhongBan,CostCenter, GhiChu)
		select distinct 3,"U_TenNghiepVu","U_TaiKhoanCPThuong","CHIPHITHUONG",0,"PhongBan","CostCenter",'Chi phí thưởng'
		from :JE;
			
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,CHIPHITHUONG,PhongBan,CostCenter,GhiChu)
		select distinct 3,"U_TenNghiepVu","U_TaiKhoan334",0,"CHIPHITHUONG" as "Credit","CHIPHITHUONG","PhongBan","CostCenter",'Chi phí thưởng'
		from :JE;
	 

--4-- N: 642103/627103 -- C:334: Chi Phí Tăng Ca - OT 	

		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,PhongBan,CostCenter, GhiChu)
		select distinct 4,"U_TenNghiepVu","U_TaiKhoanCPOT","CHIPHIOT",0,"PhongBan","CostCenter",'Chi phí tăng ca-OT'
		from :JE;
			
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,CHIPHIOT,PhongBan,CostCenter,GhiChu)
		select distinct 4,"U_TenNghiepVu","U_TaiKhoan334",0,"CHIPHIOT","CHIPHIOT","PhongBan","CostCenter",'Chi phí tăng ca-OT'
		from :JE;	
		
--5-- N: 642104/627104 -- C:334: Chi phí phụ cấp	

		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,PhongBan,CostCenter, GhiChu)
		select distinct 5,"U_TenNghiepVu","U_TaiKhoanCPPhuCap","CHIPHIPHUCAP",0,"PhongBan","CostCenter",'Chi phí phụ cấp'
		from :JE;
			
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,CHIPHIPHUCAP,PhongBan,CostCenter,GhiChu)
		select distinct 5,"U_TenNghiepVu","U_TaiKhoan334",0,"CHIPHIPHUCAP","CHIPHIPHUCAP","PhongBan","CostCenter",'Chi phí phụ cấp'
		from :JE;		
		
		 

--6 Doanh nghiệp đóng BHXH cho người lao động
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,PhongBan,CostCenter, GhiChu)
		select distinct 6,"U_TenNghiepVu","U_TaiKhoanCP","TongBHCTY",0,"PhongBan","CostCenter",'Doanh nghiệp đóng BHXH cho người lao động'
		from :JE;
			
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,KPCD_CTY,PhongBan,CostCenter, GhiChu)
		select distinct 6,"U_TenNghiepVu","U_TaiKhoanKPCD",0,"CongDoan_CTY","CongDoan_CTY","PhongBan","CostCenter",'Doanh nghiệp đóng BHXH cho người lao động'
		from :JE;
		 			   
		 			   
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,BHXH_CTY,PhongBan,CostCenter, GhiChu)
		select distinct 6,"U_TenNghiepVu","U_TaiKhoanBHXH",0,"BHXH_CTY","BHXH_CTY","PhongBan","CostCenter",'Doanh nghiệp đóng BHXH cho người lao động'
		from :JE;
		 						   
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,BHYT_CTY,PhongBan,CostCenter, GhiChu)
		select distinct 6,"U_TenNghiepVu","U_TaiKhoanBHYT",0,"BHYT_CTY","BHYT_CTY","PhongBan","CostCenter",'Doanh nghiệp đóng BHXH cho người lao động'
		from :JE;
		 
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,BHTN_CTY,PhongBan,CostCenter, GhiChu)
		select distinct 6,"U_TenNghiepVu","U_TaiKhoanBHTN",0,"BHTN_CTY","BHTN_CTY","PhongBan","CostCenter",'Doanh nghiệp đóng BHXH cho người lao động'
		from :JE;
		
--7 Người lao động trích đóng BHXH
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,PhongBan,CostCenter, GhiChu)
		select distinct 7,"U_TenNghiepVu","U_TaiKhoan334","TongBHNV",0,"PhongBan","CostCenter",'Người lao động trích đóng BH'
		from :JE;
		
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,BHXH_NLD,PhongBan,CostCenter, GhiChu)
		select distinct 7,"U_TenNghiepVu","U_TaiKhoanBHXH",0,"BHXH_NV","BHXH_NV","PhongBan","CostCenter",'Người lao động trích đóng BHXH'
		from :JE;
		
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,BHYT_NLD,PhongBan,CostCenter, GhiChu)
		select distinct 7,"U_TenNghiepVu","U_TaiKhoanBHYT",0,"BHYT_NV","BHYT_NV","PhongBan","CostCenter",'Người lao động trích đóng BHXH'
		from :JE;
		
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,BHTN_NLD ,PhongBan,CostCenter, GhiChu)
		select distinct 7,"U_TenNghiepVu","U_TaiKhoanBHTN",0,"BHTN_NV","BHTN_NV","PhongBan","CostCenter",'Người lao động trích đóng BHXH'
		from :JE;	
/*---- Người Lao Động: Trích phí công đoàn 1%
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit, PhongBan, GhiChu)
		select distinct 5,"U_TenNghiepVu","U_TaiKhoan334","CongDoan_NV",0,"PhongBan",'Công đoàn phí người lao động'
		from :JE;
		
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,KPCD_NLD ,PhongBan,CostCenter, GhiChu)
		select distinct 6,"U_TenNghiepVu","U_TaiKhoanKPCD",0 as  "Debit","CongDoan_NV","CongDoan_NV","PhongBan","CostCenter",'Người lao động trích đóng BHXH'  
		from :JE; */	
			
--8-- N:334 - C: 3335: Thuế TNCN
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,PhongBan,CostCenter, GhiChu)
		select distinct 8,"U_TenNghiepVu","U_TaiKhoan334","ThueTNCN",0,"PhongBan","CostCenter",'Thuế TNCN'
		from :JE; 
			
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,TNCN_NLD,PhongBan,CostCenter, GhiChu)
		select distinct 8,"U_TenNghiepVu","U_TaiKhoanTTNCN",0,"ThueTNCN","ThueTNCN","PhongBan","CostCenter",'Thuế TNCN'
		from :JE; 

--9-- N: 641/627 -- C:334: Điều chỉnh tăng sau thuế
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,PhongBan,CostCenter, GhiChu)
		select distinct 9,"U_TenNghiepVu","U_TaiKhoanCP","DIEUCHINHTANG",0,"PhongBan","CostCenter",'Điều chỉnh tăng sau thuế'
		from :JE;
			
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,DIEUCHINHTANG,PhongBan,CostCenter,GhiChu)
		select distinct 9,"U_TenNghiepVu","U_TaiKhoan334",0,"DIEUCHINHTANG","DIEUCHINHTANG","CostCenter","PhongBan",'Điều chỉnh tăng sau thuế'
		from :JE;
--10-- N: 641/627 -- C:334: Điều chỉnh giảm sau thuế
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,PhongBan,CostCenter, GhiChu)
		select distinct 10,"U_TenNghiepVu","U_TaiKhoan334","DIEUCHINHGIAM",0,"PhongBan","CostCenter",'Điều chỉnh tăng sau thuế'
		from :JE;
			
		insert into #tbl (NhomButToan,TenTaiKhoanCP,TaiKhoan,Debit,Credit,DIEUCHINHTANG,PhongBan,CostCenter,GhiChu)
		select distinct 10,"U_TenNghiepVu","U_TaiKhoanCP",0 as "Debit","DIEUCHINHGIAM","DIEUCHINHGIAM","CostCenter","PhongBan",'Điều chỉnh tăng sau thuế'
		from :JE;
		
---
		delete 
		from #tbl
		where Debit = 0 and Credit  = 0;
--
		update #tbl
		set Thang = :Thang, Nam  = :Nam, UserCode  = :UserCode, 
		  UserID = ( select top 1 "USERID" from OUSR where "USER_CODE"  = :UserCode);
		 
		 
		 update a
		 set Debit  = Debit+ifnull((select sum(KPCD_CTY) from  #tbl b where a.PhongBan = b.PhongBan  ),0)
		 from #tbl a
		 where TaiKhoan like '6%' and NhomButToan = 6 ;  
		 
		 update a
		 set Debit  = Debit+ifnull((select sum(KPCD_NLD) from  #tbl b where a.PhongBan = b.PhongBan  ),0)
		 from #tbl a
		 where TaiKhoan like '334%' and NhomButToan = 7 ;  
		 
		   
		
		select  row_number() over (order by Thang,NhomButToan, PhongBan asc) as "STT",
				NhomButToan,TaiKhoan,Debit,Credit,
				CHIPHITHUONG,
				CHIPHITHUONGTAM,
				CHIPHIOT,
				CHIPHIPHUCAP,
				DIEUCHINHTANG,
				DIEUCHINHGIAM,
				BHXH_NLD,BHYT_NLD,BHTN_NLD,KPCD_NLD,TNCN_NLD,THUC_LINH,
		BHXH_CTY,BHYT_CTY,BHTNNLD_CTY,BHTN_CTY,KPCD_CTY,PhongBan, CostCenter,
				GhiChu ,Thang,Nam,  UserID, UserCode
		from #tbl
		order by  STT;
		 
		
		drop table #tbl;
		drop table #ThuongTamTinh;
	End If;
END IF;
 

end;

