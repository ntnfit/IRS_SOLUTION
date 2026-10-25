
CREATE PROCEDURE apz_process_SaveSalInfoByEmp
AS
BEGIN
	declare error int =0;
	declare error_prefix int =800005;
	declare Num int = 0;
	declare CheckDuplicate int;
	declare checkCount int = 0;
	
	create local temporary table #APZ_SalInfoByEmpData as (select top 0 * from APZ_OHSI);
	exec 'drop table #APZ_SalInfoByEmpData;';
	exec 'create local temporary table #APZ_SalInfoByEmpData as (select * from "#APZ_SalInfoByEmp");';
	
	select IFNULL((select Max(ifnull("LineNum",0)) from #APZ_SalInfoByEmpData),0) into Num FROM DUMMY;
		
	select IFNULL((select top 1 1 from #APZ_SalInfoByEmpData where ifnull("LineNum",0) = 0),0) into checkCount from DUMMY;
	while :checkCount = 1
	DO
		Num = :num + 1;
		update top 1 #APZ_SalInfoByEmpData set "LineNum" = :Num where ifnull("LineNum",0) = 0;
		select IFNULL((select top 1 1 from #APZ_SalInfoByEmpData where ifnull("LineNum",0) = 0),0) into checkCount from DUMMY;
	
	END WHILE;
	
	select IFNULL((SELECT TOP 1 a."EmpId" from #APZ_SalInfoByEmpData a INNER JOIN #APZ_SalInfoByEmpData b 
	ON a."LineNum" <> b."LineNum" AND a."EmpId" = b."EmpId" AND ((a."ValidTo" is null and b."ValidTo" is null) OR ((b."ValidFrom" between a."ValidFrom" and ifnull(a."ValidTo",b."ValidFrom"))  
	or (a."ValidFrom" between b."ValidFrom" and ifnull(b."ValidTo",a."ValidFrom"))))),0) into CheckDuplicate from DUMMY;

	IF :CheckDuplicate > 0
	THEN
		SELECT :error_prefix+1 "ErrorNumber",N'Timespan cannot be duplicate' "ErrorMessage" FROM DUMMY;
		return;
	END IF;
	
	UPDATE a Set a."UserSign2" = b."UserSign2",
				a."UpdateDate" = b."UpdateDate"
				from #APZ_SalInfoByEmpData a  join APZ_OHSI b ON a."EmpId" = b."EmpId" AND a."LineNum" = b."LineNum";
	
	Delete From APZ_OHSI Where "EmpId" = (Select "EmpId" From #APZ_SalInfoByEmpData limit 1);
	Insert Into APZ_OHSI Select * From #APZ_SalInfoByEmpData;
	
END



