-- converted from HANA catalog export (APZ_D_OCPA); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OCPA]') IS NULL
CREATE TABLE dbo.[APZ_D_OCPA] (
    [Code] nvarchar(100) NOT NULL,
    [VisitCode] nvarchar(100) NOT NULL,
    [StoreCode] nvarchar(50) NULL,
    [CountDate] datetime NULL,
    [CountTime] time NULL,
    [StatusCode] char(1) NULL,
    [CreateDate] datetime NULL,
    [CreateTime] time NULL,
    [UserSign] int NULL,
    [UserSign2] int NULL,
    [UpdateDate] datetime NULL,
    [UpdateTime] time NULL,
    [CompetitorName] nvarchar(100) NULL,
    [Notes] nvarchar(254) NULL,
    CONSTRAINT [PK_APZ_D_OCPA] PRIMARY KEY (Code,  VisitCode)
);
