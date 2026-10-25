
CREATE PROCEDURE APZ_Voucher_Saves(in tranType varchar(1) default '')
AS 
BEGIN
	
	declare pinCode nvarchar(50) = '';
	declare fromDate timestamp;
	declare toDate timestamp;
	declare chkExists varchar(2) = '';
	
	create local temporary table #APZ_VOURCHERTemp("PinCode" nvarchar(50) NOT NULL, "PinName" nvarchar(250) NULL,  "Type" char(1) NULL,  "Value" decimal(18, 3) NULL, "Currency" varchar(10) NULL, "SpecialBp" nvarchar(50) NULL, "SpecialBpGroup" nvarchar(50) NULL, "Status" char(1) NULL, "FromDate" timestamp NULL,  "ToDate" timestamp NULL);
	exec 'drop table #APZ_VOURCHERTemp';
	exec 'create local temporary table #APZ_VOURCHERTemp as (select * from #APZ_VOURCHER)';
	
	create local temporary table #APZ_VOURCHER1Temp("PinCode" nvarchar(50) NOT NULL, "LineNum" integer NOT NULL, "Date" timestamp NULL, "Pic" nvarchar(250) NULL, "Type" char(1) NULL, "LinkedTransId" integer NULL, "TransType" char(1) NULL, "Remark" nvarchar(250) NULL);
	exec 'drop table #APZ_VOURCHER1Temp';
	exec 'create local temporary table #APZ_VOURCHER1Temp as (select * from #APZ_VOURCHER1)';
	
	select (select top 1 "FromDate" from #APZ_VOURCHERTemp) into fromDate from DUMMY;
	select (select top 1 "ToDate" from #APZ_VOURCHERTemp) into toDate from DUMMY;
	select (select top 1 "PinCode" from #APZ_VOURCHERTemp) into pinCode from DUMMY;
	
	if :tranType = '' or (:tranType <> 'A' and :tranType <> 'U' and :tranType <> 'D')  then
		select 101 as "ErrorCode", 'Invalid action type' as "ErrorMessage" from DUMMY;
		return;
	else
		if :tranType = 'U' then
			delete from APZ_VOURCHER1 where "PinCode" = :pinCode;
			delete from APZ_VOURCHER where "PinCode" = :pinCode;
		elseif :tranType = 'A' then
			select (select 1 from APZ_VOURCHER a inner join #APZ_VOURCHERTemp b on a."PinCode" = b."PinCode") into chkExists from DUMMY;
			if :chkExists = '1' then
				select 121 as "ErrorCode", 'Voucher is overlapping with an existing Voucher' as "ErrorMessage" from DUMMY;
				return;
			end if;
		end if;
	end if;
	
	select (select 1 from #APZ_VOURCHERTemp group by "PinCode" having count(1) > 1) into chkExists from DUMMY;
	if :chkExists = '1' then
		select 122 as "ErrorCode",'Voucher is overlapping with an existing Voucher' as "ErrorMessage" from DUMMY;
		return;
	end if;	 
	
	select (select 1 from #APZ_VOURCHER1Temp where "Date" < :fromDate or "Date" > :toDate) into chkExists from DUMMY;
	if :chkExists = '1' then
		select 123 as "ErrorCode",'The voucher has no effect' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	select (select 1 from #APZ_VOURCHER1Temp where ifnull("Date", '') = '') into chkExists from DUMMY;
	if :chkExists = '1' then
		select 124 as "ErrorCode",'Date cannot empty value' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	insert into APZ_VOURCHER select * from #APZ_VOURCHERTemp;
	insert into APZ_VOURCHER1 select :pinCode as "PinCode", ROW_NUMBER() OVER(ORDER BY "PinCode") as "LineNum", "Date", "Pic", "Type", "LinkedTransId", "TransType", "Remark" from #APZ_VOURCHER1Temp;
	
	select 125, '' from DUMMY;
END;



