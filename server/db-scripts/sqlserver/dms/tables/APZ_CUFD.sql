-- converted from HANA catalog export (APZ_CUFD); review before production use
IF OBJECT_ID(N'dbo.[APZ_CUFD]') IS NULL
CREATE TABLE dbo.[APZ_CUFD] (
    [TableID] nvarchar(20) NOT NULL,
    [FieldID] smallint NOT NULL,
    [AliasID] nvarchar(50) NOT NULL,
    [Descr] nvarchar(80) NULL,
    [TypeID] char(1) NULL,
    [EditType] char(1) NULL,
    [SizeID] smallint NULL,
    [EditSize] smallint NULL,
    [Dflt] nvarchar(254) NULL,
    [NotNull] char(1) NULL DEFAULT 'N',
    [IndexID] char(1) NULL DEFAULT 'N',
    [RTable] nvarchar(20) NULL,
    [RField] smallint NULL,
    [Action] char(1) NULL,
    [Sys] char(1) NULL DEFAULT 'N',
    [DfltDate] datetime NULL,
    [RelUDO] nvarchar(20) NULL,
    [ValidRule] nvarchar(254) NULL,
    [RelSO] nvarchar(20) NULL,
    [RThrdPTab] nvarchar(100) NULL,
    [RThrdPFld] nvarchar(100) NULL,
    CONSTRAINT [PK_APZ_CUFD] PRIMARY KEY (TableID,  FieldID)
);
