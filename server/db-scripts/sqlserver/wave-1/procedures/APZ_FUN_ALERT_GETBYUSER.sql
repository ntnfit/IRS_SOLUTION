-- HANA: table function apz_fun_alert_getbyuser — internal SAP alerts sent to a user (MailType 0 only)
CREATE OR ALTER FUNCTION dbo.apz_fun_alert_getbyuser (@UserName varchar(100), @MailType int)
RETURNS TABLE
AS
RETURN
    SELECT CAST(t0.Priority AS varchar(1)) AS Priority, N'0' AS Attachment, t0.Code, t0.Name, t0.UserSign,
           t5.USER_CODE AS FromUser, t4.QString, ISNULL(t6.Opened, N'N') AS Opened, ISNULL(t6.WasRead, N'N') AS WasRead,
           ISNULL(t6.Deleted, N'N') AS Deleted, ISNULL(t6.Failed, N'N') AS Failed,
           -- RecDate + RecTime (hhmm int); when unread the current time is used, as in HANA
           DATEADD(MINUTE,
                   (ISNULL(t6.RecTime, DATEPART(HOUR, GETDATE()) * 100 + DATEPART(MINUTE, GETDATE())) / 100) * 60
                   + ISNULL(t6.RecTime, DATEPART(HOUR, GETDATE()) * 100 + DATEPART(MINUTE, GETDATE())) % 100,
                   CAST(CAST(ISNULL(t6.RecDate, GETDATE()) AS date) AS datetime)) AS Received
    FROM OALT t0
    INNER JOIN ALT1 t2 ON t0.Code = t2.Code AND t2.SendIntrnl = 'Y'
    INNER JOIN OUSR t3 ON t2.UserSign = t3.USERID AND t3.USER_CODE = @UserName
    INNER JOIN OUQR t4 ON t0.QueryId = t4.IntrnalKey
    INNER JOIN OUSR t5 ON t0.UserSign = t5.USERID
    LEFT JOIN OAIB t6 ON t0.Code = t6.AlertCode AND t2.UserSign = t6.UserSign
    WHERE t0.Active = 'Y' AND @MailType = 0;
