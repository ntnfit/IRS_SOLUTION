-- converted from HANA catalog export (APZ_DAB1); review before production use
IF OBJECT_ID(N'dbo.[APZ_DAB1]') IS NULL
CREATE TABLE dbo.[APZ_DAB1] (
    [Id] int NOT NULL,
    [Name] nvarchar(254) NULL,
    [CatId] int NULL,
    [UserSign] int NULL,
    [Layout] nvarchar(max) NULL,
    [ValidFor] tinyint NULL,
    [SqlType] char(1) NULL,
    [Query] nvarchar(max) NULL,
    [PatchFile] nvarchar(250) NULL,
    [Description] nvarchar(max) NULL,
    [LayoutDefault] nvarchar(max) NULL,
    [FormId] nvarchar(100) NULL,
    [ItemId] nvarchar(100) NULL,
    [SourceType] char(1) NULL DEFAULT 'X',
    [UdrCode] nvarchar(100) NULL,
    [MarkAsRemoved] char(1) NULL,
    CONSTRAINT [PK_APZ_DAB1] PRIMARY KEY (Id)
);
