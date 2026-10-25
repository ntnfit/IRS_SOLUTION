
CREATE PROCEDURE APZ_SalaryInformation_SearchAllowance(
	in branch nvarchar(40000) default null,
	in dept nvarchar(40000) default null,
	in empNo int default null,
	in empName nvarchar(200) default null,
	in status nvarchar(40000) default null,
	in validFrom timestamp,
	in validTo timestamp default null)
as
BEGIN
	declare maxdate timestamp;
	-- Select Salary Allowance
	DECLARE allowanceRange NVARCHAR(40000)='';
	declare sqlString nvarchar(40000);
	
	select ifnull((select max("ValidTo") from APZ_HSI1),'9999-12-31') into maxdate from DUMMY;
	
	-- a string for pivot
	select IFNULL((SELECT  string_agg('(select max("Amount") from #tbl where "empID" = a."empID" and "Name" = ''EmpAllowance_' || cast("Code" as nvarchar) || '_' || cast("Name" as nvarchar) || ''') "EmpAllowance_' || cast("Code" as nvarchar) || '_' || cast("Name" as nvarchar) || '"', ',') from APZ_OHAT),'') into allowanceRange FROM DUMMY;
	
	create local temporary table #tbl ("empID" int, "ExtEmpNo" nvarchar(50), "LineNum" int, "ValidFrom" timestamp, "ValidTo" timestamp, "Currency" nvarchar(10), "Amount" numeric(19,6)
								, "dept" smallint, "status" int, "branch" smallint, "empName" nvarchar(254), "Name" nvarchar(254));
	
	insert into #tbl
		SELECT b."empID", b."ExtEmpNo", a."LineNum", ifnull(a."ValidFrom", :validFrom) AS "ValidFrom"
			, CASE WHEN (a."LineNum" = 0 OR a."LineNum" IS NULL) THEN ifnull(a."ValidTo", :validTo) ELSE a."ValidTo" END AS "ValidTo", a."Currency", a."Amount", b."dept", b."branch", b."status"
			, b."empID" AS "empName", n'EmpAllowance_' || CAST(c."Code" AS nvarchar) || '_' || CAST(c."Name" AS nvarchar) AS "Name" 
		FROM OHEM b LEFT OUTER JOIN APZ_HSI1 a ON a."EmpId" = b."empID"
			and ((a."ValidFrom" between :validFrom and ifnull(:validTo,:maxdate) or ifnull(a."ValidTo",a."ValidFrom") between :validFrom and ifnull(:validTo,:maxdate) or :validFrom between a."ValidFrom" and ifnull(a."ValidTo",:maxdate))
			or (:validFrom is null and  a."ValidFrom" < ifnull(:validTo,:maxdate)))
			LEFT JOIN APZ_OHAT c ON a."AllowanceType" = c."Code"
			where (ifnull(:branch,0)=0 or (:branch<>0 and b."branch" = :branch)) 
			AND (ifnull(:dept,'')='' or :dept = '0' or (:dept<>'' and :dept <> '0' and cast(b."dept" as nvarchar) in (select data from split(:dept,','))))
			AND (ifnull(:empName,'')='' or (:empName<>'' and 
					(b."firstName" like N'%' || :empName || '%'
					or b."middleName" like N'%' || :empName || '%'
					or b."lastName" like N'%' || :empName || '%')))
			AND (ifnull(:empNo,0)=0 or (:empNo<>0 and b."empID"=:empNo))
			AND (ifnull(:status,'')='' or (:status<>'' and cast(b."status" as nvarchar) in (select data from split(:status,','))))
			and b."Active" = 'Y';
		
	
		if :allowanceRange != ''
		THEN
			sqlString = N'SELECT * , ' || :allowanceRange || ' FROM 
					(select distinct "empID", "ExtEmpNo", "LineNum", "ValidFrom", "ValidTo", "Currency", "dept", "status", "branch", "empName" from #tbl) a;';
		
		else
		
			sqlString=N'SELECT "empID","ExtEmpNo","empName","LineNum","ValidFrom","ValidTo","Currency","dept","branch","status" FROM #tbl;';
		end IF;
	
		--PRINT :sql
		EXECUTE immediate :sqlString;
		
		DROP TABLE #tbl;
END




