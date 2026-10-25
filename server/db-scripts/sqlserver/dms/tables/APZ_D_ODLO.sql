-- converted from HANA catalog export (APZ_D_ODLO); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_ODLO]') IS NULL
CREATE TABLE dbo.[APZ_D_ODLO] (
    [DocEntry] int NULL,
    [DOEntry] int NULL,
    [CardCode] nvarchar(50) NULL,
    [CardName] nvarchar(100) NULL,
    [ContactPerson] nvarchar(100) NULL,
    [Phone] nvarchar(20) NULL,
    [ScheduledDate] datetime NULL,
    [ScheduledTime] smallint NULL,
    [ShipToCode] nvarchar(100) NULL,
    [ShipToAddress] nvarchar(max) NULL,
    [Amount] decimal(19,6) NULL,
    [Remark] nvarchar(max) NULL,
    [LateTime] datetime NULL,
    [LateReason] nvarchar(254) NULL,
    [ActualAmount] decimal(19,6) NULL,
    [DeliveredFull] tinyint NULL DEFAULT 1,
    [AddressConfirm] nvarchar(100) NULL,
    [Latitude] nvarchar(50) NULL,
    [Longtitude] nvarchar(50) NULL,
    [StartTime] datetime NULL,
    [ConfirmTime] datetime NULL,
    [Feedback] nvarchar(max) NULL,
    [UserSign] nvarchar(100) NULL,
    [Status] nvarchar(50) NULL
);
