
Create PROCEDURE APZ_D_SetIndicatorKPIs_Save(in typeName char(1) default '')
as
begin
	Declare code nvarchar(50)='';
	create local temporary table #APZ_D_KPISData as (select top 0 * from APZ_D_KPIS);
	exec 'insert into #APZ_D_KPISData select * from #APZ_D_KPIS';
	
	select top 1 "Code" into code from #APZ_D_KPISData;
	if :typeName='U'
	THEN
		delete from APZ_D_KPIS where "Code" =:code;
		delete from APZ_D_KPIS1 where "Code"=:code;
	end if;
	
	if exists(select 1 from #APZ_D_KPISData a INNER JOIN APZ_D_KPIS b ON a."Code"=b."Code")
	THEN
	
		select 122 ,'Code ' || :code ||  ' is overlapping with an existing database' from DUMMY;
		return;
	
	END IF;
	
	insert into APZ_D_KPIS select * from #APZ_D_KPISData;
	exec 'insert into APZ_D_KPIS1 select * from #APZ_D_KPIS1';
		
	select :code,'' from DUMMY;
end




