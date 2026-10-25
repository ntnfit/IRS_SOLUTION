
Create procedure Apz_PromotionCondition_SaveProcess (in type char default '')
AS
BEGIN 
	declare id INT = 0;
	declare code NVARCHAR(50) = '';

	-- kiểm tra bảng tạm đã khởi tạo chưa
	if not exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_OPCN' and schema_name = current_schema) then
		select 404, 'Temp data not exists' from dummy;
		return;
	end if;
	
	create local temporary table #APZ_OPCNDATA as (select top 0 * from APZ_OPCN);
	exec 'drop table #APZ_OPCNDATA';
	exec 'create local temporary table #APZ_OPCNDATA as (select * from #APZ_OPCN);';
	
	select top 1 "Code" into code default '' from #APZ_OPCNDATA; 
	if :code = '' then
		select 405, 'Code cannot be empty' from dummy;
	end if;

	-- Nếu delete
	if :type = 'D' then
		-- todo: check condition có nằm trong condition rule nào ko
		
		-- nếu ko thì xóa
		delete from APZ_OPCN where "Code" = :code;
		delete from APZ_PCN1 where "Code" = :code;
		
		select 0, '' from dummy;
		return;
	end if; 

	if :type = 'A' then
		id := ifnull((select max("DocEntry") + 1 from APZ_OPCN), 1);
		update #APZ_OPCNDATA set "UpdateDate" = null, "CreateDate" = current_date, "DocEntry" = :id;
	elseif :type = 'U' then
		select top 1 "DocEntry" into id default 0 from #APZ_OPCNDATA;
		delete from APZ_OPCN where "DocEntry" = :id;
		delete from APZ_PCN1 where "DocEntry" = :id;
		update #APZ_OPCNDATA set "UpdateDate" = current_date;
	end if;

	if exists (select 1 from APZ_OPCN a inner join #APZ_OPCNDATA b on a."Code" = b."Code") then
		select 123, 'Object is overlapping with an existing object' from dummy;
		return;
	end if;
	
	insert into APZ_OPCN select * from #APZ_OPCNDATA;

	if exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_PCN1' and schema_name = current_schema) then
		declare rowCount, i int;
		create local temporary table #APZ_PCN1DATA as (select top 0 * from APZ_PCN1);
		exec 'drop table #APZ_PCN1DATA';
		exec 'create local temporary table #APZ_PCN1DATA as (select * from #APZ_PCN1);';
		
		update #APZ_PCN1DATA set "DocEntry" = :id, "Code" = :code, "LineId" = null;
		select count(1) into rowCount default 0 from #APZ_PCN1DATA;
		for i in 1..:rowCount do
			update top 1 #APZ_PCN1DATA set "LineId" = :i where "LineId" is null;
		end for;
		
		insert into APZ_PCN1 select * from #APZ_PCN1DATA;
	end if;
		
	SELECT 0, '' from dummy;
END




