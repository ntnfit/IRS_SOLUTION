
CREATE PROCEDURE APZ_TimeSheeting_SaveLeaveInformation
AS
BEGIN

	create local temporary table #dtAPZ_TSL2Data as (select top 0 *, 0 "empID", '' "empName" from APZ_TSL2);
	exec 'drop table #dtAPZ_TSL2Data';
	exec 'create local temporary table #dtAPZ_TSL2Data as (select * from "#dtAPZ_TSL2");';
	
	update APZ_TSL2 a set "LeaveTotal"=b."LeaveTotal", "Used"=b."Used", "Remain"=b."Remain","UserSign2" = b."UserSign2" from APZ_TSL2 a inner join #dtAPZ_TSL2Data b ON a."EmpId"=b."empID" and a."Period"=b."Period";
		
	INSERT into  APZ_TSL2("Period","EmpId","EmpName","LeaveTotal","Used","Remain","UserSign2","UpdateDate")  
		select b."Period",b."empID",b."empName",b."LeaveTotal",b."Used",b."Remain",b."UserSign2",b."UpdateDate" from #dtAPZ_TSL2Data b where not exists(select 1 from APZ_TSL2 a where a."EmpId"=b."empID" and a."Period"=b."Period");


END




