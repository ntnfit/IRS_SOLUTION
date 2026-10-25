
CALL APZ_EXISTSPROC('APZ_WorkScheduleSetup_Save');

GO

CREATE PROCEDURE APZ_WorkScheduleSetup_Save
AS
BEGIN
	declare code int;
	declare branch int;
	declare maxLength int;
	
	create local temporary table #APZ_OWSCLOCAL as (select top 0 * from APZ_OWSC);
	exec 'drop table #APZ_OWSCLOCAL;';
	exec 'create local temporary table #APZ_OWSCLOCAL as(select * from #APZ_OWSC);';
	select ifnull((select top 1 "SchID" from #APZ_OWSCLOCAL),0) into code from DUMMY;
	if	:code = 0 
	THEN
		SELECT IFnull((select Max("SchID") from APZ_OWSC),0) + 1 into code from DUMMY;
	end IF;
	
	update #APZ_OWSCLOCAL set "SchID" = :code;

	select ifnull((select top 1 "branch" from #APZ_OWSCLOCAL),0) into branch from DUMMY;
	delete from APZ_OWSC where "SchID" = :code;
	delete from APZ_WSC1 where "SchID" = :code;
	delete from APZ_WSC2 where "SchID" = :code;
	delete from APZ_WSC3 where "SchID" = :code;

	insert into APZ_OWSC select * from #APZ_OWSCLOCAL;
	
	create local temporary table #APZ_WSC1LOCAL as (select top 0 "DayID", "ShiftID" as "WorkShift" from APZ_WSC1);
	exec 'drop table #APZ_WSC1LOCAL';
	exec 'create local temporary table #APZ_WSC1LOCAL as (Select "DayID","WorkShift" from #APZ_WSC1)';
	If Exists(Select 1 From #APZ_WSC1LOCAL Where IFNULL("WorkShift",0) != 0) Then
		Insert Into APZ_WSC1("SchID","DayID","ShiftID") Select :code,"DayID","WorkShift" From #APZ_WSC1LOCAL Where IFNULL("WorkShift",0) != 0;
	End If;
	
	--select ifnull((select max(APZ_GetCountCharInString("WorkShift",',')) from #APZ_WSC1LOCAL), 0) into maxLength from DUMMY;
	--Create local temporary table #Number (indexdata int);
	--Insert Into #Number
	--	SELECT GENERATED_PERIOD_START FROM SERIES_GENERATE_DECIMAL(1, 1, :maxLength + 2);
		
	--insert into APZ_WSC1 select :code,a."DayID",c."ShiftID"
	--	from #APZ_WSC1LOCAL a left join #Number b ON 1=1 
	--							left join APZ_OEWS c on GetStringSplitIndex(a."WorkShift",',', b.indexdata) = c."ShiftCode" and c."branch" = :branch
	--	where c."ShiftID" is not null;
	
	create local temporary table #APZ_WSC2LOCAL as (select top 0 "DayID", "ShiftID" as "WorkShift" from APZ_WSC2);
	exec 'DROP TABLE #APZ_WSC2LOCAL;';
	exec 'create local temporary table #APZ_WSC2LOCAL as (select "DayID","WorkShift" from #APZ_WSC2);';
	
	If Exists(Select 1 From #APZ_WSC2LOCAL Where IFNULL("WorkShift",0) != 0) Then
		Insert Into APZ_WSC2("SchID","DayID","ShiftID") Select :code,"DayID","WorkShift" From #APZ_WSC2LOCAL Where IFNULL("WorkShift",0) != 0;
	End If;
	
	--delete from #Number;
	--select ifnull((select max(APZ_GetCountCharInString("WorkShift",',')) from #APZ_WSC2LOCAL), 0) into maxLength from DUMMY;
	--Insert Into #Number
	--	SELECT GENERATED_PERIOD_START FROM SERIES_GENERATE_DECIMAL(1, 1, :maxLength + 2);
		
	--insert into APZ_WSC2 select :code,a."DayID",c."ShiftID"
	--		from #APZ_WSC2LOCAL a left join #Number b ON 1=1
	--								left join APZ_OEWS c on GetStringSplitIndex(a."WorkShift",',', b.indexdata) = c."ShiftCode" and c."branch" = :branch
	--		where c."ShiftID" is not null;
	
	create local temporary table #APZ_WSC3LOCAL as (select top 0 "DayID", "ShiftID" as "WorkShift" from APZ_WSC3);
	exec 'drop table #APZ_WSC3LOCAL;';
	exec 'create local temporary table #APZ_WSC3LOCAL as(Select "DayID", "WorkShift" from #APZ_WSC3);';
	
	If Exists(Select 1 From #APZ_WSC3LOCAL Where IFNULL("WorkShift",0) != 0) Then
		Insert Into APZ_WSC3("SchID","DayID","ShiftID") Select :code,"DayID","WorkShift" From #APZ_WSC3LOCAL Where IFNULL("WorkShift",0) != 0;
	End If;
	
	
	--delete from #Number;
	--select ifnull((select max(APZ_GetCountCharInString("WorkShift",',')) from #APZ_WSC3LOCAL),0) into maxLength from DUMMY;
	--Insert Into #Number
	--	SELECT GENERATED_PERIOD_START FROM SERIES_GENERATE_DECIMAL(1, 1, :maxLength + 2);
		
	--insert into APZ_WSC3 select :code,a."DayID",c."ShiftID"
	--	from #APZ_WSC3LOCAL a left join #Number b ON 1=1
	--							left join APZ_OEWS c on GetStringSplitIndex(a."WorkShift",',', b.indexdata) = c."ShiftCode" and c."branch" = :branch
	--	where c."ShiftID" is not null;
	
	
	select :code,'' from DUMMY;
END;