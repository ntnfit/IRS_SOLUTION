
CREATE PROCEDURE APZ_D_Save_AppSetting
AS
BEGIN

	create local temporary table #APZ_D_ODASData as(select top 0 * from APZ_D_ODAS);
	exec 'drop table #APZ_D_ODASData';
	exec 'create local temporary table #APZ_D_ODASData as (select * from #APZ_D_ODAS)';
	
	DELETE FROM APZ_D_ODAS A where exists(select 1 from #APZ_D_ODASData B where A."UserId" = B."UserId" AND A."DeviceId" = B."DeviceId" AND A."FormId" = B."FormId");
	
	insert into APZ_D_ODAS
	(
	    "UserId",
	    "DeviceId",
	    "DeviceName",
	    "DeviceType",
	    "DeviceModel",
	    "SystemName",
	    "SystemVersion",
	    "MacAddress",
	    "BatteryPercent",
	    "AppVersion",
	    "CreateDate",
	    "FormId",
	    "Setting"
	)
		select a."UserId",
		a."DeviceId",a."DeviceName",a."DeviceType",a."DeviceModel",a."SystemName",a."SystemVersion",a."MacAddress",a."BatteryPercent",a."AppVersion",Current_date,a."FormId",
		a."Setting"
		from #APZ_D_ODASData a;
		if ::ROWCOUNT<=0
		THEN
			select 400 as "Code",'Insert table header fail' as "Message" from DUMMY;
			return;
		else
			select 200 as "Code",'' as "Message" from DUMMY;
		end IF;
END




