CREATE PROCEDURE APZ_HolidaySetup_Save
AS
BEGIN
	
	declare code nvarchar(20)='';
	--Create temp table
	create local temporary table #APZ_OHLDLocal as(select top 0 * from APZ_OHLD);
	exec 'DROP TABLE #APZ_OHLDLocal;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OHLDLocal as(select * from #APZ_OHLD);';
	
	create local temporary table #APZ_HLD1Local as(select top 0 * from APZ_HLD1);
	exec 'DROP TABLE #APZ_HLD1Local;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_HLD1Local as(select * from #APZ_HLD1);';
	
	--Get HldCode from Temp table
	select ifnull((select top 1 "HldCode" from #APZ_OHLDLocal),'') into code from DUMMY;
	
	--Insert data
	delete from APZ_OHLD where "HldCode"=:code;
	delete from APZ_HLD1 where "HldCode"=:code;
	insert into APZ_OHLD select * from #APZ_OHLDLocal;
	insert into APZ_HLD1 select * from #APZ_HLD1Local where ifnull("StrDate", '') <> '';
	
	--Return HldCode inserted
	select :code,'' from DUMMY;
END;

