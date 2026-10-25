-- converted from HANA catalog export (APZ_D_OSTC); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OSTC]') IS NULL
CREATE TABLE dbo.[APZ_D_OSTC] (
    [Code] nvarchar(100) NOT NULL,
    [CountDate] datetime NULL,
    [CountTime] time NULL,
    [StatusCode] char(1) NULL,
    [CreateDate] datetime NULL,
    [CreateTime] time NULL,
    [UserSign] int NULL,
    [UserSign2] int NULL,
    [UpdateDate] datetime NULL,
    [UpdateTime] time NULL,
    CONSTRAINT [PK_APZ_D_OSTC] PRIMARY KEY (Code)
);
