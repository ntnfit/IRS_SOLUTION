CREATE procedure  ILD_R_IRS_BaoCaoPhepNam
(
	 in Year nvarchar(4)
 
)
as
Begin
--------------------------  Kỳ công ILD : 18 --> 17
declare FromDate date; -- vdu: Thang =  9, năm =  2022 --> Fromdate: '2022-08-18';
declare ToDate date;-- vdu: Thang =  9, năm =  2022 --> Todate: '2022-09-17';

/*select ADD_MONTHS(TO_DATE(Cast(cast(Nam as char(4))  ||'-18' as date)),-1)  as  "FromDate",
	    TO_DATE(Cast(cast(Nam as char(4))  ||'-17' as date))    as  "ToDate"
	   --ADD_DAYS (TO_DATE (ADD_MONTHS (TO_DATE (Cast(cast(nam as char(4)) ||'-'|| cast(thang as char(2)) ||'-20' as date), 'YYYY-MM-DD'), 1), 'YYYY-MM-DD'), -1)	
into FromDate,ToDate
from Dummy;*/

create local temporary table #tbl
(
	Thang          nvarchar(20),
	ReportTitle    nvarchar(50),
	empID 		   int,
	EmpCode        nvarchar(20),
	Department     nvarchar(20),
	PLNV           nvarchar(50),
	FullName       nvarchar(50),
	JoinedDate	   timestamp,
	Eligible       decimal(19,6),
	TLeaveMDays    decimal(19,6),
	Leavedate      timestamp,
	TakenDays      nvarchar(50),
	BalanceDays    nvarchar(100)	
);

create local temporary table #tbl1 (Leavedate timestamp, empID int, WorkCode nvarchar(50), WorkType int, LineNum int, Rate decimal(19,6));
insert into #tbl1(Leavedate, empID, WorkCode, WorkType, LineNum, Rate)
select distinct a."WorkDay", a."EmpId", "WorkCode", b."WorkType", a."LineNum", b."Rate"
from APZ_TSL1 a
left join APZ_OHWC b on a."WorkCode" = b."Code"
left join APZ_OHWT c on b."WorkType" = c."Id"
left join OHEM d on a."EmpId" = d."empID"
where c."Typ" = 'L' and a."Period" in (select "PeriodCode" from APZ_OHWP where "PYear" = Year) and d."status" is not null;

insert into #tbl(empID, EmpCode,Department,PLNV,FullName,JoinedDate, Eligible, Leavedate, Thang)
select a."empID",a."EmpCode",a."Department" ,(select b."Descr" from UFD1 b where b."TableID" = 'OHEM' and b."FieldID" = '78' and a."PLNV" = b."FldValue"), Max(a."FullName") as "FullName", Max(a."JoinedDate") as "JoinedDate", a."WorkQuota", b.Leavedate, case when to_char(b.Leavedate,'dd') >= 18 then to_char(b.Leavedate,'mm') +1 else to_char(b.Leavedate,'mm') end 
from
(
	select a."empID",a."ExtEmpNo" as "EmpCode",d."Name" as "Department",a."U_Type" as"PLNV",a."startDate" as "JoinedDate",
	       	(a."lastName" || case when ifnull( a."middleName",'') = '' then '' else ' '||  a."middleName"  ||' '||  a."firstName"  end )as "FullName",
	       	(select sum(c."WorkQuota") from APZ_OHWP c where a."branch" = c."Branch") as "WorkQuota"
	from  OHEM a
	left join  APZ_IOD1 b on a."empID"  = b."empID"
	left join OUDP d on d."Code"  = a."dept"

)a
left join #tbl1 b on b.empID = a."empID"
group by a."empID",a."EmpCode",a."Department", a."WorkQuota", b.Leavedate, a."PLNV";

update #tbl set TakenDays = 1;
update #tbl set BalanceDays = Eligible - TakenDays;
update #tbl set TLeaveMDays = (select sum(b.Rate) from #tbl1 b where #tbl.empID = b.empID);
-------------------------------------
update  a
set ReportTitle = 'Báo Cáo Nghỉ Phép Năm'
from #tbl a;
--------
select * from #tbl order by Thang, empID, Leavedate asc;

drop table #tbl;
drop table #tbl1;
END;

