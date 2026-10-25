USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.Apz_D_Delivery_SaveLocationNote
    @type char(1) = 'A'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @type = 'A'
        INSERT INTO dbo.APZ_D_DLO4 (ShipCode, Note)
        SELECT ShipCode, Note FROM #APZ_D_DLO4;
    ELSE IF @type = 'D'
        DELETE target
        FROM dbo.APZ_D_DLO4 target
        INNER JOIN #APZ_D_DLO4 source ON source.ID = target.ID;

    SELECT 0 AS ErrorCode, N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Apz_D_Delivery_SaveBPNote
    @type char(1) = 'A'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @type = 'A'
        INSERT INTO dbo.APZ_D_DLO5 (CardCode, Note)
        SELECT CardCode, Note FROM #APZ_D_DLO5;
    ELSE IF @type = 'D'
        DELETE target
        FROM dbo.APZ_D_DLO5 target
        INNER JOIN #APZ_D_DLO5 source ON source.ID = target.ID;

    SELECT 0 AS ErrorCode, N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_UploadFile_Save
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @objCode nvarchar(50) = ISNULL((SELECT TOP (1) ObjectCode FROM #APZ_D_UUPIM), N'');
    DECLARE @objType nvarchar(50) = ISNULL((SELECT TOP (1) ObjectType FROM #APZ_D_UUPIM), N'');

    DELETE FROM dbo.APZ_D_UUPIM WHERE ObjectCode = @objCode AND ObjectType = @objType;

    INSERT INTO dbo.APZ_D_UUPIM
    (
        Code, UserSign, CreateDate, CreateTime, UploadDate, UploadTime, ObjectType,
        ObjectCode, AlbumId, FileId, FileName, FileNameDrive, Domain, Path,
        PathThumbnail, LinkDownload, LinkThumbnails, WebUrl, MimeType, Size,
        Extension, DateCreated, TimestampCreated
    )
    SELECT Code, UserSign, CreateDate, CreateTime, UploadDate, UploadTime, ObjectType,
           ObjectCode, AlbumId, FileId, FileName, FileNameDrive, Domain, Path,
           PathThumbnail, LinkDownload, LinkThumbnails, WebUrl, MimeType, Size,
           Extension, DateCreated, TimestampCreated
    FROM #APZ_D_UUPIM;

    SELECT @objCode AS ObjectCode, N'' AS ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Image_GetListItem
    @fromCode nvarchar(50),
    @toCode nvarchar(50),
    @itemGroup nvarchar(4000)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH latest_image AS
    (
        SELECT image.*,
               CONVERT(date, image.UploadDate) AS UploadOnlyDate,
               ROW_NUMBER() OVER
               (
                   PARTITION BY image.ObjectCode, image.ObjectType
                   ORDER BY image.UploadDate DESC, image.UploadTime DESC
               ) AS rn
        FROM dbo.APZ_D_UUPIM image
        WHERE image.ObjectType = '810039'
    )
    SELECT item.ItemCode, item.ItemName, item.ItmsGrpCod, image.*
    FROM dbo.OITM item
    LEFT JOIN latest_image image ON image.ObjectCode = item.ItemCode
    WHERE ((ISNULL(@fromCode, N'') <> N'' AND ISNULL(@toCode, N'') <> N'' AND item.ItemCode BETWEEN @fromCode AND @toCode)
           OR ISNULL(@fromCode, N'') = N'' OR ISNULL(@toCode, N'') = N'')
      AND (ISNULL(@itemGroup, N'') = N''
           OR (@itemGroup = N'N' AND item.ItmsGrpCod IS NULL)
           OR @itemGroup = N'A'
           OR CONVERT(nvarchar(50), item.ItmsGrpCod) IN (SELECT DATA FROM dbo.Split(@itemGroup, N';')))
      AND ISNULL(image.rn, 0) <= 1
    ORDER BY item.ItmsGrpCod, item.ItemCode, image.UploadOnlyDate DESC, image.UploadTime;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Image_GetList
    @fromDate datetime,
    @toDate datetime,
    @empID int = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @userId int = ISNULL((SELECT TOP (1) userId FROM dbo.OHEM WHERE empID = @empID), -1);
    DECLARE @userName nvarchar(50) = ISNULL((SELECT TOP (1) USER_CODE FROM dbo.OUSR WHERE USERID = @userId), N'');
    CREATE TABLE #tmpStore
    (
        CardCode nvarchar(50) COLLATE SQL_Latin1_General_CP850_CI_AS,
        CardName nvarchar(254) COLLATE SQL_Latin1_General_CP850_CI_AS
    );

    EXEC dbo.APZ_D_FilterStores N'', N'', 'S', @userName, N'#tmpStore';

    SELECT image.*, partner.CardName, CONVERT(date, image.UploadDate) AS UploadOnlyDate
    FROM dbo.APZ_D_UPIM image
    LEFT JOIN dbo.OCRD partner ON partner.CardCode = image.CardCode
    WHERE CONVERT(date, image.UploadDate) BETWEEN CONVERT(date, @fromDate) AND CONVERT(date, @toDate)
      AND image.CardCode IN (SELECT CardCode FROM #tmpStore)
    ORDER BY UploadOnlyDate DESC, image.UploadTime;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_PromotionSetup_CheckUsed
    @type char(1),
    @id int
AS
BEGIN
    SET NOCOUNT ON;

    IF (@type = 'C' AND EXISTS (SELECT 1 FROM dbo.APZ_D_OPMO WHERE CategoryID = @id))
       OR (@type = 'T' AND EXISTS (SELECT 1 FROM dbo.APZ_D_OPMO WHERE TypeID = @id))
       OR (@type NOT IN ('C', 'T') AND EXISTS (SELECT 1 FROM dbo.APZ_D_OPMO WHERE GroupID = @id))
    BEGIN
        SELECT N'Used' AS Result;
        RETURN;
    END;

    SELECT N'' AS Result;
END;
GO
