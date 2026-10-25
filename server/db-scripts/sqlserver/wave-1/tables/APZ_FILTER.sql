-- converted from HANA catalog export (APZ_FILTER); review before production use
IF OBJECT_ID(N'dbo.[APZ_FILTER]') IS NULL
CREATE TABLE dbo.[APZ_FILTER] (
    [Code] nvarchar(100) NOT NULL,
    [Name] nvarchar(254) NULL,
    [FilterText] nvarchar(max) NULL,
    [Valid] tinyint NULL,
    [ObjType] nvarchar(20) NULL,
    CONSTRAINT [PK_APZ_FILTER] PRIMARY KEY (Code)
);
