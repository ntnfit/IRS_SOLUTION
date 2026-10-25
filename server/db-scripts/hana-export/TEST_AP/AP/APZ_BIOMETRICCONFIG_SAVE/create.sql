
CREATE PROCEDURE APZ_BiometricConfig_Save
AS
BEGIN
	
	declare AbsEntry Int = 0;
	declare MachineId Int = 0;
	declare SerialNumber Varchar(30) = '';
	declare MaxCode Int = 0;
	
	create local temporary table #APZ_OBMCDATA as (select top 0 * from APZ_OBMC);
	exec 'drop table #APZ_OBMCDATA';
	exec 'create local temporary table #APZ_OBMCDATA as (select * from #APZ_OBMC)';

	Select ifnull("AbsEntry",0), ifnull("MachineID",0) Into AbsEntry,MachineId From #APZ_OBMCDATA;
	Select IfNull((Select Top 1 "SerialNumber" From #APZ_OBMCDATA),'') Into SerialNumber From Dummy;
		
	if :MachineId = 0
	THEN 
		select 'Machine ID must greater than 0' from DUMMY;
		return;
	END IF;
	
	if :AbsEntry = 0 And Exists(Select 1 From APZ_OBMC Where "MachineID" = MachineID)
	THEN 
		select 'Machine ID has already existed' from DUMMY;
		return;
	END IF;
	
	if :AbsEntry = 0 And Exists(Select 1 From APZ_OBMC Where "SerialNumber" = SerialNumber)
	THEN 
		select 'Serial number has already existed' from DUMMY;
		return;
	END IF;
	
	delete from APZ_OBMC where "AbsEntry" = :AbsEntry;
	Select IfNull((Select Max("AbsEntry") from APZ_OBMC),0) Into MaxCode From Dummy;
	Update #APZ_OBMCDATA Set "AbsEntry" = :MaxCode + 1 where "AbsEntry" = 0 Or "AbsEntry" Is Null;
	Insert into APZ_OBMC Select * from #APZ_OBMCDATA;
END




