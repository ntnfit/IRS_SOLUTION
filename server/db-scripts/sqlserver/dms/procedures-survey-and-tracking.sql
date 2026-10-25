USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SurveyDMS_Save
    @type char(1) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Id int;
    IF @type = 'U'
    BEGIN
        SELECT TOP (1) @Id = DocEntry FROM #APZ_D_OSUV;
        DELETE FROM dbo.APZ_D_SUV1 WHERE DocEntry = @Id;
        DELETE FROM dbo.APZ_D_OSUV WHERE DocEntry = @Id;
    END
    ELSE
    BEGIN
        SELECT @Id = ISNULL(MAX(DocEntry), 0) + 1 FROM dbo.APZ_D_OSUV WITH (UPDLOCK, HOLDLOCK);
        UPDATE #APZ_D_OSUV SET DocEntry = @Id;
    END;

    INSERT INTO dbo.APZ_D_OSUV
        (DocEntry, PersonName, Address, Phone, Email, Notes, Rate, SurveyQuessionId, ClgCode, CreateTime, UpdateTime)
    SELECT DocEntry, PersonName, Address, Phone, Email, Notes, Rate, SurveyQuessionId, ClgCode, CreateTime, UpdateTime
    FROM #APZ_D_OSUV;

    INSERT INTO dbo.APZ_D_SUV1 (DocEntry, LineNum, VisOrder, Question, Answer)
    SELECT @Id, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1, VisOrder, Question, Answer
    FROM #APZ_D_SUV1;

    SELECT @Id AS DocEntry, N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_SettingApprovalDetail_Save
    @type char(1) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Id int;
    IF @type = 'U'
    BEGIN
        SELECT TOP (1) @Id = DocEntry FROM #APZ_D_OSAD;
        DELETE FROM dbo.APZ_D_SAD2 WHERE DocEntry = @Id;
        DELETE FROM dbo.APZ_D_SAD1 WHERE DocEntry = @Id;
        DELETE FROM dbo.APZ_D_OSAD WHERE DocEntry = @Id;
    END
    ELSE
    BEGIN
        SELECT @Id = ISNULL(MAX(DocEntry), 0) + 1 FROM dbo.APZ_D_OSAD WITH (UPDLOCK, HOLDLOCK);
        UPDATE #APZ_D_OSAD SET DocEntry = @Id;
    END;

    UPDATE #APZ_D_SAD1 SET DocEntry = @Id;
    UPDATE #APZ_D_SAD2 SET DocEntry = @Id;

    INSERT INTO dbo.APZ_D_SAD2
    SELECT * FROM #APZ_D_SAD2;
    INSERT INTO dbo.APZ_D_SAD1
    SELECT * FROM #APZ_D_SAD1;
    INSERT INTO dbo.APZ_D_OSAD
    SELECT * FROM #APZ_D_OSAD;

    SELECT @Id AS DocEntry, N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_GetAllAvatarStore
    @saleUser nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #tmpStore
    (
        CardCode nvarchar(50) COLLATE SQL_Latin1_General_CP850_CI_AS,
        CardName nvarchar(254) COLLATE SQL_Latin1_General_CP850_CI_AS
    );
    EXEC dbo.APZ_D_FilterStores N'', N'', 'S', @saleUser, N'#tmpStore';

    SELECT image.*
    FROM dbo.OCRD partner
    INNER JOIN dbo.APZ_D_UPIM image ON image.CardCode = partner.CardCode AND image.IsAvatar = 'Y'
    WHERE partner.CardCode IN (SELECT CardCode FROM #tmpStore);
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_GetLastestAllEmployeeTrackingLocation
    @userName nvarchar(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @userId int = ISNULL((SELECT TOP (1) UserId FROM dbo.OUSR WHERE USER_CODE = @userName), -1);
    CREATE TABLE #tmpEmployees (EmpId int);
    EXEC dbo.APZ_D_FilterEmployees @userId, N'#tmpEmployees';

    ;WITH tracking AS
    (
        SELECT location.*,
               employee.empID,
               CONCAT(ISNULL(employee.firstName, N''), N' ', ISNULL(employee.middleName, N''), N' ', ISNULL(employee.lastName, N'')) AS empName,
               avatar.LinkDownload,
               avatar.LinkThumbnails,
               ROW_NUMBER() OVER
               (
                   PARTITION BY location.UserSign
                   ORDER BY location.TrackingDate DESC, location.TrackingTime DESC
               ) AS rn
        FROM dbo.APZ_D_TKLC location
        LEFT JOIN dbo.APZ_D_UUPIM avatar ON avatar.UserSign = location.UserSign AND avatar.ObjectType = '1470000171'
        LEFT JOIN dbo.OHEM employee ON employee.userId = location.UserSign
        WHERE CONVERT(date, location.TrackingDate) = CONVERT(date, GETDATE())
          AND location.UserSign IN
              (SELECT employee.userId FROM dbo.OHEM employee WHERE employee.empID IN (SELECT EmpId FROM #tmpEmployees))
    )
    SELECT * FROM tracking WHERE rn = 1;
END;
GO
