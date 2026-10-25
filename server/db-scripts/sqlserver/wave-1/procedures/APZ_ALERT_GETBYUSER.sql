-- HANA: apz_alert_getbyuser. C# callers also pass @User / @UserCode on one path; accepted and unused.
CREATE OR ALTER PROCEDURE dbo.apz_alert_getbyuser
    @UserName varchar(100),
    @MailType int,
    @User nvarchar(100) = NULL,
    @UserCode nvarchar(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.apz_fun_alert_getbyuser(@UserName, @MailType);
END;
