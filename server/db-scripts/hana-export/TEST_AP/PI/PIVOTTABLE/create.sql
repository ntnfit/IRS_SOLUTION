

create procedure PivotTable(
	in outTableName nvarchar(50)
	, in inTableName nvarchar(50)
	, in functionName nvarchar(50)
	, in dataFieldName nvarchar(50)
	, in dataTypeDataField nvarchar(20)
	, in lstColumnRowName nvarchar(254)
	, in lstColumPivotName nvarchar(254)
	, in fieldSourceColumnPivot nvarchar(50)
	)
as
BEGIN

	declare query nvarchar(50000) = '';
	declare addColumn nvarchar(5000) = '';
	declare compareColumn nvarchar(500) = '';
	declare columnRow nvarchar(500) = '';
	declare columnRowCreate nvarchar(50000) = '';
	
	--lấy danh sách pivot column để tạo mới table
	select (select string_agg(',cast(null as ' || :dataTypeDataField || ') "' || data || '"') from split(:lstColumPivotName, ',')) into addColumn from dummy;
	if length(:addColumn) > 1
	then
		addColumn = substring(:addColumn, 2, length(:addColumn) - 1);
	end if;
	
	--lấy danh sách row column để sử dụng trong mệnh đề group
	select (select string_agg(',x."' || data || '"') from split(:lstColumnRowName, ',')) into columnRow from dummy;
	if length(:columnRow) > 1
	then
		columnRow = substring(:columnRow, 2, length(:columnRow) -1);
	end if;
	
	--lấy danh sách row column để sử dụng trong mệnh đề where
	select (select string_agg(' and a."' || data || '" = b."' || data || '"') from split(:lstColumnRowName, ',')) into compareColumn from dummy;
	
	--lấy danh sách column để tạo table tạm
	select (select string_agg('"' || data || '" nvarchar(50),') from split(:lstColumnRowName, ',')) into columnRowCreate from dummy;
	
	query = 'DO BEGIN 
		declare countIndex int = 0;
		declare maxIndex int = 0;
		declare fieldPivot nvarchar(50) = '''';
		declare checkCount int = 0;
		create local temporary table #FPivotTable (' || :columnRowCreate || '"' || :fieldSourceColumnPivot || '" nvarchar(50),"' || :dataFieldName || '" nvarchar(50), PivotTableIndex int);
		exec ''drop table #FPivotTable;'';
		exec ''create local temporary table #FPivotTable as (select *, row_number() over(order by (select 1 from dummy)) PivotTableIndex from ' || :inTableName || ');'';
		
		exec ''drop table ' || :outTableName || ';'';
		create local temporary table ' || :outTableName || ' as (select ' || replace(:columnRow, 'x.', '') || ', ' || :addColumn || '  from #FPivotTable group by ' || replace(:columnRow, 'x.', '') || ');
		
		select ifnull((select max(PivotTableIndex) from #FPivotTable),0) into maxindex from dummy;
		
		while :countIndex <= :maxindex
		DO
			select ifnull((select "' || :fieldSourceColumnPivot || '" from #FPivotTable where PivotTableIndex = :countIndex),'''') into fieldPivot from dummy;
			
			select ifnull((select top 1 1 from split(''' || :lstColumPivotName || ''','','') where data = :fieldPivot),0) into checkCount from dummy;
			
			if :checkCount = 1
			then
			
				exec ''update a set '' || :fieldPivot || '' = b.valueupdate from ' || :outTableName || ' a inner join (select ' || :functionName || '(x."' || :dataFieldName || 
						'") valueUpdate, ' || :columnRow || ' from #FPivotTable x inner join (select * from #FPivotTable where PivotTableIndex = '' || :countIndex || '') y on x."' || :fieldSourceColumnPivot || '"='''''' || :fieldPivot ||''''''' || replace(replace(:compareColumn, 'a.','x.'), 'b.', 'y.') 
						|| ' group by ' || :columnRow  || ') b on 1=1 ' || :compareColumn || ';'';	
						
				exec ''update x set x.PivotTableIndex = y.PivotTableIndex from #FPivotTable x inner join (select * from #FPivotTable where PivotTableIndex = '' || :countIndex || '') y on x."' || :fieldSourceColumnPivot || '"='''''' 
				|| :fieldPivot ||''''''' || replace(replace(:compareColumn, 'a.','x.'), 'b.', 'y.') || ';'';
				
				exec ''delete from #FPivotTable where PivotTableIndex = '' || :countIndex || '';'';
			
			end if;
			
			countIndex = :countIndex + 1;
		END while;
		drop table #FPivotTable;
		end;';
		
		--thanhnv = :query;
		--select :query from dummy;
		exec :query;

END




