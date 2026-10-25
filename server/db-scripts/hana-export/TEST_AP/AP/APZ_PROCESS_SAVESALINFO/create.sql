
CREATE PROCEDURE apz_process_SaveSalInfo
AS
BEGIN
	declare error int =0;
	declare error_prefix int =800005;

	declare CheckDuplicate int;
	declare CheckDuplicate2 int;
	declare maxlineNum int = 0;
	
	create local temporary table #APZ_SalInfoData as (select top 0 * from APZ_OHSI);
	exec 'drop table #APZ_SalInfoData;';
	exec 'create local temporary table #APZ_SalInfoData as (select * from "#APZ_SalInfo");';
	
	select IFNULL((SELECT TOP 1 a."EmpId" from #APZ_SalInfoData a INNER JOIN APZ_OHSI b 
	ON ifnull(a."LineNum",0) <> b."LineNum" AND a."EmpId" = b."EmpId" AND ((a."ValidTo" is null and b."ValidTo" is null) 
	OR ((b."ValidFrom" between a."ValidFrom" and ifnull(a."ValidTo",b."ValidFrom")) or (a."ValidFrom" between b."ValidFrom" and ifnull(b."ValidTo",a."ValidFrom"))))),0) into CheckDuplicate FROM dummy;
	
	select ifnull((SELECT TOP 1 a."EmpId" from #APZ_SalInfoData a INNER JOIN #APZ_SalInfoData b 
	ON ifnull(a."LineNum",0) <> b."LineNum" AND a."EmpId" = b."EmpId" AND ((a."ValidTo" is null and b."ValidTo" is null) 
	OR ((b."ValidFrom" between a."ValidFrom" and ifnull(a."ValidTo",b."ValidFrom")) or (a."ValidFrom" between b."ValidFrom" and ifnull(b."ValidTo",a."ValidFrom"))))),0) into CheckDuplicate2 FROM DUMMY;
	
	if :CheckDuplicate >0
	THEN
		SELECT  :error_prefix+1 "ErrorNumber",N'Timespan duplicated in database [EmpNo = ' || cast(:CheckDuplicate as nvarchar) || ']' "ErrorMessage" FROM DUMMY;
		return;
	elseif :CheckDuplicate2 >0
	THEN
		SELECT  :error_prefix+1 "ErrorNumber",N'Timespan duplicated [EmpNo = ' || cast(:CheckDuplicate2 as nvarchar) || ']' "ErrorMessage" FROM DUMMY;
		return;
	end IF;
			
	DELETE from APZ_OHSI a where exists(select 1 from #APZ_SalInfoData b where a."EmpId" = b."EmpId" AND a."LineNum" = b."LineNum");
				
	INSERT INTO APZ_OHSI select * from #APZ_SalInfoData;
			
	
END




