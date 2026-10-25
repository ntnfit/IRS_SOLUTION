-- converted from HANA catalog export (APZ_UDR2); review before production use
IF OBJECT_ID(N'dbo.[APZ_UDR2]') IS NULL
CREATE TABLE dbo.[APZ_UDR2] (
    [Code] nvarchar(50) NOT NULL,
    [ColumnName] nvarchar(50) NOT NULL,
    [Value] nvarchar(254) NOT NULL,
    [Description] nvarchar(254) NOT NULL,
    [BackColor] nvarchar(20) NULL,
    [ForeColor] nvarchar(20) NULL,
    CONSTRAINT [PK_APZ_UDR2] PRIMARY KEY (Code,  ColumnName,  Value)
);
