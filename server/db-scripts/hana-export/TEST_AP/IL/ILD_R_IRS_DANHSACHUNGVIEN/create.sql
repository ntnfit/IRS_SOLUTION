create procedure ILD_R_IRS_DanhSachUngVien
(
	in FDate	date,
	in TDate	date,
	in StartDate date, -- Ngày nhận việc
	in FoodHealthCheck nvarchar(5000)
)
as
Begin

create local temporary table #tbl (
		DocEntry int,
		LineNum int,
		RecruitmentFromDate nvarchar(50),-- ngày bắt đầu đợt tuyển dụng
		RecruitmentToDate nvarchar(100), -- ngày kết thúc đợt tuyển dụng
		Description nvarchar(100), -- diễn giải chiến dịch tuyển dụng

		ApplicantNo nvarchar(50), -- Mã ứng viên
		ApplicantName nvarchar(100),
		GioiTinh nvarchar(10),
		Address nvarchar(150),
		Phone nvarchar(50),
		Email nvarchar(100),
		
		LuongThuViec Float,
		LuongChinhThuc float,
		NgayNhanViec nvarchar(100),
		ChucDanh nvarchar(100),
		KiemTraSucKhoeThucPham nvarchar(100)
		);


insert into #tbl(DocEntry,LineNum,RecruitmentFromDate,RecruitmentToDate,Description, 
				  ApplicantNo,ApplicantName,GioiTinh,Address,Phone,Email,
				  LuongThuViec,LuongChinhThuc,NgayNhanViec,ChucDanh,KiemTraSucKhoeThucPham)
				
 select 
	 ungvien."DocEntry",ungvien."LineNum",
	 TO_VARCHAR(master."FromDate",'DD/MM/YYYY') as "FromDate" ,
	 TO_VARCHAR(master."ToDate",'DD/MM/YYYY') as "ToDate" ,
	 master."Description",
	 ungvien."ApplicantNo",
	 ungvien."ApplicantName",
	   case when ifnull(cast(ungvien."Gender" as nvarchar),'')  = '1' then 'Nam'
	   		when ifnull(cast(ungvien."Gender" as nvarchar),'')  = '0' then N'Nữ'
	   		else '' end as "GioiTinh",
	    
	   ungvien."Address",ungvien."Phone",ungvien."Email",
	   
	   "TrialPeriodSalary" as  "LuongThuViec", 
	   "ContractSalary" as "LuongChinhThuc",
	   TO_VARCHAR("U_StartDate",'DD/MM/YYYY') as  "NgayNhanViec",
	   "U_JobTitle" as "ChucDanh",
	   case when ifnull("U_FoodHealthCheck",'') = '' then ''
	   		when ifnull("U_FoodHealthCheck",'') = '0' then N'Không cần kiểm tra'
	   		when ifnull("U_FoodHealthCheck",'') = '1' then N'Cần kiểm tra'
	   		when ifnull("U_FoodHealthCheck",'') = '2' then N'Đang chờ kết quả'
	   		when ifnull("U_FoodHealthCheck",'') = '3' then N'Kết quả đạt'
	   			 end as "KiemTraSucKhoeThucPham"
from APZ_OHJV master
inner join APZ_HJV3  ungvien on master."DocEntry"  =  ungvien."DocEntry"
inner join APZ_HJV5 offer on  ungvien."DocEntry"  =  offer."DocEntry"
 

 

where  
-- Thơi gian
 master."FromDate" between :Fdate and :Tdate

and
(
					ifnull(:FoodHealthCheck, '') = '' 
					or cast ( offer."U_FoodHealthCheck" as nvarchar) in (select "DATA" from split(:FoodHealthCheck,','))
)
 
and
(
					ifnull(:StartDate, '') = '' 
					or cast( "U_StartDate" as date) <=  cast( :StartDate as date)  
)
 
order by "DocEntry";

-------------
		
		
select * from #tbl;
drop table #tbl;
		
end;


