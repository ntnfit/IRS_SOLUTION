
CREATE PROCEDURE apz_d_promotion_benefit_process(in UserCode nvarchar(50), in transtype char(1))
as
begin
	declare error int =0;
	declare error_message nvarchar(254);
	declare UserID int;
	declare Code nvarchar(50);
	select ifnull((select USERID FROM OUSR WHERE USER_CODE=:UserCode),0) into UserID from DUMMY;
	
	create local temporary table #APZ_D_OPBFData as (select top 0 b.* from (select 1 from DUMMY) a left join APZ_D_OPBF b ON 1=1);
	exec 'insert into #APZ_D_OPBFData select * from #APZ_D_OPBF';
	
	create local temporary table #APZ_D_PBF1Data as (select top 0 b.* from (select 1 from DUMMY) a left join APZ_D_PBF1 b ON 1=1);
	exec 'insert into #APZ_D_PBF1Data select * from #APZ_D_PBF1';
	
	create local temporary table #APZ_D_PBF2Data as (select top 0 b.* from (select 1 from DUMMY) a left join APZ_D_PBF2 b ON 1=1);
	exec 'insert into #APZ_D_PBF2Data select * from #APZ_D_PBF2';
	
	create local temporary table #APZ_D_PBF3Data as (select top 0 b.* from (select 1 from DUMMY) a left join APZ_D_PBF3 b ON 1=1);
	exec 'insert into #APZ_D_PBF3Data select * from #APZ_D_PBF3';
	
	if(:UserID=0)
	THEN
		error=:error+1;
		error_message=N'Người dùng không tồn tại';
		select :error,:error_message from DUMMY;
		return;
	end IF;
	
	if(:transtype='A')
	THEN
		if exists (select 1 from APZ_D_OPBF where "Code" in (select "Code" from #APZ_D_OPBFData))
		THEN
			error=:error+2;
			error_message=N'Mã đã tồn tại';
			select :error,:error_message from DUMMY;
			return;
		end IF;
	end IF;
	
	select ifnull((select "Code" from #APZ_D_OPBFData),'') into Code from DUMMY;
	
	if(:Code='')
	THEN
			error=:error+3;
			error_message=N'Mã không được để trống';
			select :error,:error_message from DUMMY;
			return;
	end IF;
	
	update #APZ_D_PBF1Data set "Code"=:Code;
	update #APZ_D_PBF2Data set "Code"=:Code;
	update #APZ_D_PBF3Data set "Code"=:Code;

	delete from APZ_D_OPBF where "Code"=:Code;
	delete from APZ_D_PBF1 where "Code"=:Code;
	delete from APZ_D_PBF2 where "Code"=:Code;
	delete from APZ_D_PBF3 where "Code"=:Code;

	insert into APZ_D_OPBF 
	select * from #APZ_D_OPBFData;

	insert into APZ_D_PBF1 
	select * from #APZ_D_PBF1Data;

	insert into APZ_D_PBF2 
	select * from #APZ_D_PBF2Data where ifnull("ItemCode",'')<>'';

	insert into APZ_D_PBF3 
	select * from #APZ_D_PBF3Data;

	select :Code,'' from DUMMY;

end



