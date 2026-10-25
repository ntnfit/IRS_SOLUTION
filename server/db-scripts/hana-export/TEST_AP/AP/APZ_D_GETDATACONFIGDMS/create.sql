CREATE PROCEDURE APZ_D_GetDataConfigDMS
(
	IN appName NVARCHAR(50)
)
AS
BEGIN
	select *,N'{"timeout":15000,"enableHighAccuracy":true,"maximumAge":0,"accuracy":{"ios":"best","android":"high"}}' as "ConfigGetLocation" from APZ_D_OAPC where "AppName" = :appName;
END



