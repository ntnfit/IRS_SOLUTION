CREATE PROCEDURE APZ_CTA_TaoPhieuLuong(
		in period					nvarchar(100),
		in empID					nvarchar(4000)
		--in branch					int DEFAULT  -2
)
AS
BEGIN
	
	declare fdate, tdate timestamp;
	select "FromDate","ToDate" into fdate,tdate from APZ_OHWP where "PeriodCode"=:period;
	
	create local temporary table #tbl(
		TenCT						nvarchar(254),
		TieuDeRPT					nvarchar(50),
		TenBC						nvarchar(100),
		Thang						int,
		Nam							int,
		STT							int,
		
		empID						nvarchar(20),
		MaNhanVien					nvarchar(50),				
		TenNhanVien					nvarchar(50),
		ChucDanh					nvarchar(100),
		NgayVaoLam					nvarchar(50),
		MaSoThue					nvarchar(50),
		SoNguoiPhuThuoc				int,
		NgayCongChuan				numeric(21,6),
		NgayCongThucTe				numeric(21,6),
		NghiPhepNam					numeric(21,6),
		NghiKhongLuong				numeric(21,6),
		
		A_LuongCoBan				numeric(21,6),
		
		B_TongThuNhap				numeric(21,6),
		B_LuongThucTe				numeric(21,6),
		B_NghiPhepNam				numeric(21,6),
		B_PhuCapAn					numeric(21,6),
		B_PhuCapXangDau				numeric(21,6),
		B_PhuCapDienThoai			numeric(21,6),
		B_PhuCapDauXe				numeric(21,6),
		
		C_PhucLoiChiuThueTNCN		numeric(21,6),
		D_ThuNhapMienThue			numeric(21,6),
		D_ThuNhapMienThue_An		numeric(21,6),
		
		E_ThuNhapChiuThue			numeric(21,6),
		
		F_GiamTruocKhiTinhThue		numeric(21,6),
		F_GiamTruBanThan			numeric(21,6),
		F_GiamTruNguoiPhuThuoc		numeric(21,6),
		F_TongBaoHiem				numeric(21,6),
		F_TongBaoHiemDieuChinh		numeric(21,6),			
		
		G_ThuNhapTinhThue			numeric(21,6),
		H_ThueThuNhap				numeric(21,6),
		I_CacKhoanCongTruSauThue	numeric(21,6),
		J_DoanPhi					numeric(21,6),
		K_ThucLanh					numeric(21,6)
	);
	
insert into #tbl (	empID,MaNhanVien,TenNhanVien,ChucDanh,NgayVaoLam,MaSoThue,SoNguoiPhuThuoc,NgayCongChuan,
					NgayCongThucTe,NghiPhepNam,NghiKhongLuong,
					A_LuongCoBan,
					
					B_TongThuNhap,B_LuongThucTe,B_NghiPhepNam,
					B_PhuCapAn,B_PhuCapXangDau,B_PhuCapDienThoai,B_PhuCapDauXe,
					
					C_PhucLoiChiuThueTNCN,
					D_ThuNhapMienThue,D_ThuNhapMienThue_An,
					E_ThuNhapChiuThue,
					
					F_GiamTruocKhiTinhThue,F_GiamTruBanThan,F_GiamTruNguoiPhuThuoc,F_TongBaoHiem,F_TongBaoHiemDieuChinh,
					
					G_ThuNhapTinhThue,H_ThueThuNhap,I_CacKhoanCongTruSauThue,J_DoanPhi,	K_ThucLanh)

select 
					a."empID" as "empID",
					b."ExtEmpNo" as "MaNhanVien",
					IFNULL(b."lastName",'') || IFNULL(' ' || b."middleName",'') || IFNULL(' ' || b."firstName",'') as "TenNhanVien",
					b."jobTitle" as "ChucDanh",
					case when ifnull(b."startDate",'') = '' then '' else TO_VARCHAR(b."startDate",'DD/MM/YYYY') end as  "NgayVaoLam",
					b."pager" as "MaSoThue",
					0 as "SoNguoiPhuThuoc",
					ifnull("WorkQuota",0) as "NgayCongChuan",
					ifnull("WorkTotal",0)  as "NgayCongThucTe",
					ifnull(cast("LeaveSum" as decimal(21,6)),0)  as "NghiPhepNam",
					
					ifnull("AbsentSum",0) as "NghiKhongLuong",
			
					ifnull("BasicSal",0) as "LuongCoBan",
					0 as "TongThuNhap",
					ifnull("RealSalary",0) as "LuongThucTe",
					0 as "B_NghiPhepNam",
					ifnull(cast("U_AlowQta_3" as decimal(21,6)),0) as "PhuCapAn",
					ifnull(cast("U_AlowQta_2" as decimal(21,6)),0) as "PhuCapXangDau",
					0 as "PhuCapDienThoai",
					0 as "PhuCapDauXe",
					
					0 as "PhucLoiChiuThueTNCN",
					0 as "ThuNhapMienThue",
					ifnull(cast("U_ExemptMealAllow" as decimal (21,6)),0) as "ThuNhapMienThue_An",
					
					 ifnull("TaxIncAfterFamDeduc",0) as "ThuNhapChiuThue",
					
					--F
					0 as "F_GiamTruocKhiTinhThue",
					ifnull("PITSAL",0) as "GiamTruBanThan",
					ifnull("RelatedCalculation",0) as "GiamTruNguoiPhuThuoc",
					ifnull("TotalDeduction",0) as "TongBaoHiem",
					0 as "TongBaoHiemDieuChinh",
					
					ifnull("TaxIncAfterFamDeduc",0) as "ThuNhapTinhThue",
					ifnull("PersonalIncomeTax",0) as "ThueThuNhap",
					0 as "CacKhoanCongTruSauThue",
					ifnull(cast("U_EmpDeducAmt_5" as decimal(21,6)),0) as "DoanPhi",
					ifnull("TotalSalAfterTax",0) as "ThucLanh"					


from  
		APZ_OSAL a
		left join OHEM b on b."empID" = a."empID"

where
		a."Period" = :period 
		 
		and (IFNULL(NULLIF(:empID,''),'')='' or exists(select 1 from Split(cast(:empID as nvarchar),',') where Data = a."empID"));
/*

	--update x set KPIType = y."U_KPILevel",KPIRate = y."U_Rate" from #tbl x inner join "@KPI_D1" y on y."U_EmpNo"=x.MaSo inner join "@KPI_H" z on z."Code"=y."Code" and z."U_ToDate" between :fdate and :tdate;
	
	--update #tbl set ThuongHQCV = IFNULL((select sum(x."U_ThuongKPI") from APZ_OHSI x where x."EmpId"=#tbl.EmpID and :fdate between x."ValidFrom" and x."ValidTo" and :tdate between x."ValidFrom" and x."ValidTo"),0);
	
	--update #tbl set ThuongHQCV = ROUND(ThuongHQCV*KPIRate,0);

/*	
	update #tbl set OT200 = IFNULL((select sum(x."OTWorkTotal") from APZ_SAL3 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."OTCode" = 'OT.CN'),0)
					,OT200Amt = IFNULL((select sum(x."OTAmount") from APZ_SAL3 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."OTCode" = 'OT.CN'),0)
					,OT210 = IFNULL((select sum(x."OTWorkTotal") from APZ_SAL3 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."OTCode" = 'OT.Ca3'),0)
					,OT210Amt = IFNULL((select sum(x."OTAmount") from APZ_SAL3 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."OTCode" = 'OT.Ca3'),0)
					,OT295 = IFNULL((select sum(x."OTWorkTotal") from APZ_SAL3 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."OTCode" = 'OT.CNCa3'),0)
					,OT295Amt = IFNULL((select sum(x."OTAmount") from APZ_SAL3 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."OTCode" = 'OT.CNCa3'),0)
					,OT300 = IFNULL((select sum(x."OTWorkTotal") from APZ_SAL3 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."OTCode" = 'OT.Holiday'),0)
					,OT300Amt = IFNULL((select sum(x."OTAmount") from APZ_SAL3 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."OTCode" = 'OT.Holiday'),0)
					,OT390 = IFNULL((select sum(x."OTWorkTotal") from APZ_SAL3 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."OTCode" = 'OT.HoliNi'),0)
					,OT390Amt = IFNULL((select sum(x."OTAmount") from APZ_SAL3 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."OTCode" = 'OT.HoliNi'),0);
	*/
	
	/*
	update #tbl set BHXH = IFNULL((select sum(x."EmpAmount") from APZ_SAL2 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."DeducCode" = 1),0)
					,BHYT = IFNULL((select sum(x."EmpAmount") from APZ_SAL2 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."DeducCode" = 2),0)
					,BHTN = IFNULL((select sum(x."EmpAmount") from APZ_SAL2 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."DeducCode" = 3),0)
					,CongDoan = IFNULL((select sum(x."EmpAmount") from APZ_SAL2 x where x."Period" = :period and x."empID" = #tbl.EmpID and x."DeducCode" = 4),0);
	*/
 	update #tbl set TenCT = a."CompnyName", TieuDeRPT  = N'SALARY DESCRIPTION/DIỄN GIẢI' ,
	
	
	TenBC = 'PAYSLIP/PHIẾU LƯƠNG THÁNG', Thang = month(:fdate), Nam = year(:fdate) from #tbl,OADM a;
	
	 
	select * from #tbl;
	drop table #tbl;
	
END


