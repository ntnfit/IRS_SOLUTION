-- converted from HANA catalog export (APZ_D_VISIT1); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_VISIT1]') IS NULL
CREATE TABLE dbo.[APZ_D_VISIT1] (
    [DocEntry] int NOT NULL,
    [Guid] nvarchar(100) NOT NULL,
    [LineNum] int NULL,
    [StepCode] int NULL,
    [Remark] nvarchar(max) NULL,
    [CreateDate] datetime NULL,
    [CreateTime] time NULL,
    [Geolocation] nvarchar(100) NULL,
    [Status] char(1) NULL
);
