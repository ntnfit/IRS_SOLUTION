-- converted from HANA catalog export (APZ_OFCT); review before production use
IF OBJECT_ID(N'dbo.[APZ_OFCT]') IS NULL
CREATE TABLE dbo.[APZ_OFCT] (
    [Code] int NOT NULL,
    [Name] nvarchar(100) NULL,
    [UserSign] smallint NULL,
    [StartDate] datetime NULL,
    [EndDate] datetime NULL,
    [FormView] char(1) NULL DEFAULT 'D',
    [BPCode] nvarchar(15) NULL,
    [SplCode] int NULL,
    [PriceList] smallint NULL,
    [CreateDate] datetime NULL,
    [OriginType] char(1) NULL,
    CONSTRAINT [PK_APZ_OFCT] PRIMARY KEY (Code)
);
