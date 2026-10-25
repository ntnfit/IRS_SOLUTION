CREATE procedure ILD_R_IRS_DanhSachThongTinTuyenDung
(
	in FDate	date,
	in TDate	date,
	in JobTitle nvarchar(5000),  
	in RecruitmentSource nvarchar(5000)
)
as
Begin

create local temporary table #tbl (
		EmpID int,
		EmpCode nvarchar(50),
		ApplicantNo nvarchar(50),
		ApplicantName nvarchar(100), 
		TrialPeriodSalary float,
		ContractSalary float,  

		Status nvarchar(50),
		jobTitle nvarchar(100),
		
		Department nvarchar(100),
		StartDate nvarchar(50),
		RecruitmentSource nvarchar(100)
		);


insert into #tbl(EmpID,EmpCode,ApplicantNo,ApplicantName,TrialPeriodSalary,ContractSalary,
				Status,jobTitle,Department,startDate,RecruitmentSource)
				
select b."empID",b."ExtEmpNo",a."ApplicantNo", a."ApplicantName", a."TrialPeriodSalary", a."ContractSalary", 
	case  when ifnull(a."Status",'') = 'H' then 'Hired' 
		  when ifnull(a."Status",'') = 'A' then 'Accepted' 
	      when ifnull(a."Status",'') = 'R' then 'Rejected' 
	      end as "Status",
	     
	  
	  b."jobTitle",
	  c."Remarks" as "Department",
	  b."startDate",
	  
 	 case 
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '0' 	   then	 'Nhân viên giới thiệu'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '1' 	   then	 'Vietnamworks'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '2' 	   then	 'Careerbuilder'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '3'	   then	 'Jobstreet'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '4' 	   then	 'Vn.indeed.com'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '5' 	   then	 'Timviec365'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '6' 	   then	 'Timviecnhanh'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '7' 	   then	 'TopCV'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '8' 	   then	 '123Job'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '9' 	    then	 'Vieclam24h'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '10' 	then	 'Ybox.vn'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '11' 	then	 'Jobsgo'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '12' 	then	 'Mywork'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '13' 	then	 'Topdev'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '14' 	then	 'ITViec'
		when ifnull(cast(d."U_RecruitmentSource" as nvarchar),'')  = '15' 	then	 'Khác' 
		else  '' end as "RecruitmentSource"
	      

from  APZ_HJV6  a
left join APZ_HJV5 d on d."DocEntry" = a."DocEntry"
left join  OHEM  b on a."empId" = b."empID"
left join OUDP c on c."Code"  = b."dept" 
left join APZ_OHJV e on e."DocEntry" = a."DocEntry"
 

where  
-- Thơi gian
TO_DATE( e."FromDate") between :Fdate and :Tdate
 

 and
(
	 ifnull(:JobTitle, '') = '' 
	 or cast ( b."jobTitle"  as nvarchar) in (select "DATA" from split(:JobTitle,','))
)
 and
(
	 ifnull(:RecruitmentSource, '') = '' 
	 or cast (  d."U_RecruitmentSource"  as nvarchar) in (select "DATA" from split(:RecruitmentSource,','))
)
 
order by e."DocEntry";

-------------
		
		
select * from #tbl;
drop table #tbl;
		
end;


