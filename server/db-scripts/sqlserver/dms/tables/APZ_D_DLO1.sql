-- converted from HANA catalog export (APZ_D_DLO1); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_DLO1]') IS NULL
CREATE TABLE dbo.[APZ_D_DLO1] (
    [DocEntry] int NULL,
    [CardCode] nvarchar(50) NULL,
    [Type] nvarchar(10) NULL,
    [ItemCode] nvarchar(50) NULL,
    [ItemName] nvarchar(254) NULL,
    [Quantity] decimal(19,6) NULL,
    [Details] nvarchar(254) NULL,
    [Remark] nvarchar(254) NULL
);
