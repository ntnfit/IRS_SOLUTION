
CREATE procedure apz_process_MRP_QUOTA_ARR(in TypeAdd char(1) default '')
as
BEGIN 
	
	DECLARE error NVARCHAR(4000) ='';
	DECLARE error_message NVARCHAR(4000)='';
	
	declare maxPeriodID int = 0;
	declare itemCode nvarchar(50)='';
	
	select ifnull((Select max("PeriodID") From APZ_IQA1),0) into maxPeriodID from DUMMY;
	
	--************************
	-- ************************ ADD NEW
	--************************
	
	create local temporary table #APZ_MRP_QUOTA_ARRData as (select top 0 * from APZ_OIQA);
	exec 'drop table #APZ_MRP_QUOTA_ARRData;';
	exec 'create local temporary table #APZ_MRP_QUOTA_ARRData as (select * from #APZ_MRP_QUOTA_ARR);';
	
	create local temporary table #APZ_MRP_QUOTA_LINE_ARRData as (select top 0 *, cast(null as int) ROWNUM from APZ_IQA1);
	exec 'drop table #APZ_MRP_QUOTA_LINE_ARRData;';
	exec 'create local temporary table #APZ_MRP_QUOTA_LINE_ARRData as (select *, row_number() over(order by "ItemCode") ROWNUM from #APZ_MRP_QUOTA_LINE_ARR);';
	
	if :TypeAdd='A'
	THEN
		-- ************ BEGIN Update period ID in Line
		-- Update period ID
		UPDATE #APZ_MRP_QUOTA_LINE_ARRData SET "PeriodID" = :maxPeriodID + ROWNUM;
		-- ************ END Update period ID in Line
		
		-- ************BEGIN Save
		-- DOC
		insert into APZ_OIQA ("ItemCode" ,"ItemName" ,"MinQty") 
			select a."ItemCode" ,a."ItemName" ,a."MinQty" from #APZ_MRP_QUOTA_ARRData a;
		-- LINE
		insert into APZ_IQA1 ("ItemCode" ,"PeriodID" ,"FromDate" ,"ToDate" ,"Remarks") 
			select a."ItemCode" ,a."PeriodID"  ,a."FromDate" ,a."ToDate" ,a."Remarks" from #APZ_MRP_QUOTA_LINE_ARRData a;
		-- ************END Save
		
		 --************************
		-- ************************ END ADD NEW
		--************************
	
	--************************
	-- ************************ BEGIN UPDATE
	--************************
	
	ELSEIF  :TypeAdd='U'
	THEN
	
		-- Update period ID
		
		-- ************ END Update period ID in Line
		
		UPDATE #APZ_MRP_QUOTA_LINE_ARRData SET "PeriodID" = :maxPeriodID + ROWNUM Where "PeriodID" =-1;
		-- ************BEGIN Save
		-- DOC
		Update APZ_OIQA SET "ItemName" =(select t."ItemName" from #APZ_MRP_QUOTA_ARRData t)	,"MinQty"=(select t."MinQty" from #APZ_MRP_QUOTA_ARRData t) WHERE "ItemCode"=(select t."ItemCode" from #APZ_MRP_QUOTA_ARRData t);
		-- LINE
		select ifnull((Select top 1 "ItemCode" From #APZ_MRP_QUOTA_ARRData),'') into itemCode from DUMMY;
		-- Delete all period of this ItemCode
		DELETE from APZ_IQA1  Where "ItemCode"=:itemCode;
		
		
		insert into APZ_IQA1 ("ItemCode" ,"PeriodID" ,"FromDate" ,"ToDate" ,"Remarks") 
			select a."ItemCode" ,a."PeriodID" ,a."FromDate" ,a."ToDate" ,a."Remarks" from #APZ_MRP_QUOTA_LINE_ARRData a;
		
		-- Delete all date quota of periods that have been removed
		DELETE FROM APZ_IQA2  Where "PeriodID" not in (Select "PeriodID" From APZ_IQA1);
		-- ************END Save
		
		--************************
		-- ************************ END UPDATE
		--************************
	END IF;
END



