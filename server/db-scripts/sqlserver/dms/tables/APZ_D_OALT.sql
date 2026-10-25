-- converted from HANA catalog export (APZ_D_OALT); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OALT]') IS NULL
CREATE TABLE dbo.[APZ_D_OALT] (
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(254) NULL,
    [Text] nvarchar(max) NULL,
    [HtmlText] nvarchar(max) NULL,
    [Active] char(1) NULL,
    [CreateDate] datetime NULL,
    [CreateTime] int NULL,
    [UpdateDate] datetime NULL,
    [UpdateTime] int NULL,
    [UserSign] int NULL,
    [UserSign2] int NULL,
    [LogInstanc] int NULL,
    CONSTRAINT [PK_APZ_D_OALT] PRIMARY KEY (Code)
);
