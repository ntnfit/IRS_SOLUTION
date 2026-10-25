
CREATE procedure apz_process_MRP_QUOTA_DATEARR
as
BEGIN
	DECLARE error NVARCHAR(4000) ='';
	DECLARE error_message NVARCHAR(4000)='';
	
	-- ****Validate 
	declare invalidCode nvarchar(4000)= '';
	
	create local temporary table #APZ_MRP_QUOTA_DATEARRDATA as (select top 0 * from APZ_IQA2);
	exec 'drop table #APZ_MRP_QUOTA_DATEARRDATA;';
	exec 'create local temporary table #APZ_MRP_QUOTA_DATEARRDATA as (select * from #APZ_MRP_QUOTA_DATEARR);';
	
	select ifnull((Select top 1 "CardCode" From #APZ_MRP_QUOTA_DATEARRDATA T WHERE T."CardCode" NOT IN (SELECT "CardCode" FROM OCRD)),'') into invalidCode from DUMMY;
	
	if :invalidCode<>''
	THEN
		SELECT 'Invalid BP code -[' || :invalidCode || ']' from DUMMY;
		RETURN;
	END IF;
	-- ****Validate 
	
	-- Save
	INSERT INTO APZ_IQA2 ("PeriodID", "CardCode", "CardName", "LeadTime", "Per", "MaxQty", "BaseQty", "MaxLot", "MinLot", "Remarks", "OrdrIntrvl", "OrdrMulti", "Substitute") 
		SELECT a."PeriodID", a."CardCode", a."CardName", a."LeadTime", a."Per", a."MaxQty", a."BaseQty", a."MaxLot", a."MinLot", a."Remarks", "OrdrIntrvl", "OrdrMulti", "Substitute" FROM #APZ_MRP_QUOTA_DATEARRDATA a;
	
	
	select :error_message as "message" FROM DUMMY;
END



