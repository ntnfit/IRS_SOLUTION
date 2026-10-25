-- converted from HANA catalog export (APZ_D_SUDF); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_SUDF]') IS NULL
CREATE TABLE dbo.[APZ_D_SUDF] (
    [MenuID] int NOT NULL,
    [TableID] nvarchar(250) NOT NULL,
    [FieldID] int NOT NULL,
    [IsActive] nvarchar(5) NULL,
    [IsEditable] nvarchar(5) NULL,
    [CreateDate] datetime NULL,
    [UserSign] int NULL,
    CONSTRAINT [PK_APZ_D_SUDF] PRIMARY KEY (MenuID,  TableID,  FieldID)
);
