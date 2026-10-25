-- converted from HANA catalog export (APZ_D_OPMO); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OPMO]') IS NULL
CREATE TABLE dbo.[APZ_D_OPMO] (
    [Code] nvarchar(100) NOT NULL,
    [Name] nvarchar(500) NULL,
    [FromDate] datetime NULL,
    [ToDate] datetime NULL,
    [Status] nvarchar(10) NULL,
    [StartType] char(1) NULL,
    [CategoryID] int NULL,
    [TypeID] int NULL,
    [GroupID] int NULL,
    [ConditionDesc] nvarchar(max) NULL,
    [BenefitDesc] nvarchar(max) NULL,
    [AllowOverlap] char(1) NULL,
    [SplitGroup] char(1) NULL,
    [CreateDate] datetime NULL,
    [CreateTime] int NULL,
    [UpdateDate] datetime NULL,
    [UpdateTime] int NULL,
    [LogInstance] int NULL,
    [UserSign] int NULL,
    CONSTRAINT [PK_APZ_D_OPMO] PRIMARY KEY (Code)
);
