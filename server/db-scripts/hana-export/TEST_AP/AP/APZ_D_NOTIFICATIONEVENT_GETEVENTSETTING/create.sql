
CREATE PROCEDURE APZ_D_NotificationEvent_GetEventSetting(in appName nvarchar(100) default '')
AS
BEGIN
	select a.*,b."AppName",b."Active",b."Message",b."Title" from APZ_D_EVENTCONFIG a
					left join APZ_D_EVENTAPPCONFIG b on a."EventCode" = b."EventCode" and b."AppName" = :appName;
	--where Lower(@appName) in (SELECT Lower(part) FROM dbo.SplitString(a.AppUse,','))
END



