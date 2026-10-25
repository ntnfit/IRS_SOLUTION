
CREATE PROCEDURE APZ_SavePersonalIncomeTaxs(in tranType varchar(1) default '')
AS
BEGIN
	declare code int = 0;
	declare rCount int = 0;
	declare lineNum int = 0;
	declare checkCode varchar(10) = '';
	
	--Create temp table
	create local temporary table #APZ_OPITTmp as (select top 0 * from APZ_OPIT);
	exec 'DROP TABLE #APZ_OPITTmp;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OPITTmp AS (SELECT * FROM #APZ_OPIT);';
	
	create local temporary table #APZ_PIT1Tmp as (select top 0 * from APZ_PIT1);
	exec 'DROP TABLE #APZ_PIT1Tmp;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_PIT1Tmp AS (SELECT * FROM #APZ_PIT1);';
	
	--Check tranType is U then delete current record
	if :tranType = 'U' then
		select ifnull((select top 1 "Code" from #APZ_OPITTmp),0) into code from DUMMY;
		if not exists(select 1 from APZ_OPIT where "Code" =  :code)
		THEN
			select 103 as "ErrorCode", 'Document does not exists' as "ErrorMessage" from DUMMY;
			return;
		END IF;
		delete from APZ_PIT1 where "Code" = :code;
		delete from APZ_OPIT where "Code" = :code;
	end if;
	
	--Check tranType is A then delete current record and set value of code
	if :tranType = 'A' 
	then
		select ifnull((select max("Code") from APZ_OPIT),0) + 1 into code from DUMMY;
		update #APZ_OPITTmp set "Code" = :code;	
 	end if;
 	
 	select IFNULL((select Max(ifnull("LineNum",0)) from #APZ_PIT1Tmp),0) into lineNum from dummy;
 	
 	while exists(select 1 from #APZ_PIT1Tmp where ifnull("LineNum", 0) = 0)
 	DO
 		lineNum = :lineNum + 1;
 		update top 1 #APZ_PIT1Tmp set "LineNum" = :lineNum where ifnull("LineNum",0) = 0;
 	END WHILE;
	
	UPDATE #APZ_PIT1Tmp set "Code" = :code;
 	
 	--Check FromDate and Todate valid
 	--select (select top 1 a."Code" from #APZ_OPITTmp a inner join APZ_OPIT b on a."Code" <> b."Code" and ((a."FromDate" is null and b."ToDate" is null)) or ((b."FromDate" between a."FromDate" and ifnull(a."ToDate", b."FromDate"))) or (a."FromDate" between b."FromDate" and ifnull(b."ToDate", a."FromDate"))) into checkCode from DUMMY;
	--if :checkCode <> '' then
	--	select 100 as "ErrorCode",' From Date cannot be later than To Date' as "ErrorMessage" from DUMMY;
	--	return;
	--end if;
	
	select ifnull((select 1 from #APZ_OPITTmp where "FromDate" > "ToDate"), 0) into rCount from DUMMY;
	if :rCount = 1 then
		select 101 as "ErrorCode", 'From Date cannot be later than To Date' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	--Check Amount value
	select (select top 1 a."Code" from #APZ_PIT1Tmp a inner join #APZ_PIT1Tmp b ON (((b."MinAmt" between a."MinAmt" and a."MaxAmt") or (b."MaxAmt" between a."MinAmt" and a."MaxAmt") or (a."MinAmt" between b."MinAmt" and b."MaxAmt") or (a."MaxAmt" between b."MinAmt" and b."MaxAmt"))  and a."TaxLevel" != b."TaxLevel")) into checkCode from DUMMY;
	if :checkCode <> '' then
		select 102 as "ErrorCode", 'Amount Range cannot be duplicated' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	select (select 1 from #APZ_PIT1Tmp where "MinAmt" > "MaxAmt") into rCount from DUMMY;
	if :rCount = 1 then
		select 103 as "ErrorCode" ,'Min Amount cannot be larger than Max Amount' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	--Insert Data
	select ifnull((select max("LineNum") from #APZ_PIT1Tmp), 0) into lineNum from DUMMY;
	insert into APZ_OPIT("Code", "Name", "FromDate", "ToDate", "Apply", "Remarks", "UserSign2", "UpdateDate") select "Code", "Name", "FromDate", "ToDate", "Apply", "Remarks", "UserSign2", "UpdateDate" from #APZ_OPITTmp;
	insert into APZ_PIT1("Code", "TaxLevel", "MinAmt", "MaxAmt", "Ratio", "LogInstanc", "LineNum", "UserSign2", "UpdateDate") select "Code", "TaxLevel", "MinAmt", "MaxAmt", "Ratio", "LogInstanc", "LineNum", "UserSign2", "UpdateDate" from #APZ_PIT1Tmp;
	
	--Return code inserted
	select :code, '' from DUMMY;
END;




