

CREATE PROCEDURE APZ_Cash_Saves(in tranType varchar(1) default '')
AS
BEGIN
	declare code nvarchar(50) = '';
	declare checkExst varchar(2) = '';
	
	/* Create new temporary table */
	create local temporary table #APZ_CASHTmp("CashCode" nvarchar(50) not null, "CashName" nvarchar(250) null, "Type" char(1) null, "Group" nchar(1) null, "Currency" varchar(10) null, "Default" char(1) null, "Remark" nvarchar(5000) null);
	exec 'drop table #APZ_CASHTmp';
	exec 'create local temporary table #APZ_CASHTmp as (select * from #APZ_CASH)';
	
	create local temporary table #APZ_CASH1Tmp("CashCode" nvarchar(50) not null, "LineNum" integer not null, "InNumber" decimal(18, 3) null, "InWork" nvarchar(5000) null, "Remark" nvarchar(5000) null, "Note" nvarchar(5000) null);
	exec 'drop table #APZ_CASH1Tmp';
	exec 'create local temporary table #APZ_CASH1Tmp as (select * from #APZ_CASH1)';
	
	select (select top 1 "CashCode" from #APZ_CASHTmp) into code from DUMMY;
	if :tranType = '' or (:tranType <> 'A' and :tranType <> 'U' and :tranType <> 'D')  then
		select 101 as "ErrorCode", 'Invalid action type' as "ErrorMessage" from DUMMY;
		return;
	else
		if :tranType = 'U' then
			--Update record
			update #APZ_CASHTmp set "Default" = 'N' where "Default" = 'Y';
			delete from APZ_CASH1 where "CashCode" = :code;
			delete from APZ_CASH where "CashCode" = :code;
		elseif :tranType = 'A' then
			--Check CashCode exists
			select (select 1 from APZ_CASH a inner join #APZ_CASHTmp b on a."CashCode" = b."CashCode") into checkExst from DUMMY;
			if :checkExst = '1' then
				select '121' as "ErrorCode", 'Cash is overlapping with an existing Cash' as "ErrorMessage" from DUMMY;
				return;
			end if;
		end if;
	end if;
	
	--Check CashCode dulplicate in temp table
	select (select 1 from #APZ_CASHTmp group by "CashCode" having count(1) > 1) into checkExst from DUMMY;
	if :checkExst = '1' then
		select '122' as "ErrorCode", 'Cash is overlapping with an existing Cash' as "ErrorMessage" from DUMMY;
		return;
	end if;
		
	--Check InWork and Linenum valid
	select(select distinct 1 from #APZ_CASH1Tmp a inner join #APZ_CASH1Tmp b on a."InWork" = b."InWork" and a."LineNum" != b."LineNum") into checkExst from DUMMY;
	if :checkExst = '1' then
		select '123' as "ErrorCode", 'Cash detail is overlapping with an existing Cash detail' as "ErrorMessage" from DUMMY;
		return;
	end if;
		
	--Check InNumber exists
	select (select 1 from #APZ_CASH1Tmp where ifnull("InNumber", 0) = 0) into checkExst from DUMMY;
	if :checkExst = '1' then
		select '124' as "ErrorCode", 'InNumber could not null' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	--Insert data
	insert into APZ_CASH select * from #APZ_CASHTmp;
	insert into APZ_CASH1 select :code as "CashCode", ROW_NUMBER() OVER(ORDER BY "CashCode") as "LineNum", "InNumber", "InWork", "Remark", "Note" from #APZ_CASH1Tmp;
	
	select 0, '' from DUMMY;	
END;




