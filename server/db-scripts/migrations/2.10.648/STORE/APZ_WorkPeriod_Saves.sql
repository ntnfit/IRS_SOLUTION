
CALL APZ_EXISTSPROC('APZ_WorkPeriod_Saves');

GO


CREATE PROCEDURE APZ_WorkPeriod_Saves(in type varchar(1) default '')
AS
BEGIN
	declare code nvarchar(50)='';
	declare checkCode nvarchar(50) = '';
	
	--Create temp table
	create local temporary table #APZ_OHWPLocal as(select top 0 * from APZ_OHWP);
	exec 'DROP TABLE #APZ_OHWPLocal;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OHWPLocal as(SELECT * FROM #APZ_OHWP);';

	--Remove record exists 
	if :type='U' then
		select (select top 1 "PeriodCode" from #APZ_OHWPLocal) into code from DUMMY;
		if :code <> '' then
			delete from APZ_OHWP where "PeriodCode"=:code;
		end if;
	end if;	
	
	--Check Period code exists
	if :type='A' then
		select ifnull((select a."PeriodCode" from APZ_OHWP a inner join #APZ_OHWPLocal b on a."PeriodCode" = b."PeriodCode"), '') into checkCode from DUMMY;
		if :checkCode <> '' then
			select '100 ' as ErrorCode, 'Period Code ' || :checkCode ||' is exists' as ErrorMessage from DUMMY;
			return;
		end if;
	end if;
	
	--Insert data
	Insert into APZ_OHWP select * from #APZ_OHWPLocal;
	select :code,'' from DUMMY;
END;