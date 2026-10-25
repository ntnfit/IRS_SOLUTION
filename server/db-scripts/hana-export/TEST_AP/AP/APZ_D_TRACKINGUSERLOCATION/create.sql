

CREATE PROCEDURE APZ_D_TrackingUserLocation
AS
BEGIN

	--Create local temporary table
	create local temporary table #APZ_D_TKLCLocal as (select top 0 * from APZ_D_TKLC);
	exec 'drop table #APZ_D_TKLCLocal';
	exec 'create local temporary table #APZ_D_TKLCLocal as (select * from #APZ_D_TKLC)';
	
	--Insert new data	
	insert into APZ_D_TKLC("Code", "LineNum", "UserSign", "DeviceId", "DeviceName", "DeviceType", "DeviceModel", "SystemName", "SystemVersion",
						   "MacAddress", "BatteryPercent", "Latitude", "Longitude", "TrackingDate", "TrackingTime", "CreateDate", "CreateTime","CreateType","Distance", "AppVersion") 
	select a."Code"
			,((SELECT ifnull(MAX("LineNum"),0) FROM APZ_D_TKLC Where "UserSign"=a."UserSign") + ROW_NUMBER() OVER (ORDER BY a."TrackingDate" ASC)),
			 a."UserSign", a."DeviceId", a."DeviceName", a."DeviceType", a."DeviceModel", a."SystemName", a."SystemVersion", 
			a."MacAddress", a."BatteryPercent", a."Latitude", a."Longitude", a."TrackingDate", a."TrackingDate", current_date, current_time
			--#region namnx 8/1/2021 thêm trường thông tin tracking location
			,a."CreateType",a."Distance"
			--#endregion
			--#region namnx 14/1/2021 thêm cột app version
			,a."AppVersion"
	--#endregion
	from #APZ_D_TKLCLocal a;
	
	--Return message of result
	if ::ROWCOUNT <= 0
	then
		select 400 as "Code", 'Insert table header fail' as "Message" from dummy;
		return;
	end if;
	
	select 200 as "Code", '' as "Message" from dummy;
END




