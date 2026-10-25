-- converted from HANA catalog export (APZ_D_OCHM); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_OCHM]') IS NULL
CREATE TABLE dbo.[APZ_D_OCHM] (
    [DocEntry] int NOT NULL,
    [UserSign] int NULL,
    [Status] char(1) NULL,
    [CreateTime] datetime NULL,
    [UpdateTime] datetime NULL,
    [Remark] nvarchar(max) NULL,
    CONSTRAINT [PK_APZ_D_OCHM] PRIMARY KEY (DocEntry)
);
