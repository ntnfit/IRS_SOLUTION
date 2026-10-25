-- converted from HANA catalog export (APZ_D_GGP1); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_GGP1]') IS NULL
CREATE TABLE dbo.[APZ_D_GGP1] (
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(254) NULL,
    [CountryCode] nvarchar(50) NULL,
    [FatherCode] nvarchar(50) NULL,
    [Status] varchar(1) NULL,
    [Type] nvarchar(50) NULL,
    [Remarks] nvarchar(254) NULL,
    [Id] int NULL,
    [FatherId] int NULL,
    [NameWithType] nvarchar(254) NULL,
    [NamePath] nvarchar(254) NULL,
    [NamePathWithType] nvarchar(254) NULL,
    [CreateDate] datetime NULL,
    [CreateTime] int NULL,
    [UpdateDate] datetime NULL,
    [UpdateTime] int NULL,
    CONSTRAINT [PK_APZ_D_GGP1] PRIMARY KEY (Code)
);
