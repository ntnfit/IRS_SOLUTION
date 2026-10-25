-- converted from HANA catalog export (APZ_D_MARKER); review before production use
IF OBJECT_ID(N'dbo.[APZ_D_MARKER]') IS NULL
CREATE TABLE dbo.[APZ_D_MARKER] (
    [Code] nvarchar(50) NULL,
    [Name] nvarchar(254) NULL,
    [SvgData] nvarchar(max) NULL,
    [DfltColorHex] nvarchar(50) NULL,
    [DfltSize] int NULL
);
