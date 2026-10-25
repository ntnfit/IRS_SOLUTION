CREATE PROCEDURE APZ_D_SettingReport_Save(in actionType char(1) default '')
AS
BEGIN

	declare id int = -1;
	
	create local temporary table #APZ_D_OSRTLocal as (select top 0 * from APZ_D_OSRT);
	exec 'drop table #APZ_D_OSRTLocal';
	exec 'create local temporary table #APZ_D_OSRTLocal as (select * from "#APZ_D_OSRT")';
	
	create local temporary table #APZ_D_SRT1Local as (select top 0 * from APZ_D_SRT1);
	exec 'drop table #APZ_D_SRT1Local';
	exec 'create local temporary table #APZ_D_SRT1Local as (select * from "#APZ_D_SRT1")';
	
	create local temporary table #APZ_D_SRT2Local as (select top 0 * from APZ_D_SRT2);
	exec 'drop table #APZ_D_SRT2Local';
	exec 'create local temporary table #APZ_D_SRT2Local as (select * from "#APZ_D_SRT2")';
	
	create local temporary table #APZ_D_SRT3Local as (select top 0 * from APZ_D_SRT3);
	exec 'drop table #APZ_D_SRT3Local';
	exec 'create local temporary table #APZ_D_SRT3Local as (select * from "#APZ_D_SRT3")';
	
	if :actionType = 'U'
	then
		id := cast((select top 1 "DocEntry" from #APZ_D_OSRTLocal) as int);
		delete from APZ_D_SRT2 where "DocEntry" = :id;
		delete from APZ_D_SRT1 where "DocEntry" = :id;
		delete from APZ_D_OSRT where "DocEntry" = :id;
		delete from APZ_D_SRT3 where "DocEntry" = :id;
	else 
		id := cast((select ifnull(max("DocEntry"), 0) + 1 from APZ_D_OSRT) as int);
		update #APZ_D_OSRTLocal set "DocEntry" = :id;
	end if;
	
	update #APZ_D_SRT1Local set "DocEntry" = :id;
	update #APZ_D_SRT2Local set "DocEntry" = :id;
	update #APZ_D_SRT3Local set "DocEntry" = :id;
	
	insert into APZ_D_OSRT--("DocEntry", "ReportId", "UserSign", "CreateDate", "Active")
	select * from #APZ_D_OSRTLocal;
	
	insert into APZ_D_SRT1--("DocEntry", "RowId", "paddingBottom", "paddingLeft", "paddingTop", "paddingRight", "justifyContent", "alignItems", "paddingHorizontal", "paddingVertical", "fontSize")
	select * from #APZ_D_SRT1Local;
	
	insert into APZ_D_SRT2--("DocEntry", "LineNum", "FatherNum", "RowId", "ItemType", "fieldName", "size", "textBefore", "textAfter", "color", "alignItems", "justifyContent", "fontSize", "fontWeight", "paddingTop", "paddingBottom", "paddingLeft", "paddingRight", "textAlign", "alignSelf")
	select * from #APZ_D_SRT2Local;
	
	insert into APZ_D_SRT3--("DocEntry", "LineNum", "FatherNum", "RowId", "ItemType", "fieldName", "size", "textBefore", "textAfter", "color", "alignItems", "justifyContent", "fontSize", "fontWeight", "paddingTop", "paddingBottom", "paddingLeft", "paddingRight", "textAlign", "alignSelf")
	select "DocEntry",Row_number() over (Order BY "DashboardName"),"DashboardName","Layout" from #APZ_D_SRT3Local;
	
	select :id, '' from dummy;
	
END



