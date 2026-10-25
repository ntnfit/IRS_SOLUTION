

CREATE PROCEDURE APZ_Coupon_Saves(in tranType varchar(1) default '')
AS
BEGIN
	declare code nvarchar(50) = '';
	declare chkExists varchar(2) = '';
	
	--Create new temp table
	create local temporary table #APZ_COUPONTemp("CouponCode" nvarchar(50) NOT NULL, "CouponName" nvarchar(250) NULL, "ItemCode" nvarchar(50) NULL, "Status" char(1) NULL, "Remark" nvarchar(5000) NULL, "ItemName" nvarchar(5000) null);
	exec 'drop table #APZ_COUPONTemp';
	exec 'create local temporary table #APZ_COUPONTemp as (select * from #APZ_COUPON)';
	
	create local temporary table #APZ_COUPON1Temp("CouponCode" nvarchar(50) NOT NULL, "LineNum" integer NOT NULL, "PinCode" nvarchar(50) NULL, "CouponValue" decimal(18, 3) NULL, "Currency" varchar(10) NULL, "OneTime" char(1) NULL,  "BPCode" nvarchar(50) NULL,  "IssueDate" timestamp NULL, "IssueTransId" integer NULL, "IssueTransType" char(1) NULL, "Remain" nvarchar(50) NULL);
	exec 'drop table #APZ_COUPON1Temp';
	exec 'create local temporary table #APZ_COUPON1Temp as (select * from #APZ_COUPON1)';
	
	
	select (select top 1 ifnull("CouponCode", '') from #APZ_COUPONTemp) into code from DUMMY;
	
	if :tranType = '' or (:tranType <> 'A' and :tranType <> 'U' and :tranType <> 'D')  then
		select 120 as "ErrorCode", 'Invalid action type' as "ErrorMessage" from DUMMY;
		return;
	else
		--Update record
		if :tranType = 'U' then
			delete from APZ_COUPON1 where "CouponCode" = :code;
			delete from APZ_COUPON where "CouponCode" = :code;	
			update #APZ_COUPON1Temp set "CouponCode" = :code where "CouponCode" is null;
		elseif :tranType = 'A' then
			--Check record exists
			select (select 1 from APZ_COUPON a inner join #APZ_COUPONTemp b on a."CouponCode" = b."CouponCode") into chkExists from DUMMY;
			if :chkExists = '1' then
				select 121 as "ErrorCode" ,'Coupon is overlapping with an existing Coupon' as "ErrorMessage" from DUMMY;
				return;
			end if;
		end if;
	end if;
	
	--Check CouponCode dulplicated
	select (select 1 from #APZ_COUPONTemp group by "CouponCode" having count(1) > 1) into chkExists from DUMMY;
	if :chkExists = '1' then
		select 122 as "ErrorCode" ,'Coupon is overlapping with an existing Coupon' as "ErrorMessage" from DUMMY;
		return;
	end if;
		
	--Check PinCode duplicated
	select (select 1 from #APZ_COUPON1Temp group by "PinCode" having count(1) > 1) into chkExists from DUMMY;
	if :chkExists = '1' then
		select 123 as "ErrorCode", 'PinCode is overlapping with an existing Pincode' as "ErrorMessage" from DUMMY;
		return;
	end if;
		
	--Check PinCode not exists
	select (select 1 from #APZ_COUPON1Temp where ifnull("PinCode", '') = '') into chkExists from DUMMY;
	if :chkExists = '1' then
		select 124 as "ErrorCode", 'PinCode cannot empty value' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	--Insert data
	update #APZ_COUPON1Temp set "CouponCode" = :code where "CouponCode" is null;
	insert into APZ_COUPON select "CouponCode", "CouponName", "ItemCode", "Status", "Remark" from #APZ_COUPONTemp;
	insert into APZ_COUPON1 select :code as "CouponCode", ROW_NUMBER() OVER(ORDER BY "CouponCode") as "LineNum", "PinCode", "CouponValue", "Currency", "OneTime", "BPCode", "IssueDate", "IssueTransId", "IssueTransType", "Remain" from #APZ_COUPON1Temp;
	
	select 125, '' from DUMMY;
END;



