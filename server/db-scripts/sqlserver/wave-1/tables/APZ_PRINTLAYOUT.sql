-- converted from HANA catalog export (APZ_PRINTLAYOUT); review before production use
IF OBJECT_ID(N'dbo.[APZ_PRINTLAYOUT]') IS NULL
CREATE TABLE dbo.[APZ_PRINTLAYOUT] (
    [TypeCode] nvarchar(50) NOT NULL,
    [TypeName] nvarchar(100) NULL,
    [DocCode] nvarchar(50) NOT NULL,
    [DocName] nvarchar(254) NULL,
    [FileName] nvarchar(254) NULL,
    CONSTRAINT [PK_APZ_PRINTLAYOUT] PRIMARY KEY (TypeCode,  DocCode)
);
