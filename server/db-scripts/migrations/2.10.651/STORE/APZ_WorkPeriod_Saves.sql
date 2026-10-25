
CALL APZ_EXISTSPROC('APZ_WorkPeriod_Saves');

GO


CREATE PROCEDURE APZ_WorkPeriod_Saves(in type varchar(1) default '')
AS
BEGIN
	declare code nvarchar(50)='';
	declare checkCode nvarchar(50) = '';
	declare MaxId Int = 0;
	
	--Create temp table
	create local temporary table #APZ_OHWPLocal as(select top 0 * from APZ_OHWP);
	exec 'DROP TABLE #APZ_OHWPLocal;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OHWPLocal as(SELECT * FROM #APZ_OHWP);';

	--Remove record exists 
	If :type='U' then
		Select (select top 1 "PeriodCode" from #APZ_OHWPLocal) into code from DUMMY;
		--If :code <> '' then
			--delete from APZ_OHWP where "PeriodCode"=:code;
		--End if;
		Delete from APZ_OHWP a where EXISTS (Select 1 From #APZ_OHWPLocal b Where IFNULL(a."ID",0) = IFNULL(b."ID",0));
	End If;	
	
	--Check Period code exists
	If :type='A' then
		select ifnull((select a."PeriodCode" from APZ_OHWP a inner join #APZ_OHWPLocal b on a."PeriodCode" = b."PeriodCode"), '') into checkCode from DUMMY;
		If :checkCode <> '' then
			select '100 ' as ErrorCode, 'Period Code ' || :checkCode ||' is exists' as ErrorMessage from DUMMY;
			return;
		End If;
		
		Select ifnull((select Max("ID") from APZ_OHWP), 0) into MaxId from DUMMY;
		WHILE EXISTS(SELECT 1 FROM #APZ_OHWPLocal WHERE "ID" = 0) 
		DO
			UPDATE top 1 #APZ_OHWPLocal set "ID" = :MaxId + 1 WHERE "ID" = 0;
			MaxId = :MaxId + 1;
		END WHILE;
	End If;
	
	--Insert data
	Insert into APZ_OHWP select * from #APZ_OHWPLocal;
	select :code,'' from DUMMY;
END;