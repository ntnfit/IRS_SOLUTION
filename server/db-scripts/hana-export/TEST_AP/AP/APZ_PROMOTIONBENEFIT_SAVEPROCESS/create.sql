
Create PROCEDURE Apz_PromotionBenefit_SaveProcess (IN type CHAR(1) DEFAULT '')
AS
BEGIN 
	declare id int = 0;
	declare code nvarchar(50) = '';
	
	-- kiểm tra bảng tạm đã khởi tạo chưa
	if not exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_OPBF' and schema_name = current_schema) then
		select 404, 'Temp data not exists' from dummy;
		return;
	end if;
	
	create local temporary table #APZ_OPBFDATA as (select top 0 * from APZ_OPBF);
	exec 'drop table #APZ_OPBFDATA';
	exec 'create local temporary table #APZ_OPBFDATA as (select * from #APZ_OPBF);';
	
	select top 1 "Code" into code default '' from #APZ_OPBFDATA;
	if :code = '' then
		select 405, 'Code cannot be empty' from dummy;
		return;
	end if;

	-- Nếu delete
	if :type = 'D' then
		-- todo: check benefit có nằm trong nghiệp vụ # nào ko
		
		-- nếu ko thì xóa
		delete from APZ_OPBF where "Code" = :code;
		delete from APZ_PBF1 where "Code" = :code;
		
		select 0, '' from dummy;
		return;
	elseif :type = 'A' then
		id := ifnull((select max("DocEntry") + 1 from APZ_OPBF), 1);
		update #APZ_OPBFDATA set "UpdateDate" = null, "CreateDate" = current_date, "DocEntry" = :id;
	elseif :type = 'U' then
		select top 1 "DocEntry" into id default 0 from #APZ_OPBFDATA;
		delete from APZ_OPBF where "DocEntry" = :id;
		delete from APZ_PBF1 where "DocEntry" = :id;
		update #APZ_OPBFDATA set "UpdateDate" = current_date;
	end if;

	if exists (select 1 from APZ_OPBF a inner join #APZ_OPBFDATA b on a."Code" = b."Code") then
		select 123, 'Object is overlapping with an existing object' from dummy;
		return;
	end if;
	
	insert into APZ_OPBF select * from #APZ_OPBFDATA;

	if exists(select 1 from M_TEMPORARY_TABLES a Where TABLE_NAME = '#APZ_PBF1' and schema_name = current_schema) then
		declare rowCount, i int;
		create local temporary table #APZ_PBF1DATA as (select top 0 * from APZ_PBF1);
		exec 'drop table #APZ_PBF1DATA';
		exec 'create local temporary table #APZ_PBF1DATA as (select * from #APZ_PBF1);';
		
		update #APZ_PBF1DATA set "DocEntry" = :id, "Code" = :code, "LineId" = null;
		select count(1) into rowCount default 0 from #APZ_PBF1DATA;
		for i in 1..:rowCount do
			update top 1 #APZ_PBF1DATA set "LineId" = :i where "LineId" is null;
		end for;
		
		insert into APZ_PBF1 select * from #APZ_PBF1DATA;
	end if;
		
	SELECT 0, '' from dummy;
END





