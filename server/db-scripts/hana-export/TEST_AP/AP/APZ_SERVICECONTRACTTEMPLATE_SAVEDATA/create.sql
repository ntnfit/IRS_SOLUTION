

CREATE PROCEDURE APZ_ServiceContractTemplate_SaveData(in RowCount int)
AS
BEGIN
	declare chkExists varchar(2) = '';
	declare Id int = 0;
	
	/* Create new temporary table */	
	create local temporary table #APZ_OCTTLocal as (select top 0 * from APZ_OCTT);
	exec 'drop table #APZ_OCTTLocal';
	exec 'create local temporary table #APZ_OCTTLocal as (select * from #APZ_OCTT)';
	
	/* Check SLAID value is exists */
	select (select 1 from #APZ_OCTTLocal a where a."SLACode" is not null and not exists(select 1 from APZ_OSLA b where a."SLACode" = b."SLAID")) into chkExists from DUMMY;
	if :chkExists = '1' then
		select 01 as "ErrorCode", 'SLAID Not Found' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	/* Check CycleData value is exists */
	select (select 1 from #APZ_OCTTLocal where ("ContractCycle" is null and "CycleUnit" is not null) or ("ContractCycle" is not null and "CycleUnit" is null)) into chkExists from DUMMY;
	if :chkExists = '1' then
		select 02 as "ErrorCode", 'Missing Contract Cycle Data' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	/* Check Tolerant Time value is exists */
	select (select 1 from #APZ_OCTTLocal where ("TolerantTime" is null and "TolerantUnit" is not null) or ("TolerantTime" is not null and "TolerantUnit" is null)) into chkExists from DUMMY;
	if :chkExists = '1' then
		select 03 as "ErrorCode", 'Missing Tolerant Time Data' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	/* Insert new data */
	delete from APZ_OCTT;
	select (select ifnull(max("ContractID"), 0) from APZ_OCTT) into Id from DUMMY;
	insert into APZ_OCTT("ContractID", "Type", "ContractTemplateNum", "Description", "ContractCycle", "CycleUnit", "TolerantTime", "TolerantUnit", "Status", "SLACode", "Remark") 
	select :Id + row_number() over(order by "ContractID") as "ContractID", "Type", "ContractTemplateNum", "Description", "ContractCycle", "CycleUnit", "TolerantTime", "TolerantUnit", "Status", "SLACode", "Remark" from #APZ_OCTTLocal;
	select 0, '' from DUMMY;
END;



