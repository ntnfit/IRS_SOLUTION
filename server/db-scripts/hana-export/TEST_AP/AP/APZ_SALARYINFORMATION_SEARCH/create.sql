
CREATE PROCEDURE APZ_SalaryInformation_Search(
	in branch nvarchar(40000) default null,
	in dept nvarchar(40000) default null,
	in empNo int default null,
	in empName nvarchar(200) default null,
	in status nvarchar(40000) default null)
as
BEGIN
	-- Select MAIN Information
	declare maxdate timestamp;
	-- Select Salary Allowance
	DECLARE allowanceRange NVARCHAR(40000)='';
	declare sqlString nvarchar(40000);
	declare settingColumns nvarchar(40000);
	
	select ifnull((select max("ValidTo") from APZ_OHSI),'9999-12-31') into maxdate from DUMMY;
	
	select ifnull((SELECT string_agg('a."' || COLUMN_NAME || '"', ',') FROM TABLE_COLUMNS WHERE schema_name = current_schema and TABLE_NAME='APZ_OHSI' and  COLUMN_NAME not in ('EmpId','LogInstanc')),'') into settingColumns from dummy;
	
	sqlString = 'SELECT b."empID", b."ExtEmpNo", ' || :settingColumns || ', b."dept"
			, IFNULL(b."lastName", '''') || '', '' || IFNULL(b."firstName", '''') AS "empName", b."branch", b."status" 
		FROM OHEM b LEFT OUTER JOIN APZ_OHSI a ON a."EmpId" = b."empID" AND CURRENT_DATE BETWEEN a."ValidFrom" AND IFNULL(a."ValidTo", cast(''' || TO_VARCHAR(:maxdate, 'yyyy-MM-dd') || ''' as timestamp))
			where (' || cast(ifnull(:branch,0) as nvarchar) || '=0  or (b."branch" = ' || cast(ifnull(:branch,0) as nvarchar) || ')) 
			AND (''' || ifnull(:dept,'') || '''='''' or ''' || ifnull(:dept,'') || ''' = ''0'' or (cast(b."dept" as nvarchar) in (select data from split(''' || ifnull(:dept,'') || ''','',''))))
			AND (''' || ifnull(:empName,'') || '''='''' or (
					(b."firstName" like N''%' || ifnull(:empName,'') || '%''
					or b."middleName" like N''%' || ifnull(:empName,'') || '%''
					or b."lastName" like N''%' || ifnull(:empName,'') || '%'')))
			AND (' || cast(ifnull(:empNo,0) as nvarchar) || '=0 or (b."empID"=' || cast(ifnull(:empNo,0) as nvarchar) || '))
			AND (''' || ifnull(:status,'') || '''= '''' or (cast(b."status" as nvarchar) in (select data from split(''' || ifnull(:status,'') || ''','',''))))
			and b."Active" = ''Y''';
			
			--select :sqlString from dummy;
			
	EXECUTE immediate :sqlString;
	
	-- a string for pivot
	select IFNULL((SELECT  string_agg('(select max("Amount") from #tbl where "empID" = a."empID" and "Name" = ''EmpAllowance_' || cast("Code" as nvarchar) || '_' || cast("Name" as nvarchar) || ''') "EmpAllowance_' || cast("Code" as nvarchar) || '_' || cast("Name" as nvarchar) || '"', ',') from APZ_OHAT),'') into allowanceRange FROM DUMMY;
	
	create local temporary table #tbl ("empID" int, "ExtEmpNo" nvarchar(50), "LineNum" int, "ValidFrom" timestamp, "ValidTo" timestamp, "Currency" nvarchar(10), "Amount" numeric(19,6)
								, "dept" smallint, "status" int, "branch" smallint, "empName" nvarchar(254), "Name" nvarchar(254));
	insert into #tbl
		SELECT b."empID", b."ExtEmpNo", "LineNum", a."ValidFrom", a."ValidTo", a."Currency", a."Amount", b."dept", b."status", b."branch"
			, ifnull(b."lastName", '') || ', ' || ifnull(b."firstName", '') AS "empName", n'EmpAllowance_' || CAST(c."Code" AS nvarchar) || '_' || CAST(c."Name" AS nvarchar) AS "Name" 
		FROM OHEM b LEFT OUTER JOIN APZ_HSI1 a ON a."EmpId" = b."empID" AND CURRENT_DATE BETWEEN a."ValidFrom" AND ifnull(a."ValidTo", :maxdate) LEFT OUTER JOIN APZ_OHAT c ON a."AllowanceType" = c."Code"
			where (ifnull(:branch,0)=0 or (:branch<>0 and b."branch" = :branch)) 
				AND (ifnull(:dept,'')='' or :dept = '0' or (cast(b."dept" as nvarchar) in (select data from split(:dept,','))))
				AND (ifnull(:empName,'')='' or (:empName<>'' and 
						(b."firstName" like N'%' || :empName || '%'
						or b."middleName" like N'%' || :empName || '%'
						or b."lastName" like N'%' || :empName || '%')))
				AND (ifnull(:empNo,0)=0 or (:empNo<>0 and b."empID"=:empNo))
				AND (ifnull(:status,'')='' or (:status<>'' and cast(b."status" as nvarchar) in (select data from split(:status,','))))
				and b."Active" = 'Y';
	
	if :allowanceRange != ''
	THEN
		sqlString = N'SELECT *, ' || :allowanceRange || ' FROM (select distinct "empID", "ExtEmpNo", "LineNum", "ValidFrom", "ValidTo", "Currency", "dept", "status", "branch", "empName" from #tbl) a;';
		
	else
		sqlString=N'SELECT "empID","ExtEmpNo","empName","LineNum","ValidFrom","ValidTo","Currency","dept","branch","status" FROM #tbl;';
	end IF;
		
	EXECUTE immediate :sqlString;
		
	DROP TABLE #tbl;
END




