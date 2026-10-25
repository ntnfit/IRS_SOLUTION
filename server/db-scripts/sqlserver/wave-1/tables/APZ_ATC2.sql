-- converted from HANA catalog export (APZ_ATC2); review before production use
IF OBJECT_ID(N'dbo.[APZ_ATC2]') IS NULL
CREATE TABLE dbo.[APZ_ATC2] (
    [ObjType] nvarchar(50) NOT NULL,
    [Path] nvarchar(255) NULL,
    CONSTRAINT [PK_APZ_ATC2] PRIMARY KEY (ObjType)
);
