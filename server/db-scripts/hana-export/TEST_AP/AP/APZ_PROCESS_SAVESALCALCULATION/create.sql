
CREATE PROCEDURE apz_process_SaveSalCalculation 

AS
BEGIN

	declare error int =0;
	declare error_prefix int =800010;
	declare CheckValid int;
	
	create local temporary table #APZ_SalMainInfoData as(select top 0 * from APZ_OSAL);
	exec 'insert into #APZ_SalMainInfoData select * from "#APZ_SalMainInfo"';
	SELECT IFnull((SELECT TOP 1 "empID" from #APZ_SalMainInfoData where "WorkTotal" < 0 or "BasicSal" < 0 or "SalesSal" < 0 or "SalTotal" <0),0) into CheckValid from DUMMY;
	--ON a.empID = b.empID AND a.AllowanceType = b.AllowanceType AND ((b.ValidFrom between a.ValidFrom and isnull(a.ValidTo,GetDate())) or (b.ValidTo between a.ValidFrom and isnull(a.ValidTo,GetDate())) or (a.ValidFrom between b.ValidFrom and isnull(b.ValidTo,GetDate())) or (isnull(a.ValidTo,GetDate()) between b.ValidFrom and isnull(b.ValidTo,GetDate()))))
	
	IF :CheckValid > 0
	THEN
		SELECT  :error_prefix+1 "ErrorNumber"
							,N'Value can not be negative' "ErrorMessage" from DUMMY;
		return;
	END IF;
	
	--HEADER INFO
	DELETE from APZ_OSAL where "empID" in (Select Distinct("empID") from #APZ_SalMainInfoData) and "Period" = (select top 1 "Period" from #APZ_SalMainInfoData);
	INSERT INTO APZ_OSAL select * from #APZ_SalMainInfodata;
		--OVERTIME

	create local temporary table #APZ_SalOverTimedata as (select top 0 * from APZ_SAL3);
	exec 'insert into #APZ_SalOverTimedata select * from "#APZ_SalOvertime"';
	DELETE from APZ_SAL3 where "empID" in (Select Distinct("empID") from #APZ_SalOverTimeData) and "Period" = (select top 1 "Period" from #APZ_SalOverTimeData);
	INSERT INTO APZ_SAL3 select * from #APZ_SalOverTimeData;

	create local temporary table #APZ_SalAllowancedata as (select top 0 * from APZ_SAL1);
	exec 'insert into #APZ_SalAllowancedata select * from "#APZ_SalAllowance"';
	--SALARY ALLOWANCE
	DELETE from APZ_SAL1 where "empID" in (Select Distinct("empID") from #APZ_SalAllowancedata) and "Period" = (select top 1 "Period" from #APZ_SalAllowancedata);
	INSERT INTO APZ_SAL1 select * from #APZ_SalAllowancedata;

	create local temporary table #APZ_SalDeductiondata as (select top 0 * from APZ_SAL2);
	exec 'insert into #APZ_SalDeductiondata select * from "#APZ_SalDeduction"';
	--DEDUCTION
	DELETE from APZ_SAL2 where "empID" in (Select Distinct("empID") from #APZ_SalDeductionData) and "Period" = (select top 1 "Period" from #APZ_SalDeductiondata);
	INSERT INTO APZ_SAL2 select * from #APZ_SalDeductiondata;

END




