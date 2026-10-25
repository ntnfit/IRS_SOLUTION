-- converted from HANA catalog export (APZ_FILTERASSIGN); review before production use
IF OBJECT_ID(N'dbo.[APZ_FILTERASSIGN]') IS NULL
CREATE TABLE dbo.[APZ_FILTERASSIGN] (
    [ObjType] nvarchar(20) NULL,
    [FilterCode] nvarchar(100) NULL,
    [UserName] nvarchar(100) NULL
);
