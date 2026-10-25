CREATE PROCEDURE APZ_EmployeeLeave_SearchLeaveInformation(
	in branch int,
	in dept nvarchar(4000) default null,
	in empNo int default null,
	in empName nvarchar(200) default null,
	in status nvarchar(4000) default null,
	in period nvarchar(50) default null)
as
BEGIN

	-- Select Salary Allowance
	DECLARE leaveCodeRange NVARCHAR(40000)='';
	declare sqlString nvarchar(40000) = '';
	declare fdate,tdate timestamp;
	
	select "FromDate","ToDate" into fdate,tdate from APZ_OHWP where "PeriodCode"=:period;
	if ifnull(:branch,0)=0
	THEN
		return;
	END IF;
	
	create local temporary table #tblLeaveCode as (select top 0 * from APZ_OHWC);
	
	insert into #tblLeaveCode
		SELECT a.* FROM APZ_OHWC a join APZ_OHWT b on a."WorkType" = b."Id" where b."Typ" = 'L' and a."Branch" = :branch;
		
	-- a string for pivot
	select ifnull((SELECT string_agg(N'(select SUM("WorkTotal") from #tbl where "Name" = ''' || 'WorkCode_' || cast("Code" as nvarchar) || ''' And "empID" =y."empID") "WorkCode_' || cast("Code" as nvarchar) || '"', ',') 
					from #tblLeaveCode), '') into leaveCodeRange FROM DUMMY;
	
	-- Lấy cấu trúc của bảng trong trường hợp không có dữ liệu
	create local temporary table #tbl ("empID" int, "ExtEmpNo" nvarchar(254), "empName" nvarchar(254), "startDate" timestamp, "dept" smallint, "branch" smallint, "status" int, "Period" nvarchar(50), "WorkTotal" numeric(19,6),
										"LeaveTotal" numeric(19,6), "Used" numeric(19,6), "Remain" numeric(19,6), "UserSign2" int, "UpdateDate" timestamp, "Name" nvarchar(254));
	insert into #tbl	
		SELECT b."empID", b."ExtEmpNo", IFNULL(b."lastName", '') || ', ' || IFNULL(b."firstName", '') AS "empName", b."startDate", b."dept", b."branch", b."status", :period AS "Period", 
			Ifnull(a."WorkTotal",0) As "WorkTotal", d."LeaveTotal", d."Used", d."Remain", d."UserSign2", d."UpdateDate", n'WorkCode_' || CAST(c."Code" AS nvarchar) AS "Name" 
		FROM OHEM b LEFT OUTER JOIN APZ_TSL2 d ON b."empID" = d."EmpId" AND d."Period" = :period 
					LEFT OUTER JOIN APZ_TSL1 a ON a."EmpId" = b."empID" AND a."Period" = :period AND a."WorkCode" IN (SELECT e."Code" FROM #tblLeaveCode e) 
					LEFT OUTER JOIN #tblLeaveCode c ON a."WorkCode" = c."Code"
			where (ifnull(:branch,0)=0 or (:branch<>0 and b."branch" = :branch)) 
			AND (ifnull(:dept,'')='' or :dept = '0' or (:dept<>'' and :dept <> '0' and cast(b."dept" as nvarchar) in (select data from split(:dept,','))))
			AND (ifnull(:empName,'')='' or (:empName<>'' and 
			(b."firstName" like N'%' || :empName || '%'
			or b."middleName" like N'%' || :empName || '%'
			or b."lastName" like N'%' || :empName || '%')))
			AND (ifnull(:empNo,0)=0 or (:empNo<>0 and b."empID"=:empNo))
			AND (ifnull(:status,'')='' or (:status<>'' and cast(b."status" as nvarchar) in (select data from split(:status,','))))
			AND (ifnull(b."startDate",tdate)<=tdate and ifnull(b."termDate",fdate)>=fdate)
			and b."Active" = 'Y';

	if :leaveCodeRange != ''
	THEN
		sqlString = N'SELECT "empID", "ExtEmpNo", "empName", "startDate", "dept", "branch", "status", "Period","LeaveTotal", "Used", "Remain", "UserSign2", "UpdateDate", ' || :leaveCodeRange || ' 
					FROM #tbl y
					GROUP BY "empID", "ExtEmpNo", "empName", "startDate", "dept", "branch", "status", "Period","LeaveTotal", "Used", "Remain", "UserSign2", "UpdateDate";';
	else
		sqlString = 'Update #tbl Set "WorkTotal" = "Used";';
		exec :sqlString;
		sqlString = N'SELECT "empID","ExtEmpNo","empName","startDate","dept","branch","status","Period","WorkTotal","LeaveTotal","Used","Remain","UserSign2","UpdateDate" 
					FROM #tbl GROUP BY "empID","ExtEmpNo","empName","startDate","dept","branch","status","Period","WorkTotal","LeaveTotal","Used","Remain","UserSign2","UpdateDate";';
	end IF;

	--PRINT :sql
	EXECUTE immediate :sqlString;
	
	DROP TABLE #tbl;
END
