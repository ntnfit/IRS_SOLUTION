CREATE procedure ILD_R_IRS_TheoDoiSucKhoe
(
	in FDate	date,
	in TDate	date,
	in Department 	nvarchar(5000),
	in Employee 	nvarchar(5000)
)
as
Begin
--
create local temporary table #tbl (
		EmployeeID int,
		EmployeeCode nvarchar(50),
		EmployeeName nvarchar(100),
		JobTitle nvarchar(100),
		
		JoinedDate nvarchar(100),
		
		Department nvarchar(100),
		
		NgayKiemTraSucKhoe nvarchar(50),
		
		LoaiBenhMacPhai nvarchar(150),
		
		HoSoBenhAn nvarchar(150),
		
		NgayTaiKiem nvarchar(50),
		
		GhiChu nvarchar(200)
		);

insert into #tbl(EmployeeID,EmployeeCode,EmployeeName,JobTitle,JoinedDate,Department,
				  NgayKiemTraSucKhoe,LoaiBenhMacPhai,HoSoBenhAn,NgayTaiKiem,GhiChu)
				  
				
select 
	   ep."empID" as "EmployeeID",
	   ep."Code" as "EmployeeCode",
	   ep."lastName" || case when ifnull(ep."middleName",'') = '' then '' else ' '|| ep."middleName"  ||' '|| ep."firstName"  end as "EmployeeName",
	   ep."jobTitle" as  "JobTitle",
	   
	   ifnull(TO_VARCHAR(ep."startDate",'DD/MM/YYYY'), '')  as "JoinedDate",
	   dp."Name" as "Department",
	   
	   ifnull(TO_VARCHAR(a."medicalDate",'DD/MM/YYYY'), '')as "NgayKiemTraSucKhoe",  --Ngày phát hiện	
   
	   a."disease" as "LoaiBenhMacPhai",
	   a."diseaseProfile" as "HoSoBenhAn", --Đã/ đang mắc bệnh nghề nghiệp
	    ifnull(TO_VARCHAR(a."U_NgayTaiKiem",'DD/MM/YYYY'), '') as  "NgayTaiKiem",
	   a."note" as  "GhiChu"
	   
from APZ_OHEH a

inner join  OHEM ep  on a."empID"  =  ep."empID" 
left join OUDP dp on dp."Code"  = ep."dept"	   

where  
-- Thơi gian
 a."medicalDate" between :Fdate and :Tdate

and
(
					ifnull(:Department, '') = '' 
					or cast (dp."Code" as nvarchar) in (select "DATA" from split(:Department,','))
)

and
(
					ifnull(:Employee, '') = '' 
					or cast(ep."empID" as nvarchar) in (select "DATA" from split(:Employee,','))
)


order by  ep."empID", a."medicalDate";


-------------
		
		
select * from #tbl;
drop table #tbl;
		
end;


