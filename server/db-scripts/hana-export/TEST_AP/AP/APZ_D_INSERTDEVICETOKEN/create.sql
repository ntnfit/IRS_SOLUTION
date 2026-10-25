

CREATE PROCEDURE APZ_D_InsertDeviceToken
AS
BEGIN

	create local temporary table #APZ_D_OADTLocal as (select top 0 * from APZ_D_OADT);
	exec 'drop table #APZ_D_OADTLocal';
	exec 'create local temporary table #APZ_D_OADTLocal as (select * from #APZ_D_OADT)';

	merge into APZ_D_OADT a
	using #APZ_D_OADTLocal as b
	on a."UserID" = b."UserID"  and a."DeviceID" = b."DeviceID" and a."DeviceName" = b."DeviceName" and a."DatabaseName" = b."DatabaseName" AND a."AppCode" = b."AppCode"
	--Update data
	when matched then 
	update set a."UserName" = b."UserName",
			   a."AppType" = b."AppType",
			   a."DeviceToken" = b."DeviceToken",
			   a."OSName" = b."OSName",
			   a."OSVersion" = b."OSVersion",
			   a."MacAddress" = b."MacAddress",
			   a."AppVersion" = b."AppVersion",
			   a."UpdateTime" = CURRENT_DATE
	--Insert data
	when not matched then insert("ID", "UserID", "UserName", "DeviceID", "DeviceName", "Domain", "DatabaseName", "AppType", "DeviceToken", "OSName", "OSVersion", "MacAddress", 
								 "AppCode", "AppVersion", "CreateTime", "UpdateTime")
						  values((select IFNULL(MAX(ID), 0) from APZ_D_OADT) + 1, b."UserID", b."UserName", b."DeviceID", b."DeviceName", b."Domain", b."DatabaseName", b."AppType", 
						 		b."DeviceToken", b."OSName", b."OSVersion", b."MacAddress", b."AppCode", b."AppVersion", CURRENT_DATE, CURRENT_DATE);

END



