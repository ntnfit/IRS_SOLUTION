-- converted from HANA catalog export (APZ_RDOC); review before production use
IF OBJECT_ID(N'dbo.[APZ_RDOC]') IS NULL
CREATE TABLE dbo.[APZ_RDOC] (
    [ID] int NOT NULL,
    [TypeCode] nvarchar(50) NULL,
    [TypeName] nvarchar(254) NULL,
    [DocCode] nvarchar(50) NULL,
    [DocName] nvarchar(254) NULL,
    [Menu] nvarchar(50) NULL,
    [CreateDate] datetime NULL,
    [UpdateDate] datetime NULL,
    [UserSign] nvarchar(50) NULL,
    [UserSign2] nvarchar(50) NULL,
    [Status] varchar(1) NULL DEFAULT 'A',
    [Description] nvarchar(254) NULL,
    [Text] nvarchar(max) NULL,
    CONSTRAINT [PK_APZ_RDOC] PRIMARY KEY (ID)
);
