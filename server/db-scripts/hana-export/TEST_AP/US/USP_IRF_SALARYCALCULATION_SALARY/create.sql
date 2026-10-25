CREATE procedure USP_IRF_SalaryCalculation_Salary
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

-- Salary
	If (select  count(ifnull("ValidTo","ValidFrom")) from APZ_OHSI  where "EmpId" = empID ) >= 1 then 
	 
	 select  top 1 ifnull("ValidTo","ValidFrom") 
	 into ValidDateSalary 
	 from APZ_OHSI  
	 where "EmpId" = empID and ifnull("ValidTo","ValidFrom") >= LastDatePeriod
	 order by "ValidFrom" asc;
	 
	 -- Insert data bang tbl
	Insert into #tbl (STTType,Name,Amount)
	Select 21,'BasicSal',"BasicSal"
	From  APZ_OHSI
	where "EmpId" = empID 
		  and   ifnull("ValidTo","ValidFrom") = ValidDateSalary 
		  and  ((ifnull("ValidTo","ValidFrom") >= ToDatePeriod) 
		  or (LAST_DAY(TO_DATE('01' || '/' || month(ifnull("ValidTo","ValidFrom")) || '/' || year(ifnull("ValidTo","ValidFrom")),'DD/MM/YYYY')) >= ToDatePeriod)
		   );-- ValidDateSalary >= ToDate(ky) or (NgayCuoiThang(ValidDateSalary)) >=Todate (ky)
	End If;
-- End Salary
	
	select * from #tbl order by STTType;
	drop table #tbl;
 
End;

