-- converted from HANA catalog export (APZ_FILTEROBJECT); review before production use
IF OBJECT_ID(N'dbo.[APZ_FILTEROBJECT]') IS NULL
CREATE TABLE dbo.[APZ_FILTEROBJECT] (
    [ObjType] nvarchar(20) NOT NULL,
    [ObjDesc] nvarchar(254) NULL,
    [DataFilter] nvarchar(max) NULL,
    CONSTRAINT [PK_APZ_FILTEROBJECT] PRIMARY KEY (ObjType)
);
