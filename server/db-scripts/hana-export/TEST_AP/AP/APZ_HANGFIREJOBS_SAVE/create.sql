

CREATE PROCEDURE APZ_HangfireJobs_Save
AS
BEGIN
	declare id int;
	declare code nvarchar(254);
	declare cron nvarchar(100);
	declare storeName nvarchar(254);
	
	create local temporary table #JOBLocal as (select top 0 * from APZ_OJOB);
	exec 'drop table #JOBLocal;';
	exec 'create local temporary table #JOBLocal as (select * from #APZ_OJOB);';
	
	create local temporary table #RECLocal as (select top 0 * from APZ_OREC);
	exec 'drop table #RECLocal;';
	exec 'create local temporary table #RECLocal as (select * from #APZ_OREC);';
	
	select top 1 "JobId", "JobCode", "CronString", "StoreName" into id, code, cron, storeName default 0, '', '', '' from #JOBLocal;
	
	--Check code rỗng
	if :code = '' then
		select 0 as "ErrorNumber", N'Vui lòng nhập mã Job!' as "ErrorMessage" from dummy;
		return;
	end if;
	-- check cron rỗng
	if :cron = '' then
		select 0 as "ErrorNumber", N'Chuỗi cron không hợp lệ!' as "ErrorMessage" from dummy;
		return;
	end if;
	-- check tên store
	if :storeName = '' then
		select 0 as "ErrorNumber", N'Vui lòng nhập tên Store!' as "ErrorMessage" from dummy;
		return;
	end if;
	
	--id = 0 => tạo mới thì sẽ check xem JobCode có trùng không
	if ifnull(:id,0) = 0 then 
		 if exists(select top 1 1 from APZ_OJOB where "JobCode" = :code) then
			select 0 as "ErrorNumber", N'Mã job đã được sử dụng!' as "ErrorMessage" from dummy;
			return;
		end if;
		id := ifnull((select Max("JobId") from APZ_OJOB),0) + 1; 
	end if;
	
	update #JOBLocal set "JobId" = :id;
	update #RECLocal set "JobId" = :id;
	
	delete from APZ_OJOB where "JobId" = :id;
	delete from APZ_OREC  where "JobId" = :id;

	insert into APZ_OJOB select * from #JOBLocal;
	insert into APZ_OREC select * from #RECLocal;
	
	select :id,'' from dummy;
	
END




