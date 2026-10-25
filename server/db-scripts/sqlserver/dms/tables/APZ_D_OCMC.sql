-- converted from HANA catalog export (APZ_D_OCMC); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OCMC]') IS NULL
CREATE TABLE dbo.[APZ_D_OCMC] (
    [Code] nvarchar(100) NOT NULL,
    [Content] nvarchar(max) NULL,
    [RelatedTyp] nvarchar(20) NULL,
    [RelatedEnt] nvarchar(100) NULL,
    [UserSign] int NULL,
    [UserName] nvarchar(254) NULL,
    [CreatedTime] datetime NULL,
    [UpdatedTime] datetime NULL,
    [LogInstance] int NULL,
    CONSTRAINT [PK_APZ_D_OCMC] PRIMARY KEY (Code)
);
