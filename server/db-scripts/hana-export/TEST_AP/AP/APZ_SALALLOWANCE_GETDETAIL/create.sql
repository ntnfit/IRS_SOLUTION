
CREATE PROCEDURE APZ_SalAllowance_GetDetail
	(in empId int)
as
BEGIN
	 	DECLARE allowanceRange NVARCHAR(5000)='';
	 	declare query nvarchar(5000);
-- a string for pivot
	SELECT IFNULL((SELECT string_agg('[EmpAllowance_' ||TO_NVARCHAR("Code")||'_'||TO_NVARCHAR("Name") ||'],') from APZ_OHAT),0) INTO allowanceRange from Dummy;

	allowanceRange=LEFT(allowanceRange,LENGTH(allowanceRange)-1);
	create local temporary table  #tbl as (
	 
     select b."empID",IFNULL(a."LineNum",1) as "LineNum",a."ValidFrom",a."ValidTo",a."Currency",a."Amount",b."dept",b."branch",b."status",IFNULL(b."lastName",'') ||', '|| IFNULL(b."firstName",'') as "empName", N'EmpAllowance_' ||TO_NVARCHAR(c."Code")||'_'||TO_NVARCHAR(c."Name") AS "Name" 
 
	from OHEM b  LEFT JOIN APZ_HSI1 a ON a."EmpId" = b."empID" LEFT JOIN APZ_OHAT c ON a."AllowanceType" = c."Code"

);

	if (allowanceRange != '')
		then
	query=N'SELECT * FROM #tbl Where "empID" ='||:empId||' PIVOT	   (MAX("Amount") FOR "Name" in ('||:allowanceRange||')) AS p ';
	else
	
	query=N'SELECT "empID","empName","LineNum","ValidFrom","ValidTo","Currency","dept","branch","status" FROM #tbl Where "empID" = '||:empId;
	end if;
	EXECUTE IMMEDIATE :query;
	
	DROP TABLE #tbl;
END




