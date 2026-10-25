-- converted from HANA catalog export (APZ_TBLOBJECT); review before production use
IF OBJECT_ID(N'dbo.[APZ_TBLOBJECT]') IS NULL
CREATE TABLE dbo.[APZ_TBLOBJECT] (
    [ObjType] nvarchar(20) NULL,
    [TblName] nvarchar(10) NULL,
    [TblLine] nvarchar(10) NULL,
    [Remark] nvarchar(50) NULL
);
