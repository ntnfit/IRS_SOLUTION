-- converted from HANA catalog export (APZ_UFD1); review before production use
IF OBJECT_ID(N'dbo.[APZ_UFD1]') IS NULL
CREATE TABLE dbo.[APZ_UFD1] (
    [TableID] nvarchar(20) NOT NULL,
    [FieldID] smallint NOT NULL,
    [IndexID] smallint NOT NULL,
    [FldValue] nvarchar(254) NULL,
    [Descr] nvarchar(254) NULL,
    [FldDate] datetime NULL,
    CONSTRAINT [PK_APZ_UFD1] PRIMARY KEY (TableID,  FieldID,  IndexID)
);
