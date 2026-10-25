-- converted from HANA catalog export (APZ_D_IMAGEALBUM); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_IMAGEALBUM]') IS NULL
CREATE TABLE dbo.[APZ_D_IMAGEALBUM] (
    [Id] int NOT NULL,
    [Name] nvarchar(254) NOT NULL,
    [Remarks] nvarchar(500) NULL,
    CONSTRAINT [PK_APZ_D_IMAGEALBUM] PRIMARY KEY (Id)
);
