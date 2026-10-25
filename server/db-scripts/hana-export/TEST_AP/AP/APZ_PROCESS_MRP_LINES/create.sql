
CREATE procedure apz_process_MRP_LINES
as
BEGIN
	DECLARE error NVARCHAR(40000) ='';
	DECLARE error_message NVARCHAR(40000)='';
	declare sqlstr VARCHAR(40000) = '';
	declare listUDFCols VARCHAR(40000) = '';
	-- Validate items 
	declare itemCodenotIn nvarchar(50);
	declare branch int = -1;
	
	create local temporary table #APZ_MRP_LINESData as (select top 0 * from APZ_LIN1);
	exec 'drop table #APZ_MRP_LINESData;';
	exec 'create local temporary table #APZ_MRP_LINESData as(select * from "#APZ_MRP_LINES");';
	
	select ifnull((select top 1 a."ItemCode" from #APZ_MRP_LINESData a where a."ItemCode" not in (select b."ItemCode" from oitm b) ),'') into itemCodenotIn from DUMMY;
	if :itemCodenotIn<>''
	THEN
		select 'Invalid item - ' || :itemCodenotIn from DUMMY;
		return;
	end IF;
	
	select IFNULL((Select top 1 "Branch" From APZ_OLIN Where "LineID"=(Select top 1 "LineID" From #APZ_MRP_LINESData)), -1) into branch from DUMMY;
	
	select ifnull((select top 1 a."ItemCode" from #APZ_MRP_LINESData a Inner join
								(Select "ItemCode" From APZ_LIN1 t1 inner join APZ_OLIN t2 on t1."LineID"=t2."LineID" Where t2."Branch"=:branch) t ON a."ItemCode" = t."ItemCode"),'') into itemCodenotIn from DUMMY;
	if :itemCodenotIn<>''
	THEN
		select 'Item - ' || :itemCodenotIn || ' existed in this branch' from DUMMY;
		return;
	end IF;
	
	select ifnull((Select "ItemCode" From (SELECT top 1 "ItemCode" FROM #APZ_MRP_LINESData GROUP BY "ItemCode" HAVING COUNT(1) > 1) t),'') into itemCodenotIn from DUMMY;
	if :itemCodenotIn<>''
	THEN
		select 'Duplicate Item - ' || :itemCodenotIn from DUMMY;
		return;
	END IF;
	-- Save
	INSERT INTO APZ_LIN1 ("LineID", "ItemCode", "ItemName", "CycleTime", "TotalTime", "NumberOfMch") (SELECT a."LineID", a."ItemCode", a."ItemName", a."CycleTime", a."TotalTime", a."NumberOfMch" FROM #APZ_MRP_LINESData a);
	
	select '' "message" from DUMMY;
END



