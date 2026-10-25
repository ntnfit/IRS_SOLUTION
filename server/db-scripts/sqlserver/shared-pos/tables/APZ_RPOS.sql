-- converted from HANA catalog export (APZ_RPOS); review before production use
IF OBJECT_ID(N'dbo.[APZ_RPOS]') IS NULL
CREATE TABLE dbo.[APZ_RPOS] (
    [PosID] int NOT NULL,
    [PosCode] nvarchar(50) NOT NULL,
    [Store] nvarchar(50) NULL,
    [Dscription] nvarchar(100) NULL,
    [UserSign] smallint NULL,
    [UpdateDate] datetime NULL,
    [CreateDate] datetime NULL,
    [UserSign2] smallint NULL,
    [Inactive] char(1) NULL DEFAULT 'N',
    CONSTRAINT [PK_APZ_RPOS] PRIMARY KEY (PosID)
);
