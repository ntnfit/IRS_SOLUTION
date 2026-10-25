CREATE procedure  ILD_R_IRS_BangCongMay
(
	 in Thang nvarchar(2),
	 in Nam nvarchar(4),
	 in PhongBan  table (Code nvarchar(5000)),
	 --
	 out tblOut table 
	(	
		Thang nvarchar(20),
		Nam nvarchar(20),
		ReportTitle nvarchar(50),
		ReportTitle2 nvarchar(50),
		empID int,
		EmpCode nvarchar(20),
		Department nvarchar(20),
		FullName nvarchar(50),
		
		WorkingHours decimal(19,6),
		NightShift		decimal(19,6),
		ProbationHours decimal(19,6),
		Adanced			decimal(19,6),
		Acutal			decimal(19,6),
		Adjust			decimal(19,6),
		AllDays decimal(19,6),
		TotalPaidDays	decimal(19,6),
		EmployeeStatus nvarchar(50),
		
		WorkCode nvarchar(100),
		InTime1 nvarchar(10),
		OutTime1 nvarchar(10),
		InTime2 nvarchar(10),
		OutTime2 nvarchar(10),
		InTime3 nvarchar(10),
		OutTime3 nvarchar(10),
		InTime4 nvarchar(10),
		OutTime4 nvarchar(10),
		InTime5 nvarchar(10),
		OutTime5 nvarchar(10),
		InTime6 nvarchar(10),
		OutTime6 nvarchar(10),
		InTime7 nvarchar(10),
		OutTime7 nvarchar(10),
		InTime8 nvarchar(10),
		OutTime8 nvarchar(10),
		InTime9 nvarchar(10),
		OutTime9 nvarchar(10),
		InTime10 nvarchar(10),
		OutTime10 nvarchar(10),
		InTime11 nvarchar(10),
		OutTime11 nvarchar(10),
		InTime12 nvarchar(10),
		OutTime12 nvarchar(10),
		InTime13 nvarchar(10),
		OutTime13 nvarchar(10),
		InTime14 nvarchar(10),
		OutTime14 nvarchar(10),
		
		InTime15 nvarchar(10),
		OutTime15 nvarchar(10),
		
		InTime16 nvarchar(10),
		OutTime16 nvarchar(10),
		InTime17 nvarchar(10),
		OutTime17 nvarchar(10),
		InTime18 nvarchar(10),
		OutTime18 nvarchar(10),
		InTime19 nvarchar(10),
		OutTime19 nvarchar(10),
		InTime20 nvarchar(10),
		OutTime20 nvarchar(10),
		
		InTime21 nvarchar(10),
		OutTime21 nvarchar(10),
		
		InTime22 nvarchar(10),
		OutTime22 nvarchar(10),
		
		InTime23 nvarchar(10),
		OutTime23 nvarchar(10),
		
		InTime24 nvarchar(10),
		OutTime24 nvarchar(10),
		
		InTime25 nvarchar(10),
		OutTime25 nvarchar(10),
		
		InTime26 nvarchar(10),
		OutTime26 nvarchar(10),
		
		InTime27 nvarchar(10),
		OutTime27 nvarchar(10),
		
		InTime28 nvarchar(10),
		OutTime28 nvarchar(10),
		
		InTime29 nvarchar(10),
		OutTime29 nvarchar(10),
		
		InTime30 nvarchar(10),
		OutTime30 nvarchar(10),
		
		InTime31 nvarchar(10),
		OutTime31 nvarchar(10),
		
		-- OT
		OT150_NgayThuong decimal(19,6),
		OT200_CN decimal(19,6),
		OT200 decimal(19,6),
	
		OT210_NgayNghi decimal(19,6),
		OT270_DemNghi decimal(19,6),
		OT300_NgayLe decimal(19,6),
		OT390_DemLe decimal(19,6),
		
		-- Nghỉ
		PhepNam decimal(19,6),
		NghiBu decimal(19,6),
		NghiMaChay_HieuHi decimal(19,6),
	
		NghiKhamThai decimal(19,6),
		NghiThaiSan decimal(19,6),
		CongTac decimal(19,6),
		NghiVoSinh decimal(19,6), -- Vợ sinh
		NghiLe decimal(19,6),
		NghiBenh decimal(19,6),
		NghiKhongPhep decimal(19,6),
		TongNghi_CoLuong decimal(19,6),--Paid leaves: tổng ngày nghỉ có hưởng lương
		TongNghi_KhongLuong decimal(19,6),-- Unpaid leaves
		TongHuongLuong decimal(19,6) --  Total paid days: tổng số ngày đi làm có hưởng lương + các ngày nghỉ có hưởng lương,

	)
	 
)
as
Begin
--------------------------  Kỳ công ILD : 18 --> 17
declare FromDate date; -- vdu: Thang =  9, năm =  2022 --> Fromdate: '2022-08-18';
declare ToDate date;-- vdu: Thang =  9, năm =  2022 --> Todate: '2022-09-17';



select ADD_MONTHS(TO_DATE(Cast(cast(nam as char(4)) ||'-'|| cast(thang as char(2)) ||'-18' as date)),-1)  as  "FromDate",
	    TO_DATE(Cast(cast(nam as char(4)) ||'-'|| cast(thang as char(2)) ||'-17' as date))    as  "ToDate"
	   --ADD_DAYS (TO_DATE (ADD_MONTHS (TO_DATE (Cast(cast(nam as char(4)) ||'-'|| cast(thang as char(2)) ||'-20' as date), 'YYYY-MM-DD'), 1), 'YYYY-MM-DD'), -1)	
into FromDate,ToDate
from Dummy;

create local temporary table #tbl 
(
	Thang nvarchar(20),
	Nam nvarchar(20),
	ReportTitle nvarchar(50),
	ReportTitle2 nvarchar(50),
	empID int,
	EmpCode nvarchar(20),
	Department nvarchar(20),
	
	FullName nvarchar(50),
	
	WorkingHours decimal(19,6),
	NightShift		decimal(19,6),
	ProbationHours decimal(19,6),
	Adanced			decimal(19,6),
	Acutal			decimal(19,6),
	Adjust			decimal(19,6),	
	AllDays decimal(19,6),
	TotalPaidDays	decimal(19,6),
	EmployeeStatus nvarchar(50),
	
	WorkCode nvarchar(100),
	InTime1 nvarchar(10),
	OutTime1 nvarchar(10),
	InTime2 nvarchar(10),
	OutTime2 nvarchar(10),
	InTime3 nvarchar(10),
	OutTime3 nvarchar(10),
	InTime4 nvarchar(10),
	OutTime4 nvarchar(10),
	InTime5 nvarchar(10),
	OutTime5 nvarchar(10),
	InTime6 nvarchar(10),
	OutTime6 nvarchar(10),
	InTime7 nvarchar(10),
	OutTime7 nvarchar(10),
	InTime8 nvarchar(10),
	OutTime8 nvarchar(10),
	InTime9 nvarchar(10),
	OutTime9 nvarchar(10),
	InTime10 nvarchar(10),
	OutTime10 nvarchar(10),
	InTime11 nvarchar(10),
	OutTime11 nvarchar(10),
	InTime12 nvarchar(10),
	OutTime12 nvarchar(10),
	InTime13 nvarchar(10),
	OutTime13 nvarchar(10),
	InTime14 nvarchar(10),
	OutTime14 nvarchar(10),
	
	InTime15 nvarchar(10),
	OutTime15 nvarchar(10),
	
	InTime16 nvarchar(10),
	OutTime16 nvarchar(10),
	InTime17 nvarchar(10),
	OutTime17 nvarchar(10),
	InTime18 nvarchar(10),
	OutTime18 nvarchar(10),
	InTime19 nvarchar(10),
	OutTime19 nvarchar(10),
	InTime20 nvarchar(10),
	OutTime20 nvarchar(10),
	
	InTime21 nvarchar(10),
	OutTime21 nvarchar(10),
	
	InTime22 nvarchar(10),
	OutTime22 nvarchar(10),
	
	InTime23 nvarchar(10),
	OutTime23 nvarchar(10),
	
	InTime24 nvarchar(10),
	OutTime24 nvarchar(10),
	
	InTime25 nvarchar(10),
	OutTime25 nvarchar(10),
	
	InTime26 nvarchar(10),
	OutTime26 nvarchar(10),
	
	InTime27 nvarchar(10),
	OutTime27 nvarchar(10),
	
	InTime28 nvarchar(10),
	OutTime28 nvarchar(10),
	
	InTime29 nvarchar(10),
	OutTime29 nvarchar(10),
	
	InTime30 nvarchar(10),
	OutTime30 nvarchar(10),
	
	InTime31 nvarchar(10),
	OutTime31 nvarchar(10)
);
insert into #tbl(empID, EmpCode,Department,	FullName,EmployeeStatus,
						InTime1,OutTime1,
						InTime2,OutTime2,
						InTime3,OutTime3,
						InTime4,OutTime4,
						InTime5,OutTime5,
						InTime6,OutTime6,
						InTime7,OutTime7,
						InTime8,OutTime8,
						InTime9,OutTime9,
						InTime10,OutTime10,
						InTime11,OutTime11,
						InTime12,OutTime12,
						InTime13,OutTime13,
						InTime14,OutTime14,
						InTime15,OutTime15,
						InTime16,OutTime16,
						InTime17,OutTime17,
						InTime18,OutTime18,
						InTime19,OutTime19,
						InTime20,OutTime20,
						InTime21,OutTime21,
						InTime22,OutTime22,
						InTime23,OutTime23,
						InTime24,OutTime24,
						InTime25,OutTime25,
						InTime26,OutTime26,
						InTime27,OutTime27,
						InTime28,OutTime28,
						InTime29,OutTime29,
						InTime30,OutTime30,
						InTime31,OutTime31

						)
select "empID",	"EmpCode","Department" , Max("FullName") as "FullName", 
		Max("Status") as "EmployeeStatus",
		Max(case when  DAYOFMONTH("WorkDay") =1 then "InTime" else '' end ) as  "InTime1",
		Max(case when  DAYOFMONTH("WorkDay") =1 then "OutTime" else '' end ) as  "OutTime1",
		Max(case when  DAYOFMONTH("WorkDay") =2 then "InTime" else '' end ) as  "InTime2",
		Max(case when  DAYOFMONTH("WorkDay") =2 then "OutTime" else '' end ) as  "OutTime2",
		
		Max(case when  DAYOFMONTH("WorkDay") =3 then "InTime" else '' end ) as  "InTime3",
		Max(case when  DAYOFMONTH("WorkDay") =3 then "OutTime" else '' end ) as  "OutTime3",
		
		Max(case when  DAYOFMONTH("WorkDay") =4 then "InTime" else '' end ) as  "InTime4",
		Max(case when  DAYOFMONTH("WorkDay") =4 then "OutTime" else '' end ) as  "OutTime4",
		
		Max(case when  DAYOFMONTH("WorkDay") =5 then "InTime" else '' end ) as  "InTime5",
		Max(case when  DAYOFMONTH("WorkDay") =5 then "OutTime" else '' end ) as  "OutTime5",
		Max(case when  DAYOFMONTH("WorkDay") =6 then "InTime" else '' end ) as  "InTime6",
		Max(case when  DAYOFMONTH("WorkDay") =6 then "OutTime" else '' end ) as  "OutTime6",
		
		Max(case when  DAYOFMONTH("WorkDay") =7 then "InTime" else '' end ) as  "InTime7",
		Max(case when  DAYOFMONTH("WorkDay") =7 then "OutTime" else '' end ) as  "OutTime7",
		
		Max(case when  DAYOFMONTH("WorkDay") =8 then "InTime" else '' end ) as  "InTime8",
		Max(case when  DAYOFMONTH("WorkDay") =8 then "OutTime" else '' end ) as  "OutTime8",
		
		Max(case when  DAYOFMONTH("WorkDay") =9 then "InTime" else '' end ) as  "InTime9",
		Max(case when  DAYOFMONTH("WorkDay") =9 then "OutTime" else '' end ) as  "OutTime9",
		
		Max(case when  DAYOFMONTH("WorkDay") =10 then "InTime" else '' end ) as  "InTime10",
		Max(case when  DAYOFMONTH("WorkDay") =10 then "OutTime" else '' end ) as  "OutTime10",
		
		Max(case when  DAYOFMONTH("WorkDay") =11 then "InTime" else '' end ) as  "InTime11",
		Max(case when  DAYOFMONTH("WorkDay") =11 then "OutTime" else '' end ) as  "OutTime11",
		
		Max(case when  DAYOFMONTH("WorkDay") =12 then "InTime" else '' end ) as  "InTime12",
		Max(case when  DAYOFMONTH("WorkDay") =12 then "OutTime" else '' end ) as  "OutTime12",
		
		Max(case when  DAYOFMONTH("WorkDay") =13 then "InTime" else '' end ) as  "InTime13",
		Max(case when  DAYOFMONTH("WorkDay") =13 then "OutTime" else '' end ) as  "OutTime13",
		
		Max(case when  DAYOFMONTH("WorkDay") =14 then "InTime" else '' end ) as  "InTime14",
		Max(case when  DAYOFMONTH("WorkDay") =14 then "OutTime" else '' end ) as  "OutTime14",
		
		Max(case when  DAYOFMONTH("WorkDay") =15 then "InTime" else '' end ) as  "InTime15",
		Max(case when  DAYOFMONTH("WorkDay") =15 then "OutTime" else '' end ) as  "OutTime15",
		
		Max(case when  DAYOFMONTH("WorkDay") =16 then "InTime" else '' end ) as  "InTime16",
		Max(case when  DAYOFMONTH("WorkDay") =16 then "OutTime" else '' end ) as  "OutTime16",
		
		Max(case when  DAYOFMONTH("WorkDay") =17 then "InTime" else '' end ) as  "InTime17",
		Max(case when  DAYOFMONTH("WorkDay") =17 then "OutTime" else '' end ) as  "OutTime17",
		
		Max(case when  DAYOFMONTH("WorkDay") =18 then "InTime" else '' end ) as  "InTime18",
		Max(case when  DAYOFMONTH("WorkDay") =18 then "OutTime" else '' end ) as  "OutTime18",
		
		Max(case when  DAYOFMONTH("WorkDay") =19 then "InTime" else '' end ) as  "InTime19",
		Max(case when  DAYOFMONTH("WorkDay") =19 then "OutTime" else '' end ) as  "OutTime19",
		
		Max(case when  DAYOFMONTH("WorkDay") =20 then "InTime" else '' end ) as  "InTime20",
		Max(case when  DAYOFMONTH("WorkDay") =20 then "OutTime" else '' end ) as  "OutTime20",
		
		Max(case when  DAYOFMONTH("WorkDay") =21 then "InTime" else '' end ) as  "InTime21",
		Max(case when  DAYOFMONTH("WorkDay") =21 then "OutTime" else '' end ) as  "OutTime21",
		
		Max(case when  DAYOFMONTH("WorkDay") =22 then "InTime" else '' end ) as  "InTime22",
		Max(case when  DAYOFMONTH("WorkDay") =22 then "OutTime" else '' end ) as  "OutTime22",
		
		Max(case when  DAYOFMONTH("WorkDay") =23 then "InTime" else '' end ) as  "InTime23",
		Max(case when  DAYOFMONTH("WorkDay") =23 then "OutTime" else '' end ) as  "OutTime23",
		
		Max(case when  DAYOFMONTH("WorkDay") =24 then "InTime" else '' end ) as  "InTime24",
		Max(case when  DAYOFMONTH("WorkDay") =24 then "OutTime" else '' end ) as  "OutTime24",
		
		Max(case when  DAYOFMONTH("WorkDay") =25 then "InTime" else '' end ) as  "InTime25",
		Max(case when  DAYOFMONTH("WorkDay") =25 then "OutTime" else '' end ) as  "OutTime25",
		
		Max(case when  DAYOFMONTH("WorkDay") =26 then "InTime" else '' end ) as  "InTime26",
		Max(case when  DAYOFMONTH("WorkDay") =26 then "OutTime" else '' end ) as  "OutTime26",
		
		Max(case when  DAYOFMONTH("WorkDay") =27 then "InTime" else '' end ) as  "InTime27",
		Max(case when  DAYOFMONTH("WorkDay") =27 then "OutTime" else '' end ) as  "OutTime27",
		
		Max(case when  DAYOFMONTH("WorkDay") =28 then "InTime" else '' end ) as  "InTime28",
		Max(case when  DAYOFMONTH("WorkDay") =28 then "OutTime" else '' end ) as  "OutTime28",
		
		Max(case when  DAYOFMONTH("WorkDay") =29 then "InTime" else '' end ) as  "InTime29",
		Max(case when  DAYOFMONTH("WorkDay") =29 then "OutTime" else '' end ) as  "OutTime29",
		
		Max(case when  DAYOFMONTH("WorkDay") =30 then "InTime" else '' end ) as  "InTime30",
		Max(case when  DAYOFMONTH("WorkDay") =30 then "OutTime" else '' end ) as  "OutTime30",
		
		Max(case when  DAYOFMONTH("WorkDay") =31 then "InTime" else '' end ) as  "InTime31",
		Max(case when  DAYOFMONTH("WorkDay") =31 then "OutTime" else '' end ) as  "OutTime31"
		
from
(
select a."empID",a."ExtEmpNo" as "EmpCode",d."Name" as "Department",stus."name" as "Status",

       	a."lastName" || (case when ifnull( a."middleName",'') = '' then '' else ' ' end) ||  ifnull(a."middleName",'')  ||' '||  a."firstName"  
	    as "FullName", 
		TO_DATE("WorkDay") as "WorkDay",
		"WorkCode",
		case 
		 when a."U_FullWorkCode" = 'Y' or e."WorkType" in ('11','12') then (select distinct '0'||left(c."StartTime",1) || ':' || right(c."StartTime",2)  from APZ_OEWS c left join APZ_WSC1 e on c."ShiftID" = e."ShiftID" where c."ShiftID" = b."ShiftID" and (case DAYNAME("WorkDay") when 'SUNDAY' then 1 when 'MONDAY' then 2 when 'TUESDAY' then 3 when 'WEDNESDAY' then 4 when 'THURSDAY' then 5 when 'FRIDAY' then 6 when 'SATURDAY' then 7 end)  = e."DayID")
	 	 when LENGTH("InTime")  = 3 then  '0'||left("InTime",1) || ':' || right("InTime",2) 
		 when LENGTH("InTime")  = 4 then   left("InTime",2) || ':' || right("InTime",2)  
		 when "WorkType" is not null then "WorkCode"
	 	else '' end  as "InTime",
	 
case 
	 when a."U_FullWorkCode" = 'Y' or e."WorkType" in ('11','12') then (select distinct left(c."EndTime",2) || ':' || right(c."EndTime",2)  from APZ_OEWS c left join APZ_WSC1 e on c."ShiftID" = e."ShiftID" where c."ShiftID" = b."ShiftID" and (case DAYNAME("WorkDay") when 'SUNDAY' then 1 when 'MONDAY' then 2 when 'TUESDAY' then 3 when 'WEDNESDAY' then 4 when 'THURSDAY' then 5 when 'FRIDAY' then 6 when 'SATURDAY' then 7 end)  = e."DayID")
	 when LENGTH("OutTime")  = 3 then  '0'||left("OutTime",1) || ':' || right("OutTime",2) 
	 when LENGTH("OutTime")  = 4 then   left("OutTime",2) || ':' || right("OutTime",2)  
	 else ''
	 
	 end  as "OutTime",

   DAYOFMONTH("WorkDay")|| char(13) || DAYNAME ("WorkDay") as "NgayVaThu"

from  OHEM a
left join  APZ_IOD1 b on a."empID"  = b."empID"
left join OUDP d on d."Code"  = a."dept"
left join OHST stus on a."status"  = stus."statusID"
left join APZ_OHWC e on b."WorkCode" = e."Code"

where  TO_DATE("WorkDay") Between FromDate and ToDate  
and a."Active" = 'Y'
and (cast(d."Code" as varchar) in (select Code from :PhongBan) 	
		or ifnull((select STRING_AGG(Code) from :PhongBan), '') = '' )

order by  "empID","WorkDay") as Nguon1
group by  "empID","EmpCode","Department";

create local temporary table #tbl1 (WorkingHours decimal(19,6), AllDays decimal(19,6), empID int, WorkCode nvarchar(50), WorkType nvarchar(10), LineNum int, WorkDay timestamp, SchID int, WorkQuota numeric(21,6));
insert into #tbl1(WorkingHours, empID, WorkCode, WorkType, LineNum, WorkDay, SchID, WorkQuota)
select distinct (case when ifnull("WorkCode",'') != '' and ifnull(a."DWorkHour",0) = 0 and c."Typ" = 'W' then b."Rate" * 8 else a."DWorkHour" end), a."empID", "WorkCode", c."Typ", a."LineNum", a."WorkDay", a."SchID", ifnull(d."WorkQuota",0)
from APZ_IOD1 a
left join APZ_OHWC b on a."WorkCode" = b."Code"
left join APZ_OHWT c on b."WorkType" = c."Id"
left join APZ_OTSL d on a."empID" = d."EmpId" and a."Period" = d."Period" and d."Period" in (select "PeriodCode" from APZ_OHWP where "PYear" = Nam and Right("PeriodCode",2)= cast(Thang as int))

where a."Period" in (select "PeriodCode" from APZ_OHWP where "PYear" = Nam and Right("PeriodCode",2)= cast(Thang as int))
order by month(a."WorkDay") asc
;
update #tbl set TotalPaidDays = (select max("WorkQuota") from APZ_TSL1, APZ_OHWP where "PeriodCode" = "Period" and "EmpId" = #tbl.empID) 
								- (select ifnull(count(a."WorkDay"),0) from APZ_TSL1 a
																				left join APZ_OHWC b on a."WorkCode" = b."Code"
																				left join APZ_OHWT c on b."WorkType" = c."Id" --and c."Typ" = 'A'
																				inner join OHEM d on b."Branch" = d."branch" and a."EmpId" = d."empID"
																				where a."EmpId" = #tbl.empID and a."WorkDay" between ADD_MONTHS(FromDate,1) and ADD_MONTHS(ToDate,1) and c."Typ" = 'A');
--update #tbl set WorkingHours = ifnull((select sum(a.WorkingHours) from #tbl1 a where a.empID = #tbl.empID), (select distinct b."WorkingTotal" from APZ_IOD1 a left join APZ_OEWS b on a."ShiftID" = b."ShiftID" left join OHEM c on c."empID" = a."empID" left join APZ_OHWC d on a."WorkCode" = d."Code" where a."empID" = #tbl.empID and (c."U_FullWorkCode" = 'Y' or d."WorkType" in ('11','12'))));
--update #tbl set ProbationHours = (select sum(WorkingHours) from OHEM a left join #tbl1 b on a."empID" = b.empID where a."empID" = #tbl.empID and a."status" = 2);
--update #tbl set AllDays = (select ifnull(sum(a.WorkingHours)/8,0) from #tbl1 a where a.empID = #tbl.empID);
update #tbl set Acutal = (select ifnull(sum(a.WorkingHours)/8,0) from #tbl1 a where a.empID = #tbl.empID and to_char(WorkDay,'dd') between 18 and to_char(last_day(WorkDay),'dd'));
update #tbl set Adanced = (select count(cast(to_char(a.WorkDay,'dd') as int)) from #tbl1 a inner join APZ_WSC1 b on a.SchID = b."SchID" and (case DAYNAME(a.WorkDay) when 'SUNDAY' then 1 when 'MONDAY' then 2 when 'TUESDAY' then 3 when 'WEDNESDAY' then 4 when 'THURSDAY' then 5 when 'FRIDAY' then 6 when 'SATURDAY' then 7 end) = b."DayID" where a.empID = #tbl.empID and to_char(ADD_MONTHS(WorkDay, -1),'dd') between 18 and cast(to_char(last_day(ADD_MONTHS(ToDate,-1)),'dd') as int));
update #tbl set Adjust = Acutal - Adanced;
update #tbl set NightShift = (select count(distinct a.WorkDay) * 8 from #tbl1 a inner join APZ_IOD1 b on a.empID = b."empID" and a.SchID = b."SchID" inner join APZ_OEWS c on b."ShiftID" = c."ShiftID" where a.empID = #tbl.empID and c."IsNightShift" = 'Y');

TimeSheet = select distinct a."EmpId",a."WorkDay", a."Period", b."WorkType", c."Typ", a."WorkCode", b."Rate"
			from APZ_TSL1 a
			left join APZ_OHWC b on a."WorkCode" = b."Code"
			left join APZ_OHWT c on b."WorkType" = c."Id"
			where a."Period" in (select "PeriodCode" from APZ_OHWP where "PYear" = Nam and Right("PeriodCode",2)= cast(Thang as int))
			order by a."EmpId",a."WorkDay" asc;
				
update #tbl set WorkingHours = AllDays * 8;


create local temporary table #tblOT
(
	empID int,
	OT150_NgayThuong decimal(19,6),
	OT200_CN decimal(19,6),
	OT200 decimal(19,6),

	OT210_NgayNghi decimal(19,6),
	OT270_DemNghi decimal(19,6),
	OT300_NgayLe decimal(19,6),
	OT390_DemLe decimal(19,6)
	
);
insert into  #tblOT (empID,OT150_NgayThuong,OT200,OT210_NgayNghi,OT270_DemNghi,OT300_NgayLe,OT390_DemLe)
select  "EmpId", 
Sum(case when "OvertimeType" like 'OT1%' then ifnull("WorkTotal",0) else 0 end) as "OT150_NgayThuong",

Sum(case when "OvertimeType" like 'OT2%' then ifnull("WorkTotal",0) else 0 end) as "OT200",

Sum(case when "OvertimeType" like 'OT3%' then ifnull("WorkTotal",0) else 0 end) as "OT210_NgayNghi",
Sum(case when "OvertimeType" like 'OT4%' then ifnull("WorkTotal",0) else 0 end) as "OT270_DemNghi",
Sum(case when "OvertimeType" like 'OT5%' then ifnull("WorkTotal",0) else 0 end) as "OT300_NgayLe",
Sum(case when "OvertimeType" like 'OT6%' then ifnull("WorkTotal",0) else 0 end) as "OT390_DemLe"					
 
from APZ_OTSH -- Tong OT
where "Period"  in (select "PeriodCode"
							from APZ_OHWP 
							where  "PYear"  = Nam  and  Right("PeriodCode",2)= cast(Thang as int) )
group by "EmpId" ;


------------- CÁC LOẠI NGHỈ

create local temporary table #tblNghi
(
	empID int,
	PhepNam decimal(19,6),
	NghiBu decimal(19,6),
	NghiMaChay_HieuHi decimal(19,6),

	NghiKhamThai decimal(19,6),
	NghiThaiSan decimal(19,6),
	CongTac decimal(19,6),
	NghiVoSinh decimal(19,6), -- Vợ sinh
	NghiLe decimal(19,6),
	NghiBenh decimal(19,6),
	NghiKhongPhep decimal(19,6),
	TongNghi_CoLuong decimal(19,6),--Paid leaves: tổng ngày nghỉ có hưởng lương
	TongNghi_KhongLuong decimal(19,6),-- Unpaid leaves
	TongHuongLuong decimal(19,6) --  Total paid days: tổng số ngày đi làm có hưởng lương + các ngày nghỉ có hưởng lương
	
);


insert into #tblNghi (empID,PhepNam,NghiBu,NghiMaChay_HieuHi,
					  NghiKhamThai, NghiThaiSan, 
					  CongTac, NghiVoSinh,
					  NghiLe, NghiBenh, NghiKhongPhep,
					   TongNghi_KhongLuong
					  )
					  
					  
select "EmpId",
sum(case when b."WorkType"  in ('5','6') then 1 * b."Rate"  else 0 end) as "PhepNam",
sum(case when b."WorkType" = 15 then "WorkTotal"  else 0 end) as "NghiBu",
sum(case when b."WorkType" in ('10','16') then "WorkTotal"  else 0 end) as "NghiMaChay_HieuHi",

sum(case when b."WorkType" = 14 then "WorkTotal"  else 0 end) as "NghiKhamThai",
sum(case when b."WorkType" = 8 then "WorkTotal"  else 0 end) as "NghiThaiSan",

sum(case when b."WorkType" = 11 then "WorkTotal"  else 0 end) as "CongTac",
sum(case when b."WorkType" = 13 then "WorkTotal"  else 0 end) as "NghiVoSinh",-- Nghi vợ sinh

sum(case when b."WorkType" = 18 then "WorkTotal"  else 0 end) as "NghiLe",
sum(case when b."WorkType" = 7 then "WorkTotal"  else 0 end) as "NghiBenh",
sum(case when b."WorkType" = 19 then "WorkTotal"  else 0 end) as "NghiKhongPhep",

sum(case when b."WorkType" = 17 then "WorkTotal"  else 0 end) as "TongNghi_KhongLuong" -- -->  = Không phép 

from APZ_TSL1 a
inner join APZ_OHWC b on a."WorkCode"  =  b."Code"
inner join OHEM c on a."EmpId" = "empID" and b."Branch" = c."branch"
where "Period"  in (select "PeriodCode"
							from APZ_OHWP 
							where  "PYear" = Nam  and  Right("PeriodCode",2)= cast(Thang as int))

group by "EmpId";



-------------------------------------
update a set TongNghi_CoLuong = PhepNam + NghiMaChay_HieuHi + NghiBu + NghiLe from #tblNghi a; 

update a set AllDays = a.TotalPaidDays - b.TongNghi_CoLuong from #tbl a left join #tblNghi b on a.empID = b.empID;
update a set ProbationHours = a.AllDays * 8 from #tbl a left join OHEM b on b."empID" = a.empID and b."status" = 2;

update  a
set ReportTitle = 'BẢNG CHẤM CÔNG', ReportTitle2  = 'Tháng:' || :Thang ||'/ ' ||:Nam,
	Thang = :Thang, Nam = :Nam
from #tbl a;
--------

tblOut = select a.*,b.OT150_NgayThuong,b.OT200_CN,b.OT200,b.OT210_NgayNghi,b.OT270_DemNghi,b.OT300_NgayLe,b.OT390_DemLe,
		  c.PhepNam,c.NghiBu,c.NghiMaChay_HieuHi,
		  c.NghiKhamThai, c.NghiThaiSan, 
		  c.CongTac, c.NghiVoSinh,
		  c.NghiLe, c.NghiBenh, c.NghiKhongPhep,
		  c.TongNghi_CoLuong,  c.TongNghi_KhongLuong,
		  c.TongHuongLuong
from  #tbl a
left join #tblOT b on a.empID = b.empID 
left join #tblNghi c on a.empID = c.empID 
order by  empID;

drop table #tbl;
drop table #tbl1;
drop table #tblOT;
drop table #tblNghi;
END;

