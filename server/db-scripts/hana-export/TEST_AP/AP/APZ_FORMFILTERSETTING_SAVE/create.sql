
CREATE procedure APZ_FormFilterSetting_Save(in obj nvarchar(50))
as 
begin
	declare docEntry int;
	
	create local temporary table #Filter as (select top 0 * from APZ_FILTERSETTING);
	exec 'insert into #Filter select * from #APZ_FILTERSETTING;';
	create local temporary table #FilterLine as (select top 0 * from APZ_FILTERSETTINGLINE);
	exec 'insert into #FilterLine select * from #APZ_FILTERSETTINGLINE where trim("Code") is not null;';
	create local temporary table #FilterLine2 as (select top 0 * from APZ_FILTERSETTINGLINE2);
	exec 'insert into #FilterLine2 select * from #APZ_FILTERSETTINGLINE2 where trim("TableName") is not null and trim("Code") is not null and trim("Name") is not null;';
	
	select (select top 1 "DocEntry" from #Filter) into docEntry from dummy;
	
	if :docEntry > 0 then
	begin
		delete FROM APZ_FILTERSETTINGLINE where "DocEntry" = :docEntry;
		delete FROM APZ_FILTERSETTINGLINE2 where "DocEntry" = :docEntry;
		delete FROM APZ_FILTERSETTING where "ObjType"= :obj and "DocEntry" = :docEntry;
	end;
	else
	begin
		select ifnull((select max("DocEntry") from APZ_FILTERSETTING), 0) + 1 into docEntry from dummy;
		update #Filter set "DocEntry" = :docEntry;
		update #FilterLine set "DocEntry" = :docEntry;
		update #FilterLine2 set "DocEntry" = :docEntry;
	end;
	end if;
	
	insert into APZ_FILTERSETTING select * from #Filter;
	insert into APZ_FILTERSETTINGLINE select "DocEntry",(ROW_NUMBER() over(order by "DocEntry")) "LineNum","Code", "Name", "Active", "Remarks" from #FilterLine;
	insert into APZ_FILTERSETTINGLINE2 select * from #FilterLine2;
end




