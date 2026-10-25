-- converted from HANA catalog export (APZ_D_PMO3); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_PMO3]') IS NULL
CREATE TABLE dbo.[APZ_D_PMO3] (
    [Code] nvarchar(50) NOT NULL,
    [CardCode] nvarchar(50) NOT NULL,
    [GroupCode] nvarchar(100) NOT NULL DEFAULT '-1',
    [StatusCode] char(1) NULL,
    [Quantity] decimal(19,6) NULL,
    [RegisterRemarks] nvarchar(256) NULL,
    [CreateDate] datetime NULL,
    [CreateTime] int NULL,
    [UpdateDate] datetime NULL,
    [UpdateTime] smallint NULL,
    [UserSign] int NULL,
    [UserSign2] int NULL,
    [LogIntance] int NULL,
    CONSTRAINT [PK_APZ_D_PMO3] PRIMARY KEY (Code,  CardCode,  GroupCode)
);
