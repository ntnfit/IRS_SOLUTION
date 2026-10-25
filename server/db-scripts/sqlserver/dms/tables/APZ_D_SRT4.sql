-- converted from HANA catalog export (APZ_D_SRT4); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_SRT4]') IS NULL
CREATE TABLE dbo.[APZ_D_SRT4] (
    [DocEntry] int NULL,
    [UserCode] nvarchar(100) NULL,
    [Name] nvarchar(100) NULL,
    [Value] nvarchar(max) NULL,
    [CreateDate] datetime NULL,
    [CreateTime] time NULL
);
