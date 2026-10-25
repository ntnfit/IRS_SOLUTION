

CREATE PROCEDURE APZ_Process_SaveLeaveParameter
AS
BEGIN
	declare checkDulp int = 0;
	declare code int = 0;
	declare branch varchar(50) = 0;
	
	--Create temp table
	create local temporary table #APZ_OSLPTmp as (select top 0 * from APZ_OSLP);
	exec 'DROP TABLE #APZ_OSLPTmp;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OSLPTmp AS (SELECT * FROM #APZ_OSLP);';
		
	--Get max code in temp table
	select (select ifnull(max("Code"), 0) from #APZ_OSLPTmp) into code from DUMMY;
	
	--Update Code and Branch	
	update #APZ_OSLPTmp set "Code" = :code + 1 where ifnull("Code", 0) = 0;
	update #APZ_OSLPTmp set "branch" = -2 where ifnull("branch", 0) = 0;
	
	--Get dulplicate Branch if exists
	select (select top 1 a."branch" from #APZ_OSLPTmp a inner join #APZ_OSLPTmp b on a."Code" <> b."Code" AND a."branch" = b."branch" and ((a."ToDate" is null and b."ToDate" is null)  or ((b."FromDate" between a."FromDate" and ifnull(a."ToDate",b."FromDate")) or (a."FromDate" between b."FromDate" and ifnull(b."ToDate", a."FromDate"))))) into checkDulp from DUMMY; 
	
	--Check exists dulplicate Branch 
	if :checkDulp <> 0 then
		select (select top 1 "Name" from OUBR where "Code" = :checkDulp) into branch from DUMMY;
		select 800013 as "ErrorNumber", 'Timestamp cannot be duplicate in branch ['|| :branch ||']' as "ErrorMessage" from DUMMY;
		return;
	end if;
	
	--Insert data
	delete from APZ_OSLP;
	insert into APZ_OSLP select * from #APZ_OSLPTmp;
END;




