
CREATE PROCEDURE APZ_AboutRangeTime_Save()
AS
BEGIN

	create local temporary table #APZ_OKPGData as (select top 0 b.* from (select 1 A from DUMMY) a left join APZ_OKPG b ON 1=2);
	exec 'insert into #APZ_OKPGData select * from #APZ_OKPG';
	
	--- xóa các bản ghi bị null
	delete from #APZ_OKPGData where "FromTime" is null or "ToTime" is null or "RecTime" is null;
	--kiểm tra điều kiện from time and to time
	if exists(select 1 from #APZ_OKPGData a where "ToTime" < "FromTime")
	THEN
	
		select 100002, 'To Time must be greater than From Time' from DUMMY;
		return;
	
	END IF;
	--kiểm tra điều kiện overlap time
	if exists(select 1 from #APZ_OKPGData a inner join #APZ_OKPGData b on a."FromTime" > b."FromTime" and a."FromTime" < b."ToTime" or a."ToTime" > b."FromTime" and a."ToTime" < b."ToTime")
	THEN
	
		select 100001, 'Setting overlap time' from DUMMY;
		return;
	
	END IF;
	
	delete from APZ_OKPG;
	insert into APZ_OKPG select * from #APZ_OKPGData;
	select 200, '' from DUMMY;
	

END;



