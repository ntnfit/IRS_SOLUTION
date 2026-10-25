-- converted from HANA catalog export (APZ_D_OGGP); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OGGP]') IS NULL
CREATE TABLE dbo.[APZ_D_OGGP] (
    [CountryCode] nvarchar(50) NOT NULL,
    [CountryName] nvarchar(254) NULL,
    [Status] char(1) NULL,
    [Remarks] nvarchar(254) NULL,
    [Location] nvarchar(254) NULL,
    [UserSign] int NULL,
    [CreateDate] datetime NULL,
    [CreateTime] int NULL,
    [UpdateDate] datetime NULL,
    [UpdateTime] int NULL,
    CONSTRAINT [PK_APZ_D_OGGP] PRIMARY KEY (CountryCode)
);
