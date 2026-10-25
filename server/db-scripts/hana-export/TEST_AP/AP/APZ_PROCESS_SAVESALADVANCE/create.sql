
CREATE PROCEDURE apz_process_SaveSalAdvance
AS
BEGIN
	declare error int = 0;
	declare error_prefix int = 800005;
	declare CheckDuplicate int;
	declare Num int;
	declare checkUpdateLine int;
	
	create local temporary table #APZ_SalAdvanceData as (select top 0 *, 0 "empID" from APZ_HSI2);
	exec 'drop table #APZ_SalAdvanceData';
	exec 'create local temporary table #APZ_SalAdvanceData as (select * from "#APZ_SalAdvance")';

	update #APZ_SalAdvanceData set "LineNum" = 0 where ifnull("LineNum",0) = 0;
	select IFNULL((select Max("LineNum") from #APZ_SalAdvanceData),0) into Num from Dummy;
	select ifnull((select top 1 1 from #APZ_SalAdvanceData where "LineNum" = 0),0) into checkUpdateLine from dummy;
	
	while(:checkUpdateLine = 1)
	DO
		Num = :Num + 1;
		update top 1 #APZ_SalAdvanceData set "LineNum" = :Num where IFNULL("LineNum",0) = 0;
		select ifnull((select top 1 1 from #APZ_SalAdvanceData where "LineNum" = 0),0) into checkUpdateLine from dummy;
	END WHILE;
	
	SELECT IFNULL((SELECT TOP 1 a."empID"  from #APZ_SalAdvanceData a INNER JOIN #APZ_SalAdvanceData b 
	ON a."LineNum" <> b."LineNum" AND a."Period" = b."Period" AND a."empID" = b."empID"),0) into CheckDuplicate FROM DUMMY; 
	IF CheckDuplicate > 0
	then
	BEGIN
	SELECT  
							:error_prefix+1 "ErrorNumber"
							,N'Period cannot be duplicate' "ErrorMessage" from Dummy;
						return;
	END;
	end if;
	
	update t set "Period"=S."Period",	
					"Amount"=S."Amount",
					"Remain" = S."Remain",
					"MonthlyPay" = S."MonthlyPay",
					"Received" = S."Received",
					"Remarks" = S."Remarks",
					"UserSign2" = S."UserSign2",
					"UpdateDate" = S."UpdateDate" from APZ_HSI2 t inner join #APZ_SalAdvanceData S ON T."EmpId" = S."empID" AND T."LineNum" = S."LineNum";
	
	insert into APZ_HSI2("EmpId", "LineNum", "Period", "Amount", "Remain", "MonthlyPay", "Received", "Remarks", "UserSign2", "UpdateDate")
		select "empID", "LineNum", "Period", "Amount", "Remain", "MonthlyPay", "Received", "Remarks", "UserSign2", "UpdateDate" from #APZ_SalAdvanceData s
			where not exists(select 1 from APZ_HSI2 t where T."EmpId" = S."empID" AND T."LineNum" = S."LineNum");
	
END




