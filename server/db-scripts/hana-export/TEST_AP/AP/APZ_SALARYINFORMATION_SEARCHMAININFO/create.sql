
CREATE PROCEDURE APZ_SalaryInformation_SearchMainInfo(
	in branch nvarchar(40000) default null,
	in dept nvarchar(40000) default null,
	in empNo int default null,
	in empName nvarchar(200) default null,
	in status nvarchar(40000) default null,
	in validFrom timestamp,
	in validTo timestamp default null)
as
BEGIN
	--print :empName
	declare maxdate timestamp;
	declare sqlQuery nvarchar(400000);
	declare columnQuery nvarchar(400000);
	select ifnull((select max("ValidTo") from APZ_OHSI),'9999-12-31') into maxdate FROM DUMMY;
	
	select string_agg('a."' || COLUMN_NAME || '"', ',') into columnQuery from TABLE_COLUMNS 
					where TABLE_NAME = 'APZ_OHSI' and SCHEMA_NAME = current_schema and COLUMN_NAME not in ('EmpId','LogInstanc','ValidFrom','ValidTo','LineStatus');
					
	sqlQuery = N'select b."empID",b."ExtEmpNo",' || :columnQuery || ',ifnull(a."ValidFrom",cast(''' || cast(:validFrom as nvarchar) || ''' as datetime)) as "ValidFrom"
	, case when (a."LineNum" =0 or a."LineNum" is null) then ifnull(a."ValidTo",cast(''' || cast(:validTo as nvarchar) || ''' as datetime)) else a."ValidTo"  end as  "ValidTo"
	, case when (a."LineNum" =0 or a."LineNum" is null) then b."status" else a."LineStatus" end as "LineStatus",b."dept",
	b."empID" as "empName"
	,b."branch",b."status"  
	from OHEM b  
	LEFT OUTER JOIN APZ_OHSI a ON a."EmpId" = b."empID"
	and ((a."ValidFrom" between cast(''' || cast(:validFrom as nvarchar) || ''' as datetime) and cast(''' || cast(ifnull(:validTo,:maxdate) as nvarchar) || ''' as datetime) or ifnull(a."ValidTo",a."ValidFrom")
			between cast('''|| cast(:validFrom as nvarchar) || ''' as datetime) and cast(''' || cast(ifnull(:validTo,:maxdate) as nvarchar) || ''' as datetime) 
				or cast(''' || cast(:validFrom as nvarchar) || ''' as datetime) between a."ValidFrom" and ifnull(a."ValidTo",cast(''' || cast(:validFrom as nvarchar) || ''' as datetime)) 
				and ''' || ifnull(cast(:validFrom as nvarchar),'') || ''' != '''')
	or (''' || ifnull(cast(:validFrom as nvarchar),'') || ''' = '''' and  a."ValidFrom" < cast(''' || cast(ifnull(:validTo,:maxdate) as nvarchar) || ''' as datetime)))
	where 
	(' || cast(ifnull(:branch,0) as nvarchar) || '=0  or (b."branch" = ' || cast(ifnull(:branch,0) as nvarchar) || ')) 
	AND (''' || ifnull(:dept,'') || '''='''' or ''' || ifnull(:dept,'') || ''' = ''0'' or (b."dept" in (select data from split(''' || ifnull(:dept,'') || ''','',''))))
	AND (''' || ifnull(:empName,'') || '''='''' or ((b."firstName" like N''%''' || ifnull(:empName,'') || '''%''
			or b."middleName" like N''%''' || ifnull(:empName,'') || '''%''
			or b."lastName" like N''%''' || ifnull(:empName,'') || '''%'')))
	AND (' || cast(ifnull(:empNo,0) as nvarchar) || '=0 or (b."empID"=' || cast(ifnull(:empNo,0) as nvarchar) || '))
	AND (''' || ifnull(:status,'') || '''='''' or (b."status" in (select data from split('''|| ifnull(:status,'') || ''','',''))))
	and b."Active" = ''Y''
	ORDER BY b."empID";';
	execute immediate :sqlQuery;
	
END




