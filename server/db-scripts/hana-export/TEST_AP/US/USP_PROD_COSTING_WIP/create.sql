CREATE procedure USP_PROD_COSTING_WIP (
	in period 	nvarchar(20), 
	in PrdLine 	nvarchar(1000), 
	in branch 	int
)
as
begin
	declare fdate,tdate timestamp;
	select "F_RefDate","T_RefDate" into fdate,tdate from "OFPR" where "Code"=period;
	
	create local temporary table #tbl (
		Typ 		char(1), 
		AcctCode 	nvarchar(20), 
		Debit 		numeric(28,0), 
		Credit 		numeric(28,0), 
		ItmCode 	nvarchar(20) default N'', 
		Notes 		nvarchar(100) 
	);

	/*
	insert into #tbl (AcctCode,Debit,Credit)
	select "Account",sum("Debit"-"Credit"),sum("Credit"-"Debit")
	from "JDT1" 
	where "Account" like '154%' and "RefDate"<fdate 
	group by "Account";

	select 	AcctCode "Tài khoản", 
			TO_VARCHAR(case when Debit < 0 then 0 else Debit end, '999,999,999,999') as "Dư Nợ",
			TO_VARCHAR(case when Credit < 0 then 0 else Credit end, '999,999,999,999') as "Dư Có" 
	from #tbl order by Typ;
	*/
	
	insert into #tbl (AcctCode,Debit,Credit)
	select '1548',0,0 from dummy;
	
	insert into #tbl (AcctCode,Debit,Credit)
	select '1548',0,0 from dummy;
	select AcctCode AcctCode,Debit,Credit,ItmCode "U_ItmCode", Notes from #tbl order by Typ;

	drop table #tbl;
end

