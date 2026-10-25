CREATE procedure ILD_R_IRS_KhenThuongKyLuat
(
	in FDate	date,
	in TDate	date,
	in Department 	nvarchar(5000),
	in Employee 	nvarchar(5000),
	in PBtype 	nvarchar(5000) -- Penalty/ Bonus type
)
as
Begin
create local temporary table #tbl (
		EmployeeID int,
		EmployeeCode nvarchar(50),
		EmployeeName nvarchar(100),
		JobTitle nvarchar(100),
		Department nvarchar(100),
		LineManager1 nvarchar(50),
		LineManager2 nvarchar(50),
		NoiDungKhenThuong nvarchar(250),
		MucKhenThuong float,
		NgayKhenThuong nvarchar(100),
		NoiDungKyLuat nvarchar(250),
		HinhThucKyLuat nvarchar(100),
		NgayHetHanKyLuat nvarchar(100)
		);


insert into #tbl(EmployeeID,EmployeeCode,EmployeeName,JobTitle,Department,LineManager1,LineManager2,
				  NoiDungKhenThuong,MucKhenThuong,NgayKhenThuong,
				  NoiDungKyLuat,HinhThucKyLuat,NgayHetHanKyLuat)
				
Select ep."empID" as "EmployeeID",
	   ep."Code" as "EmployeeCode",
	   ep."lastName" || case when ifnull(ep."middleName",'') = '' then '' else ' '|| ep."middleName"  ||' '|| ep."firstName"  end as "EmployeeName",
	   ep."jobTitle" as  "JobTitle",
	   dp."Name" as "Department",
	   
	   --Quản lý 
	   manager1."lastName" || case when ifnull(manager1."middleName",'') = '' then '' else ' '|| manager1."middleName"  ||' '|| manager1."firstName"  end as "LineManager1",
	   '' as "LineManager2",
 	  -- Khen thưởng
	  case when loai."Type" = 'B' then ifnull(detail."Note",'') end as "NoiDungKhenThuong",
	  case when loai."Type" = 'B' then ifnull(detail."TotalAmount",0) end as "MucKhenThuong",
	  case when loai."Type" = 'B' then TO_VARCHAR( detail."Date",'DD/MM/YYYY') end as "NgayKhenThuong",
	  -- Kỷ luật
	  case when loai."Type" = 'P' then ifnull(detail."Note",'') end as "NoiDungKyLuat",
	  case when loai."Type" = 'P' then ifnull(loai."Name",'') end as "HinhThucKyLuat",
	  case when loai."Type" = 'P' then ifnull(TO_VARCHAR("U_PenaltyExpirationDate",'DD/MM/YYYY'), '')  end as "NgayHetHanKyLuat"
from  APZ_HPT2  master
inner join APZ_OHWP WPeriod  on master."Period"  =  WPeriod."PeriodCode"


inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
inner join APZ_OHPT loai  on loai."Code" = detail."PenalizationType"
inner join  OHEM ep on ep."empID"  = detail."EmpId"
left join OUDP dp on dp."Code"  = ep."dept"

--Manager 
left join  OHEM manager1 on ep."manager"  = manager1."empID"

where  
-- Thơi gian
 detail."Date" between :Fdate and :Tdate

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

and
(
					ifnull(:PBtype, '') = '' 
					or cast(loai."Code" as nvarchar)  in (select "DATA" from split(:PBtype,','))
)

Order by  ep."empID",  master."Period";


-------------
		
		
select * from #tbl;
drop table #tbl;
		
end;


