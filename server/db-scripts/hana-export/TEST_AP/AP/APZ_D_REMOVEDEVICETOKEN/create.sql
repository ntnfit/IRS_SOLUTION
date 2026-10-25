

CREATE PROCEDURE APZ_D_RemoveDeviceToken
AS
BEGIN

	create local temporary table #APZ_D_OADTLocal as (select top 0 * from APZ_D_OADT);
	exec 'drop table #APZ_D_OADTLocal';
	exec 'create local temporary table #APZ_D_OADTLocal as (select * from #APZ_D_OADT)';

	merge into APZ_D_OADT as a
	using #APZ_D_OADTLocal as b 
	on a."UserID" = b."UserID" and a."DeviceID" = b."DeviceID" and a."DeviceName" = b."DeviceName"
	and a."DatabaseName" = b."DatabaseName" and a."AppCode" = b."AppCode"
	when matched then delete;
	
END



