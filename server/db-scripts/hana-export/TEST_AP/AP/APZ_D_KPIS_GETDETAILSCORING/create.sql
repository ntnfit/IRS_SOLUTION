

CREATE PROCEDURE APZ_D_KPIs_GetDetailScoring(
	in setCode nvarchar(50) default '',
	in period datetime default null,
	in absEntry int default null)
AS
BEGIN
	
	create local temporary table #tbl
	(
		"Code" nvarchar(50), 
		"KPI_Code" nvarchar(50), 
		"EmpId" integer, 
		"Period" datetime, 
		"ViewType" nvarchar(5), 
		"U_fullName" nvarchar(254), 
		"Benchmarking" decimal(19, 6), 
		"KPI_Name" nvarchar(100), 
		"FormulaType" nvarchar(5), 
		"JobTitle" nvarchar(100), 
		"Position" nvarchar(100),
		"Department" nvarchar(100), 
		"Branch" nvarchar(100), 
		"Manager" nvarchar(100), 
		"ManagerId" integer
	);

	insert into #tbl("Code", "KPI_Code", "EmpId", "Period", "ViewType", "U_fullName", "Benchmarking", "KPI_Name", "FormulaType", "JobTitle", "Position", "Department", "Branch", "Manager", "ManagerId")
	select A."Code", B."KPI_Code", C."empID", C."Period", C."ViewType", ifnull(D."firstName", '') || ' ' || ifnull(D."middleName", '') || ' ' || ifnull(D."lastName", '') as "U_fullName",
	B."Point" as "Benchmarking", E."Name" as "KPI_Name", E."FormulaType", D."jobTitle" as "JobTitle", F."name" as "Position", G."Name" as "Department", H."Name" as "Branch",
	(select "fullName" from (select ifnull(EMP."firstName", '') || ' ' || ifnull(EMP."middleName", '') || ' ' || ifnull(EMP."lastName", '') as "fullName", row_number() over(order by EMP."firstName") as "lineId" from OHEM EMP where EMP."empID" = D."manager") xx where xx."lineId" = 1) as "Manager", 
	D."manager" as "ManagerId"
	from APZ_D_KPIS A 
	inner join APZ_D_KPIS1 B on A."Code" = B."Code"
	inner join APZ_D_OKPM C on A."Code" = C."Code"
	left join OHEM D on C."empID" = D."empID"
	left join APZ_D_OKPI E on B."KPI_Code" = E."Code"
	left join OHPS F on D."position" = F."posID"
	left join OUDP G on D."dept" = G."Code"
	left join OUBR H on D."branch" = H."Code"
	where A."Code" = :setCode and C."Period" = :period;
	
	if exists (select top 1 "KPI_Code" from APZ_D_KPR1 where "AbsEntry" = :absEntry)
	then
		select *, 
		(select "AbsEntry" from (select "AbsEntry", row_number() over(order by "AbsEntry") as "LineId" from APZ_D_KPR1 where "AbsEntry" = :absEntry and "KPI_Code" = A."KPI_Code" and "EmpId" = A."EmpId") xx where "LineId" = 1) as "AbsEntry",
		ifnull((select "Point" from (select "Point", row_number() over(order by "Point") as "LineId" from APZ_D_KPR1 where "AbsEntry" = :absEntry and "KPI_Code" = A."KPI_Code" and "EmpId" = A."EmpId") xx where "LineId" = 1), 0) as "Point",
		(select "UserSign" from (select "UserSign", row_number() over(order by "UserSign") as "LineId" from APZ_D_KPR1 where "AbsEntry" = :absEntry and "KPI_Code" = A."KPI_Code" and "EmpId" = A."EmpId") xx where "LineId" = 1) as "UserSign",
		(select "CreateDate" from (select "CreateDate", row_number() over(order by "CreateDate") as "LineId" from APZ_D_KPR1 where "AbsEntry" = :absEntry and "KPI_Code" = A."KPI_Code" and "EmpId" = A."EmpId") xx where "LineId" = 1) as "CreateDate"
		from #tbl A;
		return;
	end if;
	
	select *, null as "AbsEntry", 0 as "Point", null as "UserSign", null as "CreateDate"
	from #tbl A;
	
END



