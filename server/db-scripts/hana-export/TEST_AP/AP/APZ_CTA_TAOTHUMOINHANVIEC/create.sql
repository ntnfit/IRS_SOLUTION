CREATE procedure APZ_CTA_TaoThuMoiNhanViec(in clgCode nvarchar(10), in DocEntry nvarchar(10), in LineNum nvarchar(10))

as
BEGIN
 
	declare NgayThangNam nvarchar(50);
	declare NgayHienTai nvarchar(50);
	declare HovaTen nvarchar(100);
	declare HovaTenTA nvarchar(100);
	declare DienThoai nvarchar(100);
	declare Email nvarchar(100);

	declare NgayNhanViec nvarchar(50);
	declare DenNgay nvarchar(50);
	
	declare ChucDanh nvarchar(100);
	declare ChucDanhTA nvarchar(100);
	
	declare LuongThuViec float;
	declare LuongThuViecText nvarchar(100);
	
	declare LuongChinhThuc  float;
	declare LuongChinhThucText nvarchar(100);
	
	declare ThoiGianLamViec nvarchar(150);
	declare ThoiGianLamViecTA nvarchar(150);
	
	declare PhuongTienDiLam nvarchar(150);
	declare PhuongTienDiLamTA nvarchar(150);
	
	 
	
if exists(select 1 from APZ_HJV5 a where cast(a."DocEntry" as nvarchar) =  :DocEntry
		 	and cast(a."LineNum" as nvarchar)  = :LineNum)
	 
THEN

Select   'Ngày ' || TO_VARCHAR(CURRENT_DATE,'DD') ||  ' Tháng ' ||   TO_VARCHAR(CURRENT_DATE,'MM') || ' Năm ' ||  TO_VARCHAR(CURRENT_DATE,'YYYY') as  "NgayThangNam",
		 TO_VARCHAR(CURRENT_DATE,'DD/MM/YYYY')   as "NgayHienTai",

		a."ApplicantName" as "HovaTen",
		a."ApplicantName" as "HovaTenTA",-- Hoac la khong dau
		b."Phone" as "DienThoai",
		b."Email" as  "Email",
		"U_StartDate" as "NgayNhanViec",
		"U_ToDate" as  "DenNgay",
		"U_JobTitle" as "ChucDanh",
		"U_JobTitleEN" as "ChucDanhTA",
		
		"TrialPeriodSalary"  as  "LuongThuViec",
		Num2Text( cast ( "TrialPeriodSalary" as float) ) as "LuongThuViecText",

		"ContractSalary" as  "LuongChinhThuc",
		Num2Text(cast("ContractSalary" as float)) as "LuongChinhThucText",

-- Thoi Gian Lam viec
		case when a."U_WorkingHours" =  40 then N'40 giờ/tuần (08 giờ/ngày), từ Thứ Hai đến Thứ Sáu'
			 when a."U_WorkingHours" =  44 then N'44 giờ/tuần (08 giờ/ngày), từ Thứ Hai đến hết Sáng Thứ Bảy'
			 when a."U_WorkingHours" =  48 then N'48 giờ/tuần (08 giờ/ngày), từ Thứ Hai đến Thứ Bảy'
				  else '' end as "ThoiGianLamViec",
				  
				  
		case when a."U_WorkingHours" =  40 then N'40 hours/week (08 hours/day), from Monday to Friday'
			 when a."U_WorkingHours" =  44 then N'44 hours/week (08 hours/day), from Monday to Saturday Morning'
			 when a."U_WorkingHours" =  48 then N'48 hours/week (08 hours/day), from Monday to Saturday'
				  else '' end as "ThoiGianLamViecTA"	  
				  
-- Phuong tien di lam		 
		,case when ifnull(a."U_TransTowork",'') =  '' then '' 
							 when a."U_TransTowork" = '0' then N'Công ty sẽ cung cấp phương tiện đi lại cho nhân viên khi làm việc tại nhà máy KCN Protrade'
							 when a."U_TransTowork" = '1' then N'Nhân viên tự túc phương tiện đi lại khi làm việc tại KCN Protrade'	 end 
							 as "PhuongTienDiLam",
							 
						-- Phuong tien di lam tieng Anh
						case when ifnull(a."U_TransTowork",'') =  '' then '' 
							 when a."U_TransTowork" = '0' then N'The Company will provide means of transportation for employee when working Ho Chi Minh office or other locations for business purposes'
							 when a."U_TransTowork" = '1' then N'Employee self travel when working at working at Factory in Protrade Industrial Zone'	 end 
							   as "PhuongTienDiLamTA"

	
	
Into NgayThangNam,NgayHienTai,HovaTen,HovaTenTA,DienThoai,Email,NgayNhanViec,DenNgay, ChucDanh,ChucDanhTA,LuongThuViec,LuongThuViecText,LuongChinhThuc, LuongChinhThucText,
		ThoiGianLamViec,ThoiGianLamViecTA,
		PhuongTienDiLam,PhuongTienDiLamTA
	

From APZ_HJV5 a
--left join (select  distinct    "Phone",  "Email" ,"ApplicantNo"     from   APZ_HJV3    ) as b   on a."ApplicantNo"  = b."ApplicantNo"
left join (select  distinct    "Phone",  "Email" ,"ApplicantNo"     from   APZ_OHAP    ) as b   on a."ApplicantNo"  = b."ApplicantNo"

Where cast(a."DocEntry" as nvarchar) =  :DocEntry
			 and cast(a."LineNum" as nvarchar)  = :LineNum; 
			
END IF;
	select 
	 "Details", "CardCode", "Action", "Location", "status", "personal", "parentType", "parentId", "SeriesNum", "parentTask", "PrjCode", "OcrCode", "OcrCode2", "OcrCode3", "OcrCode4", "OcrCode5", "RecurrenceInfo"
		 	,"ResourceIdsInfo", "ProjectEntry", "ProjectWBS", "BaseType", "BaseEntry", "BaseLine", "BaseCode" ,"DocEntry",
		 	2 as "LineNum",
			 
	
			replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace("HtmlNotes", 
					  '[%0]', ifnull(:NgayThangNam, ''))
					, '[%1]', ifnull(:HovaTen, ''))
					, '[%2]', ifnull(:HovaTenTA, ''))
					, '[%3]', ifnull(:DienThoai, ''))
					, '[%4]', ifnull(:Email, ''))
					, '[%5]', ifnull(TO_VARCHAR(:NgayNhanViec,'DD/MM/YYYY'), ''))
					, '[%6]', ifnull(TO_VARCHAR(:DenNgay,'DD/MM/YYYY'), ''))
					--, '[%6]', ifnull(:DenNgay, ''))
					, '[%7]', ifnull(:ChucDanh, ''))
					, '[%8]', ifnull(:ChucDanhTA, ''))	
					, '[%9]', ifnull(TO_VARCHAR(:LuongThuViec,'9,999'), ''))
					, '[%10]', ifnull(:LuongThuViecText, ''))
					
					--, '[%11]', ifnull(:LuongChinhThuc, 0))
					, '[%11]', ifnull(TO_VARCHAR(:LuongChinhThuc,'9,999'), ''))
					, '[%12]', ifnull(:LuongChinhThucText, '')) 
					
					, '[%13]', ifnull(:ThoiGianLamViec, ''))
					, '[%14]', ifnull(:ThoiGianLamViecTA, '')) 
					
					, '[%15]', ifnull(:PhuongTienDiLam, '')) 
					, '[%16]', ifnull(:PhuongTienDiLamTA, '')) 
					, '[%17]', ifnull(:NgayHienTai, '')) 
					, '[%18]', ifnull(TO_VARCHAR(ADD_DAYS(ifnull(:NgayNhanViec, CURRENT_DATE),60),'DD/MM/YYYY'), ''))
					, '&',  '&amp;')



					"HtmlNotes" 
	from APZ_OCLG where cast("ClgCode" as nvarchar) = :clgCode;

END

