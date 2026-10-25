-- converted from HANA catalog export (APZ_D_TKLC); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_TKLC]') IS NULL
CREATE TABLE dbo.[APZ_D_TKLC] (
    [Code] nvarchar(254) NOT NULL,
    [LineNum] int NULL,
    [UserSign] int NULL,
    [DeviceId] nvarchar(100) NULL,
    [DeviceName] nvarchar(100) NULL,
    [DeviceType] nvarchar(100) NULL,
    [DeviceModel] nvarchar(100) NULL,
    [SystemName] nvarchar(50) NULL,
    [SystemVersion] nvarchar(50) NULL,
    [MacAddress] nvarchar(100) NULL,
    [BatteryPercent] decimal(16,9) NULL,
    [Latitude] nvarchar(50) NULL,
    [Longitude] nvarchar(50) NULL,
    [TrackingDate] datetime NULL,
    [TrackingTime] time NULL,
    [CreateDate] datetime NULL,
    [CreateTime] time NULL,
    [CreateType] nvarchar(100) NULL,
    [Distance] decimal(16,9) NULL,
    [AppVersion] nvarchar(100) NULL,
    CONSTRAINT [PK_APZ_D_TKLC] PRIMARY KEY (Code)
);
