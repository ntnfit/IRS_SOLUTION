-- converted from HANA catalog export (APZ_D_VISIT); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_VISIT]') IS NULL
CREATE TABLE dbo.[APZ_D_VISIT] (
    [DocEntry] int NOT NULL,
    [Guid] nvarchar(100) NOT NULL,
    [UserSign] int NULL,
    [StoreCode] nvarchar(50) NULL,
    [StoreName] nvarchar(500) NULL,
    [Address] nvarchar(max) NULL,
    [CreateDate] datetime NULL,
    [CreateTime] time NULL,
    [LogInstance] int NULL,
    [StartDate] datetime NULL,
    [StartTime] time NULL,
    [FinishedDate] datetime NULL,
    [FinishedTime] time NULL,
    [Geolocation] nvarchar(100) NULL,
    CONSTRAINT [PK_APZ_D_VISIT] PRIMARY KEY (DocEntry)
);
