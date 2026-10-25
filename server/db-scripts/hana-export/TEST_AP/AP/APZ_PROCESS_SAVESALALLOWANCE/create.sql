
CREATE PROCEDURE apz_process_SaveSalAllowance

AS
BEGIN
	 declare error int =0;
	declare error_prefix int =800005;
	declare Num int;
	declare CheckDuplicate int;
	declare CheckDuplicate2 int;
	
	create local temporary table #tempData as (select top 0 * from APZ_HSI1);
	exec 'drop table #tempData';
	exec 'create local temporary table #tempData as (select * from "#APZ_SalAllowance");';
	
	select Max(ifnull("LineNum",0))  into Num from #tempData;
	
	while exists(select 1 from #tempData where ifnull("LineNum",0) = 0)
	DO
		Num = :Num + 1;
		update top 1 #tempData set "LineNum" = :Num where ifnull("LineNum",0) = 0;
	END WHILE;

	select ifnull((SELECT TOP 1 a."EmpId" from #tempData a INNER JOIN APZ_HSI1 b ON ifnull(a."LineNum",0) <> b."LineNum" AND a."EmpId" = b."EmpId" AND ((a."ValidTo" is null and b."ValidTo" is null) 
										OR ((b."ValidFrom" between a."ValidFrom" and ifnull(a."ValidTo",b."ValidFrom")) or (a."ValidFrom" between b."ValidFrom" and ifnull(b."ValidTo",a."ValidFrom"))))), 0) 
						into CheckDuplicate from DUMMY;
	select ifnull((SELECT TOP 1 a."EmpId" from #tempData a INNER JOIN #tempData b 
			ON ifnull(a."LineNum",0) <> b."LineNum" AND a."EmpId" = b."EmpId" AND ((a."ValidTo" is null and b."ValidTo" is null) OR ((b."ValidFrom" between a."ValidFrom" and ifnull(a."ValidTo",b."ValidFrom")) 
			or (a."ValidFrom" between b."ValidFrom" and ifnull(b."ValidTo",a."ValidFrom"))))),0)
		into CheckDuplicate2 from DUMMY;
		
	IF (:CheckDuplicate > 0)
	THEN
		SELECT  :error_prefix+1 "ErrorNumber"
				,N'Timespan duplicated in database [EmpNo = ' || cast(:CheckDuplicate as nvarchar) || ']' "ErrorMessage" from DUMMY;
		return;
	END IF;
	
	IF (:CheckDuplicate2 > 0)
	THEN
		SELECT :error_prefix+1 "ErrorNumber"
							,N'Timespan duplicated [EmpNo = ' || cast(:CheckDuplicate2 as nvarchar) || ']' "ErrorMessage" from dummy;
		return;
	END IF;
	
	update a set a."ValidFrom"=b."ValidFrom",	
				a."ValidTo"=b."ValidTo",
				a."AllowanceType" = b."AllowanceType",
				a."Amount" = b."Amount",
				a."Currency" = b."Currency",
				a."UserSign2" = b."UserSign2",
				a."UpdateDate" = b."UpdateDate" 
		from APZ_HSI1 a inner join #tempData b on a."EmpId" = b."EmpId" AND a."AllowanceType" = b."AllowanceType" AND a."LineNum" = b."LineNum";
		
	insert into APZ_HSI1("EmpId", "LineNum", "ValidFrom", "ValidTo", "AllowanceType", "Amount", "Currency", "UserSign2", "UpdateDate")
		select "EmpId","LineNum", "ValidFrom", "ValidTo", "AllowanceType", "Amount", "Currency", "UserSign2", "UpdateDate" from #tempData a
				where not exists(select 1 from APZ_HSI1 b where a."EmpId" = b."EmpId" AND a."AllowanceType" = b."AllowanceType" AND a."LineNum" = b."LineNum");
		
END





