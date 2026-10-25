Create Procedure Apz_PromotionProgram_SaveProcess (in type char(1) default '')
AS
BEGIN 
	declare id int = 0;
	declare code nvarchar(50) = '';

	-- kiểm tra bảng tạm đã khởi tạo chưa
	if not exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_OPPR' and schema_name = current_schema) then
		select 404, 'Temp data not exists' from dummy;
		return;
	end if;
	
	create local temporary table #APZ_OPPRDATA as (select top 0 * from APZ_OPPR);
	exec 'drop table #APZ_OPPRDATA';
	exec 'create local temporary table #APZ_OPPRDATA as (select * from #APZ_OPPR);';
	
	select top 1 "Code" into code default '' from  #APZ_OPPRDATA;
	
	if :code = '' then
		select 405, 'Code cannot be empty' from dummy;
		return;
	end if;
	
	select top 1 "DocEntry" into id default 0 from #APZ_OPPRDATA;

	-- Nếu delete
	if type = 'D' then
		-- todo: check KPI Indicator có nằm trong nghiệp vụ # nào ko
		
		-- nếu ko thì xóa
		delete from APZ_OPPR where "DocEntry" = :id;
		delete from APZ_PPR1 where "DocEntry" = :id;
		delete from APZ_PPR2 where "DocEntry" = :id;
		
		select :id, '' from dummy;
		return;
	elseif type = 'C' then
		delete from APZ_OPPR where "DocEntry" = :id;
		update #APZ_OPPRDATA set "UpdateDate" = current_date;
		insert into APZ_OPPR select * from #APZ_OPPRDATA;
		select :id, '' from dummy;
		return;
	elseif type = 'A' then
		id := ifnull((select max("DocEntry") + 1 from APZ_OPPR), 1);
		update #APZ_OPPRDATA set "UpdateDate" = null, "CreateDate" = current_date, "DocEntry" = :id;
	elseif :type = 'U' then
		delete from APZ_OPPR where "DocEntry" = :id;
		delete from APZ_PPR1 where "DocEntry" = :id;
		delete from APZ_PPR2 where "DocEntry" = :id;
		
		update #APZ_OPPRDATA set "UpdateDate" = current_date;
				
	end if;

	if exists (select 1 from APZ_OPPR a inner join #APZ_OPPRDATA b on a."Code" = b."Code") then
		select 123, 'Object is overlapping with an existing object' from dummy;
		return;
	end if;
	
	insert into APZ_OPPR select * from #APZ_OPPRDATA;
	
	if exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_PPR1' and schema_name = current_schema) then
		declare rowCount, i int;
		create local temporary table #APZ_PPR1DATA as (select top 0 * from APZ_PPR1);
		exec 'drop table #APZ_PPR1DATA';
		exec 'create local temporary table #APZ_PPR1DATA as (select * from #APZ_PPR1);';
		
		update #APZ_PPR1DATA set "DocEntry" = :id, "Code" = :code, "LineId" = null;
		select count(1) into rowCount default 0 from #APZ_PPR1DATA;
		for i in 1..:rowCount do
			update top 1 #APZ_PPR1DATA set "LineId" = :i where "LineId" is null;
		end for;
		
		insert into APZ_PPR1 select * from #APZ_PPR1DATA;
	end if;
	
	if exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_PPR2' and schema_name = current_schema) then
		declare rowCount, i int;
		create local temporary table #APZ_PPR2DATA as (select top 0 * from APZ_PPR2);
		exec 'drop table #APZ_PPR2DATA';
		exec 'create local temporary table #APZ_PPR2DATA as (select * from #APZ_PPR2);';
		
		update #APZ_PPR2DATA set "DocEntry" = :id, "Code" = :code, "LineId" = null;
		select count(1) into rowCount default 0 from #APZ_PPR2DATA;
		for i in 1..:rowCount do
			update top 1 #APZ_PPR2DATA set "LineId" = :i where "LineId" is null;
		end for;
		
		insert into APZ_PPR2 select * from #APZ_PPR2DATA;
	end if;

	select :id, '' from dummy;
END



