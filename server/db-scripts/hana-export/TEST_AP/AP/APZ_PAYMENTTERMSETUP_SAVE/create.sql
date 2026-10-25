
create procedure APZ_PaymentTermSetUp_Save
as 
begin
	declare rowCount int=0;
	declare maxId int=0;
	declare dulpValue varchar(100)='';
	create local temporary table #APZ_PAYMENTTERMLOCAL
	(
		"BslineDate" NVARCHAR(1),
		"CrdMthd" NVARCHAR(1),
		"CredLimit" DECIMAL(21,3),
		"CshRelev" NVARCHAR(1),
		"DataSource" NVARCHAR(1),
		"DiscCode" NVARCHAR(20),
		"DunningCod" NVARCHAR(20),
		"ExtraDays" SMALLINT,
		"ExtraMonth" SMALLINT,
		"GroupNum" SMALLINT,
		"InstNum" SMALLINT,
		"LatePyChrg" DECIMAL(21,3),
		"ListNum" SMALLINT,
		"NumOfPmnts" SMALLINT,
		"ObligLimit" DECIMAL(21,3),
		"OpenRcpt" NVARCHAR(1),
		"PayDuMonth" NVARCHAR(1),
		"Payment1" DECIMAL(21,3),
		"Payments" NVARCHAR(1),
		"PaymntsNum" SMALLINT,
		"PymntGroup" NVARCHAR(100),
		"TolDays" SMALLINT,
		"UserSign" SMALLINT,
		"VATFirst" NVARCHAR(1),
		"VolumDscnt" DECIMAL(21,3)		
	);
	exec 'drop table #APZ_PAYMENTTERMLOCAL;';
	exec 'create local temporary table #APZ_PAYMENTTERMLOCAL as (select * from "#APZ_PAYMENTTERM");';
	
	select(select count(1) from #APZ_PAYMENTTERMLOCAL) into rowCount from DUMMY;
	if rowCount = 0 then
		select '01' as "ErrorCode", 'Source table has no rows to save.' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	select (select count("PymntGroup") as "Count" from #APZ_PAYMENTTERMLOCAL group by "PymntGroup" having(count("PymntGroup") > 1)) into rowCount from DUMMY;
	if rowCount > 1 then
		select (select top 1 "PymntGroup" from #APZ_PAYMENTTERMLOCAL group by "PymntGroup" having(count("PymntGroup") > 1)) into dulpValue from DUMMY;
		select '02' as "ErrorCode", 'Duplicate value: ' || :dulpValue from DUMMY;
		return;
	end if;
	
	select(select count("PymntGroup")  as "Count" from #APZ_PAYMENTTERMLOCAL where "PymntGroup"='') into rowCount from DUMMY;
	if rowCount > 0 then
		select 'Payment Terms Code can''t empty value' from DUMMY;
		return;
	end if;
	
	
	select (ifnull((select max("GroupNum") as "MaxId" from OCTG), 0)) into maxId from DUMMY;
	select maxId + 1 into maxID from DUMMY;
	
	delete from OCTG;
	update #APZ_PAYMENTTERMLOCAL set "GroupNum"=maxId+1 where ifnull("GroupNum",0)=0;
	update ONNM set "AutoKey"=maxId where "ObjectCode"='40';
	
	insert into OCTG select * from #APZ_PAYMENTTERMLOCAL;
end;




