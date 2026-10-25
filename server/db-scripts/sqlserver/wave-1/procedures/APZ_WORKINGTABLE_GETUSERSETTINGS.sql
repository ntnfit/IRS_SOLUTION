-- HANA: APZ_WorkingTable_GetUserSettings
CREATE OR ALTER PROCEDURE dbo.APZ_WorkingTable_GetUserSettings
    @user_code nvarchar(50) = N''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AlertPolFr, ScreenLock FROM OUSR WHERE USER_CODE = @user_code;
END;
