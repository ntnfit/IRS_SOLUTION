CREATE procedure USP_IRF_SalaryCalculation_WorkQuota
(
	in PeriodCode nvarchar(20),
	in Branch nvarchar(10) -- Tham chieu den bang OUBR
)
As
Begin
	declare Thang int;
	declare Nam int;
	declare workHours nvarchar(2);
	declare i int =18;
	declare firstDate date;
	declare lastDate date;
	declare totalDaysOfMonth int;
	
	declare fromLastDate date;
  	declare toLastDate date;
/* Tao table insert tu ngay 18/Thang/Nam --> CuoiThang/Nam
-- xem ky do la 40: thi lam viec nghi thu 7, Chu nhat  --> count - (so ngay thu 7 + cn)
-- xem ky do la 48: thi lam viec nghi Chu nhat  --> count - (cn) */
if (select count(x."ToDate") from "APZ_OHWP" x where x."PeriodCode"=:PeriodCode) >= 1 then 
    select Month("ToDate"),Year("ToDate")  into Thang,Nam   from  APZ_OHWP where "PeriodCode"  = :PeriodCode;
    
end if;
    select case when Branch = '1' then '48' else '40' end    into workHours from dummy;
	select TO_DATE('18' || '/' || Thang || '/' || Nam,'DD/MM/YYYY') into firstDate from dummy;
	select LAST_DAY(:firstDate) into lastDate from dummy;
	select DAYS_BETWEEN (:firstDate, :lastDate) into totalDaysOfMonth  from dummy;
	create local temporary table #tbl
	(
		Ngay Date,
		Thu  int
	);
	while :i<= :totalDaysOfMonth +18
	DO
			insert into #tbl (Ngay,Thu)
			values (TO_DATE(:i || '/' || Thang || '/' || Nam,'DD/MM/YYYY'),
					weekday(TO_DATE(:i || '/' || Thang || '/' || Nam,'DD/MM/YYYY'))+2);
			  	i := i + 1;
	END WHILE;
	
	select case when workHours= '40' then 1+ totalDaysOfMonth -   (Sum(Thu7) + Sum(CN)) when workHours= '48' then totalDaysOfMonth -   (Sum(CN)) end
	 as "WorkQuota"
	from 
	(
		select case when Thu= 7 then 1 else 0 end Thu7,
			   case when Thu= 8 then 1 else 0 end CN
		from #tbl
	);
	drop table #tbl;
End;

