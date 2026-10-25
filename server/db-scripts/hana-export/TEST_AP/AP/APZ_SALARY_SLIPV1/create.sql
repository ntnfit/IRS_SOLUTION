
CREATE PROCEDURE APZ_SALARY_SLIPV1(in IDNV NVARCHAR(40000), in period nvarchar(100), in branch INT)
AS
BEGIN
	declare thang int;
	declare nam int;
	declare empId nvarchar(40000);
	select IFNULL((select month(x."FromDate") from APZ_OHWP x where x."PeriodCode"=:period),0) into thang from DUMMY;
	select IFNULL((select year(x."ToDate") from APZ_OHWP x where x."PeriodCode"=:period),0) into nam from DUMMY;

	create local temporary table #tbl (
		"TieuDe"				NVARCHAR(300)
		,"ThangNam"				NVARCHAR(200)
		,"IDNV"					NVARCHAR(100)
		,"MaNV"					NVARCHAR(100)
		,"TenNV"				NVARCHAR(300)
		,"MaPB"					NVARCHAR(100)
		,"TenPB"				NVARCHAR(500)
		,"DonViNP"				NVARCHAR(100)
		,"DonViNB"				NVARCHAR(100)
		,"DonViTien"			NVARCHAR(10)
		,"SoNgayCong"			FLOAT
		,"PhepLKTT"				FLOAT -- Lũy kế đến cuối tháng trước
		,"PhepThang"			FLOAT -- Được nghỉ phép tháng này
		,"SoNNPhepThang"		FLOAT -- số ngày nghỉ phép tháng này
		,"PhepLKCT"				FLOAT -- Lũy kế đến cuối tháng này
		,"NghiBHXH"				FLOAT -- Nghỉ hưởng chế độ BHXH
		,"NghiLe"				FLOAT -- Nghỉ lễ
		,"NBLKTT"				FLOAT -- Nghỉ bù lũy kế đến cuối tháng trước
		,"NBThang"				FLOAT -- Được nghỉ bù tháng này
		,"SoGioNBThang"			FLOAT -- Số giờ nghỉ bù tháng này
		,"NBLKCT"				FLOAT -- Nghỉ bù lũy kế đến cuối tháng này
		,"NKhongLuong"			FLOAT -- Nghỉ không hưởng lương
		,"OT150"				FLOAT -- Thêm giờ ngày thường (150%)
		,"OT200"				FLOAT -- Thêm giờ ngày chủ nhật (200%)
		,"OT300"				FLOAT -- Thêm giờ ngày lễ (300%)
		,"LuongBHXH"			numeric(19,6) -- Mức lương tham gia BHXH
		,"LuongTT"				numeric(19,6) -- Mức lương thỏa thuận
		,"LuongTG"				numeric(19,6) -- Lương thời gian
		,"LuongOT"				numeric(19,6) -- Lương làm thêm giờ
		,"LuongCong"			numeric(19,6) -- Các khoản cộng lương
		,"LuongTru"				numeric(19,6) -- Các khoản trừ lương
		,"TongLuong"			numeric(19,6) -- Tổng lương
		,"TamUngLuong"			numeric(19,6) -- Tạm ứng lương
		,"BHXH"					numeric(19,6) -- BHXH (8%)
		,"BHYT"					numeric(19,6) -- BHYT (1.5%)
		,"BHTN"					numeric(19,6) -- BHTN (1%)
		,"ThueTNCN"				numeric(19,6) -- Thuế TNCN (1%)
		,"TongKT"				numeric(19,6) -- Tổng các khoản phải khấu trừ vào lương
		,"ThucLinh"				numeric(19,6) -- Thực lĩnh = Tổng lương - Tạm ứng lương - Tổng các khoản phải khấu trừ vào lương
	);
	
	INSERT INTO #tbl (IDNV, "MaNV", "TenNV", "MaPB", "TenPB", "SoNgayCong", "LuongBHXH", "LuongTT", "LuongTG", "LuongOT", "LuongCong", "LuongTru", "TamUngLuong", "ThueTNCN", "ThucLinh") 
		SELECT "empID", IFNULL("ExtEmpNo", cast("empID" as nvarchar)), "firstName" || IFNULL(' ' || "middleName", '') || IFNULL(' ' || "lastName", ''), "dept", IFNULL("Name", ''), SUM(IFNULL(T."WorkTotal", 0)), "InsuranceSal", "BasicSal"
		, "SalTotal", SUM(IFNULL("TotalOT", 0)), SUM(IFNULL("BonusTotal", 0)), SUM(IFNULL("PenalTotal", 0)), SUM(IFNULL("AdvanceAmount", 0)), SUM(IFNULL("PersonalIncomeTax", 0)), SUM(IFNULL("NetSalary", 0))
			from 
			(SELECT a."ExtEmpNo", a."empID", a."firstName", a."middleName", a."lastName", a."dept", IFNULL(b."Name", '') AS "Name", IFNULL(c."WorkTotal", 0) AS "WorkTotal"
					, LAST_VALUE(c."InsuranceSal") OVER (PARTITION BY c."empID" ORDER BY c."CalFrom") AS "InsuranceSal"
					, LAST_VALUE(c."BasicSal") OVER (PARTITION BY c."empID" ORDER BY c."CalFrom") AS "BasicSal"
					, LAST_VALUE(c."SalTotal") OVER (PARTITION BY c."empID" ORDER BY c."CalFrom") AS "SalTotal"
				, IFNULL(c."TotalOT", 0) AS "TotalOT", IFNULL(c."BonusTotal", 0) AS "BonusTotal", IFNULL(c."PenalTotal", 0) AS "PenalTotal", IFNULL(c."AdvanceAmount", 0) AS "AdvanceAmount"
				, IFNULL(c."PersonalIncomeTax", 0) AS "PersonalIncomeTax", IFNULL(c."NetSalary", 0) AS "NetSalary" 
			FROM OHEM a LEFT OUTER JOIN OUDP b ON a."dept" = b."Code" INNER JOIN APZ_OSAL c ON c."empID" = a."empID" AND c."Period" = :period AND c."branch" = :branch
			WHERE 
				(a."empID" IN (SELECT data FROM Split(:IDNV,',')) OR :IDNV='')
				AND a."Active"='Y' AND a."branch"=:branch ) as T 
			 GROUP BY "empID","ExtEmpNo","firstName","lastName","middleName","dept","Name","BasicSal","InsuranceSal","SalTotal";

	-- Nghi phep
	UPDATE a SET a."PhepThang" = IFNULL(b."IncreaseLeaveTotal", 0) + IFNULL(b."FPLTotal", 0) FROM #tbl a, (select top 1 "IncreaseLeaveTotal", "FPLTotal" from APZ_OHWP WHERE "PeriodCode" = :period AND "Branch" = :branch) b;
	UPDATE #tbl SET "PhepLKTT" = IFNULL((SELECT IFNULL(x."LeaveTotal", 0) - "PhepThang" FROM APZ_TSL2 x WHERE x."EmpId" = #tbl.IDNV AND x."Period" = :period), 0);
	UPDATE #tbl SET "SoNNPhepThang" = IFNULL((SELECT IFNULL(x."Used", 0) FROM APZ_TSL2 x WHERE x."EmpId" = #tbl.IDNV AND x."Period" = :period), 0);
	UPDATE #tbl SET "PhepLKCT" = CASE WHEN ("PhepLKTT" + "PhepThang" - "SoNNPhepThang") > 0 THEN ("PhepLKTT" + "PhepThang" - "SoNNPhepThang") ELSE 0 END;
	--update @tbl set PhepLKCT=isnull(PhepLKTT)
	-- Tinh OT
	UPDATE #tbl SET OT150 = IFNULL((SELECT SUM(x."OTWorkTotal") FROM APZ_SAL3 x WHERE x."empID" = #tbl.IDNV AND x."Period" = :period AND "OTCode" = 'OT150'), 0)
					, OT200 = IFNULL((SELECT SUM(x."OTWorkTotal") FROM APZ_SAL3 x WHERE x."empID" = #tbl.IDNV AND x."Period" = :period AND "OTCode" = 'OT200'), 0)
					, OT300 = IFNULL((SELECT SUM(x."OTWorkTotal") FROM APZ_SAL3 x WHERE x."empID" = #tbl.IDNV AND x."Period" = :period AND "OTCode" = 'OT300'), 0);

	-- Nghi bu
	--update @tbl set NBLKTT = isnull((select isnull(x.CompLeaveTotal,0) from APZ_TSL3 x where x.EmpId=[@tbl].IDNV and x.Period=@period),0)
	--update @tbl set NBThang = isnull((select isnull(x.OverTimeTotal,0) from APZ_TSL3 x where x.EmpId=[@tbl].IDNV and x.Period=@period),0)
	--update @tbl set SoGioNBThang = isnull((select isnull(x.Used,0) from APZ_TSL3 x where x.EmpId=[@tbl].IDNV and x.Period=@period),0)
	--update @tbl set NBLKCT = CASE WHEN (NBLKTT+  NBThang - SoGioNBThang)>0 THEN (NBLKTT+  NBThang - SoGioNBThang) else 0 end

	--Nghi BHXH
	UPDATE #tbl SET "NghiBHXH" = IFNULL((SELECT SUM(IFNULL(x."WorkTotal", 0)) FROM APZ_TSL1 x INNER JOIN APZ_OHWC y ON x."WorkCode" = y."Code" WHERE x."EmpId" = #tbl.IDNV AND x."WorkCode" = 'CD' AND x."Period" = :period AND y."Branch" = :branch), 0);
	
	--Nghi le
	UPDATE #tbl SET "NghiLe" = IFNULL((SELECT SUM(IFNULL(x."WorkTotal", 0)) FROM APZ_TSL1 x 
		INNER JOIN APZ_OHWC y ON x."WorkCode" = y."Code" WHERE x."EmpId" = #tbl.IDNV AND (x."WorkCode" = 'NL' OR x."WorkCode" = 'NL\2') AND x."Period" = :period AND y."Branch" = :branch), 0);


	--Nghi khong luong
	UPDATE #tbl SET "NKhongLuong" = IFNULL((SELECT SUM(IFNULL(x."WorkTotal", 0)) FROM APZ_TSL1 x INNER JOIN APZ_OHWC y ON x."WorkCode" = y."Code" INNER JOIN APZ_OHWT z ON y."WorkType" = z."Id" 
					WHERE x."EmpId" = #tbl.IDNV AND z."Typ" = 'A' AND x."Period" = :period AND y."Branch" = :branch), 0);

	-- Tinh tong tien luong
	UPDATE #tbl SET "TongLuong" = IFNULL("LuongTG", 0) + IFNULL("LuongOT", 0) + IFNULL("LuongCong", 0) - IFNULL("LuongTru", 0);
	-- Cac khoan phai khau tru vao luong
	UPDATE #tbl SET BHXH = IFNULL((SELECT SUM(x."EmpAmount") FROM APZ_SAL2 x WHERE x."empID" = #tbl.IDNV AND x."Period" = :period AND x."DeducCode" = 1), 0)
					, BHYT = IFNULL((SELECT SUM(x."EmpAmount") FROM APZ_SAL2 x WHERE x."empID" = #tbl.IDNV AND x."Period" = :period AND x."DeducCode" = 2), 0)
					, BHTN = IFNULL((SELECT SUM(x."EmpAmount") FROM APZ_SAL2 x WHERE x."empID" = #tbl.IDNV AND x."Period" = :period AND x."DeducCode" = 3), 0);
	UPDATE #tbl SET "TongKT" = IFNULL(BHXH, 0) + IFNULL(BHYT, 0) + IFNULL(BHTN, 0) + IFNULL("ThueTNCN", 0);

	UPDATE #tbl SET "TieuDe" = n'PHIẾU LƯƠNG', "ThangNam" = n'Tháng ' || RIGHT('00' || CAST(:thang AS nvarchar), 2) || n' năm ' || CAST(:nam AS nvarchar), "DonViNP" = n'ngày', "DonViNB" = n'giờ', "DonViTien" = 'VND';

	SELECT * FROM #tbl;
      
END




