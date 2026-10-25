-- converted from HANA catalog export (APZ_D_OADT); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OADT]') IS NULL
CREATE TABLE dbo.[APZ_D_OADT] (
    [ID] int NOT NULL,
    [UserID] int NOT NULL,
    [UserName] nvarchar(50) NULL,
    [DeviceID] nvarchar(100) NULL,
    [DeviceName] nvarchar(100) NULL,
    [Domain] nvarchar(50) NULL,
    [DatabaseName] nvarchar(50) NULL,
    [AppType] varchar(1) NULL,
    [DeviceToken] nvarchar(500) NULL,
    [OSName] nvarchar(50) NULL,
    [OSVersion] nvarchar(50) NULL,
    [MacAddress] nchar(50) NULL,
    [AppCode] nvarchar(50) NULL,
    [AppVersion] nvarchar(50) NULL,
    [CreateTime] datetime NULL,
    [UpdateTime] datetime NULL,
    [FailCount] int NULL,
    CONSTRAINT [PK_APZ_D_OADT] PRIMARY KEY (ID)
);
