-- HANA: APZ_SubActivities_GetAllNotification — the user's notifications: activities/tasks assigned to him,
-- tracking issues he handles, SAP alerts with results, and unread comments; filtered by APZ_NOSE window.
-- Two HANA defects are fixed to the evident intent (the SQL Server original behaved this way):
--   * alert result counts: HANA ran the count only inside an EXIT HANDLER, so it never ran; here TRY/CATCH skips bad queries
--   * timeIndex: HANA formatted CURRENT_DATE (always 00:00); here the current time (hhmm) is used
CREATE OR ALTER PROCEDURE dbo.APZ_SubActivities_GetAllNotification
    @userId int = -1
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @userCode nvarchar(50) = (SELECT USER_CODE FROM OUSR WHERE USERID = @userId);

    SELECT Code, Name, UserSign, FromUser, CAST(QString AS nvarchar(max)) AS QString,
           ROW_NUMBER() OVER (ORDER BY Code) AS Id, 0 AS ResultValue
    INTO #tempDate
    FROM dbo.apz_fun_alert_getbyuser(@userCode, 0);

    DECLARE @i int = ISNULL((SELECT MAX(Id) FROM #tempDate), 0), @q nvarchar(max), @cnt int;
    WHILE @i > 0
    BEGIN
        SELECT @q = N'SELECT @c = COUNT(1) FROM (' + QString + N') a' FROM #tempDate WHERE Id = @i;
        BEGIN TRY
            EXEC sys.sp_executesql @q, N'@c int OUTPUT', @c = @cnt OUTPUT;
            UPDATE #tempDate SET ResultValue = @cnt WHERE Id = @i;
        END TRY
        BEGIN CATCH
            -- an alert query that does not compile on SQL Server is skipped, as the HANA handler intended
        END CATCH;
        SET @i -= 1;
    END;

    DECLARE @timeIndex int = DATEPART(HOUR, GETDATE()) * 100 + DATEPART(MINUTE, GETDATE());
    DECLARE @today date = CAST(GETDATE() AS date);
    DECLARE @employee int = ISNULL((SELECT empID FROM OHEM WHERE userId = @userId), 0);

    CREATE TABLE #tempSetting (CreateUserCode nvarchar(50) COLLATE DATABASE_DEFAULT, AssignUser nvarchar(4000) COLLATE DATABASE_DEFAULT, DueDate datetime, DueTime int, Details nvarchar(4000) COLLATE DATABASE_DEFAULT,
        NotifiedId int, Location int, ObjectTypes nvarchar(20) COLLATE DATABASE_DEFAULT, CreateUserName nvarchar(254) COLLATE DATABASE_DEFAULT, SumResult int, TranType nchar(1) COLLATE DATABASE_DEFAULT, CmtCode int,
        CntctType nvarchar(50) COLLATE DATABASE_DEFAULT, RemType nvarchar(50) COLLATE DATABASE_DEFAULT, RemQty numeric(19, 6), RecurrenceInfo nvarchar(max) COLLATE DATABASE_DEFAULT);

    INSERT INTO #tempSetting
    SELECT b.USER_CODE, a.GroupUser,
           ISNULL(CASE WHEN a.Action = 'T' THEN a.endDate ELSE a.Recontact END, '9999-12-30'),
           CASE WHEN a.Action = 'T' THEN a.ENDTime ELSE a.BeginTime END,
           a.Details, a.ClgCode, a.Location,
           CASE WHEN a.Action = 'T' THEN '1470000128' ELSE '1470000129' END,
           b.U_NAME, 0, 'A', 0, a.CntctType, a.RemType, a.RemQty, a.RecurrenceInfo
    FROM dbo.APZ_OCLG a
    LEFT JOIN OUSR b ON a.AssignedBy = b.USERID
    WHERE ISNULL(a.parentTask, 0) = 0 AND a.Action IN ('T', 'V') AND a.status = 1
      AND (a.Action = 'T' OR a.Recontact > @today OR (a.Recontact = @today AND a.BeginTime >= @timeIndex))
      AND ((ISNULL(a.GroupUser, N'') <> N'' AND dbo.CompareSplit(CAST(@userId AS nvarchar(20)), a.GroupUser, N';') = 1)
        OR (ISNULL(a.GroupEmployee, N'') <> N'' AND dbo.CompareSplit(CAST(@employee AS nvarchar(20)), a.GroupEmployee, N';') = 1))
    UNION ALL
    SELECT CAST(b.USER_CODE AS nvarchar(50)), CAST(@userId AS nvarchar(20)),
           ISNULL(CASE WHEN a.ResponseDate IS NULL THEN a.IssueDate ELSE a.ResponseDate END, '9999-12-30'), 0,
           a.Note, a.Id, NULL, '1470000130', b.U_NAME, 0, 'A', 0, NULL, NULL, NULL, NULL
    FROM dbo.APZ_TGIE a
    LEFT JOIN OHEM c ON a.Reporter = c.empID
    LEFT JOIN OUSR b ON c.userId = b.USERID
    WHERE CAST(a.Status AS int) <= 4 AND @employee = a.HandleBy
    UNION ALL
    SELECT NULL, NULL, NULL, 0, Name, Code, NULL, '1200004', NULL, ResultValue, 'A', 0, NULL, NULL, NULL, NULL
    FROM #tempDate WHERE ResultValue > 0
    UNION ALL
    SELECT c.USER_CODE, CAST(@userId AS nvarchar(20)), a.CreatedTime,
           DATEPART(HOUR, a.CreatedTime) * 100 + DATEPART(MINUTE, a.CreatedTime), a.Content, a.ID, NULL,
           a.ObjType, c.U_NAME, 0, 'M', a.CmtCode, NULL, NULL, NULL, NULL
    FROM dbo.APZ_CLG1 a
    INNER JOIN (SELECT CmtCode FROM dbo.APZ_CLG3 WHERE UserSign = @userId AND Unread = 'Y') b ON a.CmtCode = b.CmtCode
    INNER JOIN OUSR c ON a.UserSign = c.USERID;

    -- notification window from the user's APZ_NOSE setting (A = all)
    DECLARE @type nchar(1), @timeSetting datetime, @n int, @specify datetime = '1753-01-01';
    SELECT TOP 1 @type = TypeSpecify, @timeSetting = TimeSetting, @n = SpecifyNotification FROM dbo.APZ_NOSE WHERE UserCode = @userCode;
    IF ISNULL(@type, N'') NOT IN (N'', N'A') AND @timeSetting IS NOT NULL
        SET @specify = CASE @type
            WHEN N'D' THEN DATEADD(DAY, -@n, @timeSetting)
            WHEN N'W' THEN DATEADD(DAY, -@n * 7, @timeSetting)
            WHEN N'M' THEN DATEADD(MONTH, -@n, @timeSetting)
            WHEN N'Y' THEN DATEADD(YEAR, -@n, @timeSetting)
            ELSE '1753-01-01' END;

    -- HANA ordered the inserted rows by DueDate desc; order the result the same way
    SELECT * FROM #tempSetting WHERE DueDate >= @specify ORDER BY DueDate DESC;
END;
