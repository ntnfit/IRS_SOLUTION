
--	APZ_TrackingIssue_ChangeSatus
Create PROCEDURE APZ_TrackingIssue_ChangeSatus(
	in status char(1) default '',
	in id int default 0)
as
begin
	declare lineNum int=0;
	--if( :status not in('1','2','3','4','5','6','7','8') )
	--THEN
		
	--		select 124 ,'Status not exist value is -' || :status FROM DUMMY;
	--		return;

	--end IF;
	create local temporary table #APZ_TGIE1Data as(select top 0 * from APZ_TGIE1);
	exec 'drop table #APZ_TGIE1Data';
	exec 'create local temporary table #APZ_TGIE1Data as (select * from "#APZ_TGIE1");';
	
	update APZ_TGIE set "Status"=:status where "Id"=:id;
	select ifnull((select max("LineNum") from APZ_TGIE1 where "Id"=:id), 0)+1 into lineNum from DUMMY;
	UPDATE #APZ_TGIE1Data set "Id"=:id, "LineNum" = :lineNum; 
	--insert into APZ_TGIE1 values(@id,@lineNum,'Status',@oldValue,@newValue,@user,@date,'MadeChange')
	insert into APZ_TGIE1 select * from #APZ_TGIE1Data;
	select :id,'' from DUMMY;
end




