-- converted from HANA catalog export (APZ_OPCN); review before production use
IF OBJECT_ID(N'dbo.[APZ_OPCN]') IS NULL
CREATE TABLE dbo.[APZ_OPCN] (
    [DocEntry] int NOT NULL,
    [Code] nvarchar(50) NULL,
    [Name] nvarchar(254) NULL,
    [Type] nvarchar(50) NULL,
    [ValueMin] decimal(19,6) NULL,
    [ValueMax] decimal(19,6) NULL,
    [Active] char(1) NULL DEFAULT 'Y',
    [Remark] nvarchar(max) NULL,
    [UserSign] nvarchar(50) NULL,
    [CreateDate] datetime NULL,
    [UserSign2] nvarchar(50) NULL,
    [UpdateDate] datetime NULL,
    CONSTRAINT [PK_APZ_OPCN] PRIMARY KEY (DocEntry)
);
