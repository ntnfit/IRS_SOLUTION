-- converted from HANA catalog export (APZ_NOSE); review before production use
IF OBJECT_ID(N'dbo.[APZ_NOSE]') IS NULL
CREATE TABLE dbo.[APZ_NOSE] (
    [UserCode] nvarchar(25) NOT NULL,
    [TypeDay] nvarchar(25) NULL,
    [FromDay] int NULL,
    [ToDay] int NULL,
    [PickDay] nvarchar(100) NULL,
    [TypeTime] nvarchar(25) NULL,
    [FromTime] int NULL,
    [ToTime] int NULL,
    [SpecifyNotification] int NULL,
    [TypeSpecify] nvarchar(25) NULL,
    [TimeSetting] datetime NULL,
    CONSTRAINT [PK_APZ_NOSE] PRIMARY KEY (UserCode)
);
