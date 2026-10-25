CREATE procedure USP_IRF_SalaryCalculation_Allow
(
	in PeriodCode nvarchar(20),
	in empID int
)
as
begin

	declare ToDatePeriod date;
	--declare empID int = 213;
	declare ValidDatePhuCap date;
	declare ValidDateSalary date;
	--LastDate Period
	declare LastDatePeriod date; -- Vi du: ky 2023-03 --> 18/2 --> 17/03 --> LastDatePeriod =  31/03
	
	
	If (select count(x."ToDate") from "APZ_OHWP" x where x."PeriodCode"=:PeriodCode) >= 1 then 
    	select  "ToDate", LAST_DAY("ToDate")    into ToDatePeriod, LastDatePeriod  from  APZ_OHWP where "PeriodCode"  = :PeriodCode;
    
	End if;
	
	create local temporary table #tbl
	(
		STTType int,
		Name nvarchar(20),
		Amount decimal(19,6)
	);
	If (select  count(ifnull("ValidTo","ValidFrom")) from APZ_HSI1  where "EmpId" = empID ) >= 1 then 
	 
	 select  top 1 ifnull("ValidTo","ValidFrom") 
	 into ValidDatePhuCap 
	 from APZ_HSI1  
	 where "EmpId" = empID and ifnull("ValidTo","ValidFrom") >= LastDatePeriod
	 order by "ValidFrom" asc;
	 
	 -- Insert data bang tbl
	Insert into #tbl (STTType,Name, Amount)
	Select "AllowanceType",'Allow_'||"AllowanceType","Amount"
	From  APZ_HSI1
	where "EmpId" = empID 
		  and   ifnull("ValidTo","ValidFrom") = ValidDatePhuCap 
		  and  ((ifnull("ValidTo","ValidFrom") >= ToDatePeriod) 
		  or (LAST_DAY(TO_DATE('01' || '/' || month(ifnull("ValidTo","ValidFrom")) || '/' || year(ifnull("ValidTo","ValidFrom")),'DD/MM/YYYY')) >= ToDatePeriod)
		   );-- ValidDatePhuCap >= ToDate(ky) or (NgayCuoiThang(ValidDatePhuCap)) >=Todate (ky)
	End If;

	
	select * from #tbl order by STTType;
	drop table #tbl;
 
End;

