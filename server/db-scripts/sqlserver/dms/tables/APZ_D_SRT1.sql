-- converted from HANA catalog export (APZ_D_SRT1); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_SRT1]') IS NULL
CREATE TABLE dbo.[APZ_D_SRT1] (
    [DocEntry] int NOT NULL,
    [OrderId] int NULL,
    [FieldName] nvarchar(100) NULL,
    [DataType] nvarchar(100) NULL,
    [DisplayText] nvarchar(100) NULL,
    [ChartVisible] nvarchar(10) NULL,
    [Color] nvarchar(100) NULL,
    [IsKey] nvarchar(10) NULL
);
