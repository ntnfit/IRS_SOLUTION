-- converted from HANA catalog export (APZ_ITEM_LISTCFLFIELD); review before production use
IF OBJECT_ID(N'dbo.[APZ_ITEM_LISTCFLFIELD]') IS NULL
CREATE TABLE dbo.[APZ_ITEM_LISTCFLFIELD] (
    [FieldName] nvarchar(30) NOT NULL,
    [QueryField] nvarchar(300) NULL,
    [JoinAlias] nvarchar(3) NULL,
    CONSTRAINT [PK_APZ_ITEM_LISTCFLFIELD] PRIMARY KEY (FieldName)
);
