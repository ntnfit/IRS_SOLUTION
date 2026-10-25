USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_QuestionMasterData_FindNextLastFrist
    @id int = 0,
    @type int
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @minId int = ISNULL((SELECT MIN(DocEntry) FROM dbo.APZ_D_OCHM), 0);
    DECLARE @maxId int = ISNULL((SELECT MAX(DocEntry) FROM dbo.APZ_D_OCHM), 0);

    SELECT TOP (1) *
    FROM dbo.APZ_D_OCHM
    WHERE DocEntry = CASE @type
        WHEN 0 THEN @minId
        WHEN 1 THEN CASE WHEN @id <= @minId THEN @maxId ELSE ISNULL((SELECT MAX(DocEntry) FROM dbo.APZ_D_OCHM WHERE DocEntry < @id), @maxId) END
        WHEN 2 THEN CASE WHEN @id >= @maxId THEN @minId ELSE ISNULL((SELECT MIN(DocEntry) FROM dbo.APZ_D_OCHM WHERE DocEntry > @id), @minId) END
        ELSE @maxId
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_QuestionMasterData_Save
    @type char(1) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @id int;
    IF @type = 'U'
    BEGIN
        SELECT TOP (1) @id = DocEntry FROM #APZ_D_OCHM;
        DELETE FROM dbo.APZ_D_CHM1 WHERE DocEntry = @id;
        DELETE FROM dbo.APZ_D_OCHM WHERE DocEntry = @id;
    END
    ELSE
    BEGIN
        SELECT @id = ISNULL(MAX(DocEntry), 0) + 1 FROM dbo.APZ_D_OCHM WITH (UPDLOCK, HOLDLOCK);
        UPDATE #APZ_D_OCHM SET DocEntry = @id;
    END;

    UPDATE #APZ_D_CHM1 SET DocEntry = @id;
    INSERT INTO dbo.APZ_D_OCHM (DocEntry, UserSign, Status, CreateTime, UpdateTime, Remark)
    SELECT DocEntry, UserSign, Status, CreateTime, UpdateTime, Remark FROM #APZ_D_OCHM;
    INSERT INTO dbo.APZ_D_CHM1 (DocEntry, LineNum, VisOrder, Question)
    SELECT DocEntry, LineNum, VisOrder, Question FROM #APZ_D_CHM1;

    SELECT @id AS DocEntry, N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SettingReport_FindNextLastFrist
    @id bigint,
    @type int
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @minId int = ISNULL((SELECT MIN(DocEntry) FROM dbo.APZ_D_OSRT), 0);
    DECLARE @maxId int = ISNULL((SELECT MAX(DocEntry) FROM dbo.APZ_D_OSRT), 0);

    SELECT TOP (1) *
    FROM dbo.APZ_D_OSRT
    WHERE DocEntry = CASE @type
        WHEN 0 THEN @minId
        WHEN 1 THEN CASE WHEN @id <= @minId THEN @maxId ELSE ISNULL((SELECT MAX(DocEntry) FROM dbo.APZ_D_OSRT WHERE DocEntry < @id), @maxId) END
        WHEN 2 THEN CASE WHEN @id >= @maxId THEN @minId ELSE ISNULL((SELECT MIN(DocEntry) FROM dbo.APZ_D_OSRT WHERE DocEntry > @id), @minId) END
        ELSE @maxId
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SettingReport_Save
    @type char(1) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @id int;
    IF @type = 'U'
    BEGIN
        SELECT TOP (1) @id = DocEntry FROM #APZ_D_OSRT;
        DELETE FROM dbo.APZ_D_SRT3 WHERE DocEntry = @id;
        DELETE FROM dbo.APZ_D_SRT2 WHERE DocEntry = @id;
        DELETE FROM dbo.APZ_D_SRT1 WHERE DocEntry = @id;
        DELETE FROM dbo.APZ_D_OSRT WHERE DocEntry = @id;
    END
    ELSE
    BEGIN
        SELECT @id = ISNULL(MAX(DocEntry), 0) + 1 FROM dbo.APZ_D_OSRT WITH (UPDLOCK, HOLDLOCK);
        UPDATE #APZ_D_OSRT SET DocEntry = @id;
    END;

    UPDATE #APZ_D_SRT1 SET DocEntry = @id;
    UPDATE #APZ_D_SRT2 SET DocEntry = @id;
    UPDATE #APZ_D_SRT3 SET DocEntry = @id;

    INSERT INTO dbo.APZ_D_OSRT SELECT * FROM #APZ_D_OSRT;
    INSERT INTO dbo.APZ_D_SRT1 SELECT * FROM #APZ_D_SRT1;
    INSERT INTO dbo.APZ_D_SRT2 SELECT * FROM #APZ_D_SRT2;
    INSERT INTO dbo.APZ_D_SRT3 (DocEntry, DashboardId, DashboardName, Layout)
    SELECT DocEntry,
           ROW_NUMBER() OVER (ORDER BY DashboardName),
           DashboardName,
           Layout
    FROM #APZ_D_SRT3;

    SELECT @id AS DocEntry, N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Save_AppSetting
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DELETE target
    FROM dbo.APZ_D_ODAS target
    INNER JOIN #APZ_D_ODAS source
        ON source.UserId = target.UserId AND source.DeviceId = target.DeviceId AND source.FormId = target.FormId;

    INSERT INTO dbo.APZ_D_ODAS
        (UserId, DeviceId, DeviceName, DeviceType, DeviceModel, SystemName, SystemVersion,
         MacAddress, BatteryPercent, AppVersion, CreateDate, FormId, Setting)
    SELECT UserId, DeviceId, DeviceName, DeviceType, DeviceModel, SystemName, SystemVersion,
           MacAddress, BatteryPercent, AppVersion, CONVERT(date, GETDATE()), FormId, Setting
    FROM #APZ_D_ODAS;

    IF @@ROWCOUNT <= 0
        SELECT 400 AS Code, N'Insert table header fail' AS Message;
    ELSE
        SELECT 200 AS Code, N'' AS Message;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_VisitSchedule_GetData
    @userName nvarchar(50) = N'',
    @date datetime
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #tmpStore
    (
        CardCode nvarchar(50) COLLATE SQL_Latin1_General_CP850_CI_AS,
        CardName nvarchar(254) COLLATE SQL_Latin1_General_CP850_CI_AS
    );
    EXEC dbo.APZ_D_FilterStores N'', N'', 'S', @userName, N'#tmpStore';

    ;WITH schedules AS
    (
        SELECT ISNULL(schedule.Code, partner.CardCode) AS Code,
               ISNULL(schedule.Name, partner.CardName) AS Name,
               ISNULL(schedule.Address, partner.Address) AS Address,
               schedule.Description, schedule.StartDate,
               ISNULL(schedule.Finished, 'N') AS Finished,
               ISNULL(schedule.Recurring, 'W') AS Recurring,
               schedule.RecurNum, schedule.RecurNote,
               ISNULL(schedule.EveryDay, 'N') AS EveryDay,
               ISNULL(schedule.DayOfWeek, 'N') AS DayOfWeek,
               ISNULL(schedule.Mon, 'N') AS Mon, ISNULL(schedule.Tue, 'N') AS Tue,
               ISNULL(schedule.Wed, 'N') AS Wed, ISNULL(schedule.Thu, 'N') AS Thu,
               ISNULL(schedule.Fri, 'N') AS Fri, ISNULL(schedule.Sat, 'N') AS Sat,
               ISNULL(schedule.Sun, 'N') AS Sun,
               ISNULL(schedule.DayOfMonth, 'N') AS DayOfMonth,
               ISNULL(schedule.ListOfDate, N'') AS ListOfDate,
               partner.U_Latitude, partner.U_Longitude
        FROM dbo.OCRD partner
        LEFT JOIN dbo.APZ_D_OOVF schedule ON schedule.Code = partner.CardCode
        WHERE partner.CardCode IN (SELECT CardCode FROM #tmpStore)
    )
    SELECT Code AS CardCode, Name AS CardName, U_Latitude, U_Longitude
    FROM schedules
    WHERE StartDate < @date AND Finished = 'N'
      AND
      (
          EveryDay = 'Y'
          OR (DayOfMonth = 'Y' AND CONVERT(nvarchar(3), DAY(@date)) IN (SELECT DATA FROM dbo.Split(ListOfDate, N',')))
          OR (DayOfWeek = 'Y' AND
              CASE DATENAME(weekday, @date)
                  WHEN 'Monday' THEN Mon WHEN 'Tuesday' THEN Tue WHEN 'Wednesday' THEN Wed
                  WHEN 'Thursday' THEN Thu WHEN 'Friday' THEN Fri WHEN 'Saturday' THEN Sat
                  WHEN 'Sunday' THEN Sun ELSE 'N'
              END = 'Y')
      );
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_RPT_SalesCoverage
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH latest_promotion AS
    (
        SELECT promotion.*,
               ROW_NUMBER() OVER (PARTITION BY promotion.CardCode ORDER BY promotion.CreateDate DESC) AS rn
        FROM dbo.APZ_D_PMO3 promotion
    )
    SELECT DISTINCT partner.CardCode, partner.CardName, partner.Address, partner.Cellular, partner.CntctPrsn,
           partner.U_Latitude, partner.U_Longitude, image.LinkDownload, image.LinkThumbnails, image.WebUrl,
           contact.Tel1, contact.BirthDate,
           LTRIM(RTRIM(CASE
               WHEN LEN(partner.Address) - LEN(REPLACE(partner.Address, ',', '')) > 1
                   THEN PARSENAME(REPLACE(partner.Address, ',', '.'), 2)
               ELSE PARSENAME(REPLACE(partner.Address, ',', '.'), 1)
           END)) AS TinhTP,
           CASE WHEN promotion.CreateDate BETWEEN DATEADD(year, -1, CONVERT(date, GETDATE())) AND CONVERT(date, GETDATE()) THEN 'red' ELSE 'dodgerblue' END AS MarkerColor,
           route.RouteCode, route.RouteName
    FROM dbo.OCRD partner
    LEFT JOIN dbo.APZ_D_STR2 assignment ON assignment.CardCode = partner.CardCode
    LEFT JOIN dbo.APZ_D_OSTR route ON route.RouteCode = assignment.RouteCode
    LEFT JOIN dbo.OCPR contact ON contact.CardCode = partner.CardCode AND contact.Name = partner.CntctPrsn
    INNER JOIN dbo.APZ_D_UPIM image ON image.CardCode = partner.CardCode AND image.IsAvatar = 'Y'
    LEFT JOIN latest_promotion promotion ON promotion.CardCode = partner.CardCode AND promotion.rn = 1
    WHERE partner.U_SubType = 'S';
END;
GO
