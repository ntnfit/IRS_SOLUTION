-- converted from HANA catalog export (APZ_D_UPIM); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_UPIM]') IS NULL
CREATE TABLE dbo.[APZ_D_UPIM] (
    [Code] nvarchar(100) NOT NULL,
    [CardCode] nvarchar(15) NULL,
    [VisitCode] nvarchar(100) NULL,
    [CreateDate] datetime NULL,
    [CreateTime] time NULL,
    [IsAvatar] char(1) NULL,
    [UserSign] int NULL,
    [AlbumId] int NULL,
    [UploadDate] datetime NULL,
    [UploadTime] time NULL,
    [FileId] nvarchar(100) NULL,
    [FileName] nvarchar(100) NULL,
    [FileNameDrive] nvarchar(100) NULL,
    [Domain] nvarchar(100) NULL,
    [Path] nvarchar(500) NULL,
    [PathThumbnail] nvarchar(500) NULL,
    [LinkDownload] nvarchar(500) NULL,
    [LinkThumbnails] nvarchar(500) NULL,
    [WebUrl] nvarchar(500) NULL,
    [MimeType] nvarchar(100) NULL,
    [Size] float NULL,
    [Extension] nvarchar(100) NULL,
    [DateCreated] datetime NULL,
    [TimestampCreated] bigint NULL,
    CONSTRAINT [PK_APZ_D_UPIM] PRIMARY KEY (Code)
);
