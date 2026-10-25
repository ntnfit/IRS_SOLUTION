

--	APZ_WorkingTable_GetUserSettings 'thanhnv', 'B01','HN01', 100

CREATE PROCEDURE APZ_WorkingTable_GetUserSettings(
	in user_code nvarchar(50) default '')
AS
BEGIN
	--select 0
	Select "AlertPolFr", "ScreenLock" from OUSR where USER_CODE = :user_code;
END




