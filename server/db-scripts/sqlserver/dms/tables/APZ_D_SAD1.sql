-- converted from HANA catalog export (APZ_D_SAD1); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_SAD1]') IS NULL
CREATE TABLE dbo.[APZ_D_SAD1] (
    [DocEntry] int NOT NULL,
    [OrderId] int NULL,
    [FieldName] nvarchar(100) NULL,
    [DataType] nvarchar(100) NULL,
    [DisplayText] nvarchar(100) NULL
);
