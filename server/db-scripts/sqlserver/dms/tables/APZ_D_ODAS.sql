-- converted from HANA catalog export (APZ_D_ODAS); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_ODAS]') IS NULL
CREATE TABLE dbo.[APZ_D_ODAS] (
    [UserId] int NULL,
    [DeviceId] nvarchar(100) NULL,
    [DeviceName] nvarchar(100) NULL,
    [DeviceType] nvarchar(100) NULL,
    [DeviceModel] nvarchar(100) NULL,
    [SystemName] nvarchar(50) NULL,
    [SystemVersion] nvarchar(50) NULL,
    [MacAddress] nvarchar(100) NULL,
    [BatteryPercent] decimal(16,9) NULL,
    [AppVersion] nvarchar(100) NULL,
    [CreateDate] datetime NULL,
    [FormId] nvarchar(100) NULL,
    [Setting] nvarchar(max) NULL
);
