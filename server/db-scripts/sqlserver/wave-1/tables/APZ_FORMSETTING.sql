-- converted from HANA catalog export (APZ_FORMSETTING); review before production use
IF OBJECT_ID(N'dbo.[APZ_FORMSETTING]') IS NULL
CREATE TABLE dbo.[APZ_FORMSETTING] (
    [FormId] nvarchar(100) NOT NULL,
    [ItemId] nvarchar(50) NOT NULL,
    [ColumnId] nvarchar(50) NOT NULL,
    [UserSign] nvarchar(20) NOT NULL,
    [LngCode] nvarchar(50) NOT NULL DEFAULT '-',
    [Width] int NULL DEFAULT 100,
    [Visible] tinyint NULL DEFAULT 1,
    [VisIndex] int NULL,
    [Editable] tinyint NULL DEFAULT 1,
    [IsReadOnly] tinyint NULL DEFAULT 0,
    [ColumnName] nvarchar(254) NULL,
    [Fixed] char(1) NULL,
    [AllowChSet] tinyint NULL DEFAULT 1,
    [VisSetting] tinyint NULL DEFAULT 1,
    [LogInstanc] int NULL,
    [UserUpdate] int NULL,
    [UpdateDate] datetime NULL,
    [GroupIndex] int NULL DEFAULT -1,
    [CurrField] nvarchar(50) NULL DEFAULT '',
    CONSTRAINT [PK_APZ_FORMSETTING] PRIMARY KEY (FormId,  ItemId,  ColumnId,  UserSign,  LngCode)
);
