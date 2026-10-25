CREATE PROCEDURE APZ_SalaryCalculation_Calculate1(in empNo nvarchar(40000) default null, in branch int, in period nvarchar(50) default null)
as
BEGIN

	-- DECLARE VARS
	DECLARE sqlText nvarchar(40000) = '';
	DECLARE allowanceRange NVARCHAR(40000) = '';
	DECLARE deductionRange NVARCHAR(40000) = '';
	DECLARE OverTimeRange NVARCHAR(40000) = '';
	declare allowanceSum nvarchar(40000) = '';
	declare deductionSum nvarchar(40000) = '';
	DECLARE Udfcols NVARCHAR(40000);
	DECLARE pivotCols NVARCHAR(40000);
	DECLARE empcols NVARCHAR(40000);
	DECLARE Time1 timestamp;
	DECLARE Time2 timestamp;
	declare ohemQry nvarchar(40000);
	DECLARE SalInfoUdfCols NVARCHAR(40000);
	declare lstColumnUnpivot NVARCHAR(40000);
	
	create local temporary table #tblOHEM as (select top 0 * from OHEM);
	exec 'drop table #tblOHEM';
	create local temporary table #tempStructure as (select distinct COLUMN_NAME,DATA_TYPE_NAME, "LENGTH", "POSITION" from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = 'OHEM' order by "POSITION");
	
	select ifnull((select string_agg('"' || COLUMN_NAME || '" ' || 
					case when DATA_TYPE_NAME = N'NCLOB' or DATA_TYPE_NAME = N'CLOB' or COLUMN_NAME in ('remark', 'attachment','WorkBuild','HomeBuild') then n'nclob'
					when DATA_TYPE_NAME = 'INTEGER' or DATA_TYPE_NAME = 'SMALLINT' or DATA_TYPE_NAME = 'TIMESTAMP' or DATA_TYPE_NAME = 'DATE' or DATA_TYPE_NAME = 'BIGINT' or DATA_TYPE_NAME = 'TINYINT' then DATA_TYPE_NAME
					when DATA_TYPE_NAME = 'DECIMAL' then n'numeric(19,6)'
					else DATA_TYPE_NAME || '(' || "LENGTH" || ')' end, ',') from #tempStructure),'') into ohemQry from DUMMY;
	exec 'create local temporary table #tblOHEM (' || :ohemQry || ')';
	
	insert into #tblOHEM select * from OHEM;

	create local temporary table #tbl ("empID" int, "branch" int, "DeptRemarks" nvarchar(254), "Period" nvarchar(50), "WorkQuota" numeric(19,6), "LateSum" numeric(19,6), "EarlySum" numeric(19,6), "HourPerDay" numeric(19,6)
			, "IsLastPeriod" char(1), "FromDate" timestamp, "ToDate" timestamp, "CalFrom" timestamp, "CalTo" timestamp, "LineNum" int, "LinesCount" int, "WorkTotal" numeric(19,6), "CalcWorkTotal" numeric(19,6)
			, "WorkSum" numeric(19,6), "LeaveSum" numeric(19,6), "AbsentSum" numeric(19,6), "LineActualWork" numeric(19,6), "TotalActualWork" numeric(19,6), "LineStatus" int, "BasicSal" numeric(19,6), "SalesSal" numeric(19,6)
			, "SalCur" nvarchar(10), "SalTotal" numeric(19,6), "RealSalary" numeric(19,6), "InsuranceSal" numeric(19,6), "RealSalTotal" numeric(19,6), "AdvanceAmount" numeric(19,6), "LeaveRemain" numeric(19,6)
			, "PITSAL" numeric(19,6), "AmtRelat" numeric(19,6),"SalInfoLine" int);
			
	lstColumnUnpivot = '"empID", "branch", "DeptRemarks", "Period", "WorkQuota", "LateSum", "EarlySum", "HourPerDay", "IsLastPeriod", "FromDate", "ToDate", "CalFrom", "CalTo", "LineNum", "LinesCount", "WorkTotal"
		, "CalcWorkTotal", "WorkSum", "LeaveSum", "AbsentSum", "LineActualWork", "TotalActualWork", "LineStatus", "BasicSal", "SalesSal", "SalCur", "SalTotal", "RealSalary", "InsuranceSal", "RealSalTotal"
		, "AdvanceAmount", "LeaveRemain", "PITSAL", "AmtRelat","SalInfoLine"';
	 
	 insert into #tbl
		 SELECT a."empID", :branch AS "branch", p."Remarks" AS "DeptRemarks", IFNULL(b."Period", '') AS "Period", Case When IFNULL(b."WorkQuota",0)=0 Then h."WorkQuota" Else b."WorkQuota" End as "WorkQuota", 
		 	b."LateSum", b."EarlySum", h."HourPerDay", h."IsLastPeriod", h."FromDate", h."ToDate"
		 	, CASE WHEN IFNULL(c."ValidFrom", h."FromDate") < h."FromDate" THEN h."FromDate" ELSE IFNULL(c."ValidFrom", h."FromDate") END AS "CalFrom"
		 	, CASE WHEN IFNULL(c."ValidTo", h."ToDate") > h."ToDate" THEN h."ToDate" ELSE IFNULL(c."ValidTo", h."ToDate") END AS "CalTo"
		 	, ROW_NUMBER() OVER (PARTITION BY a."empID","FromDate" ORDER BY "FromDate" ASC) AS "LineNum"
		 	, SUM(1)  over (PARTITION by a."empID","FromDate" ORder by "FromDate" ASC) AS "LinesCount"
		 	, CAST((SELECT SUM("Rate") FROM APZ_TSL1 x INNER JOIN APZ_OHWC y ON x."WorkCode" = y."Code" INNER JOIN APZ_OHWT z ON y."WorkType" = z."Id" AND y."Branch" = :branch 
		 				WHERE z."Typ" IN ('W','L') AND x."EmpId" = a."empID" AND ("WorkDay" BETWEEN CASE WHEN IFNULL(c."ValidFrom", h."FromDate") < h."FromDate" THEN h."FromDate" ELSE IFNULL(c."ValidFrom", h."FromDate") END 
		 				AND CASE WHEN IFNULL(c."ValidTo", h."ToDate") > h."ToDate" THEN h."ToDate" ELSE IFNULL(c."ValidTo", h."ToDate") END) AND "Period" = :period) 
		 			+ CASE WHEN IFNULL(CASE WHEN c."ValidTo" > h."ToDate" THEN h."ToDate" ELSE c."ValidTo" END, h."ToDate") = h."ToDate" THEN IFNULL(b."Extra", 0) ELSE 0 END AS decimal(19, 6)) AS "WorkTotal"
		 	, 0.0 AS "CalcWorkTotal"
		 	, (SELECT SUM("Rate") FROM APZ_TSL1 x INNER JOIN APZ_OHWC y ON x."WorkCode" = y."Code" INNER JOIN APZ_OHWT z ON y."WorkType" = z."Id" AND y."Branch" = :branch 
		 			WHERE z."Typ" = 'W' AND x."EmpId" = a."empID" AND ("WorkDay" BETWEEN CASE WHEN IFNULL(c."ValidFrom", h."FromDate") < h."FromDate" THEN h."FromDate" ELSE IFNULL(c."ValidFrom", h."FromDate") END 
		 			AND CASE WHEN IFNULL(c."ValidTo", h."ToDate") > h."ToDate" THEN h."ToDate" ELSE IFNULL(c."ValidTo", h."ToDate") END) AND "Period" = :period) AS "WorkSum"
		 	, (SELECT SUM("Rate") FROM APZ_TSL1 x INNER JOIN APZ_OHWC y ON x."WorkCode" = y."Code" INNER JOIN APZ_OHWT z ON y."WorkType" = z."Id" AND y."Branch" = :branch 
		 			WHERE z."Typ" = 'L' AND x."EmpId" = a."empID" AND ("WorkDay" BETWEEN CASE WHEN IFNULL(c."ValidFrom", h."FromDate") < h."FromDate" THEN h."FromDate" ELSE IFNULL(c."ValidFrom", h."FromDate") END 
		 			AND CASE WHEN IFNULL(c."ValidTo", h."ToDate") > h."ToDate" THEN h."ToDate" ELSE IFNULL(c."ValidTo", h."ToDate") END) AND "Period" = :period) AS "LeaveSum"
		 	, (SELECT SUM("Rate") FROM APZ_TSL1 x INNER JOIN APZ_OHWC y ON x."WorkCode" = y."Code" INNER JOIN APZ_OHWT z ON y."WorkType" = z."Id" AND y."Branch" = :branch 
		 			WHERE z."Typ" = 'A' AND x."EmpId" = a."empID" AND ("WorkDay" BETWEEN CASE WHEN IFNULL(c."ValidFrom", h."FromDate") < h."FromDate" THEN h."FromDate" ELSE IFNULL(c."ValidFrom", h."FromDate") END 
		 			AND CASE WHEN IFNULL(c."ValidTo", h."ToDate") > h."ToDate" THEN h."ToDate" ELSE IFNULL(c."ValidTo", h."ToDate") END) AND "Period" = :period) AS "AbsentSum"
		 	, CAST(0 AS decimal(19, 6)) AS "LineActualWork", (SELECT SUM("Rate") FROM APZ_TSL1 x INNER JOIN APZ_OHWC y ON x."WorkCode" = y."Code" INNER JOIN APZ_OHWT z ON y."WorkType" = z."Id" AND y."Branch" = :branch 
		 			WHERE x."EmpId" = a."empID" AND ("WorkDay" BETWEEN h."FromDate" AND h."ToDate") AND "Period" = :period) + IFNULL(b."Extra", 0) AS "TotalActualWork"
		 	, c."LineStatus", c."BasicSal", c."SalesSal", c."SalCur", c."SalTotal", 0.0 AS "RealSalary", 0.0 AS "InsuranceSal", 0.0 AS "RealSalTotal"
		 	, CASE WHEN IFNULL(CASE WHEN c."ValidTo" > h."ToDate" THEN h."ToDate" ELSE c."ValidTo" END, h."ToDate") = h."ToDate" THEN i."Amount" ELSE 0 END AS "AdvanceAmount"
		 	, CASE WHEN IFNULL(CASE WHEN c."ValidTo" > h."ToDate" THEN h."ToDate" ELSE c."ValidTo" END, h."ToDate") = h."ToDate" THEN b."LeaveTotal" - b."LeaveSum" ELSE 0 END AS "LeaveRemain"
		 	, k."PITSal", k."AmtRelat" ,c."LineNum" as "SalInfoLine"
		 FROM #tblOHEM a LEFT OUTER JOIN OUDP p ON a."dept" = p."Code" LEFT OUTER JOIN APZ_OHWP h ON h."PeriodCode" = :period AND h."Branch" = a."branch" 
		 				 LEFT OUTER JOIN APZ_OTSL b ON a."empID" = b."EmpId" AND h."PeriodCode" = b."Period" 
		 				 LEFT OUTER JOIN APZ_OHSI c ON a."empID" = c."EmpId" AND ((c."ValidFrom" <= h."FromDate" AND IFNULL(c."ValidTo", h."ToDate") >= h."ToDate") OR (c."ValidFrom" BETWEEN h."FromDate" AND h."ToDate") 
		 				 		OR ((c."ValidTo" IS NOT NULL AND c."ValidTo" BETWEEN h."FromDate" AND h."ToDate") OR (c."ValidTo" IS NULL AND c."ValidFrom" BETWEEN h."FromDate" AND h."ToDate"))) 
		 				 LEFT OUTER JOIN APZ_HSI2 i ON i."EmpId" = a."empID" AND h."PeriodCode" = i."Period" 
		 				 LEFT OUTER JOIN APZ_OHSP k ON (h."ToDate" = k."ValidTo" AND h."FromDate" = k."ValidFrom") OR (h."ToDate" <= IFNULL(k."ValidTo", h."ToDate") AND h."FromDate" >= k."ValidFrom")
			where  a."branch" = :branch and ((ifnull(:empNo,'') = '' or (:empNo <> '' and a."empID" in (select data from split(:empNo,',')))));
			
		update #tbl SET "Period" = ifnull(:period,'') where ifnull("Period",'') = '';
		update #tbl set "LineActualWork" = ifnull("WorkTotal",0)+ ifnull("AbsentSum",0);
		
		--Penalization
		create local temporary table #tblPen ("empID" int, "PenalTotal" numeric(19,6));	
		insert into #tblPen	
			SELECT b."EmpId", SUM(b."TotalAmount") AS "PenalTotal" FROM APZ_HPT1 b INNER JOIN APZ_OHPT a ON a."Code" = b."PenalizationType" INNER JOIN APZ_HPT2 c ON b."DocNum" = c."DocNum" AND c."Period" = :period WHERE a."Type" = 'P' AND c."IncludePenalty" = 'Y' GROUP BY b."EmpId";
	
		--Bonus
		create local temporary table #tblBonus("empID" int, "BonusTotal" numeric(19,6));
		insert into #tblBonus 
			select b."EmpId" ,SUM(b."TotalAmount") from  APZ_HPT1 b  join APZ_OHPT a  on a."Code" = b."PenalizationType" 
																			   join APZ_HPT2 c on b."DocNum" = c."DocNum" and c."Period" = :period 
															where a."Type" = 'B' and c."IncludeBonus" = 'Y' Group by b."EmpId";

	 --INSERT INTO #tbl2 (Get Salary Deduction Table)
	 create local temporary table #tbl2 ("Code" int, "Name" nvarchar(254), "CompRat" numeric(19,6), "EmpRat" numeric(19,6), "LimitedAmt" numeric(19,6), "Apply" char(1), "EmpAmount" numeric(19,6), "CompAmount" numeric(19,6));
	 insert into #tbl2
		 select a."Code",a."Name",ifnull(b."CompRat",0),ifnull(b."EmpRat",0),ifnull(b."LimitedAmt",0),b."Apply" ,0.0,0.0
			from  APZ_OHDT a  join  APZ_HDT1 b on a."Code" = b."Code" and b."Apply" ='Y' 
							  join  APZ_OHWP c ON c."PeriodCode" = :period and c."Branch" = :branch and ((c."ToDate"= b."ValidTo" and c."FromDate" = b."ValidFrom") or (c."ToDate" <=ifnull(b."ValidTo",c."ToDate") and c."FromDate" >=b."ValidFrom"));				
	
	 --INSERT INTO #tbl3  (Get new Table to store types of Deduction)
	create local temporary table #tbl3("new_col" nvarchar(254), "value" numeric(19,6), "LimitedAmt" numeric(19,6));
	insert into #tbl3 
		select 'EmpRat_' || CAST("Code" as NVARCHAR) "new_col",ifnull("EmpRat",0) "value","LimitedAmt" from #tbl2
			UNION ALL
		select 'EmpDeducAmt_' || CAST("Code" as NVARCHAR) "new_col",ifnull("EmpAmount",0) "value","LimitedAmt" from #tbl2
			UNION ALL
		select 'CompRat_' || CAST("Code" as NVARCHAR) "new_col",ifnull("CompRat",0) "value","LimitedAmt" from #tbl2
			UNION ALL
		select 'CompDeducAmt_' || CAST("Code" as NVARCHAR) "new_col",ifnull("CompAmount",0) "value","LimitedAmt" from #tbl2;
	 
	 -- Bảng chứa thông tin lương theo tất cả các khoảng của tất cả nhân viên
	create local temporary table #tblRange("EmpId" int, "FromDate" timestamp, "ToDate" timestamp, "CalFrom" timestamp, "CalTo" timestamp);
	insert into #tblRange 
	 	select c."EmpId",p."FromDate",p."ToDate",case when ifnull(c."ValidFrom",p."FromDate") < p."FromDate" then p."FromDate" else ifnull(c."ValidFrom",p."FromDate") end as "CalFrom"
			,case when ifnull(c."ValidTo",p."ToDate") > p."ToDate" then p."ToDate" else ifnull(c."ValidTo",p."ToDate") end as "CalTo"
			from APZ_OHWP p 
				LEFT JOIN APZ_OHSI c on  ((c."ValidFrom" <= p."FromDate" and ifnull(c."ValidTo",p."ToDate") >= p."ToDate") or (c."ValidFrom" between p."FromDate" and p."ToDate")
									 or ((c."ValidTo" is not null and c."ValidTo" between p."FromDate" and p."ToDate") or (c."ValidTo" is null and c."ValidFrom" between p."FromDate" and p."ToDate")))
				where p."PeriodCode" = :period and p."Branch" = :branch;

	--INSERT INTO #tbl4  (Get new Table to store Overtime information)

	create local temporary table #tbl4("new_col1" nvarchar(254), "value1" numeric(19,6), "OTWorkTotal" numeric(19,6), "OTAmount" numeric(19,6), "OTRate" numeric(19,6), "EmpId" int, "CalFrom" timestamp, "CalTo" timestamp);
	create local temporary table #temp4("OTWorkTotal" numeric(19,6), "OTAmount" numeric(19,6), "OTCode" nvarchar(50), "OTName" nvarchar(254), "EmpId" int, "OTRate" numeric(19,6), "CalFrom" timestamp, "CalTo" timestamp);
	
	insert into #temp4 
		select (select SUM("OvertimeHour") from APZ_TSH1 x  where x."EmpId" = r."EmpId" and  "WorkDay" between r."CalFrom" and ifnull(r."CalTo",r."ToDate") and x."Period" = :period and x."OvertimeType" = g."Code")
				,0 ,cast(g."Code" as NVARCHAR), g."Name" ,r."EmpId", g."Rate" ,r."CalFrom" ,r."CalTo"
			from #tblRange r left join APZ_OHOT g on g."Branch" = :branch;
	insert into #tbl4 
		select 'OTWTotal_' || Cast("OTCode" as NVARCHAR),ifnull("OTWorkTotal",0),ifnull("OTWorkTotal",0),"OTAmount","OTRate","EmpId","CalFrom","CalTo" from #temp4
			UNION ALL
		select 'OTRate_' || Cast("OTCode" as NVARCHAR),ifnull("OTRate",0),ifnull("OTWorkTotal",0),"OTAmount","OTRate","EmpId","CalFrom","CalTo" from #temp4
			UNION ALL
		select 'OTAmt_' || Cast("OTCode" as NVARCHAR),ifnull("OTAmount",0),ifnull("OTWorkTotal",0),"OTAmount","OTRate","EmpId","CalFrom","CalTo" from #temp4;
	
	-- Tổng hợp thông tin trợ cấp. khoảng thời gian sẽ vấn lấy theo bàng #tblRange (lấy các khoảng lương làm chuẩn). Khi đó sẽ lấy giá trị khoản trợ cấp mà có khoảng thời gian bằng hoặc bao ngoài khoảng lương tương ứng.
	-- Ví dụ: có 2 khoảng lương từ 1/1 -> 15/01 và 16/01 -> 31/01 thì nếu khai báo ở bảng trợ cấp cũng theo khoảng thời gian đó thì giá trị sẽ được lấy tương ứng
			-- Trường hợp nằm ngoài khoảng. VD ngoài khoảng hẳn của kì. Ví dụ trợ cấp khai báo từ trước 01/01 đến sau 31/01 thì sẽ chỉ lấy 1 giá trị trong khoảng đó cho 2 khoảng tính lương
					--  + ngoài khoảng của từng khoảng con: khoản trợ cấp sẽ được lấy giá trị nếu khoảng khai báo trợ cấp bao ngoài khoảng khai báo lương

	create local temporary table #tbl5("new_col2" nvarchar(254), "value2" numeric(19,6), "EmpId" int, "CalFrom"timestamp, "CalTo" timestamp);
		
	insert into #tbl5
		select 'AllowQta_' || CAST("Code" as NVARCHAR),ifnull("Amount",0), "EmpId"
			,case when "CalFrom" < "FromDate" then "FromDate" else "CalFrom" end as "CalFrom"
			,case when "CalTo" > "ToDate" then "ToDate" else "CalTo" end as "CalTo"
		from (select ifnull(d."Amount",0) "Amount",e."Code",e."Name",d."EmpId",d."ValidFrom",ifnull(d."ValidTo",c."ToDate") as "ValidTo",c."FromDate",c."ToDate",r."CalFrom",r."CalTo" 
				from #tblRange r 
				left join APZ_HSI1 d on r."EmpId"= d."EmpId" and (r."CalFrom" >= d."ValidFrom" and r."CalTo" <= ifnull(d."ValidTo",r."CalTo")) 
				left JOIN APZ_OHAT e ON d."AllowanceType" = e."Code" 
				left join APZ_OHWP c ON c."PeriodCode" = :period and c."Branch" = :branch
				where ((d."ValidFrom" <= c."FromDate" and ifnull(d."ValidTo",c."ToDate") >= c."ToDate") or (d."ValidFrom" between c."FromDate" and c."ToDate") 
						or ((d."ValidTo" is not null and d."ValidTo" between c."FromDate" and c."ToDate") or (d."ValidTo" is null and d."ValidFrom" between c."FromDate" and c."ToDate")))
				
				) x;
				--[APZ_SalaryCalculation_Calculate1] @branch = -2,@period = '2017-01'
			
	 --INSERT INTO #tbl5 (CALCULATE deduction amounts)
	create local temporary table #tbl12("empID" int, "RelatedPerson" int);
	insert into #tbl12 
	select b."empID", ifnull((select Count(1) from APZ_OHER a join APZ_OHWP c ON c."PeriodCode" = :period and c."Branch" = :branch where a."empID" = b."empID" and a."famDeduction" = 'Y' 
	 							and ((c."ToDate" = a."deducToDate" and c."FromDate" = a."deducFromDate") or (c."ToDate" <=ifnull(a."deducToDate",c."ToDate") and c."FromDate" >= ifnull(a."deducFromDate",c."FromDate"))) Group by a."empID"),0) 
	 	 from OHEM b;

	-- AllowanceType arrays for pivot
	select STRING_AGG('(select MAX(ifnull("value2",0)) from #tbl8 where "empID" = a."empID" and "new_col2" = ''' || cast("new_col" as NVARCHAR) || ''') "' || cast("new_col" as NVARCHAR) || '"',',') into allowanceRange 
		from (select 'AllowQta_' || cast("Code" as NVARCHAR) "new_col"
					   --Table Salary Allowance
					from (select e."Code",e."Name" from APZ_OHAT e) x) src;
	
	---- Deduction arrays for pivot
	select string_agg('(select MAX(ifnull("value",0)) from #tbl8 where "empID" = a."empID" and "new_col" = ''' || cast("new_col" as NVARCHAR) || ''') "' || cast("new_col" as NVARCHAR) || '"', ',') into deductionRange  
		from (select 'EmpRat_' || cast("Code" as NVARCHAR) "new_col" from  APZ_OHDT
				UNION ALL
			select 'EmpDeducAmt_' || cast("Code" as NVARCHAR) "new_col" from  APZ_OHDT
				UNION ALL
			select 'CompRat_' || cast("Code" as NVARCHAR) "new_col" from  APZ_OHDT
				UNION ALL
			select 'CompDeducAmt_' || cast("Code" as NVARCHAR) "new_col" from  APZ_OHDT) src;
	
	-- OverTime arrays for pivot
	select string_agg('(select MAX(ifnull("value1",0)) from #tbl8 where "empID" = a."empID" and "new_col1" = ''' || cast("new_col1" as NVARCHAR) || ''') "' || cast("new_col1" as NVARCHAR) || '"', ',') into OverTimeRange 
		from (select  'OTWTotal_' || CAST("Code" as NVARCHAR) "new_col1" from APZ_OHOT where "Branch" = :branch
					UNION ALL
				select  'OTRate_' || CAST("Code" as NVARCHAR) "new_col1" from APZ_OHOT where "Branch" = :branch
					UNION ALL
				select  'OTAmt_' || CAST("Code" as NVARCHAR) "new_col1" from APZ_OHOT where "Branch" = :branch) src;

	create local temporary table #tbl8 as (select a.*,b."new_col2",b."value2",c."new_col", c."value",e."RelatedPerson",d."new_col1",d."value1"
			from #tbl a left join #tbl5 b on a."empID" = b."EmpId" and a."CalFrom" = b."CalFrom" and a."CalTo" = b."CalTo"
						LEFT JOIN #tbl3 C ON 1=1
						left join #tbl4 d on a."empID" = d."EmpId" and a."CalFrom" = d."CalFrom" and a."CalTo" = d."CalTo"
						left join #tbl12 e on a."empID" = e."empID");
	
	lstColumnUnpivot = :lstColumnUnpivot || ',"RelatedPerson"';
	
	--PIVOT DATA TO SHOW
	sqlText = :sqlText || N' 
		create local temporary table #tbl11 as (SELECT *' || (case when ifnull(:OverTimeRange, '') = '' then '' else ', ' || :OverTimeRange end) || 
					(case when ifnull(:deductionRange, '') = '' then '' else ', ' || :deductionRange end) || 
					(case when ifnull(:allowanceRange, '') = '' then '' else ', ' || :allowanceRange end) || ', 0.0 "TotalOT", 0.0 as "TotalDeduction",0.0 as "TotalAllowance"  FROM (select distinct ' || :lstColumnUnpivot || ' from #tbl8) a);';
					
	exec :sqlText;
    
    SELECT string_agg('f."' || COLUMN_NAME || '"', ',') into SalInfoUdfCols
           from TABLE_COLUMNS
				where TABLE_NAME = 'APZ_OHSI' AND COLUMN_NAME like 'U__%' ESCAPE '_'  and schema_name = current_schema;
    
	SELECT string_agg('e."' || COLUMN_NAME || '"', ',') into Udfcols
                    from TABLE_COLUMNS where TABLE_NAME = 'APZ_OSAL' AND COLUMN_NAME like 'U__%' ESCAPE '_'  and schema_name = current_schema;

	SELECT string_agg('a."' || COLUMN_NAME || '"', ',') into empcols
           from TABLE_COLUMNS where TABLE_NAME = 'OHEM' and schema_name = current_schema AND COLUMN_NAME not in ('empID','branch','UpdateDate','CreateDate','UserSign','UserSign2','LogInstanc');
        -- [APZ_SalaryCalculation_Calculate1] @branch = -2,@period = '2017-01'   
		
	sqlText = N'create local temporary table #tblResult as(Select d.* ,e."RelatedCalculation" ,e."TaxableIncome",e."NontaxableIncome",e."TaxIncAfterFamDeduc",e."TotalSalAfterTax",e."PersonalIncomeTax",e."Arrears",e."RetroactiveSalaryIncrease",e."NetSalary"
				,e."Cash",e."BankTransferAmt", ifnull(e."BankTransfer",''N'') as "BankTransfer",e."Remarks",e."LeavePayAmt",b."PenalTotal",c."BonusTotal"
	 			,e."UserSign",e."UserSign2",e."CreateDate",e."UpdateDate",e."LogInstanc",e."MailSent",e."SendMailSuccess",e."FailReason", e."TaxableOT", e."ExemptOT",e."IncomeBefTax"' 
	 			|| (case when ifnull(:Udfcols, '') = '' then '' else ', ' || :Udfcols end) || (case when ifnull(:SalInfoUdfCols, '') = '' then '' else ', ' || :SalInfoUdfCols end) || '
	 from  #tbl a 
	 inner join #tbl11 d  on a."empID" = d."empID" and a."CalFrom" = d."CalFrom" and a."CalTo" = d."CalTo"
			left join #tblPen b on a."empID" = b."empID" and ifnull(case when a."CalTo" > a."ToDate" then a."ToDate" else a."CalTo" end,a."ToDate") = a."ToDate"
			left join #tblBonus c on a."empID" = c."empID" and ifnull(case when a."CalTo" > a."ToDate" then a."ToDate" else a."CalTo" end,a."ToDate") = a."ToDate" 
			left join APZ_OSAL e on 0=1 
			left join APZ_OHSI f on f."EmpId" = a."empID" and a."SalInfoLine" = f."LineNum");';
			
	exec :sqlText;
	
	 if(:empNo is null and :period is null)
	 THEN
		sqlText = N' SELECT Ifnull(a."lastName",'''') || 
							Case When Ifnull(a."middleName",'''') != '''' Then '' '' ||  Ifnull(a."middleName",'''') Else '''' End ||
							Case When Ifnull(a."firstName",'''') != '''' Then '', '' ||  Ifnull(a."firstName",'''') Else '''' End as "empName",b.*,' || :empcols || ' 
			From #tblResult b  left join #tblOHEM a on a."empID" = b."empID" where 0=1 order by "empID" ASC;';
	 else
		sqlText = N' SELECT Ifnull(a."lastName",'''') || 
							Case When Ifnull(a."middleName",'''') != '''' Then '' '' ||  Ifnull(a."middleName",'''') Else '''' End ||
							Case When Ifnull(a."firstName",'''') != '''' Then '', '' ||  Ifnull(a."firstName",'''') Else '''' End as "empName",b.*,' || :empcols || ' 
			From #tblResult b inner join #tblOHEM a on a."empID" = b."empID" and "LineNum"  =1 order by "empID" ASC;';
	 end if;
	
	Time1 = current_date;
	EXECUTE immediate :sqltext;
	Time2 = current_date;
END

