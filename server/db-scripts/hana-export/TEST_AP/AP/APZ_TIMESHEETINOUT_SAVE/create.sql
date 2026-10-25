CREATE procedure APZ_TimeSheetInOut_Save
as
begin

	declare absEntry int;
	declare baseDoc int;
	declare lineNum int = 0;
	
	create local temporary table #APZ_OIOD_Local As (select top 0 * from APZ_OIOD);
	exec 'drop table #APZ_OIOD_Local';
	exec 'create local temporary table #APZ_OIOD_Local as (select * from #APZ_OIOD)';
	
	create local temporary table #APZ_IOD1_Local As (select top 0 * from APZ_IOD1);
	exec 'drop table #APZ_IOD1_Local';
	exec 'create local temporary table #APZ_IOD1_Local as (select * from #APZ_IOD1)';
	
	create local temporary table #APZ_IOD2_Local As (select top 0 * from APZ_IOD2);
	exec 'drop table #APZ_IOD2_Local';
	exec 'create local temporary table #APZ_IOD2_Local as (select * from #APZ_IOD2)';

	absEntry = ifnull((select top 1 "AbsEntry" from #APZ_OIOD_Local),0);
	baseDoc = ifnull((select top 1 "BaseDoc" from #APZ_OIOD_Local),0);
	Delete From #APZ_IOD2_Local Where IFNULL("OvertimeType",'') = '';
	
	if :absEntry <= 0
	then
		absEntry = ifnull((select Max("AbsEntry") from APZ_OIOD),0) +1;
	END IF;

	Update #APZ_OIOD_Local set "AbsEntry" = :absEntry;
	Update #APZ_IOD1_Local set "AbsEntry" = :absEntry, "LineNum" = null;
	Update #APZ_IOD2_Local set "AbsEntry" = :absEntry, "LineNum" = null;
	
	while exists(select 1 from #APZ_IOD1_Local where "LineNum" is null)
	DO
		lineNum = :lineNum + 1;
		update top 1 #APZ_IOD1_Local set "LineNum" = :lineNum where "LineNum" is null;
	END WHILE;
	
	lineNum = 0;
	while exists(select 1 from #APZ_IOD2_Local where "LineNum" is null)
	DO
		lineNum = :lineNum + 1;
		update top 1 #APZ_IOD2_Local set "LineNum" = :lineNum where "LineNum" is null;
	END WHILE;
	
	--UPDATE #APZ_IOD1 set  a.LineNum = b.RN from #APZ_IOD1 a inner join #UpdateData b on a.empID = b.empID where isnull(a.LineNum,0) <=0 and (a.InTime is not null or a.OutTime is not null)
	delete from APZ_OIOD where "AbsEntry" = :absEntry;
	delete from APZ_IOD1 where "AbsEntry" = :absEntry;
	delete from APZ_IOD2 where "AbsEntry" = :absEntry;

	INSERT INTO APZ_OIOD select * from #APZ_OIOD_Local;
	INSERT INTO APZ_IOD1 select * from #APZ_IOD1_Local where ifnull("LineNum",0) > 0;
	INSERT INTO APZ_IOD2 select * from #APZ_IOD2_Local where ifnull("LineNum",0) > 0;

	UPDATE APZ_OTBD set "status" = 'C' where "AbsEntry" = :baseDoc;
	
	select :absEntry, '' from dummy;
	
end;
