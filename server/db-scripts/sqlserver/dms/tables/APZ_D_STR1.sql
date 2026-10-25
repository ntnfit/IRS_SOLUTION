-- converted from HANA catalog export (APZ_D_STR1); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_STR1]') IS NULL
CREATE TABLE dbo.[APZ_D_STR1] (
    [Id] int NOT NULL,
    [RouteCode] varchar(50) NOT NULL,
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(254) NULL,
    [CountryCode] varchar(50) NULL,
    [Type] char(1) NULL,
    [Status] char(1) NULL,
    [Remarks] nvarchar(254) NULL,
    [EmpId] int NULL,
    [SuperVisor] int NULL,
    [GgpCode] nvarchar(50) NULL,
    [FatherId] int NULL,
    [LevelGgp] int NULL,
    [SalesAdmin] nvarchar(1000) NULL,
    [LogInstance] int NULL,
    CONSTRAINT [PK_APZ_D_STR1] PRIMARY KEY (Id,  RouteCode)
);
