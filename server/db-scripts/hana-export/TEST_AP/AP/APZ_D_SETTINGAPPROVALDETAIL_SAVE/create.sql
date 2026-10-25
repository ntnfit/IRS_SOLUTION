
Create PROCEDURE APZ_D_SettingApprovalDetail_Save(in typeName char(1) default '')
as
BEGIN
	Declare Id int =0;
	create local temporary table #APZ_D_OSADDATA as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_OSAD b on 1=2);
	exec 'insert into #APZ_D_OSADDATA select * from #APZ_D_OSAD';
	
	create local temporary table #APZ_D_SAD1DATA as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_SAD1 b on 1=2);
	exec 'insert into #APZ_D_SAD1DATA select * from #APZ_D_SAD1';
	
	create local temporary table #APZ_D_SAD2DATA as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_D_SAD2 b on 1=2);
	exec 'insert into #APZ_D_SAD2DATA select * from #APZ_D_SAD2';
	
	if :typeName='U'
	THEN
		select top 1 "DocEntry" into id from #APZ_D_OSADDATA;
		delete from APZ_D_SAD2 where "DocEntry" =:Id;
		delete from APZ_D_SAD1 where "DocEntry" =:Id;
		delete from APZ_D_OSAD where "DocEntry"=:Id;
	else
		select ifnull((select max("DocEntry") from APZ_D_OSAD), 0)+1 into id from DUMMY;
		update #APZ_D_OSADDATA set "DocEntry" = :Id;
	end if;
	
	UPDATE #APZ_D_SAD2DATA set "DocEntry" = :Id;
	update #APZ_D_SAD1Data set "DocEntry" = :Id;
	insert into APZ_D_SAD2 select * from #APZ_D_SAD2DATA;
	insert into APZ_D_SAD1 select * from #APZ_D_SAD1DATA;
	insert into APZ_D_OSAD select * from #APZ_D_OSADDATA;
	select :Id,'' from DUMMY;
end



