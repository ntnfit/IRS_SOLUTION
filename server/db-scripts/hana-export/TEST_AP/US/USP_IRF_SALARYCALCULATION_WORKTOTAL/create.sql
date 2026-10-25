CREATE procedure USP_IRF_SalaryCalculation_WorkTotal
(
	PeriodCode nvarchar(20),
	in empID int
)
as
begin
/*
	(WorkTotal trên bảng công chỉ tính từ ngày 1-> 17 hàng tháng) + (Work quota từ ngày 18 đến cuối tháng này) 
							- (Absent sum tính từ ngày 18 đến cuối tháng trước)
	-- Vi du:
	"Ví du tính lương tháng 3 --> kỳ 2023-40-03: 18/2/2023 --> 17/03/2023... của nhân viên 213
	- work total từ 1/3 -->17/3: 11 ngày
	- work quota: Ngày công chuẩn từ 18/3 --> 31/3: 10 ngày
	- absent sum từ 18/2 --> 28/2: 2 ngày
	=> WorkTotal cuối cùng dùng để tính lương tháng 3 = 11+10-2 = 19 ngày"
*/
--declare PeriodCode nvarchar(20)  =  '2023-40-03';
declare monthFromDate int;
declare yearFromDate int;
declare monthToDate int;
declare yearToDate int;
declare fromDate date;
declare toDate date;

declare fromLastDate date;
declare toLastDate date;
if (select count(x."ToDate") from "APZ_OHWP" x where x."PeriodCode"=:PeriodCode) >= 1 then 
	Select  Month("FromDate"),Year("FromDate"),Month("ToDate"),Year("ToDate") 
	into monthFromDate,yearFromDate,monthToDate,yearToDate  from  APZ_OHWP where "PeriodCode"  = :PeriodCode;
	-- From date: ngay 01/Thang; Todate: 17/Thang
	 select TO_DATE('01' || '/' || monthToDate || '/' || yearToDate,'DD/MM/YYYY') into fromDate from dummy;
	 select TO_DATE('17' || '/' || monthToDate || '/' || yearToDate,'DD/MM/YYYY') into toDate from dummy;
	 -- FromDate cua Period
	 select TO_DATE('18' || '/' || monthFromDate || '/' || yearFromDate,'DD/MM/YYYY') into fromLastDate from dummy;
	 select LAST_DAY(TO_DATE('01' || '/' || monthFromDate || '/' || yearFromDate,'DD/MM/YYYY')) into toLastDate from dummy;
end if;

select sum("LamViec")  - sum ("NghiKhongLuong") as "Worktotal"
from (
		select    
		case when c."Typ" in ( 'W','L') then   b."Rate" else 0 end as  "LamViec",
		case when c."Typ"  = 'A' then   b."Rate" else 0 end as  "NghiKhongLuong"
			   
		from APZ_TSL1 a -- Time Sheeting 
		inner join  (select "Code","Rate","WorkType" From APZ_OHWC where "Branch" = 1) b on   b."Code" = a."WorkCode" 
		inner join APZ_OHWT c on b."WorkType"  = c."Id"
		
		where 
		       a."EmpId"  =    :empID 
		       and ( (a."WorkDay" between fromDate and toDate and c."Typ"  in ('W','L')) 
		       				or (a."WorkDay" between fromLastDate and toLastDate and c."Typ"  = 'A'))
			   		
			   
		)as Nguon1;
end;

