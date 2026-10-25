-- converted from HANA catalog export (APZ_FORMULA); review before production use
IF OBJECT_ID(N'dbo.[APZ_FORMULA]') IS NULL
CREATE TABLE dbo.[APZ_FORMULA] (
    [FormID] nvarchar(50) NOT NULL,
    [ItemId] nvarchar(50) NOT NULL,
    [MethodID] nvarchar(50) NOT NULL,
    [Description] nvarchar(254) NULL,
    [Formula] nvarchar(max) NULL,
    [Active] char(1) NULL,
    CONSTRAINT [PK_APZ_FORMULA] PRIMARY KEY (FormID,  MethodID,  ItemId)
);
