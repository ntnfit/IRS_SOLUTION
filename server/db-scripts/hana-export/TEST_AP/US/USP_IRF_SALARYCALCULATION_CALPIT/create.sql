CREATE procedure USP_IRF_SalaryCalculation_CalPIT (
	in TaxableAmt			numeric(19,6),
	in branch				int,
	in period				nvarchar(20),
	in empID				int
)
as
begin
	declare idx int;
	declare taxcode int;
	declare LvlAmt,TaxableAmount numeric(28,10);
	create local temporary table #tbl (
		ID					int,
		MinAmt				numeric(28,10),
		MaxAmt				numeric(28,10),
		DifAmt				numeric(28,10),
		Rate				numeric(28,10),
		TaxLevel			int,
		Alloc				numeric(28,10),
		TaxAmount			numeric(28,10),
		TaxCode				int
	);	
	insert into #tbl (ID,MinAmt,MaxAmt,Rate,TaxLevel,TaxCode)
	SELECT ROW_NUMBER() OVER (ORDER BY b."TaxLevel"),b."MinAmt",b."MaxAmt",b."Ratio",b."TaxLevel",a."Code"
	FROM  APZ_OPIT a, APZ_PIT1 b, APZ_OHWP c,APZ_OHSI d
	WHERE a."Code"=b."Code" and c."PeriodCode" = :period and c."Branch" = :branch 
	and c."FromDate" between a."FromDate" and ifnull(a."ToDate",'9999-12-31')
	and c."ToDate" between a."FromDate" and ifnull(a."ToDate",'9999-12-31')
	--and d."ValidFrom" between a."FromDate" and ifnull(a."ToDate",'9999-12-31') 
	and c."ToDate" between d."ValidFrom" and d."ValidTo"
	and a."Code" = d."TaxCode" and d."EmpId"=:empID
	and a."Apply" = 'Y' 
	and c."FromDate" between d."ValidFrom" and d."ValidTo"
	GROUP by b."TaxLevel",b."MaxAmt",b."MinAmt",b."Ratio",b."TaxLevel",a."Code";
	
	update #tbl set DifAmt = ifnull(MaxAmt,0) - ifnull(MinAmt,0) + 1;
	update #tbl set DifAmt = DifAmt - 1 where ID=1 and MinAmt=0;
	
	select max(TaxCode) into taxcode from #tbl;
	select TaxableAmt into TaxableAmount from dummy; 
	
	idx = 1;
	while idx<= (select max(ID) from #tbl) and TaxableAmount>0 do
		select DifAmt into LvlAmt from #tbl where ID=:idx;
		if :LvlAmt<=:TaxableAmount then
			update #tbl set Alloc = LvlAmt where ID = idx;
			TaxableAmount = TaxableAmount - LvlAmt;
		else if :LvlAmt>:TaxableAmount then
			update #tbl set Alloc = TaxableAmount where ID = idx;
			TaxableAmount = 0;
		end if;
		end if;
		idx = idx + 1;
	end while;
	
	update #tbl set TaxAmount = Alloc * Rate;
	
	select round(sum(TaxAmount),0) as PIT from #tbl;
	drop table #tbl;
end

