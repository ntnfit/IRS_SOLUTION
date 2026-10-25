-- converted from HANA catalog export (APZ_PCN1); review before production use
IF OBJECT_ID(N'dbo.[APZ_PCN1]') IS NULL
CREATE TABLE dbo.[APZ_PCN1] (
    [DocEntry] int NOT NULL,
    [Code] nvarchar(50) NOT NULL,
    [LineId] int NOT NULL,
    [TypeCode] nvarchar(150) NULL,
    [TypeName] nvarchar(254) NULL,
    [Rate] decimal(19,6) NULL,
    [Remark] nvarchar(max) NULL
);
